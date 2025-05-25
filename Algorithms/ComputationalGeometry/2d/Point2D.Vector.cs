#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using Algorithms.Mathematics;

#endregion

namespace Algorithms.ComputationalGeometry;

/// <summary>
///     Summary description for Point2D Vector.
/// </summary>
public partial struct Point2D
{
    public const double Eps = 1e-9;

    #region Object Overrides

    public static Point2D Parse(string text)
    {
        string[] terms = text.Trim().Split(',');
        if (terms.Length != 2)
            throw new FormatException();
        return new Point2D(
            double.Parse(terms[0]),
            double.Parse(terms[1]));
    }

    #endregion

    #region Properties

    public bool IsEmpty => X == 0 && Y == 0;

    public double Length => Math.Sqrt(X * X + Y * Y);

    public double Norm => X * X + Y * Y;

    #endregion

    #region Operations

    public void Negate()
    {
        X = -X;
        Y = -Y;
    }

    public static Point2D Interpolate(Point2D vec1, Point2D vec2,
        double a) =>
        new(vec1.X + a * (vec2.X - vec1.X),
            vec1.Y + a * (vec2.Y - vec1.Y));

    public static Point2D Combine(double factor1, Point2D pt1,
        double factor2, Point2D pt2) =>
        new(factor1 * pt1.X + factor2 * pt2.X,
            factor1 * pt1.Y + factor2 * pt2.Y);

    public Point2D Normalize()
    {
        double d = X * X + Y * Y;
        return d > Numbers.Epsilon && d != 1.0 ? new Point2D(X / d, Y / d) : this;
    }

    public double Angle() => Math.Atan2(Y, X);

    public Point2D Perp() => new(-Y, X);

    public double Angle(Point2D v2) => Math.Atan2(Cross(v2), Dot(v2));

    public static Point2D Projection(Point2D vector, Point2D onto) => vector.Dot(onto) / onto.Norm * onto;
    // Alternatively, vector * onto / vector.Length

    public static Point2D Perpendicular(Point2D vector, Point2D onto) => vector - Projection(vector, onto);

    public static double Area(Point2D v1, Point2D v2) => v1.Cross(v2);

    public bool IsPerpendicularTo(Point2D vector) => Dot(vector) == 0;

    public bool IsParallelTo(Point2D vector) => Cross(vector) == 0;

    #endregion

    public bool IsCloseTo(Point2D vec) => Math.Abs(X - vec.X) <= Eps && Math.Abs(Y - vec.Y) <= Eps;

    public Point2D Intersect(Point2D a1, Point2D d1, Point2D a2, Point2D d2) =>
        a1 + d2.Cross(a2 - a1) / d2.Cross(d1) * d1;

    public double Distance(Point2D rhs)
    {
        double dx = X - rhs.X;
        double dy = Y - rhs.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public double Distance2(Point2D rhs)
    {
        double dx = X - rhs.X;
        double dy = Y - rhs.Y;
        return dx * dx + dy * dy;
    }
}