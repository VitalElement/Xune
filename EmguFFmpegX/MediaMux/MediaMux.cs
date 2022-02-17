using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    /// <summary>
    /// <see cref="AVFormatContext"/> wapper
    /// </summary>
    public unsafe abstract class MediaMux : IDisposable,  IReadOnlyList<MediaStream>
    {
        protected AVFormatContext* pFormatContext;

        public AVFormatContext AVFormatContext => *pFormatContext;

        public string Url => ((IntPtr)pFormatContext->url).PtrToStringUTF8();


        public static implicit operator AVFormatContext*(MediaMux value)
        {
            if (value == null) return null;
            return value.pFormatContext;
        }

        #region IReadOnlyList<MediaStream>

        protected List<MediaStream> streams = new List<MediaStream>();

        /// <summary>
        /// stream count in mux.
        /// </summary>
        public int Count => (int)pFormatContext->nb_streams;

        /// <summary>
        /// get stream
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MediaStream this[int index] => new MediaStream(pFormatContext->streams[index]);

        /// <summary>
        /// enum stream
        /// </summary>
        /// <returns></returns>
        public IEnumerator<MediaStream> GetEnumerator()
        {
            return streams.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IReadOnlyList<MediaStream>

        #region IDisposable Support

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MediaMux()
        {
            Dispose(false);
        }

        protected abstract void Dispose(bool disposing);

        #endregion IDisposable Support
    }
}
