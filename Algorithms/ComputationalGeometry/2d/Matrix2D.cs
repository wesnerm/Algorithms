#region

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

#pragma warning disable 253

namespace Algorithms.ComputationalGeometry;

[StructLayout(LayoutKind.Sequential)]
[DebuggerStepThrough]
[InlineArray(6)]
public struct Matrix2D
{
    #region Construction

    public Matrix2D(double e11, double e12,
        double e21, double e22,
        double dx, double dy)
    {
        E11 = e11;
        E12 = e12;
        E21 = e21;
        E22 = e22;
        OffsetX = dx;
        OffsetY = dy;
    }

    #endregion

    #region Elements

    private double data;

    public double E11
    {
        get => this[0];
        set => this[0] = value;
    }
    public double E12
    {
        get => this[1];
        set => this[1] = value;
    }

    public double E21
    {
        get => this[2];
        set => this[2] = value;
    }

    public double E22
    {
        get => this[3];
        set => this[3] = value;
    }

    public double OffsetX
    {
        get => this[4];
        set => this[4] = value;
    }

    public double OffsetY
    {
        get => this[5];
        set => this[5] = value;
    }


    #endregion

    #region Object Overloads

    public override bool Equals(object obj) => obj is Matrix2D && Equals((Matrix2D)obj);

    public bool Equals(Matrix2D matrix) =>
        E11 == matrix.E11 && E22 == matrix.E22
                          && E12 == matrix.E12 && E21 == matrix.E21
                          && OffsetX == matrix.OffsetX && OffsetY == matrix.OffsetY;

    public static bool operator ==(Matrix2D matrix1, Matrix2D matrix2) => matrix1.Equals(matrix2);

    public static bool operator !=(Matrix2D matrix1, Matrix2D matrix2) => !matrix1.Equals(matrix2);

    public override int GetHashCode() =>
        E11.GetHashCode() ^ E12.GetHashCode()
                          ^ E21.GetHashCode() ^ E22.GetHashCode()
                          ^ OffsetX.GetHashCode() ^ OffsetY.GetHashCode();

    public override string ToString() => $"{E11},{E12};{E21},{E22};{OffsetX},{OffsetY}";

    #endregion

    #region Properties

    public static readonly Matrix2D Identity = new(1, 0, 0, 1, 0, 0);

    /// <summary>
    ///     Indicates whether volume is conserved
    /// </summary>
    public bool IsOrthogonal => Math.Abs(Determinant).AreClose(1);

    public bool IsInvertible => !Numbers.IsZeroed(Determinant);

    public bool IsIdentity => E11 == 1 && E22 == 1 && E12 == 0 && E21 == 0 && OffsetX == 0 && OffsetY == 0;

    public Point2D Offset => new(OffsetX, OffsetY);

    public double Determinant => E11 * E22 - E12 * E21;

    public Matrix2D Inverse
    {
        [DebuggerStepThrough]
        get
        {
            Matrix2D result = this;
            result.Invert();
            return result;
        }
    }

    #endregion

    #region Operations

    public void Invert()
    {
        // Get Determinant
        double m11 = E22;
        double m21 = -E12;
        double det = E21 * m21 + E11 * m11;
        if (Numbers.IsZeroed(det))
            throw new InvalidOperationException();

        // Get Adjoint
        double f = 1.0 / det;
        double m22 = E11;
        double m12 = -E21;

        // Handle Translation
        double t13 = E11 * OffsetY - OffsetX * E12;
        double t23 = E21 * OffsetY - OffsetX * E22;

        // Transpose
        E11 = m11 * f;
        E12 = m21 * f;
        E21 = m12 * f;
        E22 = m22 * f;

        // REVIEW: are Dx and Dy adequately represented
        OffsetX = t13;
        OffsetY = t23;
    }

    public void Multiply(Matrix2D matrix, ApplyOrder order = ApplyOrder.Append)
    {
        if (order == ApplyOrder.Prepend)
            AssignProduct(matrix, this);
        else
            AssignProduct(this, matrix);
    }

    public void Assign(ref Matrix2D matrix)
    {
        E11 = matrix.E11;
        E12 = matrix.E12;
        E21 = matrix.E21;
        E22 = matrix.E22;
        OffsetX = matrix.OffsetX;
        OffsetY = matrix.OffsetY;
    }

    void AssignProduct(Matrix2D m1, Matrix2D m2)
    {
        double t11 = m1.E11 * m2.E11 + m1.E12 * m2.E21;
        double t12 = m1.E11 * m2.E12 + m1.E12 * m2.E22;
        double t21 = m1.E21 * m2.E11 + m1.E22 * m2.E21;
        double t22 = m1.E21 * m2.E12 + m1.E22 * m2.E22;
        double tx = m1.OffsetX * m2.E11 + m1.OffsetY * m2.E21 + m2.OffsetX;
        double ty = m1.OffsetX * m2.E12 + m1.OffsetY * m2.E22 + m2.OffsetY;

        E11 = t11;
        E12 = t12;
        E21 = t21;
        E22 = t22;
        OffsetX = tx;
        OffsetY = ty;
    }

    [DebuggerStepThrough]
    public void Reset()
    {
        OffsetY = OffsetX = 0.0;
        E12 = E21 = 0.0;
        E11 = E22 = 1.0;
    }

    public void Rotate(double angle, Point2D pt, ApplyOrder order = ApplyOrder.Append)
    {
        Multiply(RotationMatrix(angle).CenterAt(pt), order);
    }

    public void Scale(double sx, double sy, Point2D point = default)
    {
        Scale(sx, sy, point, ApplyOrder.Append);
    }

    public void Scale(double sx, double sy, Point2D point, ApplyOrder order)
    {
        Multiply(ScaleMatrix(sx, sy).CenterAt(point), order);
    }

    public Point2D TransformPoint(Point2D point) =>
        new(point.X * E11 + point.Y * E21 + OffsetX,
            point.X * E12 + point.Y * E22 + OffsetY);

    public Point2D TransformVector(Point2D vector) =>
        new(vector.X * E11 + vector.Y * E21,
            vector.X * E12 + vector.Y * E22);

    public void TransformPoints(Point2D[] points)
    {
        for (int i = 0; i < points.Length; i++)
            points[i] = TransformPoint(points[i]);
    }

    public void TransformVectors(Point2D[] vectors)
    {
        for (int i = 0; i < vectors.Length; i++)
            vectors[i] = TransformVector(vectors[i]);
    }

    public void Translate(double dx, double dy, ApplyOrder order = ApplyOrder.Append)
    {
        Multiply(TranslationMatrix(dx, dy), order);
    }

    public void Transpose2x2()
    {
        double tmp = E12;
        E12 = E21;
        E21 = tmp;
    }

    #endregion

    #region Operation

    public Matrix2D CenterAt(double x, double y)
    {
        if (x != 0 || y != 0)
        {
            OffsetX += x - x * E11 - y * E21;
            OffsetY += y - x * E12 - y * E22;
        }

        return this;
    }

    public Matrix2D CenterAt(Point2D point) => CenterAt(point.X, point.Y);

    public static Matrix2D RotationMatrix(double angle)
    {
        double sin = Math.Sin(angle);
        double cos = Math.Cos(angle);
        var matrix = new Matrix2D
        {
            E11 = cos,
            E12 = sin,
            E21 = -sin,
            E22 = cos,
        };
        return matrix;
    }

    public static Matrix2D ScaleMatrix(double sx, double sy) =>
        new()
        {
            E11 = sx,
            E22 = sy,
        };

    public static Matrix2D TranslationMatrix(double dx, double dy) =>
        new()
        {
            E11 = 1,
            E22 = 1,
            OffsetX = dx,
            OffsetY = dy,
        };

    public static Matrix2D ShearMatrix(double shearX, double shearY) =>
        new()
        {
            E11 = 1,
            E22 = 1,
            E12 = shearX,
            E21 = shearY,
        };

    public static Matrix2D SkewMatrix(double skewX, double skewY) => ShearMatrix(Math.Tan(skewY), Math.Tan(skewX));

    #endregion

    #region Operators

    public static Point2D operator *(Point2D point, Matrix2D matrix) =>
        new(
            point.X * matrix.E11 + point.Y * matrix.E21 + matrix.OffsetX,
            point.X * matrix.E12 + point.Y * matrix.E22 + matrix.OffsetY
        );

    public static Matrix2D operator *(Matrix2D m1, Matrix2D m2)
    {
        var m = new Matrix2D();
        m.AssignProduct(m1, m2);
        return m;
    }

    #endregion

    #region Apply To Functions

    public Box2D ApplyTo(Box2D rectangle)
    {
        double x = rectangle.Left * E11 + rectangle.Top * E12 + OffsetX;
        double y = rectangle.Left * E21 + rectangle.Top * E22 + OffsetY;
        double width = rectangle.Width * E11 + rectangle.Height * E12;
        double height = rectangle.Width * E21 + rectangle.Height * E22;

        if (width < 0)
        {
            x += width;
            width = -width;
        }

        if (height < 0)
        {
            y += height;
            height = -height;
        }

        return Box2D.FromWidthHeight(x, y, width, height);
    }

    Box2D RotateRectangle(Box2D rect)
    {
        Point2D[] point2Ds = rect.GetPoints();
        TransformPoints(point2Ds);
        return Box2D.From(point2Ds);
    }

    [DebuggerStepThrough]
    public Point2D ApplyToPoint(Point2D point2D) =>
        new(point2D.X * E11 + point2D.Y * E12 + OffsetX,
            point2D.X * E21 + point2D.Y * E22 + OffsetY);

    [DebuggerStepThrough]
    public Point2D ApplyToVector(Point2D size) =>
        new(size.X * E11 + size.Y * E12,
            size.X * E21 + size.Y * E22);

    #endregion
}