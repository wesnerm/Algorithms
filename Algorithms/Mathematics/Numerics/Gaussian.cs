using static Algorithms.Mathematics.ModularMath;

namespace Algorithms.Mathematics;

public struct Gaussian
{
    #region Field

    public long Real, Imag;

    #endregion

    #region Constructors

    public Gaussian(long re, long im = 0)
    {
        Real = re;
        Imag = im;
    }

    #endregion

    #region Properties

    public Gaussian Conjugate => new(Real, -Imag);

    public bool IsZero => Imag == 0 && Real == 0;

    #endregion

    #region Operators

    public static Gaussian operator +(Gaussian arg1, Gaussian arg2) =>
        new(arg1.Real + arg2.Real, arg1.Imag + arg2.Imag);

    public static Gaussian operator -(Gaussian arg1) => new(-arg1.Real, -arg1.Imag);

    public static Gaussian operator -(Gaussian arg1, Gaussian arg2) =>
        new(arg1.Real - arg2.Real, arg1.Imag - arg2.Imag);

    public static Gaussian operator *(Gaussian arg1, Gaussian arg2) =>
        new(arg1.Real * arg2.Real - arg1.Imag * arg2.Imag,
            arg1.Real * arg2.Imag + arg2.Real * arg1.Imag);

    public Gaussian Div(Gaussian arg2, long mod)
    {
        long d = arg2.Real * arg2.Real % mod + arg2.Imag * arg2.Imag % mod;
        d = ModInverse(d, mod);
        long c1 = Real * arg2.Real % mod + Imag * arg2.Imag % mod;
        long c2 = Imag * arg2.Real % mod - Real * arg2.Imag % mod;
        return new Gaussian(c1 * d, c2 * d);
    }

    public static bool operator ==(Gaussian c1, Gaussian c2) => c1.Real == c2.Real && c1.Imag == c2.Imag;

    public static bool operator !=(Gaussian c1, Gaussian c2) => c1.Real != c2.Real || c1.Imag != c2.Imag;

    public static bool operator ==(Gaussian c1, long c2) => c1.Real == c2 && c1.Imag == 0;

    public static bool operator !=(Gaussian c1, long c2) => c1.Real != c2 || c1.Imag != 0;

    public static implicit operator Gaussian(long d) => new(d);

    #endregion

    #region Functions

    public double Abs() => Math.Sqrt(Real * Real + Imag * Imag);

    public override string ToString()
    {
        if (Imag == 0) return Real.ToString();
        if (Real == 0) return Imag + "i";
        return $"{Real}+{Imag}i";
    }

    public Gaussian ModPow(long p)
    {
        Gaussian b = this;
        Gaussian result = 1;
        while (p != 0) {
            if ((p & 1) != 0)
                result *= b;
            p >>= 1;
            b *= b;
        }

        return result;
    }

    public override bool Equals(object obj) => obj is Gaussian && (Gaussian)obj == this;

    public override int GetHashCode() => (Imag * Math.PI).GetHashCode() ^ Real.GetHashCode();

    #endregion
}