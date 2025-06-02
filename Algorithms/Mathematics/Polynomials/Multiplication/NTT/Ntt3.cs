namespace Algorithms.Mathematics.Multiplication.NTT;

// https://www.codechef.com/viewsolution/26459031

public class Ntt3 : NttBase
{
    readonly long[][] w = new long[30][];

    public Ntt3(int maxsize, int mod) : base(maxsize)
    {
        Init(mod);
    }

    void Init(int mod, int g = 3)
    {
        int n = A.Length;
        for (int i = 2, t = 0; i <= n; i <<= 1, t++) {
            long[] wt = w[t] = new long[i >> 1];
            long wn = ModPow(g, (mod - 1) / i, mod);
            wt[0] = 1;
            for (int j = 1; j < i >> 1; j++)
                wt[j] = wt[j - 1] * wn % mod;
        }
    }

    void Reverse(Span<long> dest)
    {
        int len = dest.Length;
        int j = len >> 1;
        for (int i = 1; i < len - 1; i++) {
            if (i < j)
                (dest[i], dest[j]) = (dest[j], dest[i]);
            int k = len >> 1;
            while (j >= k) {
                j -= k;
                k >>= 1;
            }

            if (j < k)
                j += k;
        }
    }

    protected override void NttCore(Span<long> dest, bool inverse, int mod, int g)
    {
        int n = dest.Length;
        unchecked {
            Reverse(dest);
            for (int i = 2, t = 0; i <= n; i <<= 1, t++)
            for (int j = 0; j < n; j += i)
            for (int k = j; k < j + (i >> 1); k++) {
                long u = dest[k];
                long v = w[t][k - j] * dest[k + (i >> 1)];
                dest[k] = (u + v) % mod;
                dest[k + (i >> 1)] = (u - v) % mod;
            }

            if (inverse) {
                dest.Slice(1, n - 1).Reverse();
                long nev = Invl(n, mod);
                for (int i = 0; i < n; i++)
                    dest[i] = (dest[i] + mod) * nev % mod;
            }
        }
    }
}