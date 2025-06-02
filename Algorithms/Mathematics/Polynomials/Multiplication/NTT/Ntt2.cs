namespace Algorithms.Mathematics.Multiplication.NTT;

public class Ntt2 : NttBase
{
    readonly long[] wsArray;

    public Ntt2(int maxsize) : base(maxsize) => wsArray = new long[A.Length];

    protected override void NttCore(Span<long> dest, bool inverse, int mod, int g)
    {
        int n = dest.Length;
        unchecked
        {
            Span<long> ws = wsArray;
            Span<long> w = ws.Slice(n >> 1);
            long t;
            w[0] = 1;
            w[1] = ModPow(g, (mod - 1) / n, mod);
            if (inverse)
                w[1] = Invl(w[1], mod);

            int nDiv2 = n >> 1;
            for (int i = 2; i < nDiv2; i++)
                w[i] = 1L * w[i - 1] * w[1] % mod;

            for (int i = n >> 1; --i > 0;)
                ws[i] = ws[i * 2];

            w = ws.Slice(1);
            for (int i = 0, j = 0; i < n; i++)
            {
                if (i < j)
                {
                    t = dest[i];
                    dest[i] = dest[j];
                    dest[j] = t;
                }

                for (int tt = n >> 1; (j ^= tt) < tt; tt >>= 1) { }
            }

            for (int i = 1; i < n; w = w.Slice(i), i *= 2)
                for (int j = 0; j < n; j += i * 2)
                    for (int k = 0; k < i; k++)
                    {
                        t = 1L * dest[i + j + k] * w[k] % mod;
                        dest[i + j + k] = (dest[j + k] + mod - t) % mod;
                        dest[j + k] = (dest[j + k] + t) % mod;
                    }

            if (inverse)
            {
                long I = Invl(n, mod);
                for (int i = 0; i < n; i++)
                    dest[i] = dest[i] * I % mod;
            }
        }
    }
}