namespace Algorithms.Mathematics.Multiplication;

public static class FastWalshTransform
{
    // Fast Walsh-Hadamard Transformation

    public static void Fwt(int[] a, int n, bool invert = false)
    {
        for (int d = 1; d < n; d <<= 1)
        for (int i = 0; i < n; i += d << 1)
        for (int j = 0; j < d; j++) {
            int x = a[i + j];
            int y = a[d + i + j];
            a[i + j] = x + y;
            if (a[i + j] >= MOD) a[i + j] -= MOD;
            a[d + i + j] = x - y;
            if (a[d + i + j] < 0) a[d + i + j] += MOD;
            // a[i+j] = x+y; // and
            // a[i+j+d] = x+y; // or
        }

        if (invert) {
            long inv = Inverse(n);
            for (int i = 0; i < n; i++) a[i] = (int)(a[i] * inv % MOD);
        }
    }

    // a, b are two polynomials and n is size which is power of two
    public static void Convolution(int[] a, int[] b, int n)
    {
        Fwt(a, n);
        Fwt(b, n);
        for (int i = 0; i < n; i++)
            a[i] = a[i] * b[i];
        Fwt(a, n, true);
    }
}