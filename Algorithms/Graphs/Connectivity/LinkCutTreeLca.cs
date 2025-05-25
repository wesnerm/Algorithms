namespace Algorithms.Graphs;

// This is for dynamic lca queries
public class LinkCutTreeLca
{
    #region Constructor

    public LinkCutTreeLca(int label = 0) => Label = label;

    #endregion

    #region Variables

    public int Label;
    public int Size;
    public LinkCutTreeLca Left;
    public LinkCutTreeLca Right;
    public LinkCutTreeLca Parent;

    #endregion

    #region Public Methods

    public LinkCutTreeLca Access()
    {
        LinkCutTreeLca last = null;
        for (LinkCutTreeLca? y = this; y != null; y = y.Parent) {
            y.Splay();
            y.Left = last;
            last = y;
        }

        Splay();
        return last;
    }

    public bool Connected(LinkCutTreeLca y) => FindRoot() == y.FindRoot();

    public bool Link(LinkCutTreeLca y)
    {
        if (Connected(y)) return false;
        Access();
        Debug.Assert(Right == null); //x._right==null <=> x is rootnode
        Parent = y;
        return true;
    }

    public void Cut()
    {
        Access();
        Debug.Assert(Right != null); //x._right==null <=> x is rootnode
        Right.Parent = null;
        Right = null;
    }

    public LinkCutTreeLca FindRoot()
    {
        LinkCutTreeLca x = this;
        x.Access();
        while (x.Right != null)
            x = x.Right;
        x.Splay();
        return x;
    }

    public LinkCutTreeLca Lca(LinkCutTreeLca y)
    {
        if (FindRoot() != y.FindRoot())
            return null;
        Access();
        return y.Access();
    }

    #endregion

    #region Private Methods

    bool IsRoot() => Parent == null || (Parent.Left != this && Parent.Right != this);

    void Rotate()
    {
        LinkCutTreeLca p = Parent;
        LinkCutTreeLca g = p.Parent;
        bool isRootP = p.IsRoot();
        bool leftChildX = this == p.Left;

        // create 3 edges: (x.r(l),p), (p,x), (x,g)
        Connect(leftChildX ? Right : Left, p, leftChildX);
        Connect(p, this, !leftChildX);
        Connect(this, g, isRootP ? null : p == g.Left);
    }

    void Splay()
    {
        while (!IsRoot()) {
            LinkCutTreeLca p = Parent;
            LinkCutTreeLca g = p.Parent;
            if (!p.IsRoot())
                (this == p.Left == (p == g.Left) ? p : this).Rotate();
            Rotate();
        }
    }

    static void Connect(LinkCutTreeLca ch, LinkCutTreeLca p, bool? isLeftChild)
    {
        if (ch != null)
            ch.Parent = p;
        if (isLeftChild != null) {
            if (isLeftChild == true)
                p.Left = ch;
            else
                p.Right = ch;
        }
    }

    #endregion
}