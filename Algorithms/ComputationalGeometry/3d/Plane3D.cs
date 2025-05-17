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
/// A plane is expressed by N * x = d.
/// </summary>
public struct Plane3D : IEquatable<Plane3D>
{
    #region Plane

    Vector3D normal;
    double d;

    #endregion

    #region Construction

    public Plane3D(Vector3D normal, double constant)
    {
        this.normal = normal;
        if (this.normal.IsZeroVector)
        {
            d = 0;
            return;
        }
        d = constant;
        this.normal.Normalize();
    }

    public Plane3D(double a, double b, double c, double constant)
        : this(new Vector3D(a, b, c), constant)
    {
    }

    public Plane3D(Ray3D ray) : this(ray.Vector, ray.Vector.Dot(ray.Point))
    {
    }

    public Plane3D(Vector3D normal, Point3D point) : this(normal, normal.Dot(point))
    {
    }

    public Plane3D(Point3D pt1, Point3D pt2, Point3D pt3)
    {
        normal = (pt2 - pt1)*(pt3 - pt2);
        if (normal.IsZeroVector)
        {
            d = 0;
            return;
        }
        d = normal.Dot(pt1);
    }

    #endregion

    #region Object Overrides

    public override bool Equals(object obj)
    {
        if (!(obj is Plane3D))
            return false;
        return Equals((Plane3D) obj);
    }

    public bool Equals(Plane3D plane)
    {
        return normal.Equals(plane.normal) && d == plane.d;
    }

    public bool IsCloseTo(Plane3D plane)
    {
        return normal.IsCloseTo(plane.normal)
               && Numbers.IsZeroed(plane.d - d);
    }

    public bool IsParallelTo(Plane3D plane)
    {
        return normal.IsParallelTo(plane.normal);
    }

    public bool IsPerpendicularTo(Plane3D plane)
    {
        return normal.IsPerpendicularTo(plane.normal);
    }

    public override int GetHashCode()
    {
        return d.GetHashCode() ^ normal.GetHashCode();
    }

    public override string ToString() => $"{normal};{d}";

    public static Plane3D Parse(string text)
    {
        var terms = text.Trim().Split(';');
        if (terms.Length != 2)
            throw new FormatException();
        return new Plane3D(
            Vector3D.Parse(terms[0]),
            double.Parse(terms[1]));
    }

    #endregion

    #region Properties

    public string Text => ToString();

    public static Plane3D YZPlane => new Plane3D(Vector3D.XAxis, 0);

    public static Plane3D XZPlane => new Plane3D(Vector3D.YAxis, 0);

    public static Plane3D XYPlane => new Plane3D(Vector3D.ZAxis, 0);

    public double Constant
    {
        [DebuggerStepThrough]
        get { return d; }
        set { d = value; }
    }

    public Vector3D Normal
    {
        [DebuggerStepThrough]
        get { return normal; }
        set { normal = value; }
    }

    /// <summary>
    /// Returns a point from the plane where x is guaranteed to be non-zero
    /// except in the YZ plane, where both y and z are equal to 1
    /// </summary>
    public Point3D Point
    {
        get
        {
            if (Numbers.IsZeroed(normal.X))
            {
                if (!Numbers.IsZeroed(normal.Y))
                    return new Point3D(1, d/normal.Y, 0);
                if (!Numbers.IsZeroed(normal.Z))
                    return new Point3D(1, 0, d/normal.Z);
                return new Point3D(1, 0, 0);
            }
            if (Numbers.IsZeroed(d))
            {
                if (!Numbers.IsZeroed(normal.Y))
                    return new Point3D(1, -normal.X/normal.Y, 0);
                if (!Numbers.IsZeroed(normal.Z))
                    return new Point3D(1, 0, -normal.X/normal.Z);
                return new Point3D(0, 1, 1);
            }
            return new Point3D(d/normal.X, 0, 0);
        }
    }

    public bool IsEmpty => normal.IsZeroVector;

    public static Plane3D Empty => new Plane3D();

    #endregion

    #region Operations

    public void Negate()
    {
        normal.Negate();
        d = -d;
    }

    #endregion

    #region Utility

    public static void Swap(ref Plane3D plane1, ref Plane3D plane2)
    {
        var tmp = plane1;
        plane1 = plane2;
        plane2 = tmp;
    }

    #endregion

    #region Math

    public static bool operator ==(Plane3D plane1, Plane3D plane2)
    {
        return plane1.Equals(plane2);
    }

    public static bool operator !=(Plane3D plane1, Plane3D plane2)
    {
        return !plane1.Equals(plane2);
    }

    #endregion

    #region Plane Geometry

    public bool Contains(Point3D pt)
    {
        return d.AreClose(normal.Dot(pt));
    }

    public bool Contains(Ray3D ray)
    {
        return Numbers.IsZeroed(normal.Dot(ray.Vector))
               && Contains(ray.Point);
    }

    /// <summary>
    /// Produces the signed distance from the plane to the point
    /// </summary>
    /// <param name="pt"></param>
    /// <returns></returns>
    public double DistanceTo(Point3D pt)
    {
        return normal.Dot(pt) - d;
    }

    public Point3D ClosestPoint(Point3D pt)
    {
        return pt - normal*DistanceTo(pt);
    }

    public Point3D Projection(Point3D pt)
    {
        return pt - normal*DistanceTo(pt);
    }

    public Vector3D Projection(Vector3D vector)
    {
        return vector - normal.Dot(vector)*normal;
    }

    public Ray3D Projection(Ray3D ray)
    {
        return new Ray3D(Projection(ray.Point), Projection(ray.Vector));
    }

    public static bool Intersection(Plane3D plane1, Plane3D plane2,
                                    Plane3D plane3, out Point3D point)
    {
        double det = Vector3D.Determinant(plane1.normal, plane2.normal,
                                        plane3.normal);
        if (Numbers.IsZeroed(det))
        {
            point = Point3D.Origin;
            return false;
        }

        point = (Point3D)
                (plane1.d*(plane2.normal*plane3.normal)
                 + plane2.d*(plane3.normal*plane1.normal)
                 + plane3.d*(plane1.normal*plane2.normal));

        point /= det;
        return true;
    }

    public static bool Intersection(Plane3D plane1, Plane3D plane2, out Ray3D line)
    {
        line = new Ray3D {Vector = plane1.normal * plane2.normal};
        if (line.Vector.IsZeroVector)
            return false;

        double n1_n2 = plane1.normal.Dot(plane2.Normal);
        double n1_nsq = plane1.normal.SquaredLength;
        double n2_nsq = plane2.normal.SquaredLength;
        double denom = n1_n2*n1_n2 - n1_nsq*n2_nsq;
        double a = (plane2.d*n1_n2 + plane1.d*n2_nsq)/denom;
        double b = (plane1.d*n1_n2 + plane2.d*n1_nsq)/denom;
        line.Point = (Point3D) Vector3D.Combine(a, plane1.normal,
                                              b, plane2.normal);
        return true;
    }

    public static bool Intersection(Plane3D plane, Ray3D line, out Point3D pt)
    {
        if (!line.Intersect(plane, out double t))
        {
            pt = new Point2D();
            return false;
        }

        pt = line[t];
        return true;
    }

    #endregion
}