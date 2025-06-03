using System.Numerics;
using System.Runtime.CompilerServices;

namespace Algorithms.Mathematics.Multiplication.NTT;

public class NttMB : NttBase
{
    public NttMB(int maxsize) : base(maxsize) { }

    protected override void NttCore(Span<long> dst, bool inverse, int mod, int g)
    {
        int n = dst.Length;
        unchecked {
            int h = BitOperations.Log2((uint)(n & -n));
            long b = HighestOneBit((long)mod) << 1;
            int H = BitOperations.Log2((ulong)b) * 2;
            long m = b * b / mod;

            int[] wws = new int[1 << (h - 1)];
            long dw = inverse ? ModPow(g, mod - 1 - (mod - 1) / n, mod) : ModPow(g, (mod - 1) / n, mod);
            long w = (1L << 32) % mod;
            for (int k = 0; k < 1 << (h - 1); k++) {
                wws[k] = (int)w;
                w = Modh(w * dw, m, H, mod);
            }

            long J = Invl(mod, 1L << 32);
            for (int i = 0; i < h; i++) {
                for (int j = 0; j < 1 << i; j++) {
                    int hlimit = 1 << (h - i - 1);
                    for (int k = 0, s = j << (h - i), t = s | hlimit; k < hlimit; k++, s++, t++) {
                        long u = (dst[s] - dst[t] + 2 * mod) * wws[k];
                        dst[s] += dst[t];
                        if (dst[s] >= 2 * mod) dst[s] -= 2 * mod;
                        long Q = (long)((ulong)((u << 32) * J) >> 32);
                        dst[t] = (u >> 32) - ((Q * mod) >> 32) + mod;
                    }
                }

                if (i < h - 1)
                    for (int k = 0; k < 1 << (h - i - 2); k++)
                        wws[k] = wws[k * 2];
            }

            for (int i = 0; i < n; i++)
                if (dst[i] >= mod)
                    dst[i] -= mod;
            for (int i = 0; i < n; i++) {
                int rev = unchecked((int)((uint)Reverse(i) >> -h));
                if (i < rev) {
                    long d = dst[i];
                    dst[i] = dst[rev];
                    dst[rev] = d;
                }
            }

            if (inverse) {
                long inv = Invl(n, mod);
                for (int i = 0; i < n; i++)
                    dst[i] = Modh(dst[i] * inv, m, H, mod);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static long Modh(long a, long M, int h, int MOD)
    {
        long r = a - ((((M * (a & int.MaxValue)) >> 31) + M * (a >> 31)) >> (h - 31)) * MOD;
        return r < MOD ? r : r - MOD;
    }
}