using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    public unsafe class MediaCodecParserContext : IDisposable
    {
        protected AVCodecParserContext* pCodecParserContext;


        public static MediaCodecParserContext FromNative(IntPtr pAVCodecParserContext, int bufferSize = 4096, bool isDisposeByOwner = true)
        {
            return new MediaCodecParserContext((AVCodecParserContext*)pAVCodecParserContext, bufferSize, !isDisposeByOwner);
        }

        public static MediaCodecParserContext FromNative(AVCodecParserContext* pAVCodecParserContext, int bufferSize = 4096, bool isDisposeByOwner = true)
        {
            return new MediaCodecParserContext(pAVCodecParserContext, bufferSize, !isDisposeByOwner);
        }

        protected MediaCodecParserContext(AVCodecParserContext* pAVCodecParserContext, int bufferSize = 4096, bool isDisposeByOwner = true)
        {
            pCodecParserContext = pAVCodecParserContext;
            disposedValue = !isDisposeByOwner;
            size = bufferSize;
            buffer = new byte[bufferSize + ffmpeg.AV_INPUT_BUFFER_PADDING_SIZE];
        }

        public MediaCodecParserContext(int codecId, int bufferSize = 4096)
        {
            pCodecParserContext = ffmpeg.av_parser_init(codecId);
            size = bufferSize;
            buffer = new byte[bufferSize + ffmpeg.AV_INPUT_BUFFER_PADDING_SIZE];
        }

        protected int size;
        protected byte[] buffer;

        /// <summary>
        /// TODO:
        /// </summary>
        /// <param name="codecContext"></param>
        /// <param name="stream"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public IEnumerator<MediaPacket> ParserPacket(MediaCodecContext codecContext, Stream stream, MediaPacket packet = null)
        {
            //int dataSize;
            //while ((dataSize = stream.Read(buffer, 0, size)) != 0)
            //{
            //    fixed (byte* inbuf = buffer)
            //    {
            //        byte* data = inbuf;
            //        while (dataSize > 0)
            //        {

            //            var ret = ffmpeg.av_parser_parse2(pCodecParserContext, codecContext, &((AVPacket*)packet)->data, &((AVPacket*)packet)->size, data, dataSize, ffmpeg.AV_NOPTS_VALUE, ffmpeg.AV_NOPTS_VALUE, 0).ThrowIfError();
            //            data += ret;
            //            dataSize -= ret;
            //            //if (packet.AVPacket.size > 0)
            //            //    yield return packet;
            //        }
            //    }
            //}
            throw new NotImplementedException();
        }


        /// <summary>
        /// TODO:
        /// </summary>
        /// <param name="codecContext"></param>
        /// <param name="stream"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public IEnumerable<MediaFrame> Parser(MediaCodecContext codecContext, Stream stream, MediaPacket packet = null)
        {
            throw new NotImplementedException();
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                ffmpeg.av_parser_close(pCodecParserContext);
                disposedValue = true;
            }
        }

        ~MediaCodecParserContext()
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
