namespace Algorithms.Mathematics.Multiplication.NTT;

// https://www.codechef.com/viewsolution/27204812

public unsafe class Ntt2 : NttBase
{
    readonly long[] wsArray;

    public Ntt2(int maxsize) : base(maxsize) => wsArray = new long[A.Length];

    protected override void NttCore(int n, long* dest, bool inverse, int mod, int g)
    {
        unchecked {
            fixed (long* ws = wsArray) {
                long* w = ws + (n >> 1);
                long t;
                w[w[0] = 1] = (int)ModPow(g, (mod - 1) / n, mod);
                if (inverse)
                    w[1] = Invl(w[1], mod);

                for (int i = 2; i < n / 2; i++)
                    w[i] = (int)(1l * w[i - 1] * w[1] % mod);

                for (int i = n >> 1; --i > 0;)
                    ws[i] = ws[i * 2];

                w = ws + 1;
                for (int i = 0, j = 0; i < n; i++) {
                    if (i < j) {
                        t = dest[i];
                        dest[i] = dest[j];
                        dest[j] = t;
                    }

                    for (int tt = n >> 1; (j ^= tt) < tt; tt >>= 1) { }
                }

                for (int i = 1; i < n; w += i, i *= 2)
                for (int j = 0; j < n; j += i * 2)
                for (int k = 0; k < i; k++) {
                    t = 1L * dest[i + j + k] * w[k] % mod;
                    dest[i + j + k] = (dest[j + k] + mod - t) % mod;
                    dest[j + k] = (dest[j + k] + t) % mod;
                }

                if (inverse) {
                    long I = Invl(n, mod);
                    for (int i = 0; i < n; i++)
                        dest[i] = dest[i] * I % mod;
                }
            }
        }
    }
}