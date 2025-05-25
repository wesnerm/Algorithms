namespace Algorithms.Graphs;

public class MinCostFlowDense
{
    readonly List<Edge>[] graph;
    long LastFlow;

    public MinCostFlowDense(int n)
    {
        graph = new List<Edge>[n];
        for (int i = 0; i < n; i++)
            graph[i] = new List<Edge>();
    }

    public void AddEdge(int s, int t, int cap, int cost = 0)
    {
        graph[s].Add(new Edge(t, cap, cost, graph[t].Count));
        graph[t].Add(new Edge(s, 0, -cost, graph[s].Count - 1));
    }

    static void BellmanFord(List<Edge>[] graph, int s, int[] dist)
    {
        int n = graph.Length;

        for (int i = 0; i < dist.Length; i++)
            dist[i] = int.MaxValue;

        dist[s] = 0;
        bool[] inqueue = new bool[n];
        int[] q = new int[n];
        int qt = 0;
        q[qt++] = s;
        for (int qh = 0; (qh - qt) % n != 0; qh++) {
            int u = q[qh % n];
            inqueue[u] = false;
            for (int i = 0; i < graph[u].Count; i++) {
                Edge e = graph[u][i];
                if (e.cap <= e.f)
                    continue;
                int v = e.to;
                int ndist = dist[u] + e.cost;
                if (dist[v] > ndist) {
                    dist[v] = ndist;
                    if (!inqueue[v]) {
                        inqueue[v] = true;
                        q[qt++ % n] = v;
                    }
                }
            }
        }
    }

    public long GetMaxFlow(int s, int t, int maxf)
    {
        int n = graph.Length;
        int[] prio = new int[n];
        int[] curflow = new int[n];
        int[] prevedge = new int[n];
        int[] prevnode = new int[n];
        int[] pot = new int[n];

        BellmanFord(graph, s, pot);
        int flow = 0;
        int flowCost = 0;
        while (flow < maxf) {
            for (int i = 0; i < prio.Length; i++)
                prio[i] = int.MaxValue;
            prio[s] = 0;
            bool[] finished = new bool[n];
            curflow[s] = int.MaxValue;
            for (int i = 0; i < n && !finished[t]; i++) {
                int u = -1;
                for (int j = 0; j < n; j++)
                    if (!finished[j] && (u == -1 || prio[u] > prio[j]))
                        u = j;
                if (prio[u] == int.MaxValue)
                    break;
                finished[u] = true;
                for (int k = 0; k < graph[u].Count; k++) {
                    Edge e = graph[u][k];
                    if (e.f >= e.cap)
                        continue;
                    int v = e.to;
                    int nprio = prio[u] + e.cost + pot[u] - pot[v];
                    if (prio[v] > nprio) {
                        prio[v] = nprio;
                        prevnode[v] = u;
                        prevedge[v] = k;
                        curflow[v] = Math.Min(curflow[u], e.cap - e.f);
                    }
                }
            }

            if (prio[t] == int.MaxValue)
                break;
            for (int i = 0; i < n; i++)
                if (finished[i])
                    pot[i] += prio[i] - prio[t];
            int df = Math.Min(curflow[t], maxf - flow);
            flow += df;
            for (int v = t; v != s; v = prevnode[v]) {
                Edge e = graph[prevnode[v]][prevedge[v]];
                e.f += df;
                graph[v][e.rev].f -= df;
                flowCost += df * e.cost;
            }
        }

        LastFlow = flow;
        return flowCost;
    }

    class Edge
    {
        public readonly int cap;
        public readonly int cost;
        public readonly int rev;
        public readonly int to;
        public int f;

        public Edge(int to, int cap, int cost, int rev)
        {
            this.to = to;
            this.cap = cap;
            this.cost = cost;
            this.rev = rev;
        }
    }
}