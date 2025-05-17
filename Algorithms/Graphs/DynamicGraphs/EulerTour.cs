namespace Algorithms.Graphs;

public partial class DynamicGraph
{
    public class EulerHalfEdge
    {
        public readonly object Label;
        public TreapNode Node;
        public EulerVertex S;
        public EulerVertex T;
        public EulerHalfEdge Opposite;

        EulerHalfEdge(object label, EulerVertex s, EulerVertex t)
        {
            Label = label;
            S = s;
            T = t;
        }

        public static EulerHalfEdge Link(EulerVertex s, EulerVertex t, object label = null)
        {
            s.MakeRoot();
            t.MakeRoot();

            //Create half edges and link them to each other
            var st = new EulerHalfEdge(label, s, t);
            var ts = new EulerHalfEdge(label, t, s);
            st.Opposite = ts;
            ts.Opposite = st;

            //Insert entries in Euler tours
            st.Node = s.Node.Insert(st);
            ts.Node = t.Node.Insert(ts);

            //Link tours together
            s.Node.Concat(t.Node);
            return st;
        }

        void Cleanup()
        {
            var v = Node;
            v.Remove();
            Node = null;
            //v.Value = null;
            //Opposite = null;
            //S = null;
            //T = null;
        }

        public void Cut()
        {
            var other = Opposite;

            //Split into parts
            var a = Node;
            var b = a.Split();
            var c = other.Node;
            var d = c.Split();

            //Pull out the roots
            if (d != null && a.Root() != d.Root())
            {
                //a comes before c: [a, bc, d]
                a.Concat(d);
            }
            else if (b != null && c.Root() != b.Root())
            {
                //c comes before a: [c, da, b]
                c.Concat(b);
            }

            //Clean up mess
            Cleanup();
            other.Cleanup();
        }

        public override string ToString() => $"{S}->{T} {Label}";
    }

    public class EulerVertex 
    {
        public readonly object Label;
        public TreapNode Node;

        public EulerVertex(object label)
        {
            Label = label;
            Node = new TreapNode(this);
        }

        //If flag is set, then this vertex has incident edges of at least level v
        public void SetFlag(bool f)
        {
            Node.SetFlag(f);
        }

        public bool Connected(EulerVertex other)
        {
            return Node.Root() == other.Node.Root();
        }

        public void MakeRoot()
        {
            var a = Node;
            var b = a.Split();
            b?.Concat(a);
        }

        public EulerHalfEdge Link(EulerVertex other, object label=null)
        {
            return EulerHalfEdge.Link(this, other, label);
        }

        public int Count => Node.Root().Count;

        public override string ToString() => $"{Label}";
    }
}