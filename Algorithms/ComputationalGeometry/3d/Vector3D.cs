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
///     Summary description for Vector.
/// </summary>
public struct Vector3D
{
    #region Construction

    [DebuggerStepThrough]
    public Vector3D(double x, double y, double z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    #endregion

    #region Object Overrides

    public override bool Equals(object obj)
    {
        if (!(obj is Vector3D))
            return false;
        return Equals((Vector3D)obj);
    }

    public bool Equals(Vector3D vec) => X == vec.X && Y == vec.Y && Z == vec.Z;

    public bool IsCloseTo(Vector3D vec) =>
        X.AreClose(vec.X)
        && Y.AreClose(vec.Y)
        && Z.AreClose(vec.Z);

    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

    public override string ToString() => string.Format("{0},{1},{2}", X, Y, Z);

    public static Vector3D Parse(string text)
    {
        string[] terms = text.Trim().Split(',');
        if (terms.Length != 3)
            throw new FormatException();
        return new Vector3D(
            double.Parse(terms[0]),
            double.Parse(terms[1]),
            double.Parse(terms[2]));
    }

    #endregion

    #region Properties

    public string Text => ToString();

    public static Vector3D XAxis {
        [DebuggerStepThrough] get => new(1f, 0);
    }

    public static Vector3D YAxis {
        [DebuggerStepThrough] get => new(0, 1f);
    }

    public static Vector3D ZAxis {
        [DebuggerStepThrough] get => new(0, 0, 1f);
    }

    public double X { [DebuggerStepThrough] get; set; }

    public double Y { [DebuggerStepThrough] get; set; }

    public double Z { [DebuggerStepThrough] get; set; }

    public bool IsZeroVector => X == 0 && Y == 0 && Z == 0;

    public static Vector3D ZeroVector => new();

    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

    public double SquaredLength => X * X + Y * Y + Z * Z;

    #endregion

    #region Conversions

    public static explicit operator Vector3D(double d) => new(d, d, d);

    public static explicit operator Point3D(Vector3D vector) => new(vector.X, vector.Y, vector.Z);

    #endregion

    #region Operations

    public void Negate()
    {
        X = -X;
        Y = -Y;
        Z = -Z;
    }

    public static Vector3D Interpolate(Vector3D vec1, Vector3D vec2,
        double a) =>
        new(vec1.X + a * (vec2.X - vec1.X),
            vec1.Y + a * (vec2.Y - vec1.Y),
            vec1.Z + a * (vec2.Z - vec1.Z));

    public static Vector3D Combine(double factor1, Vector3D pt1,
        double factor2, Vector3D pt2) =>
        new(factor1 * pt1.X + factor2 * pt2.X,
            factor1 * pt1.Y + factor2 * pt2.Y,
            factor1 * pt1.Z + factor2 * pt2.Z);

    public void Normalize()
    {
        double d = X * X + Y * Y + Z * Z;
        if (d > Numbers.Epsilon && d != 1.0) {
            d = Math.Sqrt(d);
            X = X / d;
            Y = Y / d;
            Z = Z / d;
        }
    }

    public double AngleBetween(Vector3D v1, Vector3D v2) =>
        Math.Acos(Dot(v1, v2) / (v1.Length * v2.Length)) * (180 / Math.PI);

    public static Vector3D Projection(Vector3D vector, Vector3D onto) => Dot(vector, onto) / onto.SquaredLength * onto;

    public static Vector3D Perpendicular(Vector3D vector, Vector3D onto) => vector - Projection(vector, onto);

    public static double Area(Vector3D v1, Vector3D v2) => (v1 * v2).Length;

    /// <summary>
    ///     Provides a determinant for the matrix whose rows (or columns)
    ///     are vec1, vec2, vec3
    ///     Also known as the scalar triple product (vec1 * vec2) . vec3
    /// </summary>
    /// <returns></returns>
    public static double Determinant(Vector3D vec1, Vector3D vec2, Vector3D vec3) =>
        vec1.X * (vec2.Y * vec3.Z - vec2.Z * vec3.Y)
        + vec1.Y * (vec2.Z * vec3.X - vec2.X * vec3.Z)
        + vec1.Z * (vec2.X * vec3.Y - vec2.Y * vec3.X);

    public static Matrix3D TensorProduct(Vector3D v1, Vector3D v2) =>
        new(v1.X * v2.X, v1.X * v2.Y, v1.X * v2.Z,
            v1.Y * v2.X, v1.Y * v2.Y, v1.Y * v2.Z,
            v1.Z * v2.X, v1.Z * v2.Y, v1.Z * v2.Z,
            0, 0, 0);

    public static Matrix3D SkewSymmetric(Vector3D v) => new(0, v.Z, -v.Y, -v.Z, 0, v.X, v.Y, -v.X, 0, 0, 0, 0);

    public static double Volume(Vector3D v1, Vector3D v2, Vector3D v3) => Math.Abs(Determinant(v1, v2, v3));

    public bool IsPerpendicularTo(Vector3D vector) => Numbers.IsZeroed(Dot(vector));

    public bool IsParallelTo(Vector3D vector) => Numbers.IsZeroed((this * vector).SquaredLength);

    #endregion

    #region Utility

    public static Vector3D Max(Vector3D vec1, Vector3D vec2) =>
        new(Math.Max(vec1.X, vec2.X),
            Math.Max(vec1.Y, vec2.Y),
            Math.Max(vec1.Z, vec2.Z));

    public static Vector3D Min(Vector3D vec1, Vector3D vec2) =>
        new(Math.Min(vec1.X, vec2.X),
            Math.Min(vec1.Y, vec2.Y),
            Math.Min(vec1.Z, vec2.Z));

    public static void Swap(ref Vector3D vec1, ref Vector3D vec2)
    {
        Vector3D tmp = vec1;
        vec1 = vec2;
        vec2 = tmp;
    }

    #endregion

    #region Math

    public static Point3D operator +(Vector3D vec, Point3D pt) => new(pt.X + vec.X, pt.Y + vec.Y, pt.Z + vec.Z);

    public static Vector3D operator -(Vector3D vec, Vector3D vec2) =>
        new(vec.X - vec2.X, vec.Y - vec2.Y, vec.Z - vec2.Z);

    public static Vector3D operator +(Vector3D vec, Vector3D vec2) =>
        new(vec.X + vec2.X, vec.X + vec2.Y, vec.Z + vec2.Z);

    public static Vector3D operator -(Vector3D vec) => new(-vec.X, -vec.Y, -vec.Z);

    public static Vector3D operator *(double s, Vector3D vec) => new(vec.X * s, vec.Y * s, vec.Z * s);

    public static Vector3D operator *(Vector3D vec, double s) => new(vec.X * s, vec.Y * s, vec.Z * s);

    public static Vector3D operator /(Vector3D vec, double s)
    {
        s = 1 / s;
        return new Vector3D(vec.X * s, vec.Y * s, vec.Z * s);
    }

    public static bool operator ==(Vector3D vec1, Vector3D vec2) => vec1.Equals(vec2);

    public static bool operator !=(Vector3D vec1, Vector3D vec2) => !vec1.Equals(vec2);

    public static Vector3D operator *(Vector3D vec1, Vector3D vec2) =>
        new(
            vec1.Y * vec2.Z - vec1.Z * vec2.Y,
            vec1.Z * vec2.X - vec1.X * vec2.Z,
            vec1.X * vec2.Y - vec1.Y * vec2.Z);

    public static Vector3D Cross(Vector3D vec1, Vector3D vec2) =>
        new(
            vec1.Y * vec2.Z - vec1.Z * vec2.Y,
            vec1.Z * vec2.X - vec1.X * vec2.Z,
            vec1.X * vec2.Y - vec1.Y * vec2.Z);

    public double Dot(Vector3D vector) => X * vector.X + Y * vector.Y + Z * vector.Z;

    public double Dot(Point3D point) => X * point.X + Y * point.Y + Z * point.Z;

    public static double Dot(Vector3D vec1, Vector3D vec2) => vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;

    #endregion
}