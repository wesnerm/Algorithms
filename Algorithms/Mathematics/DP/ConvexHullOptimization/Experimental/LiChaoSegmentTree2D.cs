namespace Algorithms.Mathematics.DP;

public class LiChaoMinSegmentTree2D
{
    const long Infinity = (long)2e9;
    public Node Root;
    public int Size;

    public LiChaoMinSegmentTree2D(int size) => Size = size + 1;

    public void AddLine(Func<long, long, long> fx)
    {
        long c = fx(0, 0);
        AddLine(fx(1, 0) - c, fx(0, 1) - c, c);
    }

    public void AddLine(long a, long b, long c)
    {
        Insert(new Plane(a, b, c), -Size, Size, -Size, Size, ref Root);
    }

    void Insert(Plane p, int x0, int x1, int y0, int y1, ref Node node)
    {
        if (node == null) {
            node = new Node(p);
            return;
        }

        int midx = (x0 + x1) >> 1;
        int midy = (y0 + y1) >> 1;
        if (node.F.Eval(midx, midy) > p.Eval(midx, midy))
            (p, node.F) = (node.F, p);

        // p is greater than the center
        // Search the 4 corners for a spot to place p

        Plane f = node.F;
        if (f.Eval(x0, y0) > p.Eval(x0, y0))
            Insert(p, x0, midx, y0, midy, ref node.C00);

        if (f.Eval(x0, y1) > p.Eval(x0, y1))
            Insert(p, x0, midx, midy + 1, y1, ref node.C01);

        if (f.Eval(x1, y0) > p.Eval(x1, y0))
            Insert(p, midx + 1, x1, y0, midy, ref node.C10);

        if (f.Eval(x1, y1) > p.Eval(x1, y1))
            Insert(p, midx + 1, x1, midy + 1, y1, ref node.C11);
    }

    public long Query(int x, int y)
    {
        Node? node = Root;
        long result = Infinity;
        int x0 = -Size, x1 = Size;
        int y0 = -Size, y1 = Size;

        while (node != null) {
            result = Math.Min(result, node.F.Eval(x, y));

            if (x0 == x1 && y0 == y1)
                break;

            int midx = (x0 + x1) >> 1;
            int midy = (y0 + y1) >> 1;

            if (y <= midy)
                y1 = midy;
            else
                y0 = midy + 1;

            if (x <= midx) {
                x1 = midx;
                node = y <= midy ? node.C00 : node.C01;
            } else {
                x0 = midx + 1;
                node = y <= midy ? node.C10 : node.C11;
            }
        }

        return result;
    }

    public class Plane
    {
        public long A, B, C;

        public Plane() { }

        public Plane(long a, long b, long c)
        {
            A = a;
            B = b;
            C = c;
        }

        public long Eval(long x, long y) => A * x + B * y + C;
    }

    public class Node
    {
        public Node C00, C01, C10, C11;
        public Plane F;
        public bool Y;

        public Node() : this(new Plane()) { }

        public Node(long a, long b, long c) : this(new Plane(a, b, c)) { }

        public Node(Plane v) => F = v;
    }
}