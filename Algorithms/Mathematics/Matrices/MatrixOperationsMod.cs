using static System.Math;
using T = long;

namespace Algorithms.Mathematics.Matrices;

//[PublicAPI]
public static class MatrixOperationsMod
{
    #region Construction

    public static T[,] Diagonal(int n, T d = 1)
    {
        T[,] id = new T[n, n];
        for (int i = 0; i < n; i++)
            id[i, i] = d;
        return id;
    }

    #endregion

    #region Scaling

    public static T[,] Scale(T[,] a, T s, T[,] c, T p)
    {
        int arows = a.GetLength(0);
        int bcols = a.GetLength(1);

        if (c == null) c = new T[arows, bcols];
        for (int i = 0; i < arows; i++)
        for (int j = 0; j < bcols; j++)
            c[i, j] = a[i, j] * s % p;

        return c;
    }

    #endregion

    #region Pow

    public static T[,] Pow(this T[,] a, int p, int mod)
    {
        int n = a.GetLength(0);
        T[,] tmp = new T[n, n];
        T[,] result = Diagonal(n);
        T[,] b = (T[,])a.Clone();

        while (p > 0) {
            if ((p & 1) != 0) {
                Mult(result, b, mod, tmp);
                Array.Copy(tmp, 0, result, 0, tmp.Length);
            }

            p >>= 1;
            Mult(b, b, mod, tmp);
            Array.Copy(tmp, 0, b, 0, tmp.Length);
        }

        return result;
    }

    #endregion

    #region Addition and Subtraction

    public static T[,] Add(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);
        if (c == null) c = new T[rows, cols];

        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            c[i, j] = (a[i, j] + b[i, j]) % mod;

        return c;
    }

    public static T[,] Subtract(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);
        if (c == null) c = new T[rows, cols];

        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            c[i, j] = (a[i, j] - b[i, j]) % mod;

        return c;
    }

    #endregion

    #region Multiplication

    public static T[,] Mult(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        int arows = a.GetLength(0);
        int bcols = b.GetLength(1);
        int mid = a.GetLength(1);
        if (c == null) c = new T[arows, bcols];

        for (int i = 0; i < arows; i++)
        for (int j = 0; j < bcols; j++) {
            T t = 0;
            for (int k = 0; k < mid; k++)
                t = (t + 1L * a[i, k] * b[k, j]) % mod;
            c[i, j] = t;
        }

        return c;
    }

    // Slower -- slightly faster for large n
    public static T[,] Multv2(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        int arows = a.GetLength(0);
        int bcols = b.GetLength(1);
        int mid = a.GetLength(1);
        if (c == null) c = new T[arows, bcols];
        else Array.Clear(c, 0, c.Length);

        for (int i = 0; i < arows; i++)
        for (int k = 0; k < mid; k++) {
            T aik = a[i, k];
            for (int j = 0; j < bcols; j++)
                c[i, j] = (c[i, j] + aik * b[k, j]) % mod;
        }

        return c;
    }

    // Cache-friendly multiply for large n -- runtime 88% at n=750 and blocksize=25
    // https://courses.engr.illinois.edu/cs232/sp2009/lectures/X18.pdf
    public static int[,] MultBlock(int[,] a, int[,] b, int mod, int[,] c = null)
    {
        int arows = a.GetLength(0);
        int bcols = b.GetLength(1);
        int mid = a.GetLength(1);

        if (c == null) c = new int[arows, bcols];
        else Array.Clear(c, 0, c.Length);

        const int bsize = 25;
        for (int jj = 0; jj < bcols; jj += bsize)
        for (int kk = 0; kk < mid; kk += bsize)
        for (int i = 0; i < arows; i++) {
            int maxj = Min(jj + bsize, bcols);
            for (int j = jj; j < maxj; j++) {
                long sum = c[i, j];
                int maxk = Min(kk + bsize, mid);
                for (int k = kk; k < maxk; k++)
                    sum = (sum + (long)a[i, k] * b[k, j]) % mod;
                c[i, j] = (int)sum;
            }
        }

        return c;
    }

    #endregion

    #region Sparse Multiplication

    public static T[,] MultSparse(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        int arows = a.GetLength(0);
        int mid = a.GetLength(1);
        int bcols = b.GetLength(1);

        if (c == null) c = new long[arows, bcols];
        else Array.Clear(c, 0, c.Length);

        for (int i = 0; i < arows; i++)
        for (int k = 0; k < mid; k++)
            if (a[i, k] != 0)
                for (int j = 0; j < bcols; j++)
                    c[i, j] = (c[i, j] + a[i, k] * b[k, j]) % mod;
        return c;
    }

    public static T[,] MultSparse2(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        int arows = a.GetLength(0);
        int mid = a.GetLength(1);
        int bcols = b.GetLength(1);

        if (c == null) c = new T[arows, bcols];
        else Array.Clear(c, 0, c.Length);

        for (int j = 0; j < bcols; j++)
        for (int k = 0; k < mid; k++)
            if (b[k, j] != 0)
                for (int i = 0; i < arows; i++)
                    c[i, j] = (c[i, j] + a[i, k] * b[k, j]) % mod;
        return c;
    }

    #endregion

    #region Vector Multiplication

    public static T[] MultVector(T[,] a, T[] b, T mod, T[] c = null)
    {
        int n = a.GetLength(0);
        int m = b.Length;

        if (c == null) c = new T[n];
        for (int i = 0; i < n; i++) {
            T t = 0;
            for (int k = 0; k < m; k++)
                t = (t + 1L * a[i, k] * b[k]) % mod;
            c[i] = t;
        }

        return c;
    }

    public static T[] MultVector(T[] a, T[,] b, T mod, T[] c = null)
    {
        int bcols = b.GetLength(1);
        int mid = a.Length;

        if (c == null) c = new T[bcols];
        for (int j = 0; j < bcols; j++) {
            long t = 0;
            for (int k = 0; k < mid; k++)
                t = (t + 1L * a[k] * b[k, j]) % mod;
            c[j] = t;
        }

        return c;
    }

    public static T[] InnerProduct(T[] v1, T[] v2, T mod, T[] c = null)
    {
        int n = v1.Length;
        if (c == null) c = new T[n];
        for (int i = 0; i < n; i++)
            c[i] = 1L * v1[i] * v2[i] % mod;
        return c;
    }

    public static T[,] OuterProduct(T[] v1, T[] v2, T mod, T[,] m = null)
    {
        int n = v1.Length;
        if (m == null) m = new T[n, n];
        for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            m[i, j] = 1L * v1[i] * v2[j] % mod;
        return m;
    }

    #endregion

    #region Optimized Multiplication

    // mod < 1^30

    public static T[,] Mult2(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        if (c == null) c = new T[2, 2];
        c[0, 0] = (a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0]) % mod;
        c[0, 1] = (a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1]) % mod;
        c[1, 0] = (a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0]) % mod;
        c[1, 1] = (a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1]) % mod;
        return c;
    }

    public static T[] Mult2(T[,] a, T[] b, T mod, T[] c = null)
    {
        if (c == null) c = new T[2];
        c[0] = (a[0, 0] * b[0] + a[0, 1] * b[1]) % mod;
        c[1] = (a[1, 0] * b[0] + a[1, 1] * b[1]) % mod;
        return c;
    }

    public static T[,] Mult3(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        if (c == null) c = new T[3, 3];
        c[0, 0] = (1L * a[0, 0] * b[0, 0] + 1L * a[0, 1] * b[1, 0] + 1L * a[0, 2] * b[2, 0]) % mod;
        c[0, 1] = (1L * a[0, 0] * b[0, 1] + 1L * a[0, 1] * b[1, 1] + 1L * a[0, 2] * b[2, 1]) % mod;
        c[0, 2] = (1L * a[0, 0] * b[0, 2] + 1L * a[0, 1] * b[1, 2] + 1L * a[0, 2] * b[2, 2]) % mod;
        c[1, 0] = (1L * a[1, 0] * b[0, 0] + 1L * a[1, 1] * b[1, 0] + 1L * a[1, 2] * b[2, 0]) % mod;
        c[1, 1] = (1L * a[1, 0] * b[0, 1] + 1L * a[1, 1] * b[1, 1] + 1L * a[1, 2] * b[2, 1]) % mod;
        c[1, 2] = (1L * a[1, 0] * b[0, 2] + 1L * a[1, 1] * b[1, 2] + 1L * a[1, 2] * b[2, 2]) % mod;
        c[2, 0] = (1L * a[2, 0] * b[0, 0] + 1L * a[2, 1] * b[1, 0] + 1L * a[2, 2] * b[2, 0]) % mod;
        c[2, 1] = (1L * a[2, 0] * b[0, 1] + 1L * a[2, 1] * b[1, 1] + 1L * a[2, 2] * b[2, 1]) % mod;
        c[2, 2] = (1L * a[2, 0] * b[0, 2] + 1L * a[2, 1] * b[1, 2] + 1L * a[2, 2] * b[2, 2]) % mod;
        return c;
    }

    public static T[] Mult3(T[,] a, T[] b, T mod, T[] c = null)
    {
        if (c == null) c = new T[3];
        c[0] = (1L * a[0, 0] * b[0] + 1L * a[0, 1] * b[1] + 1L * a[0, 2] * b[2]) % mod;
        c[1] = (1L * a[1, 0] * b[0] + 1L * a[1, 1] * b[1] + 1L * a[1, 2] * b[2]) % mod;
        c[2] = (1L * a[2, 0] * b[0] + 1L * a[2, 1] * b[1] + 1L * a[2, 2] * b[2]) % mod;
        return c;
    }

    public static T[,] Mult4(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        if (c == null) c = new T[4, 4];
        c[0, 0] = (1L * a[0, 0] * b[0, 0] + 1L * a[0, 1] * b[1, 0] + 1L * a[0, 2] * b[2, 0] + 1L * a[0, 3] * b[3, 0]) %
                  mod;
        c[0, 1] = (1L * a[0, 0] * b[0, 1] + 1L * a[0, 1] * b[1, 1] + 1L * a[0, 2] * b[2, 1] + 1L * a[0, 3] * b[3, 1]) %
                  mod;
        c[0, 2] = (1L * a[0, 0] * b[0, 2] + 1L * a[0, 1] * b[1, 2] + 1L * a[0, 2] * b[2, 2] + 1L * a[0, 3] * b[3, 2]) %
                  mod;
        c[0, 3] = (1L * a[0, 0] * b[0, 3] + 1L * a[0, 1] * b[1, 3] + 1L * a[0, 2] * b[2, 3] + 1L * a[0, 3] * b[3, 3]) %
                  mod;
        c[1, 0] = (1L * a[1, 0] * b[0, 0] + 1L * a[1, 1] * b[1, 0] + 1L * a[1, 2] * b[2, 0] + 1L * a[1, 3] * b[3, 0]) %
                  mod;
        c[1, 1] = (1L * a[1, 0] * b[0, 1] + 1L * a[1, 1] * b[1, 1] + 1L * a[1, 2] * b[2, 1] + 1L * a[1, 3] * b[3, 1]) %
                  mod;
        c[1, 2] = (1L * a[1, 0] * b[0, 2] + 1L * a[1, 1] * b[1, 2] + 1L * a[1, 2] * b[2, 2] + 1L * a[1, 3] * b[3, 2]) %
                  mod;
        c[1, 3] = (1L * a[1, 0] * b[0, 3] + 1L * a[1, 1] * b[1, 3] + 1L * a[1, 2] * b[2, 3] + 1L * a[1, 3] * b[3, 3]) %
                  mod;
        c[2, 0] = (1L * a[2, 0] * b[0, 0] + 1L * a[2, 1] * b[1, 0] + 1L * a[2, 2] * b[2, 0] + 1L * a[2, 3] * b[3, 0]) %
                  mod;
        c[2, 1] = (1L * a[2, 0] * b[0, 1] + 1L * a[2, 1] * b[1, 1] + 1L * a[2, 2] * b[2, 1] + 1L * a[2, 3] * b[3, 1]) %
                  mod;
        c[2, 2] = (1L * a[2, 0] * b[0, 2] + 1L * a[2, 1] * b[1, 2] + 1L * a[2, 2] * b[2, 2] + 1L * a[2, 3] * b[3, 2]) %
                  mod;
        c[2, 3] = (1L * a[2, 0] * b[0, 3] + 1L * a[2, 1] * b[1, 3] + 1L * a[2, 2] * b[2, 3] + 1L * a[2, 3] * b[3, 3]) %
                  mod;
        c[3, 0] = (1L * a[3, 0] * b[0, 0] + 1L * a[3, 1] * b[1, 0] + 1L * a[3, 2] * b[2, 0] + 1L * a[3, 3] * b[3, 0]) %
                  mod;
        c[3, 1] = (1L * a[3, 0] * b[0, 1] + 1L * a[3, 1] * b[1, 1] + 1L * a[3, 2] * b[2, 1] + 1L * a[3, 3] * b[3, 1]) %
                  mod;
        c[3, 2] = (1L * a[3, 0] * b[0, 2] + 1L * a[3, 1] * b[1, 2] + 1L * a[3, 2] * b[2, 2] + 1L * a[3, 3] * b[3, 2]) %
                  mod;
        c[3, 3] = (1L * a[3, 0] * b[0, 3] + 1L * a[3, 1] * b[1, 3] + 1L * a[3, 2] * b[2, 3] + 1L * a[3, 3] * b[3, 3]) %
                  mod;
        return c;
    }

    public static T[] Mult4(T[,] a, T[] b, T mod, T[] c = null)
    {
        if (c == null) c = new T[4];
        c[0] = (1L * a[0, 0] * b[0] + 1L * a[0, 1] * b[1] + 1L * a[0, 2] * b[2] + 1L * a[0, 3] * b[3]) % mod;
        c[1] = (1L * a[1, 0] * b[0] + 1L * a[1, 1] * b[1] + 1L * a[1, 2] * b[2] + 1L * a[1, 3] * b[3]) % mod;
        c[2] = (1L * a[2, 0] * b[0] + 1L * a[2, 1] * b[1] + 1L * a[2, 2] * b[2] + 1L * a[2, 3] * b[3]) % mod;
        c[3] = (1L * a[3, 0] * b[0] + 1L * a[3, 1] * b[1] + 1L * a[3, 2] * b[2] + 1L * a[3, 3] * b[3]) % mod;
        return c;
    }

    public static T[,] Mult5(T[,] a, T[,] b, T mod, T[,] c = null)
    {
        if (c == null) c = new T[5, 5];
        c[0, 0] = (1L * a[0, 0] * b[0, 0] + 1L * a[0, 1] * b[1, 0] + 1L * a[0, 2] * b[2, 0] + 1L * a[0, 3] * b[3, 0] +
                   1L * a[0, 4] * b[4, 0]) % mod;
        c[0, 1] = (1L * a[0, 0] * b[0, 1] + 1L * a[0, 1] * b[1, 1] + 1L * a[0, 2] * b[2, 1] + 1L * a[0, 3] * b[3, 1] +
                   1L * a[0, 4] * b[4, 1]) % mod;
        c[0, 2] = (1L * a[0, 0] * b[0, 2] + 1L * a[0, 1] * b[1, 2] + 1L * a[0, 2] * b[2, 2] + 1L * a[0, 3] * b[3, 2] +
                   1L * a[0, 4] * b[4, 2]) % mod;
        c[0, 3] = (1L * a[0, 0] * b[0, 3] + 1L * a[0, 1] * b[1, 3] + 1L * a[0, 2] * b[2, 3] + 1L * a[0, 3] * b[3, 3] +
                   1L * a[0, 4] * b[4, 3]) % mod;
        c[0, 4] = (1L * a[0, 0] * b[0, 4] + 1L * a[0, 1] * b[1, 4] + 1L * a[0, 2] * b[2, 4] + 1L * a[0, 3] * b[3, 4] +
                   1L * a[0, 4] * b[4, 4]) % mod;
        c[1, 0] = (1L * a[1, 0] * b[0, 0] + 1L * a[1, 1] * b[1, 0] + 1L * a[1, 2] * b[2, 0] + 1L * a[1, 3] * b[3, 0] +
                   1L * a[1, 4] * b[4, 0]) % mod;
        c[1, 1] = (1L * a[1, 0] * b[0, 1] + 1L * a[1, 1] * b[1, 1] + 1L * a[1, 2] * b[2, 1] + 1L * a[1, 3] * b[3, 1] +
                   1L * a[1, 4] * b[4, 1]) % mod;
        c[1, 2] = (1L * a[1, 0] * b[0, 2] + 1L * a[1, 1] * b[1, 2] + 1L * a[1, 2] * b[2, 2] + 1L * a[1, 3] * b[3, 2] +
                   1L * a[1, 4] * b[4, 2]) % mod;
        c[1, 3] = (1L * a[1, 0] * b[0, 3] + 1L * a[1, 1] * b[1, 3] + 1L * a[1, 2] * b[2, 3] + 1L * a[1, 3] * b[3, 3] +
                   1L * a[1, 4] * b[4, 3]) % mod;
        c[1, 4] = (1L * a[1, 0] * b[0, 4] + 1L * a[1, 1] * b[1, 4] + 1L * a[1, 2] * b[2, 4] + 1L * a[1, 3] * b[3, 4] +
                   1L * a[1, 4] * b[4, 4]) % mod;
        c[2, 0] = (1L * a[2, 0] * b[0, 0] + 1L * a[2, 1] * b[1, 0] + 1L * a[2, 2] * b[2, 0] + 1L * a[2, 3] * b[3, 0] +
                   1L * a[2, 4] * b[4, 0]) % mod;
        c[2, 1] = (1L * a[2, 0] * b[0, 1] + 1L * a[2, 1] * b[1, 1] + 1L * a[2, 2] * b[2, 1] + 1L * a[2, 3] * b[3, 1] +
                   1L * a[2, 4] * b[4, 1]) % mod;
        c[2, 2] = (1L * a[2, 0] * b[0, 2] + 1L * a[2, 1] * b[1, 2] + 1L * a[2, 2] * b[2, 2] + 1L * a[2, 3] * b[3, 2] +
                   1L * a[2, 4] * b[4, 2]) % mod;
        c[2, 3] = (1L * a[2, 0] * b[0, 3] + 1L * a[2, 1] * b[1, 3] + 1L * a[2, 2] * b[2, 3] + 1L * a[2, 3] * b[3, 3] +
                   1L * a[2, 4] * b[4, 3]) % mod;
        c[2, 4] = (1L * a[2, 0] * b[0, 4] + 1L * a[2, 1] * b[1, 4] + 1L * a[2, 2] * b[2, 4] + 1L * a[2, 3] * b[3, 4] +
                   1L * a[2, 4] * b[4, 4]) % mod;
        c[3, 0] = (1L * a[3, 0] * b[0, 0] + 1L * a[3, 1] * b[1, 0] + 1L * a[3, 2] * b[2, 0] + 1L * a[3, 3] * b[3, 0] +
                   1L * a[3, 4] * b[4, 0]) % mod;
        c[3, 1] = (1L * a[3, 0] * b[0, 1] + 1L * a[3, 1] * b[1, 1] + 1L * a[3, 2] * b[2, 1] + 1L * a[3, 3] * b[3, 1] +
                   1L * a[3, 4] * b[4, 1]) % mod;
        c[3, 2] = (1L * a[3, 0] * b[0, 2] + 1L * a[3, 1] * b[1, 2] + 1L * a[3, 2] * b[2, 2] + 1L * a[3, 3] * b[3, 2] +
                   1L * a[3, 4] * b[4, 2]) % mod;
        c[3, 3] = (1L * a[3, 0] * b[0, 3] + 1L * a[3, 1] * b[1, 3] + 1L * a[3, 2] * b[2, 3] + 1L * a[3, 3] * b[3, 3] +
                   1L * a[3, 4] * b[4, 3]) % mod;
        c[3, 4] = (1L * a[3, 0] * b[0, 4] + 1L * a[3, 1] * b[1, 4] + 1L * a[3, 2] * b[2, 4] + 1L * a[3, 3] * b[3, 4] +
                   1L * a[3, 4] * b[4, 4]) % mod;
        c[4, 0] = (1L * a[4, 0] * b[0, 0] + 1L * a[4, 1] * b[1, 0] + 1L * a[4, 2] * b[2, 0] + 1L * a[4, 3] * b[3, 0] +
                   1L * a[4, 4] * b[4, 0]) % mod;
        c[4, 1] = (1L * a[4, 0] * b[0, 1] + 1L * a[4, 1] * b[1, 1] + 1L * a[4, 2] * b[2, 1] + 1L * a[4, 3] * b[3, 1] +
                   1L * a[4, 4] * b[4, 1]) % mod;
        c[4, 2] = (1L * a[4, 0] * b[0, 2] + 1L * a[4, 1] * b[1, 2] + 1L * a[4, 2] * b[2, 2] + 1L * a[4, 3] * b[3, 2] +
                   1L * a[4, 4] * b[4, 2]) % mod;
        c[4, 3] = (1L * a[4, 0] * b[0, 3] + 1L * a[4, 1] * b[1, 3] + 1L * a[4, 2] * b[2, 3] + 1L * a[4, 3] * b[3, 3] +
                   1L * a[4, 4] * b[4, 3]) % mod;
        c[4, 4] = (1L * a[4, 0] * b[0, 4] + 1L * a[4, 1] * b[1, 4] + 1L * a[4, 2] * b[2, 4] + 1L * a[4, 3] * b[3, 4] +
                   1L * a[4, 4] * b[4, 4]) % mod;
        return c;
    }

    public static T[] Mult5(T[,] a, T[] b, T mod, T[] c = null)
    {
        if (c == null) c = new T[5];
        c[0] = (1L * a[0, 0] * b[0] + 1L * a[0, 1] * b[1] % +1L * a[0, 2] * b[2] + 1L * a[0, 3] * b[3] +
                1L * a[0, 4] * b[4]) % mod;
        c[1] = (1L * a[1, 0] * b[0] + 1L * a[1, 1] * b[1] % +1L * a[1, 2] * b[2] + 1L * a[1, 3] * b[3] +
                1L * a[1, 4] * b[4]) % mod;
        c[2] = (1L * a[2, 0] * b[0] + 1L * a[2, 1] * b[1] % +1L * a[2, 2] * b[2] + 1L * a[2, 3] * b[3] +
                1L * a[2, 4] * b[4]) % mod;
        c[3] = (1L * a[3, 0] * b[0] + 1L * a[3, 1] * b[1] % +1L * a[3, 2] * b[2] + 1L * a[3, 3] * b[3] +
                1L * a[3, 4] * b[4]) % mod;
        c[4] = (1L * a[4, 0] * b[0] + 1L * a[4, 1] * b[1] % +1L * a[4, 2] * b[2] + 1L * a[4, 3] * b[3] +
                1L * a[4, 4] * b[4]) % mod;
        return c;
    }

    #endregion
}