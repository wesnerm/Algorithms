namespace Algorithms.Mathematics;

public static class NumberTheory
{
    public static int ModulusInverse(int x, int mod) => (int)ModulusInverse(x, (long)mod);

    public static long ModulusInverse(long x, long mod)
    {
        long gcd = ExtendedGcd(x, mod, out long xFactor, out long modFactor);
        Debug.Assert(gcd == 1);

        long inverse = Modulus(xFactor, mod);
        Debug.Assert(inverse * x % mod == 1);
        return inverse;
    }

    public static double ModulusInverse(double x, double mod)
    {
        double gcd = ExtendedGcd(x, mod, out double xFactor, out _);
        Debug.Assert(gcd.AreClose(1.0));

        double inverse = Modulus(xFactor, mod);
        Debug.Assert(1d.AreClose(inverse * x % mod));
        return inverse;
    }

    public static int Modulus(int x, int mod) => (int)Modulus((long)x, mod);

    public static long Modulus(long x, long mod)
    {
        // Hint: Not true modulus
        // -x % y == - ( (-x) % y )
        long result = x % mod;
        if (result < 0)
            result = mod + result;
        return result;
    }

    public static double Modulus(double x, double mod)
    {
        // Hint: Not true modulus
        // -x % y == - ( (-x) % y )
        double result = x % mod;
        if (result < 0)
            result = mod + result;
        return result;
    }

    public static int ExtendedGcd(int x, int y, out int xFactor, out int yFactor)
    {
        long gcd = ExtendedGcd(x, y, out long xFactor2, out long yFactor2);
        xFactor = (int)xFactor2;
        yFactor = (int)yFactor2;
        return (int)gcd;
    }

    public static long Gcd(long x, long y, out long xf, out long yf)
    {
        if (x == 0) {
            xf = 0;
            yf = 1;
            return y;
        }

        long gcd = Gcd(y % x, x, out yf, out long xf2);
        xf = xf2 - yf * (y / x);
        return gcd;
    }

    public static long ExtendedGcd(long x, long y, out long xFactor, out long yFactor)
    {
        long gcd;

        if (x > y)
            return ExtendedGcd(y, x, out yFactor, out xFactor);

        if (x < 0) {
            gcd = ExtendedGcd(-x, y, out xFactor, out yFactor);
            xFactor = -xFactor;
            yFactor = -yFactor;
            Debug.Assert(xFactor * x + yFactor * y == gcd);
            return gcd;
        }

        if (x == y || x <= 1) {
            xFactor = 1;
            yFactor = 0;
            gcd = x;
            Debug.Assert(xFactor * x + yFactor * y == gcd);
            return gcd;
        }

        long quot = y / x;
        long rem = y % x;
        Debug.Assert(quot * x + rem == y);

        gcd = ExtendedGcd(rem, x, out yFactor, out long xFactor2);

        // yFactor2 * x + xfactor2 * rem = gcd
        // quot * x + rem = y 
        // ---------------------
        // xFactor * x + yFactor2 * (y - quot *x) = gcd
        // (xFactor2 - yFactor * quot)*x + yFactor * y = gcd

        xFactor = xFactor2 - yFactor * quot;
        Debug.Assert(xFactor * x + yFactor * y == gcd);
        return gcd;
    }

    public static double ExtendedGcd(double x, double y, out double xFactor, out double yFactor)
    {
        double gcd;

        if (x > y)
            return ExtendedGcd(y, x, out yFactor, out xFactor);

        if (x < 0) {
            gcd = ExtendedGcd(-x, y, out xFactor, out yFactor);
            xFactor = -xFactor;
            yFactor = -yFactor;
            Debug.Assert(gcd.AreClose(xFactor * x + yFactor * y));
            return gcd;
        }

        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (x == y || x <= 1d)
            // ReSharper restore CompareOfFloatsByEqualityOperator
        {
            xFactor = 1;
            yFactor = 0;
            gcd = x;
            Debug.Assert(gcd.AreClose(xFactor * x + yFactor * y));
            return gcd;
        }

        double quot = y / x;
        double rem = y % x;
        Debug.Assert(y.AreClose(quot * x + rem));

        gcd = ExtendedGcd(rem, x, out yFactor, out double xFactor2);

        // yFactor2 * x + xfactor2 * rem = gcd
        // quot * x + rem = y 
        // ---------------------
        // xFactor * x + yFactor2 * (y - quot *x) = gcd
        // (xFactor2 - yFactor * quot)*x + yFactor * y = gcd

        xFactor = xFactor2 - yFactor * quot;
        Debug.Assert(gcd.AreClose(xFactor * x + yFactor * y));
        return gcd;
    }

    /// <summary>
    ///     Solves for positive integer solutions where ax + by = n
    /// </summary>
    /// <param name="a">The coefficient of x.</param>
    /// <param name="b">The coefficient of y.</param>
    /// <param name="n">n.</param>
    /// <param name="x">The x output variable.</param>
    /// <param name="y">The y output variable.</param>
    /// <param name="positiveSolutionsOnly"></param>
    /// <returns>true if positive solutions found</returns>
    public static bool LinearDiophantineSolve(long a, long b, long n,
        out long x, out long y, bool positiveSolutionsOnly = false)
    {
        long g = Gcd(a, b, out x, out y);
        if (n % g != 0)
            return false;

        long div = n / g;
        x *= div;
        y *= div;

        if (positiveSolutionsOnly) {
            double d = g;
            if (x <= 0) {
                long k = (long)Math.Ceiling((1 - x) / (b / d));
                x += k * b / g;
                y -= k * a / g;
            } else if (y <= 0) {
                long k = (long)Math.Ceiling((1 - y) / (a / d));
                x -= k * b / g;
                y += k * a / g;
            }

            return x > 0 && y > 0;
        }

        return true;
    }

    #region Prime Values

    static readonly int[] primes =
    {
        3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
        1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143,
        14591, 17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631,
        130363, 156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689,
        672827, 807403, 968897, 1162687, 1395263, 1674319, 2009191, 2411033,
        2893249, 3471899, 4166287, 4999559, 5999471, 7199369,
    };

    public static bool IsPrime(int candidate)
    {
        if (candidate <= 2)
            return candidate == 2;

        if ((candidate & 1) != 0) {
            int limit = (int)Math.Sqrt(candidate);
            for (int divisor = 3; divisor <= limit; divisor += 2)
                if (candidate % divisor == 0)
                    return false;
            return true;
        }

        return false;
    }

    [DebuggerStepThrough]
    public static int GetPrime(int minSize)
    {
        for (int i = 0; i < primes.Length; i++) {
            int size = primes[i];
            if (size >= minSize) return size;
        }

        for (int j = minSize | 1; j < int.MaxValue; j += 2)
            if (IsPrime(j))
                return j;

        return minSize;
    }

    public static int ModulusRoot(int n, int root, int mod)
    {
        int pow = ModulusInverse(root, mod - 1);
        return ModulusPow(n, pow, mod);
    }

    public static int ModulusPow(int n, int pow, int mod)
    {
        if (pow <= 1)
            switch (pow) {
                case 0:
                    return 1;
                case 1:
                    return n;
                default:
                    return ModulusPow(ModulusInverse(n, mod), -pow, mod);
            }

        if (n <= 1)
            switch (n) {
                case 0:
                    return 0;
                case 1:
                    return 1;
                default:
                    return ModulusPow(Modulus(n, mod), pow, mod);
            }

        //if (pow >= mod - 1)
        //  return ModulusPow(n, Phi(mod), mod); // Phi = cardinality of all invertible elements in Z mod

        if (n >= mod)
            return ModulusPow(Modulus(n, mod), pow, mod);

        int result = ModulusPow(n, pow / 2, mod);
        result = result * result % mod;
        if ((pow & 1) == 1)
            result = result * n % mod;
        return result;
    }

    public static int ModulusLog(int n, int b, int mod)
    {
        int current = b;
        int count = 1;

        while (current != 0) {
            if (current == n)
                return count;

            int next = current * b % mod;
            count++;
            if (count > n) return 0;

            current = next;
        }

        return 0;
    }

    #endregion

    #region GCD

    public static int Gcd(int n, int m)
    {
        if (m < 0) m = -m;
        if (n < 0) n = -n;
        if (m == n) return n;

        while (true) {
            if (m == 0) return n;
            n %= m;
            if (n == 0) return m;
            m %= n;
        }
    }

    public static long Gcd(long n, long m)
    {
        while (true) {
            if (m == 0) return n >= 0 ? n : -n;
            n %= m;
            if (n == 0) return m >= 0 ? m : -m;
            m %= n;
        }
    }

    public static ulong Gcd(ulong n, ulong m)
    {
        if (m == n) return n;
        while (true) {
            if (m == 0) return n;
            n %= m;
            if (n == 0) return m;
            m %= n;
        }
    }

    public static double Gcd(double d1, double d2)
    {
        if (!(d1 >= 0)) {
            if (d1 < 0) d1 = -d1;
            else return 1;
        }

        if (!(d2 >= 0)) {
            if (d2 < 0) d2 = -d2;
            else return 1;
        }

        int count = 20;
        do {
            if (d1 == 0) return d2;
            d2 %= d1;
            if (d2 == 0) return d1;
            d1 %= d2;
        } while (--count > 0);

        return 1;
    }

    #endregion

    #region Tetration

    public static long Tetration(long a, long n, long mod)
    {
        if (mod == 1) return 0;
        //if (a % mod == 0) return 0;

        switch (n) {
            case 0: return 1;
            case 1: return a % mod;
            case 2: return ModPow(a, a, mod);
        }

        long simple = TetrateSimply(a, n);
        if (simple != -1)
            return simple % mod;

        long tot = FactorizationSingle.TotientFunctionPR(mod);
        long p = Tetration(a, n - 1, tot);
        return ModPow(a, p + tot, mod);
    }

    static long TetrateSimply(long a, long n)
    {
        if (a < 2) {
            if (a == 0) return (n & 1) == 1 ? 0 : 1;
            if (a == 1) return 1;
        }

        switch (n) {
            case 0: return 1;
            case 1: return a;
            case 2:
            case 3:
            case 4:
                if (a == 2 || (a == 3 && n <= 3))
                    return (long)Math.Pow(a, TetrateSimply(a, n - 1));
                break;
        }

        return -1;
    }

    #endregion
}