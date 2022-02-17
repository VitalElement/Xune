using System;
using System.Collections.Generic;
using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    /// <summary>
    /// <see cref="AVOutputFormat"/> wapper
    /// </summary>
    public unsafe class OutFormat : MediaFormat
    {
        protected AVOutputFormat* pOutputFormat = null;

        public OutFormat(AVOutputFormat* oformat)
            : this((IntPtr)oformat) { }

        /// <summary>
        /// <see cref="AVOutputFormat"/> adapter.
        /// </summary>
        /// <param name="pAVOutputFormat"></param>
        public OutFormat(IntPtr pAVOutputFormat)
        {
            if (pAVOutputFormat == IntPtr.Zero) throw new FFmpegException(FFmpegException.NullReference);
            pOutputFormat = (AVOutputFormat*)pAVOutputFormat;
        }

        /// <summary>
        /// get muxer format by name,e.g. "mp4" ".mp4"
        /// </summary>
        /// <param name="name"></param>
        public static OutFormat Get(string name)
        {
            name = name.Trim().TrimStart('.');
            if (!string.IsNullOrEmpty(name))
            {
                foreach (var format in Formats)
                {
                    // e.g. format.Name == "mov,mp4,m4a,3gp,3g2,mj2"
                    string[] names = format.Name.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in names)
                    {
                        if (string.Compare(item, name, true) == 0)
                        {
                            return format;
                        }
                    }
                }
            }
            throw new FFmpegException(ffmpeg.AVERROR_MUXER_NOT_FOUND);
        }

        /// <summary>
        /// Return the output format in the list of registered output formats which best matches the
        /// provided parameters, or return NULL if there is no match.
        /// </summary>
        /// <param name="shortName">
        /// if non-NULL checks if short_name matches with the names of the registered formats
        /// </param>
        /// <param name="fileName">
        /// if non-NULL checks if filename terminates with the extensions of the registered formats
        /// </param>
        /// <param name="mimeType">
        /// if non-NULL checks if mime_type matches with the MIME type of the registered formats
        /// </param>
        /// <returns></returns>
        public static OutFormat GuessFormat(string shortName, string fileName, string mimeType)
        {
            return new OutFormat(ffmpeg.av_guess_format(shortName, fileName, mimeType));
        }

        /// <summary>
        /// get all supported output formats
        /// </summary>
        public static IEnumerable<OutFormat> Formats
        {
            get
            {
                IntPtr oformat;
                IntPtr2Ptr ofmtOpaque = IntPtr2Ptr.Null;
                while ((oformat = av_muxer_iterate_safe(ofmtOpaque)) != IntPtr.Zero)
                {
                    yield return new OutFormat(oformat);
                }
            }
        }

        #region Safe wapper for IEnumerable

        private static IntPtr av_muxer_iterate_safe(IntPtr2Ptr ptr)
        {
            return (IntPtr)ffmpeg.av_muxer_iterate(ptr);
        }

        #endregion Safe wapper for IEnumerable

        public AVOutputFormat AVOutputFormat => *pOutputFormat;

        public static implicit operator AVOutputFormat*(OutFormat value)
        {
            if (value == null) return null;
            return value.pOutputFormat;
        }

        public AVCodecID VideoCodec => pOutputFormat->video_codec;
        public AVCodecID AudioCodec => pOutputFormat->audio_codec;
        public AVCodecID DataCodec => pOutputFormat->data_codec;
        public AVCodecID SubtitleCodec => pOutputFormat->subtitle_codec;
        public override int Flags => pOutputFormat->flags;
        public override string Name => ((IntPtr)pOutputFormat->name).PtrToStringUTF8();
        public override string LongName => ((IntPtr)pOutputFormat->long_name).PtrToStringUTF8();
        public override string Extensions => ((IntPtr)pOutputFormat->extensions).PtrToStringUTF8();
        public override string MimeType => ((IntPtr)pOutputFormat->mime_type).PtrToStringUTF8();
    }
}
