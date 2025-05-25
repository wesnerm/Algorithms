namespace Algorithms.Strings;

public static class Periods
{
    public static int MinimumPeriod(string s) => (s + s).IndexOf(s, 1);

    public static int[,] MinimumPeriodOfSubstrings(string s)
    {
        // EncodeStringWithShortestLength

        int[,] period = new int[s.Length, s.Length + 1];
        for (int k = 1; k <= s.Length; k++)
        for (int i = 0; i + k <= s.Length; i++)
            period[i, k] = k;

        for (int k = s.Length - 1; k >= 1; k--) {
            int prev = 0;
            for (int i = 0; i + k <= s.Length; i++) {
                int j = i + k - 1;
                int curr = i >= 1 && s[j] == s[i - 1] ? prev + 1 : 0;
                for (int m = 2, p = curr; p >= k; m++, p -= k)
                    period[i - k * (m - 1), k * m] = k;
                prev = curr;
            }
        }

        return period;
    }

    public static void FindMaximumSpanningPeriodFromRight(int[] seq, out int period, out int maxspan)
    {
        int[] presum = new int[seq.Length + 1];
        int sum = 0;
        for (int i = 1; i < presum.Length; i++)
            presum[i] = sum += seq[i - 1];

        int mid = seq.Length / 2;
        period = 1;
        maxspan = 1;
        for (int i = 1; i <= mid; i++) {
            int t = i;
            int span = 0;
            for (; t <= mid; t *= 2) {
                int substr1 = seq.Length - t;
                int substr2 = substr1 - t;
                int pre1 = presum[substr1 + t] - presum[substr1];
                int pre2 = presum[substr2 + t] - presum[substr2];
                bool good = pre1 == pre2;
                for (int jj = 0; jj < t && good; jj++)
                    good &= seq[substr1 + jj] == seq[substr2 + jj];

                if (!good) break;
                span = t;
            }

            if (span == 0) continue;

            for (; t >= i; t >>= 1) {
                if (span + t > seq.Length) continue;

                int substr1 = seq.Length - t;
                int substr2 = seq.Length - span - t;
                int pre1 = presum[substr1 + t - 1] - presum[substr1 - 1];
                int pre2 = presum[substr2 + t - 1] - presum[substr2 - 1];
                bool good = pre1 == pre2;
                for (int jj = 0; jj < t && good; jj++)
                    good &= seq[substr1 + jj] == seq[substr2 + jj];
                if (good) span += t;
            }

            maxspan = span;
            period = i;
            i = span;
        }
    }

    // https://www.hackerrank.com/contests/projecteuler/challenges/euler167
    public static long ExtrapolatePeriodicSequence(IList<int> list, int k)
    {
        int[] diff = new int[list.Count - 1];
        for (int i = 0; i < diff.Length; i++)
            diff[i] = list[i + 1] - list[i];

        int[] presum = new int[diff.Length];
        presum[0] = diff[0];
        for (int i = 1; i < presum.Length; i++)
            presum[i] = diff[i] + presum[i - 1];

        FindMaximumSpanningPeriodFromRight(diff,
            out int period, out int span);

        int add = (k - list.Count) / period;
        int rem = (k - list.Count) % period;
        long last = list[list.Count - 1];
        int pos = presum.Length - period;
        long ans = last + add * (presum[presum.Length - 1] - presum[pos - 1]);
        return ans + (presum[pos + rem - 1] - presum[pos - 1]);
    }
}