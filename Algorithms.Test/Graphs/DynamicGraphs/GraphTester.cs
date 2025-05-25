using Algorithms.Collections;
using static Algorithms.Graphs.DynamicGraph;

namespace Algorithms.Graphs.DynamicGraphs;

/// <summary>
///     Maintains a spanning forest for an arbitrary undirected graph under
///     incremental edge insertions and deletions.
///     Implementation based on the following algorithm:
///     J. Holm, K.de Lichtenberg and M.Thorrup. "Poly-logarithmic deterministic
///     fully-dynamic  algorithms for connectivity, minimum spanning tree,
///     2-edge connectivity and biconnectivity".
///     2001. Journal of the ACM
///     http://www.cs.princeton.edu/courses/archive/spr10/cos423/handouts/NearOpt.pdf
///     Connectivity queries -- O(log(size of component)),
///     Updates -- O(log(V)^2) time.
///     Space complexity -- O((E + V) * log(V)).
///     SOURCE: https://github.com/mikolalysenko/dynamic-forest (MIT LICENSE)
/// </summary>
class EdgePair
{
    public Edge E;
    public int S, T;
}

class GraphTester
{
    internal readonly List<EdgePair> Edges = new();
    readonly int log2N;
    readonly int n;
    readonly Vertex[] vertices;

    public GraphTester(int n)
    {
        vertices = new Vertex[n];
        for (int i = 0; i < n; ++i)
            vertices[i] = new Vertex(i);

        this.n = n;
        log2N = BitTools.Log2(n);
    }

    void ValidateGraph(bool skipConn = false)
    {
        var forest = new UnionFind(n);
        var eulerForest = new UnionFind(n);
        for (int i = 0; i < Edges.Count; ++i) {
            EdgePair e = Edges[i];
            IsTrue(e.E.S.Adjacent.IndexOf(e.E) >= 0, "checking edge contained in s adj list");
            IsTrue(e.E.T.Adjacent.IndexOf(e.E) >= 0, "checking edge contained in t adj list");
            IsTrue(e.E.Level >= 0, "checking Level is > 0");
            IsTrue(e.E.Level < log2N, "checking Level is valid " + e.E.Level + " < " + log2N);
            forest.Union(e.S, e.T);
            if (e.E.Euler != null) {
                IsTrue(e.E.Level < e.E.S.Euler.Count, "checking euler tree s entries present");
                IsTrue(e.E.Level < e.E.T.Euler.Count, "checkign euler tree t entries present");
                eulerForest.Union(e.S, e.T);
                for (int j = 0; j < e.E.Euler.Count; ++j) {
                    IsTrue(e.E.Euler[j] != null, "checking euler entry " + j);
                    IsTrue(e.E.Euler[j] is EulerHalfEdge, "checking euler type=edge");
                    AreEqual(e.E.Euler[j].Label, e.E, "checking pointers");
                    AreEqual(e.E.Euler[j].S, e.E.S.Euler[j], "checking s-Link consistent");
                    AreEqual(e.E.Euler[j].T, e.E.T.Euler[j], "checking t-Link consistent");
                }
            }
        }

        for (int i = 0; i < n; ++i) {
            Vertex v = vertices[i];
            int c = n;
            IsTrue(v.Euler.Count <= log2N, "checking num levels < " + log2N);
            for (int j = 0; j < v.Euler.Count; ++j) {
                AreEqual(v.Euler[j].Label, v, "checking vertex euler entry consistent @ Level" + j);
                IsTrue(v.Euler[j] is EulerVertex, "checking vertex type consistent @ Level " + j);
                IsTrue(v.Euler[j].Count <= c,
                    "checking component size consistent: " + v.Euler[j].Count + "<=" + c);
                c >>= 1;
            }
        }

        if (skipConn) return;
        for (int i = 0; i < n; ++i)
        for (int j = 0; j < n; ++j) {
            bool conn = forest.Find(i) == forest.Find(j);
            AreEqual(vertices[i].Connected(vertices[j]), conn, "checking connectivity");
            AreEqual(eulerForest.Find(i) == eulerForest.Find(j), conn,
                "checking connectivity for forest");
        }
    }

    public void Verify(bool b = false)
    {
        ValidateGraph(b);
    }

    public EdgePair Link(int i, int j)
    {
        var e = new EdgePair
        {
            S = i,
            T = j,
        };

        e.E = vertices[i].Link(vertices[j], e);
        Edges.Add(e);
        return e;
    }

    public void Cut(EdgePair e)
    {
        e.E.Cut();
        Edges.Remove(e);
    }

    public bool Connected(int i, int j) => vertices[i].Connected(vertices[j]);
}