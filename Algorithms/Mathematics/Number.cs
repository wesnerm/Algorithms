#region

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
//
// Copyright (C) 2008, Wesner Moise.
// ----------------------------
// (C) Copyright Paul Moore 1999. Permission to copy, use, modify, sell and
// distribute this software is granted provided this copyright notice appears
// in all copies. This software is provided "as is" without express or
// implied warranty, and with no claim as to its suitability for any purpose.
// See http://www.boost.org/libs/rational for documentation.
//////////////////////////////////////////////////////////////////////////////

using System.Globalization;
using System.Runtime.InteropServices;

#endregion

namespace Algorithms.Mathematics;

#pragma warning disable 0660
/// <summary>
///     Summary description for Number.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
//[DebuggerStepThrough]
[Serializable]
[DebuggerDisplay("{ToString(),nq}")]
public struct Number :
    IComparable, IFormattable,
    IComparable<Number>, IEquatable<Number>
{
    #region Variables

    [FieldOffset(0)] double _real;
    [FieldOffset(0)] ulong _num;
    [FieldOffset(8)] long _den;

    #endregion

    #region Constructor

    [DebuggerStepThrough]
    public Number(double d)
        : this()
    {
        double d2;

        if (d >= 0) {
            d2 = d;
            _den = 1;
        } else {
            d2 = -d;
            _den = -1;
        }

        if (d2 <= ulong.MaxValue) {
            _num = (ulong)d2;
            if (_num == d2)
                return;
        }

        _real = d;
        _den = 0;
    }

    [DebuggerStepThrough]
    public Number(long n)
        : this()
    {
        if (n >= 0) {
            _num = (ulong)n;
            _den = 1;
        } else {
            _den = -1;
            _num = unchecked((ulong)-n);
        }
    }

    [DebuggerStepThrough]
    public Number(ulong n)
        : this()
    {
        _num = n;
        _den = 1;
    }

    [DebuggerStepThrough]
    public Number(long num, long den)
        : this()
    {
        if (den == 0 || (ulong)(den + MaxLong) >= 2 * MaxLong) {
            this = new Number(num / (double)den);
            return;
        }

        if (num <= 0) {
            num = -num;
            den = -den;
        }

        _num = (ulong)num;
        _den = den;

        if (den != 1) {
            long g = NumberTheory.Gcd(num, den);
            _num /= (ulong)g;
            _den /= g;
        }
    }

    [DebuggerStepThrough]
    public Number(ulong num, long den)
        : this()
    {
        if (den == 0 || (ulong)(den + MaxLong) >= 2 * MaxLong) {
            this = new Number((double)num / den);
            return;
        }

        _num = num;
        _den = den;

        if (den != 1) {
            if (num == 0) {
                _den = 1;
            } else {
                ulong g = NumberTheory.Gcd(num, (ulong)(den > 0 ? den : unchecked(-den)));
                if (g != 1) {
                    _num /= g;
                    _den /= (long)g;
                }
            }
        }
    }

    #endregion

    #region Properties

    public long Denominator {
        get
        {
            if (_den > 0)
                return _den;
            if (_den < 0)
                return unchecked(-_den);
            return 0;
        }
    }

    public ulong Numerator {
        get
        {
            if (_den != 0) return _num;
            throw new InvalidOperationException();
        }
    }

    public bool IsMultipleOf(ulong n) => (_den == 1 || _den == -1) && _num % n == 0;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsEven => (_den == 1 || _den == -1) && (_num & 1) == 0;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsOdd => (_den == 1 || _den == -1) && (_num & 1) != 0;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsRational => _den != 0;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsIntegral {
        get
        {
            unchecked {
                switch (_den) {
                    default:
                        return false;
                    case 1:
                    case -1:
                        return true;
                    case 0:
                        return Math.Truncate(_real) == _real;
                }
            }
        }
    }

    public bool IsInfinity => _den == 0 && double.IsInfinity(_real);

    public bool IsNaN => _den == 0 && double.IsNaN(_real);

    public static Number Zero = new(0);
    public static Number One = new(1);

    #endregion

    #region Operators

    public static Number operator -(Number n)
    {
        if (n._den == 0) n._real = -n._real;
        else if (n._num != 0) n._den = -n._den;
        return n;
    }

    public Number Ceiling()
    {
        if (_den == 0) return new Number(Math.Ceiling(_real));
        if (_den <= 1) return Truncate();
        return new Number(_num / unchecked((ulong)_den) + 1, 1);
    }

    public Number Floor()
    {
        if (_den == 0) return new Number(Math.Floor(_real));
        if (_den >= -1) return Truncate();
        return new Number(_num / unchecked((ulong)-_den) + 1, -1);
    }

    public Number Truncate()
    {
        if (_den == 0) return new Number(Math.Truncate(_real));
        if (_den > 1) return new Number(_num / (ulong)_den, 1);
        if (_den < -1) return new Number(_num / (ulong)-_den, -1);
        return this;
    }

    public static double Fraction(double d) => d - Math.Truncate(d);

    public Number Fraction()
    {
        unchecked {
            if (_den == 0) return Fraction(_real);
            if (_den == 1 || _den == -1) return Zero;
            ulong dn = _den > 0 ? (ulong)_den : (ulong)-_den;
            return new Number(_num % dn, _den);
        }
    }

    public bool ToInteger(out int result)
    {
        unchecked {
            if (_den == 1) {
                if (_num <= int.MaxValue) {
                    result = (int)_num;
                    return true;
                }
            } else if (_den == -1) {
                if (_num <= (uint)int.MaxValue + 1) {
                    result = (int)-(long)_num;
                    return true;
                }
            }

            result = 0;
            return false;
        }
    }

    public bool ToInteger(out uint result)
    {
        unchecked {
            if (_den == 1 && _num <= uint.MaxValue) {
                result = (uint)_num;
                return true;
            }

            result = 0;
            return false;
        }
    }

    public bool ToInteger(out long result)
    {
        unchecked {
            if (_den == 1) {
                result = (long)_num;
                if (result >= 0) return true;
            } else if (_den == -1) {
                result = -(long)_num;
                if (result < 0) return true;
            }

            result = 0;
            return false;
        }
    }

    public bool ToInteger(out ulong result)
    {
        unchecked {
            if (_den == 1) {
                result = _num;
                return true;
            }

            result = 0;
            return false;
        }
    }

    public bool IsZero => _num == 0;

    public bool IsOne => _num == 1 && _den == 1;

    public bool IsPositive =>
        (_den > 0 && _num != 0)
        || (_den == 0 && _real > 0);

    public bool IsNegative => _den < 0 || (_den == 0 && _real < 0);

    public bool IsNormal => _den != 0 || (_real < double.PositiveInfinity && _real > double.NegativeInfinity);

    public int Sign()
    {
        if (_den != 0) {
            if (_den < 0) return -1;
            if (_num != 0) return 1;
        } else {
            if (_real > 0) return 1;
            if (_real < 0) return -1;
        }

        return 0;
    }

    public Number Reciprocal()
    {
        unchecked {
            if (_den != 0 && _num <= long.MaxValue)
                return new Number(_den, (long)_num);
            return new Number(1.0 / Value);
        }
    }

    public Number Abs()
    {
        Number n = this;
        if (_den == 0) {
            if (_real < 0)
                n._real = -n._real;
        } else if (_den < 0) {
            n._den = -n._den;
        }

        return n;
    }

    public Number Pow(Number number)
    {
        if (number._num == 1) {
            if (number._den == 1) return this;
            if (number._den == -1) return Reciprocal();
        }

        return Rational(Math.Pow(Value, number.Value));
    }

    public static bool TryParse(string s, out Number number) =>
        TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, out number);

    public static Number Parse(string s)
    {
        Number n;
        if (!TryParse(s, out n))
            throw new FormatException();
        return n;
    }

    public static Number Parse(string s, NumberStyles style)
    {
        Number n;
        if (!TryParse(s, style, out n))
            throw new FormatException();
        return n;
    }

    public static bool TryParse(string s,
        NumberStyles style, out Number number)
    {
        int index = s.IndexOf('/');
        if (index < 0) {
            long integer;
            if (long.TryParse(s, style, null, out integer)) {
                number = integer;
                return true;
            }

            double real;
            if (double.TryParse(s, style, null, out real)) {
                number = Rational(real);
                Debug.Assert(number.Value == real);
                return true;
            }

            number = default;
            return false;
        }

        string[] array = s.Split('/');
        decimal dec;
        long n2;
        if (array.Length == 2
            && decimal.TryParse(array[0], out dec)
            && long.TryParse(array[1], out n2)
            && n2 > 0 && n2 <= MaxLong)
            unchecked {
                if (dec < 0) {
                    dec = -dec;
                    n2 = -n2;
                }

                if (dec <= ulong.MaxValue) {
                    number = new Number((ulong)dec, n2);
                    return true;
                }
            }

        number = Zero;
        return false;
    }

    #endregion

    #region Operators

    // This calculation avoids overflow, and minimises the number of expensive
    // calculations. Thanks to Nickolay Mladenov for this algorithm.
    //
    // Proof:
    // We have to compute a/b + c/d, where gcd(a,b)=1 and gcd(b,c)=1.
    // Let g = gcd(b,d), and b = b1*g, d=d1*g. Then gcd(b1,d1)=1
    //
    // The result is (a*d1 + c*b1) / (b1*d1*g).
    // Now we have to normalize this ratio.
    // Let's assume h | gcd((a*d1 + c*b1), (b1*d1*g)), and h > 1
    // If h | b1 then gcd(h,d1)=1 and hence h|(a*d1+c*b1) => h|a.
    // But since gcd(a,b1)=1 we have h=1.
    // Similarly h|d1 leads to h=1.
    // So we have that h | gcd((a*d1 + c*b1) , (b1*d1*g)) => h|g
    // Finally we have gcd((a*d1 + c*b1), (b1*d1*g)) = gcd((a*d1 + c*b1), g)
    // Which proves that instead of normalizing the result, it is better to
    // divide num and den by gcd((a*d1 + c*b1), g)

    public static Number operator ++(Number n) => n + One;

    public static Number operator --(Number n) => n - One;

    public static Number operator +(Number n1, Number n2)
    {
        if (n1._den == 0 || n2._den == 0)
            return n1.Value + n2.Value;

        AddRational(ref n1, ref n2);
        return n1;
    }

    public static Number operator -(Number n1, Number n2)
    {
        if (n1._den == 0 || n2._den == 0)
            return n1.Value - n2.Value;

        n2._den = -n2._den;
        AddRational(ref n1, ref n2);
        return n1;
    }

    static void AddRational(ref Number n1, ref Number n2)
    {
        unchecked {
            if (n2._num == 0) return;
            if (n1._num == 0) {
                n1 = n2;
                return;
            }

            if (n1._den == n2._den) {
                ulong tmp = n1._num + n2._num;
                if (tmp < n1._num)
                    n1 = ((double)n1._num + n2._num) / n1._den;
                else
                    n1 = new Number(tmp, n1._den);
                return;
            }

            if (n1._den == -n2._den) {
                n1 = n1._num > n2._num
                    ? new Number(n1._num - n2._num, n1._den)
                    : new Number(n2._num - n1._num, n2._den);
                return;
            }

            double n;
            double d = n1._den;
            double g = NumberTheory.Gcd(d, n2._den);
            if (g == 1.0) {
                n = n1._num * (double)n2._den + n2._num * d;
                d *= n2._den;
            } else {
                d /= g; // = b1 from the calculations above
                n = n1._num * (n2._den / g) + n2._num * d;
                g = NumberTheory.Gcd(n, g);
                n /= g;
                d *= n2._den / g;
            }

            if (n < 0) {
                n = -n;
                d = -d;
            }

            if (n <= ulong.MaxValue && n > 0
                                    && d <= MaxLong && d >= -MaxLong) {
                n1._num = (ulong)n;
                n1._den = (long)d;
                return;
            }

            n1 = n / d;
        }
    }

    void MultiplyRational(ulong n2, ulong d2)
    {
        if (n2 == 0 || _num == 0) {
            this = Zero;
            return;
        }

        double gcd1 = NumberTheory.Gcd(_num, d2);
        double gcd2 = NumberTheory.Gcd(n2, _den);
        double n = _num / gcd1 * (n2 / gcd2);
        double d = _den / gcd2 * (d2 / gcd1);

        if (n < 0) {
            n = -n;
            d = -d;
        }

        if (n <= ulong.MaxValue && n > 0 && d != 0
            && d <= MaxLong && d >= -MaxLong) {
            _num = (ulong)n;
            _den = (long)d;
            return;
        }

        this = n / d;
    }

    public static Number operator *(Number n1, Number n2)
    {
        if (n1._den == 0 || n2._den == 0)
            return n1.Value * n2.Value;

        if (n2._den < 0) {
            n1._den = -n1._den;
            n2._den = -n2._den;
        }

        n1.MultiplyRational(n2._num, (ulong)n2._den);
        return n1;
    }

    public static Number operator /(Number n1, Number n2)
    {
        if (n1._den == 0 || n2._den == 0 || n2._num == 0)
            return n1.Value / n2.Value;

        if (n2._den < 0) {
            n1._den = -n1._den;
            n2._den = -n2._den;
        }

        n1.MultiplyRational((ulong)n2._den, n2._num);
        return n1;
    }

    public static Number operator %(Number n1, Number n2)
    {
        if (n1._den == 0 || n2._den == 0 || n2._num == 0)
            return n1.Value % n2.Value;

        if (n2._den < 0)
            n2._den = -n2._den;

        if (n1._den == n2._den)
            return new Number(n1._num % n2._num, n1._den);

        return n1 - (n1 / n2).Truncate() * n2;
    }

    public static void QuotRem(
        Number n1, Number n2,
        out Number quot,
        out Number rem)
    {
        quot = (n1 / n2).Truncate();
        rem = n1 - quot * n2;
    }

    public static Number operator ~(Number n1) => -(n1 + 1);

    public static Number Max(Number n1, Number n2)
    {
        int cmp = n1.CompareTo(n2);
        if (cmp >= 0) return n1;
        return n2;
    }

    public static Number Min(Number n1, Number n2)
    {
        int cmp = n1.CompareTo(n2);
        if (cmp <= 0) return n1;
        return n2;
    }

    // Against other numbers

    public static bool operator >(Number n1, Number n2) =>
        (n1._den == 0 ? n1._real : n1._num / (double)n1._den)
        > (n2._den == 0 ? n2._real : n2._num / (double)n2._den);

    public static bool operator >=(Number n1, Number n2) =>
        (n1._den == 0 ? n1._real : n1._num / (double)n1._den)
        >= (n2._den == 0 ? n2._real : n2._num / (double)n2._den);

    public static bool operator <(Number n1, Number n2) =>
        (n1._den == 0 ? n1._real : n1._num / (double)n1._den)
        < (n2._den == 0 ? n2._real : n2._num / (double)n2._den);

    public static bool operator <=(Number n1, Number n2) =>
        (n1._den == 0 ? n1._real : n1._num / (double)n1._den)
        <= (n2._den == 0 ? n2._real : n2._num / (double)n2._den);

    public static bool operator ==(Number n1, Number n2) =>
        (n1._den == 0 ? n1._real : n1._num / (double)n1._den)
        == (n2._den == 0 ? n2._real : n2._num / (double)n2._den);

    public static bool operator !=(Number n1, Number n2) =>
        (n1._den == 0 ? n1._real : n1._num / (double)n1._den)
        != (n2._den == 0 ? n2._real : n2._num / (double)n2._den);

    int IComparable.CompareTo(object obj) => CompareTo((Number)obj);

    public int CompareTo(Number n)
    {
        int cmp = Value.CompareTo(n.Value);
        if (cmp != 0) return cmp;
        if (_den == n._den) return 0;
        if (_den < n._den) return -1;
        return 1;
    }

    #endregion

    #region Boost

    public static Number GCD(Number n1, Number n2)
    {
        unchecked {
            if (n1._den < 0) n1._den = -n1._den;
            if (n2._den < 0) n2._den = -n2._den;

            if (n1._den == n2._den) {
                if (n1._num == n2._num) return n2;
                if (n1._den != 0)
                    return new Number(NumberTheory.Gcd(n1._num, n2._num), n1._den);
            }

            if (n1._den == 0 || n2._den == 0) return Zero;
            double g = NumberTheory.Gcd((double)n1._den, n2._den);
            double lcm = n1._den / g * n2._den;
            if (lcm <= MaxLong) {
                n1._num = NumberTheory.Gcd(n1._num, n2._num);
                n1._den = (long)lcm;
                return n1;
            }

            return Zero;
        }
    }

    public Number Rationalize()
    {
        if (_den != 0) return this;
        return Rational(_real);
    }

    public static Number Rational(double r)
    {
        ulong n;
        long d;
        double d1;

        if (r < 0) {
            d1 = -r;
            d = -1;
        } else {
            d1 = r;
            d = 1;
        }

        if (!(r <= ulong.MaxValue))
            goto Fail;

        n = (ulong)d1;
        if (d1 == n)
            new Number(n, d);

        if (!(r <= MaxLong))
            goto Fail;

        double d2 = 1d;
        const double epsilon = 1e-10;

        int i = 0;
        while (true) {
            if (d1 <= epsilon) {
                d1 = d2;
                break;
            }

            d2 %= d1;
            if (d2 <= epsilon) break;
            d1 %= d2;
            if (i++ >= 8) goto Fail;
        }

        d1 = 1 / d1;
        if (!(d1 < MaxLong)) goto Fail;
        d *= (long)(d1 + .001);

        double nd = r * d + .001;
        if (!(nd < MaxLong)) goto Fail;

        var result = new Number((ulong)nd, d);
        if (result.Value == r)
            return result;

    Fail:
        return new Number(r);
    }

    #endregion

    #region IComparable<Number> Members

    public const double GoldenRatio = 1.618033988749895;

    [DebuggerStepThrough]
    public override int GetHashCode()
    {
        if (_den != 0 && Value != this)
            return _num.GetHashCode()
                   ^ _den.GetHashCode()
                   ^ 0x5EA14EA1;

        return (Value * GoldenRatio).GetHashCode() ^ 0x5EA14EA1;
    }

    public bool Equals(Number n2) =>
        (_den == 0 ? _real : _num / (double)_den)
        == (n2._den == 0 ? n2._real : n2._num / (double)n2._den);

    public override bool Equals(object number) => number is Number && Equals((Number)number);

    public override string ToString()
    {
        unchecked {
            if (_den == 0) return Value.ToString("G8");

            string prefix = null;
            string suffix = null;

            ulong d = (ulong)_den;
            if (_den < 0) {
                prefix = "-";
                d = (ulong)-_den;
            }

            if (d != 1)
                suffix = "/" + d;

            return prefix + _num + suffix;
        }
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (string.IsNullOrEmpty(format))
            return ToString();
        return Value.ToString(format, formatProvider);
    }

    #endregion

    #region Conversions

    public const long MaxLong = 9007199254740992;

    public double Value {
        get
        {
            unchecked {
                if (_den == 0) return _real;
                if ((ulong)(_den + 1) > 2) return (double)_num / _den;
                if (_den > 0) return _num;
                return -(double)_num;
            }
        }
    }

    [DebuggerStepThrough]
    public static implicit operator Number(double d) => new(d);

    [DebuggerStepThrough]
    public static implicit operator Number(int d) => new(d);

    [DebuggerStepThrough]
    public static implicit operator Number(long d) => new(d);

    [DebuggerStepThrough]
    public static implicit operator Number(ulong d) => new(d);

    [DebuggerStepThrough]
    public static implicit operator Number(decimal m)
    {
        if (decimal.Truncate(m) == m && m <= ulong.MaxValue) {
            if (m >= 0)
                return new Number((ulong)m);
            if (m >= long.MinValue)
                return new Number((long)m);
        }

        return new Number((double)m);
    }

    public bool Match(ulong num, long den) => _num == num && _den == den;

    public bool Match(long num)
    {
        if (num < 0)
            return _den == -1 && (long)_num == -num;

        return _den == 1 && (long)_num == num;
    }

    public bool Match(ulong num) => _den == 1 && _num == num;

    public bool Match(double d) => _den == 0 && _real.Equals(d);

    #endregion
}