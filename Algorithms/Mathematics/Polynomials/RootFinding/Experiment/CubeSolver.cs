using System.Numerics;

namespace Algorithms.Mathematics.RootFinding.Experiment;

public class CubeSolver
{
    /// <summary>
    /// Return distinct roots of a*x^3+b*x^2+cx+d
    /// </summary>
    public static List<double> Solve(double a, double b, double c, double d)
    { 
        const double EPS = 1e-10;
        var res = new List<double>(3);

        if (Math.Abs(a) < EPS)
        {
            if (Math.Abs(b) < EPS)
            {
                if (Math.Abs(c) >= EPS)
                    res.Add(-d / c);
                return res;
            }

            double D = c * c - 4 * b * d;
            if (D < 0)
                return res;

            double A = -c / (2 * b);
            if (D < EPS)
                res.Add(A);
            else
            {
                D = Math.Sqrt(D) / (2 * b);
                res.Add(A + D);
                res.Add(A - D);
            }
        }
        else
        {
            double Q = (3 * a * c - b * b) / (9 * a * a);
            double R = (9 * a * b * c - 27 * a * a * d - 2 * b * b * b) / (54 * a * a * a);
            double D = Q * Q * Q + R * R;
            var S = Complex.Pow(R + Complex.Sqrt(D), 1 / 3.0);
            var T = Complex.Pow(R - Complex.Sqrt(D), 1 / 3.0);
            var sol1 = S + T - b / (3 * a);
            var sol2 = new Complex(-1.0 / 2, 0) * (S + T) - (b / (3 * a)) + new Complex(0, Math.Sqrt(3.0) / 2) * (S - T);
            var sol3 = new Complex(-1.0 / 2, 0) * (S + T) - (b / (3 * a)) - new Complex(0, Math.Sqrt(3.0) / 2) * (S - T);
            res.Add(sol1.Real);
            if (Math.Abs(sol2.Imaginary) < EPS) res.Add(sol2.Real);
            if (Math.Abs(sol3.Imaginary) < EPS) res.Add(sol3.Real);
        }
        return res;
    }
}
