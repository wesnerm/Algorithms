using static System.Math;

namespace Algorithms.NpComplete;

public class KnapsackAlgorithm
{
    public static int Knapsack01(int[] wt, int[] val, int maxw)
    {
        int n = wt.Length;
        int[] maxv = new int[maxw + 1];
        for (int i = 0; i < n; i++)
        for (int j = maxw; j >= wt[i]; j--)
            maxv[j] = Max(maxv[j], val[i] + maxv[j - wt[i]]);
        return maxv[maxw];
    }

    public static int Knapsack01_2D(int[] wt0, int[] wt1, int m, int n)
    {
        int[,] dp = new int[m + 1, n + 1];
        for (int k = 0; k < wt0.Length; k++) {
            int zeros = wt0[k];
            int ones = wt1[k];
            for (int i = m; i >= zeros; i--)
            for (int j = n; j >= ones; j--)
                dp[i, j] = Max(dp[i, j], dp[i - zeros, j - ones] + 1);
        }

        return dp[m, n];
    }

    public static int Knapsack01_2D(int[] wt0, int[] wt1, int[] val, int m, int n)
    {
        int[,] dp = new int[m + 1, n + 1];
        for (int k = 0; k < wt0.Length; k++) {
            int zeros = wt0[k];
            int ones = wt1[k];
            int v = val[k];
            for (int i = m; i >= zeros; i--)
            for (int j = n; j >= ones; j--)
                dp[i, j] = Max(dp[i, j], dp[i - zeros, j - ones] + v);
        }

        return dp[m, n];
    }
}