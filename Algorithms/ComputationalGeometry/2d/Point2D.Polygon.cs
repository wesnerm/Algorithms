#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.ComputationalGeometry;

/// <summary>
///     Summary description for Point2D Vector.
/// </summary>
public partial struct Point2D
{
    public bool? InTriangle(Point2D a, Point2D b, Point2D c)
    {
        double ab = Math.Abs(Cross(a, b));
        double bc = Math.Abs(Cross(b, c));
        double ca = Math.Abs(Cross(c, a));
        double s1 = Math.Abs(a.Cross(b, c));
        double s2 = ab + bc + ca;

        if (s1 != s2)
            return false;

        return ab != 0 && bc != 0 && ca != 0 ? true : null;
    }

    public bool? InConvexPolygon(Point2D[] poly)
    {
        // May need to cull out collinear points before calling this function
        int left = 1, right = poly.Length - 1;
        while (left <= right) {
            int mid = (left + right) >> 1;
            double cmp = poly[0].Cross(poly[mid], this);
            if (cmp > 0 || (cmp == 0 && mid > 1))
                left = mid + 1;
            else
                right = mid - 1;
        }

        if (left >= poly.Length) left = 0;
        double cross = poly[right].Cross(poly[left], this);
        return cross == 0 && Dot(poly[left], poly[right]) <= 0 ? null : cross > 0;
    }

    public bool? InPolygon(Point2D[] poly, bool winding = true)
    {
        int inside = 0;
        for (int i = poly.Length - 1, j = 0; i >= 0; j = i--) {
            Point2D a = poly[i], b = poly[j];
            if ((a.Y - Y) * (b.Y - Y) > 0) continue;
            double cross = a.Cross(b, this);
            if (cross == 0 && Dot(a, b) <= 0) return null;
            if (Y < (cross > 0 ? a.Y : b.Y) && cross != 0)
                inside = winding ? inside + Math.Sign(cross) : inside ^ 1;
        }

        return inside != 0;
    }
}