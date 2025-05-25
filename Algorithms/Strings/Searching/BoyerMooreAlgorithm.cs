using static System.Math;

namespace Algorithms.Strings;

// SOURCE: http://www.geeksforgeeks.org/pattern-searching-set-7-boyer-moore-algorithm-bad-character-heuristic/

public class BoyerMooreAlgorithm
{
    readonly int[] badcharHeuristic;
    readonly int offset;
    readonly string pattern;

    /// <summary>
    ///     Performs preprocessing for the bad character heuristic of boyer moore
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    public BoyerMooreAlgorithm(string pattern)
    {
        int minChar = int.MaxValue;
        int maxChar = int.MinValue;
        this.pattern = pattern;

        foreach (char c in pattern) {
            if (c < minChar) minChar = c;
            if (c > maxChar) maxChar = c;
        }

        // Initialize all occurrences as -1
        offset = minChar;
        badcharHeuristic = new int[maxChar - minChar + 1];
        for (int i = 0; i < badcharHeuristic.Length; i++)
            badcharHeuristic[i] = -1;

        // Fill the actual value of last occurrence of a character
        for (int i = 0; i < pattern.Length; i++)
            badcharHeuristic[pattern[i] - offset] = i;
    }

    /// <summary>
    ///     Searches the specified test.
    /// </summary>
    /// <param name="test">The test.</param>
    /// <returns></returns>
    public IEnumerable<int> Search(string test)
    {
        int m = pattern.Length;
        int n = test.Length;

        int s = 0; // s is shift of the pattern with respect to text
        while (s <= n - m) {
            int j = m - 1;

            /* Keep reducing index j of pattern while characters of
               pattern and text are matching at this shift s */
            while (j >= 0 && pattern[j] == test[s + j])
                j--;

            /* If the pattern is present at current shift, then index j
               will become -1 after the above loop */
            if (j < 0) {
                yield return s;

                /* Shift the pattern so that the next character in text
                   aligns with the last occurrence of it in pattern.
                   The condition s+m < n is necessary for the case when
                   pattern occurs at the end of text */

                int c = test[s + m];

                s += s + m < n ? m - BadCharHeuristic(test[s + m]) : 1;
            } else {
                /* Shift the pattern so that the bad character in text
                   aligns with the last occurrence of it in pattern. The
                   max function is used to make sure that we get a positive
                   shift. We may get a negative shift if the last occurrence
                   of bad character in pattern is on the right side of the
                   current character. */
                s += Max(1, j - BadCharHeuristic(test[s + j]));
            }
        }
    }

    int BadCharHeuristic(char ch)
    {
        int ich = ch - offset;
        return ich >= 0 && ich < badcharHeuristic.Length ? badcharHeuristic[ich] : -1;
    }
}