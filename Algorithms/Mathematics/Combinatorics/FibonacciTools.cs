namespace Algorithms.Mathematics.Combinatorics;

public class FibonacciTools
{
    //http://www.maths.surrey.ac.uk/hosted-sites/R.Knott/Fibonacci/fibmaths.html#section1
    // F(n)^2 + F(n+1)^2 = F( 2*n + 1 )
    // F(n+m) = F(m-1)*F(n) + F(m)*F(n+1)
    // gcd(F(m),F(n)) = F( gcd(m,n) )

    // T(n) = F(n+1)
    // T(n+m) = T(m-1)*F(n-1) + F(m)*F(n)

    public const double Root5 = 1.224744871391589;
    public const double Phi = (1 + Root5) / 2;

    // http://fedelebron.com/fast-modular-fibonacci
    const int PisanoPeriod = 2000000016;

    public static long FibonacciConstantTime(int n) => (long)Math.Floor(Math.Pow(Phi, n + 1) / Root5 + 0.5);

    public static long FibonacciSum0(long a, long am1, long b, long bm1, long mod)
    {
        long a1 = a + am1;
        if (a1 >= mod) a1 -= mod;
        long result = a * bm1 % mod + a1 * b % mod;
        if (result >= mod) result -= mod;
        return result;
    }

    public static long FibonacciSum1(long a, long am1, long b, long bm1, long mod)
    {
        long result = a * b % mod + am1 * bm1 % mod;
        if (result >= mod) result -= mod;
        return result;
    }

    public static long FibonacciSum(long x, long mod)
    {
        long fib = Fibonacci(x + 2, mod) - 1;
        if (fib < 0) fib += mod;
        return fib;
    }

    // SOURCE: https://en.wikipedia.org/wiki/Fibonacci_number#Matrix_form

    public static long IsFibonacci(long fn)
    {
        // Untested
        long term = 5 * fn * fn + 4;
        long sqrt = (long)Math.Sqrt(term);
        if (sqrt * sqrt != term) {
            term = 5 * fn * fn - 4;
            sqrt = (long)Math.Sqrt(term);
            if (sqrt * sqrt != term)
                return -1;
        }

        double sqrt5 = Math.Sqrt(5);
        double log = Math.Log(0.5 * (fn * sqrt5 + Math.Sqrt(term)));
        return (long)(log / Math.Log(Phi) + .5);
    }

    public static long Fibonacci(long n, long mod)
    {
        long x;
        return Fibonacci(n, out x, mod);
    }

    public static long Fibonacci(long n, out long fnp1, long mod)
    {
        if (n == 0) {
            fnp1 = 1;
            return 0;
        }

        long b;
        long a = Fibonacci(n >> 1, out b, mod);
        long c = 2 * b - a;
        if (c < 0) c += mod;
        c = a * c % mod;
        long d = (a * a + b * b) % mod;
        if ((n & 1) == 0) {
            fnp1 = d;
            return c;
        }

        fnp1 = c + d;
        return d;
    }
}