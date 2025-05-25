namespace Algorithms.Graphs.Connectivity;

/// <summary>
///     Depth First Tour Tree, partly based on paper https://arxiv.org/pdf/1502.05292.pdf
/// </summary>
public class Euler
{
    #region Properties

    public long Value {
        get
        {
            Splay();
            return value;
        }
        set
        {
            Splay();
            this.value = value;
            Up();
            ;
        }
    }

    #endregion

    #region Constructor

    public int Id;
    int Size;
    public int Rep;
    readonly Euler End;
    Euler Parent;
    Euler Left;
    Euler Right;
    long value;
    public long Sum;
    long lazyAdd;
    bool cover;

    #endregion

    #region Constructor

    Euler() { }

    public Euler(int id, int value)
    {
        var node2 = new Euler { Id = id, Parent = this };
        Id = id;
        Size = 1;
        value = value;
        Sum = value;
        Rep = id;
        End = node2;
        Right = node2;
    }

    #endregion

    #region Methods

    public void Cut()
    {
        Euler right = Split(End, false);
        Euler left = Split(this, true);
        Join(left, right);
    }

    public void Link(Euler parent)
    {
        Euler right = Split(parent, false);
        Join(parent, this);
        Join(End, right); // Join(parent, right) should also work
    }

    public bool IsConnected(Euler euler)
    {
        euler.Splay();
        return Splay() == euler;
    }

    public Euler Splay()
    {
        Euler last = this;
        Down();
        while (Parent != null) {
            last = Parent;
            RotateUp();
        }

        Up();
        //Down();
        return last;
    }

    void RotateUp()
    {
        Euler? p = Parent;
        Euler? g = p.Parent;

        g?.Down();
        p?.Down();
        Down();

        Parent = g;
        if (g != null) {
            if (g.Left == p) g.Left = this;
            else g.Right = this;
        }

        p.Parent = this;
        if (p.Left == this) {
            p.Left = Right;
            if (p.Left != null) p.Left.Parent = p;
            Right = p;
        } else {
            p.Right = Left;
            if (p.Right != null) p.Right.Parent = p;
            Left = p;
        }

        p?.Up();
        Up();
        g?.Up();
    }

    public void Down()
    {
        if (cover) {
            cover = false;
            Left?.Cover(lazyAdd);
            Right?.Cover(lazyAdd);
            lazyAdd = 0;
        } else if (lazyAdd != 0) {
            Left?.Add(lazyAdd);
            Right?.Add(lazyAdd);
            lazyAdd = 0;
        }
    }

    public void Cover(long value)
    {
        if (End != null) this.value = value;
        lazyAdd = value;
        Sum = value * Size;
        cover = true;
    }

    public void Add(long value)
    {
        if (End != null) this.value += value;
        Sum += value * Size;
        lazyAdd += value;
    }

    public void Up()
    {
        if (cover || lazyAdd != 0)
            Down();

        int size = End != null ? 1 : 0;
        long sum = value;
        int rep = Id;

        if (Left != null) {
            size += Left.Size;
            sum += Left.Sum;
            if (Left.Rep < rep) rep = Left.Rep;
        }

        if (Right != null) {
            size += Right.Size;
            sum += Right.Sum;
            if (Right.Rep < rep) rep = Right.Rep;
        }

        Size = size;
        Sum = sum;
        Rep = rep;
    }

    static Euler Split(Euler node, bool left)
    {
        node.Splay();

        Euler child;
        if (left) {
            child = node.Left;
            node.Left = null;
        } else {
            child = node.Right;
            node.Right = null;
        }

        if (child != null) child.Parent = null;
        node.Up();
        return child;
    }

    static Euler Join(Euler left, Euler right)
    {
        if (left == null || right == null)
            return left ?? right;

        Euler edge = Last(left);
        edge.Splay();

        right.Splay();
        edge.Right = right;
        right.Parent = edge;
        edge.Up();
        return edge;
    }

    static bool Precedes(Euler u, Euler v)
    {
        Euler? after = Split(u, false);
        bool answer = after != null && u == v.Splay();
        Join(u, after);
        return answer;
    }

    static Euler First(Euler node)
    {
        while (node.Left != null) node = node.Left;
        return node;
    }

    static Euler Last(Euler node)
    {
        while (node.Right != null) node = node.Right;
        return node;
    }

    static Euler SplayNext(Euler node)
    {
        if (node != null) {
            Euler? tmp = node.Right;
            if (tmp != null)
                return First(tmp);

            do {
                tmp = node;
                node = node.Parent;
            } while (node != null && node.Left != tmp);
        }

        return node;
    }

    static Euler SplayPrev(Euler node)
    {
        if (node != null) {
            Euler? tmp = node.Left;
            if (tmp != null)
                return Last(tmp);

            do {
                tmp = node;
                node = node.Parent;
            } while (node != null && node.Right != tmp);
        }

        return node;
    }

    static Euler SplayRoot(Euler node)
    {
        if (node == null)
            return null;

        while (true) {
            Euler? parent = node.Parent;
            if (parent == null)
                return node;
            node = parent;
        }
    }

    /// <summary>
    ///     Ensures that tour consists of one tree identified by root
    /// </summary>
    /// <param name="root"></param>
    static void Splice(Euler root)
    {
        Split(root.End, false);
        Split(root, true);
    }

    // Dft-To-Tree

    int DftToTree(Euler dft, int n, bool dual = false)
    {
        var stack = new Stack<int>();
        var graph = new List<int>[n];
        int root = -1;
        int p = -1;
        for (Euler? node = First(dft); node != null; node = SplayNext(node)) {
            int u = node.Id;
            if (node.End != null) {
                if (stack.Count != 0) {
                    graph[p].Add(u);
                    if (dual) graph[u].Add(p);
                } else {
                    root = u;
                }

                stack.Push(u);
                p = u;
            } else {
                stack.Pop();
                p = stack.Count > 0 ? stack.Peek() : -1;
            }
        }

        return root;
    }

    // Import-Tree

    // Export-Tree

    static void SplayErase(Euler u)
    {
        u.Splay();

        Euler? left = u.Left;
        if (left != null) {
            left.Parent = null;
            u.Left = null;
        }

        Euler? right = u.Right;
        if (right != null) {
            right.Parent = null;
            u.Right = null;
        }

        Euler node = Join(left, right);
        Euler? parent = u.Parent;
        if (parent != null) {
            if (u == parent.Left)
                parent.Left = node;
            else if (u == parent.Right)
                parent.Right = node;
            u.Parent = null;
        }
    }

    public void Condense()
    {
        SplayErase(this);
        SplayErase(End);
    }

    public void Erase(Euler u)
    {
        Cut();
        Condense();
    }

    public static bool IsDescendant(Euler u, Euler v) => Precedes(v, u) && Precedes(u.End, v.End);

    public static IEnumerable<Euler> ListChildren(Euler v)
    {
        Euler current = SplayNext(v);
        while (current != v.End) {
            yield return current;
            current = SplayNext(current.End);
        }
    }

    #region Advanced

    public static Euler Root(Euler node) => First(SplayRoot(node));

    public static int DownSummary(int down1, int up1, int down2, int up2) =>
        down1 + (up1 + down2 > 0 ? 0 : down2 + up1);

    public static int UpSummary(int down1, int up1, int down2, int up2) => up2 + (up1 + down2 > 0 ? up1 + down2 : 0);

    public static void Summary(int down1, int up1, int down2, int up2, out int down, out int up)
    {
        if (up1 + down2 > 0) {
            down = down1;
            up = up1 + up2 + down2;
        } else {
            down = down1 + down2 + up1;
            up = up2;
        }
    }

    int upSum;
    int downSum;

    static Euler RecursiveParent(Euler node, int down, int up)
    {
        Debug.Assert(down == 0);
        Euler? left = node.Left;
        if (left != null) {
            int down2, up2;
            Summary(down, up, left.downSum, left.upSum, out down2, out up2);
            if (down2 < 0) return RecursiveParent(left, down, up);
            up = up2;
            down = down2;
        }

        Summary(down, up, node.downSum, node.upSum, out down, out up);
        if (down < 0) return node;
        return RecursiveParent(node.Right, down, up);
    }

    public static Euler TreeParent(Euler node)
    {
        Euler root = Root(node);
        if (node != root) {
            Euler after = Split(node.End, false);
            Euler closeParent = RecursiveParent(SplayRoot(after), 0, 0);
            Join(node.End, after);
            return closeParent; // Need to return the openParent
        }

        return null;
    }

    // Also, called Evert
    public static void Reroot(Euler v)
    {
        Euler? parent = TreeParent(v);
        if (parent != null) {
            v.Cut();
            Reroot(parent);
            parent.Link(v);
        }
    }

    public static Euler Lca(Euler u, Euler v)
    {
        if (IsDescendant(u, v)) return v;
        if (IsDescendant(v, u)) return u;
        if (Precedes(u, v)) return Lca(v, u);

        Euler predu = Split(u.End, true);
        Euler succv = Split(v.End, false);
        Euler rangeRoot = SplayRoot(v.End);
        int dftW = RangeLcaSummaryP(rangeRoot);
        Join(predu, u.End);
        Join(u.End, succv);
        Euler w = NodeAssociatedWith(dftW);
        return TreeParent(w);
    }

    public static Euler TreeRoot(Euler v)
    {
        Euler pred = Split(v.End, true);
        Euler rangeRoot = SplayRoot(pred);
        int dftW = RangeLcaSummaryP(rangeRoot);
        Join(pred, v.End);
        Euler w = NodeAssociatedWith(dftW);
        return w;
    }

    static int RangeLcaSummaryP(Euler v) => throw new NotImplementedException();

    static Euler NodeAssociatedWith(int p) => throw new NotImplementedException();

    #endregion

    #endregion
}