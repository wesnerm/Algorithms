using static System.Math;

namespace Algorithms.Graphs;

public partial class TreeGraph
{
    #region DSU On Tree

    public void DisjointSetUnion(
        Action<int> add,
        Action<int> remove,
        Action<int> compute)
    {
        int last = TreeSize - 1;
        for (int iv = last; iv >= 0; iv--) {
            int v = Trace[iv];
            int iw = iv + 1;
            if (Sizes[v] > 1)
                for (int iu = iw + Sizes[Trace[iw]], limit = iv + Sizes[v];
                     iu < limit;
                     iu++)
                    add(Trace[iu]);
            else if (iv < last)
                for (int iu = iw, limit = iu + Sizes[Trace[iu]];
                     iu < limit;
                     iu++)
                    remove(Trace[iu]);
            add(v);
            compute(v);
        }
    }

    #endregion

    #region Dynamic Programming

    public long[] NodeSum(Func<int, long> func, long[] reuseBuffer = null)
    {
        List<int>[] g = Graph;
        long[] result = reuseBuffer ?? new long[g.Length];
        int[] queue = Trace;

        for (int i = TreeSize - 1; i >= 0; i--) {
            int u = queue[i];
            int p = Parent[u];

            long sum = func(u);
            foreach (int v in g[u]) {
                if (v == p) continue;
                sum += result[v];
            }

            result[u] = sum;
        }

        return result;
    }

    public long[] NodeMin(Func<int, long> func, bool minimize = true, long[] reuseBuffer = null)
    {
        List<int>[] g = Graph;
        long[] result = reuseBuffer ?? new long[g.Length];
        int[] queue = Trace;

        for (int i = TreeSize - 1; i >= 0; i--) {
            int u = queue[i];
            int p = Parent[u];

            long min = func(u);
            foreach (int v in g[u]) {
                if (v == p) continue;
                min = minimize
                    ? Min(min, result[v])
                    : Max(min, result[v]);
            }

            result[u] = min;
        }

        return result;
    }

    public long[] NodeSet(Func<int, long> func, long[] reuseBuffer = null)
    {
        List<int>[] g = Graph;
        long[] result = reuseBuffer ?? new long[g.Length];
        int[] queue = Trace;

        for (int i = TreeSize - 1; i >= 0; i--) {
            int u = queue[i];
            result[u] = func(u);
        }

        return result;
    }

    public void MinimizeNodeStatistics(long[] childStats, bool minimize = true)
    {
        List<int>[] g = Graph;
        int[] parents = Parent;
        int[] queue = Trace;
        for (int iu = TreeSize - 1; iu >= 0; iu--) {
            int u = queue[iu];
            int p = parents[u];
            long min = childStats[u];
            foreach (int v in g[u])
                if (v != p)
                    min = minimize
                        ? Min(min, childStats[v])
                        : Max(min, childStats[v]);

            childStats[u] = min;
        }
    }

    public void MinimizeParentStatistics(long[] childStats, long[] parentStats,
        bool merge = true,
        bool minimize = true)
    {
        int treeSize = TreeSize;
        List<int>[] g = Graph;
        int[] queue = Trace;
        var empty = ExcludedExtrema.Empty;

        for (int iu = 0; iu < treeSize; iu++) {
            int u = queue[iu];
            ExcludedExtrema extrema = empty;
            extrema.Add(parentStats[u]);

            int p = Parent[u];
            foreach (int v in g[u])
                if (v != p)
                    extrema.Add(childStats[v]);

            foreach (int v in g[u])
                if (v != p)
                    parentStats[v] = minimize
                        ? Min(parentStats[v], extrema.ExcludedMinimum(childStats[v]))
                        : Max(parentStats[v], extrema.ExcludedMaximum(childStats[v]));

            if (merge)
                parentStats[u] = minimize ? extrema.Minimum : extrema.Maximum;
        }

        // var offBulbs = NodeSum(u => lit[u] ? 0 : 1);
        // var totals = NodeSum(x => 1);
        // var odd = NodeSet(v => (offBulbs[v] & 1) != 0 ? totals[v] : int.MaxValue);
        // var iodd = NodeSet(v => (offBulbs[0] - offBulbs[v] & 1) == 1 ? totals[0] - totals[v] : int.MaxValue);
        // MinimizeNodeStatistics(odd);
        // MinimizeParentStatistics(odd, iodd, true);
        // for (int u = 0; u < g.Length; u++)
        //     WriteLine(Max(0, g.Length - iodd[u]));

        //var dp = t.NodeAgg(x => a[x], (x, y) => x | y);
        //var dp2 = t.NodeSet(x => x == 0 ? 0 : a[t.Parents[x]]);
        //int count = 0;
        //t.MinimizeParentStatistics(dp, dp2,false);
        //for (int i = 1; i < n; i++)
        //    if (dp[i] == dp2[i]) count++;
        //WriteLine(count);
    }

    #endregion
}