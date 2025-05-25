using Algorithms.Mathematics.Equations;
using static Algorithms.Mathematics.ModularMath;
using static Algorithms.Mathematics.Matrices.MatrixOperationsMod;
using static Algorithms.Mathematics.Matrices.MatrixRotations;

namespace Algorithms.Mathematics.Numerics;

public static class InterpolationMod
{
    /// <summary>
    ///     Lagrange interpolation (linear-time)
    /// </summary>
    /// <param name="y">
    ///     Must be a list of points for x in [0, deg],
    ///     where degree is the expected degree of the interpolating polynomial
    /// </param>
    /// <param name="x">Target x</param>
    /// <returns></returns>
    public static long Lagrange(long[] y, long x, long MOD)
    {
        if (x < y.Length && x >= 0)
            return y[(int)x];

        long answer = 0;
        long coef = 1;
        for (int j = 1; j < y.Length; j++)
            coef = Div(coef * (j - x) % MOD, j);

        for (int i = 0; i < y.Length; i++) {
            answer = (answer + y[i] * coef) % MOD;
            if (i + 1 >= y.Length) break;
            coef = Div(coef * (x - i) % MOD * (i - y.Length + 1) % MOD,
                (x - (i + 1)) * (i + 1) % MOD);
        }

        return Fix(answer);
    }

    /// <summary>
    ///     O(n^2) Interpolation which returns a polynomial
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="mod"></param>
    /// <returns></returns>
    public static long[] Interpolate(long[] x, long[] y, int mod)
    {
        int n = x.Length;
        long[] dp = new long[n + 1];
        dp[0] = 1;
        for (int i = 0; i < n; ++i)
        for (int j = i; j >= 0; --j) {
            dp[j + 1] = (dp[j + 1] + dp[j]) % mod;
            dp[j] = dp[j] * (mod - x[i]) % mod;
        }

        long[] r = new long[n];
        for (int i = 0; i < n; ++i) {
            long den = 1, res = 0;
            for (int j = 0; j < n; ++j)
                if (i != j)
                    den = den * ((x[i] - x[j]) % mod) % mod;
            den = Inverse(den);

            for (int j = n - 1; j >= 0; --j) {
                res = (dp[j + 1] + res * x[i] % mod) % mod;
                r[j] = (r[j] + res * (den * y[i] % mod) % mod) % mod;
            }
        }

        return r;
    }

    #region System of Power Equations

    // http://www4.ncsu.edu/~kaltofen/bibliography/88/KaLa88.pdf
    // https://discuss.codechef.com/questions/140093/nov18-problem-discussion

    public static long[] SystemOfPowerEquations(long[] xs, long[] ys, int mod)
    {
        // Transpose Vandermonde matrix
        long[,] invL = Transpose(InverseL(xs));
        long[,] invU = Transpose(InverseU(xs));
        return MultVector(invL, MultVector(invU, ys, mod), mod);
    }

    #endregion

    #region Vandermonde LU Matrix

    public static int[][] VandermondeMatrix(int[] xs, int mod)
    {
        int n = xs.Length;
        int[][] mat = new int[n][];

        mat[0] = new int[n];
        for (int j = 0; j < n; j++)
            mat[0][j] = 1;

        for (int i = 1; i < n; i++) {
            int[] pre = mat[i - 1];
            int[] row = mat[i] = new int[n];
            for (int j = 0; j < n; j++)
                row[j] = (int)((long)pre[j] * xs[j] % mod);
        }

        return mat;
    }

    public static int[] InterpolateSlow(int[] xs, int[] ys, int mod)
    {
        int[][] mat = VandermondeMatrix(xs, mod);
        return EquationSolvingMod.GaussianElimination(mat, ys, mod);
    }

    public static long[,] InverseL(long[] A)
    {
        int n = A.Length;
        long[,] mat = new long[n, n];

        mat[0, 0] = 1;
        for (int i = 1; i < n; i++) {
            long tmp = 1;

            for (int j = 0; j < i; j++) {
                long inv = ModInverse(A[j] - A[i], MOD);
                mat[i, j] = (int)(inv * mat[i - 1, j] % MOD);
                tmp = Mult(tmp, A[i] - A[j]);
            }

            mat[i, i] = Inverse(tmp);
        }

        return mat;
    }

    public static long[,] InverseU(long[] A)
    {
        int n = A.Length;
        long[,] mat = new long[n, n];

        for (int i = 0; i < n; i++) {
            mat[i, i] = 1;
            for (int j = i + 1; j < n; j++)
                if (i != 0)
                    mat[i, j] = (mat[i - 1, j - 1] - mat[i, j - 1] * A[j - 1] % MOD) % MOD;
                else
                    mat[i, j] = -mat[i, j - 1] * A[j - 1] % MOD;
        }

        return mat;
    }

    public static long[] Interpolate2(long[] xs, long[] ys, int mod)
    {
        long[,] invL = InverseL(xs);
        long[,] invU = InverseU(ys);

        // TODO: Make sure order is right
        return MultVector(invL, MultVector(invU, xs, mod), mod);
    }

    #endregion
}