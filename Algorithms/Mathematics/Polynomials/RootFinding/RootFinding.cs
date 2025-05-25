namespace Algorithms.Mathematics.RootFinding;

public static class RootFinding
{
    public static double[] FindRoots(double[] poly)
    {
        switch (poly.Length) {
            case 0:
            case 1: return Array.Empty<double>();
            case 2: return new[] { -poly[0] / poly[1] };
            case 3: return QuadraticRoots(poly[2], poly[1], poly[0]);
            default:
                // ComplexRootFinding
                return null;
        }
    }

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
}