using System.Runtime.CompilerServices;

namespace Algorithms.Graphs;

// This is an optimized version of LinkCutTree without the path queries
public class LinkCutTreeConnectivity
{
    #region Variables

    bool _revert;
    public LinkCutTreeConnectivity Left, Right, Parent;

    #endregion

    #region Public Methods

    public LinkCutTreeConnectivity Access()
    {
        LinkCutTreeConnectivity last = null;
        for (LinkCutTreeConnectivity? y = this; y != null; y = y.Parent) {
            y.Splay();
            y.Left = last;
            last = y;
        }

        Splay();
        return last;
    }

    public bool Connected(LinkCutTreeConnectivity y)
    {
        if (this == y)
            return true;
        Access();
        // now x.parent is null
        y.Access();
        return Parent != null;
    }

    public bool Link(LinkCutTreeConnectivity y)
    {
        if (Connected(y)) return false;
        PrivateMakeRoot();
        Parent = y;
        return true;
    }

    public bool Cut(LinkCutTreeConnectivity y)
    {
        PrivateMakeRoot();
        y.Access();
        if (y.Right != this || Left != null || Right != null)
            return false;
        y.Right.Parent = null;
        y.Right = null;
        return true;
    }

    #endregion

    #region Private Methods

    bool IsRoot() => Parent == null || (Parent.Left != this && Parent.Right != this);

    void Push()
    {
        if (_revert) {
            _revert = false;
            LinkCutTreeConnectivity t = Left;
            Left = Right;
            Right = t;
            if (Left != null) Left._revert = !Left._revert;
            if (Right != null) Right._revert = !Right._revert;
        }
    }

    void Rotate()
    {
        LinkCutTreeConnectivity p = Parent;
        LinkCutTreeConnectivity g = p.Parent;
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
            LinkCutTreeConnectivity p = Parent;
            LinkCutTreeConnectivity g = p.Parent;
            if (!p.IsRoot())
                g.Push();
            p.Push();
            Push();
            if (!p.IsRoot())
                (this == p.Left == (p == g.Left) ? p : this).Rotate();
            Rotate();
        }

        Push();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void PrivateMakeRoot()
    {
        Access();
        _revert = !_revert;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Connect(LinkCutTreeConnectivity ch, LinkCutTreeConnectivity p, bool? isLeftChild)
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