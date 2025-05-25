using Algorithms.Mathematics;

namespace Algorithms.Graphs;

using EulerVertex = DynamicGraph.EulerVertex;
using EulerHalfEdge = DynamicGraph.EulerHalfEdge;

[TestFixture]
public class EulerTest
{
    public static object[][] TourOf(EulerVertex v)
    {
        v.MakeRoot();
        DynamicGraph.TreapNode? cur = v.Node.First();
        var list = new List<object[]>();
        while (cur != null) {
            if (cur.Value is EulerHalfEdge e)
                list.Add(new[] { e.S.Label, e.T.Label });
            else if (cur.Value is EulerVertex ev)
                list.Add(new[] { ev.Label, ev.Label });

            cur = cur.Next;
        }

        return list.ToArray();
    }

    public void CheckEulerTour(EulerVertex v)
    {
        DynamicGraph.TreapNode? cur = v.Node.First();
        int count = 0;
        while (cur != null) {
            DynamicGraph.TreapNode prev = cur.Prev ?? cur.Last();
            DynamicGraph.TreapNode next = cur.Next ?? cur.First();

            object pe = prev.Value;
            object ce = cur.Value;
            object ne = next.Value;

            if (ce is EulerHalfEdge e) {
                AreEqual(e.Node, cur);
                //Verify opposite node is consistent
                EulerHalfEdge o = e.Opposite;
                AreEqual(o.Opposite, ce, "checking opposite link");
                AreEqual(o.S, e.T, "check opposite head");
                AreEqual(o.T, e.S, "check opposite tail");
                //Check is s/t
                if (pe is EulerHalfEdge e1)
                    AreEqual(e.S, e1.T, "check head - e");
                else
                    AreEqual(e.S, pe, "check head - v");
                if (ne is EulerHalfEdge e2)
                    AreEqual(e.T, e2.S, "check tail - e");
                else
                    AreEqual(e.T, ne, "check tail - v");
            } else if (ce is EulerVertex ev) {
                AreEqual(ev.Node, cur);
                ++count;
                if (pe is EulerHalfEdge e2) {
                    AreEqual(ce, e2.T, "check prev-edge");
                } else {
                    AreEqual(ce, pe, "check circular connectivity singleton");
                    AreEqual(ce, ne, "check circular connectivity");
                    AreEqual(ce, v, "check circular connectivity");
                    AreEqual(cur.Next, null, "must be root");
                    AreEqual(cur.Prev, null, "must be root");
                }

                if (ne is EulerHalfEdge e3) {
                    AreEqual(ce, e3.S, "check next-edge");
                } else {
                    AreEqual(ce, pe, "check circular connectivity for singleton");
                    AreEqual(ce, ne, "check circular connectivity");
                    AreEqual(ce, v, "check circular connectivity");
                    AreEqual(cur.Next, null, "must be root");
                    AreEqual(cur.Prev, null, "must be root");
                }
            } else {
                IsTrue(false, "check type - fail");
            }

            cur = cur.Next;
        }

        AreEqual(v.Count, count, "checking count is consistent");
    }

    [Test]
    public void EulerTourTreeSimple()
    {
        var a = new EulerVertex("a");
        var b = new EulerVertex("b");
        var c = new EulerVertex("c");

        AreEqual(TourOf(a), new[] { new[] { "a", "a" } });
        AreEqual(TourOf(b), new[] { new[] { "b", "b" } });
        AreEqual(TourOf(c), new[] { new[] { "c", "c" } });

        CheckEulerTour(a);
        CheckEulerTour(b);
        CheckEulerTour(c);
        IsTrue(a.Connected(a), "a --- a");
        IsTrue(!a.Connected(b), "a -/- b");
        IsTrue(!a.Connected(c), "a -/- c");
        IsTrue(!b.Connected(a), "b -/- a");
        IsTrue(b.Connected(b), "b --- b");
        IsTrue(!b.Connected(c), "b -/- c");
        IsTrue(!c.Connected(a), "c -/- a");
        IsTrue(!c.Connected(b), "c -/- b");
        IsTrue(c.Connected(c), "c --- c");

        EulerHalfEdge ab = a.Link(b, "ab");

        AreEqual(TourOf(a), new[]
            {
                new[] { "a", "b" },
                new[] { "b", "b" },
                new[] { "b", "a" },
                new[] { "a", "a" },
            }
        );

        AreEqual(TourOf(b), new[]
        {
            new[] { "b", "a" },
            new[] { "a", "a" },
            new[] { "a", "b" },
            new[] { "b", "b" },
        });

        AreEqual(TourOf(c), new[] { new[] { "c", "c" } });

        CheckEulerTour(a);
        CheckEulerTour(b);
        CheckEulerTour(c);
        IsTrue(a.Connected(a), "a --- a");
        IsTrue(a.Connected(b), "a --- b");
        IsTrue(!a.Connected(c), "a -/- c");
        IsTrue(b.Connected(a), "b --- a");
        IsTrue(b.Connected(b), "b --- b");
        IsTrue(!b.Connected(c), "b -/- c");
        IsTrue(!c.Connected(a), "c -/- a");
        IsTrue(!c.Connected(b), "c -/- b");
        IsTrue(c.Connected(c), "c --- c");

        EulerHalfEdge ac = a.Link(c, "ac");

        CheckEulerTour(a);
        CheckEulerTour(b);
        CheckEulerTour(c);
        IsTrue(a.Connected(a), "a --- a");
        IsTrue(a.Connected(b), "a --- b");
        IsTrue(a.Connected(c), "a --- c");
        IsTrue(b.Connected(a), "b --- a");
        IsTrue(b.Connected(b), "b --- b");
        IsTrue(b.Connected(c), "b --- c");
        IsTrue(c.Connected(a), "c --- a");
        IsTrue(c.Connected(b), "c --- b");
        IsTrue(c.Connected(c), "c --- c");

        ab.Cut();

        CheckEulerTour(a);
        CheckEulerTour(b);
        CheckEulerTour(c);
        IsTrue(a.Connected(a), "a --- a");
        IsFalse(a.Connected(b), "a -/- b");
        IsTrue(a.Connected(c), "a --- c");
        IsFalse(b.Connected(a), "b -/- a");
        IsTrue(b.Connected(b), "b --- b");
        IsFalse(b.Connected(c), "b -/- c");
        IsTrue(c.Connected(a), "c --- a");
        IsFalse(c.Connected(b), "c -/- b");
        IsTrue(c.Connected(c), "c --- c");
    }

    [Test]
    public void RandomEulerTree()
    {
        for (int i = 0; i < 2; ++i) {
            int[] tree = TestGenerator.RandomTree(16);
            var v = new EulerVertex[16];
            var e = new EulerHalfEdge[15];
            for (int j = 0; j < 16; ++j)
                v[j] = new EulerVertex(j);

            int ei = 0;
            for (int j = 0; j < tree.Length; j++) {
                int x = tree[j];
                if (x < 0) continue;
                e[ei++] = v[j].Link(v[x], new[] { x, j });
                CheckEulerTour(v[0]);
            }

            Permutations.Shuffle(e);
            for (int j = 0; j < 15; ++j) {
                EulerVertex vv = e[j].S;
                EulerVertex uu = e[j].T;
                IsTrue(vv.Connected(uu), "checking path connectivity before unlink");
                IsTrue(uu.Connected(vv), "checking path connectivity before unlink");
                e[j].Cut();
                CheckEulerTour(vv);
                CheckEulerTour(uu);
                IsFalse(vv.Connected(uu), "checking unlink successful");
                IsFalse(uu.Connected(vv), "checking unlink successful");
            }
        }
    }
}