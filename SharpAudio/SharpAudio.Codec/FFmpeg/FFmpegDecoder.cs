using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FFmpeg.AutoGen;
using EmguFFmpeg;


namespace SharpAudio.Codec.FFmpeg
{
    internal class FFmpegDecoder : Decoder
    {
        private const int fsStreamSize = 8192;

        private readonly int _DESIRED_CHANNEL_COUNT = 2;
        private readonly AVSampleFormat _DESIRED_SAMPLE_FORMAT = AVSampleFormat.AV_SAMPLE_FMT_S16;
        private readonly int _DESIRED_SAMPLE_RATE = 44_100;

        private readonly byte[] ffmpegFSBuf = new byte[fsStreamSize];
        private readonly int sampleByteSize;
        private readonly Stream targetStream;
        private volatile bool _isDecoderFinished;
        private bool _isFinished;
        private CircularBuffer _slidestream;
        private bool anchorNewPos;
        private avio_alloc_context_read_packet avioRead;
        private avio_alloc_context_seek avioSeek;
        private TimeSpan curPos;
        private volatile bool doSeek;
         private TimeSpan seekTimeTarget;
        private int stream_index;
        private byte[] tempSampleBuf;
        private volatile bool _isDisposed;
        private Thread _decoderThread;
        private readonly MediaReader _reader;
        private readonly IEnumerator<MediaPacket> _packetEnum;
        private readonly int _audioIndex;
        private readonly MediaStream _audioStream;
        private bool _startup;

        static FFmpegDecoder()
        {
            DoLibraryRuntimePathDetection();
        }

        private static void DoLibraryRuntimePathDetection()
        {
            // For common case native binaries located in specific for OS+Architecture folder:
            // - runtimes/
            // - - win7-x86/
            // - - - native/*.dll
            // - - osx-x86/
            // - - - native/*.dll
            // But when we pack application with MSIX or self-contained for specific architecture, it has another structure:
            // - runtime/*.dll

            // HACK TODO: This tries to look up the ffmpeg installed from homebrew. 
            // Should be removed later!
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) &&
                RuntimeInformation.OSArchitecture == Architecture.Arm64)
            {
                ffmpeg.RootPath = "/opt/homebrew/Cellar/ffmpeg/5.0/lib";
                return;
            }

            string runtimeId = null;

            // Just use the system-wide ffmpeg libraries when on linux.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                runtimeId = RuntimeInformation.OSArchitecture switch
                {
                    Architecture.X64 => "win7-x64",
                    Architecture.X86 => "win7-x86",
                    _ => runtimeId
                };
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                runtimeId = RuntimeInformation.OSArchitecture switch
                {
                    Architecture.X64 => "osx-x64",
                    Architecture.X86 => "osx-x86",
                    _ => runtimeId
                };

            var curPath = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var specificRuntimeFolder = Path.Combine(curPath, "runtimes", runtimeId, "native");
            if (Directory.Exists(specificRuntimeFolder))
            {
                ffmpeg.RootPath = specificRuntimeFolder;
            }
            else
            {
                var singleRuntimeFolder = Path.Combine(curPath, "runtime");
                if (Directory.Exists(singleRuntimeFolder))
                {
                    ffmpeg.RootPath = singleRuntimeFolder;
                }
            }
        }

        public FFmpegDecoder(Stream src)
        {
            targetStream = src;

            if (targetStream.CanSeek)
            {
                targetStream.Seek(0, SeekOrigin.Begin);
            }

            sampleByteSize = _DESIRED_SAMPLE_RATE * _DESIRED_CHANNEL_COUNT * sizeof(ushort);

            _reader = new MediaReader(targetStream);


            _packetEnum = _reader.ReadPacket().GetEnumerator();

            // audio maybe have one more stream, e.g. 0 is mp3 audio, 1 is mpeg cover
            _audioIndex = _reader.First(_ => _.Codec.AVCodecContext.codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
                .Index;

            _audioStream = _reader[_audioIndex];

            SetAudioFormat();

            tempSampleBuf = new byte[_audioFormat.SampleRate * _audioFormat.Channels * 5];
            _slidestream = new CircularBuffer(tempSampleBuf.Length);

            _decoderThread = new Thread(MainLoop);
            _decoderThread.Start();
        }

        public override bool IsFinished => _isFinished;

        public override TimeSpan Position => curPos;

        public override bool HasPosition { get; } = true;

        public override TimeSpan Duration => _audioStream.ToTimeSpan(_audioStream.Duration);

        private unsafe void SetAudioFormat()
        {
            _audioFormat.SampleRate = _DESIRED_SAMPLE_RATE;
            _audioFormat.Channels = _DESIRED_CHANNEL_COUNT;
            _audioFormat.BitsPerSample = 16;
            _numSamples = (int) (_audioStream.Duration / (float) ffmpeg.AV_TIME_BASE * _DESIRED_SAMPLE_RATE *
                                 _DESIRED_CHANNEL_COUNT);
        }

        private unsafe void MainLoop()
        {
            while (true)
            {
                if (_isDisposed)
                    break;

                Thread.Sleep(1);
                
                if (_slidestream.Length > sampleByteSize)
                    continue;

                if (!_packetEnum.MoveNext())
                {
                    _isDecoderFinished = true;
                    break;
                }

                var packet = _packetEnum.Current;
                using var audioFrame = new AudioFrame(_DESIRED_CHANNEL_COUNT, 1024, AVSampleFormat.AV_SAMPLE_FMT_S16, 44100);
                using var converter = new SampleConverter(audioFrame);
                foreach (var frame in _audioStream.ReadFrame(packet))
                {
                    converter.ConvertFrame(frame, out var x, out var y);
                     // using var fs = new FileStream("test16.raw", FileMode.Append | FileMode.OpenOrCreate);
                    // fs.Seek(fs.Length, SeekOrigin.Begin);
                    // fs.Write(interleaved);
                    var data = audioFrame.GetData()[0];
                    _slidestream.Write(data, 0, data.Length);
                }

            }
        }

        public override long GetSamples(int samples, ref byte[] data)
        {
 
            data = new byte[samples];
            var res = _slidestream.Read(data, 0, samples);

            if ((res == 0) & _isDecoderFinished)
            {
                _isFinished = true;
                return -1;
            }

            if (res > 0)
            {
                var x = res / (double) sampleByteSize;
                curPos += TimeSpan.FromSeconds(x);
                if (data.Length != res)
                    data = data[0..res];

                return res;
            }

            return 0;
        }

        public override bool TrySeek(TimeSpan time)
        {
            if (!doSeek & targetStream.CanSeek)
            {
                doSeek = true;
                seekTimeTarget = time;
                return true;
            }

            return false;
        }

        public override void Dispose()
        {
            _reader?.Dispose();

            if (targetStream.CanSeek)
                targetStream.Seek(0, SeekOrigin.Begin);

            _isDisposed = true;
            _decoderThread.Join();
        }
    }
}