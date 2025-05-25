using Algorithms.Collections;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Algorithms.Mathematics;

public static class LinearAlgebra
{
    // Gauss-Jordan elimination with full pivoting.
    //
    // Uses:
    //   (1) solving systems of linear equations (AX=B)
    //   (2) inverting matrices (AX=I)
    //   (3) computing determinants of square matrices
    //
    // Running time: O(n^3)
    //
    // INPUT:    a[][] = an nxn matrix
    //           b[][] = an nxm matrix
    //
    // OUTPUT:   X      = an nxm matrix (stored in b[][])
    //           A^{-1} = an nxn matrix (stored in a[][])
    //           returns determinant of a[][]

    const double Epsilon = 1e-10;

    public static double GaussJordan(double[][] a, double[][] b)
    {
        int n = a.Length;
        int m = b[0].Length;
        int[] irow = new int[n];
        int[] icol = new int[n];
        int[] ipiv = new int[n];
        double det = 1;

        for (int i = 0; i < n; i++) {
            int pj = -1, pk = -1;
            for (int j = 0; j < n; j++)
                if (ipiv[j] == 0)
                    for (int k = 0; k < n; k++)
                        if (ipiv[k] == 0)
                            if (pj == -1 || Math.Abs(a[j][k]) > Math.Abs(a[pj][pk])) {
                                pj = j;
                                pk = k;
                            }

            if (Math.Abs(a[pj][pk]) < Epsilon)
                throw new InvalidOperationException("Matrix is singular.");

            ipiv[pk]++;
            (a[pj], a[pk]) = (a[pk], a[pj]);
            (b[pj], b[pk]) = (b[pk], b[pj]);
            if (pj != pk) det *= -1;
            irow[i] = pj;
            icol[i] = pk;

            double c = 1.0 / a[pk][pk];
            det *= a[pk][pk];
            a[pk][pk] = 1.0;
            for (int p = 0; p < n; p++) a[pk][p] *= c;
            for (int p = 0; p < m; p++) b[pk][p] *= c;
            for (int p = 0; p < n; p++)
                if (p != pk) {
                    c = a[p][pk];
                    a[p][pk] = 0;
                    for (int q = 0; q < n; q++) a[p][q] -= a[pk][q] * c;
                    for (int q = 0; q < m; q++) b[p][q] -= b[pk][q] * c;
                }
        }

        for (int p = n - 1; p >= 0; p--)
            if (irow[p] != icol[p])
                for (int k = 0; k < n; k++)
                    Swap(ref a[k][irow[p]], ref a[k][icol[p]]);

        return det;
    }

    public static double GaussJordan(double[,] a, double[,] b)
    {
        int n = a.GetLength(0);
        int m = b.GetLength(1);
        int[] irow = new int[n];
        int[] icol = new int[n];
        int[] ipiv = new int[n];
        double det = 1;

        for (int i = 0; i < n; i++) {
            int pj = -1, pk = -1;
            for (int j = 0; j < n; j++)
                if (ipiv[j] == 0)
                    for (int k = 0; k < n; k++)
                        if (ipiv[k] == 0)
                            if (pj == -1 || Math.Abs(a[j, k]) > Math.Abs(a[pj, pk])) {
                                pj = j;
                                pk = k;
                            }

            if (Math.Abs(a[pj, pk]) < Epsilon)
                throw new InvalidOperationException("Matrix is singular.");

            ipiv[pk]++;
            a.SwapRow(pj, pk);
            b.SwapRow(pj, pk);
            if (pj != pk) det *= -1;
            irow[i] = pj;
            icol[i] = pk;

            double c = 1.0 / a[pk, pk];
            det *= a[pk, pk];
            a[pk, pk] = 1.0;
            for (int p = 0; p < n; p++) a[pk, p] *= c;
            for (int p = 0; p < m; p++) b[pk, p] *= c;
            for (int p = 0; p < n; p++)
                if (p != pk) {
                    c = a[p, pk];
                    a[p, pk] = 0;
                    for (int q = 0; q < n; q++)
                        a[p, q] -= a[pk, q] * c;
                    for (int q = 0; q < m; q++)
                        b[p, q] -= b[pk, q] * c;
                }
        }

        for (int p = n - 1; p >= 0; p--)
            if (irow[p] != icol[p])
                for (int k = 0; k < n; k++)
                    Swap(ref a[k, irow[p]], ref a[k, icol[p]]);

        return det;
    }

    // Reduced row echelon form via Gauss-Jordan elimination 
    // with partial pivoting.  This can be used for computing
    // the rank of a matrix.
    //
    // Running time: O(n^3)
    //
    // INPUT:    a[][] = an nxm matrix
    //
    // OUTPUT:   rref[][] = an nxm matrix (stored in a[][])
    //           returns rank of a[][]

    public static int ReducedRowEchelonForm(double[][] a)
    {
        int n = a.Length;
        int m = a[0].Length;
        int r = 0;
        for (int c = 0; c < m && r < n; c++) {
            int j = r;
            for (int i = r + 1; i < n; i++)
                if (Math.Abs(a[i][c]) > Math.Abs(a[j][c]))
                    j = i;
            if (Math.Abs(a[j][c]) < Epsilon) continue;

            Swap(ref a[j], ref a[r]);

            double s = 1.0 / a[r][c];
            for (j = 0; j < m; j++) a[r][j] *= s;

            for (int i = 0; i < n; i++)
                if (i != r) {
                    double t = a[i][c];
                    for (j = 0; j < m; j++)
                        a[i][j] -= t * a[r][j];
                }

            r++;
        }

        return r;
    }

    public static int ReducedRowEchelonForm(double[,] a)
    {
        int n = a.GetLength(0);
        int m = a.GetLength(1);
        int r = 0;
        for (int c = 0; c < m && r < n; c++) {
            int j = r;
            for (int i = r + 1; i < n; i++)
                if (Math.Abs(a[i, c]) > Math.Abs(a[j, c]))
                    j = i;
            if (Math.Abs(a[j, c]) < Epsilon) continue;

            for (int i = 0; i < m; i++)
                Swap(ref a[j, i], ref a[r, i]);

            double s = 1.0 / a[r, c];
            for (j = 0; j < m; j++) a[r, j] *= s;

            for (int i = 0; i < n; i++)
                if (i != r) {
                    double t = a[i, c];
                    for (j = 0; j < m; j++)
                        a[i, j] -= t * a[r, j];
                }

            r++;
        }

        return r;
    }
}