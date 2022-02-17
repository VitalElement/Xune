using System;

using FFmpeg.AutoGen;

namespace EmguFFmpeg
{

    public class MediaRational : IComparable<MediaRational>, IEquatable<MediaRational>
    {

        private AVRational rational;

        public static MediaRational FromFPS(int fps)
        {
            return new MediaRational(1, fps);
        }

        public int CompareTo(MediaRational other)
        {
            return ToDouble().CompareTo(other?.ToDouble());
        }

        public MediaRational() { }

        public MediaRational(int num, int den)
        {
            if (den == 0) throw new ArgumentException("Denominator cannot be zero.", nameof(den));
            rational.num = num;
            rational.den = den;
        }

        public int Num
        {
            get => rational.num;
            set => rational.num = value;
        }

        public int Den
        {
            get => rational.den;
            set => rational.den = value;
        }

        /// <summary>
        /// Convert an <see cref="AVRational"/> to a double use <see cref="ffmpeg.av_q2d(AVRational)"/>.
        /// <para>
        /// NOTE: this will lose precision !!
        /// </para>
        /// </summary>
        /// <returns></returns>
        public double ToDouble()
        {
            return ffmpeg.av_q2d(rational);
        }

        /// <summary>
        /// Invert a <see cref="AVRational"/> use <see cref="ffmpeg.av_inv_q(AVRational)"/>
        /// </summary>
        /// <returns></returns>
        public MediaRational ToInvert()
        {
            return ffmpeg.av_inv_q(rational);
        }

        public override int GetHashCode()
        {
            return Num.GetHashCode() | Den.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MediaRational rational)
                return Equals(rational);
            return false;
        }
        public bool Equals(MediaRational other)
        {
            return Num == other.Num && Den == other.Den;
        }


        public static MediaRational operator +(MediaRational a) => a;
        public static MediaRational operator -(MediaRational a) => new MediaRational(-a.Num, a.Den);

        public static MediaRational operator +(MediaRational a, MediaRational b)
            => new MediaRational(a.Num * b.Den + b.Num * a.Den, a.Den * b.Den);

        public static MediaRational operator -(MediaRational a, MediaRational b)
            => a + (-b);

        public static MediaRational operator *(MediaRational a, MediaRational b)
            => new MediaRational(a.Num * b.Num, a.Den * b.Den);

        public static MediaRational operator /(MediaRational a, MediaRational b)
        {
            int den = a.Den * b.Num;
            if (den == 0) throw new DivideByZeroException();
            return new MediaRational(a.Num * b.Den, den);
        }

        public static bool operator ==(MediaRational a, MediaRational b) => a.Equals(b);

        public static bool operator !=(MediaRational a, MediaRational b) => !a.Equals(b);

        public override string ToString() => $"{Num} / {Den}";

        public static implicit operator AVRational(MediaRational value)
        {
            return value.rational;
        }

        public static implicit operator MediaRational(AVRational value)
        {
            return new MediaRational { rational = value };
        }
    }

    public static class AVRationalEx
    {
        public static AVRational ToInvert(this AVRational rational)
        {
            return ffmpeg.av_inv_q(rational);
        }
    }

}
