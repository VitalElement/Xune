using System;
using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    public unsafe class MediaPacket : IDisposable, ICloneable
    {
        protected AVPacket* pPacket;

        public MediaPacket Get(IntPtr pAVPacket, bool isDisposeByOwner = true)
        {
            if (pAVPacket == IntPtr.Zero) throw new FFmpegException(FFmpegException.NullReference);
            return new MediaPacket((AVPacket*)pAVPacket) { disposedValue = !isDisposeByOwner };
        }

        internal MediaPacket(AVPacket* pPacket)
        {
            this.pPacket = pPacket;
        }

        public MediaPacket()
        {
            pPacket = ffmpeg.av_packet_alloc();
        }

        public AVPacket AVPacket => *pPacket;

        public long Dts
        {
            get => pPacket->dts;
            set => pPacket->dts = value;
        }

        public long Duration
        {
            get => pPacket->duration;
            set => pPacket->duration = value;
        }

        public int Flags
        {
            get => pPacket->flags;
            set => pPacket->flags = value;
        }

        public long Pos
        {
            get => pPacket->pos;
            set => pPacket->pos = value;
        }

        public long Pts
        {
            get => pPacket->pts;
            set => pPacket->pts = value;
        }

        public int Size
        {
            get => pPacket->size;
            set => pPacket->size = value;
        }

        public int StreamIndex
        {
            get => pPacket->stream_index;
            set => pPacket->stream_index = value;
        }

        /// <summary>
        /// [Unsafe]
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator AVPacket*(MediaPacket value)
        {
            if (value == null) return null;
            return value.pPacket;
        }

        /// <summary>
        /// <see cref="ffmpeg.av_packet_unref(AVPacket*)"/>
        /// </summary>
        public void Unref()
        {
            ffmpeg.av_packet_unref(pPacket);
        }

        /// <summary>
        /// Deep copy
        /// <para><see cref="ffmpeg.av_packet_ref(AVPacket*, AVPacket*)"/></para>
        /// <para><see cref="ffmpeg.av_packet_copy_props(AVPacket*, AVPacket*)"/></para>
        /// </summary>
        /// <exception cref="FFmpegException"/>
        /// <returns></returns>
        public MediaPacket Clone()
        {
            MediaPacket packet = new MediaPacket();
            AVPacket* dstpkt = packet;
            int ret;
            if ((ret = ffmpeg.av_packet_ref(dstpkt, pPacket)) < 0)
            {
                ffmpeg.av_packet_free(&dstpkt);
                throw new FFmpegException(ret);
            }
            ffmpeg.av_packet_copy_props(dstpkt, pPacket).ThrowIfError();
            return packet;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                fixed (AVPacket** ppPacket = &pPacket)
                {
                    ffmpeg.av_packet_free(ppPacket);
                }

                disposedValue = true;
            }
        }

        ~MediaPacket()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}
