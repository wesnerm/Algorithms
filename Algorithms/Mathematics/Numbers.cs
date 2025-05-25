#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
//
// Copyright (C) 2008, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.ComponentModel;

#endregion

namespace Algorithms.Mathematics;

#pragma warning disable 0660

/// <summary>
///     Summary description for Number.
/// </summary>

//[DebuggerStepThrough]
public static class Numbers
{
    public static bool ParseULong(string text, out ulong number, uint baseSystem = 10)
    {
        unchecked {
            number = 0;
            foreach (char ch in text) {
                int n = ch - '0';
                if (n > 9) {
                    n = StringTools.HexDigit(ch);
                    if (n < 10)
                        return false;
                }

                if (n < 0 || n >= baseSystem)
                    return false;

                ulong oldNumber = number;
                number = number * baseSystem + (uint)n;

                // Fail on overflow
                if (number < oldNumber)
                    return false;
            }

            return true;
        }
    }

    public static string GetNumberString(double d)
    {
        if (d == double.PositiveInfinity) return "+∞";
        if (d == double.NegativeInfinity) return "-∞";
        return d.ToString();
    }

    #region Constants

    public const double Epsilon = 10e-9;
    public const float EpsilonFloat = 10e-5f;
    public const double Degrees = Math.PI / 180.0;

    #endregion

    #region Static Routines

    #region Comparison

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new static bool Equals(object num1, object num2) => throw new NotSupportedException();

    [DebuggerStepThrough]
    public static bool AreClose(this double num1, double num2)
    {
        double cmp = num1 - num2;
        return cmp > -Epsilon && cmp < Epsilon;
    }

    [DebuggerStepThrough]
    public static bool AreClose(this float num1, float num2)
    {
        double cmp = num1 - num2;
        return cmp > -EpsilonFloat && cmp < EpsilonFloat;
    }

    [DebuggerStepThrough]
    public static bool AreClose(this double num1, double num2, double epsilon)
    {
        double cmp = num1 - num2;
        return cmp < epsilon && cmp > -epsilon;
    }

    [DebuggerStepThrough]
    public static bool IsNormalNumber(double d) => d < double.PositiveInfinity && d > double.NegativeInfinity;

    [DebuggerStepThrough]
    public static bool IsZeroed(double d) => d < Epsilon && d > -Epsilon;

    [DebuggerStepThrough]
    public static int Round(this double d)
    {
        if (d > int.MaxValue)
            return int.MaxValue;
        if (d < int.MinValue)
            return int.MinValue;
        return (int)Math.Round(d);
    }

    [DebuggerStepThrough]
    public static int Compare(double num1, double num2)
    {
        double cmp = num1 - num2;
        if (cmp > Epsilon) return 1;
        if (cmp < -Epsilon) return -1;
        return 0;
    }

    [DebuggerStepThrough]
    public static bool IsGreater(double num1, double num2) => Compare(num1, num2) > 0;

    [DebuggerStepThrough]
    public static bool IsGreaterEqual(double num1, double num2) => Compare(num1, num2) >= 0;

    [DebuggerStepThrough]
    public static bool IsLess(double num1, double num2) => Compare(num1, num2) < 0;

    [DebuggerStepThrough]
    public static bool IsLessEqual(double num1, double num2) => Compare(num1, num2) <= 0;

    #endregion

    #region Text Tools

    public static bool ToInteger(this double d, out long value)
    {
        if (d <= long.MaxValue && d >= long.MinValue) {
            value = (long)d;
            return true;
        }

        value = 0;
        return false;
    }

    public static bool ToInteger(this double d, out ulong value)
    {
        if (d <= ulong.MaxValue && d >= 0) {
            value = (ulong)d;
            return true;
        }

        value = 0;
        return false;
    }

    public static bool ToInteger(this double d, out int value)
    {
        if (d <= int.MaxValue && d >= int.MinValue) {
            value = (int)d;
            return true;
        }

        value = 0;
        return false;
    }

    public static bool ToInteger(long d, out int value)
    {
        if (d <= int.MaxValue && d >= int.MinValue) {
            value = (int)d;
            return true;
        }

        value = 0;
        return false;
    }

    #endregion

    #endregion

    #region Utilities

    public static long Interpolate(long value, long oldStart, long oldEnd, long newStart, long newEnd)
    {
        long den = oldEnd - oldStart;
        if (den == 0)
            return value;

        long result = newStart + (value - oldStart) * (newEnd - newStart) / den;
        return result;
    }

    public static double Interpolate(double value, double oldStart, double oldEnd, double newStart, double newEnd)
    {
        double den = oldEnd - oldStart;
        if (den == 0)
            return value;

        double result = newStart + (value - oldStart) * ((newEnd - newStart) / den);
        return result;
    }

    public static int Cap(int value, int start, int end)
    {
        if (value < start)
            return start;
        if (value > end)
            return end;
        return value;
    }

    public static double Cap(double value, double start, double end)
    {
        if (value < start)
            return start;
        if (value > end)
            return end;
        return value;
    }

    #endregion
}