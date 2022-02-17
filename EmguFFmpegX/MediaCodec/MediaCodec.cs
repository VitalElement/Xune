using System;
using System.Collections.Generic;

using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    public unsafe class MediaCodec
    {
        /// <summary>
        /// Get <see cref="MediaCodec"/> from <see cref="AVCodec"/>*
        /// </summary>
        /// <param name="pCodec"><see cref="AVCodec"/>*</param>
        /// <returns></returns>
        public static MediaCodec FromNative(AVCodec* pCodec)
        {
            if (pCodec == null) throw new FFmpegException(FFmpegException.NullReference);
            return new MediaCodec(pCodec);
        }

        /// <summary>
        /// Get <see cref="MediaCodec"/> from <see cref="AVCodec"/>*
        /// </summary>
        /// <param name="pCodec"><see cref="AVCodec"/>*</param>
        /// <returns></returns>
        public static MediaCodec FromNative(IntPtr pCodec)
        {
            return FromNative((AVCodec*)pCodec);
        }

        /// <summary>
        /// Get <see cref="MediaCodec"/> by <see cref="ffmpeg.avcodec_find_encoder_by_name(string)"/>
        /// </summary>
        /// <param name="codecName"></param>
        /// <returns></returns>
        public static MediaCodec GetEncoder(string codecName)
        {
            AVCodec* pCodec;
            if ((pCodec = ffmpeg.avcodec_find_encoder_by_name(codecName)) == null)
                throw new FFmpegException(ffmpeg.AVERROR_ENCODER_NOT_FOUND);
            return new MediaCodec(pCodec);
        }

        /// <summary>
        /// Get <see cref="MediaCodec"/> by <see cref="ffmpeg.avcodec_find_encoder(AVCodecID)"/>
        /// </summary>
        /// <param name="codecId"></param>
        /// <returns></returns>

        public static MediaCodec GetEncoder(AVCodecID codecId)
        {
            AVCodec* pCodec;
            if ((pCodec = ffmpeg.avcodec_find_encoder(codecId)) == null && codecId != AVCodecID.AV_CODEC_ID_NONE)
                throw new FFmpegException(ffmpeg.AVERROR_ENCODER_NOT_FOUND);
            return new MediaCodec(pCodec);
        }

        /// <summary>
        /// Get <see cref="MediaCodec"/> by <see cref="ffmpeg.avcodec_find_decoder_by_name(string)"/>
        /// </summary>
        /// <param name="codecName"></param>
        /// <returns></returns>
        public static MediaCodec GetDecoder(string codecName)
        {
            AVCodec* pCodec;
            if ((pCodec = ffmpeg.avcodec_find_decoder_by_name(codecName)) == null)
                throw new FFmpegException(ffmpeg.AVERROR_ENCODER_NOT_FOUND);
            return new MediaCodec(pCodec);
        }

        /// <summary>
        /// Get <see cref="MediaCodec"/> by <see cref="ffmpeg.avcodec_find_decoder(AVCodecID)"/>
        /// </summary>
        /// <param name="codecId"></param>
        /// <returns></returns>

        public static MediaCodec GetDecoder(AVCodecID codecId)
        {
            AVCodec* pCodec;
            if ((pCodec = ffmpeg.avcodec_find_decoder(codecId)) == null && codecId != AVCodecID.AV_CODEC_ID_NONE)
                throw new FFmpegException(ffmpeg.AVERROR_ENCODER_NOT_FOUND);
            return new MediaCodec(pCodec);
        }

        internal MediaCodec(AVCodec* codec)
        {
            pCodec = codec;
        }

        protected AVCodec* pCodec = null;

        public AVCodec AVCodec => *pCodec;
        public AVMediaType Type => pCodec->type;
        public AVCodecID Id => pCodec->id;
        public string Name => ((IntPtr)pCodec->name).PtrToStringUTF8();
        public string LongName => ((IntPtr)pCodec->long_name).PtrToStringUTF8();
        public string WrapperName => ((IntPtr)pCodec->wrapper_name).PtrToStringUTF8();
        public bool IsDecoder => ffmpeg.av_codec_is_decoder(pCodec) > 0;
        public bool IsEncoder => ffmpeg.av_codec_is_encoder(pCodec) > 0;
        public int Capabilities => pCodec->capabilities;

        #region safe wapper for IEnumerable

        protected static IntPtr av_codec_iterate_safe(IntPtr2Ptr opaque)
        {
            return (IntPtr)ffmpeg.av_codec_iterate(opaque);
        }

        /// <summary>
        /// Get all supported codec
        /// </summary>
        public static IEnumerable<MediaCodec> Codecs
        {
            get
            {
                IntPtr pCodec;
                IntPtr2Ptr opaque = IntPtr2Ptr.Null;
                while ((pCodec = av_codec_iterate_safe(opaque)) != IntPtr.Zero)
                {
                    yield return FromNative(pCodec);
                }
            }
        }

        #endregion safe wapper for IEnumerable

        #region Supported

        protected static KeyValuePair<int, string>? av_get_profile_name_safe(MediaCodec codec, int i)
        {
            var ptr = codec.pCodec->profiles + i;
            return ptr != null ?
                new KeyValuePair<int, string>(ptr->profile, ((IntPtr)ptr->name).PtrToStringUTF8()) :
                (KeyValuePair<int, string>?)null;
        }

        public IEnumerable<KeyValuePair<int, string>> Profiles
        {
            get
            {
                KeyValuePair<int, string>? profile;
                for (int i = 0; (profile = av_get_profile_name_safe(this, i)) != null; i++)
                {
                    yield return profile.Value;
                }
            }
        }

        protected static AVCodecHWConfig? avcodec_get_hw_config_safe(MediaCodec codec, int i)
        {
            var ptr = ffmpeg.avcodec_get_hw_config(codec, i);
            return ptr != null ? *ptr : (AVCodecHWConfig?)null;
        }

        public IEnumerable<AVCodecHWConfig> SupportedHardware
        {
            get
            {
                AVCodecHWConfig? config;
                for (int i = 0; (config = avcodec_get_hw_config_safe(this, i)) != null; i++)
                {
                    yield return config.Value;
                }
            }
        }

        protected static AVPixelFormat? pix_fmts_next_safe(MediaCodec codec, int i)
        {
            var ptr = codec.pCodec->pix_fmts + i;
            return ptr != null ? *ptr : (AVPixelFormat?)null;
        }

        public IEnumerable<AVPixelFormat> SupportedPixelFmts
        {
            get
            {
                AVPixelFormat? p;
                for (int i = 0; (p = pix_fmts_next_safe(this, i)) != null; i++)
                {
                    if (p == AVPixelFormat.AV_PIX_FMT_NONE)
                        yield break;
                    else
                        yield return p.Value;
                }
            }
        }

        protected AVRational? supported_framerates_next_safe(MediaCodec codec, int i)
        {
            var ptr = codec.pCodec->supported_framerates + i;
            return ptr != null ? *ptr : (AVRational?)null;
        }

        public IEnumerable<AVRational> SupportedFrameRates
        {
            get
            {
                AVRational? p;
                for (int i = 0; (p = supported_framerates_next_safe(this, i)) != null; i++)
                {
                    if (p.Value.num != 0)
                        yield return p.Value;
                    else
                        yield break;
                }
            }
        }

        protected AVSampleFormat? sample_fmts_next_safe(MediaCodec codec, int i)
        {
            var ptr = codec.pCodec->sample_fmts + i;
            return ptr != null ? *ptr : (AVSampleFormat?)null;
        }

        public IEnumerable<AVSampleFormat> SupportedSampelFmts
        {
            get
            {
                AVSampleFormat? p;
                for (int i = 0; (p = sample_fmts_next_safe(this, i)) != null; i++)
                {
                    if (p == AVSampleFormat.AV_SAMPLE_FMT_NONE)
                        yield break;
                    else
                        yield return p.Value;
                }
            }
        }

        protected int? supported_samplerates_next_safe(MediaCodec codec, int i)
        {
            var ptr = codec.pCodec->supported_samplerates + i;
            return ptr != null ? *ptr : (int?)null;
        }

        public IEnumerable<int> SupportedSampleRates
        {
            get
            {
                int? p;
                for (int i = 0; (p = supported_samplerates_next_safe(this, i)) != null; i++)
                {
                    if (p == 0)
                        yield break;
                    else
                        yield return p.Value;
                }
            }
        }

        protected ulong? channel_layouts_next_safe(MediaCodec codec, int i)
        {
            var ptr = codec.pCodec->channel_layouts + i;
            return ptr != null ? *ptr : (ulong?)null;
        }

        public IEnumerable<ulong> SupportedChannelLayout
        {
            get
            {
                ulong? p;
                for (int i = 0; (p = channel_layouts_next_safe(this, i)) != null; i++)
                {
                    if (p == 0)
                        yield break;
                    else
                        yield return p.Value;
                }
            }
        }

        #endregion Supported

        public override string ToString()
        {
            return $"[{Name}]{LongName}";
        }

        public static implicit operator AVCodec*(MediaCodec value)
        {
            if (value == null) return null;
            return value.pCodec;
        }
    }
}
