using static System.Math;
using static Algorithms.Mathematics.NumberTheory;

namespace Algorithms.Mathematics;

public class FactorizationSingle
{
    /// <summary>
    ///     O(sqrt(n))
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Dictionary<long, int> PrimeFactors(long n)
    {
        var dict = new Dictionary<long, int>();
        if (MillerRabin.IsPrime(n))
            return dict;

        int cnt = 0;
        while (n % 2 == 0) {
            n /= 2;
            cnt++;
        }

        if (cnt > 0)
            dict[2] = cnt;

        for (int i = 3; i * i <= n; i += 2) {
            cnt = 0;
            while (n % i == 0) {
                n /= i;
                cnt++;
            }

            if (cnt > 0)
                dict[i] = cnt;
        }

        if (n != 1)
            dict[n] = 1;

        return dict;
    }

    public static List<long> Factor(long n)
    {
        Dictionary<long, int> primes = PrimeFactors(n);

        var list = new List<long> { 1 };
        foreach (KeyValuePair<long, int> pair in primes) {
            int count = list.Count;

            long f = pair.Key;
            for (int j = 0; j < pair.Value; j++, f *= pair.Key)
            for (int i = 0; i < count; i++)
                list.Add(list[i] * f);
        }

        return list;
    }

    public static long FermatFactor(long n)
    {
        long a = (long)Ceiling(Sqrt(n));
        long b2 = a * a - n;
        while (true) {
            long sqrt = (long)Sqrt(b2);
            if (sqrt * sqrt == b2)
                break;
            a = a + 1;
            b2 = a * a - n;
        }

        return a - (int)Sqrt(b2);
    }

    public static long TotientFunction(long n)
    {
        if (MillerRabin.IsPrime(n))
            return n - 1;

        long result = n;

        if ((n & 1) == 0) {
            while ((n & 1) == 0) n >>= 1;
            result >>= 1;
        }

        for (int p = 3; p * p <= n; p += 2)
            if (n % p == 0) {
                while (n % p == 0) n /= p;
                result -= result / p;
            }

        if (n > 1)
            result -= result / n;
        return result;
    }

    public static long TotientFunctionPR(long n)
    {
        List<long> factors = Factorize(n);

        long result = n;

        foreach (long f in factors)
            result -= result / f;

        return result;
    }

    /// <summary>
    ///     Computes Carmichael's Lambda function
    ///     of n - the smallest exponent e such that b** e = 1 for all b coprime to n.
    ///     Otherwise defined as the exponent of the group of integers mod n.
    /// </summary>
    /// <param name="n">The n.</param>
    public static long CarmichaelLambda(long n)
    {
        List<long> thefactors = Factorize(n).ToList();
        thefactors.Sort();
        thefactors.Add(0); // Mark the end of the list of factors
        long carlambda = 1; // The Carmichael Lambda function of n
        long carlambdaComp = 1; // The Carmichael Lambda function of the component p**e
        long oldfact = 1;
        foreach (long fact in thefactors)
            if (fact == oldfact) {
                carlambdaComp = carlambdaComp * fact;
            } else {
                if (oldfact == 2 && carlambdaComp == 4)
                    carlambdaComp >>= 1; // Z_(2**e) is not cyclic for e>=3
                carlambda = carlambda == 1
                    ? carlambdaComp
                    : carlambda * carlambdaComp /
                      Gcd(carlambda, carlambdaComp);
                carlambdaComp = fact - 1;
                oldfact = fact;
            }

        return carlambda;
    }

    public static int Degree(long n, long factor)
    {
        int degree = 0;
        while (n != 0 && n % factor == 0) {
            n /= factor;
            degree++;
        }

        return degree;
    }

    /// <summary>
    ///     Produces a factor in O)(n^(1/4)).
    /// </summary>
    /// <param name="n">The number to factorize... It must be composite.</param>
    /// <remarks>
    ///     https://en.wikipedia.org/wiki/Pollard's_rho_algorithm.
    ///     Additional optimizations available.
    /// </remarks>
    /// <returns></returns>
    public static long PollardsRhoFactorization(long n)
    {
        if (n == 1) return n;
        if ((n & 1) == 0) return 2;
        if (MillerRabin.IsPrime(n)) return n;

        int ni = (int)Min(n, int.MaxValue);
        var r = new Random();

        long d;
        do {
            long c = r.Next(1, ni);
            long x = r.Next(2, ni);
            long y = x;

            do {
                x = (ModularMath.Mult(x, x, n) + c) % n;
                y = (ModularMath.Mult(y, y, n) + c) % n;
                y = (ModularMath.Mult(y, y, n) + c) % n;
                d = Gcd(Abs(x - y), n);
            } while (d == 1);
        } while (d == n);

        return d;
    }

    public static List<long> FullFactorize(long n)
    {
        var result = new List<long> { 1 };
        List<long> factors = Factorize(n);
        foreach (long f in factors) {
            if (f <= 1) continue;
            int start = 0;
            while (n % f == 0) {
                n /= f;
                int end = result.Count;
                for (; start < end; start++)
                    result.Add(result[start] * f);
            }
        }

        result.Sort();
        return result;
    }

    public static List<long> Factorize(long n)
    {
        var list = new List<long>();
        Factorize(list, n);
        list.Sort();
        return list;
    }

    static void Factorize(List<long> list, long n)
    {
        while (n > 1) {
            long factor = PollardsRhoFactorization(n);
            while (n % factor == 0) n /= factor;

            long gcd;
            while ((gcd = Gcd(n, factor)) != 1) n /= gcd;

            if (n != 1) {
                Factorize(list, factor);
            } else {
                list.Add(factor);
                return;
            }
        }
    }

    /// <summary>
    ///     Counts the number of divisors in O(n^(1/3)).
    /// </summary>
    /// <remarks>http://codeforces.com/blog/entry/22317</remarks>
    /// <returns></returns>
    public static int CountOfDivisors(int n, int[] primes)
    {
        int ret = 1;
        for (int i = 0; i < primes.Length && i * i * i <= n; i++) {
            int count = 0;
            while (n % primes[i] == 0) {
                count++;
                n /= primes[i];
            }

            ret *= count + 1;
        }

        if (n == 1) return ret;
        long r = MathUtil.Sqrt(n);
        if (r * r == n) return 3 * ret;
        if (MillerRabin.IsPrime(n)) return 2 * ret;
        return 4 * ret;
    }
}