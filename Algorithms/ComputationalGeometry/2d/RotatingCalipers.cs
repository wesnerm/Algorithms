#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.ComputationalGeometry;

public static class RotatingCalipers
{
    public static IEnumerable<Tuple<int, int>> Generate(Point2D[] pts)
    {
        int n = pts.Length - 1;
        int i0 = Next(0, n);
        Point2D pt0 = pts[0];
        Point2D pt1 = pts[i0];

        while (pt0.Cross(pt1, pts[Next(i0, n)]) > pt0.Cross(pt1, pts[i0]))
            i0 = Next(i0, n);

        for (int i = i0, k = 0; i != n; k = Next(k, n)) {
            yield return Tuple.Create(k, i);

            pt0 = pts[k];
            pt1 = pts[Next(k, n)];

            while (pt0.Cross(pt1, pts[Next(i, n)]) > pt0.Cross(pt1, pts[i])) {
                i = Next(i, n);
                if (k == i0 && i == n) yield break;
                yield return Tuple.Create(k, i);
            }

            if (pts[i].Cross(pt1, pts[Next(i, n)]) == pt0.Cross(pt1, pts[i]))
                yield return k != i0 || i != n
                    ? Tuple.Create(k, Next(i, n))
                    : Tuple.Create(Next(k, n), i);
        }
    }

    public static IEnumerable<Tuple<int, int>> Generate2(Point2D[] pts)
    {
        int n = pts.Length - 1;
        int j = Next(0, n);
        Point2D pt0 = pts[0];
        Point2D pt1 = pts[j];

        while (pt0.Cross(pt1, pts[Next(j, n)]) > pt0.Cross(pt1, pts[j]))
            j = Next(j, n);

        for (int i = 0, j0 = j; i + 1 < j0 || j <= n;) {
            yield return Tuple.Create(i, j);
            if (i + 1 < j0
                && (j >= n || (pts[j + 1] - pts[j]).Cross(pts[i + 1] - pts[i]) > 0))
                i++;
            else
                j++;
        }
    }

    static int Next(int i, int n) => i < n ? i + 1 : 0;
}