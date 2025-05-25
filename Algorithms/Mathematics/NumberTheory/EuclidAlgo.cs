namespace Algorithms.Mathematics;

public static class EuclidAlgo
{
    // O(sqrt(x)) Exhaustive Primality Test
    public const double EPS = 1e-7;
    // This is a collection of useful code for solving problems that
    // involve modular linear equations.  Note that all of the
    // algorithms described here work on nonnegative integers.

    // computes gcd(a,b)
    public static int Gcd(int a, int b)
    {
        while (true) {
            if (a == 0) return b;
            b %= a;
            if (b == 0) return a;
            a %= b;
        }
    }

    // computes lcm(a,b)
    public static int Lcm(int a, int b) => a / Gcd(a, b) * b;

    // returns d = gcd(a,b); finds x,y such that d = ax + by
    public static int ExtendedEuclid(int a, int b, out int x, out int y)
    {
        int xx = y = 0;
        int yy = x = 1;
        while (b != 0) {
            int q = a / b;
            int t = b;
            b = a % b;
            a = t;
            t = xx;
            xx = x - q * xx;
            x = t;
            t = yy;
            yy = y - q * yy;
            y = t;
        }

        return a;
    }

    public static long ExtendedEuclid(long a, long b, out long x, out long y)
    {
        long xx = y = 0;
        long yy = x = 1;
        while (b != 0) {
            long q = a / b;
            long t = b;
            b = a % b;
            a = t;
            t = xx;
            xx = x - q * xx;
            x = t;
            t = yy;
            yy = y - q * yy;
            y = t;
        }

        return a;
    }

    // https://en.wikipedia.org/wiki/Binary_GCD_algorithm
    public static int BinaryGcd(int u, int v)
    {
        int shift;

        if (u == 0) return v;
        if (v == 0) return u;

        for (shift = 0; ((u | v) & 1) == 0; ++shift) {
            u >>= 1;
            v >>= 1;
        }

        while ((u & 1) == 0)
            u >>= 1;

        do {
            while ((v & 1) == 0)
                v >>= 1;

            if (u > v) {
                int t = v;
                v = u;
                u = t;
            }

            v = v - u;
        } while (v != 0);

        return u << shift;
    }

    // SOURCE: http://www.ucl.ac.uk/~ucahcjm/combopt/ext_gcd_python_programs.pdf
    public static int ExtendedBinaryGcd(int a, int b, out int s, out int t)
    {
        int u = 1;
        int v = 0;
        int r = 0;
        s = 0;
        t = 1;

        while ((a & 1) == 0 && (b & 1) == 0) {
            a >>= 1;
            b >>= 1;
            r++;
        }

        int alpha = a;
        int beta = b;

        while ((a & 1) == 0) {
            a >>= 1;
            if ((u & 1) == 0 && (v & 1) == 0) {
                u >>= 1;
                v >>= 1;
            } else {
                u = (u + beta) >> 1;
                v = (v - alpha) >> 1;
            }
        }

        while (a != b)
            if ((b & 1) == 0) {
                b >>= 1;

                if ((s & 1) == 0 && (t & 1) == 0) {
                    s >>= 1;
                    t >>= 1;
                } else {
                    s = (s + beta) >> 1;
                    t = (t - alpha) >> 1;
                }
            } else if (b < a) {
                int tmp = a;
                a = b;
                b = tmp;

                tmp = u;
                int tmp2 = v;
                u = s;
                v = t;
                s = tmp;
                t = tmp2;
            } else {
                b -= a;
                s -= u;
                t -= v;
            }

        return (1 << r) * a;
    }

    // finds all solutions to ax = b (mod n)
    public static List<int> ModularLinearEquationSolver(int a, int b, int n)
    {
        int x, y;
        var solutions = new List<int>();
        int d = ExtendedEuclid(a, n, out x, out y);
        if (b % d == 0) {
            x = ModularMath.Mod(x * (b / d), n);
            for (int i = 0; i < d; i++) solutions.Add(ModularMath.Mod(x + i * (n / d), n));
        }

        return solutions;
    }

    // Chinese remainder theorem (special case): find z such that
    // z % x = a, z % y = b.  Here, z is unique modulo M = lcm(x,y).
    // Return (z,M).  On failure, M = -1.
    public static (int, int) ChineseRemainderTheorem(int x, int a, int y, int b)
    {
        int s, t;
        int d = ExtendedEuclid(x, y, out s, out t);
        if (a % d != b % d) return (0, -1);
        return (ModularMath.Mod(s * b * x + t * a * y, x * y) / d, x * y / d);
    }

    // Chinese remainder theorem: find z such that
    // z % x[i] = a[i] for all i.  Note that the solution is
    // unique modulo M = lcm_i (x[i]).  Return (z,M).  On 
    // failure, M = -1.  Note that we do not require the a[i]'s
    // to be relatively prime.
    public static (int, int) ChineseRemainderTheorem(List<int> x, List<int> a)
    {
        (int, int) ret = (a[0], x[0]);
        for (int i = 1; i < x.Count; i++) {
            ret = ChineseRemainderTheorem(ret.Item2, ret.Item1, x[i], a[i]);
            if (ret.Item2 == -1) break;
        }

        return ret;
    }

    // computes x and y such that ax + by = c; on failure, x = y =-1
    public static void LinearDiophantine(int a, int b, int c, out int x, out int y)
    {
        int d = Gcd(a, b);
        if (c % d != 0) {
            x = y = -1;
        } else {
            x = c / d * ModularMath.ModInverse(a / d, b / d);
            y = (c - a * x) / b;
        }
    }

    static bool IsPrimeSlow(long x)
    {
        if (x <= 1) return false;
        if (x <= 3) return true;
        if (x % 2 == 0 || x % 3 == 0) return false;
        long s = (long)(Math.Sqrt(x) + EPS);
        for (long i = 5; i <= s; i += 6)
            if (x % i == 0 || x % (i + 2) == 0)
                return false;
        return true;
    }

    #region Chinese Remainder Theorem

    // SOURCE: https://www37.atwiki.jp/uwicoder/pages/2118.html

    public static long[] ExGcd(long a, long b)
    {
        if (a == 0 || b == 0)
            return null;
        int @as = Math.Sign(a);
        int bs = Math.Sign(b);
        a = Math.Abs(a);
        b = Math.Abs(b);
        long p = 1, q = 0, r = 0, s = 1;
        while (b > 0) {
            long c = a / b;
            long d;
            d = a;
            a = b;
            b = d % b;
            d = p;
            p = q;
            q = d - c * q;
            d = r;
            r = s;
            s = d - c * s;
        }

        return new[] { a, p * @as, r * bs };
    }

    public static long Crt(long[] divs, long[] mods)
    {
        long div = divs[0];
        long mod = mods[0];
        for (int i = 1; i < divs.Length; i++) {
            long[] apr = ExGcd(div, divs[i]);
            if ((mods[i] - mod) % apr[0] != 0)
                return -1;
            long ndiv = div * divs[i] / apr[0];
            long nmod = (apr[1] * (mods[i] - mod) / apr[0] * div + mod) % ndiv;
            if (nmod < 0)
                nmod += ndiv;
            div = ndiv;
            mod = nmod;
        }

        return mod;
    }

    #endregion

    //    661   673   677   683   691   701   709   719   727   733   739   743
    //    599   601   607   613   617   619   631   641   643   647   653   659
    //    509   521   523   541   547   557   563   569   571   577   587   593
    //    439   443   449   457   461   463   467   479   487   491   499   503
    //    367   373   379   383   389   397   401   409   419   421   431   433
    //    283   293   307   311   313   317   331   337   347   349   353   359
    //    227   229   233   239   241   251   257   263   269   271   277   281
    //    157   163   167   173   179   181   191   193   197   199   211   223
    //     97   101   103   107   109   113   127   131   137   139   149   151
    //     41    43    47    53    59    61    67    71    73    79    83    89
    //      2     3     5     7    11    13    17    19    23    29    31    37
    // Primes less than 1000:
    //    751   757   761   769   773   787   797   809   811   821   823   827
    //    829   839   853   857   859   863   877   881   883   887   907   911
    //    919   929   937   941   947   953   967   971   977   983   991   997

    // Other primes:
    //    The largest prime smaller than 10 is 7.
    //    The largest prime smaller than 100 is 97.
    //    The largest prime smaller than 1000 is 997.
    //    The largest prime smaller than 10000 is 9973.
    //    The largest prime smaller than 100000 is 99991.
    //    The largest prime smaller than 1000000 is 999983.
    //    The largest prime smaller than 10000000 is 9999991.
    //    The largest prime smaller than 100000000 is 99999989.
    //    The largest prime smaller than 1000000000 is 999999937.
    //    The largest prime smaller than 10000000000 is 9999999967.
    //    The largest prime smaller than 100000000000 is 99999999977.
    //    The largest prime smaller than 1000000000000 is 999999999989.
    //    The largest prime smaller than 10000000000000 is 9999999999971.
    //    The largest prime smaller than 100000000000000 is 99999999999973.
    //    The largest prime smaller than 1000000000000000 is 999999999999989.
    //    The largest prime smaller than 10000000000000000 is 9999999999999937.
    //    The largest prime smaller than 100000000000000000 is 99999999999999997.
    //    The largest prime smaller than 1000000000000000000 is 999999999999999989.
}