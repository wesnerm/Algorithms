namespace Algorithms.ComputationalGeometry;

public class Geometry3D
{
    // distance from point (px, py, pz) to line (x1, y1, z1)-(x2, y2, z2)
    // (or ray, or segment; in the case of the ray, the endpoint is the
    // first point)
    public const int Line = 0;
    public const int Segment = 1;
    public const int Ray = 2;

    /// <summary>
    ///     distance from point (x, y, z) to plane aX + bY + cZ + d = 0
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    public static double PtPlaneDist(double x, double y, double z,
        double a, double b, double c, double d) =>
        Math.Abs(a * x + b * y + c * z + d) / Math.Sqrt(a * a + b * b + c * c);

    /// <summary>
    ///     distance between parallel planes aX + bY + cZ + d1 = 0 and
    ///     aX + bY + cZ + d2 = 0
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
    /// <returns></returns>
    public static double PlanePlaneDist(double a, double b, double c,
        double d1, double d2) =>
        Math.Abs(d1 - d2) / Math.Sqrt(a * a + b * b + c * c);

    /// <summary>
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="z1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="z2"></param>
    /// <param name="px"></param>
    /// <param name="py"></param>
    /// <param name="pz"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static double PtLineDistSq(double x1, double y1, double z1,
        double x2, double y2, double z2, double px, double py, double pz,
        int type)
    {
        double pd2 = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z1 - z2) * (z1 - z2);

        double x, y, z;
        if (pd2 == 0) {
            x = x1;
            y = y1;
            z = z1;
        } else {
            double u = ((px - x1) * (x2 - x1) + (py - y1) * (y2 - y1) + (pz - z1) * (z2 - z1)) / pd2;
            x = x1 + u * (x2 - x1);
            y = y1 + u * (y2 - y1);
            z = z1 + u * (z2 - z1);
            if (type != Line && u < 0) {
                x = x1;
                y = y1;
                z = z1;
            }

            if (type == Segment && u > 1.0) {
                x = x2;
                y = y2;
                z = z2;
            }
        }

        return (x - px) * (x - px) + (y - py) * (y - py) + (z - pz) * (z - pz);
    }

    public static double PtLineDist(double x1, double y1, double z1,
        double x2, double y2, double z2, double px, double py, double pz,
        int type) =>
        Math.Sqrt(PtLineDistSq(x1, y1, z1, x2, y2, z2, px, py, pz, type));
}