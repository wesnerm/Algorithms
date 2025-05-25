#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2003-2004, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using static Algorithms.Mathematics.ModularMath;

#endregion

namespace Algorithms.Mathematics;

public struct Algebraic
{
    #region Fields

    public readonly long X, Y, S;

    #endregion

    #region Constructors

    public Algebraic(long X, long Y, long S)
    {
        this.X = X;
        this.Y = Y;
        this.S = S;
    }

    #endregion

    #region Properties

    public Algebraic Conjugate => new(X, -Y, S);

    public bool IsZero => X == 0 && Y == 0;

    #endregion

    #region Operators

    public static Algebraic operator +(Algebraic a, Algebraic b) => new(a.X + b.X, a.Y + b.Y, a.S | b.S);

    public static Algebraic operator -(Algebraic a, Algebraic b) => new(a.X - b.X, a.Y - b.Y, a.S | b.S);

    public static Algebraic operator -(Algebraic a) => new(-a.X, -a.Y, a.S);

    public static Algebraic operator *(Algebraic a, Algebraic b) =>
        new(a.X * b.X + a.Y * b.Y * a.S,
            a.X * b.Y + a.Y * b.X, a.S | b.S);

    public static Algebraic operator *(Algebraic a, long b) => new(a.X * b, a.Y * b, a.S);

    public static Algebraic operator /(Algebraic a, long d) => new(a.X / d, a.Y / d, a.S);

    public Algebraic Mult(Algebraic b, long mod)
    {
        long s = S | b.S;
        return new Algebraic((X * b.X % mod + Y * b.Y % mod * s) % mod,
            X * b.Y % mod + Y * b.X % mod, s);
    }

    public Algebraic Div(Algebraic div, long mod)
    {
        long s = S | div.S;
        long d = (div.X * div.X % mod - div.Y * div.Y % mod * s) % mod;
        d = ModInverse(d, mod);
        long c1 = (X * div.X % mod - Y * div.Y % mod * s) % mod;
        long c2 = Y * div.X % mod - X * div.Y % mod;
        return new Algebraic(c1 * d % mod, c2 * d % mod, s);
    }

    public static Algebraic operator %(Algebraic a, long b) => new(a.X % b, a.Y % b, a.S);

    public static implicit operator Algebraic(long a) => new(a, 0, 0);

    public static bool operator ==(Algebraic c1, Algebraic c2) => c1.X == c2.X && c1.Y == c2.Y;

    public static bool operator !=(Algebraic c1, Algebraic c2) => c1.X != c2.X || c1.Y != c2.Y;

    public static bool operator ==(Algebraic c1, long c2) => c1.X == c2 && c1.Y == 0;

    public static bool operator !=(Algebraic c1, long c2) => c1.X != c2 || c1.Y != 0;

    #endregion

    #region Functions

    public Algebraic ModPow(long p, long mod = long.MaxValue)
    {
        Algebraic b = this;
        Algebraic r = 1;
        while (p != 0) {
            if ((p & 1) != 0) r = r.Mult(b, mod);
            p >>= 1;
            b = b.Mult(b, mod);
        }

        return r;
    }

    public Algebraic Pow(long p)
    {
        Algebraic b = this;
        Algebraic r = 1;
        while (p != 0) {
            if ((p & 1) != 0) r *= b;
            p >>= 1;
            b *= b;
        }

        return r;
    }

    public override string ToString()
    {
        if (Y == 0) return X.ToString();
        if (X == 0) return Y + "√";
        return $"{X}+{Y}√S";
    }

    public override bool Equals(object obj) => obj is Algebraic && (Algebraic)obj == this;

    public override int GetHashCode() => (((long)Y.GetHashCode() << 24) ^ X).GetHashCode();

    #endregion
}