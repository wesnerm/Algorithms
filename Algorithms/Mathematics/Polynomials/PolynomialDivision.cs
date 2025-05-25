namespace Algorithms.Mathematics;

using static Math;

// Polynomial Division and Greatest Common Divisors
// http://web.cs.iastate.edu/~cs577/handouts/polydivide.pdf

public class PolynomialDivision
{
    public static Func<long[], long[], int, long[]> Multiply;

    static long[] divInvrev;
    static long[] divDivisor;

    //static long[] leftBuffer = new long[1 << 18];
    //static long[] rightBuffer = new long[1 << 18];
    //static long[] retBuffer = new long[1 << 18];
    //public static long[] Invert(long[] poly, int n)
    //{
    //    Debug.Assert(poly != null && poly.Length > 0 && poly[0] != 0);
    //    Array.Clear(retBuffer, 0, n);

    //    fixed (long* ppoly = poly)
    //    fixed (long* left = leftBuffer)
    //    fixed (long* right = rightBuffer)
    //    fixed (long* ret = retBuffer)
    //    {
    //        ret[0] = Inverse(poly[0]);
    //        for (int i = 1; i < n; i <<= 1)
    //        {
    //            Array.Clear(leftBuffer, 0, 2 * i);
    //            Array.Clear(rightBuffer, 0, 2 * i);

    //            int leftLen = Multiply(ppoly, Min(i, poly.Length), ret, i, left, 2 * i);
    //            int rightLen = Multiply(ppoly + Min(i, poly.Length),
    //                Min(2 * i, poly.Length) - Min(i, poly.Length),
    //                ret, i, right, i);

    //            for (int j = 0; j < i - 1; ++j)
    //            {
    //                if (j + i >= leftLen) break;
    //                right[j] += left[j + i];
    //                if (right[j] >= MOD)
    //                    right[j] -= MOD;
    //            }

    //            int nextLen = Multiply(right, Min(i, rightLen), ret, i, left, i);
    //            for (int j = 0; j < i; ++j)
    //            {
    //                long t = ret[i + j] + MOD - left[j];
    //                if (t >= MOD) t -= MOD;
    //                ret[i + j] = t;
    //            }
    //        }
    //    }

    //    return GetRange(retBuffer, 0, n);
    //}

    public static long[] Invert(long[] poly, int n)
    {
        Debug.Assert(poly != null && poly.Length > 0 && poly[0] != 0);

        long[] ret = new long[2 * n];
        ret[0] = Inverse(poly[0]);

        for (int i = 1; i < n; i <<= 1) {
            long[] left = GetRange(poly, 0, Min(i, poly.Length), true);
            long[] right = GetRange(poly, Min(i, poly.Length), Min(2 * i, poly.Length) - Min(i, poly.Length), true);

            left = Multiply(left, ret, 2 * i);
            right = Multiply(right, ret, i);

            for (int j = 0; j < i - 1; ++j) {
                if (j + i >= left.Length) break;
                right[j] += left[j + i];
                if (right[j] >= MOD)
                    right[j] -= MOD;
            }

            right = Multiply(right, ret, i);
            for (int j = 0; j < i; ++j) {
                long t = ret[i + j] + MOD - right[j];
                if (t >= MOD) t -= MOD;
                ret[i + j] = t;
            }
        }

        return GetRange(ret, 0, n);
    }

    public static long[] InvertRev(long[] right, int n)
    {
        long[] invrev;
        if (right == divDivisor && divDivisor.Length >= n) {
            invrev = divInvrev;
        } else {
            long[] invrevOld = (long[])right.Clone();
            Array.Reverse(invrevOld);
            invrev = Invert(invrevOld, n);
            if (invrev.Length < n)
                invrev = GetRange(invrev, 0, n, true);
        }

        divDivisor = right;
        divInvrev = invrev;
        if (invrev.Length > 2 * n)
            invrev = GetRange(invrev, 0, n);
        return invrev;
    }

    public static long[] DivPolynomial(long[] left, long[] right)
    {
        if (right.Length > left.Length)
            return new long[1];

        int rsize = left.Length - right.Length + 1;
        long[] invrev = InvertRev(right, rsize);

        long[] q = (long[])left.Clone();
        Array.Reverse(q);
        q = Multiply(q, invrev, rsize);
        Array.Reverse(q);
        return q;
    }

    public static long[] ModPolynomial(long[] left, long[] right, long[] quotient = null)
    {
        if (right.Length > left.Length)
            return left;

        if (quotient == null)
            quotient = DivPolynomial(left, right);

        long[] r = (long[])left.Clone();
        long[] qright = Multiply(quotient, right, r.Length);
        for (int i = 0; i < qright.Length; i++)
            r[i] = (r[i] - qright[i] + MOD) % MOD;
        return PolynomialOperation.Trim(r);
    }
}