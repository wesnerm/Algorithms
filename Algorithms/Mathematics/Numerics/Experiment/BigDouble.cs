namespace Algorithms.Mathematics.DP;

public struct BigDouble : IFormattable, IEquatable<BigDouble>, IComparable<BigDouble>
{
    #region Constructor

    readonly double hi;
    readonly double lo;

    public BigDouble(double d)
    {
        hi = d;
        lo = 0;
    }

    public BigDouble(double hi, double lo)
    {
        this.hi = hi + lo;
        this.lo = lo + (hi - this.hi);
    }

    public BigDouble(long d)
    {
        hi = d;
        lo = d - (long)hi;
    }

    public BigDouble(ulong d)
    {
        hi = d;
        lo = d - (ulong)hi;
    }

    #endregion

    #region Conversions

    public static explicit operator double(BigDouble d) => d.hi;

    public static explicit operator decimal(BigDouble d) => (decimal)d.hi + (decimal)d.lo;

    public static explicit operator long(BigDouble d) => (long)d.hi + (long)d.lo;

    public static explicit operator ulong(BigDouble d) => (ulong)d.hi + (ulong)d.lo;

    public static implicit operator BigDouble(double d) => new(d);

    public static explicit operator BigDouble(decimal d)
    {
        double hi = (double)d;
        return new BigDouble(hi, (double)(d - (decimal)hi));
    }

    #endregion

    #region Operations

    public static BigDouble operator +(BigDouble d1, BigDouble d2)
    {
        double hi = d1.hi + d2.hi;
        double lo = d1.lo + d2.lo;
        lo += Math.Abs(d1.hi) > Math.Abs(d2.hi)
            ? d1.hi - hi + d2.hi
            : d2.hi - hi + d1.hi;
        return new BigDouble(hi, lo);
    }

    public static BigDouble operator -(BigDouble d1, BigDouble d2)
    {
        double hi = d1.hi - d2.hi;
        double lo = d1.lo - d2.lo;
        lo += Math.Abs(d1.hi) > Math.Abs(d2.hi)
            ? d1.hi - hi - d2.hi
            : d1.hi - (hi + d2.hi);
        return new BigDouble(hi, lo);
    }

    public static BigDouble operator -(BigDouble d1) => new(-d1.hi, -d1.lo);

    public static BigDouble operator *(BigDouble d1, BigDouble d2)
    {
        double d1h = (float)d1.hi;
        double d1l = d1.hi - d1h + d1.lo;
        double d2h = (float)d2.hi;
        double d2l = d2.hi - d2h + d2.lo;
        return new BigDouble(d1h * d2h, d1l * d2h + d1h * d2l + d1l * d2l);
    }

    public static BigDouble operator /(BigDouble d1, BigDouble d2) => new(d1.hi / d2.hi, d1.lo / d2.hi);

    public static BigDouble operator %(BigDouble d1, BigDouble d2) =>
        d2.lo == 0
            ? new BigDouble(d1.hi % d2.hi, d1.lo != 0 ? d1.lo % d2.hi : 0)
            : d1 - d2 * (d1 / d2).Truncate();

    public override bool Equals(object obj) => obj is BigDouble && Equals((BigDouble)obj);

    public override int GetHashCode() => hi.GetHashCode() ^ lo.GetHashCode();

    public bool Equals(BigDouble other) => other.hi == hi && other.lo == lo;

    public int CompareTo(BigDouble other)
    {
        int cmp = hi.CompareTo(other.hi);
        if (cmp != 0) return cmp;
        return lo.CompareTo(other.lo);
    }

    public override string ToString() => ToString(string.Empty, null);

    public string ToString(string format, IFormatProvider formatProvider) =>
        hi >= (double)decimal.MaxValue || hi <= (double)decimal.MinValue
            ? hi.ToString(format, formatProvider)
            : ((decimal)hi + (decimal)lo).ToString(format, formatProvider);

    public bool TryParse(string s, out BigDouble d)
    {
        d = 0;
        bool sign = false;
        int decPos = -1;
        int exp = 0;
        int i = 0;
        for (; i < s.Length; i++) {
            char ch = s[i];
            if (ch >= '0' && ch <= '9') {
                d = d * 10 + (ch - '0');
            } else if (ch == '.' && decPos == -1) {
                decPos = i + 1;
            } else if (ch == '-' && i == 0) {
                sign = true;
            } else {
                if ((ch != 'e' && ch != 'E') || !int.TryParse(s.Substring(i), out exp))
                    return false;
                break;
            }
        }

        if (sign) d = -d;
        if (decPos > -1) exp += i - decPos;
        while (exp > 0) d *= 10;
        while (exp < 0) d /= 10;
        return true;
    }

    public BigDouble Truncate() => new(Math.Truncate(hi), Math.Truncate(lo));

    public BigDouble Floor()
    {
        double hi = Math.Floor(this.hi);
        return new BigDouble(hi, this.hi != hi ? 0 : Math.Floor(lo));
    }

    public BigDouble Ceiling()
    {
        double hi = Math.Ceiling(this.hi);
        return new BigDouble(hi, this.hi != hi ? 0 : Math.Ceiling(lo));
    }

    public int Sign() => Math.Sign(hi);

    public BigDouble Abs() => hi >= 0 ? this : -this;

    #endregion
}