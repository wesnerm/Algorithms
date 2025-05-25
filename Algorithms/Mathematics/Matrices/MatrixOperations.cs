using System.CodeDom.Compiler;
using Algorithms.Mathematics.Equations;
using static System.Math;
using T = double;

// MORE RESOURCES: https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/Matrix3x3.cs
//                SharpDS: Mit License

namespace Algorithms.Mathematics.Matrices;

public static class MatrixOperations
{
    const double Epsilon = 1e-9;

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

    public static T[,] Scale(T[,] a, T s, T[,] c)
    {
        int arows = a.GetLength(0);
        int bcols = a.GetLength(1);

        if (c == null) c = new T[arows, bcols];
        for (int i = 0; i < arows; i++)
        for (int j = 0; j < bcols; j++)
            c[i, j] = a[i, j] * s;

        return c;
    }

    #endregion

    #region Pow

    public static T[,] Pow(this T[,] a, int p)
    {
        int n = a.GetLength(0);
        T[,] tmp = new T[n, n];
        T[,] result = Diagonal(n);
        T[,] b = (T[,])a.Clone();

        while (p > 0) {
            if ((p & 1) != 0) {
                Mult(result, b, tmp);
                Array.Copy(tmp, 0, result, 0, tmp.Length);
            }

            p >>= 1;
            Mult(b, b, tmp);
            Array.Copy(tmp, 0, b, 0, tmp.Length);
        }

        return result;
    }

    #endregion

    #region Addition and Subtraction

    public static T[,] Add(T[,] a, T[,] b, T[,] c = null)
    {
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);
        if (c == null) c = new T[rows, cols];

        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            c[i, j] = a[i, j] + b[i, j];

        return c;
    }

    public static T[,] Subtract(T[,] a, T[,] b, T[,] c = null)
    {
        int rows = a.GetLength(0);
        int cols = a.GetLength(1);
        if (c == null) c = new T[rows, cols];

        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            c[i, j] = a[i, j] - b[i, j];

        return c;
    }

    #endregion

    #region Multiplication

    public static T[,] Mult(T[,] a, T[,] b, T[,] c = null)
    {
        int arows = a.GetLength(0);
        int bcols = b.GetLength(1);
        if (c == null) c = new T[arows, bcols];

        int mid = a.GetLength(1);
        for (int i = 0; i < arows; i++)
        for (int j = 0; j < bcols; j++) {
            c[i, j] = 0;
            for (int k = 0; k < mid; k++)
                c[i, j] = c[i, j] + a[i, k] * b[k, j];
        }

        return c;
    }

    // Slower -- slightly faster for large n
    public static T[,] Multv2(T[,] a, T[,] b, T[,] c = null)
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
                c[i, j] = c[i, j] + 1L * aik * b[k, j];
        }

        return c;
    }

    // Cache-friendly multiply for large n -- runtime 88% at n=750 and blocksize=25
    // https://courses.engr.illinois.edu/cs232/sp2009/lectures/X18.pdf
    public static int[,] MultBlock(int[,] a, int[,] b, int[,] c = null)
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
                T sum = c[i, j];
                int maxk = Min(kk + bsize, mid);
                for (int k = kk; k < maxk; k++)
                    sum += a[i, k] * b[k, j];
                c[i, j] = (int)sum;
            }
        }

        return c;
    }

    #endregion

    #region Sparse Multiplication

    public static T[,] MultSparse(T[,] a, T[,] b, T[,] result = null)
    {
        int m = a.GetLength(0);
        int n = a.GetLength(1);
        int p = b.GetLength(1);

        if (result == null) result = new T[m, p];
        else Array.Clear(result, 0, result.Length);

        for (int i = 0; i < m; i++)
        for (int k = 0; k < n; k++)
            if (a[i, k] != 0)
                for (int j = 0; j < p; j++)
                    result[i, j] += a[i, k] * b[k, j];

        return result;
    }

    public static T[,] MultSparse2(T[,] a, T[,] b, T[,] c = null)
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
                    c[i, j] = c[i, j] + a[i, k] * b[k, j];
        return c;
    }

    #endregion

    #region Vector Multiplication

    public static T[] MultVector(T[,] a, T[] b, T[] c = null)
    {
        int n = a.GetLength(0);
        int m = b.Length;

        if (c == null) c = new T[n];
        for (int i = 0; i < n; i++) {
            T t = 0;
            for (int k = 0; k < m; k++)
                t += a[i, k] * b[k];
            c[i] = t;
        }

        return c;
    }

    public static T[] MultVector(T[] a, T[,] b, T[] c = null)
    {
        int bcols = b.GetLength(1);
        int mid = a.Length;

        if (c == null) c = new T[bcols];
        for (int j = 0; j < bcols; j++) {
            T t = 0;
            for (int k = 0; k < mid; k++)
                t = t + 1L * a[k] * b[k, j];
            c[j] = t;
        }

        return c;
    }

    public static T[] InnerProduct(T[] v1, T[] v2, T[] c = null)
    {
        int n = v1.Length;
        if (c == null) c = new T[n];
        for (int i = 0; i < n; i++)
            c[i] = 1L * v1[i] * v2[i];
        return c;
    }

    public static T[,] OuterProduct(T[] v1, T[] v2, T[,] m = null)
    {
        int n = v1.Length;
        if (m == null) m = new T[n, n];
        for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            m[i, j] = v1[i] * v2[j];
        return m;
    }

    #endregion

    #region Optimized Multiplication

    public static void MultX(int n, string mod = null, bool useWhile = false)
    {
        var writer = new IndentedTextWriter(Console.Error);
        string modParam = mod == null ? "" : $" T {mod},";
        writer.WriteLine($"public static T[,] Mult{n}(T[,] a, T[,] b,{modParam} T[,] c = null)");
        writer.WriteLine("{");
        writer.Indent++;
        writer.WriteLine($"if (c==null) c = new T[{n},{n}];");
        string suf = mod != null ? "%mod" : "";
        for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++) {
            string cij = $"c[{i},{j}]";
            writer.Write(cij);
            for (int k = 0; k < n; k++) {
                char c = k == 0 ? '=' : '+';
                writer.Write($" {c} a[{i},{k}]*b[{k},{j}]{suf}");
            }

            writer.WriteLine(';');
            if (mod != null) {
                if (useWhile)
                    writer.WriteLine($"while ({cij}>=mod) {cij} -= mod;");
                else
                    writer.WriteLine($"{cij} %= {mod};");
            }
        }

        writer.WriteLine("return c;");
        writer.Indent--;
        writer.WriteLine("}\n");

        writer.WriteLine($"public static T[] Mult{n}(T[,] a, T[] b,{modParam} T[] c = null)");
        writer.WriteLine("{");
        writer.Indent++;
        writer.WriteLine($"if (c==null) c = new T[{n}];");
        for (int i = 0; i < n; i++) {
            string cij = $"c[{i}]";
            writer.Write(cij);
            for (int k = 0; k < n; k++) {
                char c = k == 0 ? '=' : '+';
                writer.Write($" {c} a[{i},{k}]*b[{k}]{suf}");
            }

            writer.WriteLine(';');
            if (mod != null) {
                if (useWhile)
                    writer.WriteLine($"while ({cij}>=mod) {cij} -= mod;");
                else
                    writer.WriteLine($"{cij} %= {mod};");
            }
        }

        writer.WriteLine("return c;");
        writer.Indent--;
        writer.WriteLine("}\n");
        writer.Flush();
    }

    public static T[,] Mult2(T[,] a, T[,] b, T[,] c = null)
    {
        if (c == null) c = new T[2, 2];
        c[0, 0] = a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0];
        c[0, 1] = a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1];
        c[1, 0] = a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0];
        c[1, 1] = a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1];
        return c;
    }

    public static T[] Mult2(T[,] a, T[] b, T[] c = null)
    {
        if (c == null) c = new T[2];
        c[0] = a[0, 0] * b[0] + a[0, 1] * b[1];
        c[1] = a[1, 0] * b[0] + a[1, 1] * b[1];
        return c;
    }

    public static T[,] Mult3(T[,] a, T[,] b, T[,] c = null)
    {
        if (c == null) c = new T[3, 3];
        c[0, 0] = a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0];
        c[0, 1] = a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1] + a[0, 2] * b[2, 1];
        c[0, 2] = a[0, 0] * b[0, 2] + a[0, 1] * b[1, 2] + a[0, 2] * b[2, 2];
        c[1, 0] = a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0] + a[1, 2] * b[2, 0];
        c[1, 1] = a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1] + a[1, 2] * b[2, 1];
        c[1, 2] = a[1, 0] * b[0, 2] + a[1, 1] * b[1, 2] + a[1, 2] * b[2, 2];
        c[2, 0] = a[2, 0] * b[0, 0] + a[2, 1] * b[1, 0] + a[2, 2] * b[2, 0];
        c[2, 1] = a[2, 0] * b[0, 1] + a[2, 1] * b[1, 1] + a[2, 2] * b[2, 1];
        c[2, 2] = a[2, 0] * b[0, 2] + a[2, 1] * b[1, 2] + a[2, 2] * b[2, 2];
        return c;
    }

    public static T[] Mult3(T[,] a, T[] b, T[] c = null)
    {
        if (c == null) c = new T[3];
        c[0] = a[0, 0] * b[0] + a[0, 1] * b[1] + a[0, 2] * b[2];
        c[1] = a[1, 0] * b[0] + a[1, 1] * b[1] + a[1, 2] * b[2];
        c[2] = a[2, 0] * b[0] + a[2, 1] * b[1] + a[2, 2] * b[2];
        return c;
    }

    public static T[,] Mult4(T[,] a, T[,] b, T[,] c = null)
    {
        if (c == null) c = new T[4, 4];
        c[0, 0] = a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0] + a[0, 3] * b[3, 0];
        c[0, 1] = a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1] + a[0, 2] * b[2, 1] + a[0, 3] * b[3, 1];
        c[0, 2] = a[0, 0] * b[0, 2] + a[0, 1] * b[1, 2] + a[0, 2] * b[2, 2] + a[0, 3] * b[3, 2];
        c[0, 3] = a[0, 0] * b[0, 3] + a[0, 1] * b[1, 3] + a[0, 2] * b[2, 3] + a[0, 3] * b[3, 3];
        c[1, 0] = a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0] + a[1, 2] * b[2, 0] + a[1, 3] * b[3, 0];
        c[1, 1] = a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1] + a[1, 2] * b[2, 1] + a[1, 3] * b[3, 1];
        c[1, 2] = a[1, 0] * b[0, 2] + a[1, 1] * b[1, 2] + a[1, 2] * b[2, 2] + a[1, 3] * b[3, 2];
        c[1, 3] = a[1, 0] * b[0, 3] + a[1, 1] * b[1, 3] + a[1, 2] * b[2, 3] + a[1, 3] * b[3, 3];
        c[2, 0] = a[2, 0] * b[0, 0] + a[2, 1] * b[1, 0] + a[2, 2] * b[2, 0] + a[2, 3] * b[3, 0];
        c[2, 1] = a[2, 0] * b[0, 1] + a[2, 1] * b[1, 1] + a[2, 2] * b[2, 1] + a[2, 3] * b[3, 1];
        c[2, 2] = a[2, 0] * b[0, 2] + a[2, 1] * b[1, 2] + a[2, 2] * b[2, 2] + a[2, 3] * b[3, 2];
        c[2, 3] = a[2, 0] * b[0, 3] + a[2, 1] * b[1, 3] + a[2, 2] * b[2, 3] + a[2, 3] * b[3, 3];
        c[3, 0] = a[3, 0] * b[0, 0] + a[3, 1] * b[1, 0] + a[3, 2] * b[2, 0] + a[3, 3] * b[3, 0];
        c[3, 1] = a[3, 0] * b[0, 1] + a[3, 1] * b[1, 1] + a[3, 2] * b[2, 1] + a[3, 3] * b[3, 1];
        c[3, 2] = a[3, 0] * b[0, 2] + a[3, 1] * b[1, 2] + a[3, 2] * b[2, 2] + a[3, 3] * b[3, 2];
        c[3, 3] = a[3, 0] * b[0, 3] + a[3, 1] * b[1, 3] + a[3, 2] * b[2, 3] + a[3, 3] * b[3, 3];
        return c;
    }

    public static T[] Mult4(T[,] a, T[] b, T[] c = null)
    {
        if (c == null) c = new T[4];
        c[0] = a[0, 0] * b[0] + a[0, 1] * b[1] + a[0, 2] * b[2] + a[0, 3] * b[3];
        c[1] = a[1, 0] * b[0] + a[1, 1] * b[1] + a[1, 2] * b[2] + a[1, 3] * b[3];
        c[2] = a[2, 0] * b[0] + a[2, 1] * b[1] + a[2, 2] * b[2] + a[2, 3] * b[3];
        c[3] = a[3, 0] * b[0] + a[3, 1] * b[1] + a[3, 2] * b[2] + a[3, 3] * b[3];
        return c;
    }

    public static T[,] Mult5(T[,] a, T[,] b, T[,] c = null)
    {
        if (c == null) c = new T[5, 5];
        c[0, 0] = a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0] + a[0, 3] * b[3, 0] + a[0, 4] * b[4, 0];
        c[0, 1] = a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1] + a[0, 2] * b[2, 1] + a[0, 3] * b[3, 1] + a[0, 4] * b[4, 1];
        c[0, 2] = a[0, 0] * b[0, 2] + a[0, 1] * b[1, 2] + a[0, 2] * b[2, 2] + a[0, 3] * b[3, 2] + a[0, 4] * b[4, 2];
        c[0, 3] = a[0, 0] * b[0, 3] + a[0, 1] * b[1, 3] + a[0, 2] * b[2, 3] + a[0, 3] * b[3, 3] + a[0, 4] * b[4, 3];
        c[0, 4] = a[0, 0] * b[0, 4] + a[0, 1] * b[1, 4] + a[0, 2] * b[2, 4] + a[0, 3] * b[3, 4] + a[0, 4] * b[4, 4];
        c[1, 0] = a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0] + a[1, 2] * b[2, 0] + a[1, 3] * b[3, 0] + a[1, 4] * b[4, 0];
        c[1, 1] = a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1] + a[1, 2] * b[2, 1] + a[1, 3] * b[3, 1] + a[1, 4] * b[4, 1];
        c[1, 2] = a[1, 0] * b[0, 2] + a[1, 1] * b[1, 2] + a[1, 2] * b[2, 2] + a[1, 3] * b[3, 2] + a[1, 4] * b[4, 2];
        c[1, 3] = a[1, 0] * b[0, 3] + a[1, 1] * b[1, 3] + a[1, 2] * b[2, 3] + a[1, 3] * b[3, 3] + a[1, 4] * b[4, 3];
        c[1, 4] = a[1, 0] * b[0, 4] + a[1, 1] * b[1, 4] + a[1, 2] * b[2, 4] + a[1, 3] * b[3, 4] + a[1, 4] * b[4, 4];
        c[2, 0] = a[2, 0] * b[0, 0] + a[2, 1] * b[1, 0] + a[2, 2] * b[2, 0] + a[2, 3] * b[3, 0] + a[2, 4] * b[4, 0];
        c[2, 1] = a[2, 0] * b[0, 1] + a[2, 1] * b[1, 1] + a[2, 2] * b[2, 1] + a[2, 3] * b[3, 1] + a[2, 4] * b[4, 1];
        c[2, 2] = a[2, 0] * b[0, 2] + a[2, 1] * b[1, 2] + a[2, 2] * b[2, 2] + a[2, 3] * b[3, 2] + a[2, 4] * b[4, 2];
        c[2, 3] = a[2, 0] * b[0, 3] + a[2, 1] * b[1, 3] + a[2, 2] * b[2, 3] + a[2, 3] * b[3, 3] + a[2, 4] * b[4, 3];
        c[2, 4] = a[2, 0] * b[0, 4] + a[2, 1] * b[1, 4] + a[2, 2] * b[2, 4] + a[2, 3] * b[3, 4] + a[2, 4] * b[4, 4];
        c[3, 0] = a[3, 0] * b[0, 0] + a[3, 1] * b[1, 0] + a[3, 2] * b[2, 0] + a[3, 3] * b[3, 0] + a[3, 4] * b[4, 0];
        c[3, 1] = a[3, 0] * b[0, 1] + a[3, 1] * b[1, 1] + a[3, 2] * b[2, 1] + a[3, 3] * b[3, 1] + a[3, 4] * b[4, 1];
        c[3, 2] = a[3, 0] * b[0, 2] + a[3, 1] * b[1, 2] + a[3, 2] * b[2, 2] + a[3, 3] * b[3, 2] + a[3, 4] * b[4, 2];
        c[3, 3] = a[3, 0] * b[0, 3] + a[3, 1] * b[1, 3] + a[3, 2] * b[2, 3] + a[3, 3] * b[3, 3] + a[3, 4] * b[4, 3];
        c[3, 4] = a[3, 0] * b[0, 4] + a[3, 1] * b[1, 4] + a[3, 2] * b[2, 4] + a[3, 3] * b[3, 4] + a[3, 4] * b[4, 4];
        c[4, 0] = a[4, 0] * b[0, 0] + a[4, 1] * b[1, 0] + a[4, 2] * b[2, 0] + a[4, 3] * b[3, 0] + a[4, 4] * b[4, 0];
        c[4, 1] = a[4, 0] * b[0, 1] + a[4, 1] * b[1, 1] + a[4, 2] * b[2, 1] + a[4, 3] * b[3, 1] + a[4, 4] * b[4, 1];
        c[4, 2] = a[4, 0] * b[0, 2] + a[4, 1] * b[1, 2] + a[4, 2] * b[2, 2] + a[4, 3] * b[3, 2] + a[4, 4] * b[4, 2];
        c[4, 3] = a[4, 0] * b[0, 3] + a[4, 1] * b[1, 3] + a[4, 2] * b[2, 3] + a[4, 3] * b[3, 3] + a[4, 4] * b[4, 3];
        c[4, 4] = a[4, 0] * b[0, 4] + a[4, 1] * b[1, 4] + a[4, 2] * b[2, 4] + a[4, 3] * b[3, 4] + a[4, 4] * b[4, 4];
        return c;
    }

    public static T[] Mult5(T[,] a, T[] b, T[] c = null)
    {
        if (c == null) c = new T[5];
        c[0] = a[0, 0] * b[0] + a[0, 1] * b[1] + a[0, 2] * b[2] + a[0, 3] * b[3] + a[0, 4] * b[4];
        c[1] = a[1, 0] * b[0] + a[1, 1] * b[1] + a[1, 2] * b[2] + a[1, 3] * b[3] + a[1, 4] * b[4];
        c[2] = a[2, 0] * b[0] + a[2, 1] * b[1] + a[2, 2] * b[2] + a[2, 3] * b[3] + a[2, 4] * b[4];
        c[3] = a[3, 0] * b[0] + a[3, 1] * b[1] + a[3, 2] * b[2] + a[3, 3] * b[3] + a[3, 4] * b[4];
        c[4] = a[4, 0] * b[0] + a[4, 1] * b[1] + a[4, 2] * b[2] + a[4, 3] * b[3] + a[4, 4] * b[4];
        return c;
    }

    #endregion

    #region Inverses

    public static T Determinant2x2(T[,] m) => m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];

    // SOURCE: https://stackoverflow.com/questions/983999/simple-3x3-matrix-inverse-code-c

    public static T Determinant3x3(T[,] m) =>
        m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) -
        m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
        m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);

    public static T[,] Inverse3x3(T[,] m)
    {
        double invdet = 1 / Determinant3x3(m);
        return new[,]
        {
            {
                (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) * invdet,
                (m[0, 2] * m[2, 1] - m[0, 1] * m[2, 2]) * invdet,
                (m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1]) * invdet,
            },
            {
                (m[1, 2] * m[2, 0] - m[1, 0] * m[2, 2]) * invdet,
                (m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0]) * invdet,
                (m[1, 0] * m[0, 2] - m[0, 0] * m[1, 2]) * invdet,
            },
            {
                (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]) * invdet,
                (m[2, 0] * m[0, 1] - m[0, 0] * m[2, 1]) * invdet,
                (m[0, 0] * m[1, 1] - m[1, 0] * m[0, 1]) * invdet,
            },
        };
    }

    #endregion

    #region Vector Arithmetic

    public static T L1Norm(T[] a)
    {
        T result = 0;
        foreach (T v in a)
            result += Abs(v);
        return result;
    }

    public static double L2Norm(T[] a)
    {
        T result = 0;
        foreach (T v in a)
            result += v * v;
        return Sqrt(result);
    }

    public static T DotProduct(T[] a, T[] b)
    {
        T result = 0;
        for (int i = 0; i < a.Length; i++)
            result += a[i] * b[i];
        return result;
    }

    public static void NormalizeX(T[] v)
    {
        double d = L2Norm(v);
        if (d != 0 && d != 1)
            for (int i = 0; i < v.Length; i++)
                v[i] = v[i] / d;
    }

    public static T[] Blend(T[] a, T[] b, T fa, T fb, T[] c = null)
    {
        int n = a.Length;
        if (c == null) c = new T[n];
        for (int i = 0; i < n; i++)
            c[i] = fa * a[i] + fb * b[i];
        return c;
    }

    public static T[] CrossProduct(T[] a, T[] b, T[] c = null)
    {
        if (c == null) c = new T[3];
        c[0] = a[1] * b[2] - a[2] * b[1];
        c[1] = a[2] * b[0] - a[0] * b[2];
        c[2] = a[0] * b[1] - a[1] * b[0];
        return c;
    }

    /// <summary>
    ///     Vector perp -- assumes that n is of unit length
    ///     accepts vector v, subtracts out any component parallel to n
    /// </summary>
    /// <param name="vp"></param>
    /// <param name="v"></param>
    /// <param name="n"></param>
    public static void Perpendicular(T[] vp, T[] v, T[] n)
    {
        T vdot = DotProduct(v, n);
        vp[0] = v[0] - vdot * n[0];
        vp[1] = v[1] - vdot * n[1];
        vp[2] = v[2] - vdot * n[2];
    }

    /// <summary>
    ///     Vector parallel -- assumes that n is of unit length
    ///     accepts vector v, subtracts out any component perpendicular to n*/
    /// </summary>
    public static void Parallel(T[] vp, T[] v, T[] n)
    {
        T vdot = DotProduct(v, n);
        vp[0] = vdot * n[0];
        vp[1] = vdot * n[1];
        vp[2] = vdot * n[2];
    }

    public static void Reflect(T[] vr, T[] v, T[] n)
    {
        T vdot = DotProduct(v, n);
        vr[0] = v[0] - 2 * vdot * n[0];
        vr[1] = v[1] - 2 * vdot * n[1];
        vr[2] = v[2] - 2 * vdot * n[2];
    }

    #endregion

    #region Misc

    public static bool Equals(T[,] a, T[,] b)
    {
        if (a.GetLength(0) != b.GetLength(0)
            || a.GetLength(1) != b.GetLength(1))
            return false;

        int n = a.GetLength(0);
        int m = a.GetLength(1);
        for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++)
            if (a[i, j] != b[i, j])
                return false;
        return true;
    }

    public static bool AreClose(T[,] a, T[,] b)
    {
        if (a.GetLength(0) != b.GetLength(0)
            || a.GetLength(1) != b.GetLength(1))
            return false;

        int n = a.GetLength(0);
        int m = a.GetLength(1);
        for (int i = 0; i < n; i++)
        for (int j = 0; j < m; j++)
            if (!AreClose(a[i, j], b[i, j]))
                return false;
        return true;
    }

    public static bool Equals(T[] a, T[] b)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i])
                return false;
        return true;
    }

    public static bool AreClose(T[] a, T[] b)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
            if (!AreClose(a[i], b[i]))
                return false;
        return true;
    }

    public static bool AreClose(T a, T b) => Abs(a - b) < Epsilon;

    public static unsafe int GetHashCode(T[,] m)
    {
        unchecked {
            int hashCode = 0;
            fixed (T* p = &m[0, 0]) {
                for (int i = 0; i < m.Length; i++)
                    hashCode = (hashCode * 397) ^ p[i].GetHashCode();
            }

            return hashCode;
        }
    }

    public static int GetHashCode(int[] m)
    {
        unchecked {
            int hashCode = 0;
            for (int i = 0; i < m.Length; i++)
                hashCode = (hashCode * 397) ^ m[i].GetHashCode();
            return hashCode;
        }
    }

    /// <summary>
    ///     Produces a right matrix for transforming row vectors (points)
    ///     using the same affine transformation that tranforms src points to dest points
    ///     For n dimensions, n+1 points required or (use cross product to get n+1 point)
    /// </summary>
    /// <param name="srcPoints">The source points.</param>
    /// <param name="destPoints">The dest points.</param>
    /// <returns></returns>
    public static T[,] MapPoints(T[][] srcPoints, T[][] destPoints)
    {
        int dim = srcPoints[0].Length;
        int n = dim + 1;

        if (srcPoints.Length != n || destPoints.Length != n)
            throw new InvalidOperationException();

        T[,] matrix = new T[n, n];
        Array.Copy(srcPoints, matrix, srcPoints.Length);
        for (int i = 0; i < n; i++)
            matrix[i, n - 1] = 1;

        T[,] matrix2 = new T[n, n - 1];
        Array.Copy(destPoints, matrix2, destPoints.Length);
        for (int i = 0; i < n; i++)
            matrix2[i, n - 1] = 1;

        double[,] inverse = EquationSolving.Inverse(matrix);
        T[,] result = Mult(inverse, matrix2);
        return result;
    }

    public static void Copy(T[][] src, T[,] dest)
    {
        for (int i = 0; i < src.Length; i++) {
            T[] row = src[i];
            for (int j = 0; j < row.Length; j++)
                dest[i, j] = row[j];
        }
    }

    #endregion
}