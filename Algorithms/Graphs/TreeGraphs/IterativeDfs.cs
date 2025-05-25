namespace Algorithms.Graphs;

public partial class TreeGraph
{
    public void IterativeDfs(
        Action<int> Before = null,
        Action<int, int> ProcessingChild = null,
        Action<int> After = null
    )
    {
        int size = TreeSize;
        for (int iu = 0; iu < size; iu++) {
            int u = Trace[iu];
            Before?.Invoke(u);

            int lca = iu + 1 < size ? Parent[Trace[iu + 1]] : -1;
            for (int p; u != lca; u = p) {
                After?.Invoke(u);
                p = Parent[u];
                if (p >= 0)
                    ProcessingChild?.Invoke(p, u);
            }
        }
    }
}