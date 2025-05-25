using System.Runtime.CompilerServices;

namespace Algorithms.Collections.Trees;

public class DoubleToPositiveLongMapper
{
    // Alternative

    public const double Base = 1000d * 1000d * 1000d;
    public const double BaseInv = 1.0 / Base;

    public static long SHR(long x) => unchecked((long)(ulong)(x >> 1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long FlipNegative(long x) => x ^ SHR(x >> 63);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Convert63X(double d) => (long)((d + Base) * Base);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Convert63X(long x) => x * BaseInv - Base;

    // Lossy Positive Conversion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Convert63(double d) => SHR(long.MinValue ^ Convert64(d));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Convert63(long x) => Convert64((x << 1) ^ long.MinValue);

    // Lossless Conversion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Convert64(double d) => FlipNegative(BitConverter.DoubleToInt64Bits(d));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Convert64(long x) => BitConverter.Int64BitsToDouble(FlipNegative(x));
}