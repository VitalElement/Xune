using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using FFmpeg.AutoGen;

namespace EmguFFmpeg
{

    public unsafe class MediaCodecContext : IDisposable
    {
        protected AVCodecContext* pCodecContext = null;
        private bool disposedValue;


        public static implicit operator AVCodec*(MediaCodecContext value)
        {
            if (value == null || value.pCodecContext == null) return null;
            return value.pCodecContext->codec;
        }

        public static implicit operator AVCodecContext*(MediaCodecContext value)
        {
            if (value == null) return null;
            return value.pCodecContext;
        }

        public static MediaCodecContext FromNative(AVCodecContext* pCodecContext, bool isDisposeByOwner = true)
        {
            return new MediaCodecContext(pCodecContext, !isDisposeByOwner);
        }

        public static MediaCodecContext FromNative(IntPtr pCodecContext, bool isDisposeByOwner = true)
        {
            return new MediaCodecContext((AVCodecContext*)pCodecContext, !isDisposeByOwner);
        }

        protected MediaCodecContext(AVCodecContext* codecContext, bool isDisposeByOwner = true)
        {
            pCodecContext = codecContext;
            disposedValue = !isDisposeByOwner;
        }

        public MediaCodecContext(MediaCodec codec = null)
        {
            pCodecContext = ffmpeg.avcodec_alloc_context3(codec);
            if (pCodecContext == null)
                throw new FFmpegException(FFmpegException.NullReference);
        }

        public AVCodecContext AVCodecContext => *pCodecContext;

        public AVMediaType CodecType
        {
            get => pCodecContext->codec_type;
            set => pCodecContext->codec_type = value;
        }

        public int Height
        {
            get => pCodecContext->height;
            set => pCodecContext->height = value;
        }

        public int Width
        {
            get => pCodecContext->width;
            set => pCodecContext->width = value;
        }

        public MediaRational TimeBase
        {
            get => pCodecContext->time_base;
            set => pCodecContext->time_base = value;
        }

        public AVPixelFormat PixFmt
        {
            get => pCodecContext->pix_fmt;
            set => pCodecContext->pix_fmt = value;
        }

        public long BitRate
        {
            get => pCodecContext->bit_rate;
            set => pCodecContext->bit_rate = value;
        }


        public int Refs
        {
            get => pCodecContext->refs;
            set => pCodecContext->refs = value;
        }

        public int SampleRate
        {
            get => pCodecContext->sample_rate;
            set => pCodecContext->sample_rate = value;
        }

        public ulong ChannelLayout
        {
            get => pCodecContext->channel_layout;
            set => pCodecContext->channel_layout = value;
        }

        public AVSampleFormat SampleFmt
        {
            get => pCodecContext->sample_fmt;
            set => pCodecContext->sample_fmt = value;
        }

        public int Channels
        {
            get => pCodecContext->channels;
            set => pCodecContext->channels = value;
        }

        public int Flag
        {
            get => pCodecContext->flags;
            set => pCodecContext->flags = value;
        }

        public int Profile
        {
            get => pCodecContext->profile;
            set => pCodecContext->profile = value;
        }

        public int Level
        {
            get => pCodecContext->level;
            set => pCodecContext->level = value;
        }

        /// <summary>
        /// <see cref="ffmpeg.avcodec_open2(AVCodecContext*, AVCodec*, AVDictionary**)"/>
        /// </summary>
        /// <param name="codec"></param>
        /// <param name="opts"></param>
        public void Open(MediaCodec codec, MediaDictionary opts = null)
        {
            ffmpeg.avcodec_open2(pCodecContext, codec, opts).ThrowIfError();
        }

        /// <summary>
        /// pre process frame
        /// </summary>
        /// <param name="frame"></param>
        private void RemoveSideData(MediaFrame frame)
        {
            if (frame != null)
            {
                // Make sure Closed Captions will not be duplicated
                if (AVCodecContext.codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                    ffmpeg.av_frame_remove_side_data(frame, AVFrameSideDataType.AV_FRAME_DATA_A53_CC);
            }
        }

        /// <summary>
        /// TODO: add SubtitleFrame support
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public IEnumerable<MediaPacket> EncodeFrame(MediaFrame frame)
        {
            SendFrame(frame);
            RemoveSideData(frame);
            using (MediaPacket packet = new MediaPacket())
            {
                while (true)
                {
                    int ret = ReceivePacket(packet);
                    if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                        yield break;
                    ret.ThrowIfError();
                    yield return packet;
                }
            }
        }

        public IEnumerable<MediaPacket> EncodeFrame(MediaFrame frame, MediaPacket packet)
        {
            SendFrame(frame);
            RemoveSideData(frame);
            while (true)
            {
                int ret = ReceivePacket(packet);
                if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                    yield break;
                try
                {
                    ret.ThrowIfError();
                    yield return packet;
                }
                finally { packet.Unref(); }
            }
        }

        /// <summary>
        /// decode packet to get frame.
        /// TODO: add SubtitleFrame support
        /// <para>
        /// <see cref="SendPacket(MediaPacket)"/> and <see cref="ReceiveFrame(MediaFrame)"/>
        /// </para>
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public IEnumerable<MediaFrame> DecodePacket(MediaPacket packet)
        {
            if (SendPacket(packet) >= 0)
            {
                using (MediaFrame frame = new MediaFrame())
                {
                    while (true)
                    {
                        int ret = ReceiveFrame(frame);
                        if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                            yield break;
                        ret.ThrowIfError();
                        yield return frame;
                    }
                }
            }
        }

        /// <summary>
        /// decode packet to get frame.
        /// TODO: add SubtitleFrame support
        /// <para>
        /// <see cref="SendPacket(MediaPacket)"/> and <see cref="ReceiveFrame(MediaFrame)"/>
        /// </para>
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public IEnumerable<MediaFrame> DecodePacket(MediaPacket packet, MediaFrame frame)
        {
            if (SendPacket(packet) >= 0)
            {
                while (true)
                {
                    int ret = ReceiveFrame(frame);
                    if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF)
                        yield break;
                    ret.ThrowIfError();
                    yield return frame;
                }
            }
        }

        #region safe wapper for IEnumerable

        /// <summary>
        /// <see cref="ffmpeg.avcodec_send_frame(AVCodecContext*, AVFrame*)"/>
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public int SendFrame([In] MediaFrame frame) => ffmpeg.avcodec_send_frame(pCodecContext, frame);

        /// <summary>
        /// <see cref="ffmpeg.avcodec_receive_packet(AVCodecContext*, AVPacket*)"/>
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public int ReceivePacket([Out] MediaPacket packet) => ffmpeg.avcodec_receive_packet(pCodecContext, packet);

        /// <summary>
        /// <see cref="ffmpeg.avcodec_send_packet(AVCodecContext*, AVPacket*)"/>
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public int SendPacket([In] MediaPacket packet) => ffmpeg.avcodec_send_packet(pCodecContext, packet);

        /// <summary>
        /// <see cref="ffmpeg.avcodec_receive_frame(AVCodecContext*, AVFrame*)"/>
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public int ReceiveFrame([Out] MediaFrame frame) => ffmpeg.avcodec_receive_frame(pCodecContext, frame);

        #endregion safe wapper for IEnumerable


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                fixed (AVCodecContext** ppCodecContext = &pCodecContext)
                {
                    ffmpeg.avcodec_free_context(ppCodecContext);
                }
                disposedValue = true;
            }
        }

        ~MediaCodecContext()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
