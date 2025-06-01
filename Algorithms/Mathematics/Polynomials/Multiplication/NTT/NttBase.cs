using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Algorithms.Mathematics.Multiplication.NTT;

public abstract unsafe class NttBase
{
    readonly int elementBytes = 8;
    public long[] A, B;

    public NttBase(int maxsize)
    {
        int n = CeilingPowOfTwo(maxsize);
        A = new long[n];
        B = new long[n];
        Debug.Assert(elementBytes == Marshal.SizeOf(A.GetType().GetElementType()));
    }

    public long[] Multiply(long[] a, long[] b, int size, int mod = 998244353, int g = 3)
    {
        if (a.Length == 0 || b.Length == 0) return Array.Empty<long>();

        fixed (long* fa = a)
        fixed (long* fb = b)
        fixed (long* dest = A) {
            int resultSize = Multiply(fa, a.Length, fb, b.Length, dest, size, mod, g);
            long[] result = new long[resultSize];
            Array.Copy(A, 0, result, 0, resultSize);
            return result;
        }
    }

    public int Multiply(long* a, int alen, long* b, int blen, long* dest, int maxSize, int mod, int g)
    {
        if (maxSize == 0 || maxSize > alen + blen - 1) {
            maxSize = alen + blen - 1;
        } else {
            if (alen > maxSize) alen = maxSize;
            if (blen > maxSize) blen = maxSize;
        }

        int m = CeilingPowOfTwo(alen + blen - 1);
        fixed (long* fa = A)
        fixed (long* fb = a != b ? B : A) {
            Ntt(a, alen, m, fa, false, mod, g);
            if (a != b) Ntt(b, blen, m, fb, false, mod, g);
            for (int i = 0; i < m; i++)
                fa[i] = fa[i] * fb[i] % mod;
            Ntt(fa, m, m, dest, true, mod, g);
        }

        return maxSize;
    }

    public void Ntt(long[] a, int alen, int m, long[] dest, bool inverse, int mod, int g)
    {
        fixed (long* pa = a)
        fixed (long* pdest = dest) {
            Ntt(pa, alen, m, pdest, inverse, mod, g);
        }

        /*
        int copy = Math.Min(alen, m);
        if (a != dest) Array.Copy(a, 0, dest, 0, copy);
        if (m != copy) Array.Clear(dest, copy, m - copy);
        fixed (long *pd = dest)
            NttCore(m, pd, inverse, mod, g);
        */
    }

    public void Ntt(long* a, int alen, int m, long* dest, bool inverse, int mod, int g)
    {
        int copy = Math.Min(alen, m);
        if (a != dest) Buffer.MemoryCopy(a, dest, copy * elementBytes, copy * elementBytes);
        long* cur = dest + copy;
        long* end = dest + m;
        while (cur < end) *cur++ = 0;
        if (m >= 2)
            NttCore(m, dest, inverse, mod, g);
    }

    protected abstract void NttCore(int n, long* dst, bool inverse, int mod, int g);

    public long[] Exponentiate(long[] a, int p, int mod, int g)
    {
        // Exponentiation requires the polynomial to grow by a factor of p
        // because the curve produced by multiplication can't be described with fewer terms

        int m = CeilingPowOfTwo((a.Length - 1) * p + 1);
        long[] fa = new long[m];
        Ntt(a, a.Length, m, fa, false, mod, g);
        for (int i = 0; i < m; i++) {
            long v = fa[i];
            for (int j = 1; j < p; j++)
                fa[i] = fa[i] * v % mod;
        }

        Ntt(fa, fa.Length, m, fa, true, mod, g);
        return fa;
    }

    #region Helpers

    public static int CeilingPowOfTwo(int size) => size > 1 ? HighestOneBit(size - 1) << 1 : 1;

    public static long Invl(long a, long mod)
    {
        long b = mod, p = 1, q = 0;
        while (b > 0) {
            long c = a / b, d = a;
            a = b;
            b = d % b;
            d = p;
            p = q;
            q = d - c * q;
        }

        return p < 0 ? p + mod : p;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int Reverse(int value)
    {
        uint n = unchecked((uint)value);
        n = (n >> 16) | (n << 16);
        n = ((n >> 0x8) & 0x00ff00ff) | ((n << 0x8) & 0xff00ff00);
        n = ((n >> 0x4) & 0x0f0f0f0f) | ((n << 0x4) & 0xf0f0f0f0);
        n = ((n >> 0x2) & 0x33333333) | ((n << 0x2) & 0xcccccccc);
        n = ((n >> 0x1) & 0x55555555) | ((n << 0x1) & 0xaaaaaaaa);
        return unchecked((int)n);
    }

    #endregion
}