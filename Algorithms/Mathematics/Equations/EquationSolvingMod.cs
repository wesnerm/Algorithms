namespace Algorithms.Mathematics.Equations;

public static class EquationSolvingMod
{
    public static int[] GaussianElimination(int[][] A, int[] b, int mod)
    {
        int n = b.Length;
        for (int p = 0; p < n; p++)
        {
            int max = p;
            for (int i = p; i < n; i++)
                if (A[i][p] != 0)
                {
                    max = i;
                    break;
                }

            int[] prow = A[max];
            A[max] = A[p];
            A[p] = prow;

            int t = b[p];
            b[p] = b[max];
            b[max] = t;

            int ipivot = InverseDirect(A[p][p], mod);
            for (int i = p + 1; i < n; i++)
            {
                int[] row = A[i];
                long alpha = mod - row[p] * ipivot % mod;
                b[i] = (int)((b[i] + alpha * b[p]) % mod);
                for (int j = p; j < n; j++) row[j] = (int)((row[j] + alpha * prow[j]) % mod);
            }
        }

        int[] x = new int[n];
        for (int i = n - 1; i >= 0; i--)
        {
            int[] row = A[i];
            long sum = mod - b[i];
            for (int j = i + 1; j < n; j++) sum = (sum + (long)row[j] * x[j]) % mod;
            x[i] = (int)Div(mod - sum, row[i], mod);
        }

        return x;
    }

    public static int[][] Inverse(int[][] A, int mod)
    {
        int n = A.Length;
        int[][] inverse = new int[n][];
        for (int i = 0; i < n; i++)
        {
            inverse[i] = new int[n];
            inverse[i][i] = 1;
        }

        for (int p = 0; p < n; p++)
        {
            int max = p;
            for (int i = p; i < n; i++)
                if (A[i][p] != 0)
                {
                    max = i;
                    break;
                }

            int[] prow = A[max];
            A[max] = A[p];
            A[p] = prow;

            int[] t = inverse[p];
            inverse[p] = inverse[max];
            inverse[max] = t;

            long ipivot = InverseDirect(A[p][p], mod);
            for (int i = p + 1; i < n; i++)
            {
                int[] row = A[i];
                long alpha = mod - row[p] * ipivot % mod;

                for (int j = 0; j < n; j++)
                    inverse[i][j] = (int)((inverse[i][j] + alpha * inverse[p][j]) % mod);

                for (int j = p; j < n; j++)
                    row[j] = (int)((row[j] + alpha * prow[j]) % mod);
            }
        }

        for (int i = n - 1; i >= 0; i--)
        {
            int[] row = A[i];
            int invrowi = InverseDirect(row[i], mod);
            for (int k = 0; k < n; k++)
            {
                long sum = mod - inverse[i][k];
                for (int j = i + 1; j < n; j++)
                    sum = (sum + (long)row[j] * inverse[j][k]) % mod;
                inverse[i][k] = (int)((mod - sum) * invrowi % mod);
            }
        }

        return inverse;
    }

    public static long[,] LUDecompose(long[,] matrix, long MOD)
    {
        int n = matrix.GetLength(0);
        long[,] lu = new long[n, n];
        long sum;
        for (int i = 0; i < n; i++)
        {
            for (int j = i; j < n; j++)
            {
                sum = 0;
                for (int k = 0; k < i; k++)
                    sum = (sum + lu[i, k] * lu[k, j]) % MOD;
                lu[i, j] = (matrix[i, j] + MOD - sum) % MOD;
            }

            for (int j = i + 1; j < n; j++)
            {
                sum = 0;
                for (int k = 0; k < i; k++)
                    sum = (sum + lu[j, k] * lu[k, i]) % MOD;
                lu[j, i] = Div((matrix[j, i] + MOD - sum) % MOD, lu[i, i], MOD);
            }
        }

        return lu;
    }

    public static long[] LUSolve(long[,] lu, long[] b, long MOD)
    {
        // find solution of Ly = b
        int n = lu.GetLength(0);
        long sum;
        long[] y = new long[n];
        for (int i = 0; i < n; i++)
        {
            sum = 0;
            for (int k = 0; k < i; k++)
                sum = (sum + lu[i, k] * y[k]) % MOD;
            y[i] = (b[i] + MOD - sum) % MOD;
        }

        long[] x = new long[n];
        for (int i = n - 1; i >= 0; i--)
        {
            sum = 0;
            for (int k = i + 1; k < n; k++)
                sum = (sum + lu[i, k] * x[k]) % MOD;
            x[i] = Div((y[i] + MOD - sum) % MOD, lu[i, i], MOD);
        }

        return x;
    }

    public static long[] LuSolveOnce(long[,] matrix, long[] b, long MOD)
    {
        long[,] lu = LUDecompose(matrix, MOD);
        return LUSolve(lu, b, MOD);
    }

    public static long Div(long left, long divisor, long mod) => left * InverseDirect((int)divisor, (int)mod) % mod;

    public static int InverseDirect(int a, int MOD)
    {
        int t = 0, r = MOD, t2 = 1, r2 = a;
        while (r2 != 0)
        {
            int q = r / r2;
            t -= q * t2;
            r -= q * r2;

            if (r != 0)
            {
                q = r2 / r;
                t2 -= q * t;
                r2 -= q * r;
            }
            else
            {
                r = r2;
                t = t2;
                break;
            }
        }

        return r <= 1 ? t >= 0 ? t : t + MOD : -1;
    }

    public static int Determinant(int[][] A, int mod)
    {
        int n = A.Length;

        long det = 1;
        for (int p = 0; p < n; p++)
        {
            int max = p;
            for (int i = p; i < n; i++)
                if (A[i][p] != 0)
                {
                    max = i;
                    break;
                }

            int[] prow = A[max];
            if (p != max)
            {
                A[max] = A[p];
                A[p] = prow;
                det = mod - det;
            }

            int pivot = A[p][p];
            det = det * pivot % mod;

            long ipivot = InverseDirect(pivot, mod);
            for (int i = p + 1; i < n; i++)
            {
                int[] row = A[i];
                long alpha = mod - (int)(row[p] * ipivot % mod);
                for (int j = p; j < n; j++) row[j] = (int)((row[j] + alpha * prow[j]) % mod);
            }
        }

        return (int)(det % mod);
    }
}