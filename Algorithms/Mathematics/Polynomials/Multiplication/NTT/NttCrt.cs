namespace Algorithms.Mathematics.Multiplication.NTT;

public class NttCrt
{
    static readonly int[] Primes = { 1012924417, 1004535809, 998244353 };
    static readonly int[] PrimitiveRoots = { 5, 3, 3 };
    readonly NttBase ntt;

    public NttCrt(NttBase ntt) => this.ntt = ntt;

    public long[] Multiply(long[] a, long[] b, int size, int mod)
    {
        int use = Primes.Length;
        long[][] fs = new long[use][];
        for (int k = 0; k < use; k++)
            fs[k] = ntt.Multiply(a, b, size, Primes[k], PrimitiveRoots[k]);

        int[] mods = Primes;
        long[] gammas = Prepare(mods);
        int[] buf = new int[use];
        long[] result = fs[0];
        int n = result.Length;

        // TODO: Invert this loop, so that we don't have to allocate as much memory
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < use; j++) buf[j] = (int)fs[j][i];
            long[] res = Batch(buf, mods, gammas);
            long ret = 0;
            for (int j = res.Length - 1; j >= 0; j--) ret = (ret * mods[j] + res[j]) % mod;
            result[i] = ret;
        }

        return result;
    }

    long[] Prepare(int[] m)
    {
        int n = m.Length;
        long[] factors = new long[n];
        for (int k = 1; k < n; k++) {
            long prod = 1;
            for (int i = 0; i < k; i++)
                prod = prod * m[i] % m[k];
            factors[k] = NttMB.Invl(prod, m[k]);
        }

        return factors;
    }

    long[] Batch(int[] u, int[] m, long[] gamma, long[] buffer = null)
    {
        int n = u.Length;
        Debug.Assert(n == m.Length);
        long[] result = buffer ?? new long[n];
        result[0] = u[0];
        for (int k = 1; k < n; k++) {
            long tmp = result[k - 1];
            for (int j = k - 2; j >= 0; j--)
                tmp = (tmp * m[j] + result[j]) % m[k];
            result[k] = (u[k] - tmp) * gamma[k] % m[k];
            if (result[k] < 0) result[k] += m[k];
        }

        return result;
    }
}