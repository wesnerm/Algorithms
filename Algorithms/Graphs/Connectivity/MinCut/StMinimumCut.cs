using Algorithms.Collections;

namespace Algorithms.Graphs;

public class StMinimumCut
{
    readonly List<Edge>[] graph;
    readonly int n;

    public StMinimumCut(int n)
    {
        this.n = n;
        graph = new List<Edge>[n];
        for (int i = 0; i < n; i++)
            graph[i] = new List<Edge>();
    }

    public void Add(int s, int t, int cap)
    {
        var sEdge = new Edge(s, t, null, cap);
        Edge tEdge = sEdge.rev = new Edge(t, s, sEdge, 0);

        graph[s].Add(sEdge);
        graph[t].Add(tEdge);
    }

    /*

    // EXAMPLE PREINITIALIZATION
    public int FastInit()
    {
        var flow = 0;
        var sources = graph[0];

        for (int i = 0; i < graph.Length; i++)
            graph[i].Sort((a, b) => a.t.CompareTo(b.t));

        for (int i = 0; i < sources.Count; i++)
        {
            var u = sources[i].t;
            var dests = graph[u];
            for (int j = 0; j < dests.Count; j++)
            {
                var e = dests[j];
                var v = e.t;
                if (e.cap == 0 || e.f >= e.cap) continue;

                var e2 = graph[v][0];
                Debug.Assert(graph[v][0].t == 1); // t
                if (e2.f >= e.cap) continue;

                sources[i].f += 1;
                sources[i].rev.f -= 1;
                dests[j].f += 1;
                dests[j].rev.f -= 1;
                e2.f += 1;
                e2.rev.f -= 1;
                flow++;
                break;
            }
        }

        return flow;
    }*/

    public int MaxFlow(int s, int t, int maxflow)
    {
        int[] q = new int[graph.Length];
        var pred = new TimestampedArray<Edge>(graph.Length);

        int offset = 0;

        int flow = 0;
        while (flow < maxflow) {
            int qt = 0;
            q[qt++] = s;
            pred.Clear();
            offset += 5;

            while (qt > 0 && pred[t] == null) {
                int cur = q[--qt];

                List<Edge> edges = graph[cur];
                for (int i = edges.Count - 1, j = offset % edges.Count; i >= 0; i--, j--) {
                    if (j < 0) j += edges.Count;
                    Edge e = edges[j];
                    if (pred[e.t] == null && e.cap > e.f) {
                        pred[e.t] = e;
                        q[qt++] = e.t;
                    }
                }
            }

            if (pred[t] == null)
                break;
            int df = int.MaxValue;
            for (int u = t; u != s; u = pred[u].s)
                df = Math.Min(df, pred[u].cap - pred[u].f);
            for (int u = t; u != s; u = pred[u].s) {
                pred[u].f += df;
                pred[u].rev.f -= df;
            }

            flow += df;
        }

        return flow;
    }

    public HashSet<int> MinCut(int source, int sink, int maxflow)
    {
        int flow = MaxFlow(source, sink, maxflow);
        var cuts = new HashSet<int>();

        var q = new Queue<int>();
        bool[] marked = new bool[n];
        q.Clear();
        q.Enqueue(source);
        marked[source] = true;
        while (q.Count > 0) {
            int u = q.Dequeue();
            foreach (Edge e in graph[u]) {
                int v = e.t;
                if (e.f < e.cap) {
                    if (v != sink && marked[v] == false)
                        q.Enqueue(v);
                    marked[v] = true;
                }
            }
        }

        bool[] visited2 = new bool[n];
        q.Clear();
        q.Enqueue(source);
        visited2[source] = true;
        while (q.Count > 0) {
            int u = q.Dequeue();
            foreach (Edge e in graph[u]) {
                int v = e.t;
                if (e.f < e.cap) {
                    if (v != sink && visited2[v] == false)
                        q.Enqueue(v);
                    visited2[v] = true;
                } else {
                    if (v == sink)
                        cuts.Add(u);
                    else if (marked[v] == false)
                        cuts.Add(v);
                }
            }
        }

        return cuts;
    }

    public class Edge
    {
        public Edge rev;
        public int s, t, cap, f;

        public Edge(int s, int t, Edge rev, int cap)
        {
            this.s = s;
            this.t = t;
            this.rev = rev;
            this.cap = cap;
        }
    }
}