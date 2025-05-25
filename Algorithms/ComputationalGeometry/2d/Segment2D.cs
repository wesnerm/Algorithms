// https://codeforces.com/blog/entry/48122

namespace Algorithms.ComputationalGeometry;

public struct Segment2D
{
    public Point2D A, B;

    public Segment2D(Point2D a, Point2D b)
    {
        A = a;
        B = b;
    }

    public Point2D AB => new(B.X - A.X, B.Y - A.Y);

    public override string ToString() => A + "-" + B;

    public static bool operator ==(Segment2D lhs, Segment2D rhs) => lhs.A == rhs.A && lhs.B == rhs.B;

    public static bool operator !=(Segment2D lhs, Segment2D rhs) => lhs.A != rhs.A || lhs.B != rhs.B;

    public bool Equals(Segment2D other) => A.Equals(other.A) && B.Equals(other.B);

    public override bool Equals(object obj) => obj is Segment2D && Equals((Segment2D)obj);

    public override int GetHashCode() => unchecked((A.GetHashCode() * 397) ^ B.GetHashCode());

    public bool OnLine(Point2D point) => point.Cross(A, B) == 0;

    public bool OnSegment(Point2D point) => point.Cross(A, B) == 0 && point.Dot(A, B) <= 0;

    public double DistLine(Point2D point) =>
        A.Equals(B) ? A.Distance(point) : Math.Abs(A.Cross(B, point) / A.Distance2(B));

    public double DistSegment(Point2D point)
    {
        if (A.Dot(B, point) <= 0) return point.Distance(A);
        if (B.Dot(A, point) <= 0) return point.Distance(B);
        return DistLine(point);
    }

    public Point2D Projection(Point2D point) => A.Equals(B) ? A : A + A.Dot(B, point) * AB / A.Distance(B);

    public Point2D Reflection(Point2D point) => 2 * Projection(point) - point;

    public Point2D ClosestPoint(Point2D point) =>
        A.Dot(B, point) <= 0 ? A
        : B.Dot(A, point) <= 0 ? B
        : Projection(point);

    public Point2D? IntersectLines(Segment2D s)
    {
        double c = AB.Cross(s.AB);
        return c == 0 ? A + s.A.Cross(s.B, A) * (B - A) / c : null;
    }

    public Point2D? IntersectClosedSegment(Segment2D s)
    {
        Point2D? point = IntersectLines(s);
        return point != null && OnLine(point.Value) && s.OnLine(point.Value)
            ? point
            : null;
    }

    public Segment2D? IntersectSegments(Segment2D s)
    {
        // If they are not on the same line
        if (s.A.Cross(A, B) != 0 || s.B.Cross(A, B) != 0) {
            Point2D? point = IntersectLines(s);
            return point != null && OnLine(point.Value) && s.OnLine(point.Value)
                ? new Segment2D(point.Value, point.Value)
                : null;
        }

        Point2D a = A, b = B;
        if (a > b) Swap(ref a, ref b);
        if (s.A > s.B) Swap(ref s.A, ref s.B);
        if (a < s.A) a = s.A;
        if (b > s.B) b = s.B;
        return a <= b ? new Segment2D(a, b) : null;
    }
}