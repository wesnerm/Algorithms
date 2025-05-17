#undef UNTHREADED

namespace Algorithms.Graphs;

public partial class DynamicGraph
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    static Random _random = new Random();

    //This is a custom binary tree data structure
    //The reason for using this instead of an array or some generic search tree is that:
    //
    //    * Nodes are ordered by position not sorted by key
    //    * On average tree height is O(log(number of nodes))
    //    * Concatenation and splitting both take O(log(N))
    //    * Has augmentations for size and edge level incidence flag
    //    * Node references are not invalidated during updates
    //    * Has threaded pointers for fast sequential traversal

    public class TreapNode
    {
        public int Priority;
        public bool Flag;
        public bool FlagAggregate;
        public TreapNode Parent;

        public object Value;
        public int Count;
        public TreapNode Left;
        public TreapNode Right;
#if !UNTHREADED
        public TreapNode Next;
        public TreapNode Prev;
#endif

        static int CountOfValue(object v)
        {
            return v is EulerHalfEdge ? 0 : 1;
        }

        public TreapNode(object value)
        {
            Value = value;
            Count = CountOfValue(value);
            Priority = _random.Next();
        }

        public void BubbleUp()
        {
            TreapNode p;
            while (true)
            {
                p = Parent;
                if (p == null || p.Priority < Priority)
                    break;

                if (this == p.Left)
                {
                    var b = Right;
                    p.Left = b;
                    if (b != null)
                        b.Parent = p;
                    Right = p;
                }

                else
                {
                    var b = Left;
                    p.Right = b;
                    if (b != null)
                        b.Parent = p;
                    Left = p;
                }

                p.Update();
                Update();
                var gp = p.Parent;
                p.Parent = this;
                Parent = gp;
                if (gp != null)
                {
                    if (gp.Left == p)
                        gp.Left = this;
                    else
                        gp.Right = this;
                }
            }

            p = Parent;
            while (p != null)
            {
                p.Update();
                p = p.Parent;
            }
        }

        public TreapNode Root()
        {
            var n = this;
            while (n.Parent != null) n = n.Parent;
            return n;
        }

        public TreapNode First()
        {
            var n = Root();
            while (n.Left != null) n = n.Left;
            return n;
        }

        public TreapNode Last()
        {
            var n = Root();
            while (n.Right != null) n = n.Right;
            return n;
        }

#if UNTHREADED
        public TreapNode Next
        {
            get
            {
                var n = this;
                if (n.Right != null)
                {
                    n = n.Right;
                    while (n.Left != null) n = n.Left;
                    return n;
                }

                while (true)
                {
                    var p = n.Parent;
                    if (p == null || p.Left == n) return p;
                    n = p;
                }
            }
        }

        public TreapNode Prev
        {
            get
            {
                var n = this;
                if (n.Left != null)
                {
                    n = n.Left;
                    while (n.Right != null) n = n.Right;
                    return n;
                }

                while (true)
                {
                    var p = n.Parent;
                    if (p == null || p.Right == n) return p;
                    n = p;
                }
            }
        }
#endif

        public TreapNode Insert(object value)
        {
            TreapNode nn;
            if (Right == null)
            {
                nn = Right = new TreapNode(value) { Parent = this };
            }
            else
            {
                var next = Next;
                nn = next.Left = new TreapNode(value) { Parent = next };
            }

#if !UNTHREADED
            nn.Next = Next;
            nn.Prev = this;
            if (Next != null) Next.Prev = nn;
            Next = nn;
#endif
            nn.BubbleUp();
            return nn;
        }

        void SwapNodes(TreapNode a, TreapNode b)
        {
            var p = a.Priority;
            a.Priority = b.Priority;
            b.Priority = p;
            var t = a.Parent;
            a.Parent = b.Parent;
            if (b.Parent != null)
            {
                if (b.Parent.Left == b)
                    b.Parent.Left = a;
                else
                    b.Parent.Right = a;
            }

            b.Parent = t;
            if (t != null)
            {
                if (t.Left == a)
                    t.Left = b;
                else
                    t.Right = b;
            }

            t = a.Left;
            a.Left = b.Left;
            if (b.Left != null)
                b.Left.Parent = a;

            b.Left = t;
            if (t != null)
                t.Parent = b;

            t = a.Right;
            a.Right = b.Right;
            if (b.Right != null)
                b.Right.Parent = a;

            b.Right = t;
            if (t != null)
                t.Parent = b;

#if !UNTHREADED
            t = a.Next;
            a.Next = b.Next;
            if (b.Next != null)
                b.Next.Prev = a;

            b.Next = t;
            if (t != null)
                t.Prev = b;

            t = a.Prev;
            a.Prev = b.Prev;
            if (b.Prev != null)
                b.Prev.Next = a;

            b.Prev = t;
            if (t != null)
                t.Next = b;
#endif

            var c = a.Count;
            a.Count = b.Count;
            b.Count = c;
            var f = a.Flag;
            a.Flag = b.Flag;
            b.Flag = f;
            f = a.FlagAggregate;
            a.FlagAggregate = b.FlagAggregate;
            b.FlagAggregate = f;
        }

        public void Update()
        {
            var c = CountOfValue(Value);
            var f = Flag;
            if (Left != null)
            {
                c += Left.Count;
                f |= Left.FlagAggregate;
            }

            if (Right != null)
            {
                c += Right.Count;
                f |= Right.FlagAggregate;
            }

            Count = c;
            FlagAggregate = f;
        }

        //Set new flag state and propagate up tree
        public void SetFlag(bool f)
        {
            Flag = f;
            for (var v = this; v != null; v = v.Parent)
            {
                var pstate = v.FlagAggregate;
                v.Update();
                if (pstate == v.FlagAggregate)
                    break;
            }
        }

        public void Remove()
        {
            var node = this;
            if (node.Left != null && node.Right != null)
            {
                var other = node.Next;
                SwapNodes(other, node);
            }

#if !UNTHREADED
            if (node.Next != null)
                node.Next.Prev = node.Prev;

            if (node.Prev != null)
                node.Prev.Next = node.Next;
#endif

            var r = node.Left ?? node.Right;

            if (r != null)
                r.Parent = node.Parent;

            if (node.Parent != null)
            {
                if (node.Parent.Left == node)
                    node.Parent.Left = r;
                else
                    node.Parent.Right = r;

                //Update all ancestor counts
                var p = node.Parent;
                while (p != null)
                {
                    p.Update();
                    p = p.Parent;
                }
            }

            //Remove all pointers from detached node
            node.Parent = node.Left = node.Right = null;
            node.Count = 1;
#if !UNTHREADED
            node.Prev = node.Next = null;
#endif
        }

        public TreapNode Split()
        {
            var node = this;
            var s = node.Insert(null);
            s.Priority = int.MinValue;
            s.BubbleUp();
            var l = s.Left;
            var r = s.Right;
            if (l != null)
                l.Parent = null;

            if (r != null)
                r.Parent = null;

#if !UNTHREADED
            if (s.Prev != null)
                s.Prev.Next = null;

            if (s.Next != null)
                s.Next.Prev = null;
#endif

            return r;
        }

        TreapNode ConcatRecurse(TreapNode a, TreapNode b)
        {
            if (a == null || b == null)
                return a ?? b;

            if (a.Priority < b.Priority)
            {
                a.Right = ConcatRecurse(a.Right, b);
                a.Right.Parent = a;
                a.Update();
                return a;
            }

            b.Left = ConcatRecurse(a, b.Left);
            b.Left.Parent = b;
            b.Update();
            return b;
        }

        public TreapNode Concat(TreapNode other)
        {
            if (other == null)
                return null;

            var ra = Root();
            var ta = ra;
            while (ta.Right != null)
                ta = ta.Right;

            var rb = other.Root();
            var sb = rb;
            while (sb.Left != null)
                sb = sb.Left;

#if !UNTHREADED
            ta.Next = sb;
            sb.Prev = ta;
#endif
            var r = ConcatRecurse(ra, rb);
            r.Parent = null;
            return r;
        }

    }
}