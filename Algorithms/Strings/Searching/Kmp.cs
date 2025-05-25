namespace Algorithms.Strings;

public class Kmp
{
    public readonly int[] Lps;
    public readonly string Pattern;

    public Kmp(string pattern)
    {
        Pattern = pattern;
        int m = pattern.Length;
        Lps = new int[m + 1];

        int j = Lps[0] = -1;
        for (int i = 0; i < m; i++) {
            while (j >= 0 && pattern[i] != pattern[j])
                j = Lps[j];
            Lps[i + 1] = ++j;
        }
    }

    public IEnumerable<int> Instances(string text, int i = 0)
    {
        int m = Pattern.Length;
        for (int j = 0; i < text.Length;)
            //for (int j = 0, limit = text.Length - m; i <= limit;)
        {
            while (j >= 0 && text[i] != Pattern[j]) j = Lps[j];
            i++;
            j++;
            if (j == m) {
                yield return i - j;
                j = Lps[j];
            }
        }
    }
}

public class KmpUntested
{
    public readonly int[] Lps;
    public readonly string Pattern;

    public KmpUntested(string p)
    {
        Pattern = p;
        Lps = new int[p.Length + 1];

        for (int i = 1, j = 0; i < p.Length; Lps[++i] = j)
            j = p[j] == p[i] ? j + 1 : Lps[j];
    }

    public IEnumerable<int> Search(string text, int i = 0)
    {
        string p = Pattern;
        int m = p.Length;
        int limit = text.Length - m;
        for (int j = 0;
             i <= limit;
             i += Math.Max(1, j - Lps[j]), j = Lps[j]) {
            while (j < m && text[i + j] == p[j]) ++j;
            if (j == m) yield return i;
        }
    }
}