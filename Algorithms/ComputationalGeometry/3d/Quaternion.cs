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
///     Summary description for Quaternion.
/// </summary>
public struct Quaternion : ICloneable
{
    #region Constant

    const double Degrees = Numbers.Degrees;

    #endregion

    #region Construction

    public Quaternion(double x, double y, double z, double w)
    {
        X = (float)x;
        Y = (float)y;
        Z = (float)z;
        W = (float)w;
    }

    public Quaternion(Vector3D axis, double angleDegrees)
    {
        double angle = angleDegrees * Degrees;
        double length = axis.Length;
        if (length == 0) throw new DivideByZeroException();
        double factor = Math.Sin(angle * .5) / length;
        X = (float)(axis.X * factor);
        Y = (float)(axis.Y * factor);
        Z = (float)(axis.Z * factor);
        W = (float)Math.Cos(angle * .5);
    }

    #endregion

    #region Object Overrides

    public override bool Equals(object obj)
    {
        if (!(obj is Quaternion))
            return false;
        return Equals((Quaternion)obj);
    }

    public bool Equals(Quaternion q) => X == q.X && Y == q.Y && Z == q.Z && W == q.W;

    public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();

    public override string ToString() => $"{Axis},{Angle}";

    public static Quaternion Parse(string text)
    {
        string[] terms = text.Trim().Split(',');
        if (terms.Length != 3)
            throw new FormatException();

        int index = text.LastIndexOf(',');
        if (index == -1)
            throw new InvalidCastException();

        string angleText = text.Substring(index + 1);
        text = text.Substring(index);

        return new Quaternion(Vector3D.Parse(text),
            float.Parse(angleText));
    }

    #endregion

    #region Properties

    public string Text => ToString();

    public float X { [DebuggerStepThrough] get; set; }

    public float Y { [DebuggerStepThrough] get; set; }

    public float Z { [DebuggerStepThrough] get; set; }

    public float W { [DebuggerStepThrough] get; set; }

    public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

    public bool IsNormalized => Magnitude == 1.0;

    public Quaternion Conjugate => new(-X, -Y, -Z, W);

    public Quaternion Inverse {
        get
        {
            double norm = Magnitude;
            if (norm == 0)
                throw new DivideByZeroException();
            norm = 1 / norm;
            return new Quaternion(-X * norm, -Y * norm, -Z * norm, W * norm);
        }
    }

    public Vector3D Axis {
        get
        {
            if (!IsNormalized) throw new InvalidOperationException();
            return new Vector3D(X, Y, Z) / Magnitude;
        }
    }

    public float Angle {
        get
        {
            if (!IsNormalized) throw new InvalidOperationException();
            return (float)(Math.Acos(W) * 2.0 * Degrees);
        }
    }

    public Quaternion AxisAngle {
        get
        {
            double tmpW = Math.Acos(W);
            double scale = 1.0 / Math.Sin(tmpW);
            return new Quaternion(
                X * scale,
                Y * scale,
                Z * scale,
                tmpW * (360.0 / Math.PI));
        }
    }

    #endregion

    #region Operations

    public static Quaternion Slerp(Quaternion q1, Quaternion q2, double t)
    {
        const double delta = 1e-6;

        // Calculate cos of omega.
        double cosOmega = q1.X * q2.X + q1.Y * q2.Y + q1.Z * q2.Z + q1.W * q2.W;

        // Adjust signs if necessary.
        if (cosOmega < 0.0) {
            cosOmega = -cosOmega;
            q2.X = -q2.X;
            q2.Y = -q2.Y;
            q2.Z = -q2.Z;
            q2.W = -q2.W;
        }

        // Calculate scaling coefficients.
        double scale0, scale1;
        if (1.0 - cosOmega > delta) {
            double omega;
            if (cosOmega > 1.0)
                omega = 0;
            else if (cosOmega < -1.0)
                omega = Math.PI;
            else
                omega = Math.Acos(cosOmega);
            double sinOmega = Math.Sin(omega);
            scale0 = Math.Sin((1.0 - t) * omega) / sinOmega;
            scale1 = Math.Sin(t * omega) / sinOmega;
        } else {
            scale0 = 1.0 - t;
            scale1 = t;
        }

        return new Quaternion(
            scale0 * q1.X + scale1 * q2.X,
            scale0 * q1.Y + scale1 * q2.Y,
            scale0 * q1.Z + scale1 * q2.Z,
            scale0 * q1.W + scale1 * q2.W);
    }

    public void Normalize()
    {
        double d = Magnitude;
        if (d == 0)
            return;
        d = 1 / d;
        X = (float)(X * d);
        Y = (float)(Y * d);
        Z = (float)(Z * d);
        W = (float)(W * d);
    }

    #endregion

    #region Utility

    public static void Swap(ref Quaternion quat1, ref Quaternion quat2)
    {
        Quaternion tmp = quat1;
        quat1 = quat2;
        quat2 = tmp;
    }

    #endregion

    #region Math

    public static Quaternion operator -(Quaternion quat, Quaternion quat2) =>
        new(quat.X - quat2.X, quat.Y - quat2.Y,
            quat.Z - quat2.Z, quat.W - quat2.W);

    public static Quaternion operator +(Quaternion quat, Quaternion quat2) =>
        new(quat.X + quat2.X, quat.X + quat2.Y,
            quat.Z + quat2.Z, quat.W + quat2.W);

    public static Quaternion operator -(Quaternion quat) => new(-quat.X, -quat.Y, -quat.Z, -quat.W);

    public static Quaternion operator *(double s, Quaternion quat) =>
        new(quat.X * s, quat.Y * s, quat.Z * s, quat.W * s);

    public static Quaternion operator *(Quaternion quat, double s) =>
        new(quat.X * s, quat.Y * s, quat.Z * s, quat.W * s);

    public static Quaternion operator *(Quaternion q1, Quaternion q2) =>
        new(
            q1.W * q2.X + q1.X * q2.W +
            q1.Y * q2.Z - q1.Z * q2.Y,
            q1.W * q2.Y + q1.Y * q2.W +
            q1.Z * q2.X - q1.X * q2.Z,
            q1.W * q2.Z + q1.Z * q2.W +
            q1.X * q2.Y - q1.Y * q2.X,
            q1.W * q2.W - q1.X * q2.X -
            q1.Y * q2.Y - q1.Z * q2.Z);

    public static Quaternion operator /(Quaternion quat, double s)
    {
        s = 1 / s;
        return new Quaternion(quat.X * s, quat.Y * s, quat.Z * s, quat.W * s);
    }

    public static bool operator ==(Quaternion quat1, Quaternion quat2) => quat1.Equals(quat2);

    public static bool operator !=(Quaternion quat1, Quaternion quat2) => !quat1.Equals(quat2);

    #endregion

    #region ICloneable Members

    object ICloneable.Clone() => MemberwiseClone();

    #endregion
}