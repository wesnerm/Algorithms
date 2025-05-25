namespace Algorithms.Strings;

/// <summary>
///     implements Z algorithm for pattern searching
/// </summary>
public class ZAlgorithm
{
    public static IEnumerable<int> Search(string text, string pattern)
    {
        // Create concatenated string "P$T"
        string concat = pattern + "$" + text;
        int n = concat.Length;

        // Construct Z array
        int[] z = ZFunction(concat);

        //  now looping through Z array for matching condition
        for (int i = 0; i < n; ++i)
            // if Z[i] (matched region) is equal to pattern
            // length  we got the pattern
            if (z[i] == pattern.Length)
                yield return i - pattern.Length - 1;
    }

    public static int StringRepetition(string text)
    {
        int[] z = ZFunction(text);
        double sqrt = Math.Sqrt(text.Length);
        int n = text.Length;
        for (int i = 1; i <= sqrt; i++)
            if (i + z[i] == n && n % i == 0)
                return i;
        return n;
    }

    public static int StringRepetitionSlow(string text) =>
        // O(n^2)
        (text + text).IndexOf(text, 1);

    public static int NumberOfAdditionalDistinctSubstringsAfterAppending(string text, char c)
    {
        var sb = new StringBuilder(text.Length + 1);
        sb.Append(c);
        for (int i = text.Length - 1; i >= 0; i--)
            sb.Append(text[i]);
        int maxPrefix = ZFunction(sb.ToString()).Max();
        return sb.Length - maxPrefix;
    }

    public static int[] ZFunction(string s)
    {
        int n = s.Length;
        int[] z = new int[n];

        int left = 0;
        int right = 0;
        for (int i = 1; i < n; i++) {
            if (i <= right)
                z[i] = Math.Min(right - i + 1, z[i - left]);

            while (i + z[i] < n && s[i + z[i]] == s[z[i]])
                z[i]++;

            if (i + z[i] - 1 > right) {
                right = i + z[i] - 1;
                left = i;
            }
        }

        return z;
    }

    // Driver program
    public static void Main()
    {
        string text = "GEEKS FOR GEEKS";
        string pattern = "GEEK";

        foreach (int i in Search(text, pattern)) Console.WriteLine($"Pattern found at index {i}");
    }
}