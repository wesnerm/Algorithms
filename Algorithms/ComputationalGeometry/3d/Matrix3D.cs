#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Algorithms.Mathematics;

#endregion

namespace Algorithms.ComputationalGeometry;

[StructLayout(LayoutKind.Sequential)]
public class Matrix3D
{
    #region Variables

    public double E11, E12, E13;
    public double E21, E22, E23;
    public double E31, E32, E33;
    public double Dx, Dy, Dz;

    #endregion

    #region Construction

    public Matrix3D() => E11 = E22 = E33 = 1f;

    public Matrix3D(double e11, double e12, double e13,
        double e21, double e22, double e23,
        double e31, double e32, double e33,
        double dx, double dy, double dz)
    {
        E11 = e11;
        E12 = e12;
        E13 = e13;
        E21 = e21;
        E22 = e22;
        E23 = e23;
        E31 = e31;
        E32 = e32;
        E33 = e33;
        Dx = dx;
        Dy = dy;
        Dz = dz;
    }

    Matrix3D(Matrix3D matrix1, Matrix3D matrix2)
    {
        AssignProduct(matrix1, matrix2);
    }

    Matrix3D(bool flag) { }

    Matrix3D(double diagonal) => E11 = E22 = E33 = diagonal;

    public Matrix3D Clone() => (Matrix3D)MemberwiseClone();

    public static Matrix3D GetDiagonal(double d) => new(d);

    #endregion

    #region Object Overloads

    public override bool Equals(object obj)
    {
        if (obj == this)
            return true;

        var matrix = obj as Matrix3D;
        if (obj == null)
            return false;

        return Equals(matrix);
    }

    public bool Equals(Matrix3D matrix)
    {
        if (this == (object)matrix) return true;
        if ((object)matrix == null) return false;

        return E11 == matrix.E11 && E22 == matrix.E22 && E33 == matrix.E33
               && E12 == matrix.E12 && E21 == matrix.E21
               && E13 == matrix.E13 && E31 == matrix.E31
               && E23 == matrix.E23 && E32 == matrix.E32
               && Dx == matrix.Dx && Dy == matrix.Dy && Dz == matrix.Dz;
    }

    public static bool operator ==(Matrix3D matrix1, Matrix3D matrix2)
    {
        if ((object)matrix1 == null || (object)matrix2 == null)
            return matrix1 == (object)matrix2;
        return matrix1.Equals(matrix2);
    }

    public static bool operator !=(Matrix3D matrix1, Matrix3D matrix2)
    {
        if ((object)matrix1 == null || (object)matrix2 == null)
            return matrix1 != (object)matrix2;
        return !matrix1.Equals(matrix2);
    }

    public override int GetHashCode() =>
        E11.GetHashCode() ^ E12.GetHashCode() ^ E13.GetHashCode()
        ^ E21.GetHashCode() ^ E22.GetHashCode() ^ E23.GetHashCode()
        ^ E31.GetHashCode() ^ E32.GetHashCode() ^ E33.GetHashCode()
        ^ Dx.GetHashCode() ^ Dy.GetHashCode() ^ Dz.GetHashCode();

    public override string ToString() =>
        string.Format("{0},{1},{2};{3},{4},{5};{6},{7},{8};{9},{10},{11}",
            E11, E12, E13, E21, E22, E23,
            E31, E32, E33, Dx, Dy, Dz);

    #endregion

    #region Properties

    public string Text => ToString();

    public static Matrix3D Identity => new();

    /// <summary>
    ///     Indicates that orientation is conserved
    ///     Otherwise, matrix is either degenerate (det==0) or mirroring (det lt 0)
    /// </summary>
    public bool IsProper => Determinant > 0;

    /// <summary>
    ///     Indicates a matrix with a determinant of zero
    /// </summary>
    public bool IsDegenerate => Numbers.IsZeroed(Determinant);

    /// <summary>
    ///     Indicates whether volume is conserved
    /// </summary>
    public bool IsOrthogonal => Math.Abs(Determinant).AreClose(1);

    public bool IsInvertible => Numbers.IsZeroed(Determinant);

    public bool IsIdentity =>
        E11 == 1 && E22 == 1 && E33 == 1
        && E12 == 0 && E21 == 0 && E13 == 0
        && E31 == 0 && E23 == 0 && E32 == 0
        && Dx == 0 && Dy == 0 && Dz == 0;

    public Point3D Offset => new(Dx, Dy, Dz);

    public double Determinant =>
        E11 * (E22 * E33 - E23 * E32)
        + E12 * (E23 * E31 - E21 * E33)
        + E13 * (E21 * E32 - E22 * E31);

    public Matrix3D Inverse {
        get
        {
            Matrix3D result = Clone();
            result.Invert();
            return result;
        }
    }

    #endregion

    #region Operations

    public void Invert()
    {
        // Get Determinant
        double m11 = E22 * E33 - E32 * E23;
        double m21 = E32 * E13 - E12 * E33;
        double m31 = E12 * E23 - E22 * E13;
        double det = E31 * m31 + E21 * m21 + E11 * m11;
        if (Numbers.IsZeroed(det))
            throw new InvalidOperationException();

        // Get Adjoint
        double f = 1.0 / det;
        double m33 = E11 * E22 - E21 * E12;
        double m23 = E31 * E12 - E11 * E32;
        double m13 = E21 * E32 - E31 * E22;
        double m32 = E21 * E13 - E11 * E23;
        double m22 = E11 * E33 - E31 * E13;
        double m12 = E31 * E23 - E21 * E33;

        // Handle Translation
        double t14 = E11 * Dy - Dx * E12;
        double t24 = E21 * Dy - Dx * E22;
        double t34 = E31 * Dy - Dx * E32;
        Dz = (E23 * t14 - Dz * m33 - E13 * t24) * f;
        Dy = (E13 * t34 - E33 * t14 - Dz * m23) * f;
        Dx = (E33 * t24 - Dz * m13 - E23 * t34) * f;

        // Transpose
        E11 = m11 * f;
        E12 = m21 * f;
        E13 = m31 * f;
        E21 = m12 * f;
        E22 = m22 * f;
        E23 = m32 * f;
        E31 = m13 * f;
        E32 = m23 * f;
        E33 = m33 * f;
    }

    public void Multiply(Matrix3D matrix)
    {
        Multiply(matrix, ApplyOrder.Append);
    }

    public void Multiply(Matrix3D matrix, ApplyOrder order)
    {
        if (matrix.IsIdentity)
            return;

        if (order == ApplyOrder.Prepend)
            AssignProduct(matrix, this);
        else
            AssignProduct(this, matrix);
    }

    public void Assign(Matrix3D matrix)
    {
        E11 = matrix.E11;
        E12 = matrix.E12;
        E13 = matrix.E13;
        E21 = matrix.E21;
        E22 = matrix.E22;
        E23 = matrix.E23;
        E31 = matrix.E31;
        E32 = matrix.E32;
        E33 = matrix.E33;
        Dx = matrix.Dx;
        Dy = matrix.Dy;
        Dz = matrix.Dz;
    }

    void AssignProduct(Matrix3D m1, Matrix3D m2)
    {
        if (m2.IsIdentity) {
            Assign(m1);
            return;
        }

        if (m1.IsIdentity) {
            Assign(m2);
            return;
        }

        double t11 = m1.E11 * m2.E11 + m1.E12 * m2.E21 + m1.E13 * m2.E31;
        double t12 = m1.E11 * m2.E12 + m1.E12 * m2.E22 + m1.E13 * m2.E32;
        double t13 = m1.E11 * m2.E13 + m1.E12 * m2.E23 + m1.E13 * m2.E33;
        double t21 = m1.E21 * m2.E11 + m1.E22 * m2.E21 + m1.E23 * m2.E31;
        double t22 = m1.E21 * m2.E12 + m1.E22 * m2.E22 + m1.E23 * m2.E32;
        double t23 = m1.E21 * m2.E13 + m1.E22 * m2.E23 + m1.E23 * m2.E33;
        double t31 = m1.E31 * m2.E11 + m1.E32 * m2.E21 + m1.E33 * m2.E31;
        double t32 = m1.E31 * m2.E12 + m1.E32 * m2.E22 + m1.E33 * m2.E32;
        double t33 = m1.E31 * m2.E13 + m1.E32 * m2.E23 + m1.E33 * m2.E33;
        double t41 = m1.Dx * m2.E11 + m1.Dy * m2.E21 + m1.Dz * m2.E31 + m2.Dx;
        double t42 = m1.Dx * m2.E12 + m1.Dy * m2.E22 + m1.Dz * m2.E32 + m2.Dy;
        double t43 = m1.Dx * m2.E13 + m1.Dy * m2.E23 + m1.Dz * m2.E33 + m2.Dz;

        E11 = t11;
        E12 = t12;
        E13 = t13;
        E21 = t21;
        E22 = t22;
        E23 = t23;
        E31 = t31;
        E32 = t32;
        E33 = t33;
        Dx = t41;
        Dy = t42;
        Dz = t43;
    }

    public void Reset()
    {
        E12 = E13 = E21 = E23 = 0f;
        E31 = E32 = E31 = Dy = Dz = 0f;
        E11 = E22 = E33 = 1f;
    }

    public void Rotate(Vector3D axis, double angle)
    {
        Rotate(new Quaternion(axis, angle));
    }

    public void Rotate(Vector3D axis, double angle, Point3D pt)
    {
        Rotate(new Quaternion(axis, angle), pt);
    }

    public void Rotate(Vector3D axis, double angle, Point3D pt, ApplyOrder order)
    {
        Rotate(new Quaternion(axis, angle), pt, order);
    }

    public void Rotate(Quaternion quat)
    {
        Rotate(quat, Point3D.Origin);
    }

    public void Rotate(Quaternion quat, Point3D pt)
    {
        Rotate(quat, pt, ApplyOrder.Append);
    }

    public void Rotate(Quaternion quat, Point3D pt, ApplyOrder order)
    {
        Multiply(RotationMatrix(quat, pt), order);
    }

    public void Scale(Vector3D vector)
    {
        Scale(vector, Point3D.Origin);
    }

    public void Scale(Vector3D vector, Point3D point)
    {
        Scale(vector, point, ApplyOrder.Append);
    }

    public void Scale(Vector3D scale, Point3D point, ApplyOrder order)
    {
        Multiply(ScaleMatrix(scale, point), order);
    }

    public void Transform(Point3D[] points)
    {
        for (int i = 0; i < points.Length; i++)
            points[i] *= this;
    }

    public void Transform(Vector3D[] vectors)
    {
        for (int i = 0; i < vectors.Length; i++)
            vectors[i] *= this;
    }

    public void Translate(Vector3D vector)
    {
        Translate(vector, ApplyOrder.Append);
    }

    public void Translate(Vector3D vector, ApplyOrder order)
    {
        Multiply(TranslationMatrix(vector), order);
    }

    public void Transpose3X3()
    {
        double tmp;
        tmp = E12;
        E12 = E21;
        E21 = tmp;
        tmp = E13;
        E13 = E31;
        E31 = tmp;
        tmp = E23;
        E23 = E32;
        E32 = tmp;
    }

    public static Matrix3D RotationMatrix(Vector3D axis, double angle,
        Point3D center) =>
        RotationMatrix(new Quaternion(axis, angle), center);

    public static Matrix3D RotationMatrix(Quaternion quat, Point3D center)
    {
        double x = quat.X + quat.X;
        double y = quat.Y + quat.Y;
        double z = quat.Z + quat.Z;
        double xx = quat.X * x;
        double xy = quat.X * y;
        double xz = quat.X * z;
        double yy = quat.Y * y;
        double yz = quat.Y * z;
        double zz = quat.Z * z;
        double wx = quat.W * x;
        double wy = quat.W * y;
        double wz = quat.W * z;

        // T = Diagonal(cos t) + (1-cos t)TensorProduct + sin(0)SkewSymetric

        var matrix = new Matrix3D
        {
            E11 = 1 - (yy + zz),
            E12 = xy - wz,
            E13 = xz + wy,
            E21 = xy + wz,
            E22 = 1 - (xx + zz),
            E23 = yz - wx,
            E31 = xz - wy,
            E32 = yz + wx,
            E33 = 1 - (xx + yy),
        };
        if (!center.IsOrigin) {
            Point3D pt = center * matrix;
            matrix.Dx = center.X - pt.X;
            matrix.Dy = center.Y - pt.Y;
            matrix.Dz = center.Z - pt.Z;
        }

        return matrix;
    }

    public static Matrix3D ScaleMatrix(Vector3D scale, Point3D center)
    {
        var matrix = new Matrix3D
        {
            E11 = scale.X,
            E22 = scale.Y,
            E33 = scale.Z,
            Dx = center.X - scale.X * center.X,
            Dy = center.Y - scale.Y * center.Y,
            Dz = center.Z - scale.Z * center.Z,
        };
        return matrix;
    }

    public static Matrix3D TranslationMatrix(Vector3D offset)
    {
        var matrix = new Matrix3D
        {
            Dx = offset.X,
            Dy = offset.Y,
            Dz = offset.Z,
        };
        return matrix;
    }

    #endregion

    #region Conversions

    public static explicit operator Matrix2D(Matrix3D matrix) =>
        new(
            matrix.E11,
            matrix.E12,
            matrix.E21,
            matrix.E22,
            matrix.Dx,
            matrix.Dy);

    #endregion

    #region Operators

    public static Ray3D operator *(Ray3D ray, Matrix3D matrix) => new(ray.Point * matrix, ray.Vector * matrix);

    public static Vector3D operator *(Vector3D vector, Matrix3D matrix) =>
        new(
            vector.X * matrix.E11 + vector.Y * matrix.E21 + vector.Z * matrix.E31,
            vector.X * matrix.E12 + vector.Y * matrix.E22 + vector.Z * matrix.E32,
            vector.X * matrix.E13 + vector.Y * matrix.E23 + vector.Z * matrix.E33
        );

    public static Point3D operator *(Point3D point, Matrix3D matrix) =>
        new(
            point.X * matrix.E11 + point.Y * matrix.E21 + point.Z * matrix.E31 +
            matrix.Dx,
            point.X * matrix.E12 + point.Y * matrix.E22 + point.Z * matrix.E32 +
            matrix.Dy,
            point.X * matrix.E13 + point.Y * matrix.E23 + point.Z * matrix.E33 +
            matrix.Dz
        );

    public static Matrix3D operator *(Matrix3D m1, Matrix3D m2)
    {
        if (m1.IsIdentity)
            return m2.Clone();

        if (m2.IsIdentity)
            return m1.Clone();

        var m = new Matrix3D();
        m.AssignProduct(m1, m2);
        return m;
    }

    public static Matrix3D operator /(Matrix3D m1, Matrix3D m2)
    {
        if (m2.IsIdentity)
            return m1;

        Matrix3D result = m2.Inverse;
        result.Multiply(m1, ApplyOrder.Prepend);
        return result;
    }

    #endregion
}