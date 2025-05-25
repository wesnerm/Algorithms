using System.Numerics;

namespace Algorithms.Mathematics;

public static class MillerRabinBigInteger
{
    // Warning: Needs to be under 64 -- otherwsise prime will be cut out
    static readonly int[] PrimesBetween7And61 =
        new[] { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61 };

    public static bool IsPrime(BigInteger n)
    {
        if (n < 2) return false;
        if (n <= int.MaxValue) return MillerRabin.IsPrime32((uint)n);

        // Easy Test
        if (!MayBePrime(n))
            return false;

        // Hard test
        long[] witnesses = n < 1122004669633
            ? MillerRabin.Witness40
            : n < 3071837692357849
                ? MillerRabin.Witness51
                : MillerRabin.Witness64;

        BigInteger s = n - 1;
        while ((s & 1) == 0) s >>= 1;

        for (int i = 0; i < witnesses.Length; i++) {
            long w = witnesses[i];
            // Witnesses can't be a multiple of n
            // The inequality w < 2^31 < n is guaranteed by the 32-bit int rerouting
            // if (w % n == 0) continue;

            BigInteger v = BigInteger.ModPow(w, s, n);
            if (v != 1)
                for (BigInteger t = s; v != n - 1; t <<= 1) {
                    if (t >= n - 1)
                        return false;
                    v = v * v % n;
                }
        }

        return true;
    }

    public static bool MayBePrime(BigInteger n)
    {
        const int PrimeFilter235 = 0
                                   | (1 << 1) | (1 << 7)
                                   | (1 << 11) | (1 << 13) | (1 << 17) | (1 << 19)
                                   | (1 << 23) | (1 << 29);

        if ((PrimeFilter235 & (1 << (int)(n % 30))) == 0)
            return false;

        // Quick test
        foreach (int v in PrimesBetween7And61)
            if (n % v == 0)
                return n == v; // Safety check

        // all filtered numbers less than 61 * 67 must be prime

        return true;
    }
}