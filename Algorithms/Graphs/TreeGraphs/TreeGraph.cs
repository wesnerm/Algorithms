using static System.Math;

namespace Algorithms.Graphs;

// MOS: https://www.hackerrank.com/contests/w34/challenges/path-statistics/submissions/code/1302610431

// Incremental depth calculations: Depth += 2 - Distance(u_i, u_(i+1))

public partial class TreeGraph
{
    #region Variables

    public List<int>[] Graph;
    public int[] Sizes;
    public int[] Begin;
    public int[] Parent;
    public int[] Head;
    public int[] Trace;
    public int TreeSize;
    public int Root => Trace[0];

    public int End(int v) => Begin[v] + Sizes[v] - 1;

    #endregion

    #region Construction

    public TreeGraph(List<int>[] graph, int root = 0, int avoid = -1)
    {
        Graph = graph;
        int n = graph.Length;
        Sizes = new int[n];
        Begin = new int[n];
        Head = new int[n];
        Trace = new int[n];
        Parent = new int[n];
        Build(root, avoid);
    }

    void Build(int r = 0, int avoid = -1)
    {
        Span<int> stack = stackalloc int[Graph.Length + 1];
        int stackSize = 0, treeSize = 0;
        stack[stackSize++] = r;
        Parent[r] = -1;
        while (stackSize > 0) {
            int u = stack[--stackSize];
            Trace[treeSize++] = u;
            Head[u] = u;
            Sizes[u] = 1;
            foreach (int v in Graph[u]) {
                if (Sizes[v] > 0 || v == avoid) continue;
                Parent[v] = u;
                stack[stackSize++] = v;
            }
        }

        for (int iu = treeSize - 1; iu >= 0; iu--) {
            int u = Trace[iu];
            int p = Parent[u];
            List<int> neighbors = Graph[u];
            int maxSize = 0;
            for (int i = 0; i < neighbors.Count; i++) {
                int v = neighbors[i];
                if (v != p && v != avoid) {
                    Sizes[u] += Sizes[v];
                    if (Sizes[v] > maxSize) {
                        maxSize = Sizes[v];
                        neighbors[i] = neighbors[0];
                        neighbors[0] = v;
                    }
                }
            }
        }

        stackSize = treeSize = 0;
        stack[stackSize++] = r;
        while (stackSize > 0) {
            int u = stack[--stackSize];
            Begin[u] = treeSize;
            Trace[treeSize++] = u;
            int heavy = -1;
            List<int> children = Graph[u];
            for (int iv = children.Count - 1; iv >= 0; iv--) {
                int v = children[iv];
                if (v != Parent[u] && v != avoid)
                    stack[stackSize++] = heavy = v;
            }

            if (heavy >= 0) Head[heavy] = Head[u];
        }

        TreeSize = treeSize;
    }

    #endregion

    #region LCA

    public int Lca(int x, int y)
    {
        int ix = Begin[x];
        int diff = Begin[y] - ix;
        if (diff < Sizes[x])
            return diff >= 0 ? x : Lca(y, x);

        do
            y = Parent[Head[y]];
        while (Begin[y] > ix);

        return y;
    }

    public int Distance(int x, int y)
    {
        int dist = 0, hx = Head[x], ix = Begin[x];
        for (int iy = Begin[y]; ix <= iy; iy = Begin[y]) {
            int hy = Head[y];
            if (hx == hy) return dist + iy - ix;
            dist += 1 + iy - Begin[hy];
            y = Parent[hy];
        }

        return dist + Distance(y, x); // Max of 3 nested calls
    }

    public int Ancestor(int x, int v)
    {
        while (x >= 0) {
            int position = Begin[x] - Begin[Head[x]];
            if (v <= position) return Trace[Begin[x] - v];
            v -= position + 1;
            x = Parent[Head[x]];
        }

        return x;
    }

    public int Advance(int u, int v)
    {
        int iv = Begin[v];
        int ich = Begin[u] + 1;
        if (unchecked((uint)(iv - ich) >= (uint)(Sizes[u] - 1)))
            return Parent[u]; // Choose parent

        int ch = Trace[ich];
        ich += Sizes[ch];
        if (iv < ich) return ch; // Choose first child -- largest

        ch = Trace[ich];
        ich += Sizes[ch];
        if (iv < ich) return ch; // Choose second child (optional)

        int last = v;
        while (v != u) {
            last = Head[v];
            v = Parent[last];
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
        if (Begin[p1] > Begin[p2]) Swap(ref p1, ref p2);
        if (Begin[p2] > Begin[p3]) Swap(ref p2, ref p3);
        if (Begin[p1] > Begin[p2]) Swap(ref p1, ref p2);

        if (unchecked((uint)(Begin[p3] - Begin[p2]) < (uint)Sizes[p2]))
            return p2;

        int lca = Lca(p1, p2);
        return unchecked((uint)(Begin[p3] - Begin[lca]) >= (uint)Sizes[lca])
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
        for (int rx = Head[x], ry = Head[y]; rx != ry;)
            if (Begin[rx] > Begin[ry]) {
                segs.Add(new Segment(Begin[rx], Begin[x], 1));
                x = Parent[rx];
                rx = Head[x];
            } else {
                segs2[crev++] = new Segment(Begin[ry], Begin[y], -1);
                y = Parent[ry];
                ry = Head[y];
            }

        int lcaIndex = Min(Begin[x], Begin[y]);
        int nodeIndex = Max(Begin[x], Begin[y]);
        if (edges == false || lcaIndex < nodeIndex)
            segs.Add(new Segment(lcaIndex + (edges ? 1 : 0), nodeIndex,
                nodeIndex == Begin[x] ? 1 : -1));

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
            if (Begin[t.Start] > Begin[t.End]) Swap(ref t.Start, ref t.End);
            int s = t.Start, e = t.End;
            t.SI = euler[Begin[s]];
            t.EI = euler[Begin[e]];
            if (unchecked((uint)(Begin[e] - Begin[s]) >= (uint)Sizes[s]))
                t.SI += Sizes[s] * 2 - 1; // -2 for nodal tours 
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