using System.Numerics;

namespace Algorithms.Mathematics;

// SOURCE: https://www.codechef.com/viewsolution/21023237

public static class Polynomial2
{
    // const int mod = 998244353;
    const int L = 15; // can be 23 for 998244353
    const int N = 1 << L;

    const int mod = 998244353;
    const int MAXN = 3030;
    static bool fftInitialized;
    static List<int>[] a = new List<int>[MAXN];
    static readonly int root = GetRoot();

    static readonly long[] angles = new long[N + 1];
    static readonly int[] bitrev = new int[N];

    static int GetRoot()
    {
        int root = 1;
        while (ModPow(root, 1 << L) != 1 || ModPow(root, 1 << (L - 1)) == 1)
            ++root;
        return root;
    }

    static void FftInit()
    {
        angles[0] = 1;
        for (int i = 1; i <= N; ++i)
            angles[i] = angles[i - 1] * root % mod;

        for (int i = 0; i < N; ++i) {
            int x = i;
            for (int j = 0; j < L; ++j) {
                bitrev[i] = (bitrev[i] << 1) | (x & 1);
                x >>= 1;
            }
        }

        fftInitialized = true;
    }

    static int RevBit(int x, int len) => bitrev[x] >> (L - len);

    static void Fft(long[] a, bool inverse = false)
    {
        Debug.Assert(fftInitialized);
        int n = a.Length;
        Debug.Assert((n & (n - 1)) == 0); // work only with powers of two
        int l = (int)BitOperations.Log2((uint)(n & -n));

        for (int i = 0; i < n; ++i) {
            int j = RevBit(i, l);
            if (i < j) Swap(ref a[i], ref a[j]);
        }

        for (int len = 1; len < n; len *= 2)
        for (int start = 0; start < n; start += 2 * len)
        for (int i = 0; i < len; ++i) {
            long x = a[start + i];
            long y = a[start + len + i];
            int idx = N / 2 / len * i;
            long w = angles[inverse ? N - idx : idx];
            w = w * y % mod;
            a[start + i] = x + w;
            if (a[start + i] >= mod) a[start + i] -= mod;
            a[start + len + i] = x - w;
            if (a[start + len + i] < 0) a[start + len + i] += mod;
        }

        if (inverse) {
            int rev_deg = 1;
            for (int i = 0; i < l; ++i)
                rev_deg = (rev_deg & 1) != 0 ? (rev_deg + mod) >> 1 : rev_deg >> 1;

            for (int i = 0; i < a.Length; i++)
                a[i] = a[i] * rev_deg % mod;
        }
    }

    static long[] Multiply(long[] a, long[] b, int size = 0)
    {
        int n = 1;
        while (n < a.Length || n < b.Length) n *= 2;

        Array.Resize(ref a, 2 * n);
        Array.Resize(ref b, 2 * n);
        Fft(a);
        Fft(b);
        for (int i = 0; i < n + n; ++i)
            a[i] = a[i] * b[i] % mod;

        Fft(a, true);
        return Normalize(a, size);
    }

    static long[] Derivative(long[] a)
    {
        if (a.Length == 0)
            return a;

        long[] result = new long[a.Length - 1];
        for (int i = 1; i < a.Length; ++i)
            result[i - 1] = a[i] * i % mod;

        return result;
    }

    // returns $b(x) = \int_0^x{a(t)\,dt}$
    static long[] Integrate(long[] a)
    {
        if (a.Length == 0)
            return a;

        long[] result = new long[a.Length + 1];
        for (int i = 0; i < a.Length; ++i)
            result[i + 1] = a[i] * ModPow(i + 1, mod - 2) % mod;

        return result;
    }

    public static long[] Add(long[] a, long[] b, long mod)
    {
        long[] result = GetRange(a, 0, Math.Max(a.Length, b.Length), true);
        for (int i = 0; i < b.Length; ++i)
            result[i] = (a[i] + b[i]) % mod;
        return a;
    }

    public static long[] Sub(long[] a, long[] b, long mod)
    {
        long[] result = new long[Math.Max(a.Length, b.Length)];
        Array.Copy(a, result, a.Length);
        for (int i = 0; i < b.Length; ++i)
            result[i] = (a[i] + mod - b[i]) % mod;
        return result;
    }

    public static long[] Normalize(long[] a, int size = -1)
    {
        int len = size == -1 ? a.Length : Math.Min(size, a.Length);
        while (len > 0 && a[len - 1] == 0)
            len--;

        if (a.Length == len) return a;
        return a[..len];
    }

    public static long[] GetInverse(long[] a, int prec)
    {
        Debug.Assert(a[0] != 0);

        long[] res = new long[] { InverseDirect((int)a[0], mod) };
        int k = 1;
        while (k < prec) {
            k *= 2;
            long[] tmp = Multiply(res, GetRange(a, 0, Math.Min(k, a.Length)));
            for (int i = 0; i < tmp.Length; i++)
                tmp[i] = -tmp[i];

            tmp[0] = (tmp[0] + 2) % mod;
            res = Multiply(tmp, res, k);
        }

        return Normalize(res, prec);
    }

    // http://web.cs.iastate.edu/~cs577/handouts/polydivide.pdf
    public static void DivMod(long[] a, long[] b, out long[] quotient, out long[] remainder)
    {
        int n = a.Length;
        int m = b.Length;
        if (n < m) {
            quotient = new long[0];
            remainder = a;
            return;
        }

        Array.Reverse(a);
        Array.Reverse(b);
        quotient = Multiply(a, GetInverse(b, n - m + 1), n - m + 1);
        Array.Reverse(a);
        Array.Reverse(b);
        Array.Reverse(quotient);
        remainder = Normalize(Sub(a, Multiply(b, quotient), mod));
    }

    // get p and {x1, x2, ..., xn}, return {p(x1), p(x2), ..., p(xn)}
    public static List<long> Multipoint(long[] poly, long[] pts)
    {
        if (pts.Length == 0)
            return new List<long>();

        List<long[]> segment_polys = GetSegmentProducts(pts);
        var ans = new List<long>();
        Action<long[]> fillAns = null;
        fillAns = p => {
            if (segment_polys[segment_polys.Count - 1].Length <= 2) {
                ans.Add(p.Length == 0 ? 0 : p[0]);
                segment_polys.RemoveAt(segment_polys.Count - 1);
                return;
            }

            segment_polys.RemoveAt(segment_polys.Count - 1);

            long[] q, r;
            DivMod(p, segment_polys[segment_polys.Count - 1], out q, out r);
            fillAns(r);
            DivMod(p, segment_polys[segment_polys.Count - 1], out q, out r);
            fillAns(r);
        };
        fillAns(poly);

        ans.Reverse();
        return ans;
    }

    // this is for multipoint and interpolate functions
    static List<long[]> GetSegmentProducts(long[] pts)
    {
        var segmentPolys = new List<long[]>();
        Func<int, int, int> fillPolys = null;
        fillPolys = (left, right) => {
            if (left + 1 == right) {
                segmentPolys.Add(new[] { mod - pts[left], 1 });
                return segmentPolys.Count - 1;
            }

            int m = (left + right) >> 1;
            int i = fillPolys(left, m);
            int j = fillPolys(m, right);
            long[] newPoly = Multiply(segmentPolys[i], segmentPolys[j]);
            segmentPolys.Add(newPoly);
            return segmentPolys.Count - 1;
        };
        fillPolys(0, pts.Length);
        return segmentPolys;
    }

    // Alt implementation

    // builds evaluation tree for (x-a1)(x-a2)...(x-an)

    //public  static long[] Evaluate(long[] poly, long[] xs)
    //{
    //    int n = xs.Length;
    //    var tree = new long[n * 4][];
    //    Build(tree, xs, 0, n);
    //    return EvaluateCore(poly, tree, 1, 0, n);
    //}

    //public static long[] EvaluateCore(long[] poly, long[] x, int v, int left, int right)
    //{
    //    if (right - left == 1)
    //        return Evaluate(x[0, left]);

    //}

    static long[] Build(long[][] result, long[] pts, int left, int right, int v = 1)
    {
        if (right - left == 1) {
            result[v] = new[] { MOD - pts[left], 1 };
        } else {
            int mid = (left + right) >> 1;
            result[v] = Multiply(
                Build(result, pts, left, mid, 2 * v),
                Build(result, pts, mid, right, 2 * v + 1));
        }

        return result[v];
    }

    // get {x1, ..., xn} and {y1, ..., yn}, return such p that p(xi) = yi
    static long[] Interpolate(long[] xs, long[] ys)
    {
        Debug.Assert(xs.Length == ys.Length);
        if (xs.Length == 0)
            return xs;

        List<long[]> segment_polys = GetSegmentProducts(xs);
        long[] der = Derivative(segment_polys[segment_polys.Count - 1]);
        List<long> coeffs = Multipoint(der, xs);
        for (int i = 0; i < coeffs.Count; i++)
            coeffs[i] = Inverse(coeffs[i]);

        for (int i = 0; i < ys.Length; ++i)
            coeffs[i] = coeffs[i] * ys[i] % mod;

        Func<long[]> getAns = null;
        getAns = () => {
            if (segment_polys[segment_polys.Count - 1].Length <= 2) {
                segment_polys.RemoveAt(segment_polys.Count - 1);
                long[] res = new[] { coeffs[coeffs.Count - 1] };
                coeffs.RemoveAt(coeffs.Count - 1);
                return res;
            }

            segment_polys.RemoveAt(segment_polys.Count - 1);

            long[] p1 = segment_polys[segment_polys.Count - 1];
            long[] q1 = getAns();
            long[] p2 = segment_polys[segment_polys.Count - 1];
            long[] q2 = getAns();
            return Add(Multiply(p1, q2), Multiply(p2, q1), mod);
        };

        return Normalize(getAns());
    }

    // takes 1 + b, returns b - b^2/2 + b^3/3 - ... mod x^{prec}
    // ofc b must be divisible by x
    static long[] Logarithm(long[] a, int prec)
    {
        Debug.Assert(a[0] == 1);
        return Integrate(Multiply(Derivative(a), GetInverse(a, prec), prec - 1));
    }

    // returns 1 + a + a^2/2 + a^3/6 + ... mod x^{prec}
    // ofc a must be divisible by x
    static long[] Exponent(long[] a, int prec)
    {
        Debug.Assert(a[0] == 0);
        long[] res = new long[] { 0 };
        int k = 1;
        while (k < prec) {
            k *= 2;
            long[] tmp = a[..Math.Min(k, a.Length)];
            tmp[0] += 1;
            tmp = Sub(tmp, Logarithm(res, k), mod);
            res = Multiply(tmp, res, k);
        }

        return res[..prec];
    }
}