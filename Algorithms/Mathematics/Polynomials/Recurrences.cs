using T = long;

namespace Algorithms.Mathematics;

public static class Recurrences
{
    public static List<long> BerkelkampMassey(List<long> x, long MOD)
    {
        var ls = new List<long>();
        var cur = new List<long>();
        int lf = 0;
        long ld = 0;
        for (int i = 0; i < x.Count; ++i) {
            long t = -x[i] % MOD;
            for (int j = 0; j < cur.Count; ++j)
                t = (t + x[i - j - 1] * cur[j]) % MOD;

            if (t == 0)
                continue;

            if (cur.Count == 0) {
                while (cur.Count <= i)
                    cur.Add(0);
                lf = i;
                ld = t;
                continue;
            }

            long k = -t * Inverse(ld) % MOD;
            var c = new List<long>(i - lf - 1);
            c.Add(-k);

            for (int j = 0; j < ls.Count; ++j)
                c.Add(ls[j] * k % MOD);

            while (c.Count < cur.Count)
                c.Add(0);

            for (int j = 0; j < cur.Count; ++j) c[j] = (c[j] + cur[j]) % MOD;

            if (i - lf + ls.Count >= cur.Count) {
                ls = cur;
                lf = i;
                ld = t;
            }

            cur = c;
        }

        for (int i = 0; i < cur.Count; ++i)
            cur[i] = (cur[i] % MOD + MOD) % MOD;

        return cur;
    }

    static long[] RecurrenceMult(long[] left, long[] right, long[] a)
    {
        int n = a.Length;

        long[] result = new long[n];
        for (int i = 0; i < n; ++i) {
            for (int j = 0; j < n; ++j)
                result[j] += left[i] * right[j];

            if ((i & 7) == 7)
                for (int j = 0; j < n; ++j)
                    result[j] %= MOD;

            long mul = right[right.Length - 1];
            for (int j = n - 1; j > 0; --j)
                right[j] = right[j - 1];

            right[0] = 0;
            for (int j = 0; j < n; ++j)
                right[j] = (right[j] + mul * a[j]) % MOD;
        }

        for (int i = 0; i < n; ++i)
            result[i] %= MOD;
        return result;
    }

    public static long RecurrencePow(long[] a, long[] c, long n)
    {
        int k = c.Length;
        if (k == 1)
            return c[0] * ModPow(a[0], n) % MOD;

        long[] res = new long[k];
        res[0] = 1;

        long[] b = new long[k];
        b[1] = 1;

        while (n != 0) {
            if ((n & 1) != 0)
                res = RecurrenceMult(res, b, a);
            b = RecurrenceMult(b, b, a);
            n >>= 1;
        }

        long result = 0;
        for (int i = 0; i < c.Length; ++i)
            result = (result + c[i] * res[i]) % MOD;
        return result;
    }

    #region Recurrences

    public static T[,] RecurrenceMatrix(T[] coefficients)
    {
        int n = coefficients.Length;
        T[,] result = new T[n, n];
        result[0, 0] = coefficients[0];
        for (int i = 1; i < n; i++) {
            result[0, i] = coefficients[i];
            result[i, i - 1] = 1;
        }

        return result;
    }

    public static T[,] RecurrenceSumMatrix(T[] coefficients)
    {
        int n = coefficients.Length;
        T[,] result = new T[n + 1, n + 1];
        result[0, 0] = coefficients[0];
        for (int i = 1; i < n; i++) {
            result[0, i] = coefficients[i];
            result[i, i - 1] = 1;
        }

        result[n, n] = 1;
        for (int i = 0; i < n; i++) result[n, i] = coefficients[i];
        return result;
    }

    public static T[,] SumMatrix(T[,] m)
    {
        int n = m.GetLength(0);
        T[,] result = new T[n + 1, n + 1];

        for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            result[i, j] = m[i, j];

        for (int i = 0; i < n; i++)
            result[n, i] = m[0, i];

        result[n, n] = 1;
        return result;
    }

    public static T[,] RecurrenceMatrixWithConstant(T[] coefficients)
    {
        int n = coefficients.Length;
        T[,] result = new T[n + 1, n + 1];
        result[0, n] = result[n, n] = 1;
        result[0, 0] = coefficients[0];
        for (int i = 1; i < n; i++) {
            result[0, i] = coefficients[i];
            result[i, i - 1] = 1;
        }

        return result;
    }

    public static T[,] CombineMatrices(T[,] a, T[,] b)
    {
        int am = a.GetLength(0);
        int an = a.GetLength(1);
        int bm = b.GetLength(0);
        int bn = b.GetLength(1);

        T[,] result = new T[am + bm, bn + bm];

        for (int i = 0; i < am; i++)
        for (int j = 0; j < an; j++)
            result[i, j] = a[i, j];

        for (int i = 0; i < bm; i++)
        for (int j = 0; j < bn; j++)
            result[am + i, an + j] = b[i, j];

        return result;
    }

    #endregion
}