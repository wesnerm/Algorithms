using Algorithms.Mathematics.Equations;
using static Algorithms.Mathematics.Matrices.MatrixOperations;

namespace Algorithms.Mathematics.Numerics;

public static class Interpolation
{
    public static double LagrangeInterpolation(double[] xs, double[] ys, double xi)
    {
        double result = 0;
        for (int i = 0, n = ys.Length; i < n; i++) {
            double term = ys[i];
            for (int j = 0; j < n; j++)
                if (j != i)
                    term = term * (xi - xs[j]) / (xs[i] - xs[j]);
            result += term;
        }

        return result;
    }

    public static double[] InterpolatePolynomials(double[] xs, double[] ys)
    {
        int n = xs.Length;
        double[][] mat = new double[n][];

        mat[0] = new double[n];
        for (int j = 0; j < n; j++)
            mat[0][j] = 1;

        for (int i = 1; i < n; i++) {
            double[] pre = mat[i - 1];
            double[] row = mat[i] = new double[n];
            for (int j = 0; j < n; j++)
                row[j] = pre[j] * xs[j];
        }

        return EquationSolving.GaussianElimination(mat, ys);
    }

    public static double[] InterpolateNaive(double[] x, double[] y)
    {
        int n = x.Length;
        double[] dp = new double[n + 1];
        dp[0] = 1;
        for (int i = 0; i < n; ++i)
        for (int j = i; j >= 0; --j) {
            dp[j + 1] += dp[j];
            dp[j] *= -x[i];
        }

        double[] r = new double[n];
        for (int i = 0; i < n; ++i) {
            double den = 1, res = 0;
            for (int j = 0; j < n; ++j)
                if (i != j)
                    den *= x[i] - x[j];
            den = 1 / den;

            for (int j = n - 1; j >= 0; --j) {
                res = dp[j + 1] + res * x[i];
                r[j] += res * den * y[i];
            }
        }

        return r;
    }

    #region Vandermonde LU Matrix

    static double[,] InverseL(double[] A)
    {
        int n = A.Length;
        double[,] mat = new double[n, n];

        mat[0, 0] = 1;
        for (int i = 1; i < n; i++) {
            double tmp = 1;

            for (int j = 0; j < i; j++) {
                mat[i, j] = mat[i - 1, j] / (A[j] - A[i]);
                tmp *= A[i] - A[j];
            }

            mat[i, i] = 1 / tmp;
        }

        return mat;
    }

    static double[,] InverseU(double[] A)
    {
        int n = A.Length;
        double[,] mat = new double[n, n];

        for (int i = 0; i < n; i++) {
            mat[i, i] = 1;
            for (int j = i + 1; j < n; j++)
                if (i != 0)
                    mat[i, j] = mat[i - 1, j - 1] - mat[i, j - 1] * A[j - 1];
                else
                    mat[i, j] = -mat[i, j - 1] * A[j - 1];
        }

        return mat;
    }

    public static double[] Interpolate(double[] xs, double[] ys)
    {
        double[,] invL = InverseL(xs);
        double[,] invU = InverseU(ys);

        // TODO: Make sure order is right
        return MultVector(invL, MultVector(invU, xs));
    }

    #endregion
}