using System.Numerics;

namespace Algorithms.Mathematics;

public class MathUtil
{
    public const double Degrees = 180.0 / Math.PI;

    public static ulong Ceiling(ulong n, ulong div) => (n + div - 1) / div;

    public static long Floor(long n, long div)
    {
        if (div < 0) {
            n = -n;
            div = -div;
        }

        return n >= 0 ? n / div : (n - div + 1) / div;
    }

    public static long Sqrt(long x)
    {
        if (x == 0 || x == 1)
            return x;

        long start = 1, end = Math.Min(x, 3037000499L), ans = 1;
        while (start <= end) {
            long mid = start + ((end - start) >> 1);
            long sqr = mid * mid;
            if (sqr == x)
                return mid;

            if (sqr < x) {
                start = mid + 1;
                ans = mid;
            } else {
                end = mid - 1;
            }
        }

        return ans;
    }

    public static BigInteger Sqrt(BigInteger x)
    {
        if (x == 0 || x == 1)
            return x;

        BigInteger start = 1, end = x, ans = 1;
        while (start <= end) {
            // Using shift operators will time out
            BigInteger mid = (start + end) / 2;
            BigInteger sqr = mid * mid;
            if (sqr == x)
                return mid;

            if (sqr < x) {
                start = mid + 1;
                ans = mid;
            } else {
                end = mid - 1;
            }
        }

        return ans;
    }

    public static BigInteger Sqrt2(BigInteger n)
    {
        if (n <= 0) return 0;
        int bitLength = (int)Math.Ceiling(BigInteger.Log(n, 2));
        BigInteger root = BigInteger.One << (bitLength / 2);

        while (true) {
            BigInteger root2 = root * root;
            if (n >= root2) {
                BigInteger upper = root2 + (root << 1) + 1;
                if (n < upper) break;
            }

            root += n / root;
            root >>= 1;
        }

        return root;
    }

    static bool IsSqrt(BigInteger n, BigInteger root)
    {
        BigInteger lowerBound = root * root;
        BigInteger upperBound = (root + 1) * (root + 1);

        return n >= lowerBound && n < upperBound;
    }

    public static long CeilingSqrt(long x)
    {
        long sqrt = Sqrt(x);
        if (sqrt * sqrt == x) return sqrt;
        return sqrt + 1;
    }

    public static long CubeRoot(long x)
    {
        if (x <= 1) {
            if (x >= 0) return x;
            return x > long.MinValue ? -CubeRoot(-x) : -(1L << (63 / 3));
        }

        long start = 1, end = Math.Min(x, 2097151L), ans = 1;
        while (start <= end) {
            long mid = start + (end - start) / 2;
            long sqr = mid * mid * mid;
            if (sqr == x)
                return mid;

            if (sqr < x) {
                start = mid + 1;
                ans = mid;
            } else {
                end = mid - 1;
            }
        }

        return ans;
    }

    public static long CeilingCubeRoot(long x)
    {
        long sqrt = CubeRoot(x);
        return sqrt * sqrt * sqrt == x ? sqrt : sqrt + 1;
    }

    ulong Mod64(ulong x, ulong y, ulong z)
    {
        /* Divides (x || y) by z, for 64-bit integers x, y,
        and z, giving the remainder (modulus) as the result.
        Must have x < z (to get a 64-bit result). This is
        checked for. */
        if (x >= z)
            throw new InvalidOperationException("Bad call to modul64, must have x < z.");

        for (int i = 1; i <= 64; i++) {
            // Do 64 times.
            long t = (long)x >> 63; // All 1's if x(63) = 1.
            x = (x << 1) | (y >> 63); // Shift x || y left
            y = y << 1; // one bit.
            if ((x | (ulong)t) >= z) {
                x = x - z;
                y = y + 1;
            }
        }

        return x; // Quotient is y.
    }

    void Mult64(ulong u, ulong v, out ulong whi, out ulong wlo)
    {
        ulong u0 = u & 0xFFFFFFFF;
        ulong u1 = u >> 32;
        ulong v1 = v >> 32;
        ulong v0 = v & 0xFFFFFFFF;

        ulong t = u0 * v0;
        ulong w0 = t & 0xFFFFFFFF;
        ulong k = t >> 32;

        t = u1 * v0 + k;
        ulong w1 = t & 0xFFFFFFFF;
        ulong w2 = t >> 32;

        t = u0 * v1 + w1;
        k = t >> 32;

        wlo = (t << 32) + w0;
        whi = u1 * v1 + w2 + k;
    }

    /*
    static long mod(long a, long b, long c)
    {
        long x = 1, y = a;
        while (b > 0)
        {
            if (b % 2 == 1)
            {
                x = mul(x, y, c);
            }
            y = mul(y, y, c);
            b /= 2;
        }
        return x % c;
    }

    static long mul(long a, long b, long c)
    {
        long x = 0, y = a % c;
        while (b > 0)
        {
            if (b % 2 == 1)
            {
                x = (x + y) % c;
            }
            y = (y * 2) % c;
            b /= 2;
        }
        return x % c;
    }

        static bool Miller(long p, long it)
{
    if (p < 2)
        return false;
    if (p != 2 && p % 2 == 0)
        return false;
    long s = p - 1;
    while (s % 2 == 0)
        s /= 2;

    long i;
    var random = new Random();
    for (i = 0; i < it; i++)
    {
        long a = random.Next(2,100000), tt = s;
        long md = mod(a, tt, p);
        while (tt != p - 1 && md != 1 && md != p - 1)
        {
            md = mul(md, md, p);
            tt *= 2;
        }
        if (md != p - 1 && tt % 2 == 0)
            return false;
    }
    return true;
}
     */
}