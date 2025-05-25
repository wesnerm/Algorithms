namespace Algorithms.Mathematics.Matrices;

public class StrassenMultiplication
{
    const int MOD = 998244353;

    // CPP program to implement Strassen’s Matrix  
    // Multiplication Algorithm  

    /* Strassen's Algorithm for matrix multiplication
       Complexity:    O(n^2.808) */

    static long[][] MatrixMultiply(long[][] a, long[][] b, int n, int l, int p)
    {
        long[][] c = new long[n][];
        for (int i = 0; i < n; i++)
            c[i] = new long[p];

        for (int i = 0; i < n; i++)
        for (int j = 0; j < p; j++) {
            long sum = c[i][j];
            for (int k = 0; k < l; k++)
                sum += a[i][k] * b[k][j];
            c[i][j] = sum;
        }

        return c;
    }

    static long[][] MatrixMultiply2(long[][] a, long[][] b, int n, int l, int p)
    {
        // Not actually faster
        long[][] c = new long[n][];
        for (int i = 0; i < n; i++)
            c[i] = new long[p];

        for (int i = 0; i < n; i++) {
            long[] ci = c[i];
            for (int k = 0; k < l; k++) {
                long r = a[i][k];
                long[] bk = b[k];
                for (int j = 0; j < p; j++)
                    ci[j] += r * bk[j];
            }
        }

        return c;
    }

    //matrix mult_strassen(matrix a, matrix b)
    //{
    //    if (a.dim() <= cut)
    //        return mult_std(a, b);

    //    matrix a11 = get_part(0, 0, a);
    //    matrix a12 = get_part(0, 1, a);
    //    matrix a21 = get_part(1, 0, a);
    //    matrix a22 = get_part(1, 1, a);

    //    matrix b11 = get_part(0, 0, b);
    //    matrix b12 = get_part(0, 1, b);
    //    matrix b21 = get_part(1, 0, b);
    //    matrix b22 = get_part(1, 1, b);

    //    matrix m1 = mult_strassen(a11 + a22, b11 + b22);
    //    matrix m2 = mult_strassen(a21 + a22, b11);
    //    matrix m3 = mult_strassen(a11, b12 - b22);
    //    matrix m4 = mult_strassen(a22, b21 - b11);
    //    matrix m5 = mult_strassen(a11 + a12, b22);
    //    matrix m6 = mult_strassen(a21 - a11, b11 + b12);
    //    matrix m7 = mult_strassen(a12 - a22, b21 + b22);

    //    matrix c(a.dim(), false, true);
    //    set_part(0, 0, &c, m1 + m4 - m5 + m7);
    //    set_part(0, 1, &c, m3 + m5);
    //    set_part(1, 0, &c, m2 + m4);
    //    set_part(1, 1, &c, m1 - m2 + m3 + m6);

    //    return c;
    //}

    public static long[][] Strassen(long[][] a, long[][] b, int n, int m, int q)
    {
        if (n == 1 || m == 1 || q == 1)
            return MatrixMultiply(a, b, n, m, q);

        long[][] c = new long[n][];
        for (int i = 0; i < n; i++)
            c[i] = new long[q];

        int adjN = (n >> 1) + (n & 1);
        int adjL = (m >> 1) + (m & 1);
        int adjM = (q >> 1) + (q & 1);

        long[][][][] As = new long[2][][][];
        for (int x = 0; x < 2; x++) {
            As[x] = new long[2][][];
            for (int y = 0; y < 2; y++) {
                long[][] axy = As[x][y] = new long[adjN][];
                for (int i = 0; i < adjN; i++) {
                    axy[i] = new long[adjL];
                    for (int j = 0; j < adjL; j++) {
                        int I = i + (x & 1) * adjN;
                        int J = j + (y & 1) * adjL;
                        axy[i][j] = I < n && J < m ? a[I][J] : 0;
                    }
                }
            }
        }

        long[][][][] Bs = new long[2][][][];
        for (int x = 0; x < 2; x++) {
            Bs[x] = new long[2][][];
            for (int y = 0; y < 2; y++) {
                long[][] bxy = Bs[x][y] = new long[adjN][];
                for (int i = 0; i < adjL; i++) {
                    bxy[i] = new long[adjM];
                    for (int j = 0; j < adjM; j++) {
                        int I = i + (x & 1) * adjL;
                        int J = j + (y & 1) * adjM;
                        bxy[i][j] = I < m && J < q ? b[I][J] : 0;
                    }
                }
            }
        }

        long[][][] s = new long[10][][];
        for (int i = 0; i < 10; i++)
            switch (i) {
                case 0:
                    s[i] = new long[adjL][];
                    for (int j = 0; j < adjL; j++) {
                        s[i][j] = new long[adjM];
                        for (int k = 0; k < adjM; k++) s[i][j][k] = Bs[0][1][j][k] - Bs[1][1][j][k];
                    }

                    break;
                case 1:
                    s[i] = new long[adjN][];
                    for (int j = 0; j < adjN; j++) {
                        s[i][j] = new long[adjL];
                        for (int k = 0; k < adjL; k++) s[i][j][k] = As[0][0][j][k] + As[0][1][j][k];
                    }

                    break;
                case 2:
                    s[i] = new long[adjN][];
                    for (int j = 0; j < adjN; j++) {
                        s[i][j] = new long[adjL];
                        for (int k = 0; k < adjL; k++) s[i][j][k] = As[1][0][j][k] + As[1][1][j][k];
                    }

                    break;
                case 3:
                    s[i] = new long[adjL][];
                    for (int j = 0; j < adjL; j++) {
                        s[i][j] = new long[adjM];
                        for (int k = 0; k < adjM; k++) s[i][j][k] = Bs[1][0][j][k] - Bs[0][0][j][k];
                    }

                    break;
                case 4:
                    s[i] = new long[adjN][];
                    for (int j = 0; j < adjN; j++) {
                        s[i][j] = new long[adjL];
                        for (int k = 0; k < adjL; k++) s[i][j][k] = As[0][0][j][k] + As[1][1][j][k];
                    }

                    break;
                case 5:
                    s[i] = new long[adjL][];
                    for (int j = 0; j < adjL; j++) {
                        s[i][j] = new long[adjM];
                        for (int k = 0; k < adjM; k++) s[i][j][k] = Bs[0][0][j][k] + Bs[1][1][j][k];
                    }

                    break;
                case 6:
                    s[i] = new long[adjN][];
                    for (int j = 0; j < adjN; j++) {
                        s[i][j] = new long[adjL];
                        for (int k = 0; k < adjL; k++) s[i][j][k] = As[0][1][j][k] - As[1][1][j][k];
                    }

                    break;
                case 7:
                    s[i] = new long[adjL][];
                    for (int j = 0; j < adjL; j++) {
                        s[i][j] = new long[adjM];
                        for (int k = 0; k < adjM; k++) s[i][j][k] = Bs[1][0][j][k] + Bs[1][1][j][k];
                    }

                    break;
                case 8:
                    s[i] = new long[adjN][];
                    for (int j = 0; j < adjN; j++) {
                        s[i][j] = new long[adjL];
                        for (int k = 0; k < adjL; k++) s[i][j][k] = As[0][0][j][k] - As[1][0][j][k];
                    }

                    break;
                case 9:
                    s[i] = new long[adjL][];
                    for (int j = 0; j < adjL; j++) {
                        s[i][j] = new long[adjM];
                        for (int k = 0; k < adjM; k++) s[i][j][k] = Bs[0][0][j][k] + Bs[0][1][j][k];
                    }

                    break;
            }

        long[][] p0 = Strassen(As[0][0], s[0], adjN, adjL, adjM);
        long[][] p1 = Strassen(s[1], Bs[1][1], adjN, adjL, adjM);
        long[][] p2 = Strassen(s[2], Bs[0][0], adjN, adjL, adjM);
        long[][] p3 = Strassen(As[1][1], s[3], adjN, adjL, adjM);
        long[][] p4 = Strassen(s[4], s[5], adjN, adjL, adjM);
        long[][] p5 = Strassen(s[6], s[7], adjN, adjL, adjM);
        long[][] p6 = Strassen(s[8], s[9], adjN, adjL, adjM);

        for (int i = 0; i < adjN; i++)
        for (int j = 0; j < adjM; j++) {
            c[i][j] = p4[i][j] + p3[i][j] - p1[i][j] + p5[i][j];
            if (j + adjM < q)
                c[i][j + adjM] = p0[i][j] + p1[i][j];
            if (i + adjN < n)
                c[i + adjN][j] = p2[i][j] + p3[i][j];
            if (i + adjN < n && j + adjM < q)
                c[i + adjN][j + adjM] = p4[i][j] + p0[i][j] - p2[i][j] - p6[i][j];
        }

        return c;
    }

    public static int[,] Mult(int[,] a, int[,] b, int[,] c = null)
    {
        int arows = a.GetLength(0);
        int bcols = b.GetLength(1);
        int mid = a.GetLength(1);
        if (c == null) c = new int[arows, bcols];

        for (int i = 0; i < arows; i++)
        for (int j = 0; j < bcols; j++) {
            long t = 0;
            for (int k = 0; k < mid; k++)
                t = (t + 1L * a[i, k] * b[k, j]) % MOD;
            c[i, j] = (int)t;
        }

        return c;
    }

    public static long[,] Mult(long[,] a, long[,] b, long[,] c = null)
    {
        int arows = a.GetLength(0);
        int bcols = b.GetLength(1);
        int mid = a.GetLength(1);
        if (c == null) c = new long[arows, bcols];

        for (int i = 0; i < arows; i++)
        for (int j = 0; j < bcols; j++) {
            long t = 0;
            for (int k = 0; k < mid; k++)
                t = (t + 1L * a[i, k] * b[k, j]) % MOD;
            c[i, j] = (int)t;
        }

        return c;
    }

    public static long[,] StrassenMod(long[,] a, long[,] b, int n, int m, int q)
    {
        if (n == 1 || m == 1 || q == 1)
            return Mult(a, b);

        long[,] c = new long[n, q];

        int adjN = (n >> 1) + (n & 1);
        int adjL = (m >> 1) + (m & 1);
        int adjM = (q >> 1) + (q & 1);

        long[,][,] As = new long[2, 2][,];
        for (int x = 0; x < 2; x++)
        for (int y = 0; y < 2; y++) {
            long[,] axy = As[x, y] = new long[adjN, adjL];
            for (int i = 0; i < adjN; i++)
            for (int j = 0; j < adjL; j++) {
                int I = i + (x & 1) * adjN;
                int J = j + (y & 1) * adjL;
                axy[i, j] = I < n && J < m ? a[I, J] : 0;
            }
        }

        long[,][,] Bs = new long[2, 2][,];
        for (int x = 0; x < 2; x++)
        for (int y = 0; y < 2; y++) {
            long[,] bxy = Bs[x, y] = new long[adjN, adjM];
            for (int i = 0; i < adjL; i++)
            for (int j = 0; j < adjM; j++) {
                int I = i + (x & 1) * adjL;
                int J = j + (y & 1) * adjM;
                bxy[i, j] = I < m && J < q ? b[I, J] : 0;
            }
        }

        long[,] s0 = new long[adjL, adjM];
        for (int j = 0; j < adjL; j++)
        for (int k = 0; k < adjM; k++)
            s0[j, k] = Bs[0, 1][j, k] - Bs[1, 1][j, k];

        long[,] s1 = new long[adjN, adjL];
        for (int j = 0; j < adjN; j++)
        for (int k = 0; k < adjL; k++)
            s1[j, k] = As[0, 0][j, k] + As[0, 1][j, k];

        long[,] s2 = new long[adjN, adjL];
        for (int j = 0; j < adjN; j++)
        for (int k = 0; k < adjL; k++)
            s2[j, k] = As[1, 0][j, k] + As[1, 1][j, k];

        long[,] s3 = new long[adjL, adjM];
        for (int j = 0; j < adjL; j++)
        for (int k = 0; k < adjM; k++)
            s3[j, k] = Bs[1, 0][j, k] - Bs[0, 0][j, k];

        long[,] s4 = new long[adjN, adjL];
        for (int j = 0; j < adjN; j++)
        for (int k = 0; k < adjL; k++)
            s4[j, k] = As[0, 0][j, k] + As[1, 1][j, k];

        long[,] s5 = new long[adjL, adjM];
        for (int j = 0; j < adjL; j++)
        for (int k = 0; k < adjM; k++)
            s5[j, k] = Bs[0, 0][j, k] + Bs[1, 1][j, k];

        long[,] s6 = new long[adjN, adjL];
        for (int j = 0; j < adjN; j++)
        for (int k = 0; k < adjL; k++)
            s6[j, k] = As[0, 1][j, k] - As[1, 1][j, k];

        long[,] s7 = new long[adjL, adjM];
        for (int j = 0; j < adjL; j++)
        for (int k = 0; k < adjM; k++)
            s7[j, k] = Bs[1, 0][j, k] + Bs[1, 1][j, k];

        long[,] s8 = new long[adjN, adjL];
        for (int j = 0; j < adjN; j++)
        for (int k = 0; k < adjL; k++)
            s8[j, k] = As[0, 0][j, k] - As[1, 0][j, k];

        long[,] s9 = new long[adjL, adjM];
        for (int j = 0; j < adjL; j++)
        for (int k = 0; k < adjM; k++)
            s9[j, k] = Bs[0, 0][j, k] + Bs[0, 1][j, k];

        long[,] p0 = StrassenMod(As[0, 0], s0, adjN, adjL, adjM);
        long[,] p1 = StrassenMod(s1, Bs[1, 1], adjN, adjL, adjM);
        long[,] p2 = StrassenMod(s2, Bs[0, 0], adjN, adjL, adjM);
        long[,] p3 = StrassenMod(As[1, 1], s3, adjN, adjL, adjM);
        long[,] p4 = StrassenMod(s4, s5, adjN, adjL, adjM);
        long[,] p5 = StrassenMod(s6, s7, adjN, adjL, adjM);
        long[,] p6 = StrassenMod(s8, s9, adjN, adjL, adjM);

        for (int i = 0; i < adjN; i++)
        for (int j = 0; j < adjM; j++) {
            long t = p4[i, j] + p3[i, j] - p1[i, j] + p5[i, j];
            while (t >= MOD) t -= MOD;
            c[i, j] = t;

            if (j + adjM < q) {
                t = p0[i, j] + p1[i, j];
                while (t >= MOD) t -= MOD;
                c[i, j + adjM] = t;
            }

            if (i + adjN < n) {
                t = p2[i, j] + p3[i, j];
                while (t >= MOD) t -= MOD;
                c[i + adjN, j] = t;
            }

            if (i + adjN < n && j + adjM < q) {
                t = p4[i, j] + p0[i, j] - p2[i, j] - p6[i, j];
                while (t >= MOD) t -= MOD;
                c[i + adjN, j + adjM] = t;
            }
        }

        return c;
    }
}