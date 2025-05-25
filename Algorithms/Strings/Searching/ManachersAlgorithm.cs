namespace Algorithms.Strings;

public class ManachersAlgorithm
{
    // http://www.geeksforgeeks.org/manachers-algorithm-linear-time-longest-palindromic-substring-part-4/

    public string FindLongestPalindromicString2(string text)
    {
        int n = text.Length;
        if (n == 0)
            return "";
        n = 2 * n + 1; //Position count
        int[] l = new int[n]; //LPS Length Array
        l[0] = 0;
        l[1] = 1;
        int c = 1; //centerPosition 
        int r = 2; //centerRightPosition
        int maxLpsLength = 0;
        int maxLpsCenterPosition = 0;

        // i = currentRightPosition
        for (int i = 2; i < n; i++) {
            //get currentLeftPosition iMirror for currentRightPosition i
            int iMirror = 2 * c - i; //currentLeftPosition
            //Reset expand - means no expansion required
            int expand = 0;
            int diff = r - i;
            //If currentRightPosition i is within centerRightPosition R
            if (diff > 0) {
                if (l[iMirror] < diff) // Case 1
                {
                    l[i] = l[iMirror];
                } else if (l[iMirror] == diff && i == n - 1) // Case 2
                {
                    l[i] = l[iMirror];
                } else if (l[iMirror] == diff && i < n - 1) // Case 3
                {
                    l[i] = l[iMirror];
                    expand = 1; // expansion required
                } else if (l[iMirror] > diff) // Case 4
                {
                    l[i] = diff;
                    expand = 1; // expansion required
                }
            } else {
                l[i] = 0;
                expand = 1; // expansion required
            }

            if (expand == 1)
                //Attempt to expand palindrome centered at currentRightPosition i
                //Here for odd positions, we compare characters and 
                //if match then increment LPS Length by ONE
                //If even position, we just increment LPS by ONE without 
                //any character comparison
                while (i + l[i] < n && i - l[i] > 0 &&
                       ((i + l[i] + 1) % 2 == 0 ||
                        text[(i + l[i] + 1) / 2] == text[(i - l[i] - 1) / 2]))
                    l[i]++;

            if (l[i] > maxLpsLength) // Track maxLPSLength
            {
                maxLpsLength = l[i];
                maxLpsCenterPosition = i;
            }

            // If palindrome centered at currentRightPosition i 
            // expand beyond centerRightPosition R,
            // adjust centerPosition C based on expanded palindrome.
            if (i + l[i] > r) {
                c = i;
                r = i + l[i];
            }
        }

        int start = (maxLpsCenterPosition - maxLpsLength) / 2;
        int end = start + maxLpsLength - 1;
        return text.Substring(start, end - start + 1);
    }

    // https://discuss.leetcode.com/topic/12944/22-line-c-manacher-s-algorithm-o-n-solution

    public string AlternativeLongestPalindrome(string s)
    {
        var t = new StringBuilder();
        foreach (char i in s) {
            t.Append('#');
            t.Append(i);
        }

        t.Append('#');

        int[] p = new int[t.Length]; // Array to record longest palindrome
        int center = 0, boundary = 0, maxLen = 0, resCenter = 0;
        for (int i = 1; i < t.Length - 1; i++) {
            int iMirror = 2 * center - i; // calc mirror i = center-(i-center)
            p[i] = boundary > i ? Math.Min(boundary - i, p[iMirror]) : 0; // shortcut
            while (i - 1 - p[i] >= 0 && i + 1 + p[i] <= t.Length - 1 &&
                   t[i + 1 + p[i]] == t[i - 1 - p[i]]) // Attempt to expand palindrome centered at i
                p[i]++;
            if (i + p[i] > boundary) {
                // update center and boundary
                center = i;
                boundary = i + p[i];
            }

            if (p[i] > maxLen) {
                // update result
                maxLen = p[i];
                resCenter = i;
            }
        }

        return s.Substring((resCenter - maxLen) / 2, maxLen);
    }

    // SOURCE: http://codeforces.com/blog/entry/12143

    /// <summary>
    ///     It returns the array L[i] — index of the beginning of the longest
    ///     palindrome centered at position i. (For both even and odd positions
    ///     simultaneously.) If you need the end index, it is calculated by
    ///     symmetry: R[i] = i - L[i]. And of course, the length of the
    ///     palindrome is len[i] = R[i]-L[i]+1 = i - 2*L[i] + 1.
    /// </summary>
    public static int[] ManacherStart(string s)
    {
        int n = s.Length;
        if (n == 0) return new int[0];
        int[] res = new int[2 * n - 1];
        int p = 0, pR = -1;
        for (int pos = 0; pos < 2 * n - 1; ++pos) {
            int R = pos <= 2 * pR
                ? Math.Min(p - res[2 * p - pos], pR)
                : pos / 2;
            int L = pos - R;
            while (L > 0 && R < n - 1 && s[L - 1] == s[R + 1]) {
                --L;
                ++R;
            }

            res[pos] = L;
            if (R > pR) {
                pR = R;
                p = pos;
            }
        }

        return res;
    }

    public string FindLongestPalindromicString(string text)
    {
        int n = text.Length;
        if (n == 0)
            return "";

        n = 2 * n + 1; //Position count
        int[] l = new int[n]; //LPS Length Array
        l[0] = 0;
        l[1] = 1;
        int c = 1; //centerPosition 
        int r = 2; //centerRightPosition
        int i; //currentRightPosition
        int maxLpsLength = 0;
        int maxLpsCenterPosition = 0;

        for (i = 2; i < n; i++) {
            //get currentLeftPosition iMirror for currentRightPosition i
            int iMirror = 2 * c - i; //currentLeftPosition
            l[i] = 0;
            int diff = r - i;

            //If currentRightPosition i is within centerRightPosition R
            if (diff > 0)
                l[i] = Math.Min(l[iMirror], diff);

            //Attempt to expand palindrome centered at currentRightPosition i
            //Here for odd positions, we compare characters and 
            //if match then increment LPS Length by ONE
            //If even position, we just increment LPS by ONE without 
            //any character comparison
            while (i + l[i] < n && i - l[i] > 0 &&
                   ((i + l[i] + 1) % 2 == 0 ||
                    text[(i + l[i] + 1) / 2] == text[(i - l[i] - 1) / 2]))
                l[i]++;

            if (l[i] > maxLpsLength) // Track maxLPSLength
            {
                maxLpsLength = l[i];
                maxLpsCenterPosition = i;
            }

            //If palindrome centered at currentRightPosition i 
            //expand beyond centerRightPosition R,
            //adjust centerPosition C based on expanded palindrome.
            if (i + l[i] > r) {
                c = i;
                r = i + l[i];
            }
        }

        int start = (maxLpsCenterPosition - maxLpsLength) / 2;
        int end = start + maxLpsLength - 1;
        return text.Substring(start, end - start + 1);
    }

    static int Main()
    {
        string[] strs = new[]
        {
            "babcbabcbaccba",
            "abaaba",
            "abababa",
            "abcbabcbabcba",
            "forgeeksskeegfor",
            "caba",
            "abacdfgdcaba",
            "abacdfgdcabba",
            "abacdedcaba",
        };

        var ma = new ManachersAlgorithm();
        foreach (string s in strs) {
            string lps = ma.FindLongestPalindromicString(s);
            Console.Write($"LPS of string {s} : {lps}");
        }

        return 0;
    }
}