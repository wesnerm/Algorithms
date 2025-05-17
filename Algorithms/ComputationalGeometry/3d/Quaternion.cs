#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Text;
using Algorithms.Mathematics;

#endregion

namespace Algorithms.ComputationalGeometry;

/// <summary>
/// Summary description for Quaternion.
/// </summary>
public struct Quaternion : ICloneable
{
    #region Constant

    private const double Degrees = Numbers.Degrees;

    #endregion

    #region Variables

    private float x;
    private float y;
    private float z;
    private float w;

    #endregion

    #region Construction

    public Quaternion(double x, double y, double z, double w)
    {
        this.x = (float) x;
        this.y = (float) y;
        this.z = (float) z;
        this.w = (float) w;
    }

    public Quaternion(Vector3D axis, double angleDegrees)
    {
        double angle = angleDegrees*Degrees;
        double length = axis.Length;
        if (length == 0) throw new DivideByZeroException();
        double factor = Math.Sin(angle*.5)/length;
        x = (float) (axis.X*factor);
        y = (float) (axis.Y*factor);
        z = (float) (axis.Z*factor);
        w = (float) Math.Cos(angle*.5);
    }

    #endregion

    #region Object Overrides

    public override bool Equals(object obj)
    {
        if (!(obj is Quaternion))
            return false;
        return Equals((Quaternion) obj);
    }

    public bool Equals(Quaternion q)
    {
        return x == q.x && y == q.y && z == q.z && w == q.w;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Axis},{Angle}";
    }

    public static Quaternion Parse(string text)
    {
        var terms = text.Trim().Split(',');
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

    public string Text
    {
        get { return ToString(); }
    }

    public float X
    {
        [DebuggerStepThrough]
        get { return x; }
        set { x = value; }
    }

    public float Y
    {
        [DebuggerStepThrough]
        get { return y; }
        set { y = value; }
    }

    public float Z
    {
        [DebuggerStepThrough]
        get { return z; }
        set { z = value; }
    }

    public float W
    {
        [DebuggerStepThrough]
        get { return w; }
        set { w = value; }
    }

    public float Magnitude
    {
        get { return (float) Math.Sqrt(x*x + y*y + z*z + w*w); }
    }

    public bool IsNormalized
    {
        get { return Magnitude == 1.0; }
    }

    public Quaternion Conjugate
    {
        get { return new Quaternion(-x, -y, -z, w); }
    }

    public Quaternion Inverse
    {
        get
        {
            double norm = Magnitude;
            if (norm == 0)
                throw new DivideByZeroException();
            norm = 1/norm;
            return new Quaternion(-x*norm, -y*norm, -z*norm, w*norm);
        }
    }

    public Vector3D Axis
    {
        get
        {
            if (!IsNormalized) throw new InvalidOperationException();
            return new Vector3D(x, y, z)/Magnitude;
        }
    }

    public float Angle
    {
        get
        {
            if (!IsNormalized) throw new InvalidOperationException();
            return (float) (Math.Acos(w)*2.0*Degrees);
        }
    }

    public Quaternion AxisAngle
    {
        get
        {
            double tmpW = Math.Acos(w);
            double scale = 1.0/Math.Sin(tmpW);
            return new Quaternion(
                x*scale,
                y*scale,
                z*scale,
                tmpW*(360.0/Math.PI));
        }
    }

    #endregion

    #region Operations

    public static Quaternion Slerp(Quaternion q1, Quaternion q2, double t)
    {
        const double delta = 1e-6;

        // Calculate cos of omega.
        double cosOmega = q1.x*q2.x + q1.y*q2.y + q1.z*q2.z + q1.w*q2.w;

        // Adjust signs if necessary.
        if (cosOmega < 0.0)
        {
            cosOmega = -cosOmega;
            q2.x = -q2.x;
            q2.y = -q2.y;
            q2.z = -q2.z;
            q2.w = -q2.w;
        }

        // Calculate scaling coefficients.
        double scale0, scale1;
        if ((1.0 - cosOmega) > delta)
        {
            double omega;
            if (cosOmega > 1.0)
                omega = 0;
            else if (cosOmega < -1.0)
                omega = Math.PI;
            else
                omega = Math.Acos(cosOmega);
            double sinOmega = Math.Sin(omega);
            scale0 = Math.Sin((1.0 - t)*omega)/sinOmega;
            scale1 = Math.Sin(t*omega)/sinOmega;
        }
        else
        {
            scale0 = 1.0 - t;
            scale1 = t;
        }

        return new Quaternion(
            scale0*q1.x + scale1*q2.x,
            scale0*q1.y + scale1*q2.y,
            scale0*q1.z + scale1*q2.z,
            scale0*q1.w + scale1*q2.w);
    }

    public void Normalize()
    {
        double d = Magnitude;
        if (d == 0)
            return;
        d = 1/d;
        x = (float) (x*d);
        y = (float) (y*d);
        z = (float) (z*d);
        w = (float) (w*d);
    }

    #endregion

    #region Utility

    public static void Swap(ref Quaternion quat1, ref Quaternion quat2)
    {
        var tmp = quat1;
        quat1 = quat2;
        quat2 = tmp;
    }

    #endregion

    #region Math

    public static Quaternion operator -(Quaternion quat, Quaternion quat2)
    {
        return new Quaternion(quat.x - quat2.x, quat.y - quat2.y,
                              quat.z - quat2.z, quat.w - quat2.w);
    }

    public static Quaternion operator +(Quaternion quat, Quaternion quat2)
    {
        return new Quaternion(quat.x + quat2.x, quat.x + quat2.y,
                              quat.z + quat2.z, quat.w + quat2.w);
    }

    public static Quaternion operator -(Quaternion quat)
    {
        return new Quaternion(-quat.x, -quat.y, -quat.z, -quat.w);
    }

    public static Quaternion operator *(double s, Quaternion quat)
    {
        return new Quaternion(quat.x*s, quat.y*s, quat.z*s, quat.w*s);
    }

    public static Quaternion operator *(Quaternion quat, double s)
    {
        return new Quaternion(quat.x*s, quat.y*s, quat.z*s, quat.w*s);
    }

    public static Quaternion operator *(Quaternion q1, Quaternion q2)
    {
        return new Quaternion(
            q1.w*q2.x + q1.x*q2.w +
            q1.y*q2.z - q1.z*q2.y,
            q1.w*q2.y + q1.y*q2.w +
            q1.z*q2.x - q1.x*q2.z,
            q1.w*q2.z + q1.z*q2.w +
            q1.x*q2.y - q1.y*q2.x,
            q1.w*q2.w - q1.x*q2.x -
            q1.y*q2.y - q1.z*q2.z);
    }

    public static Quaternion operator /(Quaternion quat, double s)
    {
        s = 1/s;
        return new Quaternion(quat.x*s, quat.y*s, quat.z*s, quat.w*s);
    }

    public static bool operator ==(Quaternion quat1, Quaternion quat2)
    {
        return quat1.Equals(quat2);
    }

    public static bool operator !=(Quaternion quat1, Quaternion quat2)
    {
        return !quat1.Equals(quat2);
    }

    #endregion

    #region ICloneable Members

    object ICloneable.Clone()
    {
        return MemberwiseClone();
    }

    #endregion
}