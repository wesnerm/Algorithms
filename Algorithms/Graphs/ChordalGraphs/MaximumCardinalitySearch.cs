namespace Algorithms.Graphs.ChordalGraphs;

// REFERENCE: https://github.com/saadtaame/mcs/blob/master/mcs.cpp

public class MaximumCardinalitySearch
{
    public readonly int[] EliminationOrdering;
    public readonly int FirstVertex;
    public readonly List<int>[] Graph;
    readonly int[] pos;

    public MaximumCardinalitySearch(List<int>[] graph, int firstVertex = 0)
    {
        FirstVertex = firstVertex;
        Graph = graph;
        EliminationOrdering = new int[graph.Length - firstVertex];

        int n = graph.Length;
        int count = 0;

        pos = new int[n];
        for (int i = 0; i < n; i++)
            pos[i] = -1;

        var pairs = new Pair[n];
        for (int i = 0; i < n; i++)
            pairs[i] = new Pair { Vertex = i };

        for (int r = firstVertex; r < n; r++)
            if (pos[r] == -1) {
                var set = new SortedSet<Pair> { pairs[r] };
                while (set.Count > 0) {
                    Pair? max = set.Max;
                    set.Remove(max);

                    int u = max.Vertex;
                    pos[u] = count;
                    EliminationOrdering[count++] = u;

                    foreach (int v in Graph[u])
                        if (pos[v] == -1) {
                            if (pairs[v].Degree > 0)
                                set.Remove(pairs[v]);
                            pairs[v].Degree++;
                            set.Add(pairs[v]);
                        }
                }
            }

        Array.Reverse(EliminationOrdering, 0, EliminationOrdering.Length);
    }

    public bool IsChordalGraph()
    {
        int n = Graph.Length;

        var edges = new HashSet<long>();
        for (int i = 0; i < n; i++)
            foreach (int j in Graph[i])
                edges.Add(EdgeId(i, j));

        foreach (int v in EliminationOrdering) {
            int u = -1;
            foreach (int y in Graph[v])
                if (pos[y] < pos[v] && (u == -1 || pos[u] < pos[y]))
                    u = y;

            foreach (int y in Graph[v])
                if (y != u && pos[y] < pos[u] && !edges.Contains(EdgeId(u, y)))
                    return false;
        }

        return true;
    }

    long EdgeId(int x, int y) => x >= y ? ((long)x << 24) ^ y : EdgeId(y, x);

    class Pair : IComparable<Pair>
    {
        public int Degree, Vertex;

        public int CompareTo(Pair other)
        {
            int cmp = Degree.CompareTo(other.Degree);
            if (cmp != 0) return cmp;
            return Vertex.CompareTo(other.Vertex);
        }
    }
}