using T = long;

namespace Algorithms.Mathematics.DP;

public class LiChaoSegmentTree
{
    readonly int sign;
    readonly int size;
    Node root;

    public LiChaoSegmentTree(int sz, bool maximum)
    {
        size = sz + 1;
        sign = maximum ? -1 : 1;
    }

    public void AddLine(Func<T, T> fx)
    {
        T y1 = fx(0);
        T y2 = fx(1);
        AddLine(y2 - y1, y1);
    }

    public void AddLine(T a, T b)
    {
        Insert(new Line(a * sign, b * sign), -size, size, ref root);
    }

    public long Query(T x) => Query(x, -size, size, root) * sign;

    void Insert(Line v, int l, int r, ref Node nd)
    {
        if (nd == null) {
            nd = new Node(v);
            return;
        }

        T trl = nd.F.Eval(l), trr = nd.F.Eval(r);
        T vl = v.Eval(l), vr = v.Eval(r);

        if (trl <= vl && trr <= vr) return;
        if (trl > vl && trr > vr) {
            nd.F = v;
            return;
        }

        int mid = (l + r) >> 1;
        if (trl > vl) Swap(ref nd.F, ref v);
        if (nd.F.Eval(mid) < v.Eval(mid)) {
            Insert(v, mid + 1, r, ref nd.R);
        } else {
            Swap(ref nd.F, ref v);
            Insert(v, l, mid, ref nd.L);
        }
    }

    T Query(T x, int l, int r, Node nd)
    {
        if (nd == null) return T.MaxValue;
        if (l == r) return nd.F.Eval(x);

        int mid = (l + r) >> 1;
        return Math.Min(nd.F.Eval(x), mid >= x
            ? Query(x, l, mid, nd.L)
            : Query(x, mid + 1, r, nd.R));
    }

    class Line
    {
        public readonly T A;
        public readonly T B;

        public Line(T a, T b)
        {
            A = a;
            B = b;
        }

        public T Eval(T x) => A * 1L * x + B;
    }

    class Node
    {
        public Line F;
        public Node L, R;

        public Node(Line v) => F = v;
    }
}