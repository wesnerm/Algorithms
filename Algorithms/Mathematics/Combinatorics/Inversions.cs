namespace Algorithms.Mathematics.Combinatorics;

public static class Inversions
{
    public static long NumberOfInversionsWhenKLessThanOrEqualN(int N, int K)
    {
        long sum = Comb(N + K - 1, K);
        for (int j = 1;; j++) {
            long u = (j * (3 * j - 1)) >> 1;
            if (K < u) break;
            int k = K - (int)u;
            int k2 = k - j;
            sum += Comb(N + k - 1, k);
            if (k2 >= 0)
                sum += Comb(N + k - 1, k) + Comb(N + k2 - 1, k2);
        }

        return sum;
    }

    public static long NumberOfInversions(long N, long K)
    {
        long[] grid1 = new long[K + 1];
        long[] grid2 = new long[K + 1];

        if (K <= 1) return K == 0 ? 1 : Math.Max(0, N - 1);
        if (N <= 1) return 0;

        for (int i = 0; i <= K; i++)
            grid1[i] = 1;
        for (int n = 1; n <= N; n++) {
            long cur = grid2[0] = 1;
            for (int k = 1; k <= K; k++) {
                int start = Math.Max(0, k - n + 1);
                long sum = grid1[k] - (start == 0 ? 0 : grid1[start - 1]);
                cur = grid2[k] = cur + sum;
            }

            Swap(ref grid2, ref grid1);
        }

        return grid1[K] - grid1[K - 1];
    }

    public static long NumberOfInversions2(long N, long K) // Does this work?
    {
        long[] grid1 = new long[K + 1];
        long[] grid2 = new long[K + 1];

        if (K <= 1)
            return K == 0 ? 1 : Math.Max(0, N - 1);

        if (N <= 1)
            return 0;

        grid1[0] = 1;
        for (int n = 1; n <= N; n++) {
            long cur = grid2[0] = 1;
            for (int k = 1; k <= K; k++) {
                int start = Math.Max(0, k - n + 1);
                long sum = grid1[k] - (start == 0 ? 0 : grid1[start - 1]);
                cur = grid2[k] = cur + sum;
            }

            Swap(ref grid2, ref grid1);
        }

        return grid1[K];
    }
}