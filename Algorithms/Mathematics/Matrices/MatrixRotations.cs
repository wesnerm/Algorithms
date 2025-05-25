namespace Algorithms.Mathematics.Matrices;

//[PublicAPI]
public static class MatrixRotations
{
    #region Construction

    public static T[,] Diagonal<T>(int n, T d)
    {
        var id = new T[n, n];
        for (int i = 0; i < n; i++)
            id[i, i] = d;
        return id;
    }

    public static T[,] Clone<T>(T[,] m) => (T[,])m.Clone();

    public static void Assign<T>(T[,] dest, T[,] src)
    {
        Array.Copy(src, dest, src.Length);
    }

    public static T[][] Transpose<T>(T[][] a, T[][] result = null)
    {
        int n = a.Length;
        int m = a[0].Length;
        if (result == null) result = new T[m][];
        for (int j = 0; j < m; j++) {
            if (result[j] == null) result[j] = new T[n];
            for (int i = 0; i < n; i++)
                result[j][i] = a[i][j];
        }

        return result;
    }

    public static T[][] TransposeInplace<T>(T[][] a)
    {
        int n = a.Length;
        for (int i = 0; i < n; i++)
        for (int j = i + 1; j < n; j++) {
            T tmp = a[i][j];
            a[i][j] = a[j][i];
            a[j][i] = tmp;
        }

        return a;
    }

    public static T[,] Transpose<T>(T[,] a, T[,] result = null)
    {
        int n = a.GetLength(0);
        int m = a.GetLength(1);
        if (result == null) result = new T[m, n];
        for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++)
            result[j, i] = a[i, j];
        return result;
    }

    public static T[,] TransposeInplace<T>(T[,] a)
    {
        int n = a.GetLength(0);
        int m = a.GetLength(1);
        for (int i = 0; i < n; i++)
        for (int j = i + 1; j < m; j++) {
            T tmp = a[i, j];
            a[i, j] = a[j, i];
            a[j, i] = tmp;
        }

        return a;
    }

    public static void SwapRows<T>(T[,] a, int r1, int r2)
    {
        int m = a.GetLength(1);
        for (int i = 0; i < m; i++) {
            T tmp = a[r1, i];
            a[r1, i] = a[r2, i];
            a[r2, i] = tmp;
        }
    }

    public static T[,] Rotate<T>(T[,] a, T[,] result = null)
    {
        int n = a.GetLength(0);
        int m = a.GetLength(1);
        if (result == null) result = new T[n, m];
        for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++) {
            int i2 = n - 1 - i;
            int j2 = m - 1 - j;
            T tmp = a[i, j];
            result[i, j] = a[j, i2];
            result[j, i2] = a[i2, j2];
            result[i2, j2] = a[j2, i];
            result[j2, i] = tmp;
        }

        return result;

        // var t = Transpose(a, result);
        // return FlipRows(t, t);
    }

    public static T[,] RotateCounterClockwise<T>(T[,] a, T[,] result = null)
    {
        int n = a.GetLength(0);
        int m = a.GetLength(1);
        if (result == null) result = new T[n, m];
        for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++) {
            int i2 = n - 1 - i;
            int j2 = m - 1 - j;
            T tmp = a[i, j];
            result[i, j] = a[j2, i];
            result[j2, i] = a[i2, j2];
            result[i2, j2] = a[j, i2];
            result[j, i2] = tmp;
        }

        return result;

        // var t = FlipRows(a, result);
        // return Transpose(t, result);
    }

    public static T[,] FlipRows<T>(T[,] a, T[,] result = null)
    {
        int n = a.GetLength(0);
        int m = a.GetLength(1);
        if (result == null) result = new T[m, n];
        int start = 0;
        int end = n - 1;
        while (start < end) {
            for (int j = 0; j < m; j++) {
                T tmp = a[start, j];
                result[start, j] = a[end, j];
                result[end, j] = tmp;
            }

            start++;
            end--;
        }

        return result;
    }

    public static T[,] FlipColumns<T>(T[,] a, T[,] result = null)
    {
        int n = a.GetLength(0);
        int m = a.GetLength(1);
        if (result == null) result = new T[m, n];
        int start = 0;
        int end = m - 1;
        while (start < end) {
            for (int i = 0; i < n; i++) {
                T tmp = a[i, start];
                result[i, start] = a[i, end];
                result[i, end] = tmp;
            }

            start++;
            end--;
        }

        return result;
    }

    public static T[,] Flip<T>(T[,] a, T[,] result = null)
    {
        int n = a.GetLength(0);
        int m = a.GetLength(1);
        if (result == null) result = new T[n, m];
        for (int i = 0; i < n; i++)
        for (int j = i; j < m; j++) {
            // Supports inplace transpose
            T tmp = a[i, j];
            result[i, j] = a[n - i - 1, m - j - 1];
            result[n - i - 1, m - j - 1] = tmp;
        }

        return result;
    }

    #endregion

    #region Misc

    public static string Text<T>(T[,] mat)
    {
        var sb = new StringBuilder();
        if (mat == null)
            return null;

        int n = mat.GetLength(0);
        int m = mat.GetLength(1);

        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++) {
                if (j > 0) sb.Append(' ');
                sb.Append(mat[i, j]);
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static string Text<T>(T[] m) => "[" + string.Join(", ", m) + "]";

    public static T[,] Reinterpret<T>(Array mat, int n, int m)
    {
        // if (n * m != mat.Length) throw new InvalidOperationException();

        var result = new T[n, m];
        Array.Copy(mat, 0, result, 0, mat.Length);

        //fixed (T* psrc = &mat[0, 0])
        //fixed (T* pdest = &result[0, 0])
        //{
        //    for (int i = 0; i < result.Length; i++)
        //        pdest[i] = psrc[i];
        //}
        return result;
    }

    public static T[] Flatten<T>(Array mat)
    {
        var result = new T[mat.Length];
        Array.Copy(mat, 0, result, 0, mat.Length);
        //fixed (T* pstart = &mat[0, 0])
        //{
        //    for (int i = 0; i < mat.Length; i++)
        //        result[i] = pstart[i];
        //}
        return result;
    }

    public static T[,] Submatrix<T>(T[,] mat, int x, int y, int n, int m)
    {
        var result = new T[n, m];
        for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++)
            result[i, j] = mat[i + x, j + y];
        return result;
    }

    #endregion
}