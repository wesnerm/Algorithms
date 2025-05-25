using static System.Math;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Algorithms.Mathematics;

public class LpSolverMinNoConstraints
{
    const double Epsilon = 1e-9;
    public double Answer;

    public LpSolverMinNoConstraints(double[,] A, double[] b, double[] c)
    {
        int n = c.Length;
        int m = b.Length;

        double[,] T = new double[m + 1, n + m + 1];
        for (int j = 0; j < m; ++j) {
            for (int i = 0; i < n; ++i)
                T[j, i] = A[j, i];
            T[j, n + j] = 1;
            T[j, n + m] = b[j];
        }

        for (int i = 0; i < n; ++i)
            T[m, i] = c[i];

        while (true) {
            int p = 0, q = 0;
            for (int i = 0; i < n + m; ++i)
                if (T[m, i] <= T[m, p])
                    p = i;

            for (int j = 0; j < m; ++j)
                if (T[j, n + m] <= T[q, n + m])
                    q = j;

            double t = Min(T[m, p], T[q, n + m]);
            if (t >= -Epsilon) {
                Answer = -T[m, n + m];
                return;
            }

            if (t < T[q, n + m]) {
                for (int j = 0; j < m; ++j)
                    if (T[j, p] >= Epsilon
                        && T[j, p] * Abs(T[q, n + m] - t) >= T[q, p] * Abs(T[j, n + m] - t))
                        q = j;

                if (T[q, p] <= Epsilon) {
                    Answer = double.MaxValue;
                    return;
                }
            } else {
                for (int i = 0; i < n + m + 1; ++i)
                    T[q, i] *= -1;

                for (int i = 0; i < n + m; ++i)
                    if (T[q, i] >= Epsilon && T[q, i] * Abs(T[m, p] - t) >= T[q, p] * Abs(T[m, i] - t))
                        p = i;

                if (T[q, p] <= Epsilon) {
                    Answer = double.MinValue;
                    return;
                }
            }

            for (int j = 0; j < m + 1; ++j)
                if (j != q)
                    T[j, p] /= T[q, p];

            T[q, p] = 1;
            for (int j = 0; j < m + 1; ++j)
                if (j != q) {
                    double alpha = T[j, p] / T[q, p];
                    for (int i = 0; i < n + m + 1; ++i)
                        T[j, i] -= T[q, i] * alpha;
                }
        }
    }
}