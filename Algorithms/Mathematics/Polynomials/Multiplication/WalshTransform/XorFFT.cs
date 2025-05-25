namespace Algorithms.Mathematics.Multiplication;

public static class FastWalshHadamardTransform
{
    // Fast Walsh-Hadamard Transformation

    // https://discuss.codechef.com/questions/125559/sspld-editorial?sort=oldest
    // https://csacademy.com/blog/fast-fourier-transform-and-variations-of-it

    public static void XorFft(long[] a, bool invert)
    {
        int n = a.Length;

        //for (int len = 1; 2 * len <= n; len <<= 1) -- possible optimization
        for (int len = 1; len < n; len <<= 1)
        for (int i = 0; i < n; i += len << 1)
        for (int j = 0; j < len; j++) {
            long u = a[i + j];
            long v = a[len + i + j];
            a[i + j] = u + v;
            if (a[i + j] >= MOD)
                a[i + j] -= MOD;
            a[len + i + j] = u - v;
            if (a[len + i + j] < 0)
                a[len + i + j] += MOD;
        }

        if (invert) {
            long inv = Inverse(n);
            for (int i = 0; i < n; i++)
                a[i] = a[i] * inv % MOD;
        }
    }

    // matrix is [ 0, 1; 1, 1 ] and inverse is [ -1, 1; 1, 0 ]

    public static long[] AndFft(long[] a, bool invert)
    {
        int n = a.Length;
        for (int len = 1; 2 * len <= n; len <<= 1)
        for (int i = 0; i < n; i += 2 * len)
        for (int j = 0; j < len; j++) {
            long u = a[i + j];
            long v = a[i + len + j];

            if (!invert) {
                a[i + j] = v;
                a[i + len + j] = u + v;
            } else {
                a[i + j] = -u + v;
                a[i + len + j] = u;
            }
        }

        return a;
    }

    // matrix is [ 1, 1; 1, 0 ] and inverse is [ 0, 1; 1, -1 ]

    public static long[] OrFft(long[] a, bool invert)
    {
        int n = a.Length;
        for (int len = 1; 2 * len <= n; len <<= 1)
        for (int i = 0; i < n; i += 2 * len)
        for (int j = 0; j < len; j++) {
            long u = a[i + j];
            long v = a[i + len + j];

            if (!invert) {
                a[i + j] = u + v;
                a[i + len + j] = u;
            } else {
                a[i + j] = v;
                a[i + len + j] = u - v;
            }
        }

        return a;
    }
}