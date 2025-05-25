namespace Algorithms;

public class RegEx
{
    public bool IsMatch(string s, string p, int sindex, int pindex)
    {
        if (pindex == p.Length) return sindex == s.Length;
        char pch = p[pindex];

        bool matched = sindex < s.Length && (pch == '.' || s[sindex] == pch);

        if (pindex + 1 < p.Length && p[pindex + 1] == '*')
            while (true) {
                if (IsMatch(s, p, sindex, pindex + 2))
                    return true;

                matched = sindex < s.Length && (pch == '.' || s[sindex] == pch);
                if (!matched)
                    return false;

                sindex++;
            }

        return matched && IsMatch(s, p, sindex + 1, pindex + 1);
    }

    public bool IsMatch(string s, string p)
    {
        int pstar = -1;
        int sstar = 0;
        int pindex = 0;
        int sindex = 0;

        while (sindex < s.Length) {
            char pch = p[pindex];
            if (pch == '*') {
                sstar = sindex;
                pstar = pindex++;
            } else if (sindex >= s.Length || (pch != s[sindex] && pch != '?')) {
                if (pstar == -1)
                    return false;
                pindex = pstar + 1;
                sindex = ++sstar;
            } else {
                sindex++;
                pindex++;
            }
        }

        while (pindex < p.Length && p[pindex] == '*')
            pindex++;

        return pindex == p.Length;
    }

    public void WiggleSort(int[] nums)
    {
        for (int i = 1; i < nums.Length;)
            if (i % 2 == 1 == nums[i - 1] > nums[i]) {
                int tmp = nums[i];
                nums[i] = nums[i - 1];
                nums[i - 1] = tmp;
            }
    }
}