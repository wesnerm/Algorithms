namespace Algorithms.Graphs;

public static class MaxBartiteMatching
{
    // This code performs maximum bipartite matching.
    //
    // Running time: O(|E| |V|) -- often much faster in practice
    //
    // INPUT: w[i][j] = edge between row node i and column node j
    // OUTPUT: mr[i] = assignment for row node i, -1 if unassigned
    // mc[j] = assignment for column node j, -1 if unassigned
    // function returns number of matches made

    public static bool FindMatch(int i, List<int>[] w, int[] mr, int[] mc, BitArray seen)
    {
        if (w[i] != null)
            foreach (int j in w[i])
                if (!seen[j]) {
                    seen[j] = true;
                    if (mc[j] < 0 || FindMatch(mc[j], w, mr, mc, seen)) {
                        mr[i] = j;
                        mc[j] = i;
                        return true;
                    }
                }

        return false;
    }

    public static int BipartiteMatching(List<int>[] w, int m, out int[] mr, out int[] mc)
    {
        mr = new int[w.Length];
        mc = new int[m + 1];

        for (int i = 1; i < mr.Length; i++)
            mr[i] = -1;

        for (int i = 1; i < mc.Length; i++)
            mc[i] = -1;

        int ct = 0;
        var seen = new BitArray(m + 1);
        for (int i = 1; i < w.Length; i++) {
            seen.SetAll(false);
            if (FindMatch(i, w, mr, mc, seen)) ct++;
        }

        return ct;
    }

    public static bool FindMatch(int i, bool[,] w, int[] mr, int[] mc, BitArray seen)
    {
        int m = w.GetLength(1);
        for (int j = 1; j < m; j++)
            if (w[i, j] && !seen[j]) {
                seen[j] = true;
                if (mc[j] < 0 || FindMatch(mc[j], w, mr, mc, seen)) {
                    mr[i] = j;
                    mc[j] = i;
                    return true;
                }
            }

        return false;
    }

    public static int BipartiteMatching(bool[,] w, out int[] mr, out int[] mc)
    {
        mr = new int[w.GetLength(0)];
        mc = new int[w.GetLength(1)];

        for (int i = 1; i < mr.Length; i++)
            mr[i] = -1;

        for (int i = 1; i < mc.Length; i++)
            mc[i] = -1;

        int ct = 0;
        var seen = new BitArray(w.GetLength(1));
        for (int i = 1; i < w.GetLength(0); i++) {
            seen.SetAll(false);
            if (FindMatch(i, w, mr, mc, seen)) ct++;
        }

        return ct;
    }
}