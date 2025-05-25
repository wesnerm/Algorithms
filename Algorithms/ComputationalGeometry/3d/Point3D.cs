/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the express permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

namespace Algorithms.ComputationalGeometry;

/// <summary>
///     Summary description for Point3D.
/// </summary>
[DebuggerStepThrough]
public partial struct Point3D
{
    #region Variables

    public float X, Y, Z;

    #endregion

    #region Construction

    public Point3D(double x, double y, double z)
    {
        X = (float)x;
        Y = (float)y;
        Z = (float)z;
    }

    public Point3D(double x, double y)
    {
        X = (float)x;
        Y = (float)y;
        Z = 0;
    }

    #endregion

    #region Object Overrides

    public override bool Equals(object obj)
    {
        if (!(obj is Point3D))
            return false;
        return Equals((Point3D)obj);
    }

    public bool Equals(Point3D pt) => X == pt.X && Y == pt.Y && Z == pt.Z;

    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

    public override string ToString() => $"{X},{Y},{Z}";

    #endregion

    #region Math

    public static Point3D operator +(Vector3D vec, Point3D pt) => new(pt.X + vec.X, pt.Y + vec.Y, pt.Z + vec.Z);

    public static Point3D operator -(Point3D pt) => new(-pt.X, -pt.Y, -pt.Z);

    public static Point3D operator *(double s, Point3D pt) => new(pt.X * s, pt.Y * s, pt.Z * s);

    public static Point3D operator *(Point3D pt, double s) => new(pt.X * s, pt.Y * s, pt.Z * s);

    public static Point3D operator /(Point3D pt, double s) => new(pt.X / s, pt.Y / s, pt.Z / s);

    public static bool operator ==(Point3D pt1, Point3D pt2) => pt1.Equals(pt2);

    public static bool operator !=(Point3D pt1, Point3D pt2) => !pt1.Equals(pt2);

    #endregion
}