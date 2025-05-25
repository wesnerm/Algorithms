using static System.Math;

namespace Algorithms.ComputationalGeometry;

public static class Geometry
{
    const double Eps = 1e-12;

    public const double Degrees = PI / 180.0;
    public const double ToDegrees = 180.0 / PI;

    public static List<Point2D> ConvexHull(IList<Point2D> points)
    {
        var pts = new List<Point2D>(points);
        pts.Sort(); // Sort by X
        var hull = new List<Point2D>(Min(pts.Count, 4));
        for (int i = 0; i < pts.Count; i++) AddToHull(hull, pts[i], 1);
        int limit = hull.Count;
        for (int i = pts.Count - 2; i >= 0; i--) AddToHull(hull, pts[i], limit);
        if (hull.Count >= 2) hull.RemoveAt(hull.Count - 1);
        return hull;
    }

    static void AddToHull(List<Point2D> hull, Point2D p, int limit)
    {
        // Use < for all or <= for minimal
        while (hull.Count > limit && hull[hull.Count - 2].Cross(hull[hull.Count - 1], p) <= 0)
            hull.RemoveAt(hull.Count - 1);
        hull.Add(p);
    }

    public static double SignedArea(IList<Point2D> p)
    {
        // If polygon is simple, area of counterclockwise polygon is positive
        // Aka winding number
        double area = 0;
        for (int i = p.Count - 1, j = 0; i >= 0; j = i--)
            area += p[i].Cross(p[j]);
        return area * 0.5;
    }

    // untested
    public static void FindTangents(Point2D[] poly, Point2D p, out int left, out int right)
    {
        left = right = 0;
        for (int i = 1; i < poly.Length; i++)
            if (p.Cross(poly[right], poly[i]) < 0)
                right = i;
            else if (p.Cross(poly[left], poly[i]) > 0)
                left = i;
    }

    // untested
    public static int FindTangentConvex(Point2D[] poly, Point2D p, int sign)
    {
        // Sign => 1 = counter-clockwise extrema, -1 = clockwise extrema
        // Assumes no collinear set of three points
        int left = 0, right = poly.Length - 1;
        while (left < right) {
            int mid = (left + right) >> 1;
            double cmp = p.Cross(poly[mid], poly[mid + 1]) * sign;
            if (cmp == 0)
                cmp = p.Cross(poly[0], poly[poly.Length - 1]) * sign;
            if (cmp >= 0)
                left = mid + 1;
            else
                right = mid;
        }

        // Check boundary bi-modal case
        if (p.Cross(poly[left], poly[0]) * sign >= 0)
            left = 0;
        if (p.Cross(poly[left], poly[poly.Length - 1]) * sign >= 0)
            left = poly.Length - 1;

        return left;
    }

    public static bool IsCounterClockwiseConvex(IList<Point2D> poly) =>
        poly.Count < 3 || poly[0].Cross(poly[1], poly[2]) > 0;

    public static double MaxDistance2(IList<Point2D> poly)
    {
        double result = 0;
        int n = poly.Count;
        for (int i = 0, j = n < 2 ? 0 : 1; i < j; ++i)
            while (true) {
                int k = j + 1 < n ? j + 1 : 0;
                result = Max(result, poly[i].Distance2(poly[j]));
                if ((poly[i + 1] - poly[i]).Cross(poly[k] - poly[j]) >= 0) break;
                j = k;
            }

        return result;
    }

    static double Dist2(Point2D p, Point2D q) => p.Distance2(q);

    /// <summary>
    ///     rotate a point CCW or CW around the origin
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static Point2D RotateCcw90(Point2D p) => new(-p.Y, p.X);

    public static Point2D RotateCw90(Point2D p) => new(p.Y, -p.X);

    public static Point2D RotateCcw(Point2D p, double t) =>
        new(p.X * Cos(t) - p.Y * Sin(t),
            p.X * Sin(t) + p.Y * Cos(t));

    /// <summary>
    ///     project point c onto line through a and b
    ///     assuming a != b
    /// </summary>
    public static Point2D ProjectPointLine(Point2D a, Point2D b, Point2D c)
    {
        Point2D ab = b - a;
        return a + (c - a).Dot(ab) * ab / ab.Norm;
    }

    /// <summary>
    ///     project point c onto line segment through a and b
    /// </summary>
    public static Point2D ProjectPointSegment(Point2D a, Point2D b, Point2D c)
    {
        Point2D ab = b - a;
        double r = ab.Norm;
        if (Abs(r) < Eps) return a;
        r = ab.Dot(c - a) / r;
        return r < 0 ? a : r > 1 ? b : a + r * (b - a);
    }

    /// <summary>
    ///     compute distance from c to segment between a and b
    /// </summary>
    public static double DistancePointSegment(Point2D a, Point2D b, Point2D c) =>
        Sqrt(Dist2(c, ProjectPointSegment(a, b, c)));

    /// <summary>
    ///     compute distance between point (x,y,z) and plane ax+by+cz=d
    /// </summary>
    public static double DistancePointPlane(double x, double y, double z,
        double a, double b, double c, double d) =>
        Abs(a * x + b * y + c * z - d) / Sqrt(a * a + b * b + c * c);

    /// <summary>
    ///     determine if lines from a to b and c to d are parallel or collinear
    /// </summary>
    public static bool LinesParallel(Point2D a, Point2D b, Point2D c, Point2D d) => Abs((b - a).Cross(c - d)) < Eps;

    public static bool LinesCollinear(Point2D a, Point2D b, Point2D c, Point2D d) =>
        LinesParallel(a, b, c, d)
        && Abs((a - b).Cross(a - c)) < Eps
        && Abs((c - d).Cross(c - a)) < Eps;

    /// <summary>
    ///     determine if line segment from a to b intersects with
    ///     line segment from c to d
    /// </summary>
    public static bool SegmentsIntersect(Point2D a, Point2D b, Point2D c, Point2D d)
    {
        if (LinesCollinear(a, b, c, d)) {
            if (a.Distance2(c) < Eps || a.Distance2(d) < Eps ||
                b.Distance2(c) < Eps || b.Distance2(d) < Eps) return true;
            return (c - a).Dot(c - b) <= 0 || (d - a).Dot(d - b) <= 0 || (c - b).Dot(d - b) <= 0;
        }

        Point2D ab = b - a;
        if ((d - a).Cross(ab) * (c - a).Cross(ab) > 0) return false;
        Point2D cd = d - c;
        return (a - c).Cross(cd) * (b - c).Cross(cd) <= 0;
    }

    /// <summary>
    ///     compute intersection of line passing through a and b
    ///     with line passing through c and d, assuming that unique
    ///     intersection exists; for segment intersection, check if
    ///     segments intersect first
    /// </summary>
    public static Point2D ComputeLineIntersection(Point2D a, Point2D b, Point2D c, Point2D d)
    {
        Point2D ab = b - a;
        Point2D dc = c - d;
        Point2D ac = c - a;
        Debug.Assert(ab.Norm > Eps && dc.Norm > Eps);
        return a + ac.Cross(dc) * ab / ab.Cross(dc);
    }

    /// <summary>
    ///     compute center of circle given three points
    /// </summary>
    public static Point2D ComputeCircleCenter(Point2D a, Point2D b, Point2D c)
    {
        b = (a + b) / 2;
        c = (a + c) / 2;
        return ComputeLineIntersection(b, b + RotateCw90(a - b), c, c + RotateCw90(a - c));
    }

    /// <summary>
    ///     compute intersection of line through points a and b with
    ///     circle centered at c with radius r > 0
    /// </summary>
    public static List<Point2D> CircleLineIntersection(Point2D a, Point2D b, Point2D c, double r)
    {
        var ret = new List<Point2D>();
        Point2D ab = b - a;
        Point2D ca = a - c;
        double A = ab.Norm;
        double B = ca.Dot(ab);
        double C = ca.Norm - r * r;
        double d = B * B - A * C;
        if (d < -Eps) return ret;
        ret.Add(c + ca + (-B + Sqrt(d + Eps)) * ab / A);
        if (d > Eps)
            ret.Add(c + ca + (-B - Sqrt(d)) * ab / A);
        return ret;
    }

    /// <summary>
    ///     compute intersection of circle centered at a with radius r
    ///     with circle centered at b with radius R
    /// </summary>
    public static List<Point2D> CircleCircleIntersection(Point2D a, Point2D b, double r, double R)
    {
        var ret = new List<Point2D>();
        double d = a.Distance(b);
        if (d > r + R || d + Min(r, R) < Max(r, R)) return ret;
        double x = (d * d - R * R + r * r) / (2 * d);
        double y = Sqrt(r * r - x * x);
        Point2D v = (b - a) / d;
        ret.Add(a + x * v + y * RotateCcw90(v));
        if (y > 0)
            ret.Add(a + x * v - y * RotateCcw90(v));
        return ret;
    }

    public static Point2D ComputeCentroid(List<Point2D> p)
    {
        var c = new Point2D(0, 0);
        double scale = 6.0 * SignedArea(p);
        for (int i = p.Count - 1, j = 0; i >= 0; j = i--)
            c = c + (p[i].X * p[j].Y - p[j].X * p[i].Y) * (p[i] + p[j]);
        return c / scale;
    }

    public static bool IsSimple(List<Point2D> p)
    {
        for (int i = p.Count - 1, j = 0; i >= 0; j = i--)
        for (int k = p.Count - 1, l = 0; k > i; l = k--)
            if (i != l && j != k && SegmentsIntersect(p[i], p[j], p[k], p[l]))
                return false;
        return true;
    }

    public static void Spherical(double x, double y, double z,
        out double r, out double lat, out double lon)
    {
        r = Sqrt(x * x + y * y + z * z);
        lat = ToDegrees * Asin(z / r);
        lon = ToDegrees * Acos(x / Sqrt(x * x + y * y));
    }

    public static void Rectangular(double r, double lat, double lon,
        out double x, out double y, out double z)
    {
        double dlon = lon * Degrees;
        double dlat = lat * Degrees;
        double rcoslat = r * Cos(dlat);
        x = rcoslat * Cos(dlon);
        y = rcoslat * Sin(dlon);
        z = r * Sin(dlat);
    }

    /// <summary>
    ///     Slow but simple Delaunay triangulation. Does not handle
    ///     degenerate cases (from O'Rourke, Computational Geometry in C)
    ///     Running time: O(n^4)
    /// </summary>
    /// <param name="x">x-coordinates</param>
    /// <param name="y">y-coordinates</param>
    /// <returns>
    ///     triples = a vector containing m triples of indices
    ///     corresponding to triangle vertices
    /// </returns>
    public static IEnumerable<Tuple<int, int, int>> DelaunayTriangulation(List<double> x, List<double> y)
    {
        int n = x.Count;
        double[] z = new double[n];

        for (int i = 0; i < n; i++)
            z[i] = x[i] * x[i] + y[i] * y[i];

        for (int i = 0; i < n - 2; i++)
        for (int j = i + 1; j < n; j++)
        for (int k = i + 1; k < n; k++)
            if (j != k) {
                double xn = (y[j] - y[i]) * (z[k] - z[i]) - (y[k] - y[i]) * (z[j] - z[i]);
                double yn = (x[k] - x[i]) * (z[j] - z[i]) - (x[j] - x[i]) * (z[k] - z[i]);
                double zn = (x[j] - x[i]) * (y[k] - y[i]) - (x[k] - x[i]) * (y[j] - y[i]);
                bool flag = zn < 0;
                for (int m = 0; flag && m < n; m++)
                    flag = flag && (x[m] - x[i]) * xn +
                        (y[m] - y[i]) * yn +
                        (z[m] - z[i]) * zn <= 0;
                if (flag)
                    yield return new Tuple<int, int, int>(i, j, k);
            }
    }

    #region Clipping

    //https://www.cs.drexel.edu/~david/Classes/CS430/Lectures/L-05_Polygons.6.pdf

    // Sutherland-Hodgman Algorithm
    // Clips any polygon against a convex clipping region
    // Returns a single simple polygon
    // Clipping a concave polygon can produce multiple polygons
    public static List<Point2D> Clip(Point2D[] convexClip, Point2D[] polygon)
    {
        var orig = new List<Point2D>(polygon);
        var result = new List<Point2D>();
        for (int i = 0; i < convexClip.Length; i++) {
            Swap(ref result, ref orig);
            result.Clear();
            Point2D pt0 = convexClip[i];
            Point2D pt1 = convexClip[i + 1 < convexClip.Length ? i + 1 : 0];
            for (int j = 0; j < orig.Count; j++) {
                Point2D pt2 = orig[j];
                Point2D pt3 = orig[j + 1 < orig.Count ? j + 1 : 0];
                bool inside2 = pt0.Cross(pt1, pt2) > 0;
                bool inside3 = pt0.Cross(pt1, pt3) > 0;
                if (inside2 || inside3) {
                    if (inside2)
                        result.Add(pt2);

                    if (inside2 != inside3) {
                        double s = (pt1 - pt0).Cross(pt3 - pt2);
                        if (s != 0)
                            result.Add(pt0 + pt2.Cross(pt3, pt0) / s * (pt1 - pt0));
                    }
                }
            }
        }

        return result;
    }

    public static Point2D? IntersectLines(Point2D left1, Point2D left2, Point2D right1, Point2D right2)
    {
        double s = (left2 - left1).Cross(right2 - right1);
        return s == 0 ? null : left1 + left1.Cross(right1, left2) / s * (left2 - left1);
    }

    public static double Perimeter(IList<Point2D> pts)
    {
        if (pts.Count < 2) return 0;
        double result = pts[0].Distance(pts[pts.Count - 1]);
        for (int i = 1; i < pts.Count; i++)
            result += pts[i - 1].Distance(pts[i]);
        return result;
    }

    #endregion
}