namespace Algorithms.Graphs;

public class OfflineDfs
{
    int q;
    List<int>[] g;
    Action[] Actions;
    TreeGraph tree;

    public OfflineDfs(int queries)
    {
        q = queries;
        g = new List<int>[q];
        for (int i = 0; i < g.Length; i++)
            g[i] = new List<int>();
        Actions = new Action[q];
    }

    public void AddAction(int id, int previous, Action action)
    {
        Actions[id] = action;
        if (previous >= 0)
            g[previous].Add(id);
    }

    public void Run(int root, Func<int> getTime, Action<int> setTime)
    {
        tree = new TreeGraph(g, root);
        int prev = tree.Trace[0];
        var stack = new int[g.Length];
        for (int iu = 0; iu < tree.TreeSize; iu++)
        {
            int u = tree.Trace[iu];
            var lca = tree.Lca(prev, u);
            if (lca != prev) setTime(stack[lca]);
            Actions[u]?.Invoke();
            stack[u] = getTime();
            prev = u;
        }
    }

    public void Run(int root, Action<int> rollback)
    {
        tree = new TreeGraph(g, root);
        for (int iu = 1; iu < tree.TreeSize; iu++)
        {
            int prev = tree.Trace[iu - 1];
            int u = tree.Trace[iu];
            var lca = tree.Lca(prev, u);
            foreach (var v in tree.Query(prev, lca))
                for (int i = v.NodeIndex; i < v.HeadIndex; i++)
                {
                    var x = tree.Trace[i];
                    if (x != lca) rollback(x);
                }

            Actions[u]?.Invoke();
        }
    }
}