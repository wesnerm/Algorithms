namespace Algorithms.Mathematics.Multiplication;

public class KaratsubaMultiplication
{
    // Karatsuba Multiplication
    // a = p[0,m)
    // b = q[0,m)
    // c = p[m,n)
    // d = q[m,n]

    // hi = c * d
    // lo = a * b

    // rem2 = (a+c)*(b+d)
    // (a+c)*(b+d) - ab - cd = ab + ad + bc + cd - ab - cd = ad + bc

    public static long[] Karatsuba(ReadOnlySpan<long> a, ReadOnlySpan<long> b, int size = 0)
    {
        int expSize = Math.Max(0, a.Length + b.Length - 1);
        if (size == 0 || expSize < size) size = expSize;
        long[] result = new long[size];
        KaratsubaCore(result,
            a.Slice(0, Math.Min(size, a.Length)),
            b.Slice(0, Math.Min(size, b.Length)));
        return result;
    }

    static void KaratsubaCore(Span<long> result, ReadOnlySpan<long> p, ReadOnlySpan<long> q)
    {
        int resultLen = result.Length;
        if (p.Length < q.Length)
        {
            var t = p;
            p = q;
            q = t;
        }

        if (q.Length == 0 || result.Length == 0)
            return;

        int j;
        int n = p.Length;
        if (n <= 35 || result.Length <= 35)
        {
            for (int i = 0; i < p.Length; i++)
            {
                int end = Math.Min(q.Length, resultLen - i);
                for (j = 0; j < end; j++)
                    result[i + j] += p[i] * q[j];
            }
            return;
        }

        int m = (n + 1) >> 1;

        if (q.Length <= m)
        {
            KaratsubaCore(result, p.Slice(0, Math.Min(m, p.Length)), q);
            KaratsubaCore(result.Slice(m), p.Slice(m), q);
            return;
        }

        Span<long> span = new long[m * 2];
        Span<long> tmp = span.Slice(0, m * 2 - 1);

        KaratsubaCore(tmp,
            p.Slice(0, Math.Min(m, p.Length)),
            q.Slice(0, Math.Min(m, q.Length)));

        for (int i = Math.Min(resultLen, m * 2 - 1) - 1; i >= 0; i--)
            result[i] += tmp[i];

        if (result.Length <= m)
            return;

        for (int i = Math.Min(resultLen - m, m * 2 - 1) - 1; i >= 0; i--)
            result[m + i] -= tmp[i];

        tmp.Clear();
        KaratsubaCore(tmp, p.Slice(Math.Min(p.Length, m)), q.Slice(Math.Min(q.Length, m)));

        for (int i = Math.Min(resultLen - m, tmp.Length) - 1; i >= 0; i--)
            result[m + i] -= tmp[i];
        for (int i = Math.Min(resultLen - m * 2, tmp.Length) - 1; i >= 0; i--)
            result[m * 2 + i] += tmp[i];

        var f1 = span.Slice(0, m);
        var f2 = span.Slice(m, m);
        p.Slice(0, m).CopyTo(f1);
        q.Slice(0, m).CopyTo(f2);
        for (j = Math.Min(p.Length, n) - 1; j >= m; j--)
            f1[j - m] += p[j];
        for (j = Math.Min(q.Length, n) - 1; j >= m; j--)
            f2[j - m] += q[j];

        KaratsubaCore(result.Slice(m, Math.Min(tmp.Length, result.Length - m)), f1, f2);
    }
}