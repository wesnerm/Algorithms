using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Algorithms.Mathematics.Multiplication.NTT;

public abstract class NttBase
{
    public long[] A, B;

    public NttBase(int maxsize)
    {
        int n = CeilingPowOfTwo(maxsize);
        A = new long[n];
        B = new long[n];
    }

    public long[] Multiply(ReadOnlySpan<long> a, ReadOnlySpan<long> b, int size, int mod = 998244353, int g = 3)
    {
        if (a.Length == 0 || b.Length == 0) return Array.Empty<long>();
        int resultSize = Multiply(a, b, A, size, mod, g);
        long[] result = new long[resultSize];
        Array.Copy(A, 0, result, 0, resultSize);
        return result;
    }

    public int Multiply(ReadOnlySpan<long> a, ReadOnlySpan<long> b, Span<long> dest, int maxSize, int mod, int g)
    {
        if (maxSize == 0 || maxSize > a.Length + b.Length - 1)
        {
            maxSize = a.Length + b.Length - 1;
        }
        else
        {
            if (a.Length > maxSize) a = a.Slice(0, maxSize);
            if (b.Length > maxSize) b = b.Slice(0, maxSize);
        }

        int m = CeilingPowOfTwo(a.Length + b.Length - 1);
        var fa = new Span<long>(A, 0, m);
        var fb = a != b ? new Span<long>(B, 0, m) : A;
        Ntt(a, fa, false, mod, g);
        if (a != b) Ntt(b, fb, false, mod, g);
        for (int i = 0; i < m; i++)
            fa[i] = fa[i] * fb[i] % mod;
        Ntt(fa, dest.Slice(0, m), true, mod, g);
        return maxSize;
    }

    public void Ntt(ReadOnlySpan<long> a, Span<long> dest, bool inverse, int mod, int g)
    {
        int m = dest.Length;
        int copy = Math.Min(a.Length, m);
        if (a.Slice(0, copy) != dest.Slice(0, copy))
        {
            a.Slice(0, copy).CopyTo(dest);
        }

        dest.Slice(copy).Fill(0);
        if (m >= 2)
            NttCore(dest, inverse, mod, g);
    }

    protected abstract void NttCore(Span<long> dst, bool inverse, int mod, int g);

    public long[] Exponentiate(ReadOnlySpan<long> a, int p, int mod, int g)
    {
        // Exponentiation requires the polynomial to grow by a factor of p
        // because the curve produced by multiplication can't be described with fewer terms

        int m = CeilingPowOfTwo((a.Length - 1) * p + 1);
        long[] fa = new long[m];
        Ntt(a, fa, false, mod, g);
        for (int i = 0; i < m; i++)
        {
            long v = fa[i];
            for (int j = 1; j < p; j++)
                fa[i] = fa[i] * v % mod;
        }

        Ntt(fa, fa, true, mod, g);
        return fa;
    }

    #region Helpers

    public static int CeilingPowOfTwo(int size) => size > 1 ? HighestOneBit(size - 1) << 1 : 1;

    public static long Invl(long a, long mod)
    {
        long b = mod, p = 1, q = 0;
        while (b > 0)
        {
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