namespace Algorithms.Mathematics.DP;

public class DynamicProgramming
{
    // Matrix Ai has dimension p[i-1] x p[i] for i = 1..n
    public static int MatrixChainMultiplication(int[] dims)
    {
        /* m[i,j] = Minimum number of scalar multiplications needed
           to compute the matrix A[i]A[i+1]...A[j] = A[i..j] where
           dimension of A[i] is p[i-1] x p[i] */
        int n = dims.Length;
        int[,] m = new int[n, n];

        for (int length = 2; length < n; length++)
        for (int left = 1; left < n - length + 1; left++) {
            int right = left + length - 1;
            m[left, right] = int.MaxValue;
            for (int mid = left; mid < right; mid++) {
                int cost = m[left, mid] + m[mid + 1, right] + dims[left - 1] * dims[mid] * dims[right];
                if (cost < m[left, right])
                    m[left, right] = cost;
            }
        }

        return m[1, n - 1];
    }
}