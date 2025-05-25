using static System.Math;

namespace Algorithms.Graphs;

// Incremental depth calculations: Depth += 2 - Distance(u_i, u_(i+1))

// This implementation has a number of advantages over the previous implementation
// It doesn't change the order of the adjacency list, is implemented in safe code,
// uses fewer lines, and reduces memory usage.

public class TreeGraphImproved
{
    public struct Node
    {
        public int Parent, Index, Size, Head;
    }

    #region Variables

    public List<int>[] Graph;
    public int[] Trace;
    public int TreeSize;
    Node[] node;

    public int Root => Trace[0];

    public int Index(int v) => node[v].Index;

    public int Size(int v) => node[v].Size;

    public int Parent(int v) => node[v].Parent;

    public int Head(int v) => node[v].Head;

    public int Heavy(int v) => node[v].Size > 1 ? Trace[node[v].Index + 1] : -1;

    #endregion

    #region Construction

    public TreeGraphImproved(List<int>[] g, int root = 0)
    {
        Rebuild(g, root);
    }

    public void Rebuild(List<int>[] g, int root)
    {
        if (node?.Length >= g.Length) {
            Array.Clear(node, 0, g.Length);
        } else {
            int n = Max(g.Length, (Trace?.Length ?? 0) * 2);
            node = new Node[n];
            Trace = new int[n];
        }

        Graph = g;
        Build(root);
    }

    void Build(int r)
    {
        for (int iu = Dfs(r) - 1; iu >= 0; iu--) {
            int u = Trace[iu], p = Parent(u), size = 1;
            List<int> adj = Graph[u];
            for (int i = adj.Count - 1, maxSize = 0; i >= 0; i--) {
                int v = adj[i];
                if (v == p) continue;
                int vSize = Size(v);
                size += vSize;
                if (vSize < maxSize) continue;
                maxSize = vSize;
                node[u].Head = i; // heavy
            }

            node[u].Size = size;
        }

        for (int iu = 0, treeSize = TreeSize = Dfs(r); iu < treeSize; iu++) {
            int u = Trace[iu], p = Parent(u);
            node[u].Index = iu;
            node[u].Head = p < 0 || Index(p) != iu - 1 ? u : Head(p);
        }
    }

    int Dfs(int r)
    {
        int[] trace = Trace;
        int stackSize = trace.Length, treeSize = 0;
        trace[--stackSize] = r;
        node[r].Parent = -1;
        while (stackSize < trace.Length) {
            int u = trace[treeSize++] = trace[stackSize++];
            List<int> adj = Graph[u];
            int p = Parent(u), heavy = Head(u) > 0 ? adj[Head(u)] : -1;
            for (int iv = adj.Count - 1; iv >= 0; iv--) {
                int v = adj[iv];
                if (v == p || v == heavy) continue;
                trace[--stackSize] = v;
                node[v].Parent = u;
            }

            if (heavy < 0) continue;
            trace[--stackSize] = heavy;
            node[heavy].Parent = u;
        }

        return treeSize;
    }

    #endregion

    #region LCA

    public int Lca(int x, int y)
    {
        int ix = Index(x);
        int diff = Index(y) - ix;
        if (diff < Size(x))
            return diff < 0 ? Lca(y, x) : x;

        do
            y = Parent(Head(y));
        while (Index(y) > ix);

        return y;
    }

    public int Distance(int x, int y)
    {
        int dist = 0, hx = Head(x), ix = Index(x);
        for (int iy = Index(y); ix <= iy; iy = Index(y)) {
            int hy = Head(y);
            if (hx == hy) return dist + iy - ix;
            dist += 1 + iy - Index(hy);
            y = Parent(hy);
        }

        return dist + Distance(y, x); // Max of 3 nested calls
    }

    public int Ancestor(int x, int v)
    {
        while (x >= 0) {
            int position = Index(x) - Index(Head(x));
            if (v <= position) return Trace[Index(x) - v];
            v -= position + 1;
            x = Parent(Head(x));
        }

        return x;
    }

    public int Advance(int u, int v)
    {
        int iv = Index(v);
        int ich = Index(u) + 1;
        if (unchecked((uint)(iv - ich) >= (uint)(Size(u) - 1)))
            return Parent(u); // Choose parent

        int ch = Trace[ich];
        ich += Size(ch);
        if (iv < ich) return ch; // Choose first child -- largest

        ch = Trace[ich];
        ich += Size(ch);
        if (iv < ich) return ch; // Choose second child (optional)

        int last = v;
        while (v != u) {
            last = Head(v);
            v = Parent(last);
        }

        return last;
    }

    /// <summary>
    ///     Computes the intersection of paths of pairwise vertices
    ///     Also, computes the LCA of one pair when the other pair is the root
    ///     Lca(p1,p2,p3)=p2 iff p2 is in path of p1 and p3
    /// </summary>
    public int Intersect(int p1, int p2, int p3)
    {
        if (Index(p1) > Index(p2)) Swap(ref p1, ref p2);
        if (Index(p2) > Index(p3)) Swap(ref p2, ref p3);
        if (Index(p1) > Index(p2)) Swap(ref p1, ref p2);

        if (unchecked((uint)(Index(p3) - Index(p2)) < (uint)Size(p2)))
            return p2;

        int lca = Lca(p1, p2);
        return unchecked((uint)(Index(p3) - Index(lca)) >= (uint)Size(lca))
            ? lca
            : Lca(p2, p3);
    }

    #endregion

    #region HLD

    readonly List<Segment> segs = new(32);
    readonly Segment[] segs2 = new Segment[32];

    public List<Segment> Query(int x, int y, bool edges = false)
    {
        // up segs in ascending order, down segs in descending order
        segs.Clear();

        int crev = 0;
        for (int rx = Head(x), ry = Head(y); rx != ry;)
            if (Index(rx) > Index(ry)) {
                segs.Add(new Segment(Index(rx), Index(x), 1));
                x = Parent(rx);
                rx = Head(x);
            } else {
                segs2[crev++] = new Segment(Index(ry), Index(y), -1);
                y = Parent(ry);
                ry = Head(y);
            }

        int lcaIndex = Min(Index(x), Index(y));
        int nodeIndex = Max(Index(x), Index(y));
        if (edges == false || lcaIndex < nodeIndex)
            segs.Add(new Segment(lcaIndex + (edges ? 1 : 0), nodeIndex,
                nodeIndex == Index(x) ? 1 : -1));

        while (crev > 0) segs.Add(segs2[--crev]);
        return segs;
    }

    public struct Segment
    {
        public int HeadIndex, NodeIndex, Dir;

        public Segment(int headIndex, int nodeIndex, int dir)
        {
            HeadIndex = headIndex;
            NodeIndex = nodeIndex;
            Dir = dir;
        }
    }

    #endregion

    #region MOS

    public void AddTask(List<Task> tasks, int x, int y, Action action)
    {
        tasks.Add(new Task { Start = x, End = y, Action = action });
    }

    void SortTasks(List<Task> tasks)
    {
        int[] euler = new int[TreeSize];
        for (int i = 1; i < TreeSize; i++)
            euler[i] = euler[i - 1] + Distance(Trace[i - 1], Trace[i]);

        foreach (Task t in tasks) {
            if (Index(t.Start) > Index(t.End)) Swap(ref t.Start, ref t.End);
            int s = t.Start, e = t.End;
            t.SI = euler[Index(s)];
            t.EI = euler[Index(e)];
            if (unchecked((uint)(Index(e) - Index(s)) >= (uint)Size(s)))
                t.SI += Size(s) * 2 - 1; // -2 for nodal tours 
        }

        int r = (int)Ceiling(Sqrt(2 * TreeSize));
        tasks.Sort((x, y) => x.SI / r == y.SI / r ? x.EI - y.EI : x.SI - y.SI);
    }

    public void Execute(List<Task> tasks, Action<int> flip)
    {
        SortTasks(tasks);
        int s = tasks.Count > 0 ? tasks[0].Start : Trace[0], e = s;
        flip(s);

        foreach (Task task in tasks) {
            s = Adjust(e, s, task.Start, flip);
            e = Adjust(s, e, task.End, flip);
            task.Action();
        }
    }

    int Adjust(int start, int oldEnd, int newEnd, Action<int> flip)
    {
        if (oldEnd == newEnd) return newEnd;
        foreach (Segment seg in Query(oldEnd, newEnd))
            for (int i = seg.HeadIndex; i <= seg.NodeIndex; i++)
                flip(Trace[i]);
        flip(Intersect(start, oldEnd, newEnd));
        return newEnd;
    }

    public class Task
    {
        public Action Action;
        public int Start, End, SI, EI;

        public override string ToString() => $"[{Start},{End}]";
    }

    #endregion
}