using static Algorithms.Mathematics.ModularMath;
using static Algorithms.Mathematics.PolynomialOperation;

namespace Algorithms.Mathematics.Combinatorics;

public static class BernoulliNumbers
{
    public static long[] BernoulliNaive(int n, int MOD)
    {
        long[] a = new long[n];
        long[] b = new long[n];
        for (int m = 0; m < n; m++) {
            a[m] = ModInverse(m + 1, MOD);
            for (int j = m; j > 0; j--)
                a[j - 1] = (a[j - 1] - a[j] + MOD) * j % MOD;
            b[m] = a[0];
        }

        return b;
    }

    public static long[] Bernoulli(int n, bool pos = true)
    {
        long[] terms = new long[n];

        long ifact = 1;
        for (int i = 0; i < n; i++)
            ifact = ifact * (i + 1) % MOD;
        ifact = ModInverse(ifact, MOD);

        for (int i = n - 1; i >= 0; i--) {
            terms[i] = ifact;
            ifact = ifact * (i + 1) % MOD;
        }

        long[] p = Invert(terms, n, MOD);
        long fact = 1;
        for (int i = 0; i < n; i++) {
            p[i] = p[i] * fact % MOD;
            fact = fact * (i + 1) % MOD;
        }

        if (pos && n > 1)
            p[1] = MOD - p[1];
        return p;
    }

    /// <summary>
    ///     Produces a new polynomial expressing the sum of original evaluated from 0..n or 1..n
    /// </summary>
    public static long[] PolynomialSumNaive(long[] poly, long[] bernoulliPos, bool fromZero = true)
    {
        int deg = poly.Length;
        if (deg <= 0)
            return Array.Empty<long>();

        long[] result = new long[deg + 1];
        for (int p = 0; p < deg; p++)
        for (int i = 0; i <= p; i++) {
            long t = poly[p] * Fact(p) % MOD * InverseFact(i) % MOD * InverseFact(p + 1 - i) % MOD * bernoulliPos[i];
            result[p + 1 - i] = (result[p + 1 - i] + t) % MOD;
        }

        result[0] = fromZero ? poly[0] : 0;
        return result;
    }

    /// <summary>
    ///     Produces a new polynomial expressing the sum of original evaluated from 0..n or 1..n
    /// </summary>
    public static long[] PolynomialSum(long[] poly, long[] bernoulliPos, bool fromZero = true)
    {
        int deg = poly.Length;
        if (deg <= 0)
            return poly;

        long[] result = new long[deg + 1];
        for (int i = 0; i < deg; i++) // odd terms > 1 vanish
            result[i] = InverseFact(i) * bernoulliPos[i] % MOD;

        long[] pterms = new long[deg];
        for (int p = 0; p < deg; p++)
            pterms[deg - 1 - p] = poly[p] * Fact(p) % MOD;

        long[] prod = MultiplyPolynomialsMod(pterms, result, MOD, deg);
        Array.Clear(result, 0, result.Length);
        for (int i = 0, j = deg; i < prod.Length; ++i, --j)
            result[j] = prod[i] * InverseFact(j) % MOD;
        result[0] = fromZero ? poly[0] : 0;
        return result;
    }

    /// <summary>
    ///     Polynomial sum for a fixed n. Slightly faster than general polynomial.
    /// </summary>
    public static long PolynomialSum(long[] poly, long[] bernoulliPos, int n, bool fromZero = false)
    {
        long[] sums = FaulhabersTableForFixedN(bernoulliPos, n, poly.Length - 1, MOD);
        long result = fromZero ? poly[0] : 0;
        for (int i = 0; i < poly.Length; i++)
            result = (result + poly[i] * sums[i]) % MOD;
        return result;
    }

    /// <summary>
    ///     Produces a table of sums for each p from 1..deg for sum(k=1..n, k^p).
    ///     Add one to result to sum from zero. It could be a faster way of produce a polynomial
    ///     sum for a given n.
    /// </summary>
    public static long[] FaulhabersTableForFixedN(long[] bernoulliPos, int n, int deg, int MOD)
    {
        long[] poly1 = new long[deg + 1];
        for (int i = 0; i <= deg; i++) // odd terms > 1 vanish
            poly1[i] = bernoulliPos[i] * InverseFact(i) % MOD;

        long np1 = n;
        long[] poly2 = new long[deg + 1];
        for (int i = 0; i <= deg; i++, np1 = np1 * n % MOD)
            poly2[i] = np1 * InverseFact(i + 1) % MOD;

        long[] result = MultiplyPolynomialsMod(poly1, poly2, MOD, deg + 1);
        for (int i = 2; i < result.Length; i++)
            result[i] = Fact(i) * result[i] % MOD;
        return result;
    }

    /// <summary>
    ///     Produces a table of sums for each p from 0..deg for sum(k=1..n, k^p)
    /// </summary>
    public static long Faulhabers(long[] bernoulliPos, int n, int deg, int MOD)
    {
        n %= MOD;
        long sum = 0, np = n;
        for (int k = deg; k >= 0; k--, np = np * n % MOD) {
            long addend = np * InverseFact(k) % MOD * InverseFact(deg - k + 1) % MOD;
            sum = (sum + addend * bernoulliPos[k]) % MOD;
        }

        return sum * Fact(deg) % MOD;
    }
}