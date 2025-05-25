namespace Algorithms.Mathematics;

// fft code taken from e-maxx.ru/algo/fft_multiply

public class NumberTheoreticTransform
{
    const int Root = 3;
    const int Root1 = 332748118;
    const int RootPw = 1 << 20;
    const int MOD = 998244353;

    public void Fft(long[] a, bool invert)
    {
        int n = a.Length;
        for (int i = 1, j = 0; i < n; ++i) {
            int bit = n >> 1;
            for (; j >= bit; bit >>= 1)
                j -= bit;
            j += bit;
            if (i < j) {
                long tmp = a[i];
                a[i] = a[j];
                a[j] = tmp;
            }
        }

        for (int len = 2; len <= n; len <<= 1) {
            long wlen = invert ? Root1 : Root;
            for (int i = len; i < RootPw; i <<= 1)
                wlen = wlen * wlen % MOD;
            for (int i = 0; i < n; i += len) {
                long w = 1;
                for (int j = 0; j < len / 2; ++j) {
                    long u = a[i + j];
                    long v = a[i + j + len / 2] * w % MOD;
                    a[i + j] = u + v < MOD ? u + v : u + v - MOD;
                    a[i + j + len / 2] = u - v >= 0 ? u - v : u - v + MOD;
                    w = w * wlen % MOD;
                }
            }
        }

        if (invert) {
            long ninv = ModPow(n, MOD - 2, MOD);
            for (int i = 0; i < n; ++i)
                a[i] = a[i] * ninv % MOD;
        }
    }

    void Multiply(long[] a, long[] b, out long[] c)
    {
        int sz = 2 * a.Length;
        long[] ta = new long[sz];
        long[] tb = new long[sz];
        a.CopyTo(ta, 0);
        b.CopyTo(tb, 0);
        Fft(ta, false);
        Fft(tb, false);
        for (int i = 0; i < sz; ++i)
            ta[i] = 1L * ta[i] * tb[i] % MOD;
        Fft(ta, true);
        c = ta;
    }
}

// Produces Incorrect Results V
public class NumberTheoreticTransform2
{
    const int Root = 3;
    const int Root1 = 332748118;
    const int RootPw = 1 << 20;
    const int MOD = 998244353;

    static readonly long[] abuffer = new long[1 << 20];
    static readonly long[] bbuffer = new long[1 << 20];

    public static void Fft(long[] a, int n, bool invert)
    {
        for (int i = 1, j = 0; i < n; ++i) {
            int bit = n >> 1;
            for (; j >= bit; bit >>= 1)
                j -= bit;
            j += bit;
            if (i < j) {
                long tmp = a[i];
                a[i] = a[j];
                a[j] = tmp;
            }
        }

        for (int len = 2; len <= n; len <<= 1) {
            long wlen = invert ? Root1 : Root;
            for (int i = len; i < RootPw; i <<= 1)
                wlen = wlen * wlen % MOD;
            for (int i = 0; i < n; i += len) {
                long w = 1;
                for (int j = 0; j < len >> 1; ++j) {
                    long u = a[i + j];
                    long v = a[i + j + (len >> 1)] * w % MOD;
                    a[i + j] = u + v < MOD ? u + v : u + v - MOD;
                    a[i + j + (len >> 1)] = u - v >= 0 ? u - v : u - v + MOD;
                    w = w * wlen % MOD;
                }
            }
        }

        if (invert) {
            long ninv = ModPow(n, MOD - 2, MOD);
            for (int i = 0; i < n; ++i)
                a[i] = a[i] * ninv % MOD;
        }
    }

    static int NttSize(int size)
    {
        // This is actually the lowest power of two that is >= size
        int nttSize = size > 1 ? HighestOneBit(size - 1) << 2 : 2;
        return nttSize;
    }

    public static long[] Multiply(long[] a, long[] b, int size = 0)
    {
        if (a == null || b == null) return null;
        if (a.Length > b.Length)
            Swap(ref a, ref b);

        if (a.Length == 1) {
            if (a[0] == 1) return b;
            if (a[0] == 0) return a;
        }

        int productSize = a.Length + b.Length - 1;
        int alen = a.Length;
        int blen = b.Length;
        if (size == 0 || productSize < size) {
            size = productSize;
        } else {
            if (alen > size) alen = size;
            if (blen > size) blen = size;
        }

        int sz = NttSize(size);
        Array.Copy(a, 0, abuffer, 0, alen);
        Array.Copy(b, 0, bbuffer, 0, blen);
        Array.Clear(abuffer, alen, sz - alen);
        Array.Clear(bbuffer, blen, sz - blen);
        Fft(abuffer, sz, false);
        Fft(bbuffer, sz, false);
        for (int i = 0; i < sz; ++i)
            abuffer[i] = abuffer[i] * bbuffer[i] % MOD;
        Fft(abuffer, sz, true);

        long[] result = new long[size];
        Array.Copy(abuffer, 0, result, 0, size);
        return result;
    }
}