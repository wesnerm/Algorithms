using static System.Math;

namespace Algorithms.Graphs;

// http://codeforces.com/blog/entry/53170
// Combines LCA, EulerTour/DfsNumbering, and Hld

public class TreeGraphDfs
{
    public int[] Begin, End;
    public IList<int>[] Graph;
    public int[] Head;
    public int[] Parent, Depth;
    public int[] Queue;
    public int Root;
    public int[] Sizes;
    public int TreeSize;

    public TreeGraphDfs(IList<int>[] graph, int root)
    {
        Graph = graph;
        int n = graph.Length;
        Sizes = new int[n];
        Begin = new int[n];
        End = new int[n];
        Head = new int[n];
        Queue = new int[n];
        Parent = new int[n];
        Depth = new int[n];
        Root = root;
        DfsSz(root);
        DfsHld(root);
    }

    // TODO: Make iterative for performance reasons
    void DfsSz(int v = 0, int p = -1)
    {
        IList<int> list = Graph[v];
        int count = list.Count;
        Parent[v] = p;
        Depth[v] = p >= 0 ? Depth[p] + 1 : 0;
        for (int i = 0; i < count; i++) {
            int u = list[i];
            if (u == p) continue;
            DfsSz(u, v);
            Sizes[v] += Sizes[u];
            if (Sizes[u] > Sizes[list[0]]) {
                list[i] = list[0];
                list[0] = u;
            }
        }

        Sizes[v]++;
    }

    void DfsHld(int v = 0, int p = -1)
    {
        Queue[TreeSize] = v;
        Begin[v] = TreeSize++;
        IList<int> children = Graph[v];
        foreach (int u in children) {
            if (u == p) continue;
            Head[u] = u == children[0] ? Head[v] : u;
            DfsHld(u, v);
        }

        End[v] = TreeSize - 1;
    }

    // TODO: Depth information is really needed except for distance calcs
    // We could use Begin for all other purposes
    // Constant time LCA
    // Assume u has min Begin[u], then 
    // if v is in range of B[u] <=B[v]<=E[u], u is lca
    // else lca in T[m] where m is min of Begin[parent]

    #region LCA

    public int Lca(int x, int y)
    {
        for (int rx = Head[x], ry = Head[y]; rx != ry;)
            if (Depth[rx] > Depth[ry]) {
                x = Parent[rx];
                rx = Head[x];
            } else {
                y = Parent[ry];
                ry = Head[y];
            }

        return Begin[x] > Begin[y] ? y : x;
    }

    public int Ancestor(int x, int v)
    {
        while (x >= 0) {
            int position = Begin[x] - Begin[Head[x]];
            if (v <= position) return Queue[Begin[x] - v];
            v -= position + 1;
            x = Parent[Head[x]];
        }

        return x;
    }

    public int Distance(int u, int v) => Depth[u] + Depth[v] - 2 * Depth[Lca(u, v)];

    #endregion

    #region HLD

    readonly List<Segment> segs = new();

    public List<Segment> Query(int x, int y, bool edges = false)
    {
        // up segs in ascending order, down segs in descending order
        segs.Clear();

        for (int rx = Head[x], ry = Head[y]; rx != ry;)
            if (Depth[rx] > Depth[ry]) {
                segs.Add(new Segment(Begin[rx], Begin[x], true));
                x = Parent[rx];
                rx = Head[x];
            } else {
                segs.Add(new Segment(Begin[ry], Begin[y], false));
                y = Parent[ry];
                ry = Head[y];
            }

        int lcaIndex = Min(Begin[x], Begin[y]);
        int nodeIndex = Max(Begin[x], Begin[y]);
        if (edges == false || lcaIndex < nodeIndex)
            segs.Add(new Segment(lcaIndex,
                nodeIndex + (edges ? 1 : 0),
                nodeIndex == Begin[x]));

        return segs;
    }

    public struct Segment
    {
        public readonly int HeadIndex, NodeIndex;
        public readonly bool Up;

        public Segment(int headIndex, int nodeIndex, bool up)
        {
            HeadIndex = headIndex;
            NodeIndex = nodeIndex;
            Up = up;
        }

        public override string ToString() => $"{HeadIndex}->{NodeIndex}  Up={Up}";
    }

    #endregion
}