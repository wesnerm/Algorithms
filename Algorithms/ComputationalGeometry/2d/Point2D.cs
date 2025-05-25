namespace Algorithms.ComputationalGeometry;

[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
public partial struct Point2D : IComparable<Point2D>, IEquatable<Point2D>
{
    public double X, Y;

    public Point2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static Point2D operator +(Point2D p, Point2D p2) => new(p.X + p2.X, p.Y + p2.Y);

    public static Point2D operator -(Point2D p) => new(-p.X, -p.Y);

    public static Point2D operator -(Point2D p, Point2D p2) => new(p.X - p2.X, p.Y - p2.Y);

    public static Point2D operator *(double c, Point2D p) => new(p.X * c, p.Y * c);

    public static Point2D operator /(Point2D p, double c) => new(p.X / c, p.Y / c);

    public override string ToString() => "(" + X + "," + Y + ")";

    public static bool operator <(Point2D lhs, Point2D rhs) => lhs.CompareTo(rhs) < 0;

    public static bool operator >(Point2D lhs, Point2D rhs) => lhs.CompareTo(rhs) > 0;

    public static bool operator <=(Point2D lhs, Point2D rhs) => lhs.CompareTo(rhs) <= 0;

    public static bool operator >=(Point2D lhs, Point2D rhs) => lhs.CompareTo(rhs) >= 0;

    public static bool operator ==(Point2D lhs, Point2D rhs) => lhs.Y == rhs.Y && lhs.X == rhs.X;

    public static bool operator !=(Point2D lhs, Point2D rhs) => lhs.Y != rhs.Y || lhs.X != rhs.X;

    public double Cross(Point2D vector) => X * vector.Y - Y * vector.X;

    public double Dot(Point2D vector) => X * vector.X + Y * vector.Y;

    public double Cross(Point2D a, Point2D b) => (a.X - X) * (b.Y - Y) - (a.Y - Y) * (b.X - X);

    public double Dot(Point2D a, Point2D b) => (a.X - X) * (b.X - X) + (a.Y - Y) * (b.Y - Y);

    public int CompareTo(Point2D point)
    {
        int cmp = X.CompareTo(point.X);
        if (cmp != 0) return cmp;
        return Y.CompareTo(point.Y);
    }

    public bool Equals(Point2D other) => X.Equals(other.X) && Y.Equals(other.Y);

    public override bool Equals(object obj) => obj is Point2D && Equals((Point2D)obj);

    public override int GetHashCode() => unchecked((X.GetHashCode() * 397) ^ Y.GetHashCode());
}