using T = long;

namespace Algorithms.Mathematics.DP;

public class LiChaoSegmentTreeF
{
    readonly int sign;
    readonly int size;
    Node root;

    public LiChaoSegmentTreeF(int sz, bool maximum)
    {
        size = sz + 1;
        sign = maximum ? -1 : 1;
    }

    public void AddLine(Func<T, T> fx)
    {
        Insert(fx, -size, size, ref root);
    }

    public T Query(T x) => Query(x, -size, size, root) * sign;

    void Insert(Func<T, T> fx, int l, int r, ref Node nd)
    {
        if (nd == null) {
            nd = new Node(fx);
            return;
        }

        int sign = this.sign;
        T trl = nd.F(l) * sign, trr = nd.F(r) * sign;
        T vl = fx(l) * sign, vr = fx(r) * sign;

        if (trl <= vl && trr <= vr) return;
        if (trl > vl && trr > vr) {
            nd.F = fx;
            return;
        }

        int mid = (l + r) >> 1;
        if (trl > vl) Swap(ref nd.F, ref fx);
        if (nd.F(mid) * sign < fx(mid) * sign) {
            Insert(fx, mid + 1, r, ref nd.R);
        } else {
            Swap(ref nd.F, ref fx);
            Insert(fx, l, mid, ref nd.L);
        }
    }

    long Query(T x, int l, int r, Node nd)
    {
        if (nd == null) return T.MaxValue * sign;
        if (l == r) return nd.F(x) * sign;

        int mid = (l + r) >> 1;
        return Math.Min(nd.F(x) * sign, mid >= x
            ? Query(x, l, mid, nd.L)
            : Query(x, mid + 1, r, nd.R));
    }

    class Node
    {
        public Func<T, T> F;
        public Node L, R;

        public Node(Func<T, T> f) => F = f;
    }
}