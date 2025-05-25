using static System.Array;
using static System.Math;

namespace Algorithms.Mathematics.Numerics;

public class FastFourierTransformMod
{
    const int sz = 16;
    const long msk = (1L << sz) - 1;
    readonly double[] ai;
    readonly double[] ar;
    readonly double[] bi;
    readonly double[] br;
    readonly long MaxN;
    readonly int MOD;
    readonly double[] nai;
    readonly double[] nar;
    readonly double[] nbi;
    readonly double[] nbr;
    readonly double[] wi;
    readonly double[] wr;

    public FastFourierTransformMod(int shift = 20, int mod = 1000000007)
    {
        MOD = mod;
        MaxN = 1L << shift;
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
        for (long i = 0; i < MaxN; i++) {
            double ang = ff * i;
            wr[i] = Cos(ang);
            wi[i] = Sin(ang);
        }
    }

    unsafe void DoFFT(double* ir, double* ii, double* or, double* oi, long n, long k)
    {
        if (n == 1) {
            or[0] = ir[0];
            oi[0] = ii[0];
            return;
        }

        long t = MaxN / n;
        n >>= 1;
        DoFFT(ir, ii, or, oi, n, 2 * k);
        DoFFT(ir + k, ii + k, or + n, oi + n, n, 2 * k);
        for (long i = 0, j = 0; i < n; i++, j += t) {
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

    public unsafe List<long> Multiply(List<long> a, List<long> b, int limit = int.MaxValue)
    {
        // Try out both, sometimes the third one is faster even though the first is constant time.
        int n = FftSize(a.Count + b.Count - 1);

        for (int i = 0; i < n; i++) {
            long va = i < a.Count ? a[i] : 0;
            long vb = i < b.Count ? b[i] : 0;

            ar[i] = va & msk;
            ai[i] = va >> sz;
            br[i] = vb & msk;
            bi[i] = vb >> sz;
        }

        fixed (double* par = ar, pai = ai)
        fixed (double* pbr = br, pbi = bi)
        fixed (double* pnar = nar, pnai = nai)
        fixed (double* pnbr = nbr, pnbi = nbi) {
            DoFFT(par, pai, pnar, pnai, n, 1);
            DoFFT(pbr, pbi, pnbr, pnbi, n, 1);

            for (int i = 0, nin = 0; i < n; i++, nin = n - i) {
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

            DoFFT(par, pai, pnar, pnai, n, 1);
            DoFFT(pbr, pbi, pnbr, pnbi, n, 1);
        }

        Reverse(nar, 1, n - 1);
        Reverse(nai, 1, n - 1);
        Reverse(nbr, 1, n - 1);
        Reverse(nbi, 1, n - 1);
        int max = Min(n, limit);
        var ans = new List<long>(max);
        for (long i = 0; i < max; i++) {
            long aa = (long)(Round(nar[i] / n) % MOD);
            long bb = (long)(Round(nbr[i] / n) % MOD);
            long cc = (long)(Round(nai[i] / n) % MOD);
            ans.Add((aa + (bb << sz) + (cc << (2 * sz))) % MOD);
        }

        while (ans.Count > 1 && ans[ans.Count - 1] == 0)
            ans.RemoveAt(ans.Count - 1);
        return ans;
    }

    public List<long> Pow(List<long> x, long n)
    {
        //if (n == 0) return new List<long> { 1 };
        //var t = PolyPow(x, n / 2);
        //return (n & 1) == 0 ? Multiply(t, t) : Multiply(x, Multiply(t, t));

        if (n <= 1) return n == 1 ? x : new List<long> { 1 };
        List<long> t = Pow(x, n >> 1);
        List<long> sq = Multiply(t, t);
        return (n & 1) == 0 ? sq : Multiply(x, sq);
    }

    public static int FftSize(int size) => Max(2, HighestOneBit(size - 1) << 1);
}