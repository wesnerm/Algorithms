using System.Runtime.CompilerServices;

namespace Algorithms.Mathematics;

public static class EulerPrimeTester
{
    // Warning: Needs to be under 64 -- otherwsise prime will be cut out
    const long PrimesUnder64 = 0
                               | (1L << 02) | (1L << 03) | (1L << 05) | (1L << 07)
                               | (1L << 11) | (1L << 13) | (1L << 17) | (1L << 19)
                               | (1L << 23) | (1L << 29)
                               | (1L << 31) | (1L << 37)
                               | (1L << 41) | (1L << 43) | (1L << 47)
                               | (1L << 53) | (1L << 59)
                               | (1L << 61);

    static readonly int[] PrimesBetween7And61 =
        new[] { 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61 };

    public static long ModPow(long n, long p, long mod)
    {
        long result = 1;
        long b = n;
        while (p > 0) {
            if ((p & 1) == 1) result = Mult64(result, b, mod);
            p >>= 1;
            b = Mult64(b, b, mod);
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Mult64(long a, long b, long mod)
    {
        long q = (long)((double)a * b / mod);
        long result = unchecked(a * b - mod * q) % mod;
        return result + (result >= 0 ? 0 : mod);
    }

    public static bool MayBePrime(long n)
    {
        const int PrimeFilter235 = 0
                                   | (1 << 1) | (1 << 7)
                                   | (1 << 11) | (1 << 13) | (1 << 17) | (1 << 19)
                                   | (1 << 23) | (1 << 29);

        if ((PrimeFilter235 & (1 << (int)(n % 30))) == 0)
            return false;

        if (n > int.MaxValue) {
            // Long division is slower
            foreach (int v in PrimesBetween7And61)
                if (n % v == 0)
                    return n == v; // Safety check
        } else {
            int d = (int)n;
            foreach (int v in PrimesBetween7And61)
                if (d % v == 0)
                    return n == v; // Safety check
        }

        return true;
    }

    public static bool FermatProbablyPrime(long n)
    {
        // 2 is the first prime
        if (n < 2) return false;

        // Return primes under 64 in constant time
        // Important Step! witnesses < 64 <= n
        if (n < 64) return (PrimesUnder64 & (1L << (int)n)) != 0;

        return MayBePrime(n)
            ? FermatProbablyPrime(n, 2) && FermatProbablyPrime(n, 3) && FermatProbablyPrime(n, 5)
            : n == 2;
    }

    static bool FermatProbablyPrime(long n, long b) => ModPow(b, n - 1, n) == 1;

    public static bool EulerProbablyPrime(long n) =>
        // isprime(n) - Test whether n is prime using a variety of pseudoprime tests.*/
        MayBePrime(n)
            ? EulerProbablyPrime(n, 2) && EulerProbablyPrime(n, 3) && EulerProbablyPrime(n, 5)
            : n == 2;

    static bool EulerProbablyPrime(long n, long b)
    {
        if (!FermatProbablyPrime(n, b)) return false;

        long r = n - 1;
        while ((r & 1) == 0) r >>= 1;

        long c = ModPow(b, r, n);
        if (c != 1)
            while (c != n - 1) {
                c = ModPow(c, 2, n);
                if (c == 1) return false;
            }

        return true;
    }
}