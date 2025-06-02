namespace Algorithms.Mathematics.Multiplication.FFT;

// Based on "Fast convolution for 64-bit integers" by Quasisphere
// Article: https://codeforces.com/blog/entry/45298
// Code: https://github.com/quasisphere/conv64 (MIT License)
/*
 * Our first step is to note that the standard Radix-2 FFT has the problem that
 * its inverse transform requires division by 2, which is not invertible in R.
 *
 * We solve this by employing a Radix-3 FFT, which handles arrays whose size is
 * a power of 3, and whose inverse transform requires division by 3.
 *
 * Now for FFT to work, the ring has to have a sufficiently powerful 3^m'th
 * root of unity, and since the unit ring of R is Z_2 x Z_{2^62}, it only has
 * roots of unity of order 2^m.
 *
 * The first step to solving this is by expanding the ring to have a 3rd root
 * of unity. This extension can be realized as the ring R[omega]/(omega^2 +
 * omega + 1), that is polynomials of the form a + b*omega, with the property
 * that omega^2 = - omega - 1. It follows that omega^3 = 1.
 *
 * We call this new ring T and define the following type for its elements.
 */

public class Conv64
{
    /*
     * A couple of useful constants: `OMEGA` is a third root of unity, `OMEGA2` is
     * its square and `INV3` is the multiplicative inverse of 3.
     */

    static readonly T OMEGA = new(0, 1);
    static readonly T OMEGA2 = new(-1, -1);
    static readonly T INV3 = new(unchecked((long)12297829382473034411ul), 0);
    static readonly T ONE = 1;

    // Temporary space.
    T[] tmp;

    // Returns the product of two polynomials from the ring R[x].
    public long[] Multiply(ReadOnlySpan<long> p, ReadOnlySpan<long> q)
    {
        int resultSize = p.Length + q.Length - 1;
        if (resultSize <= 0) return Array.Empty<long>();

        int s = 1;
        while (s < resultSize) s *= 3;

        long[] pp = new long[s], qq = new long[s];
        p.CopyTo(pp);
        q.CopyTo(qq);

        long[] res = new long[s];
        MultiplyCyclicRaw(pp, qq, pp.Length, res);
        return res;
    }

    // Returns the product of a polynomial and the monomial x^t in the ring
    // T[x]/(x^m - omega). The result is placed in `to`.
    // NOTE: t must be in the range [0,3m]
    void Twiddle(ReadOnlySpan<T> p, int m, int t, Span<T> to)
    {
        if (t == 0 || t == 3 * m) {
            for (int j = 0; j < m; ++j) to[j] = p[j];
            return;
        }

        int tt;
        T mult = 1;
        if (t < m) {
            tt = t;
        } else if (t < 2 * m) {
            tt = t - m;
            mult = OMEGA;
        } else {
            tt = t - 2 * m;
            mult = OMEGA2;
        }

        for (int j = 0; j < tt; j++) to[j] = p[m - tt + j] * OMEGA * mult;
        for (int j = tt; j < m; j++) to[j] = p[j - tt] * mult;
    }

    // A "Decimation In Frequency" In-Place Radix-3 FFT Routine.
    // Input: A polynomial from (T[x]/(x^m - omega))[y]/(y^r - 1).
    // Output: Its Fourier transform (w.r.t. y) in 3-reversed order.
    void FftDif(Span<T> p, int m, int r)
    {
        if (r == 1) return;
        int rr = r / 3;
        int pos0 = 0, pos1 = m * rr, pos2 = 2 * m * rr;
        for (int i = 0; i < rr; ++i, pos0 +=m, pos1 +=m, pos2 += m) {
            for (int j = 0; j < m; ++j) {
                tmp[j] = p[pos0 + j] + p[pos1 + j] + p[pos2 + j];
                tmp[m + j] = p[pos0 + j] + OMEGA * p[pos1 + j] + OMEGA2 * p[pos2 + j];
                tmp[2 * m + j] = p[pos0 + j] + OMEGA2 * p[pos1 + j] + OMEGA * p[pos2 + j];
                p[pos0 + j] = tmp[j];
            }

            Twiddle(tmp.AsSpan(m), m, 3 * i * m / r, p.Slice(pos1));
            Twiddle(tmp.AsSpan(2 * m), m, 6 * i * m / r, p.Slice(pos2));
        }

        FftDif(p, m, rr);
        FftDif(p.Slice(m * rr), m, rr);
        FftDif(p.Slice(2 * m * rr), m, rr);
    }

    // A "Decimation In Time" In-Place Radix-3 Inverse FFT Routine.
    // Input: A polynomial in (T[x]/(x^m - omega))[y]/(y^r - 1) with coefficients
    //        in 3-reversed order.
    // Output: Its inverse Fourier transform in normal order.
    void FftDit(Span<T> p, int m, int r)
    {
        if (r == 1) return;
        int rr = r / 3;
        int pos1 = m * rr, pos2 = 2 * m * rr;
        FftDit(p, m, rr);
        FftDit(p.Slice(pos1), m, rr);
        FftDit(p.Slice(pos2), m, rr);
        for (int i = 0; i < rr; ++i) {
            Twiddle(p.Slice(pos1 + i * m), m, 3 * m - 3 * i * m / r, tmp.AsSpan(m));
            Twiddle(p.Slice(pos2 + i * m), m, 3 * m - 6 * i * m / r, tmp.AsSpan(2 * m));
            for (int j = 0; j < m; ++j) {
                tmp[j] = p[i * m + j];
                p[i * m + j] = tmp[j] + tmp[m + j] + tmp[2 * m + j];
                p[i * m + pos1 + j] = tmp[j] + OMEGA2 * tmp[m + j] + OMEGA * tmp[2 * m + j];
                p[i * m + pos2 + j] = tmp[j] + OMEGA * tmp[m + j] + OMEGA2 * tmp[2 * m + j];
            }
        }
    }

    // Computes the product of two polynomials in T[x]/(x^n - omega), where n is
    // a power of 3. The result is placed in `to`.
    void Mul(Span<T> p, Span<T> q, int n, Span<T> to)
    {
        if (n <= 27) // 3 is the lowest we can use; 27 is our go to. 
        {
            // O(n^2) grade-school multiplication
            for (int i = 0; i < n; ++i)
                to[i] = new T();

            for (int i = 0; i < n; ++i) {
                for (int j = 0; j < n - i; ++j)
                    to[i + j] += p[i] * q[j];

                for (int j = n - i; j < n; ++j)
                    to[i + j - n] += p[i] * q[j] * OMEGA;
            }

            return;
        }

        int m = 1;
        while (m * m < n)
            m *= 3;

        int r = n / m;
        T inv = 1;
        for (int i = 1; i < r; i *= 3)
            inv *= INV3;

        /**********************************************************
         * THE PRODUCT IN (T[x]/(x^m - omega))[y] / (y^r - omega) *
         **********************************************************/

        // Move to the ring (T[x]/(x^m - omega))[y]/(y^r - 1) via the map y -> x^(m/r) y
        for (int i = 0; i < r; ++i) {
            Twiddle(p.Slice(m * i), m, m / r * i, to.Slice(m * i));
            Twiddle(q.Slice(m * i), m, m / r * i, to.Slice(n + m * i));
        }

        // Multiply using FFT
        FftDif(to, m, r);
        FftDif(to.Slice(n), m, r);
        for (int i = 0; i < r; ++i)
            Mul(to.Slice(m * i), to.Slice(n + m * i), m, to.Slice(2 * n + m * i));

        FftDit(to.Slice(2 * n), m, r);
        for (int i = 0; i < n; ++i)
            to[2 * n + i] *= inv;

        // Return to the ring (T[x]/(x^m - omega))[y]/(y^r - omega)
        for (int i = 0; i < r; ++i)
            Twiddle(to.Slice(2 * n + m * i), m, 3 * m - m / r * i, to.Slice(n + m * i));

        /************************************************************
         * THE PRODUCT IN (T[x]/(x^m - omega^2))[y] / (y^r - omega) *
         ************************************************************/

        // Use conjugation to move to the ring (T[x]/(x^m - omega))[y]/(y^r - omega^2).
        // Then move to (T[x]/(x^m - omega))[y]/(y^r - 1) via the map y -> x^(2m/r) y
        for (int i = 0; i < r; ++i) {
            for (int j = 0; j < m; ++j) {
                p[m * i + j] = p[m * i + j].conj();
                q[m * i + j] = q[m * i + j].conj();
            }

            Twiddle(p.Slice(m * i), m, 2 * m / r * i, to.Slice(m * i));
            Twiddle(q.Slice(m * i), m, 2 * m / r * i, p.Slice(m * i));
        }

        FftDif(to, m, r);
        FftDif(p, m, r);
        for (int i = 0; i < r; ++i)
            Mul(to.Slice(m * i), p.Slice(m * i), m, to.Slice(2 * n + m * i));

        FftDit(to.Slice(2 * n), m, r);
        for (int i = 0; i < n; ++i)
            to[2 * n + i] *= inv;

        for (int i = 0; i < r; ++i)
            Twiddle(to.Slice(2 * n + m * i), m, 3 * m - 2 * m / r * i, q.Slice(m * i));

        /**************************************************************************
         * The product in (T[x]/(x^(2m) + x^m + 1))[y]/(y^r - omega) via CRT, and *
         * unravelling the substitution y = x^m at the same time.                 *
         **************************************************************************/

        to.Slice(0, n).Clear();
        for (int i = 0; i < r; ++i)
        for (int j = 0; j < m; ++j) {
            to[i * m + j] += (ONE - OMEGA) * to[n + i * m + j] + (ONE - OMEGA2) * q[i * m + j].conj();
            if (i * m + m + j < n)
                to[i * m + m + j] += (OMEGA2 - OMEGA) * (to[n + i * m + j] - q[i * m + j].conj());
            else
                to[i * m + m + j - n] += (ONE - OMEGA2) * (to[n + i * m + j] - q[i * m + j].conj());
        }

        for (int i = 0; i < n; ++i) to[i] *= INV3;
    }

    // Computes the product of two polynomials from the ring R[x]/(x^n - 1), where
    // n must be a power of three. The result is placed in target which must have
    // space for n elements.
    void MultiplyCyclicRaw(ReadOnlySpan<long> p, ReadOnlySpan<long> q, int n, Span<long> target)
    {
        // If n = 3^k, let m = 3^(floor(k/2)) and r = 3^(ceil(k/2))
        int m = 1;
        while (m * m <= n)
            m *= 3;
        m /= 3;
        int r = n / m;

        // Compute 3^(-r)
        T inv = 1;
        for (int i = 1; i < r; i *= 3)
            inv *= INV3;

        // Allocate some working memory, the layout is as follows:
        // pp: length n
        // qq: length n
        // to: length n + 3*m
        // tmp: length 3*m

        T[] buf = new T[3 * n + 3 * m];
        {
            Span<T> pp = buf;
            Span<T> qq = buf.AsSpan(n);
            Span<T> to = buf.AsSpan(2 * n);
            tmp = new T[3 * m];

            for (int i = 0; i < n; ++i) {
                pp[i] = p[i];
                qq[i] = q[i];
            }

            // By setting y = x^m, we may write our polynomials in the form
            //   (p_0 + p_1 x + ... + p_{m-1} x^{m-1})
            // + (p_m + ... + p_{2m-1} x^{m-1}) y
            // + ...
            // + (p_{(r-1)m} + ... + p_{rm - 1} x^{m-1}) y^r
            //
            // In this way we can view p and q as elements of the ring S[y]/(y^r - 1),
            // where S = R[x]/(x^m - omega), and since r <= 3m, we know that x^{3m/r} is
            // an rth root of unity. We can therefore use FFT to calculate the product
            // in S[y]/(y^r - 1).
            FftDif(pp, m, r);
            FftDif(qq, m, r);
            for (int i = 0; i < r; ++i) Mul(pp.Slice(i * m), qq.Slice(i * m), m, to.Slice(i * m));
            FftDit(to, m, r);
            for (int i = 0; i < n; ++i) pp[i] = to[i] * inv;

            // Now, the product in (T[x]/(x^m - omega^2))[y](y^r - 1) is simply the
            // conjugate of the product in (T[x]/(x^m - omega))[y]/(y^r - 1), because
            // there is no omega-component in the data.
            //
            // By the Chinese Remainder Theorem we can obtain the product in the
            // ring (T[x]/(x^(2m) + x^m + x))[y]/(y^r - 1), and then set y=x^m to get
            // the result.

            for (int i = 0; i < n; ++i) to[i] = 0;
            for (int i = 0; i < r; ++i)
            for (int j = 0; j < m; ++j) {
                to[i * m + j] += (ONE - OMEGA) * pp[i * m + j] + (ONE - OMEGA2) * pp[i * m + j].conj();
                if (i * m + m + j < n)
                    to[i * m + m + j] += (OMEGA2 - OMEGA) * (pp[i * m + j] - pp[i * m + j].conj());
                else
                    to[i * m + m + j - n] += (OMEGA2 - OMEGA) * (pp[i * m + j] - pp[i * m + j].conj());
            }

            for (int i = 0; i < n; ++i)
                target[i] = (to[i] * INV3).a;
        }
    }

    struct T
    {
        public readonly long a;
        public readonly long b;

        public static implicit operator T(long x) => new(x, 0);

        public T(long a, long b)
        {
            this.a = a;
            this.b = b;
        }

        //The conjugate of a + b*omega is given by mapping omega -> omega^2
        public T conj() => new(a - b, -b);

        public static T operator -(T x) => new(-x.a, -x.b);

        public static T operator +(T u, T v) => new(u.a + v.a, u.b + v.b);

        public static T operator -(T u, T v) => new(u.a - v.a, u.b - v.b);

        public static T operator *(T u, T v) => new(u.a * v.a - u.b * v.b, u.b * v.a + u.a * v.b - u.b * v.b);
    }
}