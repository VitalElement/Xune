using System;
using System.Runtime.Serialization;
using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    /// <summary>
    /// FFmpeg exception
    /// </summary>
    [Serializable]
    public unsafe class FFmpegException : Exception
    {
        public int ErrorCode { get; } = 0;

        public FFmpegException(int errorCode) : base($"{FFmpegError} [{errorCode}] {GetErrorString(errorCode)}")
        {
            ErrorCode = errorCode;
        }

        public FFmpegException(int errorCode, string message) : base($"{FFmpegError} [{errorCode}] {GetErrorString(errorCode)} {message}")
        {
            ErrorCode = errorCode;
        }

        public FFmpegException(string message) : base($"{FFmpegError} {message}")
        { }

        public FFmpegException(string message, Exception innerException) : base($"{FFmpegError} {message}", innerException)
        { }

        /// <summary>
        /// Get ffmpeg error string by error code
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static string GetErrorString(int errorCode)
        {
            byte* buffer = stackalloc byte[ffmpeg.AV_ERROR_MAX_STRING_SIZE];
            ffmpeg.av_strerror(errorCode, buffer, ffmpeg.AV_ERROR_MAX_STRING_SIZE);
            return ((IntPtr)buffer).PtrToStringUTF8();
        }

        protected FFmpegException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        { }

        #region custom error string

        public const string FFmpegError = "FFmpeg error";
        public const string NotSupportCodecId = "not supported codec id";
        public const string NotSupportSampleRate = "not supported sample rate";
        public const string NotSupportFormat = "not supported format";
        public const string NotSupportChLayout = "not supported channle layout";
        public const string NotSupportFrame = "not supported frame";
        public const string NonNegative = "argument must be non-negative";
        public const string CodecTypeError = "codec type error";
        public const string MediaTypeError = "media type error";
        public const string LineSizeError = "line size error";
        public const string PtsOutOfRange = "pts out of range";
        public const string InvalidVideoFrame = "invalid video frame";
        public const string InvalidAudioFrame = "invalid audio frame";
        public const string InvalidFrame = "invalid frame";
        public const string NotInitCodecContext = "not init codec context";
        public const string TooManyChannels = "too many channels";
        public const string FilterHasInit = "filter has init by other graph";
        public const string NeedAddToGraph = "filter need add to graph";
        public const string FilterTypeError = "filter type error";
        public const string NotSourcesFilter = "not sources filter";
        public const string NotSinksFilter = "not sinks filter";
        public static string NotImplemented { get; } = new NotImplementedException().Message; // for i18n string
        public static string NullReference { get; } = new NullReferenceException().Message; // for i18n string

        #endregion custom error string
    }
}
