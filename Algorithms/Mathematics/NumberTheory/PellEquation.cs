using System.Numerics;
using static System.Math;

namespace Algorithms.Mathematics;

public class PellEquation
{
    // Chakravala method
    public static long Pell(long d)
    {
        long p = 1;
        long k = 1;
        long x1 = 1;
        long y = 0;
        long sd = (long)Sqrt(d);
        long x = 0;

        while (k != 1 || y == 0) {
            p = k * (p / k + 1) - p;
            p = p - (p - sd) / k * k;
            x = (p * x1 + d * y) / Abs(k);
            y = (p * y + x1) / Abs(k);
            k = (p * p - d) / k;
            x1 = x;
        }

        return x;
    }

    public static BigInteger Pell(BigInteger d)
    {
        BigInteger p = 1;
        BigInteger k = 1;
        BigInteger x1 = 1;
        BigInteger y = 0;
        var sd = (BigInteger)Sqrt((double)d);
        BigInteger x = 0;

        while (k != 1 || y == 0) {
            p = k * (p / k + 1) - p;
            p = p - (long)((p - sd) / k) * k;
            x = (p * x1 + d * y) / BigInteger.Abs(k);
            y = (p * y + x1) / BigInteger.Abs(k);
            k = (p * p - d) / k;
            x1 = x;
        }

        return x;
    }

    // https://www.hackerrank.com/contests/projecteuler/challenges/euler066/submissions/code/1303425384
    // https://crypto.stanford.edu/pbc/notes/ep/root.html
    // Faster than previous two
    // Solves Pell equation x^2 - N*y^2 = 1 with minimal positive solution (p,q)
    // y^2 = sqrt((x^2-1)/N)
    // other solutions via (x + y sqrt(N)) = (p + q sqrt(N))^n

    public static void Pell(long n, out BigInteger p, out BigInteger q)
    {
        BigInteger p2 = BigInteger.One;
        BigInteger p1 = BigInteger.Zero;
        BigInteger q2 = BigInteger.Zero;
        BigInteger q1 = BigInteger.One;
        BigInteger a0, a1;
        a0 = a1 = (BigInteger)Sqrt(n);
        BigInteger g1 = BigInteger.Zero;
        BigInteger h1 = BigInteger.One;
        BigInteger n0 = n;
        while (true) {
            BigInteger g2 = a1 * h1 - g1;
            BigInteger h2 = (n0 - g2 * g2) / h1;
            BigInteger a2 = (g2 + a0) / h2;
            p = p2 * a1 + p1;
            q = q2 * a1 + q1;
            if (p * p - n0 * (q * q) == 1)
                return;
            a1 = a2;
            g1 = g2;
            h1 = h2;
            p1 = p2;
            p2 = p;
            q1 = q2;
            q2 = q;
        }
    }
}