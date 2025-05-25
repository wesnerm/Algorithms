namespace Algorithms.Graphs;

public class TreeGraphBfs
{
    public List<int>[] Graph;
    public int[] Parents, Queue, Depths;
    public int Separator;
    public int TreeSize;

    public TreeGraphBfs(List<int>[] g, int root = 0, int avoid = -1)
    {
        Graph = g;
        if (root >= 0) Init(root, avoid);
    }

    public int Root => Queue[0];

    public void Init(int root, int avoid = -1)
    {
        List<int>[] g = Graph;
        int n = g.Length;
        Separator = avoid;

        Queue = new int[n];
        Parents = new int[n];
        Depths = new int[n];

        for (int i = 0; i < Parents.Length; i++)
            Parents[i] = -1;

        Queue[0] = root;

        int treeSize = 1;
        for (int p = 0; p < treeSize; p++) {
            int cur = Queue[p];
            int par = Parents[cur];
            foreach (int child in g[cur])
                if (child != par && child != avoid) {
                    Queue[treeSize++] = child;
                    Parents[child] = cur;
                    Depths[child] = Depths[cur] + 1;
                }
        }

        TreeSize = treeSize;
    }

    public int[] Sizes()
    {
        int[] sizes = new int[Graph.Length];
        for (int i = TreeSize - 1; i >= 0; i--) {
            int current = Queue[i];
            sizes[current] = 1;
            foreach (int e in Graph[current])
                if (Parents[current] != e)
                    sizes[current] += sizes[e];
        }

        return sizes;
    }
}