namespace Algorithms.Graphs.TreeGraphs;

public class TreeDistances
{
    public int[] MaxDist;
    public int[] Height, ParentHeight;

    public TreeDistances(TreeGraph t, List<int>[] g)
    {
        int n = t.Trace.Length;
        int treeSize = t.TreeSize;
        var queue = t.Trace;
        Height = new int[n];
        ParentHeight = new int[n];
        for (int iu = t.TreeSize; iu >= 0; iu--)
        {
            int u = t.Trace[iu];
            int ht = -1;
            int p = t.Parent[u];
            foreach (var v in g[u])
            {
                if (v == p) continue;
                ht = Math.Max(Height[v], ht);
            }

            Height[u] = 1 + ht;
        }

        MaxDist = new int[n + 1];
        for (int iu = 0; iu < treeSize; iu++)
        {
            var u = queue[iu];
            int max = ParentHeight[u], max2 = 0;

            var p = t.Parent[u];
            foreach (var v in g[u])
                if (v != p)
                {
                    var h = Height[v];
                    if (h > max2)
                    {
                        max2 = h;
                        if (max2 > max)
                            Swap(ref max, ref max2);
                    }
                }

            foreach (var v in g[u])
                if (v != p)
                {
                    var h2 = Height[v] != max ? max : max2;
                    ParentHeight[v] = 1 + h2;
                }

            MaxDist[u] = Math.Max(ParentHeight[u] + 1, Height[u]);
        }
    }

}
