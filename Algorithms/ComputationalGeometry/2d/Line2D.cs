// https://codeforces.com/blog/entry/48122

namespace Algorithms.ComputationalGeometry;

public struct Line2D
{
    public Point2D A, AB;

    public Line2D(Point2D a, Point2D b, bool twoPoints = true)
    {
        A = a;
        AB = twoPoints ? b - a : b;
    }

    public Point2D B => new(A.X + AB.X, A.Y + AB.Y);

    public override string ToString() => A + "-" + B;

    public static bool operator ==(Line2D lhs, Line2D rhs) => lhs.A == rhs.A && lhs.AB == rhs.AB;

    public static bool operator !=(Line2D lhs, Line2D rhs) => lhs.A != rhs.A || lhs.AB != rhs.AB;

    public bool Equals(Line2D other) => A.Equals(other.A) && AB.Equals(other.AB);

    public override bool Equals(object obj) => obj is Line2D && Equals((Line2D)obj);

    public override int GetHashCode() => unchecked((A.GetHashCode() * 397) ^ AB.GetHashCode());

    public bool OnLine(Point2D point) => AB.IsEmpty ? point == A : AB.Cross(point - A) == 0;

    public bool OnSegment(Point2D point)
    {
        if (AB.IsEmpty) return point == A;
        return point.Cross(A, B) == 0 && point.Dot(A, B) <= 0;
    }

    public double DistLine(Point2D point)
    {
        if (AB.IsEmpty) return A.Distance(point);
        return Math.Abs(AB.Cross(A - point)) / AB.Norm;
    }

    public double DistSegment(Point2D point)
    {
        if (AB.Dot(point - A) <= 0) return point.Distance(A);
        if (AB.Dot(point - B) >= 0) return point.Distance(B);
        return DistLine(point);
    }

    public Point2D Projection(Point2D point)
    {
        Point2D res = A;
        res += AB.Dot(point - A) * AB / AB.Length;
        return res;
    }

    public Point2D Reflection(Point2D point) => 2 * Projection(point) - point;

    public Point2D ClosestPoint(Point2D point)
    {
        if (AB.Dot(point - A) <= 0) return A;
        if (AB.Dot(point - B) >= 0) return B;
        return Projection(point);
    }

    public Point2D? IntersectLines(Line2D rhs)
    {
        double s = AB.Cross(rhs.AB);
        if (s == 0) return null;
        return A + (rhs.A - A).Cross(rhs.AB) / s * AB;
    }

    public Point2D? IntersectClosedSegment(Line2D rhs)
    {
        double s = AB.Cross(rhs.AB);
        if (s == 0) return null;
        Point2D p = rhs.A - A;
        double ls = p.Cross(rhs.AB);
        double rs = p.Cross(AB);
        if (s < 0) {
            s = -s;
            ls = -ls;
            rs = -rs;
        }

        return 0 <= ls && ls <= s && 0 <= rs && rs <= s ? A + ls / s * AB : null;
    }

    public Line2D? IntersectSegments(Line2D rhs)
    {
        double s = AB.Cross(rhs.AB);
        double ls = (rhs.A - A).Cross(rhs.AB);
        if (s == 0) {
            if (ls != 0) return null;
            Point2D lhsa = A, lhsb = B;
            Point2D rhsa = rhs.A, rhsb = rhs.B;
            if (lhsa > lhsb) Swap(ref lhsa, ref lhsb);
            if (rhsa > rhsb) Swap(ref rhsa, ref rhsb);
            var result = new Line2D(lhsa < rhsa ? rhsa : lhsa, lhsb < rhsb ? lhsb : rhsb);
            return !(result.AB < default(Point2D)) ? result : null;
        }

        double rs = (rhs.A - A).Cross(AB);
        if (s < 0) {
            s = -s;
            ls = -ls;
            rs = -rs;
        }

        bool intersect = 0 <= ls && ls <= s && 0 <= rs && rs <= s;
        return intersect ? new Line2D(A + ls / s * AB, new Point2D()) : null;
    }
}