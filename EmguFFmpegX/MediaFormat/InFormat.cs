using System;
using System.Collections.Generic;
using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    /// <summary>
    /// <see cref="AVInputFormat"/> wapper
    /// </summary>
    public unsafe class InFormat : MediaFormat
    {
        protected AVInputFormat* pInputFormat = null;

        public InFormat(AVInputFormat* iformat)
            : this((IntPtr)iformat) { }

        /// <summary>
        /// <see cref="AVInputFormat"/> adapter.
        /// </summary>
        /// <param name="pAVInputFormat"></param>
        public InFormat(IntPtr pAVInputFormat)
        {
            if (pAVInputFormat == IntPtr.Zero) throw new FFmpegException(FFmpegException.NullReference);
            pInputFormat = (AVInputFormat*)pAVInputFormat;
        }

        /// <summary>
        /// get demuxer format by name
        /// </summary>
        /// <param name="name">e.g. mov,mp4 ...</param>
        public static InFormat Get(string name)
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
            throw new FFmpegException(ffmpeg.AVERROR_DEMUXER_NOT_FOUND);
        }

        /// <summary>
        /// get all supported input formats.
        /// </summary>
        public static IEnumerable<InFormat> Formats
        {
            get
            {
                IntPtr iformat;
                IntPtr2Ptr ifmtOpaque = IntPtr2Ptr.Null;
                while ((iformat = av_demuxer_iterate_safe(ifmtOpaque)) != IntPtr.Zero)
                {
                    yield return new InFormat(iformat);
                }
            }
        }

        private static IntPtr av_demuxer_iterate_safe(IntPtr2Ptr opaque)
        {
            return (IntPtr)ffmpeg.av_demuxer_iterate(opaque);
        }

        public AVInputFormat AVInputFormat => *pInputFormat;

        public static implicit operator AVInputFormat*(InFormat value)
        {
            if (value == null) return null;
            return value.pInputFormat;
        }

        public int RawCodecId => pInputFormat->raw_codec_id;
        public override int Flags => pInputFormat->flags;
        public override string Name => ((IntPtr)pInputFormat->name).PtrToStringUTF8();
        public override string LongName => ((IntPtr)pInputFormat->long_name).PtrToStringUTF8();
        public override string Extensions => ((IntPtr)pInputFormat->extensions).PtrToStringUTF8();
        public override string MimeType => ((IntPtr)pInputFormat->mime_type).PtrToStringUTF8();
    }
}
