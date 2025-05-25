namespace Algorithms.Graphs;

using TreapNode = DynamicGraph.TreapNode;

[TestFixture]
public class TreapTest
{
    readonly Random _random = new(0x12345678);

    public static List<object> TreapToList(TreapNode treap)
    {
        var l = new List<object>();
        if (treap == null)
            return l;

        for (TreapNode? cur = treap.First(); cur != null; cur = cur.Next)
            l.Add(cur.Value);

        return l;
    }

    class VisitPair
    {
        public int Count;
        public bool Flag;
    }

    void CheckTreap(TreapNode treap)
    {
        TreapNode root = treap.Root();
        //Check priorities, generate inorder list
        var list = new List<TreapNode>();

        VisitPair VisitRec(TreapNode node, TreapNode parent)
        {
            int c = 1;
            bool f = node.Flag;
            AreEqual(node.Root(), root);
            AreEqual(node.Parent, parent,
                "checking parent of " + node.Value + " expect " + parent?.Value);
            if (node.Left != null) {
                IsTrue(node.Left.Priority > node.Priority, "checking left priority dominance of " + node.Value);
                IsTrue(node.Prev != null, node.Value + " has a prev node");
                VisitPair r = VisitRec(node.Left, node);
                c += r.Count;
                f = f || r.Flag;
            }

            list.Add(node);
            if (node.Right != null) {
                IsTrue(node.Right.Priority > node.Priority, "checking right priority dominance of " + node.Value);
                IsTrue(node.Next != null, node.Value + " has a next node");
                VisitPair r = VisitRec(node.Right, node);
                c += r.Count;
                f = f || r.Flag;
            }

            AreEqual(c, node.Count, "checking count");
            AreEqual(f, node.FlagAggregate, "checking flagAggregate");
            return new VisitPair { Count = c, Flag = f };
        }

        VisitPair n = VisitRec(root, null);
        AreEqual(n.Count, list.Count, "checking total count");
        AreEqual(n.Flag, list.Any(v => v.Flag), "checking total flagAggregate");
        //Check linked list and tree are consistent
        for (int i = 0; i < list.Count; ++i) {
            if (i > 0) {
                AreEqual(list[i].Prev, list[i - 1], list[i].Value + " prev pointer");
            } else {
                AreEqual(list[i].Prev, null, "First item");
                AreEqual(list[i].Left, null, "no left ptr for First");
            }

            if (i < list.Count - 1) {
                AreEqual(list[i].Next, list[i + 1], list[i].Value + " next pointer");
            } else {
                AreEqual(list[i].Next, null, "last item");
                AreEqual(list[i].Right, null, "no right ptr for last");
            }
        }

        AreEqual(treap.First(), list[0], "First matches");
        AreEqual(treap.Last(), list[list.Count - 1], "last matches");
    }

    List<object> TreapItems(TreapNode treap)
    {
        var list = new List<object>();

        void Visit(TreapNode node)
        {
            if (node.Left != null)
                Visit(node.Left);

            list.Add(node.Value);
            if (node.Right != null)
                Visit(node.Right);
        }

        Visit(treap.Root());
        return list;
    }

    [Test]
    public void TreapBasic()
    {
        var a = new TreapNode("a");
        TreapNode b = a.Insert("b");
        TreapNode c = b.Insert("c");

        CheckTreap(a);
        AreEqual(TreapItems(a), new[] { "a", "b", "c" });

        b.Remove();
        CheckTreap(a);
        AreEqual(TreapItems(a), new[] { "a", "c" });

        a.Remove();
        CheckTreap(c);
        AreEqual(TreapItems(c), new[] { "c" });

        TreapNode d = c.Insert("d");
        TreapNode e = d.Insert("e");
        TreapNode f = e.Insert("f");
        CheckTreap(c);
        AreEqual(TreapItems(c), new[] { "c", "d", "e", "f" });

        d.Remove();
        CheckTreap(c);
        AreEqual(TreapItems(c), new[] { "c", "e", "f" });
    }

    [Test]
    public void TreapRandom()
    {
        //Build a random treap
        var x = new TreapNode(-1);
        var nodes = new List<TreapNode> { x };
        var list = new List<int> { -1 };
        List<object> values;

        for (int i = 0; i < 100; ++i) {
            int h = (int)(_random.NextDouble() * list.Count);
            TreapNode n = nodes[h];
            nodes.Insert(h, n.Insert(i));
            list.Insert(h, i);
            if (i % 20 == 0) {
                CheckTreap(x);
                values = nodes.ConvertAll(v => v.Value);
                AreEqual(values, list);
            }

            if (_random.NextDouble() < 0.1) {
                int pp = (int)(_random.NextDouble() * list.Count);
                bool s = _random.NextDouble() < 0.5;
                nodes[pp].SetFlag(s);
                AreEqual(nodes[pp].Flag, s, "checking local flag");
            }
        }

        CheckTreap(x);
        values = nodes.ConvertAll(v => v.Value);
        AreEqual(values, list);
        for (int i = 0; i < 100; ++i) {
            int h = (int)(_random.NextDouble() * list.Count);
            TreapNode n = nodes[h];
            n.Remove();
            nodes.RemoveAt(h);
            list.RemoveAt(h);

            if (i % 20 == 0) {
                CheckTreap(nodes[0]);
                values = nodes.ConvertAll(v => v.Value);
                AreEqual(values, list);
            }
        }

        CheckTreap(nodes[0]);
        values = nodes.ConvertAll(v => v.Value);
        AreEqual(values, list);
    }

    [Test]
    public void TreapMergeSplitSimple()
    {
        var x = new TreapNode(0);
        var list = new List<int> { 0 };
        var nodes = new List<TreapNode> { x };
        for (int i = 1; i < 10; ++i) {
            list.Add(i);
            nodes.Add(nodes[i - 1].Insert(i));
            if (_random.NextDouble() < 0.25) nodes[nodes.Count - 1].SetFlag(true);
        }

        CheckTreap(x);
        AreEqual(TreapItems(x), list);

        TreapNode y;

        for (int i = 0; i < 9; ++i) {
            List<int> lo = list.GetRange(0, i + 1);
            List<int> hi = list.GetRange(i + 1, list.Count - (i + 1));

            x = nodes[i];
            y = x.Split();

            CheckTreap(x);
            AreEqual(TreapItems(x), lo);
            CheckTreap(y);
            AreEqual(TreapItems(y), hi);
            y.Concat(x);
            CheckTreap(y);
            AreEqual(TreapItems(y), hi.Concat(lo));
            y = nodes[nodes.Count - 1];
            x = y.Split();
            CheckTreap(x);
            AreEqual(TreapItems(x), lo);
            CheckTreap(y);
            AreEqual(TreapItems(y), hi);
            x.Concat(y);
            CheckTreap(x);
            AreEqual(TreapItems(x), list);
        }

        x = nodes[9];
        x.Split();
        CheckTreap(x);
        AreEqual(TreapItems(x), list);

        y = new TreapNode("a");
        y.Last().Insert("b");
        y.Last().Insert("c");
        CheckTreap(y);
        AreEqual(TreapItems(y),
            new object[] { "a", "b", "c" });

        x.Concat(y);
        CheckTreap(x);
        AreEqual(TreapItems(x),
            new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, "a", "b", "c" });

        x = nodes[nodes.Count - 1];
        y = x.Split();

        CheckTreap(y);
        CheckTreap(x);
        AreEqual(TreapItems(x), list);
    }
}