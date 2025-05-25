namespace Algorithms.Graphs;

/// <summary>
///     Handles directed and undirected graphs with multiple edges and loops
/// </summary>
public class EulerianPath
{
    public readonly bool Directed;

    public readonly List<Edge>[] G;

    int edgeCount;

    public EulerianPath(int n, bool directed = false)
    {
        Directed = directed;
        G = new List<Edge>[n];
        for (int i = 0; i < n; i++)
            G[i] = new List<Edge>();
    }

    public EulerianPath(List<int>[] g, bool directed = false)
        : this(g.Length, directed)
    {
        for (int u = 0; u < g.Length; u++)
            foreach (int v in g[u])
                if (u < v || directed)
                    AddEdge(u, v);
    }

    public void AddEdge(int u, int v)
    {
        var edge = new Edge { U = u, V = v, I = edgeCount++ };
        G[u].Add(edge);
        if (!Directed)
            G[v].Add(edge);
    }

    public List<int> FindPath(int start = -1)
    {
        int[]? odds = Directed ? FindDirectedOddEdges() : FindOddEdges();
        if (odds == null)
            return null;

        List<int> path;
        if (odds.Length != 0) {
            if (start == -1) {
                start = odds[0];
            } else if (start == odds[1] && !Directed) {
                odds[1] = odds[0];
                odds[0] = start;
            }

            if (start != odds[0]) return null;
            path = FindEulerianPath(odds[0], odds[1]);
        } else {
            if (start == -1)
                start = Enumerable.Range(0, G.Length).FirstOrDefault(x => G[x].Count != 0);
            if (G[start].Count == 0 && edgeCount > 0) return null;
            path = FindEulerianCycle(start);
        }

        return path != null && path.Count == edgeCount + 1 ? path : null;
    }

    int[] FindOddEdges()
    {
        int odd = 0;
        int[] result = Array.Empty<int>();

        for (int i = 0; i < G.Length; i++) {
            List<Edge> adj = G[i];
            if ((adj.Count & 1) != 0) {
                if (odd >= 2) return null;
                if (result.Length == 0) result = new int[2];
                result[odd++] = i;
            }
        }

        return result;
    }

    int[] FindDirectedOddEdges()
    {
        int[] bal = new int[G.Length];
        for (int u = 0; u < G.Length; u++) {
            bal[u] += G[u].Count;
            foreach (Edge e in G[u])
                bal[e.V]--;
        }

        int v1 = -1, v2 = -1;
        for (int u = 0; u < G.Length; u++)
            if (bal[u] != 0) {
                if (bal[u] == 1 && v1 == -1)
                    v1 = u;
                else if (bal[u] == -1 && v2 == -1)
                    v2 = u;
                else
                    return null;
            }

        if (v1 == -1 && v2 == -1)
            return Array.Empty<int>();

        return v1 != -1 && v2 != -1 ? new[] { v1, v2 } : null;
    }

    List<int> FindEulerianPath(int v1, int v2)
    {
        List<int>? cycle = FindEulerianCycle(v1, v2);
        if (cycle != null) {
            int xor = v1 ^ v2;
            for (int i = 1; i < cycle.Count; i++) {
                int c = cycle[i];
                if ((cycle[i - 1] ^ c) == xor && (c == v1 || c == v2)) {
                    cycle.Reverse(0, i);
                    cycle.Reverse(i, cycle.Count - i);
                    break;
                }
            }
        }

        return cycle;
    }

    List<int> FindEulerianCycle(int v1, int v2 = -1)
    {
        bool[] seen = new bool[edgeCount];
        var path = new List<int>(edgeCount + 1);

        long n = G.Length;
        int[] pos = new int[n];

        var stack = new Stack<int>();
        if (v2 != -1)
            stack.Push(v2);
        stack.Push(v1);

        while (stack.Count > 0) {
            int u = stack.Peek();
            if (pos[u] < G[u].Count) {
                Edge e = G[u][pos[u]++];
                if (seen[e.I] == false) {
                    seen[e.I] = true;
                    int v = e.Other(u);
                    stack.Push(v);
                }
            } else {
                stack.Pop();
                if (stack.Count > 0 || v2 == -1)
                    path.Add(u);
            }
        }

        if (path.Count != edgeCount + 1)
            return null;

        path.Reverse();
        return path;
    }

    public class Edge
    {
        public int I, U, V;

        public int Other(int u) => U == u ? V : U;

        public override string ToString() => $"{U} -> {V}";
    }
}