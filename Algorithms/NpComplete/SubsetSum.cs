namespace Algorithms.NpComplete;

public class SubsetSum
{
    // https://discuss.leetcode.com/topic/69/find-number-of-subsets-with-equal-sum/13
    public static int MumberOfEqualSumSubset(IList<int> num)
    {
        int sum = 0;
        foreach (int n in num) sum += n;
        if ((sum & 1) == 1) return 0;
        sum >>= 1;
        int[] dp = new int[sum + 1];
        dp[0] = 1;
        int currentSum = 0;
        for (int i = 0; i < num.Count; i++) {
            currentSum += num[i];
            for (int j = Math.Min(sum, currentSum); j >= num[i]; j--)
                dp[j] += dp[j - num[i]];
        }

        return dp[sum];
    }
}