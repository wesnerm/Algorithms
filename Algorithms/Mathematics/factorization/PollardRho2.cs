using static System.Math;
using static Algorithms.Mathematics.ModularMath;
using static Algorithms.Mathematics.NumberTheory;

namespace Algorithms.Mathematics;

// https://comeoncodeon.wordpress.com/2010/09/18/pollard-rho-brent-integer-factorization/

public class PollardRho2
{
    public long PollardRho(long N)
    {
        if (N % 2 == 0)
            return 2;

        var random = new Random();
        var ni = N > int.MaxValue ? int.MaxValue : (int) N;
        long x = random.Next(1, ni);
        var y = x;
        var c = random.Next(1, ni);

        var g = 1L;
        while (g == 1)
        {
            x = (Mult(x, x, N) + c) % N;
            y = (Mult(y, y, N) + c) % N;

            y = ((y * y) % N + c) % N;
            g = Gcd(Abs(x - y), N);
        }
        return g;
    }

    public long PollardRhoBrent(long n)
    {
        if (n % 2 == 0)
            return 2;

        var random = new Random();
        var y = (long)(random.NextDouble() * (n - 1)) + 1;
        var c = (long)(random.NextDouble() * (n - 1)) + 1;
        var m = (long)(random.NextDouble() * (n - 1)) + 1;
        var d = 1L;
        var r = 1L;
        var q = 1L;
        long ys = 0L;
        long x = 0L;
        while (d == 1)
        {
            x = y;
            for (int i = 0; i < r; i++)
            {
                y = (Mult(y, y, n) + c);
                if (y >= n) y -= n;
            }

            var k = 0L;
            while (k < r && d == 1)
            {
                ys = y;
                for (int i = 0; i < m && i < r - k; i++)
                {
                    y = (Mult(y, y, n) + c);
                    if (y >= n) y -= n;
                    q = q * (Abs(x - y)) % n;
                }
                d = Gcd(q, n);
                k = k + m;
            }
            r = r * 2;
        }

        if (d == n)
        {
            while (true)
            {
                ys = (Mult(ys, ys, n) + c);
                if (ys >= n) ys -= n;
                d = Gcd(Abs(x - ys), n);
                if (d > 1) break;
            }
        }
        return d;
    }
}
