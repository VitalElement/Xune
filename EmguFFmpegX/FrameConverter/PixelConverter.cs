using System;
using System.Collections.Generic;

using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    /// <summary>
    /// <see cref="SwsContext"/> wapper
    /// </summary>
    public unsafe class PixelConverter : IFrameConverter, IDisposable
    {
        protected SwsContext* pSwsContext = null;
        public readonly AVPixelFormat DstFormat;
        public readonly int DstWidth;
        public readonly int DstHeight;
        public readonly int SwsFlag;
        public MediaFrame DstFrame { get; set; }
        private bool disposedValue;


        protected PixelConverter(SwsContext* pSws, bool isDisposeByOwner = true)
        {
            pSwsContext = pSws;
            disposedValue = !isDisposeByOwner;
        }

        /// <summary>
        /// NOTE: must set <see cref="DstFrame"/> before use!!!
        /// </summary>
        /// <param name="pSwsContext"></param>
        /// <param name="isDisposeByOwner"></param>
        /// <returns></returns>
        public static PixelConverter FromNative(IntPtr pSwsContext, bool isDisposeByOwner = true)
        {
            return new PixelConverter((SwsContext*)pSwsContext, !isDisposeByOwner);
        }

        public static PixelConverter FromNative(SwsContext* pSwsContext, bool isDisposeByOwner = true)
        {
            return new PixelConverter(pSwsContext, !isDisposeByOwner);
        }


        /// <summary>
        /// create video frame converter by dst output parames
        /// </summary>
        /// <param name="dstFormat"></param>
        /// <param name="dstWidth"></param>
        /// <param name="dstHeight"></param>
        /// <param name="flags"></param>
        public PixelConverter(AVPixelFormat dstFormat, int dstWidth, int dstHeight, int flags = ffmpeg.SWS_BILINEAR)
        {
            DstWidth = dstWidth;
            DstHeight = dstHeight;
            DstFormat = dstFormat;
            SwsFlag = flags;
            DstFrame = MediaFrame.CreateVideoFrame(dstWidth, dstHeight, dstFormat);
        }

        /// <summary>
        /// create video frame converter by dst codec
        /// </summary>
        /// <param name="dstCodec"></param>
        /// <param name="flags"></param>
        public static PixelConverter CreateByCodeContext(MediaCodecContext dstCodec, int flags = ffmpeg.SWS_BILINEAR)
        {
            if (dstCodec.AVCodecContext.codec_type != AVMediaType.AVMEDIA_TYPE_VIDEO)
                throw new FFmpegException(FFmpegException.CodecTypeError);
            return new PixelConverter(dstCodec.AVCodecContext.pix_fmt, dstCodec.AVCodecContext.width, dstCodec.AVCodecContext.height, flags);
        }

        /// <summary>
        /// create video fram converter by dst frame
        /// </summary>
        /// <param name="dstFrame"></param>
        /// <param name="flags"></param>
        public static PixelConverter CreateByDstFrame(MediaFrame dstFrame, int flags = ffmpeg.SWS_BILINEAR)
        {
            if (!dstFrame.IsVideoFrame) throw new FFmpegException(FFmpegException.InvalidVideoFrame);
            return new PixelConverter((AVPixelFormat)dstFrame.AVFrame.format, dstFrame.AVFrame.width, dstFrame.AVFrame.height, flags);
        }

        /// <summary>
        /// Convert <paramref name="srcframe"/>
        /// <para>
        /// Video conversion can be made without the use of IEnumerable,
        /// here In order to be consistent with the <see cref="SampleConverter"/> interface.
        /// </para>
        /// </summary>
        /// <param name="srcframe"></param>
        /// <returns></returns>
        public IEnumerable<MediaFrame> Convert(MediaFrame srcframe)
        {
            yield return ConvertFrame(srcframe);
        }

        public MediaFrame ConvertFrame(MediaFrame srcFrame)
        {
            AVFrame* src = srcFrame;
            AVFrame* dst = DstFrame;
            if (pSwsContext == null)
            {
                pSwsContext = ffmpeg.sws_getContext(
                    src->width, src->height, (AVPixelFormat)src->format,
                    DstWidth, DstHeight, DstFormat, SwsFlag, null, null, null);
            }
            ffmpeg.sws_scale(pSwsContext, src->data, src->linesize, 0, src->height, dst->data, dst->linesize).ThrowIfError();
            return DstFrame;
        }

        public static implicit operator SwsContext*(PixelConverter value)
        {
            if (value is null) return null;
            return value.pSwsContext;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ffmpeg.sws_freeContext(pSwsContext);
                disposedValue = true;
            }
        }

        ~PixelConverter()
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
