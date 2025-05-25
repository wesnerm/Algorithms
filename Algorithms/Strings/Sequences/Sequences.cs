namespace Algorithms.Strings;

public static class Sequences
{
    // Count number of arrays that can be constructed
    // of length n starting from 1 and ending in x
    // in which adjacent elements are different
    public static long DifferenceAdjacentCounts(int n, long k, long x, int MOD)
    {
        // https://www.hackerrank.com/contests/101hack52/challenges/construct-the-array/submissions/code/1304435495
        long isOne = 1;
        long isOther = 0;
        for (int i = 2; i <= n; i++) {
            long isOne2 = (k - 1) * isOther % MOD;
            long isOther2 = ((k - 2) * isOther + isOne) % MOD;
            isOne = isOne2;
            isOther = isOther2;
        }

        return x == 1 ? isOne : isOther;
    }

    public static void LongestSubarrayWithDistinctElements(int[] a,
        out int[] prev, out int[] next)
    {
        int n = a.Length;
        prev = new int[n];
        next = new int[n];

        int max = a.Max();
        int[] set = new int[max + 1];

        int left = 0;
        for (int right = 0; right < n; right++) {
            int x = a[right];
            set[x]++;
            while (set[x] > 1 && left < right) set[a[left++]]--;
            prev[right] = left;
            next[left] = right;
        }

        for (left = 1; left < n; left++)
            next[left] = Math.Max(next[left - 1], next[left]);
    }

    public static long[] MosOverRanges(int[][] queries,
        Func<int, int, int, int> Add)
    {
        const int shift = 8;
        int n = queries.Length;
        long[] answers = new long[n];
        int[] indices = new int[n];

        Array.Sort(indices, (a, b) => {
            int cmp = (queries[a][0] >> shift).CompareTo(queries[b][0] >> shift);
            if (cmp != 0) return cmp;
            return queries[a][1].CompareTo(queries[b][1]);
        });

        int s = 0;
        int e = -1;
        long score = 0;

        foreach (int i in indices) {
            int[] qu = queries[i];
            while (e < qu[1]) {
                ++e;
                score += Add(e, s, e);
            }

            while (e > qu[1]) {
                score -= Add(e, s, e);
                e--;
            }

            while (s < qu[0]) {
                score -= Add(s, s, e);
                s++;
            }

            while (s > qu[0]) {
                s--;
                score += Add(s, s, e);
            }

            answers[i] = score;
        }

        return answers;
    }

    public static long NumberOfSubArraysNotContain(int[] array, Predicate<int> ignore)
    {
        int n = array.Length;
        long answer = (long)n * (n + 1) / 2;
        for (int i = 0; i < n;) {
            int j = i;
            while (j < n && !ignore(array[j])) j++;
            answer -= (long)(j - i) * (j - i + 1) / 2;
            i = j + 1;
        }

        return answer;
    }

    public static long NumberOfSubArraysNotContain2(int[] array, Predicate<int> ignore)
    {
        int n = array.Length;
        long answer = (long)n * (n + 1) / 2;
        int t = 0;
        for (int i = 0; i < n; i++) {
            t = ignore(array[i]) ? 0 : t + 1;
            answer -= t;
        }

        return answer;
    }
}