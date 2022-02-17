using System;
using System.ComponentModel;
using FFmpeg.AutoGen;

namespace EmguFFmpeg
{
    /// <summary>
    /// <see cref="AVAudioFifo"/> wapper
    /// </summary>
    public unsafe class AudioFifo : IDisposable
    {
        protected AVAudioFifo* pAudioFifo;

        protected AudioFifo() { }
        public static AudioFifo FromNative(IntPtr pAVAudioFifo, bool isDisposeByOwner = true)
        {
            return new AudioFifo() { pAudioFifo = (AVAudioFifo*)pAVAudioFifo, disposedValue = !isDisposeByOwner };
        }

        /// <summary>
        /// alloc <see cref="AVAudioFifo"/>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="channels"></param>
        /// <param name="nbSamples"></param>
        public AudioFifo(AVSampleFormat format, int channels, int nbSamples = 1)
        {
            pAudioFifo = ffmpeg.av_audio_fifo_alloc(format, channels, nbSamples <= 0 ? 1 : nbSamples);
        }

        /// <summary>
        /// Get the current number of samples in the AVAudioFifo available for reading.
        /// </summary>
        public int Size => ffmpeg.av_audio_fifo_size(pAudioFifo);

        /// <summary>
        /// Get the current number of samples in the AVAudioFifo available for writing.
        /// </summary>
        public int Space => ffmpeg.av_audio_fifo_space(pAudioFifo);

        /// <summary>
        ///  Peek data from an AVAudioFifo.
        /// </summary>
        /// <param name="data"> audio data plane pointers</param>
        /// <param name="nbSamples">number of samples to peek</param>
        /// <returns>
        /// number of samples actually peek, or negative AVERROR code on failure. The number
        /// of samples actually peek will not be greater than nb_samples, and will only be
        /// less than nb_samples if av_audio_fifo_size is less than nb_samples.
        /// </returns>
        public int Peek(void** data, int nbSamples)
        {
            return ffmpeg.av_audio_fifo_peek(pAudioFifo, data, nbSamples).ThrowIfError();
        }

        /// <summary>
        /// Peek data from an AVAudioFifo.
        /// </summary>
        /// <param name="data">audio data plane pointers</param>
        /// <param name="nbSamples">number of samples to peek</param>
        /// <param name="Offset">offset from current read position</param>
        /// <returns>
        /// number of samples actually peek, or negative AVERROR code on failure. The number
        /// of samples actually peek will not be greater than nb_samples, and will only be
        /// less than nb_samples if av_audio_fifo_size is less than nb_samples.
        /// </returns>
        public int PeekAt(void** data, int nbSamples, int Offset)
        {
            return ffmpeg.av_audio_fifo_peek_at(pAudioFifo, data, nbSamples, Offset).ThrowIfError();
        }

        /// <summary>
        /// auto realloc if space less than nbSamples
        /// </summary>
        /// <param name="data"></param>
        /// <param name="nbSamples"></param>
        /// <exception cref="FFmpegException"/>
        public int Add(void** data, int nbSamples)
        {
            if (Space < nbSamples)
            {
                int ret;
                if ((ret = ffmpeg.av_audio_fifo_realloc(pAudioFifo, Size + nbSamples).ThrowIfError()) < 0)
                    return ret;
            }
            return ffmpeg.av_audio_fifo_write(pAudioFifo, data, nbSamples).ThrowIfError();
        }

        /// <summary>
        /// Read data from an AVAudioFifo.
        /// </summary>
        /// <param name="data">audio data plane pointers</param>
        /// <param name="nbSamples">number of samples to read</param>
        /// <exception cref="FFmpegException"/>
        /// <returns>
        /// number of samples actually read, or negative AVERROR code on failure. The number
        /// of samples actually read will not be greater than nb_samples, and will only be
        /// less than nb_samples if av_audio_fifo_size is less than nb_samples.
        /// </returns>
        public int Read(void** data, int nbSamples)
        {
            return ffmpeg.av_audio_fifo_read(pAudioFifo, data, nbSamples).ThrowIfError();
        }

        /// <summary>
        /// Drain data from an <see cref="AVAudioFifo"/>.
        /// </summary>
        /// <param name="nbSamples">number of samples to drain</param>
        /// <returns>0 if OK, or negative AVERROR code on failure</returns>
        /// <exception cref="FFmpegException"/>
        public int Drain(int nbSamples)
        {
            return ffmpeg.av_audio_fifo_drain(pAudioFifo, nbSamples).ThrowIfError();
        }

        /// <summary>
        /// Clear tha <see cref="AVFifoBuffer"/> buffer
        /// </summary>
        public void Reset()
        {
            ffmpeg.av_audio_fifo_reset(pAudioFifo);
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ffmpeg.av_audio_fifo_free(pAudioFifo);
                disposedValue = true;
            }
        }

        ~AudioFifo()
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
