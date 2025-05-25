using Algorithms.Graphs.DynamicGraphs;

namespace Algorithms.Graphs;

using Vertex = DynamicGraph.Vertex;

[TestFixture]
public class DynamicGraphTest
{
    readonly Random _random = new();

    [Test]
    public void BasicTest()
    {
        //Create some vertices
        var a = new Vertex("a");
        var b = new Vertex("b");
        var c = new Vertex("c");
        var d = new Vertex("d");

        //Print out connectivity between a and c
        IsFalse(a.Connected(c));

        //Link vertices together in a cycle
        DynamicGraph.Edge ab = a.Link(b);
        DynamicGraph.Edge bc = b.Link(c);
        DynamicGraph.Edge cd = c.Link(d);
        DynamicGraph.Edge da = d.Link(a);

        //Vertices are now connected
        IsTrue(a.Connected(c));
        AreEqual(4, a.ComponentSize());
        AreEqual(4, a.Component().Count());

        //Cut the edge between b and c
        bc.Cut();

        //Still connected
        IsTrue(a.Connected(c));

        //Cut edge between a and d
        da.Cut();

        //Finally a and c are disconnected
        IsFalse(a.Connected(c));
    }

    [Test]
    public void DynamicGraphSimple()
    {
        var tester = new GraphTester(4);
        tester.Verify();

        EdgePair ab = tester.Link(0, 1);
        EdgePair bc = tester.Link(1, 2);
        IsTrue(!tester.Connected(0, 3), "connection sanity check");

        EdgePair cd = tester.Link(2, 3);
        IsTrue(tester.Connected(0, 3), "connection sanity check");

        tester.Verify();
        tester.Cut(bc);
        tester.Verify();
        IsTrue(!tester.Connected(0, 3), "connection sanity check");
    }

    [Test]
    public void RandomTester()
    {
        var tester = new GraphTester(64);
        for (int i = 0; i < 1000; ++i) {
            if (_random.NextDouble() < 0.25) {
                List<EdgePair> e = tester.Edges;
                int h = (int)(_random.NextDouble() * e.Count) | 0;
                if (h < e.Count) tester.Cut(e[h]);
            } else {
                int sv = (int)(_random.NextDouble() * 64) | 0;
                int tv = (int)(_random.NextDouble() * 64) | 0;
                if (!tester.Connected(sv, tv)) tester.Link(sv, tv);
            }

            if (i % 200 == 199) tester.Verify(true);
        }

        tester.Verify(true);
    }

    public void TestLineGraph(int h)
    {
        int n = 1 << h;
        var tester = new GraphTester(n);
        var edges = new EdgePair[n - 1];
        for (int i = 1; i < n; ++i) edges[i - 1] = tester.Link(i - 1, i);

        IsTrue(tester.Connected(0, n - 1), "checking initial connectivity");
        for (int i = h - 1; i >= 0; --i) {
            int s = 1 << i;
            IsTrue(tester.Connected(0, s), "checking connectivity of component @ " + s);
            for (int j = s - 1; j < n - 1; j += s)
                if (edges[j] != null) {
                    tester.Cut(edges[j]);
                    edges[j] = null;
                }

            tester.Verify();
            IsTrue(!tester.Connected(0, s), "checking components are cut successfully @ " + s);
        }
    }

    [Test]
    public void LineGraph()
    {
        TestLineGraph(5);
    }

    class Tree
    {
        readonly List<EdgePair[][]> links;
        public readonly List<List<int>> nodes;
        public readonly GraphTester Tester;

        public Tree(int levels)
        {
            int n = (((1 << (2 * (levels + 1))) - 1) / 3) | 0;

            Tester = new GraphTester(n);

            List<List<int>> tree = nodes = new List<List<int>> { new() { 0 } };
            List<EdgePair[][]> branches = links = new List<EdgePair[][]>();
            int ptr = 1;

            for (int i = 0; i < levels; ++i) {
                List<int> l = tree[i];
                var links = new EdgePair[tree[i].Count][];
                var next = new List<int>();
                for (int j = 0; j < l.Count; ++j) {
                    var h = new EdgePair[4];
                    for (int k = 0; k < 4; ++k) {
                        h[k] = Tester.Link(l[j], ptr);
                        next.Add(ptr++);
                    }

                    links[j] = h;
                }

                branches.Add(links);
                tree.Add(next);
            }
        }

        public void IyerSequence(int k)
        {
            if (k == 0)
                return;

            List<int> l = nodes[k - 1];
            EdgePair[][] b = links[k - 1];
            IyerSequence(k - 1);
            for (int i = 0; i < l.Count; ++i) {
                EdgePair[] h = b[i];
                EdgePair e0 = h[0];
                EdgePair e1 = h[1];
                Tester.Cut(e0);
                Tester.Cut(e1);
                h[0] = Tester.Link(e0.S, e0.T);
                h[1] = Tester.Link(e1.S, e1.T);
            }

            IyerSequence(k - 1);
            for (int i = 0; i < l.Count; ++i) {
                EdgePair[] h = b[i];
                EdgePair e0 = h[2];
                EdgePair e1 = h[3];
                Tester.Cut(e0);
                Tester.Cut(e1);
                h[2] = Tester.Link(e0.S, e0.T);
                h[3] = Tester.Link(e1.S, e1.T);
            }
        }
    }

    [Test]
    public void DynamicGraphSimpleIyer()
    {
        for (int i = 1; i < 5; ++i) {
            var tree = new Tree(i);
            tree.IyerSequence(i);
            tree.Tester.Verify(true);
        }
    }
}