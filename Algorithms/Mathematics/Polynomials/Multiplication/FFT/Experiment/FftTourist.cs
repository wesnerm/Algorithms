using System.Numerics;

namespace Algorithms.Mathematics.Multiplication.FFT.Experiment;

// https://codeforces.com/contest/986/submission/42104811

public class FftTourist
{
    public static Complex[] roots = new Complex[0];
    public static Complex[] fa = new Complex[0];
    public static Complex[] fb = new Complex[0];
    public static int baseSize = 1;
    public static int[] rev = new int[0];
    public static readonly double PI = Math.Acos(-1.0);

    public static void EnsureBase(int nbase)
    {
        if (nbase <= baseSize)
            return;

        if (rev.Length < 1 << nbase) {
            Array.Resize(ref rev, 1 << nbase);
            Array.Resize(ref roots, 1 << nbase);
        }

        for (int i = 0; i < 1 << nbase; i++)
            rev[i] = (rev[i >> 1] >> 1) + ((i & 1) << (nbase - 1));

        while (baseSize < nbase) {
            double angle = 2 * PI / (1 << (baseSize + 1));
            //      Complex z = new Complex(cos(angle), sin(angle));
            for (int i = 1 << (baseSize - 1); i < 1 << baseSize; i++) {
                roots[i << 1] = roots[i];
                //        roots[(i << 1) + 1] = roots[i] * z;
                double angle_i = angle * (2 * i + 1 - (1 << baseSize));
                roots[(i << 1) + 1] = new Complex(Math.Cos(angle_i), Math.Sin(angle_i));
            }

            baseSize++;
        }
    }

    public static void Fft(Complex[] a, int n = -1)
    {
        if (n == -1)
            n = a.Length;

        Debug.Assert((n & (n - 1)) == 0);
        int zeros = BitOperations.Log2((uint)n);
        EnsureBase(zeros);
        int shift = baseSize - zeros;
        for (int i = 0; i < n; i++)
            if (i < rev[i] >> shift)
                Swap(ref a[i], ref a[rev[i] >> shift]);

        for (int k = 1; k < n; k <<= 1)
        for (int i = 0; i < n; i += 2 * k)
        for (int j = 0; j < k; j++) {
            Complex z = a[i + j + k] * roots[j + k];
            a[i + j + k] = a[i + j] - z;
            a[i + j] = a[i + j] + z;
        }
        /*    for (int len = 1; len < n; len <<= 1) {
              for (int i = 0; i < n; i += 2 * len) {
                for (int j = i, k = i + len; j < i + len; j++, k++) {
                  Complex z = a[k] * roots[k - i];
                  a[k] = a[j] - z;
                  a[j] = a[j] + z;
                }
              }
            }*/
    }

    public static int[] Multiply(int[] a, int[] b)
    {
        int need = a.Length + b.Length - 1;
        int nbase = 1;
        while (1 << nbase < need)
            nbase++;

        EnsureBase(nbase);
        int sz = 1 << nbase;
        if (sz > fa.Length)
            Array.Resize(ref fa, sz);

        for (int i = 0; i < sz; i++)
            fa[i] = new Complex(
                i < a.Length ? a[i] : 0,
                i < b.Length ? b[i] : 0);

        Fft(fa, sz);
        var r = new Complex(0, -0.25 / (sz >> 1));
        for (int i = 0; i <= sz >> 1; i++) {
            int j = (sz - i) & (sz - 1);
            Complex z = (fa[j] * fa[j] - Complex.Conjugate(fa[i] * fa[i])) * r;
            if (i != j) fa[j] = (fa[i] * fa[i] - Complex.Conjugate(fa[j] * fa[j])) * r;
            fa[i] = z;
        }

        for (int i = 0; i < sz >> 1; i++) {
            Complex A0 = (fa[i] + fa[i + (sz >> 1)]) * new Complex(0.5, 0);
            Complex A1 = (fa[i] - fa[i + (sz >> 1)]) * new Complex(0.5, 0) * roots[(sz >> 1) + i];
            fa[i] = A0 + A1 * new Complex(0, 1);
        }

        Fft(fa, sz >> 1);

        int[] res = new int[need];
        for (int i = 0; i < need; i++)
            res[i] = (int)(i % 2 == 0 ? fa[i >> 1].Real + 0.5 : fa[i >> 1].Imaginary + 0.5);
        return res;
    }

    static void Transfer(ref Complex[] fa, int[] a, int aLength, int sz)
    {
        if (sz >> 1 > fa.Length)
            Array.Resize(ref fa, sz >> 1);

        for (int i = 0; i < sz >> 1; i++)
            fa[i] = new Complex(
                2 * i < aLength ? a[2 * i] : 0,
                2 * i + 1 < aLength ? a[2 * i + 1] : 0);
    }

    public static long[] Square(int[] a)
    {
        int need = 2 * a.Length - 1;
        int nbase = 1;
        while (1 << nbase < need)
            nbase++;

        EnsureBase(nbase);
        int sz = 1 << nbase;
        Transfer(ref fa, a, a.Length, sz);
        Fft(fa, sz >> 1);
        var r = new Complex(1.0 / (sz >> 1), 0.0);
        for (int i = 0; i <= sz >> 2; i++) {
            int j = ((sz >> 1) - i) & ((sz >> 1) - 1);
            Complex fe = (fa[i] + Complex.Conjugate(fa[j])) * new Complex(0.5, 0);
            Complex fo = (fa[i] - Complex.Conjugate(fa[j])) * new Complex(0, -0.5);
            Complex aux = fe * fe + fo * fo * roots[(sz >> 1) + i] * roots[(sz >> 1) + i];
            Complex tmp = fe * fo;
            fa[i] = r * (Complex.Conjugate(aux) + new Complex(0, 2) * Complex.Conjugate(tmp));
            fa[j] = r * (aux + new Complex(0, 2) * tmp);
        }

        Fft(fa, sz >> 1);
        long[] res = new long[need];
        for (int i = 0; i < need; i++)
            res[i] = i % 2 == 0 ? (long)(fa[i >> 1].Real + 0.5) : (long)(fa[i >> 1].Imaginary + 0.5);
        return res;
    }

    public static int[] MultiplyMod(int[] a, int[] b, int m, bool eq = false)
    {
        int need = a.Length + b.Length - 1;
        int nbase = 0;
        while (1 << nbase < need)
            nbase++;

        EnsureBase(nbase);
        int sz = 1 << nbase;
        if (sz > fa.Length)
            Array.Resize(ref fa, sz);

        for (int i = 0; i < a.Length; i++) {
            int x = (a[i] % m + m) % m;
            fa[i] = new Complex(x & ((1 << 15) - 1), x >> 15);
        }

        Array.Clear(fa, a.Length, sz - a.Length);

        Fft(fa, sz);
        if (sz > fb.Length)
            Array.Resize(ref fb, sz);

        if (eq) {
            Array.Copy(fa, 0, fb, 0, sz);
        } else {
            for (int i = 0; i < b.Length; i++) {
                int x = (b[i] % m + m) % m;
                fb[i] = new Complex(x & ((1 << 15) - 1), x >> 15);
            }

            Array.Clear(fb, b.Length, sz - b.Length);
            Fft(fb, sz);
        }

        double ratio = 0.25 / sz;
        var r2 = new Complex(0, -1);
        var r3 = new Complex(ratio, 0);
        var r4 = new Complex(0, -ratio);
        var r5 = new Complex(0, 1);
        for (int i = 0; i <= sz >> 1; i++) {
            int j = (sz - i) & (sz - 1);
            Complex a1 = fa[i] + Complex.Conjugate(fa[j]);
            Complex a2 = (fa[i] - Complex.Conjugate(fa[j])) * r2;
            Complex b1 = (fb[i] + Complex.Conjugate(fb[j])) * r3;
            Complex b2 = (fb[i] - Complex.Conjugate(fb[j])) * r4;
            if (i != j) {
                Complex c1 = fa[j] + Complex.Conjugate(fa[i]);
                Complex c2 = (fa[j] - Complex.Conjugate(fa[i])) * r2;
                Complex d1 = (fb[j] + Complex.Conjugate(fb[i])) * r3;
                Complex d2 = (fb[j] - Complex.Conjugate(fb[i])) * r4;
                fa[i] = c1 * d1 + c2 * d2 * r5;
                fb[i] = c1 * d2 + c2 * d1;
            }

            fa[j] = a1 * b1 + a2 * b2 * r5;
            fb[j] = a1 * b2 + a2 * b1;
        }

        Fft(fa, sz);
        Fft(fb, sz);
        int[] res = new int[need];
        for (int i = 0; i < need; i++) {
            long aa = (long)(fa[i].Real + 0.5);
            long bb = (long)(fb[i].Real + 0.5);
            long cc = (long)(fa[i].Imaginary + 0.5);
            res[i] = (int)((aa + ((bb % m) << 15) + ((cc % m) << 30)) % m);
        }

        return res;
    }

    public static int[] SquareMod(int[] a, int m) => MultiplyMod(a, a, m, true);
}