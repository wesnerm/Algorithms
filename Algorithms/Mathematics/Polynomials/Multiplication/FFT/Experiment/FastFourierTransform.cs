using System.Numerics;
using static System.Math;

namespace Algorithms.Mathematics;

public class FastFourierTransform
{
    readonly Complex[] aa;
    readonly Complex[] bb;
    readonly int Logm = 20;
    readonly int M = 600005;
    readonly int[] rev;
    readonly Complex[,] ww;
    int _revCalced = -1;

    public FastFourierTransform(int m = 600005)
    {
        M = m;
        Logm = (int)Ceiling(Log(M, 2));
        aa = new Complex[M];
        bb = new Complex[M];
        ww = new Complex[Logm, M];
        rev = new int[M];
    }

    void Swap<T>(ref T a, ref T b)
    {
        T tmp = a;
        a = b;
        b = tmp;
    }

    void Fft(Complex[] a, int n, bool invert = false)
    {
        for (int i = 0; i < n; ++i)
            if (i < rev[i])
                Swap(ref a[i], ref a[rev[i]]);

        for (int len = 2, k = 0; len <= n; len <<= 1, k++) {
            int len2 = len >> 1;

            for (int i = 0; i < n; i += len) {
                int pu = i;
                int pv = i + len2;
                int pu_end = i + len2;
                for (int pw = 0; pu != pu_end; pu++, pv++, pw++) {
                    Complex t = invert
                        ? a[pv] * new Complex(ww[k, pw].Real, -ww[k, pw].Imaginary)
                        : a[pv] * ww[k, pw];
                    a[pv] = a[pu] - t;
                    a[pu] = a[pu] + t;
                }
            }
        }

        if (invert)
            for (int i = 0; i < n; ++i)
                a[i] = a[i] / n;
    }

    public static int Reverse(int value)
    {
        unchecked {
            uint n = unchecked((uint)value);
            n = (n >> 16) | (n << 16);
            n = ((n >> 0x8) & 0x00ff00ff) | ((n << 0x8) & 0xff00ff00);
            n = ((n >> 0x4) & 0x0f0f0f0f) | ((n << 0x4) & 0xf0f0f0f0);
            n = ((n >> 0x2) & 0x33333333) | ((n << 0x2) & 0xcccccccc);
            n = ((n >> 0x1) & 0x55555555) | ((n << 0x1) & 0xaaaaaaaa);
            return unchecked((int)n);
        }
    }

    void Precalc(int n)
    {
        if (n == _revCalced)
            return;
        _revCalced = n;
        for (int i = 1, j = 0; i < n; i++) {
            int bit = n >> 1;
            for (; j >= bit; bit >>= 1)
                j -= bit;
            j += bit;
            rev[i] = j;
        }

        for (int len = 2, k = 1; len <= n; len <<= 1, k++) {
            int len2 = len >> 1;
            double ang = 2 * PI / len;
            for (int i = 0; i < len2; ++i)
                ww[k - 1, i] = new Complex(Cos(ang * i), Sin(ang * i));
        }
    }

    void Multiply2(int[] a, int[] b, int[] c, int sza, int szb)
    {
        int n = 1;
        while (n < sza + szb)
            n <<= 1;

        Debug.Assert(n < M);

        Precalc(n);
        for (int i = 0; i < n; ++i)
            aa[i] = i < sza ? a[i] : 0;
        Fft(aa, n);

        for (int i = 0; i < n; ++i)
            bb[i] = i < szb ? b[i] : 0;
        Fft(bb, n);

        for (int i = 0; i < n; ++i)
            aa[i] = aa[i] * bb[i];

        Fft(aa, n, true);

        for (int i = 0; i < n; ++i) c[i] = (int)(aa[i].Real + .5);
    }

    public void Multiply(int[] a, int[] b, int[] c, int sza, int szb)
    {
        if (Max(sza, szb) >= 1000)
            Multiply2(a, b, c, sza, szb);
        else
            MultiplyNaive(a, b, c, sza, szb);
    }

    public void MultiplyNaive(int[] a, int[] b, int[] c, int sza, int szb)
    {
        for (int i = sza + szb - 2; i >= 0; i--)
            c[i] = 0;
        for (int i = 0; i < sza; i++)
        for (int j = 0; j < szb; j++)
            c[i + j] += a[i] * b[j];
    }
}