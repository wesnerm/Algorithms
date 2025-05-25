using Algorithms.Collections;

namespace Algorithms.Graphs;

public class TarjanLca
{
    // Tarjan Offline LCA Algorithm steps from CLRS, Section-21-3, Pg 584, 2nd /3rd edition.

    readonly List<Task> Tasks = new();

    public TarjanLca(TreeGraph tree)
    {
        var uf = new UnionFind(tree.Trace.Length);
        int[] anc = new int[tree.Trace.Length];
        int size = tree.TreeSize;
        int v = -1;
        for (int iv = 0; iv < size; iv++) {
            int u = v;
            v = tree.Trace[iv];
            int p = tree.Parent[v];

            // The action happens on the leaves
            if (p >= 0) {
                Debug.Assert(p == tree.Lca(u, v));
                for (int pu; u != p; u = pu) {
                    pu = tree.Parent[u];
                    uf.Union(u, pu);
                }

                anc[uf.Find(p)] = p;
            }

            anc[v] = v;
            // Optimize this
            foreach (Task task in Tasks)
                if (task.V == v) {
                    Debug.Assert(task.U <= v);
                    task.Action(task, anc[uf.Find(task.U)]);
                }
        }
    }

    public void AddTask(int u, int v, Action<Task, int> action)
    {
        if (u > v) Swap(ref u, ref v);
        Tasks.Add(new Task { U = u, V = v, Action = action });
    }

    public class Task
    {
        public Action<Task, int> Action;
        public int U, V;
    }
}