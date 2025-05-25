namespace Algorithms.Graphs;

public class CentroidDecomposition
{
    #region Construction

    public CentroidDecomposition(List<int>[] graph,
        bool buildDistances = false)
    {
        int n = graph.Length;
        sizes = new int[n];
        sep = new byte[n];
        Graph = graph;
        Parents = new int[n];
        Queue = new int[n];
        CdParents = new int[n];
        // Port = new int[n];
        Dist = buildDistances ? new int[n] : null;
    }

    #endregion

    #region Variables

    readonly int[] sizes;
    readonly byte[] sep;

    #endregion

    #region Properties

    public int CdDepth(int node) => 255 - sep[node];

    public readonly List<int>[] Graph;
    public readonly int[] Queue;
    public readonly int[] Parents;
    public readonly int[] CdParents;

    public readonly int[] Dist;

    // public readonly int[] Port;
    public Action<int, int> Action;
    public int TreeSize;

    #endregion

    #region Public Methods

    public void Decompose(int root)
    {
        Decompose(root, -1);
    }

    public void RebuildCentroidRegion(int u)
    {
        if (Dist != null) Dist[u] = 0;
        BuildTree(u, sep[u], 0);
    }

    //public void RebuildCentroidChildRegion(int u)
    //{
    //    if (Dist != null) Dist[u] = 0;
    //    BuildTree(Port[u], sep[u], 0);
    //}

    public int CentroidLca(int u, int v)
    {
        // O( lg(n) ) because centroid tree is perfectly balanced
        // Returns a w, which is a centroid in path of u & v 
        //  and contains both in its region

        while (u != v)
            if (sep[u] < sep[v])
                u = CdParents[u];
            else
                v = CdParents[v];
        return u;
    }

    #endregion

    #region Private Methods

    int BuildTree(int root, int level, int offset)
    {
        Parents[root] = -1;
        Queue[0] = root;
        int treeSize = 1;
        for (int p = 0; p < treeSize; p++) {
            int cur = Queue[p];
            int par = Parents[cur];
            foreach (int child in Graph[cur])
                if (par != child && sep[child] <= level) {
                    Queue[treeSize++] = child;
                    Parents[child] = cur;
                }
        }

        TreeSize = treeSize;
        if (Dist != null)
            RebuildDistances(offset);
        return treeSize;
    }

    void RebuildDistances(int offset = 1)
    {
        int treeSize = TreeSize;
        Dist[0] = offset;
        for (int iu = 1; iu < treeSize; iu++) {
            int u = Queue[iu];
            Dist[u] = Dist[Parents[u]] + 1;
        }
    }

    void Decompose(int root, int centroidParent, byte level = 255)
    {
        BuildTree(root, level, 1);
        int centroid = FindCentroid(centroidParent);

        CdParents[centroid] = centroidParent;
        // Port[centroid] = root;
        sep[centroid] = level--;

        Action?.Invoke(centroid, centroidParent);

        foreach (int e in Graph[centroid])
            if (sep[e] <= level)
                Decompose(e, centroid, level);

        //if (PostAction != null)
        //{
        //    RebuildCentroidRegion(centroid);
        //    PostAction(centroid, centroidParent);
        //}
    }

    int FindCentroid(int centroidParent)
    {
        int treeSize = TreeSize;
        if (centroidParent >= 0) sizes[centroidParent] = 0;
        for (int iu = treeSize - 1; iu >= 0; iu--) {
            int u = Queue[iu];
            sizes[u] = 1;
            foreach (int e in Graph[u])
                if (Parents[u] != e)
                    sizes[u] += sizes[e];

            if (sizes[u] << 1 >= treeSize)
                return u;
        }

        return Queue[0];
    }

    #endregion
}