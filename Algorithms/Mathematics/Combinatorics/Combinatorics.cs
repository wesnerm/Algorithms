using System.Numerics;
using Algorithms.Mathematics.Combinatorics;
using Algorithms.Mathematics.Numerics;

namespace Algorithms.Mathematics;

public class Combinatorics2
{
    // https://www.hackerrank.com/contests/projecteuler/challenges/euler076/copy-from/1302323130
    public static BigInteger[] IntegerPartitionTable(int n)
    {
        var table = new BigInteger[n + 1];

        table[0] = 1;
        for (int i = 1; i < table.Length; i++)
        for (int j = 1;; j++) {
            long p = GeneralizedPentagonalNumber(j);
            if (p > i) break;
            int sign = (((j - 1) >> 1) & 1) == 0 ? 1 : -1;
            //if (i<100)
            //    Console.Error.WriteLine($"table[{i}] += table[{i}-{p}] * {sign}");
            table[i] += table[i - p] * sign;
        }

        //if (i<100||i%10==0)
        //Console.Error.WriteLine($"{i}->{table[i]}");
        return table;
    }

    public static long PentagonalNumber(long k) => k * (3 * k - 1) / 2;

    public static long GeneralizedPentagonalNumber(long k)
    {
        if (k == 0) return k;
        long kk = (k + 1) >> 1;
        if ((k & 1) == 0) kk = -kk;
        return kk * (3 * kk - 1) / 2;
    }

    /// <summary>
    ///     Sums from i = 0 or 1 to n
    /// </summary>
    /// <param name="n">The n.</param>
    /// <returns></returns>
    public static long SumI(long n) => (n * (n + 1)) >> 1;

    public static long SumI(long m, long n) =>
        // ArithmeticSeries
        // Also, (n*(n+1) - m*(m-1)/2
        ((n + 1 - m) * (n + m)) >> 1;

    public static long SumI2(long n) =>
        // Square Pyramidal Number
        // Alternative, n * (n + 1) * (2 * n + 1) / 6;
        // Alternative, n*n*n/3 + n*n/2 + n/6
        ((n * (n + 1)) >> 1) * (2 * n + 1) / 3;

    //public static long SumI2(long n)
    //{
    //    // Square Pyramidal Number
    //    // Alternative, n * (n + 1) * (2 * n + 1) / 6;
    //    // Alternative, n*n*n/3 + n*n/2 + n/6
    //    return Div(Div(n * (n + 1) % MOD, 2) * (2 * n + 1) % MOD, 3);
    //}

    public static long SumI3(long n)
    {
        // Alternative, n*n*n*n/4 + n*n*n/2 + n*n/4
        long tmp = n * (n + 1) / 2;
        return tmp * tmp;
    }

    public static long SumI4(long n) =>
        // Alternative, n*n*n*n/4 + n*n*n/2 + n*n/4 (real numbers)
        // Alternative, ((((6 * n + 15) * n + 10) * n) * n - 1) * n / 30;
        n * (n + 1) * (2 * n + 1) / 6 * (3 * n * n + 3 * n - 1) / 5;

    public static long SumI5(long n)
    {
        // Alternatively, SumI3From01ToN(n) * (2*n*n + 2*n - 1) / 3
        long n2 = n * n;
        return (((2 * n + 6) * n + 5) * n2 - 1) * n2 / 12;
    }

    // https://en.wikipedia.org/wiki/Summation
    // https://www.math.uh.edu/~ilya/class/useful_summations.pdf
    public static long SumPowers(long a, long n) =>
        // i from 0 to n
        (1 - Pow(a, n + 1)) / (1 - a);

    public static long SumPowers(long a, long m, long n) =>
        // i from m to n
        (Pow(a, m) - Pow(a, n + 1)) / (1 - a);

    public static long SumIPowers(long a, long n)
    {
        long a1 = 1 - a;
        long pow = Pow(a, n);
        return a * (1 - (n + 1) * pow + a * n * pow) / (a1 * a1);
    }

    public static long Pow(long n, long p)
    {
        long b = n;
        long result = 1;
        while (p != 0) {
            if ((p & 1) != 0)
                result *= b;
            p >>= 1;
            b *= b;
        }

        return result;
    }

    public static long GeometricSeries(long r, long p, long mod)
    {
        long result = r != 1
            ? (ModularMath.ModPow(r, p + 1, mod) - 1) * ModularMath.ModInverse(r - 1, MOD)
            : p + 1;
        result %= MOD;
        return result >= 0 ? result : result + MOD;
    }

    public static long[][] SumsOfPowers(int n, long MOD)
    {
        long[][] poly = new long[n + 1][];
        long[] bernoulli = BernoulliNumbers.Bernoulli(n + 2);
        for (int m = 1; m < poly.Length; m++) {
            long[] p = poly[m] = new long[m + 2];
            long inv = ModularMath.ModInverse(m + 1, MOD);
            for (int k = 0; k <= m; k++) {
                long bk = bernoulli[k];
                if (bk != 0) p[m + 1 - k] = bk * Permutations.Combinations(m + 1, k) % MOD * inv % MOD;
            }
        }

        return poly;
    }

    public static long SumsOfPowers(long n, long p, long mod)
    {
        long[] y = new long[p + 2];
        long sum = 0;
        for (int i = 0; i <= p; i++) {
            sum = sum + ModularMath.ModPow(i + 1, p, mod);
            if (sum >= mod) sum -= mod;
            y[i + 1] = sum;
        }

        return InterpolationMod.Lagrange(y, n, mod);
    }
}