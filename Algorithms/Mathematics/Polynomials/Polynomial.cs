using static System.Math;
using T = long;

namespace Algorithms.Mathematics;

public static class Polynomial
{
    #region Basic

    public static double[] ShrinkPolynomial(double[] poly)
    {
        int n = poly.Length;
        while (n > 1 && poly[n - 1] == 0)
            n--;
        Array.Resize(ref poly, n);
        return poly;
    }

    public static bool IsZero(double[] poly)
    {
        for (int i = poly.Length - 1; i >= 0; i--)
            if (poly[i] != 0)
                return false;
        return true;
    }

    public static bool IsConstant(double[] poly, double c)
    {
        for (int i = poly.Length - 1; i >= 1; i--)
            if (poly[i] != 0)
                return false;
        return (poly.Length > 0 && poly[0] == c) || c == 0;
    }

    #endregion

    #region Addition and Subtraction

    public static long[] AddPolynomials(long[] a, long[] b, long[] result = null)
    {
        if (result == null) result = new long[Max(a.Length, b.Length)];
        if (result != a)
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i];
        for (int i = 0; i < b.Length; i++) result[i] += b[i];
        return result;
    }

    public static long[] SubtractPolynomials(long[] a, long[] b, long[] result = null)
    {
        if (result == null) result = new long[Max(a.Length, b.Length)];
        if (result != a)
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i];
        for (int i = 0; i < b.Length; i++) result[i] -= b[i];
        return result;
    }

    public static double[] AddPolynomials(double[] a, double[] b, double[] result = null)
    {
        if (result == null) result = new double[Max(a.Length, b.Length)];
        if (result != a)
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i];
        for (int i = 0; i < b.Length; i++) result[i] += b[i];
        return result;
    }

    public static double[] SubtractPolynomials(double[] a, double[] b, double[] result = null)
    {
        if (result == null) result = new double[Max(a.Length, b.Length)];
        if (result != a)
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i];
        for (int i = 0; i < b.Length; i++) result[i] -= b[i];
        return result;
    }

    #endregion

    #region Calculus

    public static double[] IntegratePolynomials(double[] a)
    {
        double[] result = new double[a.Length + 1];
        for (int i = 0; i < a.Length; i++) result[i + 1] = a[i] / (i + 1);
        return result;
    }

    public static double[] Differentiate(double[] a)
    {
        double[] result = new double[a.Length - 1];
        for (int i = 1; i < a.Length; i++) result[i - 1] = i * a[i];
        return result;
    }

    #endregion

    #region Multiplication / Convolution

    public static double Evaluate(double[] poly, double x)
    {
        if (poly.Length == 0) return 0;

        double result = poly[poly.Length - 1];
        for (int i = poly.Length - 2; i >= 0; i--)
            result = x * result + poly[i];
        return result;
    }

    public static long[] MultiplyPolynomials(long[] a, long[] b, int size = 0)
    {
        if (size == 0) size = a.Length + b.Length - 1;
        size = Max(0, Min(a.Length + b.Length - 1, size));
        long[] result = new long[size];
        for (int i = 0; i < a.Length; i++)
        for (int j = Min(size - i, b.Length) - 1; j >= 0; j--)
            result[i + j] += a[i] * b[j];
        return result;
    }

    public static long[] MultiplyPolynomialsMod2(long[] a, long[] b, int size = 0)
    {
        size = size > 0 ? Min(a.Length + b.Length - 1, size) : a.Length + b.Length - 1;
        long[] result = new long[size];
        for (int i = 0; i < result.Length; i++) {
            long r = 0;
            int limit = Min(a.Length, i + 1);
            for (int j = Max(0, i - b.Length + 1); j < limit; j++)
                r += a[j] * b[i - j];
            result[i] = r;
        }

        return result;
    }

    public static int[] MultiplyPolynomialsMod(int[] a, int[] b, int MOD, int size = 0)
    {
        if (size == 0) size = a.Length + b.Length - 1;
        size = Min(a.Length + b.Length - 1, size);
        if (size <= 0) return Array.Empty<int>();

        int[] result = new int[size];
        for (int i = 0; i < a.Length; i++)
        for (int j = Min(size - i, b.Length) - 1; j >= 0; j--)
            result[i + j] = (int)((result[i + j] + (long)a[i] * b[j]) % MOD);
        return result;
    }

    public static long[] MultiplyPolynomialsMod(long[] a, long[] b, int MOD, int size = 0)
    {
        if (size == 0) size = a.Length + b.Length - 1;
        size = Min(a.Length + b.Length - 1, size);
        if (size <= 0) return Array.Empty<long>();

        long[] result = new long[size];
        for (int i = 0; i < a.Length; i++)
        for (int j = Min(size - i, b.Length) - 1; j >= 0; j--) {
            long r = (result[i + j] + a[i] * b[j]) % MOD;
            // if (r >= mod) r -= mod;
            result[i + j] = r;
        }

        return result;
    }

    #region Optimized Naive

    public static long[] MultiplyPolynomialsModFast(long[] a, long[] b, int MOD, int size = 0)
    {
        unchecked {
            size = size > 0 ? Min(a.Length + b.Length - 1, size) : a.Length + b.Length - 1;
            long[] result = new long[size];
            long chop = long.MaxValue / MOD * MOD;
            for (int i = 0; i < result.Length; i++) {
                long r = 0;
                int limit = Min(a.Length, i + 1);
                for (int j = Max(0, i - b.Length + 1); j < limit; j++) {
                    r += a[j] * b[i - j];
                    if (r < 0) r -= chop;
                }

                r %= MOD;
                result[i] = r;
            }

            return result;
        }
    }

    public static long[] MultiplyPolynomialsModFastest(long[] a, long[] b, int MOD, int size = 0)
    {
        unchecked {
            size = size > 0 ? Min(a.Length + b.Length - 1, size) : a.Length + b.Length - 1;
            long[] result = new long[size];
            for (int i = 0; i < result.Length; i++) {
                long r = 0;
                int limit = Min(a.Length, i + 1);
                for (int j = Max(0, i - b.Length + 1); j < limit;) {
                    for (int limit2 = Min(j + 18, limit); j < limit2; j++)
                        r += a[j] * b[i - j];
                    r = (long)((ulong)r % (ulong)MOD);
                }

                result[i] = r;
            }

            return result;
        }
    }

    #endregion

    public static long[] PolyPow(long[] x, long n, Func<long[], long[], long[]> multiply)
    {
        if (n <= 1) return n == 1 ? x : new long[] { 1 };
        long[] t = PolyPow(x, n >> 1, multiply);
        long[] sq = multiply(t, t);
        return (n & 1) == 0 ? sq : multiply(x, sq);
    }

    public static T ConvolutionTerm(T[] a, T[] b, int term, int mod = 1000000007)
    {
        if (a.Length > b.Length)
            return ConvolutionTerm(b, a, term, mod);

        long sum = 0;
        for (int i = Min(a.Length - 1, term); i >= 0; i--) {
            int j = term - i;
            if (j >= b.Length) break;
            sum += a[i] * b[j] % mod;
        }

        return sum % mod;
    }

    // https://ideone.com/uQYLfQ
    // https://discuss.codechef.com/questions/127380/sersum-editorial-reupload
    // http://codeforces.com/blog/entry/12513

    public unsafe class AdvancePolyMath
    {
        static long[] nra;
        static long[] nrsa;
        static long[] rsa;
        static int mod;

        void poly_mulTo(int n, long* f, long* g)
        {
            long* c = stackalloc long[n + n];
            for (int i = 0; i < n; i++) c[i] = 0;
            for (int i = 0; i < n; i++)
            for (int j = 0; i + j < n; j++)
                c[i + j] = (c[i + j] + f[i] * g[j]) % mod;
            copy(c, c + n, f);
        }

        void copy(long* start, long* limit, long* result)
        {
            while (start < limit)
                *result++ = *start++;
        }

        void fill(long* start, long* limit, long value)
        {
            while (start < limit)
                *start++ = value;
        }

        public void InvertPolynomial(int n, long* f, long* r)
        {
            //R_2n(z) ≡ 2R_n(z) - Rn(z)^2 F(z) (mod z^2n)

            Debug.Assert(f[0] == 1);
            fill(r, r + n, 0);
            r[0] = 1;

            fixed (long* nr = nra) {
                for (int m = 2; m <= n; m <<= 1) {
                    int h = m >> 1;
                    copy(f, f + m, nr);
                    poly_mulTo(m, nr, r);
                    fill(nr, nr + h, 0);
                    for (int i = h; i < m; i++) nr[i] = -nr[i];
                    poly_mulTo(m, nr, r);
                    copy(nr + h, nr + m, r + h);
                }
            }
        }

        // https://s3.amazonaws.com/codechef_shared/download/Solutions/MAY18/setter/SERSUM.cpp
        public long[] Invert(long[] a, int n, int mod)
        {
            if (n == 1)
                return new[] { ModularMath.ModInverse(a[0], mod) };

            int nn = (n + 1) >> 1;
            long[] b0 = Invert(a, nn, mod);
            long[] b = new long[n];
            Array.Copy(b0, 0, b, 0, nn);
            long[] c = MultiplyPolynomialsMod(b0, b0, mod);
            long[] ac = MultiplyPolynomialsMod(a, c, mod);
            for (int i = 0; i < n; i++) {
                b[i] = 2 * b[i] - ac[i];
                if (b[i] >= mod) b[i] -= mod;
                if (b[i] < 0) b[i] += mod;
            }

            return b;
        }

        //http://www.ams.org/journals/mcom/2011-80-273/S0025-5718-2010-02392-0/S0025-5718-2010-02392-0.pdf

        // Fast Algorithms for Manipulating Formal Power Series 
        // Includes Reversion and Composition
        // http://www.eecs.harvard.edu/~htk/publication/1978-jacm-brent-kung.pdf

        // Compositional inverse
        // http://www.math.ucsd.edu/~jverstra/264A-LECTUREB.pdf

        public void SqrtPolynomial(int n, long* f, long* s)
        {
            // S_2n(x) = 1/2(S_n(z) + F(z) * S_n(z)^-1) mod z^2n

            // assert(f[0] == 1)
            fill(s, s + n, 0);
            s[0] = 1;

            int div2 = (mod + 1) / 2;

            fixed (long* rs = rsa, nrs = nrsa) {
                fill(rs, rs + n, 0);
                rs[0] = 1;
                for (int m = 2; m <= n; m <<= 1) {
                    int h = m / 2;

                    fill(nrs + h, nrs + m, 0);

                    copy(s, s + h, nrs);
                    poly_mulTo(m, nrs, rs);
                    fill(nrs, nrs + h, 0);
                    for (int i = h; i < m; i++) nrs[i] = -nrs[i];
                    poly_mulTo(m, nrs, rs);
                    copy(rs, rs + h, nrs);

                    poly_mulTo(m, nrs, f);
                    for (int i = h; i < m; i++)
                        s[i] = nrs[i] * div2 % mod;

                    copy(s, s + m, nrs);
                    poly_mulTo(m, nrs, rs);
                    fill(nrs, nrs + h, 0);
                    for (int i = h; i < m; i++) nrs[i] = -nrs[i];
                    poly_mulTo(m, nrs, rs);
                    copy(nrs + h, nrs + m, rs + h);
                }
            }
        }
    }

    #endregion

    #region Division

    static double[] DividePolynomialCore(
        double[] poly,
        double[] divisor,
        bool returnQ,
        out double[] remainder)
    {
        poly = ShrinkPolynomial(poly);
        divisor = ShrinkPolynomial(divisor);

        if (poly.Length < divisor.Length) {
            remainder = poly;
            return returnQ ? new double[] { 0 } : null;
        }

        double[] p = poly.ToArray();
        int deg = divisor.Length - 1;
        double[] q = null;
        int qshift = 0;

        if (returnQ) {
            q = new double[poly.Length - deg];
            qshift = q.Length - poly.Length;
        }

        double f0 = divisor[deg];
        for (int i = poly.Length - 1; i >= deg; i--) {
            if (p[i] == 0) continue;
            double d = p[i] / f0;
            if (q != null) q[i + qshift] = d;
            p[i] = 0;
            for (int j = 1; j < deg; j++)
                p[i - j] -= d * divisor[deg - j];
        }

        remainder = p;
        return q;
    }

    public static double[] DividePolynomial(double[] dividend, double[] divisor)
    {
        double[] remainder;
        double[] quotient = DividePolynomialCore(dividend, divisor, true, out remainder);
        return quotient;
    }

    public static double[] DividePolynomial(double[] dividend, double[] divisor,
        out double[] remainder)
    {
        double[] quotient = DividePolynomialCore(dividend, divisor, true, out remainder);
        remainder = ShrinkPolynomial(remainder);
        return quotient;
    }

    public static double[] ModPolynomial(double[] dividend, double[] divisor)
    {
        double[] remainder;
        DividePolynomialCore(dividend, divisor, false, out remainder);
        remainder = ShrinkPolynomial(remainder);
        return remainder;
    }

    public static List<double[]> SquareFreeFactorization(double[] b)
    {
        double[] d = Differentiate(b);
        var list = new List<double[]>();
        double[] a, c;
        do {
            a = GcdPolynomial(b, d);
            b = DividePolynomial(b, a);
            c = DividePolynomial(d, a);
            d = SubtractPolynomials(c, Differentiate(b));
            list.Add(a);
        } while (!IsConstant(b, 1));

        return list;
    }

    public static double[] GcdPolynomial(double[] a, double[] b)
    {
        while (true) {
            if (IsZero(a)) return b;
            b = ModPolynomial(b, a);
            if (IsZero(b)) return a;
            a = ModPolynomial(a, b);
        }
    }

    // Partial Fractions Decomposition
    // Convergence

    #endregion
}