namespace Algorithms.Mathematics;

public static class RootFinder
{
    //  Purpose:
    //    R82POLY2_TYPE analyzes a second order polynomial in two variables.
    //  Discussion:
    //    The polynomial has the form
    //      A x^2 + B y^2 + C xy + Dx + Ey + F = 0
    //    The possible types of the solution set are:
    //
    //     1: a hyperbola;
    //        9x^2 -  4y^2       -36x - 24y -  36 = 0
    //     2: a parabola;
    //        4x^2 +  1y^2 - 4xy + 3x -  4y +   1 = 0;
    //     3: an ellipse;
    //        9x^2 + 16y^2       +36x - 32y -  92 = 0;
    //     4: an imaginary ellipse (no real solutions);
    //         x^2 +   y^2       - 6x - 10y + 115 = 0;
    //     5: a pair of intersecting lines;
    //                        xy + 3x -   y -   3 = 0
    //     6: one point;
    //         x^2 +  2y^2       - 2x + 16y +  33 = 0;

    //  Parameters:
    //    Input, double A, B, C, D, E, F, the coefficients.
    //    Output, int TYPE, indicates the type of the solution set.

    public static int GetQuadraticType(double a, double b, double c,
        double d, double e, double f)
    {
        //  Handle the degenerate case.
        if (a == 0.0 && b == 0.0 && c == 0.0)
            return -1;

        double delta =
            8.0 * a * b * f
            + 2.0 * c * e * d
            - 2.0 * a * e * e
            - 2.0 * b * d * d
            - 2.0 * f * c * c;

        double j = 4.0 * a * b - c * c;

        if (delta != 0.0) {
            if (j < 0.0)
                return 1;
            if (j == 0.0)
                return 2;
            if (Math.Sign(delta) != Math.Sign(a + b))
                return 3;
            return 4;
        }

        if (j < 0.0)
            return 5;
        if (0.0 < j)
            return 6;

        return -1;
    }

    [DebuggerStepThrough]
    public static double[] QuadraticRoots(double a, double b, double c)
    {
        double d = b * b - 4 * a * c;
        if (d < 0)
            return Array.Empty<double>();
        double q = -0.5 * (b + Math.Sign(b) * Math.Sqrt(d));
        int count = q != 0 && a != 0 ? 2 : 1;
        double[] result = new double[count];
        if (a != 0)
            result[0] = q / a;
        if (q != 0)
            result[count - 1] = c / q;
        return result;
    }

    [DebuggerStepThrough]
    public static double[] GetRoots(params double[] poly) =>
        // TODO: Rootfinder
        //double[] roots = new Polynomial(poly).FindRoots();
        //if (roots.Length > 1)
        //{
        //    Array.Sort(roots);
        //    roots = ListTools.RemoveAdjacentDuplicates(roots);
        //}
        //return roots;
        null;

    [DebuggerStepThrough]
    public static Number[] GetRootsN(params double[] poly) => ToNumber(GetRoots(poly));

    [DebuggerStepThrough]
    public static Number[] ToNumber(params double[] roots)
    {
        var array = new Number[roots.Length];
        for (int i = 0; i < roots.Length; i++)
            array[i] = Number.Rational(roots[i]);
        return array;
    }
}