using static System.Math;

namespace Algorithms.Mathematics;

public static class ModularMath
{
    // TODO: Do this work for signed

    // WARNING: Unsigned math doesn't work well with modular arithmetic
    // Specifically, negative numbers and overflowed numbers

    /*
public static long Mult(long a, long b, long mod)
{
    // If both integers are positive 32-bit integers, use shortcut
    if ((ulong)(a | b) <= (1UL << 31))
        return a * b % mod;

    // If mod is positive 32-bit integer, use shortcut
    a %= mod;
    b %= mod;
    if ((ulong)mod <= (1UL << 31))
        return a * b % mod;

    if (a < b)
    {
        long tmp = a;
        a = b;
        b = tmp;
    }

    long result = 0;
    long y = a;
    while (b > 0)
    {
        if (b % 2 == 1)
            result = (result + y) % mod;
        y = (y * 2) % mod;
        b /= 2;
    }
    return result % mod;
}*/

    public static long MultSlow(long a, long b, long mod)
    {
        // If both integers are positive 32-bit integers, use shortcut
        if ((a | b) <= 1L << 31)
            return a * b % mod;

        // If mod is positive 32-bit integer, use shortcut
        a %= mod;
        b %= mod;
        if (mod <= 1L << 31)
            return a * b % mod;

        if (a < b) {
            long tmp = a;
            a = b;
            b = tmp;
        }

        long result = 0;
        long y = a;
        while (b > 0) {
            if (b % 2 == 1) {
                result = result + y;
                if (result >= mod) result -= mod;
            }

            y <<= 1;
            if (y >= mod) y -= mod;
            b >>= 1;
        }

        return result % mod;
    }

    public static long Mult(long a, long b, long mod)
    {
        // Ten times faster than MultSlow
        if ((ulong)a >= (ulong)mod) a %= mod;
        if (a < 0) a += mod;
        if ((ulong)b >= (ulong)mod) b %= mod;
        if (b < 0) b += mod;

        long ret = 0;
        int step = 62 - BitTools.Log2(mod);
        for (int x = BitTools.Log2(b); x >= 0; x -= step) {
            int shift = Min(x + 1, step);
            ret <<= shift;
            ret %= mod;
            ret += a * ((long)((ulong)b >> (x - shift + 1)) & ((1L << shift) - 1));
            ret %= mod;
        }

        return ret;
    }

    // Almost as fast as the other fastest implementations, but has full range
    public static long MultAlt(long a, long b, long mod)
    {
        long q = (long)((double)a * b / mod);
        long result = unchecked(a * b - mod * q);
        result %= mod;
        if (result < 0) result += mod;
        return result;
    }

    // code credit: https://codeforces.com/blog/entry/60442
    public static ulong MultMod2_61(ulong a, ulong b)
    {
        unchecked {
            const ulong mod = (1ul << 61) - 1;
            ulong l1 = (uint)a, h1 = a >> 32, l2 = (uint)b, h2 = b >> 32;
            ulong l = l1 * l2, m = l1 * h2 + l2 * h1, h = h1 * h2;
            ulong ret = (l & mod) + (l >> 61) + (h << 3) + (m >> 29) + ((m << 35) >> 3) + 1;
            ret = (ret & mod) + (ret >> 61);
            ret = (ret & mod) + (ret >> 61);
            return ret - 1;
        }
    }

    // https://www.hackerrank.com/contests/projecteuler/challenges/euler048
    public static ulong Mult34Bit(ulong x, ulong y, ulong mod)
    {
        // x,y,mod must fit within 34 bits
        // x and y can be made to fit within 34 bits by modding first
        // 2^34 = 1.7 * 10^10

        // Thirty times faster than MultSlow
        if (x >= 1 << 32 && y >= 1 << 32)
            // if (x > (1 << 30) && y > (1 << 30))
            // First term = Xhi (4-bits) * (Y (34-bits) * 2^30 % mod)
            // Second term = XLo (30-bits) * Y (34-bits) 
            return ((x >> 30) * ((y << 30) % mod) + y * (x & ((1 << 30) - 1))) % mod;

        // 30 bit times 34 bits < 64 bits
        ulong z = x * y;
        if (z >= mod) z %= mod;
        return z;
    }

    // Mult42Bit may be slower
    public static ulong Mult42Bit(ulong x, ulong y, ulong mod)
    {
        // x,y,mod must fit within 42 bits
        // x and y can be made to fit within 42 bits by modding first
        // 2^42 = 4.39 * 10^12

        // Thirty times faster than MultSlow
        if (x <= 1ul << 22 || y <= 1ul << 22 || (x < 1 << 32 && y < 1 << 32)) {
            ulong z = x * y;
            if (z >= mod) z %= mod;
            return z;
        }

        // First term = Xhi * (Y  % mod)
        // (maxbit-bits) + maxbit <= 64
        // Second term = XLo (30-bits) * Y (34-bits)
        // bits + maxbit = 64
        return ((x >> 22) * ((y << 22) % mod) + y * (x & ((1ul << 22) - 1))) % mod;
    }

    public static long Mult34Bit(long x, long y, long mod)
    {
        // x,y,mod must fit within 34 bits
        // x and y can be made to fit within 34 bits by modding first
        // 2^34 = 1.7 * 10^10

        // Thirty times faster than MultSlow
        if (x >= 1 << 32 && y >= 1 << 32)
            // if (x > (1 << 30) && y > (1 << 30))
            // First term = Xhi (4-bits) * (Y (34-bits) * 2^30 % mod)
            // Second term = XLo (30-bits) * Y (34-bits) 
            return ((x >> 30) * ((y << 30) % mod) + y * (x & ((1 << 30) - 1))) % mod;

        // 30 bit times 34 bits < 64 bits
        long z = x * y;
        if (z >= mod) z %= mod;
        return z;
    }

    public static long Mult42Bit(long x, long y, long mod)
    {
        // x,y,mod must fit within 42 bits
        // x and y can be made to fit within 42 bits by modding first
        // 2^42 = 4.39 * 10^12

        // Thirty times faster than MultSlow
        if ((ulong)x <= 1L << 22 || (ulong)y <= 1L << 22
                                 || ((ulong)x < 1L << 32 && (ulong)y < 1L << 32)) {
            long z = x * y;
            if (z >= mod) z %= mod;
            return z;
        }

        // First term = Xhi * (Y  % mod)
        // (maxbit-bits) + maxbit <= 64
        // Second term = XLo (30-bits) * Y (34-bits)
        // bits + maxbit = 64
        return ((x >> 22) * ((y << 22) % mod) + y * (x & ((1L << 22) - 1))) % mod;
    }

    public static ulong MultSlow(ulong a, ulong b, ulong mod)
    {
        // If both integers are positive 32-bit integers, use shortcut
        if ((a | b) <= 1UL << 31)
            return a * b % mod;

        // If mod is positive 32-bit integer, use shortcut
        a %= mod;
        b %= mod;
        if (mod <= 1UL << 31)
            return a * b % mod;

        if (a < b) {
            ulong tmp = a;
            a = b;
            b = tmp;
        }

        ulong result = 0;
        ulong y = a;
        while (b > 0) {
            if (b % 2 == 1) {
                result = result + y;
                if (result >= mod) result -= mod;
            }

            y <<= 1;
            if (y >= mod) y -= mod;
            b >>= 1;
        }

        return result % mod;
    }

    public static int Mult(int a, int b, int mod) => (int)((long)a * b % mod);

    public static ulong ModPow(ulong n, ulong p, ulong mod)
    {
        ulong result = 1;
        ulong b = n;
        while (p > 0) {
            if ((p & 1) == 1) result = MultSlow(result, b, mod);
            p >>= 1;
            b = MultSlow(b, b, mod);
        }

        return result;
    }

    public static int ModPow(int n, int p, int mod)
    {
        long result = 1;
        long b = n;
        while (p > 0) {
            if ((p & 1) == 1) result = result * b % mod;
            p >>= 1;
            b = b * b % mod;
        }

        return (int)result;
    }

    // return a % b (positive value)

    public static int Mod(int a, int b) => (a % b + b) % b;

    public static long Mod(long a, long b) => (a % b + b) % b;

    // Inverse code based on Euclidean generally at least 50% faster
    // Also, use of ints is possible which is not true for PowMod-based methods

    // computes b such that ab = 1 (mod n), returns -1 on failure

    public static long ModInverse(long a, long mod)
    {
        // Can change this line to int safely
        long t = 0, r = mod, t2 = 1, r2 = a;
        while (r2 != 0) {
            long q = r / r2;
            t -= q * t2;
            r -= q * r2;

            if (r != 0) {
                q = r2 / r;
                t2 -= q * t;
                r2 -= q * r;
            } else {
                r = r2;
                t = t2;
                break;
            }
        }

        if (r > 1) return -1;
        return t >= 0 ? t : t + mod;
    }

    public static int ModInverse(int a, int mod)
    {
        // Can change this line to int safely
        int t = 0, r = mod, t2 = 1, r2 = a;
        while (r2 != 0) {
            int q = r / r2;
            t -= q * t2;
            r -= q * r2;

            if (r != 0) {
                q = r2 / r;
                t2 -= q * t;
                r2 -= q * r;
            } else {
                r = r2;
                t = t2;
                break;
            }
        }

        if (r > 1) return -1;
        return t >= 0 ? t : t + mod;
    }

    public static int ModInverse2(int v, int mod)
    {
        int inv = 1, y = 0, xx = 0, yy = 1;
        int m = mod;
        while (m != 0) {
            int q = v / m;
            int t = m;
            m = v % m;
            v = t;
            t = xx;
            xx = inv - q * xx;
            inv = t;
            t = yy;
            yy = y - q * yy;
            y = t;
        }

        // if (v>1) return -1;

        if (unchecked((uint)inv >= mod)) {
            inv %= mod;
            if (v < 0) inv += mod;
        }

        return inv;
    }

    public static int ModInverseUntested(int a, int md)
    {
        a %= md;
        if (a < 0) a += md;
        int b = md, u = 0, v = 1;
        while (a != 0) {
            int t = b / a;
            b -= t * a;
            Swap(ref a, ref b);
            u -= t * v;
            Swap(ref u, ref v);
        }

        Debug.Assert(b == 1);
        if (u < 0) u += md;
        return u;
    }

    // SOURCE: From Euclid's GCD to Montgomery Multiplication to the Great Divide
    // http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.140.7944&rep=rep1&type=pdf

    // Compute y/x % mod
    public static long DivisionUntested(long y, long x, long mod)
    {
        long a = x;
        long b = mod;
        long u = y;
        long v = 0L;
        while (a != b)
            if ((a & 1) == 0) {
                a >>= 1;
                if ((u & 1) == 0)
                    u >>= 1;
                else
                    u = (u + mod) >> 1;
            } else if ((b & 1) == 0) {
                b >>= 1;
                if ((v & 1) == 0)
                    v >>= 1;
                else
                    v = (v + mod) >> 1;
            } else if (a > b) {
                a = (a - b) >> 1;
                u -= v;
                if (u < 0) u += mod;
                if ((u & 1) == 0)
                    u >>= 1;
                else
                    u = (u + mod) >> 1;
            } else {
                b = (b - a) >> 1;
                v -= u;
                if (v < 0) v += mod;
                if ((v & 1) == 0)
                    v >>= 1;
                else
                    v = (v + mod) >> 1;
            }

        return u;
    }

    public static long Division2Untested(long y, long x, long mod)
    {
        long a = x;
        long b = mod;
        long u = y;
        long v = 0L;
        while (true) {
            while ((a & 1) == 0) {
                a >>= 1;
                if ((u & 1) == 0)
                    u >>= 1;
                else
                    u = (u + mod) >> 1;
            }

            if (a == 1)
                return u;

            if (a < b) {
                long tmp = a;
                a = b;
                b = tmp;
                tmp = u;
                u = v;
                v = tmp;
            }

            a += b;
            u += v;
        }
    }

    public static int[] Inverses(int n, int mod)
    {
        int[] table = new int[n + 1];
        table[0] = table[1] = 1;
        for (long i = 2; i <= n; i++)
            table[i] = mod - (int)(mod / i * table[mod % i] % mod);
        return table;
    }

    public static long LongPow(long n, long p)
    {
        long result = 1;
        long b = n;
        while (p > 0) {
            if ((p & 1) == 1) result *= b;
            p >>= 1;
            b *= b;
        }

        return result;
    }

    public static long ModPow(long n, long p, long mod)
    {
        long result = 1;
        long b = n;
        while (p > 0) {
            if ((p & 1) == 1) result = Mult(result, b, mod);
            p >>= 1;
            b = Mult(b, b, mod);
        }

        return result;
    }

    public static long Pow2Prime(long a, long b, long c, long mod) => ModPow(a, ModPow(b, c, mod - 1), mod);

    public static long Pow2Coprime(long a, long b, long c, long mod)
    {
        long tot = FactorizationSingle.TotientFunctionPR(mod);
        long p = ModPow(b, c, tot);
        return ModPow(a, p, mod);
    }

    public static long ModPow2(long a, long b, long c, long mod)
    {
        // Don't optimze this...
        // ANALYSIS: https://stackoverflow.com/questions/21367824/how-to-evalute-an-exponential-tower-modulo-a-prime
        // We really only need to check if b^c > log2(mod) or 64
        if (c * Log(b) < Log(long.MaxValue))
            return ModPow(a, LongPow(b, c), mod);

        long tot = FactorizationSingle.TotientFunctionPR(mod);
        long p = ModPow(b, c, tot);

        long result = p + tot >= 0
            ? ModPow(a, p + tot, mod)
            : ModPow(a, p, mod) * ModPow(a, tot, mod) % mod;

        return result;
    }

    // SOURCE: http://www.dms.umontreal.ca/~andrew/PDF/BinCoeff.pdf
    public static long LargeComb(long n, long k, long p,
        Func<long, long, long> comb)
    {
        // p must be prime
        long n1 = (n + p - 1) / p;
        long k1 = (k + p - 1) / p;
        long n2 = n % p;
        long k2 = k % p;
        return Mult(comb(n1, k1), comb(n2, k2), p);
    }

    // SOURCE: https://www37.atwiki.jp/uwicoder/pages/2118.html
    public static long C(int n, int r, int p)
    {
        long ret = 1;
        while (true) {
            if (r == 0)
                break;
            int N = n % p;
            int R = r % p;
            if (N < R)
                return 0;

            for (int i = 0; i < R; i++) ret = ret * (N - i) % p;
            long imul = 1;
            for (int i = 0; i < R; i++) imul = imul * (i + 1) % p;
            ret = ret * ModInverse(imul, p) % p;
            n /= p;
            r /= p;
        }

        return ret;
    }

    // n<10^7  O(n/lg n)
    public static long C(int n, int r, int mod, int[] primes)
    {
        if (n < 0 || r < 0 || r > n)
            return 0;
        if (r > n / 2)
            r = n - r;
        int[] a = new int[n];
        for (int i = 0; i < r; i++)
            a[i] = n - i;

        foreach (int p in primes) {
            if (p > r)
                break;
            for (long q = p; q <= r; q *= p) {
                long m = n % q;
                for (long i = m, j = 0; j < r / q; i += q, j++) a[i] /= p;
            }
        }

        long mul = 1;
        for (int i = 0; i < r; i++) mul = mul * a[i] % mod;
        return mul;
    }

    // Mod = 4*x + 3
    public static long SqrtModSpecial(long n, long MOD)
    {
        Debug.Assert(MOD % 4 == 3);

        // Try "+(n^((p + 1)/4))"
        n = n % MOD;
        long x = ModPow(n, (MOD + 1) / 4, MOD);
        if (x * x % MOD == n)
            return x;

        // -(n ^ ((p + 1)/4))
        x = MOD - x;
        if (x * x % MOD == n)
            return x;

        return -1;
    }

    // Solves for a^x = b mod m
    public static int DiscreteLog(int a, int b, int m)
    {
        Debug.Assert(a >= 0 && a < m);
        Debug.Assert(b >= 0 && b < m);

        if (b == 1)
            return m != 1 ? 0 : -1;

        // Handle non-coprime a and b
        int count = 0;
        int t = 1;
        for (int g = Gcd(a, m); g != 1; g = Gcd(a, m)) {
            if (b % g != 0) return -1;
            b /= g;
            m /= g;
            t = (int)((long)t * a / g % m);
            count++;
            if (b == t) return count;
        }

        var hash = new Dictionary<int, int>();
        int n = (int)Sqrt(m) + 1;
        int ban = b;
        for (int i = 0; i < n; i++) {
            hash[ban] = i;
            ban = (int)((long)ban * a % m);
        }

        int an = ModPow(a, n, m);
        int tan = t;
        for (int i = 0; i <= n; i++) {
            tan = (int)((long)tan * an % m);
            if (hash.ContainsKey(tan))
                return (i + 1) * n - hash[tan] + count;
        }

        return -1;
    }

    public static long ParseBigInteger(string s, long mod)
    {
        const long chop = 10L * 1000L * 1000L * 1000L * 1000L * 1000L;
        long result = 0;
        foreach (char c in s) {
            if (result >= chop) result %= mod;
            result = result * 10 + (c - '0');
        }

        return result % mod;
    }

    #region Experiment

    static void Mult64(ulong u, ulong v, out ulong whi, out ulong wlo)
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

    /// <summary>
    ///     Divides (x || y) by z, for 64-bit integers x, y,
    ///     and z, giving the remainder(modulus) as the result.
    /// </summary>
    /// <param name="dividendHi">Hi word of dividend</param>
    /// <param name="dividendLo">Lo word of dividend</param>
    /// <param name="divisor">Divisor or modulus</param>
    /// <param name="quotientHi">Hi word of quotient</param>
    /// <param name="quotientLo">Lo word of quotient</param>
    /// <returns>The remainder of the division</returns>
    public static ulong Div64(ulong dividendHi, ulong dividendLo,
        ulong divisor,
        out ulong quotientHi, out ulong quotientLo)
    {
        ulong remainder = dividendHi % divisor;
        for (int i = 1; i <= 64; i++) {
            long t = (long)remainder >> 63; // All 1's if x(63) = 1.
            remainder = (remainder << 1) | (dividendLo >> 63); // Shift x || y left
            dividendLo = dividendLo << 1;
            if ((remainder | (ulong)t) >= divisor) {
                remainder = remainder - divisor;
                dividendLo = dividendLo + 1;
            }
        }

        quotientHi = dividendHi / divisor;
        quotientLo = dividendLo;
        return remainder;
    }

    #endregion
}