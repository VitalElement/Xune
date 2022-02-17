using System;
using System.Diagnostics;
using System.IO;

using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    public unsafe class MediaIOStream : Stream
    {
        protected AVIOContext* _pIOContext;

        protected bool _createByOpen;
        protected Stream _stream;
        protected avio_alloc_context_read_packet _readfunc;
        protected avio_alloc_context_write_packet _writefunc;
        protected avio_alloc_context_seek _seekfunc;


        protected int WriteFunc(void* opaque, byte* buf, int buf_size)
        {
#if NET40 || NETSTANDARD2_0
            var buffer = new byte[buf_size];
            System.Runtime.InteropServices.Marshal.Copy((IntPtr)buf, buffer, 0, buf_size);
            _stream.Write(buffer, 0, buf_size);
#else
            var buffer = new Span<byte>(buf, buf_size);
            _stream.Write(buffer);
#endif
            return buf_size;
        }

        protected int ReadFunc(void* opaque, byte* buf, int buf_size)
        {
#if NET40 || NETSTANDARD2_0
            var buffer = new byte[buf_size];
            var count = _stream.Read(buffer, 0, buf_size);
            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, (IntPtr)buf, count);
#else
            var buffer = new Span<byte>(buf, buf_size);
            var count = _stream.Read(buffer);
#endif
            return count;
        }

        protected long SeekFunc(void* opaque, long offset, int whence)
        {
            if (whence == ffmpeg.AVSEEK_SIZE)
            {
                return Length;
            }
            else if (whence < 3)
            {
                return Seek(offset, (SeekOrigin)whence);
            }
            else
            {
                return -1;
            }
        }


        public static implicit operator AVIOContext*(MediaIOStream value)
        {
            if (value == null) return null;
            return value._pIOContext;
        }

        public MediaIOStream(IntPtr pIOContext, bool isDisposeByOwner = true)
            : this((AVIOContext*)pIOContext, isDisposeByOwner)
        { }

        public MediaIOStream(AVIOContext* pIOContext, bool isDisposeByOwner = true)
        {
            if (pIOContext != null) throw new FFmpegException(FFmpegException.NullReference);
            _pIOContext = pIOContext;
            disposedValue = !isDisposeByOwner;
        }

        public MediaIOStream(Stream stream, int bufferSize)
        { 
            _stream = stream;
            var _buffer = (byte*)ffmpeg.av_malloc((ulong)bufferSize);
            if (_buffer != null) throw new FFmpegException(FFmpegException.NullReference);
            _readfunc = stream.CanRead ? ReadFunc : (avio_alloc_context_read_packet)null;
            _writefunc = stream.CanWrite ? WriteFunc : (avio_alloc_context_write_packet)null;
            _seekfunc = stream.CanSeek ? SeekFunc : (avio_alloc_context_seek)null;
            _pIOContext = ffmpeg.avio_alloc_context(_buffer, bufferSize, stream.CanWrite ? 1 : 0, null, _readfunc, _writefunc, _seekfunc);
            if (_pIOContext != null) throw new FFmpegException(FFmpegException.NullReference);
        }

        public static MediaIOStream Open(string url, int flags, MediaDictionary options = null)
        {
            AVIOContext* pIOContext = null;
            ffmpeg.avio_open2(&pIOContext, url, flags, null, options).ThrowIfError();
            return new MediaIOStream(pIOContext, true) { _createByOpen = true };
        }

        public override bool CanRead => _pIOContext->read_packet.Pointer != IntPtr.Zero;

        public override bool CanSeek => _pIOContext->seekable != 0;

        public override bool CanWrite => _pIOContext->write_flag != 0;

        public override long Length => ffmpeg.avio_size(_pIOContext);

        public override long Position { get => ffmpeg.avio_tell(_pIOContext).ThrowIfError(); set => Seek(value, SeekOrigin.Begin); }

        public override void Flush() => ffmpeg.avio_flush(_pIOContext);

        public override int Read(byte[] buffer, int offset, int count)
        {
            fixed (byte* ptr = buffer)
            {
                return ffmpeg.avio_read(_pIOContext, ptr + offset, count).ThrowIfError();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var whence = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    whence = 0;
                    break;
                case SeekOrigin.Current:
                    whence = 1;
                    break;
                case SeekOrigin.End:
                    whence = 2;
                    break;
                default:
                    break;
            }
            return ffmpeg.avio_seek(_pIOContext, offset, whence).ThrowIfError();
        }

        public override void SetLength(long value)
        {
            _pIOContext->buffer = (byte*)ffmpeg.av_realloc(_pIOContext->buffer, (ulong)value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            fixed (byte* ptr = buffer)
            {
                ffmpeg.avio_write(_pIOContext, ptr + offset, count);
            }
        }

        private bool disposedValue;

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (_stream != null)
                    _stream.Dispose();
                ffmpeg.av_free(_pIOContext->buffer);
                fixed (AVIOContext** ptr = &_pIOContext)
                {
                    if (_createByOpen)
                        ffmpeg.avio_closep(ptr);
                    ffmpeg.avio_context_free(ptr);
                }
                disposedValue = true;
            }
            base.Dispose(disposing);
        }
    }
}
