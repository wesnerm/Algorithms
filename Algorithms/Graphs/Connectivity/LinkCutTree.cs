using System.Runtime.CompilerServices;

namespace Algorithms.Graphs.TreeGraphs;

public partial class LinkCutTree
{
    #region Constructor

    public LinkCutTree(int value)
    {
        Value = value;
        _subTreeValue = value;
        _delta = GetNeutralDelta();
    }

    #endregion

    #region Variables

    public int Value;
    int _subTreeValue;
    int _delta;
    public int Size = 1;
    int oSize = 1;
    bool _revert;
    LinkCutTree _left;
    LinkCutTree _right;
    LinkCutTree _parent;

    #endregion

    #region Public Methods

    public LinkCutTree Access()
    {
        LinkCutTree last = null;
        for (LinkCutTree? y = this; y != null; y = y._parent) {
            y.Splay();
            y.oSize -= GetSize(last);
            y.oSize += GetSize(y._left);
            y._left = last;
            last = y;
            y.Update();
        }

        Splay();
        return last;
    }

    public bool Connected(LinkCutTree y)
    {
        if (this == y)
            return true;
        Access();
        // now x.parent is null
        y.Access();
        return _parent != null;
    }

    public bool Link(LinkCutTree y)
    {
        if (Connected(y)) return false;
        MakeRoot();
        _parent = y;
        y.oSize += Size;
        y.Update();
        return true;
    }

    public bool Cut(LinkCutTree y)
    {
        MakeRoot();
        y.Access();
        if (y._right != this || _left != null)
            // no edge (x,y)
            return false;
        y._right._parent = null;
        y._right = null;
        y.Update();
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Connect(LinkCutTree ch, LinkCutTree p, bool? isLeftChild)
    {
        if (ch != null)
            ch._parent = p;
        if (isLeftChild != null) {
            if (isLeftChild == true)
                p._left = ch;
            else
                p._right = ch;
        }
    }

    public int Query(LinkCutTree to)
    {
        MakeRoot();
        to.Access();
        return GetSubTreeValue(to);
    }

    public void Modify(LinkCutTree to, int delta)
    {
        MakeRoot();
        to.Access();
        to._delta = JoinDeltas(to._delta, delta);
    }

    #endregion

    #region Private Methods

    bool IsRoot() => _parent == null || (_parent._left != this && _parent._right != this);

    void Push()
    {
        if (_revert) {
            _revert = false;
            LinkCutTree t = _left;
            _left = _right;
            _right = t;
            if (_left != null) _left._revert = !_left._revert;
            if (_right != null) _right._revert = !_right._revert;
        }

        Value = JoinValueWithDelta(Value, _delta);
        _subTreeValue = JoinValueWithDelta(_subTreeValue, DeltaEffectOnSegment(_delta, Size));
        if (_left != null)
            _left._delta = JoinDeltas(_left._delta, _delta);
        if (_right != null)
            _right._delta = JoinDeltas(_right._delta, _delta);
        _delta = GetNeutralDelta();
    }

    void Rotate()
    {
        LinkCutTree p = _parent;
        LinkCutTree g = p._parent;
        bool isRootP = p.IsRoot();
        bool leftChildX = this == p._left;

        // create 3 edges: (x.r(l),p), (p,x), (x,g)
        Connect(leftChildX ? _right : _left, p, leftChildX);
        Connect(p, this, !leftChildX);
        Connect(this, g, isRootP ? null : p == g._left);
        p.Update();
    }

    void Splay()
    {
        while (!IsRoot()) {
            LinkCutTree p = _parent;
            LinkCutTree g = p._parent;
            if (!p.IsRoot())
                g.Push();
            p.Push();
            Push();
            if (!p.IsRoot())
                (this == p._left == (p == g._left) ? p : this).Rotate();
            Rotate();
        }

        Push();
        Update();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void MakeRoot()
    {
        Access();
        _revert = !_revert;
    }

    void Update()
    {
        _subTreeValue =
            QueryOperation(
                QueryOperation(GetSubTreeValue(_left), JoinValueWithDelta(Value, _delta)),
                GetSubTreeValue(_right));
        Size = oSize + GetSize(_left) + GetSize(_right);
    }

    #region Compute

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int ModifyOperation(int x, int y) => x + y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int QueryOperation(int leftValue, int rightValue) => leftValue + rightValue;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int DeltaEffectOnSegment(int delta, int segmentLength) => delta * segmentLength;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int GetNeutralDelta() => 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int GetNeutralValue() => 0;

    // generic code
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int JoinValueWithDelta(int value, int delta) => ModifyOperation(value, delta);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int JoinDeltas(int delta1, int delta2) => ModifyOperation(delta1, delta2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int GetSize(LinkCutTree root) => root != null ? root.Size : 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int GetSubTreeValue(LinkCutTree root) =>
        root == null
            ? GetNeutralValue()
            : JoinValueWithDelta(root._subTreeValue, DeltaEffectOnSegment(root._delta, root.Size));

    #endregion

    #endregion
}