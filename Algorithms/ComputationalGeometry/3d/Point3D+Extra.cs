using Algorithms.Mathematics;

namespace Algorithms.ComputationalGeometry;

/// <summary>
///     Summary description for Point3D.
/// </summary>
public partial struct Point3D
{
    #region Object Overrides

    public bool IsCloseTo(Point3D pt) =>
        X.AreClose(pt.X)
        && Y.AreClose(pt.Y)
        && Z.AreClose(pt.Z);

    #endregion

    #region Properties

    public bool IsOrigin => X == 0 && Y == 0 && Z == 0;

    public static Point3D Origin => new();

    #endregion

    #region Conversions

    [DebuggerStepThrough]
    public static implicit operator Point3D(Point2D pt) => new(pt.X, pt.Y);

    public static explicit operator Vector3D(Point3D pt) => new(pt.X, pt.Y, pt.Z);

    public static explicit operator Point2D(Point3D pt) => new(pt.X, pt.Y);

    public static Point2D[] ToPointArray(Point3D[] points)
    {
        var array = new Point2D[points.Length];
        for (int i = 0; i < array.Length; i++)
            array[i] = (Point2D)points[i];
        return array;
    }

    [DebuggerStepThrough]
    public static Point3D[] ToPoint3DArray(Point2D[] point2Ds)
    {
        var array = new Point3D[point2Ds.Length];
        for (int i = 0; i < array.Length; i++)
            array[i] = point2Ds[i];
        return array;
    }

    #endregion

    #region Operations

    public static Point3D Interpolate(Point3D pt1, Point3D pt2, double a) =>
        new(pt1.X + a * (pt2.X - pt1.X),
            pt1.Y + a * (pt2.Y - pt1.Y),
            pt1.Z + a * (pt2.Z - pt1.Z));

    public static Point3D Combine(double factor1, Point3D pt1,
        double factor2, Point3D pt2) =>
        new(factor1 * pt1.X + factor2 * pt2.X,
            factor1 * pt1.Y + factor2 * pt2.Y,
            factor1 * pt1.Z + factor2 * pt2.Z);

    public static double Distance(Point3D pt1, Point3D pt2) => (pt2 - pt1).Length;

    #endregion

    #region Utility

    public static Point3D Max(Point3D pt1, Point3D pt2) =>
        new(Math.Max(pt1.X, pt2.X),
            Math.Max(pt1.Y, pt2.Y),
            Math.Max(pt1.Z, pt2.Z));

    public static Point3D Min(Point3D pt1, Point3D pt2) =>
        new(Math.Min(pt1.X, pt2.X),
            Math.Min(pt1.Y, pt2.Y),
            Math.Min(pt1.Z, pt2.Z));

    public static void Swap(ref Point3D pt1, ref Point3D pt2)
    {
        Point3D tmp = pt1;
        pt1 = pt2;
        pt2 = tmp;
    }

    #endregion

    #region Math

    public static Vector3D operator -(Point3D pt, Point3D pt2) => new(pt.X - pt2.X, pt.Y - pt2.Y, pt.Z - pt2.Z);

    public static Point3D operator -(Point3D pt, Vector3D vec) => new(pt.X - vec.X, pt.Y - vec.Y, pt.Z - vec.Z);

    public static Point3D operator -(Vector3D vec, Point3D pt) => new(vec.X - pt.X, vec.Y - pt.Y, vec.Z - pt.Z);

    public static Point3D operator +(Point3D pt, Vector3D vec) => new(pt.X + vec.X, pt.Y + vec.Y, pt.Z + vec.Z);

    public static Point3D Parse(string text)
    {
        string[] terms = text.Trim().Split(',');
        if (terms.Length != 3)
            throw new FormatException();
        return new Point3D(
            float.Parse(terms[0]),
            float.Parse(terms[1]),
            float.Parse(terms[2]));
    }

    #endregion
}