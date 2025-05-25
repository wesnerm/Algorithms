using T = long;

namespace Algorithms.Mathematics.DP;

public class LiChaoSegmentTreePersistent
{
    readonly int sign;
    readonly int size;
    Node root;

    public LiChaoSegmentTreePersistent(int sz, bool maximum)
    {
        size = sz + 1;
        sign = maximum ? -1 : 1;
    }

    public LiChaoSegmentTreePersistent AddLine(T a, T b)
    {
        return AddFunction(x => a * x + b);
    }

    public LiChaoSegmentTreePersistent AddFunction(Func<T, T> fx)
    {
        Node newRoot = Insert(fx, -size, size, root);
        if (newRoot == root) return this;
        var result = (LiChaoSegmentTreePersistent)MemberwiseClone();
        result.root = newRoot;
        return result;
    }

    public T Query(T x) => Query(x, -size, size, root) * sign;

    Node Insert(Func<T, T> fx, int l, int r, Node nd)
    {
        if (nd == null)
            return new Node(fx);

        int sign = this.sign;
        Func<T, T> newF = nd.F;
        T trl = newF(l) * sign, trr = newF(r) * sign;
        T vl = fx(l) * sign, vr = fx(r) * sign;

        if (trl <= vl && trr <= vr) return nd;
        if (trl > vl && trr > vr) return new Node(fx) { L = nd.L, R = nd.R };

        int mid = (l + r) >> 1;

        Node child;
        if (trl > vl) Swap(ref newF, ref fx);
        if (newF(mid) * sign < fx(mid) * sign) {
            child = Insert(fx, mid + 1, r, nd.R);
            if (newF == nd.F && child == nd.R) return nd;
            return new Node(newF) { L = nd.L, R = child };
        }

        Swap(ref newF, ref fx);
        child = Insert(fx, l, mid, nd.L);
        if (newF == nd.F && child == nd.L) return nd;
        return new Node(newF) { L = child, R = nd.R };
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
        public readonly Func<T, T> F;
        public Node L, R;

        public Node(Func<T, T> f) => F = f;
    }
}