namespace Algorithms.Mathematics.Multiplication;

public static class FastWalshTransformGeneralized
{
    // Based on Antoine Joux, "Algorithmic Cryptanalysis", page 304.

    public static long[,] PrewalshTransform(long[] input, int p)
    {
        int size = input.Length;
        long[,] output = new long[p, size];
        for (int i = 0; i < size; i++) {
            for (int j = 1; j < p; j++)
                output[j, i] = -1;

            // TODO: Need to adapt this to multiple numbers in 
            if (input[i] != 0)
                output[input[i], i] = p - 1;
        }

        return output;
    }

    public static long[,] WalshTransformOverP(long[] input, int p, bool inverse = false)
    {
        long[,] s = PrewalshTransform(input, p);
        int n = input.Length;
        long[,] t = new long[p, p];
        for (int d = 1; d < n; d *= p)
        for (int i = 0; i < n; i += p * d)
        for (int j = 0; j < d; j++) {
            int jj = i + j;
            for (int k = 0; k < p; k++) {
                long sum = 0;
                for (int m = 1; m < p; m++) {
                    t[m, k] = s[m, k * d + jj];
                    s[m, k * d + jj] = 0;
                    sum += t[m, k];
                }

                t[0, k] = -sum;
            }

            for (int k = 0; k < p; k++) {
                int sk = inverse ? k : -k;
                for (int m = 1; m < p; m++)
                for (int x = 0; x < p; x++)
                    s[m, k * d + jj] += t[(m + sk * x) % p, x];
            }

            if (inverse)
                for (int k = 0; k < p; k++)
                for (int m = 1; m < p; m++)
                    s[m, k * d + jj] /= p;
        }

        return s;
    }
}