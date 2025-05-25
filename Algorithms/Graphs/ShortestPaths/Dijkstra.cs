using Algorithms.Collections;

namespace Algorithms.Graphs;

// Implementation of Dijkstra's algorithm using adjacency lists
// and priority queue for efficiency.
//
// Running time: O(|E| log |V|)

public class Djikstra
{
    public readonly long[] Distances;
    public readonly int[] Parents;

    public Djikstra(IList<int>[] graph, List<int>[] weights, int start, int end = -1)
    {
        int n = graph.Length;
        Distances = new long[n];
        Parents = new int[n];
        for (int i = 0; i < n; i++) {
            Distances[i] = long.MaxValue >> 4;
            Parents[i] = -1;
        }

        var q = new MinHeap<Pair>();
        q.Enqueue(new Pair(start, 0));
        Distances[start] = 0;
        while (q.Count > 0) {
            Pair tup = q.Dequeue();
            int u = tup.X;
            if (u == end) return;
            if (tup.Cost != Distances[u]) continue;

            IList<int> neighbors = graph[u];
            for (int i = 0; i < neighbors.Count; i++) {
                int v = neighbors[i];
                long d2 = Distances[u] + weights[u][i];
                if (d2 >= Distances[v]) continue;
                Parents[v] = u;
                Distances[v] = d2;
                q.Enqueue(new Pair(v, d2));
            }
        }
    }

    struct Pair : IComparable<Pair>
    {
        public readonly int X;
        public readonly long Cost;

        public Pair(int x, long cost)
        {
            X = x;
            Cost = cost;
        }

        public int CompareTo(Pair pair)
        {
            int cmp = Cost.CompareTo(pair.Cost);
            return cmp != 0 ? cmp : X.CompareTo(pair.X);
        }
    }
}