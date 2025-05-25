﻿// Microsoft Public License

//   Copyright (c) Microsoft Corporation.  All rights reserved.
/*============================================================
** Class: BigRational
**
** Purpose:
** --------
** This class is used to represent an arbitrary precision
** BigRational number
**
** A rational number (commonly called a fraction) is a ratio
** between two integers.  For example (3/6) = (2/4) = (1/2)
**
** Arithmetic
** ----------
** a/b = c/d, iff ad = bc
** a/b + c/d  == (ad + bc)/bd
** a/b - c/d  == (ad - bc)/bd
** a/b % c/d  == (ad % bc)/bd
** a/b * c/d  == (ac)/(bd)
** a/b / c/d  == (ad)/(bc)
** -(a/b)     == (-a)/b
** (a/b)^(-1) == b/a, if a != 0
**
** Reduction Algorithm
** ------------------------
** Euclid's algorithm is used to simplify the fraction.
** Calculating the greatest common divisor of two n-digit
** numbers can be found in
**
** O(n(log n)^5 (log log n)) steps as n -> +infinity
============================================================*/

using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Algorithms.Mathematics;

[Serializable]
[ComVisible(false)]
public struct BigRational : IComparable, IComparable<BigRational>, IDeserializationCallback, IEquatable<BigRational>,
    ISerializable
{
    // ---- SECTION:  members supporting exposed properties -------------*
    BigInteger m_numerator;
    BigInteger m_denominator;

    // ---- SECTION:  members for internal support ---------*

    #region Members for Internal Support

    [StructLayout(LayoutKind.Explicit)]
    internal struct DoubleUlong
    {
        [FieldOffset(0)] public double dbl;
        [FieldOffset(0)] public ulong uu;
    }

    const int DoubleMaxScale = 308;
    static readonly BigInteger s_bnDoublePrecision = BigInteger.Pow(10, DoubleMaxScale);
    static readonly BigInteger s_bnDoubleMaxValue = (BigInteger)double.MaxValue;
    static readonly BigInteger s_bnDoubleMinValue = (BigInteger)double.MinValue;

    [StructLayout(LayoutKind.Explicit)]
    internal struct DecimalUInt32
    {
        [FieldOffset(0)] public Decimal dec;
        [FieldOffset(0)] public int flags;
    }

    const int DecimalScaleMask = 0x00FF0000;
    const int DecimalSignMask = unchecked((int)0x80000000);
    const int DecimalMaxScale = 28;
    static readonly BigInteger s_bnDecimalPrecision = BigInteger.Pow(10, DecimalMaxScale);
    static readonly BigInteger s_bnDecimalMaxValue = (BigInteger)decimal.MaxValue;
    static readonly BigInteger s_bnDecimalMinValue = (BigInteger)decimal.MinValue;

    const string c_solidus = @"/";

    #endregion Members for Internal Support

    // ---- SECTION: public properties --------------*

    #region Public Properties

    public static BigRational Zero { get; } = new(BigInteger.Zero);

    public static BigRational One { get; } = new(BigInteger.One);

    public static BigRational MinusOne { get; } = new(BigInteger.MinusOne);

    public int Sign => m_numerator.Sign;

    public BigInteger Numerator => m_numerator;

    public BigInteger Denominator => m_denominator;

    #endregion Public Properties

    // ---- SECTION: public instance methods --------------*

    #region Public Instance Methods

    // GetWholePart() and GetFractionPart()
    // 
    // BigRational == Whole, Fraction
    //  0/2        ==     0,  0/2
    //  1/2        ==     0,  1/2
    // -1/2        ==     0, -1/2
    //  1/1        ==     1,  0/1
    // -1/1        ==    -1,  0/1
    // -3/2        ==    -1, -1/2
    //  3/2        ==     1,  1/2
    public BigInteger GetWholePart() => BigInteger.Divide(m_numerator, m_denominator);

    public BigRational GetFractionPart() => new(BigInteger.Remainder(m_numerator, m_denominator), m_denominator);

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (!(obj is BigRational))
            return false;
        return Equals((BigRational)obj);
    }

    public override int GetHashCode() => (m_numerator / Denominator).GetHashCode();

    // IComparable
    int IComparable.CompareTo(object obj)
    {
        if (obj == null)
            return 1;
        if (!(obj is BigRational))
            throw new ArgumentException("Argument must be of type BigRational", "obj");
        return Compare(this, (BigRational)obj);
    }

    // IComparable<BigRational>
    public int CompareTo(BigRational other) => Compare(this, other);

    // Object.ToString
    public override string ToString()
    {
        var ret = new StringBuilder();
        ret.Append(m_numerator.ToString("R", CultureInfo.InvariantCulture));
        ret.Append(c_solidus);
        ret.Append(Denominator.ToString("R", CultureInfo.InvariantCulture));
        return ret.ToString();
    }

    // IEquatable<BigRational>
    // a/b = c/d, iff ad = bc
    public bool Equals(BigRational other)
    {
        if (Denominator == other.Denominator) return m_numerator == other.m_numerator;

        return m_numerator * other.Denominator == Denominator * other.m_numerator;
    }

    #endregion Public Instance Methods

    // -------- SECTION: constructors -----------------*

    #region Constructors

    public BigRational(BigInteger numerator)
    {
        m_numerator = numerator;
        m_denominator = BigInteger.One;
    }

    // BigRational(Double)
    public BigRational(double value)
    {
        if (double.IsNaN(value)) throw new ArgumentException("Argument is not a number", "value");

        if (double.IsInfinity(value)) throw new ArgumentException("Argument is infinity", "value");

        SplitDoubleIntoParts(value, out int sign, out int exponent, out ulong significand, out bool isFinite);

        if (significand == 0) {
            this = Zero;
            return;
        }

        m_numerator = significand;
        m_denominator = 1 << 52;

        if (exponent > 0)
            m_numerator = BigInteger.Pow(m_numerator, exponent);
        else if (exponent < 0) m_denominator = BigInteger.Pow(m_denominator, -exponent);
        if (sign < 0) m_numerator = BigInteger.Negate(m_numerator);
        Simplify();
    }

    // BigRational(Decimal) -
    //
    // The Decimal type represents floating point numbers exactly, with no rounding error.
    // Values such as "0.1" in Decimal are actually representable, and convert cleanly
    // to BigRational as "11/10"
    public BigRational(decimal value)
    {
        int[]? bits = decimal.GetBits(value);
        if (bits == null || bits.Length != 4 || (bits[3] & ~(DecimalSignMask | DecimalScaleMask)) != 0 ||
            (bits[3] & DecimalScaleMask) > 28 << 16) throw new ArgumentException("invalid Decimal", "value");

        if (value == decimal.Zero) {
            this = Zero;
            return;
        }

        // build up the numerator
        ulong ul = ((ulong)(uint)bits[2] << 32) | (uint)bits[1]; // (hi    << 32) | (mid)
        m_numerator = (new BigInteger(ul) << 32) | (uint)bits[0]; // (hiMid << 32) | (low)

        bool isNegative = (bits[3] & DecimalSignMask) != 0;
        if (isNegative) m_numerator = BigInteger.Negate(m_numerator);

        // build up the denominator
        int scale = (bits[3] & DecimalScaleMask) >> 16; // 0-28, power of 10 to divide numerator by
        m_denominator = BigInteger.Pow(10, scale);

        Simplify();
    }

    public BigRational(BigInteger numerator, BigInteger denominator)
    {
        if (denominator.Sign == 0) throw new DivideByZeroException();

        if (numerator.Sign == 0) {
            // 0/m -> 0/1
            m_numerator = BigInteger.Zero;
            m_denominator = BigInteger.One;
        } else if (denominator.Sign < 0) {
            m_numerator = BigInteger.Negate(numerator);
            m_denominator = BigInteger.Negate(denominator);
        } else {
            m_numerator = numerator;
            m_denominator = denominator;
        }

        Simplify();
    }

    public BigRational(BigInteger whole, BigInteger numerator, BigInteger denominator)
    {
        if (denominator.Sign == 0) throw new DivideByZeroException();

        if (numerator.Sign == 0 && whole.Sign == 0) {
            m_numerator = BigInteger.Zero;
            m_denominator = BigInteger.One;
        } else if (denominator.Sign < 0) {
            m_denominator = BigInteger.Negate(denominator);
            m_numerator = BigInteger.Negate(whole) * m_denominator + BigInteger.Negate(numerator);
        } else {
            m_denominator = denominator;
            m_numerator = whole * denominator + numerator;
        }

        Simplify();
    }

    #endregion Constructors

    // -------- SECTION: public static methods -----------------*

    #region Public Static Methods

    public static BigRational Abs(BigRational r) =>
        r.m_numerator.Sign < 0 ? new BigRational(BigInteger.Abs(r.m_numerator), r.Denominator) : r;

    public static BigRational Negate(BigRational r) => new(BigInteger.Negate(r.m_numerator), r.Denominator);

    public static BigRational Invert(BigRational r) => new(r.Denominator, r.m_numerator);

    public static BigRational Add(BigRational x, BigRational y) => x + y;

    public static BigRational Subtract(BigRational x, BigRational y) => x - y;

    public static BigRational Multiply(BigRational x, BigRational y) => x * y;

    public static BigRational Divide(BigRational dividend, BigRational divisor) => dividend / divisor;

    public static BigRational Remainder(BigRational dividend, BigRational divisor) => dividend % divisor;

    public static BigRational DivRem(BigRational dividend, BigRational divisor, out BigRational remainder)
    {
        // a/b / c/d  == (ad)/(bc)
        // a/b % c/d  == (ad % bc)/bd

        // (ad) and (bc) need to be calculated for both the division and the remainder operations.
        BigInteger ad = dividend.m_numerator * divisor.Denominator;
        BigInteger bc = dividend.Denominator * divisor.m_numerator;
        BigInteger bd = dividend.Denominator * divisor.Denominator;

        remainder = new BigRational(ad % bc, bd);
        return new BigRational(ad, bc);
    }

    public static BigRational Pow(BigRational baseValue, BigInteger exponent)
    {
        if (exponent.Sign == 0)
            // 0^0 -> 1
            // n^0 -> 1
            return One;

        if (exponent.Sign < 0) {
            if (baseValue == Zero) throw new ArgumentException("cannot raise zero to a negative power", "baseValue");
            // n^(-e) -> (1/n)^e
            baseValue = Invert(baseValue);
            exponent = BigInteger.Negate(exponent);
        }

        BigRational result = baseValue;
        while (exponent > BigInteger.One) {
            result = result * baseValue;
            exponent--;
        }

        return result;
    }

    // Least Common Denominator (LCD)
    //
    // The LCD is the least common multiple of the two denominators.  For instance, the LCD of
    // {1/2, 1/4} is 4 because the least common multiple of 2 and 4 is 4.  Likewise, the LCD
    // of {1/2, 1/3} is 6.
    //       
    // To find the LCD:
    //
    // 1) Find the Greatest Common Divisor (GCD) of the denominators
    // 2) Multiply the denominators together
    // 3) Divide the product of the denominators by the GCD
    public static BigInteger LeastCommonDenominator(BigRational x, BigRational y) =>
        // LCD( a/b, c/d ) == (bd) / gcd(b,d)
        x.Denominator * y.Denominator / BigInteger.GreatestCommonDivisor(x.Denominator, y.Denominator);

    public static int Compare(BigRational r1, BigRational r2) =>
        //     a/b = c/d, iff ad = bc
        BigInteger.Compare(r1.m_numerator * r2.Denominator, r2.m_numerator * r1.Denominator);

    #endregion Public Static Methods

    #region Operator Overloads

    public static bool operator ==(BigRational x, BigRational y) => Compare(x, y) == 0;

    public static bool operator !=(BigRational x, BigRational y) => Compare(x, y) != 0;

    public static bool operator <(BigRational x, BigRational y) => Compare(x, y) < 0;

    public static bool operator <=(BigRational x, BigRational y) => Compare(x, y) <= 0;

    public static bool operator >(BigRational x, BigRational y) => Compare(x, y) > 0;

    public static bool operator >=(BigRational x, BigRational y) => Compare(x, y) >= 0;

    public static BigRational operator +(BigRational r) => r;

    public static BigRational operator -(BigRational r) => new(-r.m_numerator, r.Denominator);

    public static BigRational operator ++(BigRational r) => r + One;

    public static BigRational operator --(BigRational r) => r - One;

    public static BigRational operator +(BigRational r1, BigRational r2) =>
        // a/b + c/d  == (ad + bc)/bd
        new(r1.m_numerator * r2.Denominator + r1.Denominator * r2.m_numerator,
            r1.Denominator * r2.Denominator);

    public static BigRational operator -(BigRational r1, BigRational r2) =>
        // a/b - c/d  == (ad - bc)/bd
        new(r1.m_numerator * r2.Denominator - r1.Denominator * r2.m_numerator,
            r1.Denominator * r2.Denominator);

    public static BigRational operator *(BigRational r1, BigRational r2) =>
        // a/b * c/d  == (ac)/(bd)
        new(r1.m_numerator * r2.m_numerator, r1.Denominator * r2.Denominator);

    public static BigRational operator /(BigRational r1, BigRational r2) =>
        // a/b / c/d  == (ad)/(bc)
        new(r1.m_numerator * r2.Denominator, r1.Denominator * r2.m_numerator);

    public static BigRational operator %(BigRational r1, BigRational r2) =>
        // a/b % c/d  == (ad % bc)/bd
        new(r1.m_numerator * r2.Denominator % (r1.Denominator * r2.m_numerator),
            r1.Denominator * r2.Denominator);

    #endregion Operator Overloads

    // ----- SECTION: explicit conversions from BigRational to numeric base types  ----------------*

    #region explicit conversions from BigRational

    public static explicit operator sbyte(BigRational value) =>
        (sbyte)BigInteger.Divide(value.m_numerator, value.m_denominator);

    public static explicit operator ushort(BigRational value) =>
        (ushort)BigInteger.Divide(value.m_numerator, value.m_denominator);

    public static explicit operator uint(BigRational value) =>
        (uint)BigInteger.Divide(value.m_numerator, value.m_denominator);

    public static explicit operator ulong(BigRational value) =>
        (ulong)BigInteger.Divide(value.m_numerator, value.m_denominator);

    public static explicit operator byte(BigRational value) =>
        (byte)BigInteger.Divide(value.m_numerator, value.m_denominator);

    public static explicit operator short(BigRational value) =>
        (short)BigInteger.Divide(value.m_numerator, value.m_denominator);

    public static explicit operator int(BigRational value) =>
        (int)BigInteger.Divide(value.m_numerator, value.m_denominator);

    public static explicit operator long(BigRational value) =>
        (long)BigInteger.Divide(value.m_numerator, value.m_denominator);

    public static explicit operator BigInteger(BigRational value) =>
        BigInteger.Divide(value.m_numerator, value.m_denominator);

    public static explicit operator float(BigRational value) =>
        // The Single value type represents a single-precision 32-bit number with
        // values ranging from negative 3.402823e38 to positive 3.402823e38      
        // values that do not fit into this range are returned as Infinity
        (float)(double)value;

    public static explicit operator double(BigRational value)
    {
        // The Double value type represents a double-precision 64-bit number with
        // values ranging from -1.79769313486232e308 to +1.79769313486232e308
        // values that do not fit into this range are returned as +/-Infinity
        if (SafeCastToDouble(value.m_numerator) && SafeCastToDouble(value.m_denominator))
            return (double)value.m_numerator / (double)value.m_denominator;

        // scale the numerator to preseve the fraction part through the integer division
        BigInteger denormalized = value.m_numerator * s_bnDoublePrecision / value.m_denominator;
        if (denormalized.IsZero)
            return value.Sign < 0
                ? BitConverter.Int64BitsToDouble(unchecked((long)0x8000000000000000))
                : 0d; // underflow to -+0

        double result = 0;
        bool isDouble = false;
        int scale = DoubleMaxScale;

        while (scale > 0) {
            if (!isDouble) {
                if (SafeCastToDouble(denormalized)) {
                    result = (double)denormalized;
                    isDouble = true;
                } else {
                    denormalized = denormalized / 10;
                }
            }

            result = result / 10;
            scale--;
        }

        if (!isDouble)
            return value.Sign < 0 ? double.NegativeInfinity : double.PositiveInfinity;
        return result;
    }

    public static explicit operator decimal(BigRational value)
    {
        // The Decimal value type represents decimal numbers ranging
        // from +79,228,162,514,264,337,593,543,950,335 to -79,228,162,514,264,337,593,543,950,335
        // the binary representation of a Decimal value is of the form, ((-2^96 to 2^96) / 10^(0 to 28))
        if (SafeCastToDecimal(value.m_numerator) && SafeCastToDecimal(value.m_denominator))
            return (decimal)value.m_numerator / (decimal)value.m_denominator;

        // scale the numerator to preseve the fraction part through the integer division
        BigInteger denormalized = value.m_numerator * s_bnDecimalPrecision / value.m_denominator;
        if (denormalized.IsZero) return decimal.Zero; // underflow - fraction is too small to fit in a decimal
        for (int scale = DecimalMaxScale; scale >= 0; scale--)
            if (!SafeCastToDecimal(denormalized)) {
                denormalized = denormalized / 10;
            } else {
                var dec = new DecimalUInt32();
                dec.dec = (decimal)denormalized;
                dec.flags = (dec.flags & ~DecimalScaleMask) | (scale << 16);
                return dec.dec;
            }

        throw new OverflowException("Value was either too large or too small for a Decimal.");
    }

    #endregion explicit conversions from BigRational

    // ----- SECTION: implicit conversions from numeric base types to BigRational  ----------------*

    #region implicit conversions to BigRational

    public static implicit operator BigRational(sbyte value) => new((BigInteger)value);

    public static implicit operator BigRational(ushort value) => new((BigInteger)value);

    public static implicit operator BigRational(uint value) => new((BigInteger)value);

    public static implicit operator BigRational(ulong value) => new((BigInteger)value);

    public static implicit operator BigRational(byte value) => new((BigInteger)value);

    public static implicit operator BigRational(short value) => new((BigInteger)value);

    public static implicit operator BigRational(int value) => new((BigInteger)value);

    public static implicit operator BigRational(long value) => new((BigInteger)value);

    public static implicit operator BigRational(BigInteger value) => new(value);

    public static implicit operator BigRational(float value) => new(value);

    public static implicit operator BigRational(double value) => new(value);

    public static implicit operator BigRational(decimal value) => new(value);

    #endregion implicit conversions to BigRational

    // ----- SECTION: private serialization instance methods  ----------------*

    #region serialization

    void IDeserializationCallback.OnDeserialization(object sender)
    {
        try {
            // verify that the deserialized number is well formed
            if (m_denominator.Sign == 0 || m_numerator.Sign == 0) {
                // n/0 -> 0/1
                // 0/m -> 0/1
                m_numerator = BigInteger.Zero;
                m_denominator = BigInteger.One;
            } else if (m_denominator.Sign < 0) {
                m_numerator = BigInteger.Negate(m_numerator);
                m_denominator = BigInteger.Negate(m_denominator);
            }

            Simplify();
        }
        catch (ArgumentException e) {
            throw new SerializationException("invalid serialization data", e);
        }
    }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null) throw new ArgumentNullException("info");

        info.AddValue("Numerator", m_numerator);
        info.AddValue("Denominator", m_denominator);
    }

    BigRational(SerializationInfo info, StreamingContext context)
    {
        if (info == null) throw new ArgumentNullException("info");

        m_numerator = (BigInteger)info.GetValue("Numerator", typeof(BigInteger));
        m_denominator = (BigInteger)info.GetValue("Denominator", typeof(BigInteger));
    }

    #endregion serialization

    // ----- SECTION: private instance utility methods ----------------*

    #region instance helper methods

    void Simplify()
    {
        // * if the numerator is {0, +1, -1} then the fraction is already reduced
        // * if the denominator is {+1} then the fraction is already reduced
        if (m_numerator == BigInteger.Zero) m_denominator = BigInteger.One;

        BigInteger gcd = BigInteger.GreatestCommonDivisor(m_numerator, m_denominator);
        if (gcd > BigInteger.One) {
            m_numerator = m_numerator / gcd;
            m_denominator = Denominator / gcd;
        }
    }

    #endregion instance helper methods

    // ----- SECTION: private static utility methods -----------------*

    #region static helper methods

    static bool SafeCastToDouble(BigInteger value) => s_bnDoubleMinValue <= value && value <= s_bnDoubleMaxValue;

    static bool SafeCastToDecimal(BigInteger value) => s_bnDecimalMinValue <= value && value <= s_bnDecimalMaxValue;

    static void SplitDoubleIntoParts(double dbl, out int sign, out int exp, out ulong man, out bool isFinite)
    {
        DoubleUlong du;
        du.uu = 0;
        du.dbl = dbl;

        sign = 1 - ((int)(du.uu >> 62) & 2);
        man = du.uu & 0x000FFFFFFFFFFFFF;
        exp = (int)(du.uu >> 52) & 0x7FF;
        if (exp == 0) {
            // Denormalized number.
            isFinite = true;
            if (man != 0)
                exp = -1074;
        } else if (exp == 0x7FF) {
            // NaN or Infinite.
            isFinite = false;
            exp = int.MaxValue;
        } else {
            isFinite = true;
            man |= 0x0010000000000000; // mask in the implied leading 53rd significand bit
            exp -= 1075;
        }
    }

    static double GetDoubleFromParts(int sign, int exp, ulong man)
    {
        DoubleUlong du;
        du.dbl = 0;

        if (man == 0) {
            du.uu = 0;
        } else {
            // Normalize so that 0x0010 0000 0000 0000 is the highest bit set
            int cbitShift = CbitHighZero(man) - 11;
            if (cbitShift < 0)
                man >>= -cbitShift;
            else
                man <<= cbitShift;

            // Move the point to just behind the leading 1: 0x001.0 0000 0000 0000
            // (52 bits) and skew the exponent (by 0x3FF == 1023)
            exp += 1075;

            if (exp >= 0x7FF) {
                // Infinity
                du.uu = 0x7FF0000000000000;
            } else if (exp <= 0) {
                // Denormalized
                exp--;
                if (exp < -52)
                    // Underflow to zero
                    du.uu = 0;
                else
                    du.uu = man >> -exp;
            } else {
                // Mask off the implicit high bit
                du.uu = (man & 0x000FFFFFFFFFFFFF) | ((ulong)exp << 52);
            }
        }

        if (sign < 0) du.uu |= 0x8000000000000000;

        return du.dbl;
    }

    static int CbitHighZero(ulong uu)
    {
        if ((uu & 0xFFFFFFFF00000000) == 0)
            return 32 + CbitHighZero((uint)uu);
        return CbitHighZero((uint)(uu >> 32));
    }

    static int CbitHighZero(uint u)
    {
        if (u == 0)
            return 32;

        int cbit = 0;
        if ((u & 0xFFFF0000) == 0) {
            cbit += 16;
            u <<= 16;
        }

        if ((u & 0xFF000000) == 0) {
            cbit += 8;
            u <<= 8;
        }

        if ((u & 0xF0000000) == 0) {
            cbit += 4;
            u <<= 4;
        }

        if ((u & 0xC0000000) == 0) {
            cbit += 2;
            u <<= 2;
        }

        if ((u & 0x80000000) == 0)
            cbit += 1;
        return cbit;
    }

    #endregion static helper methods
} // BigRational
// namespace Numerics