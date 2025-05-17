#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Text;
using System.Windows;
using Algorithms.Mathematics;

#endregion

namespace Algorithms.ComputationalGeometry;

/// <summary>
/// Summary description for Ray.
/// </summary>
public struct Ray3D : ICloneable
{
    #region Ray

    private Point3D point;
    private Vector3D vector;

    #endregion

    #region Construction

    public Ray3D(Point3D point, Vector3D vector)
    {
        this.point = point;
        this.vector = vector;
        this.vector.Normalize();
    }

    #endregion

    #region Object Overrides

    public override bool Equals(object obj)
    {
        if (!(obj is Ray3D))
            return false;
        return Equals((Ray3D) obj);
    }

    public bool Equals(Ray3D ray)
    {
        return point.Equals(ray.point) && vector.Equals(ray.vector);
    }

    public override int GetHashCode()
    {
        return point.GetHashCode() ^ vector.GetHashCode();
    }

    public override string ToString()
    {
        return $"{point};{vector}";
    }

    public static Ray3D Parse(string text)
    {
        var terms = text.Trim().Split(';');
        if (terms.Length != 2)
            throw new FormatException();
        return new Ray3D(
            Point3D.Parse(terms[0]),
            Vector3D.Parse(terms[1]));
    }

    #endregion

    #region Properties

    public string Text
    {
        get { return ToString(); }
    }

    public static Ray3D XAxis
    {
        get { return new Ray3D(new Point2D(), Vector3D.XAxis); }
    }

    public static Ray3D YAxis
    {
        get { return new Ray3D(new Point2D(), Vector3D.YAxis); }
    }

    public static Ray3D ZAxis
    {
        get { return new Ray3D(new Point2D(), Vector3D.ZAxis); }
    }

    public Point3D Point
    {
        [DebuggerStepThrough]
        get { return point; }
        set { point = value; }
    }

    public Vector3D Vector
    {
        [DebuggerStepThrough]
        get { return vector; }
        set { vector = value; }
    }

    public bool IsZero
    {
        get { return vector.IsZeroVector && point.IsOrigin; }
    }

    public static Ray3D Zero
    {
        get { return new Ray3D(); }
    }

    #endregion

    #region Operations

    public void Negate()
    {
        vector.Negate();
    }

    public static Ray3D Interpolate(Ray3D ray1, Ray3D ray2, double a)
    {
        return new Ray3D(Point3D.Interpolate(ray1.point, ray2.point, a),
                       Vector3D.Interpolate(ray1.vector, ray2.vector, a));
    }

    public double ClosestPoint(Point3D point)
    {
        return vector.Dot(point - this.point);
    }

    public double DistanceFromPoint(Point3D point)
    {
        return (this[ClosestPoint(point)] - this.point).Length;
    }

    #endregion

    #region Utility

    public static void Swap(ref Ray3D ray1, ref Ray3D ray2)
    {
        var tmp = ray1;
        ray1 = ray2;
        ray2 = tmp;
    }

    #endregion

    #region Math

    public static bool operator ==(Ray3D ray1, Ray3D ray2)
    {
        return ray1.Equals(ray2);
    }

    public static bool operator !=(Ray3D ray1, Ray3D ray2)
    {
        return !ray1.Equals(ray2);
    }

    #endregion

    #region Parametric Operations

    /// <summary>
    /// Returns the paramaters for points of closest approach
    /// if line are skew, then s and t represent the parameters of the points of closest 
    /// </summary>
    /// <param name="ray1">first line</param>
    /// <param name="ray2">second line</param>
    /// <param name="t1">represents param of closest intersection for ray1</param>
    /// <param name="t2">represents param of closest intersection for ray2</param>
    /// <returns>true if lines are not parallel</returns>
    public static bool Intersect(Ray3D ray1, Ray3D ray2, out double t1, out double t2)
    {
        var cross = ray1.vector*ray2.vector;
        double denom = cross.SquaredLength;
        if (Numbers.IsZeroed(denom))
        {
            t1 = 0;
            t2 = 0;
            return false;
        }

        var v = ray2.point - ray1.point;
        // Note: This could be simplified further
        t1 = Vector3D.Determinant(v, ray1.vector, cross)/denom;
        t2 = Vector3D.Determinant(v, ray2.vector, cross)/denom;
        return true;
    }

    public bool Intersect(Plane3D plane, out double t)
    {
        double dot = vector.Dot(plane.Normal);
        if (Numbers.IsZeroed(dot))
        {
            t = 0;
            return false;
        }

        t = -plane.DistanceTo(point)/dot;
        return true;
    }

    public Point3D this[double t]
    {
        get
        {
            return new Point3D(point.X + vector.X*t,
                               point.Y + vector.Y*t, point.Z + vector.Z*t);
        }
    }

    #endregion

    #region ICloneable Members

    object ICloneable.Clone()
    {
        return MemberwiseClone();
    }

    #endregion
}