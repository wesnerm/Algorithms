#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2003-2004, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Mathematics.Redundant;

public struct Complex
{
    #region Field

    readonly double Real;
    readonly double Imag;

    #endregion

    #region Constructors

    public Complex(double re, double im)
    {
        Real = re;
        Imag = im;
    }

    #endregion

    #region Properties

    public Complex Conjugate => new(Real, -Imag);

    public bool IsZero => Imag == 0 && Real == 0;

    #endregion

    #region Operators

    public static Complex operator +(Complex arg1, Complex arg2) => new(arg1.Real + arg2.Real, arg1.Imag + arg2.Imag);

    public static Complex operator -(Complex arg1) => new(-arg1.Real, -arg1.Imag);

    public static Complex operator -(Complex arg1, Complex arg2) => new(arg1.Real - arg2.Real, arg1.Imag - arg2.Imag);

    public static Complex operator *(Complex arg1, Complex arg2) =>
        new(arg1.Real * arg2.Real - arg1.Imag * arg2.Imag,
            arg1.Real * arg2.Imag + arg2.Real * arg1.Imag);

    public static Complex operator /(Complex arg1, Complex arg2)
    {
        double d = arg2.Real * arg2.Real + arg2.Imag * arg2.Imag;
        d = 1 / d;
        double c1 = arg1.Real * arg2.Real + arg1.Imag * arg2.Imag;
        double c2 = arg1.Imag * arg2.Real - arg1.Real * arg2.Imag;
        return new Complex(c1 * d, c2 * d);
    }

    public static bool operator ==(Complex c1, Complex c2) => c1.Real == c2.Real && c1.Imag == c2.Imag;

    public static bool operator !=(Complex c1, Complex c2) => c1.Real != c2.Real || c1.Imag != c2.Imag;

    public static bool operator ==(Complex c1, double c2) => c1.Real == c2 && c1.Imag == 0;

    public static bool operator !=(Complex c1, double c2) => c1.Real != c2 || c1.Imag != 0;

    public static implicit operator Complex(double d) => new(d, 0);

    #endregion

    #region Functions

    public double Abs() => Math.Sqrt(Real * Real + Imag * Imag);

    public double Arg() => Real == 0 ? 0 : 180 / Math.PI * Math.Atan(Imag / Real);

    public Complex SquareRoot() => Pow(.5);

    public Complex Exp()
    {
        double r = Math.Exp(Real);
        return new Complex(r * Math.Cos(Imag), r * Math.Sin(Imag));
    }

    public Complex Pow(double n)
    {
        double r = Math.Pow(Abs(), n);
        double t = Arg();
        return new Complex(r * Math.Cos(n * t), r * Math.Sign(n * t));
    }

    public static Complex Polar(double r, double theta) => new(r * Math.Cos(theta), r * Math.Sin(theta));

    public override string ToString()
    {
        if (Imag == 0) return Real.ToString();
        if (Real == 0) return Imag + "i";
        return $"{Real} + {Imag}i";
    }

    public override bool Equals(object obj) => obj is Complex && (Complex)obj == this;

    public override int GetHashCode() => (Imag * Math.PI).GetHashCode() ^ Real.GetHashCode();

    #endregion
}