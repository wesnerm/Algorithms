namespace Algorithms;

// SOURCE: https://www.hackerrank.com/rest/contests/w35/challenges/airports/hackers/uwi/download_solution
// SOURCE: https://www.hackerrank.com/rest/contests/world-codesprint-12/challenges/keko-the-brilliant/hackers/uwi/download_solution

public class Treap
{
    public static Random gen = new();

    static readonly Treap[] Q = new Treap[100];
    public int count, max;
    public Treap Left, Right, Parent;
    public int Pos;
    public long Priority;
    public int V; // value

    public Treap(int pos, int v)
    {
        Pos = pos;
        V = v;
        Priority = gen.Next();
        Update(this);
    }

    public static int search(Treap a, int q)
    {
        int lcount = 0;
        while (a != null) {
            if (a.V == q) {
                lcount += Count(a.Left);
                break;
            }

            if (q < a.V) {
                a = a.Left;
            } else {
                lcount += Count(a.Left) + 1;
                a = a.Right;
            }
        }

        return a == null ? -(lcount + 1) : lcount;
    }

    public static Treap Next(Treap x)
    {
        if (x == null) return null;
        if (x.Right != null) {
            x = x.Right;
            while (x.Left != null) x = x.Left;
            return x;
        }

        while (true) {
            Treap? p = x.Parent;
            if (p == null) return null;
            if (p.Left == x) return p;
            x = p;
        }
    }

    public static Treap Prev(Treap x)
    {
        if (x == null) return null;
        if (x.Left != null) {
            x = x.Left;
            while (x.Right != null) x = x.Right;
            return x;
        }

        while (true) {
            Treap? p = x.Parent;
            if (p == null) return null;
            if (p.Right == x) return p;
            x = p;
        }
    }

    public static Treap Update(Treap a)
    {
        if (a == null) return null;
        a.count = 1;
        if (a.Left != null) a.count += a.Left.count;
        if (a.Right != null) a.count += a.Right.count;

        a.max = a.V;
        if (a.Left != null) a.max = Math.Max(a.Left.max, a.max);
        if (a.Right != null) a.max = Math.Max(a.Right.max, a.max);
        return a;
    }

    public static void Propagate(Treap a)
    {
        while (a != null) {
            Update(a);
            a = a.Parent;
        }
    }

    public static Treap Disconnect(Treap a)
    {
        if (a == null) return null;
        a.Left = a.Right = a.Parent = null;
        return Update(a);
    }

    public static Treap Root(Treap x)
    {
        if (x == null) return null;
        while (x.Parent != null) x = x.Parent;
        return x;
    }

    public static int Count(Treap a) => a?.count ?? 0;

    public static void SetParent(Treap a, Treap par)
    {
        if (a != null) a.Parent = par;
    }

    public static int Max(Treap a, int L, int R)
    {
        if (a == null || L >= R || L >= Count(a) || R <= 0) return -int.MaxValue / 3;
        if (L <= 0 && R >= Count(a)) return a.max;

        int lmax = Max(a.Left, L, R);
        int rmax = Max(a.Right, L - Count(a.Left) - 1, R - Count(a.Left) - 1);
        int max = Math.Max(lmax, rmax);
        if (L <= Count(a.Left) && Count(a.Left) < R) max = Math.Max(max, a.V);
        return max;
    }

    public static Treap Merge(Treap a, Treap b)
    {
        if (b == null) return a;
        if (a == null) return b;
        if (a.Priority > b.Priority) {
            SetParent(a.Right, null);
            SetParent(b, null);
            a.Right = Merge(a.Right, b);
            SetParent(a.Right, a);
            return Update(a);
        }

        SetParent(a, null);
        SetParent(b.Left, null);
        b.Left = Merge(a, b.Left);
        SetParent(b.Left, b);
        return Update(b);
    }

    // [0,K),[K,N)
    public static Treap[] Split(Treap a, int K)
    {
        if (a == null) return new Treap[] { null, null };
        if (K <= Count(a.Left)) {
            SetParent(a.Left, null);
            Treap[] s = Split(a.Left, K);
            a.Left = s[1];
            SetParent(a.Left, a);
            s[1] = Update(a);
            return s;
        } else {
            SetParent(a.Right, null);
            Treap[] s = Split(a.Right, K - Count(a.Left) - 1);
            a.Right = s[0];
            SetParent(a.Right, a);
            s[0] = Update(a);
            return s;
        }
    }

    public static Treap[] Split(Treap x)
    {
        if (x == null) return new Treap[] { null, null };
        if (x.Left != null) x.Left.Parent = null;
        Treap?[] sp = new[] { x.Left, x };
        x.Left = null;
        Update(x);
        while (x.Parent != null) {
            Treap p = x.Parent;
            x.Parent = null;
            if (x == p.Left) {
                p.Left = sp[1];
                if (sp[1] != null) sp[1].Parent = p;
                sp[1] = p;
            } else {
                p.Right = sp[0];
                if (sp[0] != null) sp[0].Parent = p;
                sp[0] = p;
            }

            Update(p);
            x = p;
        }

        return sp;
    }

    public static Treap[] Split(Treap a, params int[] ks)
    {
        int n = ks.Length;
        if (n == 0) return new[] { a };
        for (int i = 0; i < n - 1; i++)
            if (ks[i] > ks[i + 1])
                throw new ArgumentException();

        var ns = new Treap[n + 1];
        Treap cur = a;
        for (int i = n - 1; i >= 0; i--) {
            Treap[] sp = Split(cur, new[] { ks[i] });
            cur = sp[0];
            ns[i] = sp[0];
            ns[i + 1] = sp[1];
        }

        return ns;
    }

    public static Treap Insertb(Treap root, Treap x)
    {
        int ind = LowerBound(root, x.Pos);
        return Insert(root, ind, x);
    }

    public static Treap Insert(Treap a, int K, Treap b)
    {
        if (a == null) return b;
        if (b.Priority < a.Priority) {
            if (K <= Count(a.Left)) {
                a.Left = Insert(a.Left, K, b);
                SetParent(a.Left, a);
            } else {
                a.Right = Insert(a.Right, K - Count(a.Left) - 1, b);
                SetParent(a.Right, a);
            }

            return Update(a);
        }

        Treap[] ch = Split(a, K);
        b.Left = ch[0];
        b.Right = ch[1];
        SetParent(b.Left, b);
        SetParent(b.Right, b);
        return Update(b);
    }

    // delete K-th
    public static Treap Erase(Treap a, int K)
    {
        if (a == null) return null;
        if (K < Count(a.Left)) {
            a.Left = Erase(a.Left, K);
            SetParent(a.Left, a);
            return Update(a);
        }

        if (K == Count(a.Left)) {
            SetParent(a.Left, null);
            SetParent(a.Right, null);
            Treap aa = Merge(a.Left, a.Right);
            Disconnect(a);
            return aa;
        }

        a.Right = Erase(a.Right, K - Count(a.Left) - 1);
        SetParent(a.Right, a);
        return Update(a);
    }

    public static Treap Get(Treap a, int K)
    {
        while (a != null)
            if (K < Count(a.Left)) {
                a = a.Left;
            } else if (K == Count(a.Left)) {
                break;
            } else {
                K = K - Count(a.Left) - 1;
                a = a.Right;
            }

        return a;
    }

    public static int Index(Treap a)
    {
        if (a == null) return -1;
        int ind = Count(a.Left);
        while (a != null) {
            Treap? par = a.Parent;
            if (par != null && par.Right == a) ind += Count(par.Left) + 1;
            a = par;
        }

        return ind;
    }

    public static Treap Update(Treap a, int K, int v)
    {
        int p = 0;
        while (a != null) {
            Q[p++] = a;
            if (K < Count(a.Left)) {
                a = a.Left;
            } else if (K == Count(a.Left)) {
                break;
            } else {
                K = K - Count(a.Left) - 1;
                a = a.Right;
            }
        }

        a.V = v;
        while (p > 0) Update(Q[--p]);
        return a;
    }

    public static Treap mergeTechnically(Treap x, Treap y)
    {
        if (x == null) return y;
        if (y == null) return x;
        if (Count(x) > Count(y)) {
            Treap d = x;
            x = y;
            y = d;
        }

        // |x|<=|y|
        foreach (Treap cur in NodesDfs(x)) y = Insertb(y, Disconnect(cur));
        return y;
    }

    public static int LowerBound(Treap a, int q)
    {
        int lcount = 0;
        while (a != null)
            if (a.Pos >= q) {
                a = a.Left;
            } else {
                lcount += Count(a.Left) + 1;
                a = a.Right;
            }

        return lcount;
    }

    public static Treap[] Treaps(Treap a) => Treaps(a, new Treap[Count(a)], 0, Count(a));

    public static Treap[] Treaps(Treap a, Treap[] ns, int L, int R)
    {
        if (a == null) return ns;
        Treaps(a.Left, ns, L, L + Count(a.Left));
        ns[L + Count(a.Left)] = a;
        Treaps(a.Right, ns, R - Count(a.Right), R);
        return ns;
    }

    // faster than nodes but inconsistent
    public static Treap[] NodesDfs(Treap a)
    {
        return NodesDfs(a, new Treap[a.count], new[] { 0 });
    }

    public static Treap[] NodesDfs(Treap a, Treap[] ns, int[] pos)
    {
        if (a == null) return ns;
        ns[pos[0]++] = a;
        NodesDfs(a.Left, ns, pos);
        NodesDfs(a.Right, ns, pos);
        return ns;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append("Treap [v=");
        builder.Append(V);
        builder.Append(", count=");
        builder.Append(count);
        builder.Append(", parent=");
        builder.Append(Parent?.V.ToString() ?? "null");
        builder.Append(", Max=");
        builder.Append(max);
        builder.Append("]");
        return builder.ToString();
    }

    public static string ToString(Treap a, string indent)
    {
        if (a == null) return "";
        var sb = new StringBuilder();
        sb.Append(ToString(a.Left, indent + "  "));
        sb.Append(indent).Append(a).Append("\n");
        sb.Append(ToString(a.Right, indent + "  "));
        return sb.ToString();
    }
}