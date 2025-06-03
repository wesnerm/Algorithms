using System.Numerics;
using static System.Array;
using static System.Math;

namespace Algorithms.Mathematics.Multiplication;

public class FastFourierTransformMod
{
    const int sz = 16;
    const long msk = (1L << sz) - 1;
    readonly double[] ai, ar;
    readonly double[] bi, br;
    readonly double[] nai, nar;
    readonly double[] nbi, nbr;
    readonly double[] wi, wr;
    readonly int MaxN;
    readonly int MOD;

    public FastFourierTransformMod(int shift = 20, int mod = 1000000007)
    {
        MOD = mod;
        MaxN = Max(2, 1 << shift);
        double ff = 2 * PI / MaxN;
        wr = new double[MaxN];
        wi = new double[MaxN];
        ar = new double[MaxN];
        ai = new double[MaxN];
        br = new double[MaxN];
        bi = new double[MaxN];
        nar = new double[MaxN];
        nai = new double[MaxN];
        nbr = new double[MaxN];
        nbi = new double[MaxN];

        int limit = MaxN >= 4 ? MaxN / 4 : MaxN;

        //var sff = Sin(ff);
        //var cff = Cos(ff);
        //var sin = 0d;
        //var cos = 1d;

        //for (int i = 0; i < limit; i++)
        //{
        //    var cosi = wr[i] = cos;
        //    var sini = wi[i] = sin;
        //    cos = cosi * cff - sini * sff;
        //    sin = sini * cff + cosi * sff;
        //}

        for (long i = 0; i < limit; i++)
        {
            double ang = ff * i;
            wi[i] = Sin(ang);
            wr[i] = Cos(ang);
        }

        for (int i = limit; i < MaxN; i++)
        {
            wr[i] = -wi[i - limit];
            wi[i] = wr[i - limit];
        }
    }

    void DoFFT(Span<double> ir, Span<double> ii, Span<double> or, Span<double> oi, int n, int k)
    {
        if (n == 1)
        {
            or[0] = ir[0];
            oi[0] = ii[0];
            return;
        }

        int t = MaxN / n;
        n >>= 1;
        DoFFT(ir, ii, or, oi, n, 2 * k);
        DoFFT(ir.Slice(k), ii.Slice(k), or.Slice(n), oi.Slice(n), n, 2 * k);
        for (int i = 0, j = 0; i < n; i++, j += t)
        {
            // Multiplication:  (a + bi)(c + di) = (ac -bd) + (bc + ad)i
            // tmp = w[j] * output[i + n];
            double tmpr = wr[j] * or[i + n] - wi[j] * oi[i + n];
            double tmpi = wi[j] * or[i + n] + wr[j] * oi[i + n];
            or[i + n] = or[i] - tmpr;
            oi[i + n] = oi[i] - tmpi;
            or[i] += tmpr;
            oi[i] += tmpi;
        }
    }

    public long[] Multiply(ReadOnlySpan<long> a, ReadOnlySpan<long> b, int limit = int.MaxValue)
    {
        // Try out both, sometimes the third one is faster even though the first is constant time.
        int n = FftSize(a.Length + b.Length - 1);
        if (n <= 0 || limit <= 0) return Array.Empty<long>();

        for (int i = 0; i < n; i++)
        {
            long va = i < a.Length ? a[i] : 0;
            long vb = i < b.Length ? b[i] : 0;

            ar[i] = va & msk;
            ai[i] = va >> sz;
            br[i] = vb & msk;
            bi[i] = vb >> sz;
        }

        DoFFT(ar, ai, nar, nai, n, 1);
        DoFFT(br, bi, nbr, nbi, n, 1);

        for (int i = 0, nin = 0; i < n; i++, nin = n - i)
        {
            double lAr = nar[i] + nar[nin];
            double lAi = nai[i] - nai[nin];
            double gAr = nai[i] + nai[nin];
            double gAi = nar[nin] - nar[i];

            double lBr = nbr[i] + nbr[nin];
            double lBi = nbi[i] - nbi[nin];
            double gBr = nbi[i] + nbi[nin];
            double gBi = nbr[nin] - nbr[i];

            ar[i] = (lAr * lBr - gAr * gBi - gAi * gBr - lAi * lBi) * 0.25;
            ai[i] = (lAi * lBr - gAi * gBi + gAr * gBr + lAr * lBi) * 0.25;
            br[i] = (gAr * lBr - gBi * lAi + gBr * lAr - gAi * lBi) * 0.25;
            bi[i] = (gBr * lAi + gBi * lAr + gAr * lBi + gAi * lBr) * 0.25;
        }

        DoFFT(ar, ai, nar, nai, n, 1);
        DoFFT(br, bi, nbr, nbi, n, 1);

        Reverse(nar, 1, n - 1);
        Reverse(nai, 1, n - 1);
        Reverse(nbr, 1, n - 1);
        Reverse(nbi, 1, n - 1);
        int max = Min(n, limit);
        var ans = new List<long>(max);
        for (long i = 0; i < max; i++)
        {
            long aa = (long)(Round(nar[i] / n) % MOD);
            long bb = (long)(Round(nbr[i] / n) % MOD);
            long cc = (long)(Round(nai[i] / n) % MOD);
            ans.Add((aa + (bb << sz) + (cc << (2 * sz))) % MOD);
        }

        while (ans.Count > 1 && ans[ans.Count - 1] == 0)
            ans.RemoveAt(ans.Count - 1);
        return ans.ToArray();
    }

    public long[] Pow(long[] x, long n)
    {
        if (n <= 1) return n == 1 ? x : [1L];
        long[] t = Pow(x, n >> 1);
        long[] sq = Multiply(t, t);
        return (n & 1) == 0 ? sq : Multiply(x, sq);
    }

    public static int FftSize(int size) => Max(2, (int)BitOperations.RoundUpToPowerOf2((uint)size));
}