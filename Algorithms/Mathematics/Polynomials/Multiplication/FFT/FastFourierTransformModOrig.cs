using System.Numerics;

namespace Algorithms.Mathematics.Numerics;

public class FastFourierTransformModOrig
{
    const int shift = 16;
    const long mask = (1L << shift) - 1;
    readonly long MaxN;
    readonly long MOD;
    readonly Complex[] w;

    public FastFourierTransformModOrig(int shift = 20, int mod = 1000000007)
    {
        MOD = mod;
        MaxN = 1L << shift;
        double ff = 2 * Math.PI / MaxN;
        w = new Complex[MaxN];
        for (long i = 0; i < MaxN; i++)
            w[i] = Complex.FromPolarCoordinates(1, ff * i);
    }

    unsafe void DoFFT(Complex* input, Complex* output, long n, long k)
    {
        if (n == 1) {
            output[0] = input[0];
            return;
        }

        long t = MaxN / n;
        n /= 2;
        DoFFT(input, output, n, 2 * k);
        DoFFT(input + k, output + n, n, 2 * k);
        for (long i = 0, j = 0; i < n; i++, j += t) {
            Complex tmp = w[j] * output[i + n];
            output[i + n] = output[i] - tmp;
            output[i] = output[i] + tmp;
        }
    }

    public unsafe List<long> Multiply(List<long> a, List<long> b, int limit = int.MaxValue)
    {
        int n = a.Count + b.Count;
        while ((n & (n - 1)) != 0) n++;

        var A = new Complex[n];
        var B = new Complex[n];
        for (int i = 0; i < n; i++) {
            long va = i < a.Count ? a[i] : 0;
            long vb = i < b.Count ? b[i] : 0;

            A[i] = new Complex(va & mask, va >> shift);
            B[i] = new Complex(vb & mask, vb >> shift);
        }

        var nA = new Complex[n];
        var nB = new Complex[n];

        var i2 = new Complex(0, 2);
        var i1 = new Complex(0, 1);
        double inv2 = 0.5;
        Complex inv2i = 1 / i2;

        fixed (Complex* pa = A)
        fixed (Complex* pb = B)
        fixed (Complex* na = nA)
        fixed (Complex* nb = nB) {
            DoFFT(pa, na, n, 1);
            DoFFT(pb, nb, n, 1);

            for (int i = 0; i < n; i++) {
                int nin = n - i;
                if (nin == n) nin = 0;
                Complex lA = (nA[i] + Complex.Conjugate(nA[nin])) * inv2;
                Complex gA = (nA[i] - Complex.Conjugate(nA[nin])) * inv2i;
                Complex lB = (nB[i] + Complex.Conjugate(nB[nin])) * inv2;
                Complex gB = (nB[i] - Complex.Conjugate(nB[nin])) * inv2i;
                A[i] = lA * lB + i1 * gA * gB;
                B[i] = lA * gB + gA * lB;
            }

            DoFFT(pa, na, n, 1);
            DoFFT(pb, nb, n, 1);
        }

        Array.Reverse(nA, 1, nA.Length - 1);
        Array.Reverse(nB, 1, nB.Length - 1);
        var ans = new List<long>(n);
        for (long i = 0; i < n; i++) {
            long aa = (long)(Math.Round(nA[i].Real / n) % MOD);
            long bb = (long)(Math.Round(nB[i].Real / n) % MOD);
            long cc = (long)(Math.Round(nA[i].Imaginary / n) % MOD);
            ans.Add((aa + (bb << shift) + (cc << (2 * shift))) % MOD);
        }

        while (ans.Count > 1 && ans[ans.Count - 1] == 0)
            ans.RemoveAt(ans.Count - 1);
        if (ans.Count > limit)
            ans.RemoveRange(100001, ans.Count - 100001);
        return ans;
    }

    public List<long> Pow(List<long> x, long n)
    {
        if (n <= 1) return n == 1 ? x : new List<long> { 1 };
        List<long> t = Pow(x, n >> 1);
        List<long> sq = Multiply(t, t);
        return (n & 1) == 0 ? sq : Multiply(x, sq);
    }
}