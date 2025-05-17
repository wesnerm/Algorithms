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
/// Summary description for Vector.
/// </summary>
public struct Vector3D
{
    #region Vector

    double x;
    double y;
    double z;

    #endregion

    #region Construction

    [DebuggerStepThrough]
    public Vector3D(double x, double y, double z = 0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    #endregion

    #region Object Overrides

    public override bool Equals(object obj)
    {
        if (!(obj is Vector3D))
            return false;
        return Equals((Vector3D)obj);
    }

    public bool Equals(Vector3D vec)
    {
        return x == vec.x && y == vec.y && z == vec.z;
    }

    public bool IsCloseTo(Vector3D vec)
    {
        return x.AreClose(vec.x)
               && y.AreClose(vec.y)
               && z.AreClose(vec.z);
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
    }

    public override string ToString()
    {
        return String.Format("{0},{1},{2}", x, y, z);
    }

    public static Vector3D Parse(string text)
    {
        var terms = text.Trim().Split(',');
        if (terms.Length != 3)
            throw new FormatException();
        return new Vector3D(
            double.Parse(terms[0]),
            double.Parse(terms[1]),
            double.Parse(terms[2]));
    }

    #endregion

    #region Properties

    public string Text
    {
        get { return ToString(); }
    }

    public static Vector3D XAxis
    {
        [DebuggerStepThrough]
        get { return new Vector3D(1f, 0, 0); }
    }

    public static Vector3D YAxis
    {
        [DebuggerStepThrough]
        get { return new Vector3D(0, 1f, 0); }
    }

    public static Vector3D ZAxis
    {
        [DebuggerStepThrough]
        get { return new Vector3D(0, 0, 1f); }
    }

    public double X
    {
        [DebuggerStepThrough]
        get { return x; }
        set { x = value; }
    }

    public double Y
    {
        [DebuggerStepThrough]
        get { return y; }
        set { y = value; }
    }

    public double Z
    {
        [DebuggerStepThrough]
        get { return z; }
        set { z = value; }
    }

    public bool IsZeroVector => x == 0 && y == 0 && z == 0;

    public static Vector3D ZeroVector => new Vector3D();

    public double Length => Math.Sqrt(x * x + y * y + z * z);

    public double SquaredLength => x * x + y * y + z * z;

    #endregion

    #region Conversions

    public static explicit operator Vector3D(double d)
    {
        return new Vector3D(d, d, d);
    }

    public static explicit operator Point3D(Vector3D vector)
    {
        return new Point3D(vector.x, vector.y, vector.z);
    }

    #endregion

    #region Operations

    public void Negate()
    {
        x = -x;
        y = -y;
        z = -z;
    }

    public static Vector3D Interpolate(Vector3D vec1, Vector3D vec2,
                                     double a)
    {
        return new Vector3D(vec1.x + a * (vec2.x - vec1.x),
                          vec1.y + a * (vec2.y - vec1.y),
                          vec1.z + a * (vec2.z - vec1.z));
    }

    public static Vector3D Combine(double factor1, Vector3D pt1,
                                 double factor2, Vector3D pt2)
    {
        return new Vector3D(factor1 * pt1.x + factor2 * pt2.x,
                          factor1 * pt1.y + factor2 * pt2.y,
                          factor1 * pt1.z + factor2 * pt2.z);
    }

    public void Normalize()
    {
        double d = x * x + y * y + z * z;
        if (d > Numbers.Epsilon && d != 1.0)
        {
            d = Math.Sqrt(d);
            x = x / d;
            y = y / d;
            z = z / d;
        }
    }

    public double AngleBetween(Vector3D v1, Vector3D v2)
    {
        return Math.Acos(Dot(v1, v2) / (v1.Length * v2.Length)) * (180 / Math.PI);
    }

    public static Vector3D Projection(Vector3D vector, Vector3D onto)
    {
        return (Dot(vector, onto) / onto.SquaredLength) * onto;
    }

    public static Vector3D Perpendicular(Vector3D vector, Vector3D onto)
    {
        return vector - Projection(vector, onto);
    }

    public static double Area(Vector3D v1, Vector3D v2)
    {
        return (v1 * v2).Length;
    }

    /// <summary>
    /// Provides a determinant for the matrix whose rows (or columns)
    /// are vec1, vec2, vec3
    /// Also known as the scalar triple product (vec1 * vec2) . vec3
    /// </summary>
    /// <returns></returns>
    public static double Determinant(Vector3D vec1, Vector3D vec2, Vector3D vec3)
    {
        return vec1.x * (vec2.y * vec3.z - vec2.z * vec3.y)
               + vec1.y * (vec2.z * vec3.x - vec2.x * vec3.z)
               + vec1.z * (vec2.x * vec3.y - vec2.y * vec3.x);
    }

    public static Matrix3D TensorProduct(Vector3D v1, Vector3D v2)
    {
        return new Matrix3D(v1.x * v2.x, v1.x * v2.y, v1.x * v2.z,
                            v1.y * v2.x, v1.y * v2.y, v1.y * v2.z,
                            v1.z * v2.x, v1.z * v2.y, v1.z * v2.z,
                            0, 0, 0);
    }

    public static Matrix3D SkewSymmetric(Vector3D v)
    {
        return new Matrix3D(0, v.z, -v.y, -v.z, 0, v.x, v.y, -v.x, 0, 0, 0, 0);
    }

    public static double Volume(Vector3D v1, Vector3D v2, Vector3D v3)
    {
        return Math.Abs(Determinant(v1, v2, v3));
    }

    public bool IsPerpendicularTo(Vector3D vector)
    {
        return Numbers.IsZeroed(Dot(vector));
    }

    public bool IsParallelTo(Vector3D vector)
    {
        return Numbers.IsZeroed((this * vector).SquaredLength);
    }

    #endregion

    #region Utility

    public static Vector3D Max(Vector3D vec1, Vector3D vec2)
    {
        return new Vector3D(Math.Max(vec1.x, vec2.x),
                          Math.Max(vec1.y, vec2.y),
                          Math.Max(vec1.z, vec2.z));
    }

    public static Vector3D Min(Vector3D vec1, Vector3D vec2)
    {
        return new Vector3D(Math.Min(vec1.x, vec2.x),
                          Math.Min(vec1.y, vec2.y),
                          Math.Min(vec1.z, vec2.z));
    }

    public static void Swap(ref Vector3D vec1, ref Vector3D vec2)
    {
        var tmp = vec1;
        vec1 = vec2;
        vec2 = tmp;
    }

    #endregion

    #region Math

    public static Point3D operator +(Vector3D vec, Point3D pt)
    {
        return new Point3D(pt.X + vec.X, pt.Y + vec.Y, pt.Z + vec.Z);
    }

    public static Vector3D operator -(Vector3D vec, Vector3D vec2)
    {
        return new Vector3D(vec.x - vec2.x, vec.y - vec2.y, vec.z - vec2.z);
    }

    public static Vector3D operator +(Vector3D vec, Vector3D vec2)
    {
        return new Vector3D(vec.x + vec2.x, vec.x + vec2.y, vec.z + vec2.z);
    }

    public static Vector3D operator -(Vector3D vec)
    {
        return new Vector3D(-vec.x, -vec.y, -vec.z);
    }

    public static Vector3D operator *(double s, Vector3D vec)
    {
        return new Vector3D(vec.x * s, vec.y * s, vec.z * s);
    }

    public static Vector3D operator *(Vector3D vec, double s)
    {
        return new Vector3D(vec.x * s, vec.y * s, vec.z * s);
    }

    public static Vector3D operator /(Vector3D vec, double s)
    {
        s = 1 / s;
        return new Vector3D(vec.x * s, vec.y * s, vec.z * s);
    }

    public static bool operator ==(Vector3D vec1, Vector3D vec2)
    {
        return vec1.Equals(vec2);
    }

    public static bool operator !=(Vector3D vec1, Vector3D vec2)
    {
        return !vec1.Equals(vec2);
    }

    public static Vector3D operator *(Vector3D vec1, Vector3D vec2)
    {
        return new Vector3D(
            vec1.y * vec2.z - vec1.z * vec2.y,
            vec1.z * vec2.x - vec1.x * vec2.z,
            vec1.x * vec2.y - vec1.y * vec2.z);
    }

    public static Vector3D Cross(Vector3D vec1, Vector3D vec2)
    {
        return new Vector3D(
            vec1.y * vec2.z - vec1.z * vec2.y,
            vec1.z * vec2.x - vec1.x * vec2.z,
            vec1.x * vec2.y - vec1.y * vec2.z);
    }

    public double Dot(Vector3D vector)
    {
        return x * vector.x + y * vector.y + z * vector.z;
    }

    public double Dot(Point3D point)
    {
        return x * point.X + y * point.Y + z * point.Z;
    }

    public static double Dot(Vector3D vec1, Vector3D vec2)
    {
        return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z * vec2.z;
    }

    #endregion
}