namespace Algorithms.Strings.Searching;

// SOURCE: http://www.geeksforgeeks.org/pattern-searching-set-5-efficient-constructtion-of-finite-automata/

class FiniteAutomataSearch
{
    public const int NO_OF_CHARS = 256;

    /* This function builds the TF table which represents Finite Automata for a
       given pattern  */

    void ComputeTransFun(string pat, int M, int[,] TF)
    {
        int i, lps = 0, x;

        // Fill entries in first row
        for (x = 0; x < NO_OF_CHARS; x++)
            TF[0, x] = 0;
        TF[0, pat[0]] = 1;

        // Fill entries in other rows
        for (i = 1; i <= M; i++) {
            // Copy values from row at index lps
            for (x = 0; x < NO_OF_CHARS; x++)
                TF[i, x] = TF[lps, x];

            // Update the entry corresponding to this character
            TF[i, pat[i]] = i + 1;

            // Update lps for next row to be filled
            if (i < M)
                lps = TF[lps, pat[i]];
        }
    }

    /* Prints all occurrences of pat in txt */

    void Search(string pat, string txt)
    {
        int M = pat.Length;
        int N = txt.Length;

        int[,] TF = new int[M + 1, NO_OF_CHARS];

        ComputeTransFun(pat, M, TF);

        // process text over FA.
        int i, j = 0;
        for (i = 0; i < N; i++) {
            j = TF[j, txt[i]];
            if (j == M)
                Console.WriteLine("\n pattern found at index %d", i - M + 1);
        }
    }

    /* Driver program to test above function */

    int Main()
    {
        string txt = "GEEKS FOR GEEKS";
        string pat = "GEEKS";
        Search(pat, txt);
        return 0;
    }
}