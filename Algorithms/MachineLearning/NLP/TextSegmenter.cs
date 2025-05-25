namespace Algorithms.MachineLearning;

public struct TextSegmenter
{
    public string Segment(string s, Func<string, bool> isword)
    {
        int[,] dp = BuildSegmentTable(s, isword);
        bool[] segments = BuildSegmentPoints(s, dp);

        for (int i = 0; i < s.Length; i++) {
            if (!segments[i]) continue;
            if (i >= 1) {
                if (i + 1 <= s.Length && segments[i + 1] && Match(s, "ed", i - 1)) Clear(segments, i, 1);
                if (i + 1 <= s.Length && segments[i + 1] && Match(s, "er", i - 1)) Clear(segments, i, 1);
                if (i + 3 <= s.Length && segments[i + 3] && Match(s, "ing", i)) Clear(segments, i - 1, 4);
            }

            if (i + 3 <= s.Length && segments[i + 3] && Match(s, "ful", i)) Clear(segments, i, 3);
            if (i + 1 <= s.Length && segments[i + 1] && Match(s, "s", i)) Clear(segments, i, 1);
            if (i + 2 <= s.Length && segments[i + 2] && Match(s, "es", i)) Clear(segments, i, 2);
            if (i + 2 <= s.Length && segments[i + 2] && Match(s, "ed", i)) Clear(segments, i, 2);

            if (i + 2 <= s.Length && segments[i + 2] && Match(s, "im", i)) Clear(segments, i + 1, 1);
            if (i + 4 <= s.Length && segments[i + 4] && Match(s, "cant", i)) Clear(segments, i + 1, 3);
            if (i + 4 <= s.Length && segments[i + 4] && Match(s, "dont", i)) Clear(segments, i + 1, 3);
        }

        for (int i = 1; i < s.Length; i++) {
            if (!segments[i] || !segments[i - 1]) continue;
            for (int j = i; j < s.Length && segments[j + 1]; j++)
                segments[j] = false;
        }

        var sb = new StringBuilder(s.Length + dp[0, s.Length - 1] - 1);

        segments[0] = false;
        for (int i = 0; i < s.Length; i++) {
            if (segments[i])
                sb.Append(' ');
            sb.Append(s[i]);
        }

        string result = sb.ToString();

        result = result.Replace("dont", "don't");
        result = result.Replace("im ", "i'm ");
        result = result.Replace("thats", "that's");
        result = result.Replace("cant", "can't");
        return result;
    }

    public int[,] BuildSegmentTable(string s, Func<string, bool> isword)
    {
        int[,] dp = new int[s.Length, s.Length];

        for (int i = 0; i < s.Length; i++) {
            dp[i, i] = 1;

            for (int j = i; j < s.Length && !char.IsLetter(s[j]); j++)
                dp[i, j] = 1;
        }

        for (int k = 2; k <= s.Length; k++)
        for (int i = 0; i + k <= s.Length; i++) {
            int j = i + k - 1;

            // Previously analyzed
            if (dp[i, j] == 1) continue;

            // Actual word
            if (isword(s.Substring(i, k))) {
                dp[i, j] = 1;
                continue;
            }

            int min = dp[i, j];
            if (min == 0) min = k;

            for (int m = i; m < j; m++) {
                int trial = dp[i, m] + dp[m + 1, j];
                if (trial < min) min = trial;
            }

            dp[i, j] = min;
        }

        return dp;
    }

    public bool[] BuildSegmentPoints(string s, int[,] dp)
    {
        bool[] segments = new bool[s.Length + 1];
        SegmentCore(dp, segments, s, 0, s.Length - 1);

        segments[0] = true;
        segments[s.Length] = true;
        return segments;
    }

    void Clear(bool[] segments, int start, int count)
    {
        for (int i = 0; i < count; i++)
            segments[i + start] = false;
    }

    bool Match(string s, string pat, int i)
    {
        if (i + pat.Length > s.Length) return false;
        return string.Compare(s, i, pat, 0, pat.Length, true) == 0;
    }

    void SegmentCore(int[,] dp, bool[] segments, string s, int left, int right)
    {
        int count = dp[left, right];

        Console.Error.WriteLine($"SegmentCore({left}, {right}, {count})");

        if (left < right && count > 1) {
            int found = -1;

            for (int m = left; m < right; m++)
                if (dp[left, m] + dp[m + 1, right] <= count) {
                    found = m;

                    int penalty = 0;
                    int len = m - left + 1;
                    if (dp[left, m] == len && (len != 1 || "aiu".IndexOf(s[left]) < 0)) penalty += len;

                    len = right - (m + 1) + 1;
                    if (dp[m + 1, right] == len && (len != 1 || "aiu".IndexOf(s[right]) < 0)) penalty += len;
                    if (penalty == 0)
                        break;
                }

            segments[found + 1] = true;
            SegmentCore(dp, segments, s, left, found);
            SegmentCore(dp, segments, s, found + 1, right);
        }
    }
}