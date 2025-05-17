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
/// Summary description for Point2D Vector.
/// </summary>
public partial struct Point2D
{
    public bool? InTriangle(Point2D a, Point2D b, Point2D c)
    {
        var ab = Math.Abs(this.Cross(a, b));
        var bc = Math.Abs(this.Cross(b, c));
        var ca = Math.Abs(this.Cross(c, a));
        var s1 = Math.Abs(a.Cross(b, c));
        var s2 = ab + bc + ca;

        if (s1 != s2)
            return false;

        return ab != 0 && bc != 0 && ca != 0 ? true : (bool?)null;
    }

    public bool? InConvexPolygon(Point2D[] poly)
    {
        // May need to cull out collinear points before calling this function
        int left = 1, right = poly.Length - 1;
        while (left <= right)
        {
            int mid = (left + right) >> 1;
            var cmp = poly[0].Cross(poly[mid], this);
            if (cmp > 0 || cmp == 0 && mid > 1)
                left = mid + 1;
            else
                right = mid - 1;
        }

        if (left >= poly.Length) left = 0;
        var cross = poly[right].Cross(poly[left], this);
        return (cross == 0 && this.Dot(poly[left], poly[right]) <= 0) ? (bool?)null : cross > 0;
    }

    public bool? InPolygon(Point2D[] poly, bool winding = true)
    {
        int inside = 0;
        for (int i = poly.Length - 1, j = 0; i >= 0; j = i--)
        {
            Point2D a = poly[i], b = poly[j];
            if ((a.Y - Y) * (b.Y - Y) > 0) continue;
            var cross = a.Cross(b, this);
            if (cross == 0 && Dot(a, b) <= 0) return null;
            if (Y < (cross > 0 ? a.Y : b.Y) && cross != 0)
                inside = winding ? (inside + Math.Sign(cross)) : (inside ^ 1);
        }
        return inside != 0;
    }
}