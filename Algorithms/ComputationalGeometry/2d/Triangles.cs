namespace Algorithms.ComputationalGeometry._2D;

public class Triangles
{
    // A046079 Number of Pythagorean triangles with leg n.     
    // 0, 0, 1, 1, 1, 1, 1, 2, 2, 1, 1, 4, 1, 1, 4, 3, 1, 2, 1, 4, 4, 1, 1, 7, 2, 1, 3, 4, 1, 4, 1, 4, 4, 1, 4, 7, 1, 1, 4, 7, 1, 4, 1, 4, 7, 1, 1, 10, 2, 2, 4, 4, 1, 3, 4, 7, 4, 1, 1, 13, 1, 1, 7, 5, 4, 4, 1, 4, 4, 4, 1, 12, 1, 1, 7, 4, 4, 4, 1, 10, 4, 1, 1, 13, 4, 1, 4, 7, 1, 7, 4, 4, 4, 1, 4, 13, 1, 2, 7 
    // Number of ways that 2/n can be written as a sum of exactly two distinct unit fractions.For every solution to 2/n = 1 / x + 1 / y, x<y, the Pythagorean triple is (n, y-x, x+y-n). 
    // For n>2, the positions of the ones in this sequence correspond to the prime numbers and their doubles
    // Let L = length of longest leg, H = hypotenuse. For odd n: L =(n^2-1)/2 and H = L+1.  For even n,  L = (n^2-4)/4 and H = L+2.
    // Project Euler 176

    // Euler 9, 39, 139, 176, 86

    public void BasicPythagoreanTriples(Func<long, long, long, bool> action)
    {
        for (long m = 1;; m++)
        for (long n = 1; n < m; n++) {
            if (((n & 1) == 1 && (m & 1) == 1) || Gcd(m, n) != 1) continue;

            long a = m * m - n * n;
            long b = 2 * m * n;
            long c = m * m + n * n;

            long a2 = a < b ? a : b;
            long b2 = a < b ? b : a;

            if (!action(a2, b2, c))
                return;
        }
    }

    static long Gcd(long a, long b) => a == 0 ? b : Gcd(b % a, a);

    public static double Area(double[] p0, double[] p1, double[] p2) =>
        0.5 * (-p1[1] * p2[0] + p0[1] * (-p1[0] + p2[0]) + p0[0] * (p1[1] - p2[1]) + p1[0] * p2[1]);

    static double Sign(double[] p1, double[] p2, double[] p3) =>
        (p1[0] - p3[0]) * (p2[1] - p3[1]) - (p2[0] - p3[0]) * (p1[1] - p3[1]);

    static bool PointInTriangle(double[] pt, double[] v1, double[] v2, double[] v3)
    {
        bool b1 = Sign(pt, v1, v2) <= 0.0;
        bool b2 = Sign(pt, v2, v3) <= 0.0;
        bool b3 = Sign(pt, v3, v1) <= 0.0;
        return b1 == b2 && b2 == b3;
    }

    public static bool PointInTriangle2(double[] p, double[] p0, double[] p1, double[] p2)
    {
        double s, t;
        BaryocentricCoordinates(p, p0, p1, p2, out s, out t);
        return s >= 0 && t >= 0 && 1 - s - t >= 0;
    }

    public static void BaryocentricCoordinates(double[] p, double[] p0, double[] p1, double[] p2, out double s,
        out double t)
    {
        double area = Area(p0, p1, p2);
        double f = 1 / (2 * area);
        s = f * (p0[1] * p2[0] - p0[0] * p2[1] + (p2[1] - p0[1]) * p[0] + (p0[0] - p2[0]) * p[1]);
        t = f * (p0[0] * p1[1] - p0[1] * p1[0] + (p0[1] - p1[1]) * p[0] + (p1[0] - p0[0]) * p[1]);
    }
}