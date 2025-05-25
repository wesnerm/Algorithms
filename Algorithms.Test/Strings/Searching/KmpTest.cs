namespace Algorithms.Strings;

[TestFixture]
public class KmpTest
{
    const string text1 = "The example above illustrates the general technique for assembling " +
                         "the table with a minimum of fuss. The principle is that of the overall search: " +
                         "most of the work was already done in getting to the current position, so very " +
                         "little needs to be done in leaving it. The only minor complication is that the " +
                         "logic which is correct late in the string erroneously gives non-proper " +
                         "substrings at the beginning. This necessitates some initialization code.";

    const string text2 = "ABABDABACDABABCABAB";

    const string pat1 = "table";

    const string pat2 = "ABABCABAB";

    const string text3 = "ABABABABAABABABABABA";

    const string pat3 = "ABABA";

    [Test]
    public void Kmp()
    {
        Kmp kmp;
        kmp = new Kmp(pat1);
        Console.WriteLine(string.Join(" ", kmp.Lps));
        CheckOccurrences(text1, pat1, kmp.Instances(text1));
        kmp = new Kmp(pat2);
        Console.WriteLine(string.Join(" ", kmp.Lps));
        CheckOccurrences(text2, pat2, kmp.Instances(text2));
        kmp = new Kmp(pat3);
        Console.WriteLine(string.Join(" ", kmp.Lps));
        CheckOccurrences(text3, pat3, kmp.Instances(text3));
    }

    public void CheckOccurrences(string text, string pat, IEnumerable<int> results)
    {
        int occurrences = Instances(text, pat);
        int resultCount = results.Count();
        AreEqual(occurrences, resultCount);
        AreEqual(resultCount, new HashSet<int>(results).Count);

        foreach (int i in results)
            AreEqual(pat, text.Substring(i, pat.Length));
    }

    static int Instances(string text, string pat)
    {
        if (pat.Length == 0) return 0;

        int count = 0;
        int j;
        for (int i = text.Length - pat.Length; i >= 0; i--) {
            for (j = 0; j < pat.Length; j++)
                if (text[i + j] != pat[j] && pat[j] != '*')
                    break;
            if (j == pat.Length)
                count++;
        }

        return count;
    }

    // private Kmp sample;
}