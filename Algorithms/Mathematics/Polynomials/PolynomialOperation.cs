using static System.Math;

namespace Algorithms.Mathematics;

public static class PolynomialOperation
{
    static long[] divInvrev;
    static long[] divDivisor;

    // Relax, dont be lazy: https://core.ac.uk/download/pdf/81995869.pdf

    public static long[] Invert(long[] poly, int n, long mod)
    {
        Debug.Assert(poly != null && poly.Length > 0 && poly[0] != 0);

        long[] ret = new long[2 * n];
        ret[0] = Inverse(poly[0]);

        for (int i = 1; i < n; i <<= 1) {
            long[] left = GetSubrange(poly, 0, Min(i, poly.Length), true);
            long[] right = GetSubrange(poly, Min(i, poly.Length), Min(2 * i, poly.Length) - Min(i, poly.Length), true);

            left = MultiplyPolynomialsMod(left, ret, mod, 2 * i);
            right = MultiplyPolynomialsMod(right, ret, mod, i);

            for (int j = 0; j < i - 1; ++j) {
                if (j + i >= left.Length) break;
                right[j] += left[j + i];
                if (right[j] >= mod)
                    right[j] -= mod;
            }

            right = MultiplyPolynomialsMod(right, ret, mod, i);
            for (int j = 0; j < i; ++j) {
                long t = ret[i + j] + mod - right[j];
                if (t >= mod) t -= mod;
                ret[i + j] = t;
            }
        }

        return GetSubrange(ret, 0, n);
    }

    public static long[] InvertRev(long[] right, int n, long mod)
    {
        long[] invrev;
        if (right == divDivisor && divDivisor.Length >= n) {
            invrev = divInvrev;
        } else {
            long[] invrevOld = (long[])right.Clone();
            Array.Reverse(invrevOld);
            invrev = Invert(invrevOld, n, mod);
            if (invrev.Length < n)
                invrev = GetSubrange(invrev, 0, n, true);
        }

        divDivisor = right;
        divInvrev = invrev;
        if (invrev.Length > 2 * n)
            invrev = GetSubrange(invrev, 0, n);
        return invrev;
    }

    public static long[] DivPolynomial(long[] left, long[] right, long mod)
    {
        if (right.Length > left.Length)
            return new long[1];

        int rsize = left.Length - right.Length + 1;
        long[] invrev = InvertRev(right, rsize, mod);

        long[] q = (long[])left.Clone();
        Array.Reverse(q);
        q = MultiplyPolynomialsMod(q, invrev, MOD, rsize);
        Array.Reverse(q);
        return q;
    }

    public static long[] ModPolynomial(long[] left, long[] right, long mod, long[] quotient = null)
    {
        if (right.Length > left.Length)
            return left;

        if (quotient == null)
            quotient = DivPolynomial(left, right, mod);

        long[] r = (long[])left.Clone();
        long[] qright = MultiplyPolynomialsMod(quotient, right, mod, r.Length);
        for (int i = 0; i < qright.Length; i++)
            r[i] = (r[i] - qright[i] + MOD) % MOD;
        return Trim(r);
    }

    public static long[] MultiplyPolynomialsMod(long[] a, long[] b, long mod, int size = 0)
    {
        if (size == 0) size = a.Length + b.Length - 1;
        size = Min(a.Length + b.Length - 1, size);
        long[] result = new long[size];
        for (int i = 0; i < a.Length; i++)
        for (int j = Min(size - i, b.Length) - 1; j >= 0; j--) {
            long r = (result[i + j] + a[i] * b[j]) % mod;
            // if (r >= mod) r -= mod;
            result[i + j] = r;
        }

        return result;
    }

    public static int[] MultiplyPolynomialsMod(int[] a, int[] b, int mod, int size = 0)
    {
        if (size == 0) size = a.Length + b.Length - 1;
        size = Min(a.Length + b.Length - 1, size);
        int[] result = new int[size];
        for (int i = 0; i < a.Length; i++)
        for (int j = Min(size - i, b.Length) - 1; j >= 0; j--)
            result[i + j] = (int)((result[i + j] + (long)a[i] * b[j]) % mod);

        return result;
    }

    /// <summary>
    ///     Given initial values, A, and coefficients of a recurrence, C, such that
    ///     An = A[n-1]*C1 + An[n-2]*C2 + ... + A[1]*CN
    ///     Compute A[K] in O( n^2 lg k )
    /// </summary>
    /// <param name="A"></param>
    /// <param name="c"></param>
    /// <param name="K"></param>
    /// <returns></returns>
    public static int GFPow(int[] A, int[] c, long K, int MOD)
    {
        K--;
        long k = K;
        int[] a = A;

        if (k < a.Length)
            return a[k];

        long[] poly = new long[c.Length + 1];
        poly[0] = 1;
        for (int i = 1; i <= c.Length; i++)
            poly[i] = MOD - c[i - 1];

        while (k >= a.Length) {
            long[] negpoly = NegateX(poly, MOD);
            long[] combine = MultiplyPolynomialsMod(poly, negpoly, MOD);
            long[] sqrpoly = new long[poly.Length];
            for (int i = 0; i < combine.Length; i += 2)
                sqrpoly[i >> 1] = combine[i];

            int[] a2 = new int[2 * a.Length];
            Array.Copy(a, 0, a2, 0, a.Length);
            for (int i = a.Length; i < a2.Length; i++) {
                long tmp = 0L;
                for (int j = 0; j < c.Length; j++) tmp = (tmp + (MOD - poly[j + 1]) * a2[i - 1 - j]) % MOD;
                a2[i] = (int)tmp;
            }

            int[] newA = new int[a.Length];
            for (int i = 0, j = (int)(k & 1); i < newA.Length && j < a2.Length; i++, j += 2) newA[i] = a2[j];

            a = newA;
            poly = sqrpoly;
            k >>= 1;
        }

        return a[k];
    }

    static long[] NegateX(long[] poly, long MOD)
    {
        long[] result = (long[])poly.Clone();
        for (int i = 1; i < result.Length; i += 2)
            result[i] = MOD - result[i];
        return result;
    }

    /// <summary>
    ///     Given initial values, A, and coefficients of a recurrence, C, such that
    ///     An = A[n-1]*C1 + An[n-2]*C2 + ... + A[1]*CN
    ///     Compute A[K] in O( M(n) lg k )
    /// </summary>
    /// <param name="A"></param>
    /// <param name="c"></param>
    /// <param name="K"></param>
    /// <returns></returns>
    public static long GFPowFast(long[] a, long[] c, int k, long mod)
    {
        long[] b = GetCoefficientsForPower(c, k, mod);
        return ComputerPowerGivenInitialValues(a, b, mod);
    }

    public static long[] GetCoefficientsForPower(long[] coefs, int k, long MOD)
    {
        long[] m = new long[coefs.Length + 1];
        m[coefs.Length] = 1;
        for (int i = 0; i < coefs.Length; i++)
            m[i] = MOD - coefs[coefs.Length - 1 - i];

        long[] b = new long[] { 1 };
        long[] x = new long[] { 0, 1 };
        for (int i = 1 << 29; i > 0; i >>= 1) {
            if (2 * i <= k) b = ModPolynomial(MultiplyPolynomialsMod(b, b, MOD), m, MOD);
            if ((k & i) != 0) b = ModPolynomial(MultiplyPolynomialsMod(b, x, MOD), m, MOD);
        }

        Array.Resize(ref b, coefs.Length);
        return b;
    }

    public static long ComputerPowerGivenInitialValues(long[] a, long[] b, long MOD)
    {
        long tmp = 0L;
        for (int j = 0; j < b.Length; j++)
            tmp = (tmp + b[j] * a[j]) % MOD;
        return tmp;
    }

    // invert a power series mod x^n
    // P[2n] = P[n](2 - f * P[n]) mod x^(2n)
    static long[] Invert2(long[] f, int n)
    {
        // assert f[0] == 1
        // assert f != r
        // assert n is power of 2, and f, r can fit 2n

        long[] r = new long[] { 1 };
        for (int m = 2; m <= n; m <<= 1) {
            long[] nr = MultiplyPolynomialsMod(GetRange(f, 0, m), r, MOD, 2 * m);
            for (int i = 0; i < m; ++i) {
                nr[i] = -nr[i];
                if (nr[i] < 0)
                    nr[i] += MOD;
            }

            nr[0] += 2;
            if (nr[0] >= MOD)
                nr[0] -= MOD;
            r = MultiplyPolynomialsMod(nr, r, MOD, m);
        }

        return r;
    }

    // Polynomial division: divide P (deg n) by MONIC M (deg m)
    // P(x) = Q(x)M(x) + R(x)
    // x^nP(1/x) = (x^(n-m)Q(1/x))(x^mM(1/x)) + x^(n-m+1)(x^(m-1)R(x))
    // Let Q'(x)x^mM(1/x) = x^nP(1/x) mod x^(n-m+1), deg Q' <= n-m
    // Define Q(x) = x^(n-m)Q'(1/x). Then R(x) = P(x)-Q(x)M(x)
    // O(n log n)
    static long[] Divide2(long[] p, long[] d, long[] R)
    {
        if (p.Length < d.Length)
            return new long[1];

        if (d.Length <= 1)
            return p;

        int len = p.Length - d.Length + 1;
        return Reverse(MultiplyPolynomialsMod(Invert2(Reverse(d), len), Reverse(p), MOD, len));
    }

    // SOURCE: https://ideone.com/uQYLfQ
    public static long[] SqrtRootPolynomial(int n, long[] f)
    {
        Debug.Assert(f[0] == 1);
        long[] s = new long[] { 1 };
        long[] rs = new long[] { 1 };

        for (int m = 2; m <= n; m <<= 1) {
            int h = m >> 1;
            long[] nrs = MultiplyPolynomialsMod(GetRange(s, 0, h), rs, MOD, m);
            Array.Clear(nrs, 0, h);
            for (int i = h; i < m; i++)
                nrs[i] = -nrs[i];

            nrs = MultiplyPolynomialsMod(nrs, rs, MOD, m);
            Array.Copy(rs, 0, nrs, 0, h);
            nrs = MultiplyPolynomialsMod(nrs, f, MOD, m);

            Array.Copy(s, 0, nrs, 0, h);
            s = nrs;
            for (int i = h; i < m; i++)
                s[i] = Div(nrs[i], 2);

            nrs = MultiplyPolynomialsMod(s, rs, MOD, m);
            Array.Clear(nrs, 0, h);
            for (int i = h; i < m; i++)
                nrs[i] = -nrs[i];

            nrs = MultiplyPolynomialsMod(nrs, rs, MOD, m);
            rs = GetRange(rs, 0, m, true);
            Array.Copy(nrs, h, rs, h, m - h);
        }

        return s;
    }

    //
    // return p(x+a) 
    // 
    public static long[] ShiftNaive(long[] p, long a, long M)
    {
        long[] q = new long[p.Length];
        for (int i = p.Length - 1; i >= 0; --i) {
            for (int j = p.Length - i - 1; j >= 1; --j)
                q[j] = (q[j] * a % M + q[j - 1]) % M;
            q[0] = (q[0] * a % M + p[i]) % M;
        }

        return q;
    }

    public static long[] Shift(long[] f, long a)
    {
        long[] u = new long[f.Length];
        long[] v = new long[f.Length];

        int n = f.Length;
        for (int i = 0; i < n; i++)
            u[n - 1 - i] = f[i] * Fact(i) % MOD;

        v[0] = Fact(n - 1);
        for (int i = 1; i < n; i++)
            v[i] = v[i - 1] * a % MOD * Fact(i - 1) % MOD * InverseFact(i) % MOD;

        long[] uv = MultiplyPolynomialsMod(u, v, MOD, n);
        long[] result = new long[n];
        long ifactnm1 = InverseFact(n - 1);
        for (int i = 0; i < n; i++)
            result[i] = uv[n - 1 - i] * ifactnm1 % MOD * InverseFact(i) % MOD;

        return result;
    }

    #region Integer Roots

    public class IntegerRoots
    {
        public HashSet<int> Roots = new();

        public IntegerRoots(int[] a)
        {
            if (a[0] == 0)
                Roots.Add(0);

            int k = 0;
            while (a[k] == 0)
                k++;

            int a0 = a[k];
            int a0abs = Abs(a[k]);
            for (int x = 1; x * x <= a0abs; x++)
                if (a0 % x == 0) {
                    Check(a, k, x);
                    Check(a, k, -x);
                    int t = a0 / x;
                    Check(a, k, t);
                    Check(a, k, -t);
                }
        }

        void Check(int[] a, int k, int x)
        {
            long t = 0;
            for (int i = k; i < a.Length; i++) {
                t += a[i];
                if (t % x != 0) return;
                t /= x;
            }

            if (t == 0) Roots.Add(x);
        }
    }

    #endregion

    #region From Paper

    // https://core.ac.uk/download/pdf/81995869.pdf

    //public static long[] Invert(long[] f)
    //{
    //    var n = f.Length;
    //    if (n == 1) return new long[] { Inverse(f[0]) };
    //    var m = n + 1 >> 1;
    //    var g = Invert(GetRange(f, 0, m));
    //    return g - ((()))
    //}

    // Algorithm exp(f)
    // Input: f : TPS(C, n), such that exp f0 is defined and invertible in C.
    // Output: exp f : TPS(C, n)
    // if n = 1 then return (exp f0)0···1.  
    // m := ceil(n/2)
    // g := exp(f0···m)0···n
    // return g − (log(g) − f) × g

    // Right composition with polynomials  
    // f ◦ g = f_∗ ◦ g + (f^∗ ◦ g)g^floor(p/2c)
    // f_x = f0...floor(p/2)  f^* = f floor(p/2)....p

    //  Algorithm compose pol(f, H, n)
    //  Input: f : TPS(C, p), a hashtable H and an integer n;
    //         H[i] contains g^i 0··· min((q−1)i+1,n) for all i ∈ bp/2  N ∗ c ∪ dp/2 N ∗ e.
    //  Output: f0···l ◦ g0···l, where l = min((p − 1)(q − 1) + 1, n).
    //  P1. [Start]
    //  if p = 0 then return f0···1

    //  P2. [DAC]
    //  l := min((p − 1)(q − 1) + 1, n)
    //  h_∗ := compose pol(f0···bp/2c, H, n)
    //  h^* := compose pol(fbp/2c···p, H, n)
    //  return (h-∗)0···l + (h^∗ * H[floor(p / 2)])0···l

    public static long[] Compose(long[] f, long[] g) => throw new NotImplementedException();

    //If we have an algorithm compose for the composition of power series with time complexity
    //    O(C(n)) (where C(n)/n is an increasing function), then Newton’s method can
    //    still be applied in order to solve the equation
    //    f ◦ g − z = 0.

    // Algorithm revert(f)
    // Input: f : TPS(C, n) with f0 = 0 and f1 is invertible in C.
    // Output: f-1
    // if n = 1 then return 00···1
    // if n = 2 then return (z/f1)0···2
    // m := ceil(n/2)
    // g := revert(f0···m)0···n
    // N := compose(f, g) − z
    // D := compose(f', g0···n−1)
    // return g − (((N div z)/D) mul z)

    public static long[] Reversion(long[] f, long[] g) => throw new NotImplementedException();

    #endregion

    #region support Functions

    public static long[] Reverse(long[] array)
    {
        long[] result = (long[])array.Clone();
        Reverse(result);
        return result;
    }

    public static long[] Trim(long[] poly, bool force = false)
    {
        int length = poly.Length;
        while (length > 1 && poly[length - 1] == 0)
            length--;
        if (force && length == poly.Length) return (long[])poly.Clone();
        Array.Resize(ref poly, length);
        return poly;
    }

    public static T[] GetSubrange<T>(T[] x, int start, int count, bool extend = false)
    {
        if (start + count > x.Length && !extend) throw new ArgumentOutOfRangeException();
        var result = new T[count];
        Array.Copy(x, start, result, 0, Min(count, x.Length - start));
        return result;
    }

    public static double[] Derivative(double[] poly)
    {
        if (poly.Length == 0)
            return poly;

        double[] newPoly = new double[poly.Length - 1];
        for (int i = 0; i < newPoly.Length; i++)
            newPoly[i] = poly[i + 1] * (i + 1);
        return poly;
    }

    public static double[] Integrate(double[] poly)
    {
        if (poly.Length == 0)
            return poly;

        double[] newPoly = new double[poly.Length + 1];
        for (int i = 0; i < poly.Length; i++)
            newPoly[i + 1] = poly[i] / (i + 1);
        return newPoly;
    }

    public static double Evaluate(double[] poly, double x)
    {
        double result = 0;
        for (int i = poly.Length - 1; i >= 0; i--)
            result = result * x + poly[i];
        return result;
    }

    #endregion
}