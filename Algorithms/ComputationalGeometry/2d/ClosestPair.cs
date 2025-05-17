using System.Text;
using System.Threading.Tasks;

namespace Algorithms.ComputationalGeometry._2D;

public struct ClosestPair
{
    public double Distance;
    public Point2D Point1, Point2;

    public ClosestPair(Point2D[] points)
    {
        Array.Sort(points, (a, b) => a.X.CompareTo(b.X));
        this = DivideAndConquer(points, 0, points.Length);
    }

    static ClosestPair DivideAndConquer(Point2D[] p, int start, int n)
    {
        if (n <= 4)
            return BruteForce(p, start, n);

        int mid = n >> 1;
        var midPoint2D = p[start + mid];
        var rl = DivideAndConquer(p, start, mid);
        var rr = DivideAndConquer(p, start + mid, n - mid);
        var dmin = rl.Distance <= rr.Distance ? rl : rr;

        var strip = new List<Point2D>();
        // This could be optimized by having two loops going from center outward
        for (int i = 0; i < n; ++i)
            if (Math.Abs(p[start + i].X - midPoint2D.X) < dmin.Distance)
                strip.Add(p[start + i]);

        strip.Sort((a, b) => a.Y.CompareTo(b.Y));

        for (int i = 0; i < strip.Count; ++i)
            for (int j = i + 1; j < strip.Count && strip[j].Y - strip[i].Y < dmin.Distance; ++j)
            {
                var d = strip[i].Distance(strip[j]);
                if (d < dmin.Distance)
                {
                    dmin.Distance = d;
                    dmin.Point1 = strip[i];
                    dmin.Point2 = strip[j];
                }
            }

        return dmin;
    }

    static ClosestPair BruteForce(Point2D[] points, int start, int n)
    {
        var result = new ClosestPair { Distance = double.MaxValue };
        for (int i = 0; i < n; ++i)
            for (int j = i + 1; j < n; ++j)
                if (points[start + i].Distance(points[start + j]) < result.Distance)
                {
                    result.Distance = points[start + i].Distance(points[start + j]);
                    result.Point1 = points[i];
                    result.Point2 = points[j];
                }
        return result;
    }
}
