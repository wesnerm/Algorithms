using static System.Math;

namespace Algorithms.Graphs;

public class MaxFlow
{
    public class Edge
    {
        public int u, v;
        public long f, c;

        public Edge(int u, int v, long f, long c)
        {
            this.u = u;
            this.v = v;
            this.f = f;
            this.c = c;
        }
    };

    List<int>[] g;
    List<Edge> e = new List<Edge>();
    int[] dist;
    Queue<int> q;
    int n, source, sink;

    public MaxFlow(int n)
    {
        this.n = n;
        dist = new int[n];
        g = new List<int>[n];
        q = new Queue<int>(n);

        for (int i = 0; i < g.Length; i++)
            g[i] = new List<int>();
    }

    public Edge Add(int u, int v, long c, long rc = 0)
    {
        var edge = new Edge(u, v, 0, c);
        g[u].Add(e.Count);
        e.Add(edge);
        g[v].Add(e.Count);
        e.Add(new Edge(v, u, 0, rc));
        return edge;
    }

    public long Flow(int source, int sink)
    {
        this.source = source;
        this.sink = sink;
        long res = 0;
        while (Bfs())
        {
            long pushed;
            while ((pushed = Dfs(source, int.MaxValue)) != 0)
                res += pushed;
        }
        return res;
    }

    bool Bfs()
    {
        for (int i = 0; i < dist.Length; i++)
            dist[i] = int.MaxValue;

        q.Clear();
        q.Enqueue(source);
        dist[source] = 0;
        while (q.Count > 0)
        {
            int u = q.Dequeue();
            foreach (var id in g[u])
            {
                int v = e[id].v;
                if (dist[v] > dist[u] + 1 && e[id].f < e[id].c)
                {
                    dist[v] = dist[u] + 1;
                    q.Enqueue(v);
                    if (v == sink)
                        return true;
                }
            }
        }
        return false;
    }

    long Dfs(int u, long flow)
    {
        if (u == sink || flow == 0)
            return flow;
        foreach (var id in g[u])
        {
            int v = e[id].v;
            if (dist[v] == dist[u] + 1)
            {
                long pushed = Dfs(v, Min(flow, e[id].c - e[id].f));
                if (pushed != 0)
                {
                    e[id].f += pushed;
                    e[id ^ 1].f -= pushed;
                    return pushed;
                }
            }
        }
        return 0;
    }
}
