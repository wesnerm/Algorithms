using System.Runtime.InteropServices;

namespace Algorithms.Mathematics.Equations;

public static class EquationSolving
{
    public static double[] GaussianElimination(double[][] A, double[] b)
    {
        int n = b.Length;
        for (int p = 0; p < n; p++)
        {
            int max = p;
            for (int i = p + 1; i < n; i++)
                if (Math.Abs(A[i][p]) > Math.Abs(A[max][p]))
                    max = i;

            double[] prow = A[max];
            A[max] = A[p];
            A[p] = prow;

            double t = b[p];
            b[p] = b[max];
            b[max] = t;

            double ipivot = 1.0 / A[p][p];
            for (int i = p + 1; i < n; i++)
            {
                double[] row = A[i];
                double alpha = -row[p] * ipivot;
                b[i] = b[i] + alpha * b[p];
                for (int j = p; j < n; j++) row[j] += alpha * prow[j];
            }
        }

        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            double[] row = A[i];
            double sum = 0;
            for (int j = i + 1; j < n; j++) sum += row[j] * x[j];
            x[i] = (b[i] - sum) / row[i];
        }

        return x;
    }

    public static double[][] Inverse(double[][] A)
    {
        int n = A.Length;
        double[][] inverse = new double[n][];
        for (int i = 0; i < n; i++)
        {
            inverse[i] = new double[n];
            inverse[i][i] = 1.0;
        }

        for (int p = 0; p < n; p++)
        {
            int max = p;
            for (int i = p + 1; i < n; i++)
                if (Math.Abs(A[i][p]) > Math.Abs(A[max][p]))
                    max = i;

            if (A[max][p] == 0)
                return null;

            double[] prow = A[max];
            A[max] = A[p];
            A[p] = prow;

            double[] t = inverse[p];
            inverse[p] = inverse[max];
            inverse[max] = t;

            double ipivot = 1.0 / A[p][p];
            for (int i = p + 1; i < n; i++)
            {
                double[] row = A[i];
                double alpha = -row[p] * ipivot;

                for (int j = 0; j < n; j++)
                    inverse[i][j] = inverse[i][j] + alpha * inverse[p][j];

                for (int j = p; j < n; j++)
                    row[j] += alpha * prow[j];
            }
        }

        for (int i = n - 1; i >= 0; i--)
        {
            double[] row = A[i];
            for (int k = 0; k < n; k++)
            {
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                    sum += row[j] * inverse[j][k];
                inverse[i][k] = (inverse[i][k] - sum) / row[i];
            }
        }

        return inverse;
    }

    public static double[,] Inverse(double[,] A)
    {
        int n = A.GetLength(0);
        double[,] inverse = new double[n, n];
        for (int i = 0; i < n; i++)
            inverse[i, i] = 1.0;

        for (int p = 0; p < n; p++)
        {
            int max = p;
            for (int i = p + 1; i < n; i++)
                if (Math.Abs(A[i, p]) > Math.Abs(A[max, p]))
                    max = i;

            if (A[max, p] == 0)
                return null;

            SwapRows(A, p, max);
            SwapRows(inverse, p, max);

            double ipivot = 1.0 / A[p, p];
            for (int i = p + 1; i < n; i++)
            {
                var prow = MemoryMarshal.CreateSpan(ref A[p, 0], n);
                var row = MemoryMarshal.CreateSpan(ref A[i, 0], n);
                double alpha = -row[p] * ipivot;

                for (int j = 0; j < n; j++)
                    inverse[i, j] = inverse[i, j] + alpha * inverse[p, j];

                for (int j = p; j < n; j++)
                    row[j] += alpha * prow[j];
            }
        }

        for (int i = n - 1; i >= 0; i--)
        {
            var row = MemoryMarshal.CreateSpan(ref A[i, 0], n);
            for (int k = 0; k < n; k++)
            {
                double sum = 0;
                for (int j = i + 1; j < n; j++)
                    sum += row[j] * inverse[j, k];
                inverse[i, k] = (inverse[i, k] - sum) / row[i];
            }
        }

        return inverse;
    }

    public static void SwapRows(double[,] a, int r1, int r2)
    {
        int m = a.GetLength(1);
        for (int i = 0; i < m; i++)
        {
            double tmp = a[r1, i];
            a[r1, i] = a[r2, i];
            a[r2, i] = tmp;
        }
    }

    public static double Determinant(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        double det = 1.0;

        for (int i = 0; i < n; i++)
        {
            double pivotElement = matrix[i, i];
            int pivotRow = i;
            for (int r = i + 1; r < n; ++r)
                if (Math.Abs(matrix[r, i]) > Math.Abs(pivotElement))
                {
                    pivotElement = matrix[r, i];
                    pivotRow = r;
                }

            if (pivotElement == 0.0)
                return 0;

            if (pivotRow != i)
            {
                for (int j = 0; j < n; j++)
                {
                    double tmp = matrix[i, j];
                    matrix[i, j] = matrix[pivotRow, j];
                    matrix[pivotRow, j] = tmp;
                }

                det *= -1.0;
            }

            det *= pivotElement;

            for (int r = i + 1; r < n; ++r)
                for (int c = i + 1; c < n; ++c)
                    matrix[r, c] -= matrix[r, i] * matrix[i, c] / pivotElement;
        }

        return det;
    }

    public static double[,] LUDecompose(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        double[,] lu = new double[n, n];
        double sum = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = i; j < n; j++)
            {
                sum = 0;
                for (int k = 0; k < i; k++)
                    sum += lu[i, k] * lu[k, j];
                lu[i, j] = matrix[i, j] - sum;
            }

            for (int j = i + 1; j < n; j++)
            {
                sum = 0;
                for (int k = 0; k < i; k++)
                    sum += lu[j, k] * lu[k, i];
                lu[j, i] = 1 / lu[i, i] * (matrix[j, i] - sum);
            }
        }

        return lu;
    }

    public static double[] LUSolve(double[,] lu, double[] b)
    {
        // find solution of Ly = b
        int n = lu.GetLength(0);
        double sum;
        double[] y = new double[n];
        for (int i = 0; i < n; i++)
        {
            sum = 0;
            for (int k = 0; k < i; k++)
                sum += lu[i, k] * y[k];
            y[i] = b[i] - sum;
        }

        // find solution of Ux = y
        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            sum = 0;
            for (int k = i + 1; k < n; k++)
                sum += lu[i, k] * x[k];
            x[i] = 1 / lu[i, i] * (y[i] - sum);
        }

        return x;
    }

    public static double[] Solve(double[,] matrix, double[] b)
    {
        double[,] lu = LUDecompose(matrix);
        return LUSolve(lu, b);
    }
}