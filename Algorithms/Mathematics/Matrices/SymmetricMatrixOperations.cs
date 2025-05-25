namespace Algorithms.Mathematics.Matrices;

public static unsafe class SymmetricMatrixOperations
{
    public const int MOD = 1000 * 1000 * 1000 + 7;

    public static int[,] MultSym(int[,] a, int[,] b, int[,] c = null)
    {
        int n = a.GetLength(0);
        if (c == null) c = new int[n, n];

        for (int i = 0; i < n; i++)
            fixed (int* rowi = &a[i, 0]) {
                for (int j = i; j < n; j++) {
                    long t = 0;
                    int k;
                    fixed (int* rowj = &b[j, 0]) {
                        for (k = n - 1; k >= 8;) {
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t = (t + (long)rowi[k] * rowj[k]) % MOD;
                            k--;
                        }

                        if (k >= 0) {
                            for (; k >= 0; k--) t += (long)rowi[k] * rowj[k];
                            t %= MOD;
                        }

                        c[i, j] = (int)t;
                    }
                }
            }

        for (int i = n - 1; i >= 0; i--)
        for (int j = i - 1; j >= 0; j--)
            c[i, j] = c[j, i];

        return c;
    }

    public static int[,] SquareSym(int[,] a, int[,] c = null)
    {
        int n = a.GetLength(0);
        if (c == null) c = new int[n, n];

        for (int i = 0; i < n; i++)
            fixed (int* rowi = &a[i, 0]) {
                for (int j = i; j < n; j++) {
                    long t = 0;
                    int k;
                    fixed (int* rowj = &a[j, 0]) {
                        for (k = n - 1; k >= 8;) {
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t += (long)rowi[k] * rowj[k];
                            k--;
                            t = (t + (long)rowi[k] * rowj[k]) % MOD;
                            k--;
                        }

                        if (k >= 0) {
                            for (; k >= 0; k--) t += (long)rowi[k] * rowj[k];
                            t %= MOD;
                        }

                        c[i, j] = (int)t;
                    }
                }
            }

        for (int i = n - 1; i >= 0; i--)
        for (int j = i - 1; j >= 0; j--)
            c[i, j] = c[j, i];

        return c;
    }

    public static int[,] MultSparse(int[,] a, int[,] b, int[,] c = null)
    {
        int n = a.GetLength(0);
        if (c != null)
            Array.Clear(c, 0, n);
        else
            c = new int[n, n];

        for (int i = 0; i < n; i++)
        for (int k = 0; k < n; k++)
            if (a[i, k] != 0)
                for (int j = 0; j < n; j++)
                    c[i, j] = (int)(((long)c[i, j] + a[i, k] * b[k, j]) % MOD);

        return c;
    }

    public static int[] MultVector(int[,] a, int[] b, int[] c = null)
    {
        int n = a.GetLength(0);

        if (c == null) c = new int[n];

        for (int i = 0; i < n; i++) {
            long t = 0;

            int k = n - 1;

            fixed (int* row = &a[i, 0]) {
                for (; k >= 8; k--) {
                    t += (long)row[k] * b[k];
                    k--;
                    t += (long)row[k] * b[k];
                    k--;
                    t += (long)row[k] * b[k];
                    k--;
                    t += (long)row[k] * b[k];
                    k--;
                    t += (long)row[k] * b[k];
                    k--;
                    t += (long)row[k] * b[k];
                    k--;
                    t += (long)row[k] * b[k];
                    k--;
                    t += (long)row[k] * b[k];
                    k--;
                    t %= MOD;
                }

                if (k >= 0) {
                    for (; k >= 0; k--) t += (long)row[k] * b[k];
                    t %= MOD;
                }
            }

            c[i] = (int)t;
        }

        return c;
    }

    public static int[,] Pow(int[,] a, int p)
    {
        int n = a.GetLength(0);
        int[,] rtmp = new int[n, n];
        int[,] btmp = new int[n, n];
        int[,] result = null;
        int[,] b = Clone(a);
        int[,] swap;

        if (p == 0)
            return Diagonal(n);

        while (true) {
            if ((p & 1) != 0) {
                if (result != null) {
                    MultSym(result, b, rtmp);
                    Swap(ref result, ref rtmp);
                } else {
                    result = Clone(b);
                }
            }

            p >>= 1;
            if (p <= 0) break;
            SquareSym(b, btmp);
            Swap(ref b, ref btmp);
        }

        return result;
    }

    public static int[,] Diagonal(int n, int d = 1)
    {
        int[,] id = new int[n, n];
        for (int i = 0; i < n; i++) id[i, i] = d;

        return id;
    }

    public static int[,] Clone(int[,] m) => (int[,])m.Clone();

    public static void Assign(int[,] dest, int[,] src)
    {
        Array.Copy(src, dest, src.Length);
    }

    static void Swap<T>(ref T a, ref T b)
    {
        T tmp = a;
        a = b;
        b = tmp;
    }
}