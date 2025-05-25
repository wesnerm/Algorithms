using Algorithms.ComputationalGeometry;

namespace Algorithms.Mathematics.Matrices;

public class Transformation2D
{
    public double Xa, Xb, xOffset;
    public double Ya, Yb, yOffset;

    public Transformation2D() => Xa = Yb = 1;

    public double Determinant => Xa * Yb - Xb * Ya;

    public List<Point2D> Transform(List<Point2D> points)
    {
        var list = new List<Point2D>();
        foreach (Point2D pt in points)
            list.Add(Transform(pt));
        return list;
    }

    public Point2D Transform(Point2D pt) => new(Xa * pt.X + Xb * pt.Y + xOffset, Ya * pt.X + Yb * pt.Y + yOffset);

    public static Transformation2D Translate(double x, double y) => new() { xOffset = x, yOffset = y };

    public static Transformation2D Transpose() => new() { Xa = 0, Xb = 1, Ya = 0, Yb = 1 };

    public static Transformation2D MapPoints(Point2D self1, Point2D self2, Point2D self3, Point2D new1, Point2D new2,
        Point2D new3)
    {
        double[,] matrix =
        {
            { (int)self1.X, (int)self1.Y, 1 },
            { (int)self2.X, (int)self2.Y, 1 },
            { (int)self3.X, (int)self3.Y, 1 },
        };

        double[,] inverse = Invert(matrix);

        var t = new Transformation2D
        {
            Xa = inverse[0, 0] * new1.X + inverse[0, 1] * new2.X + inverse[0, 2] * new3.X,
            Xb = inverse[1, 0] * new1.X + inverse[1, 1] * new2.X + inverse[1, 2] * new3.X,
            xOffset = inverse[2, 0] * new1.X + inverse[2, 1] * new2.X + inverse[2, 2] * new3.X,
            Ya = inverse[0, 0] * new1.Y + inverse[0, 1] * new2.Y + inverse[0, 2] * new3.Y,
            Yb = inverse[1, 0] * new1.Y + inverse[1, 1] * new2.Y + inverse[1, 2] * new3.Y,
            yOffset = inverse[2, 0] * new1.Y + inverse[2, 1] * new2.Y + inverse[2, 2] * new3.Y,
        };

        Debug.Assert(t.Xa * self1.X + t.Xb * self1.Y + t.xOffset == new1.X);
        Debug.Assert(t.Ya * self1.X + t.Yb * self1.Y + t.yOffset == new1.Y);
        Debug.Assert(t.Xa * self2.X + t.Xb * self2.Y + t.xOffset == new2.X);
        Debug.Assert(t.Ya * self2.X + t.Yb * self2.Y + t.yOffset == new2.Y);
        Debug.Assert(t.Xa * self3.X + t.Xb * self3.Y + t.xOffset == new3.X);
        Debug.Assert(t.Ya * self3.X + t.Yb * self3.Y + t.yOffset == new3.Y);
        return t;
    }

    public static double[,] Invert(double[,] a)
    {
        // determinant is 1 or -1
        double det = a[0, 0] * (a[1, 1] * a[2, 2] - a[2, 1] * a[1, 2]) -
                     a[0, 1] * (a[1, 0] * a[2, 2] - a[1, 2] * a[2, 0]) +
                     a[0, 2] * (a[1, 0] * a[2, 1] - a[1, 1] * a[2, 0]);

        double invdet = 1 / det;
        return new[,]
        {
            {
                (a[1, 1] * a[2, 2] - a[2, 1] * a[1, 2]) * invdet,
                (a[0, 2] * a[2, 1] - a[0, 1] * a[2, 2]) * invdet,
                (a[0, 1] * a[1, 2] - a[0, 2] * a[1, 1]) * invdet,
            },
            {
                (a[1, 2] * a[2, 0] - a[1, 0] * a[2, 2]) * invdet,
                (a[0, 0] * a[2, 2] - a[0, 2] * a[2, 0]) * invdet,
                (a[1, 0] * a[0, 2] - a[0, 0] * a[1, 2]) * invdet,
            },
            {
                (a[1, 0] * a[2, 1] - a[2, 0] * a[1, 1]) * invdet,
                (a[2, 0] * a[0, 1] - a[0, 0] * a[2, 1]) * invdet,
                (a[0, 0] * a[1, 1] - a[1, 0] * a[0, 1]) * invdet,
            },
        };
    }
}