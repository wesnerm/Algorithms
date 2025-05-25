namespace Algorithms.Graphs;

// http://www.geeksforgeeks.org/minimum-cut-in-a-directed-graph/
// UNTESTED

public class FordFulkersonMinCut
{
    readonly int _v;
    readonly List<Tuple<int, int>> Cuts = new();
    int _s;
    int _t;

    // Prints the minimum s-t cut

    public FordFulkersonMinCut(int[,] graph, int s, int t)
    {
        _v = graph.GetLength(0);
        _s = s;
        _t = t;

        int u, v;

        // Create a residual graph and fill the residual graph with
        // given capacities in the original graph as residual capacities
        // in residual graph
        int[,] rGraph = new int[_v, _v]; // rGraph[i][j] indicates residual capacity of edge i-j
        for (u = 0; u < _v; u++)
        for (v = 0; v < _v; v++)
            rGraph[u, v] = graph[u, v];

        int[] parent = new int[_v]; // This array is filled by BFS and to store path

        // Augment the flow while tere is path from source to sink
        while (Bfs(rGraph, s, t, parent)) {
            // Find minimum residual capacity of the edhes along the
            // path filled by BFS. Or we can say find the maximum flow
            // through the path found.
            int pathFlow = int.MaxValue;
            for (v = t; v != s; v = parent[v]) {
                u = parent[v];
                pathFlow = Math.Min(pathFlow, rGraph[u, v]);
            }

            // update residual capacities of the edges and reverse edges
            // along the path
            for (v = t; v != s; v = parent[v]) {
                u = parent[v];
                rGraph[u, v] -= pathFlow;
                rGraph[v, u] += pathFlow;
            }
        }

        // Flow is maximum now, find vertices reachable from s
        var visited = new BitArray(_v);
        Dfs(rGraph, s, visited);

        // Print all edges that are from a reachable vertex to
        // non-reachable vertex in the original graph

        for (int i = 0; i < _v; i++)
        for (int j = 0; j < _v; j++)
            if (visited[i] && !visited[j] && graph[i, j] != 0)
                Cuts.Add(new Tuple<int, int>(i, j));
    }

    /* Returns true if there is a path from source 's' to sink 't' in
residual graph. Also fills parent[] to store the path */
    bool Bfs(int[,] rGraph, int s, int t, int[] parent)
    {
        // Create a visited array and mark all vertices as not visited
        var visited = new BitArray(_v);

        // Create a queue, enqueue source vertex and mark source vertex
        // as visited
        var q = new Queue<int>();
        q.Enqueue(s);
        visited[s] = true;
        parent[s] = -1;

        // Standard BFS Loop
        while (q.Count > 0) {
            int u = q.Dequeue();

            for (int v = 0; v < _v; v++)
                if (visited[v] == false && rGraph[u, v] > 0) {
                    q.Enqueue(v);
                    parent[v] = u;
                    visited[v] = true;
                }
        }

        // If we reached sink in BFS starting from source, then return
        // true, else false
        return visited[t];
    }

    // A DFS based function to find all reachable vertices from s.  The function
    // marks visited[i] as true if i is reachable from s.  The initial values in
    // visited[] must be false. We can also use BFS to find reachable vertices
    void Dfs(int[,] rGraph, int s, BitArray visited)
    {
        visited[s] = true;
        for (int i = 0; i < _v; i++)
            if (rGraph[s, i] != 0 && !visited[i])
                Dfs(rGraph, i, visited);
    }

    // Driver program to test above functions
    int main()
    {
        // Let us create a graph shown in the above example
        int[,] graph = new[,]
        {
            { 0, 16, 13, 0, 0, 0 },
            { 0, 0, 10, 12, 0, 0 },
            { 0, 4, 0, 0, 14, 0 },
            { 0, 0, 9, 0, 0, 20 },
            { 0, 0, 0, 7, 0, 4 },
            { 0, 0, 0, 0, 0, 0 },
        };

        var minCut = new FordFulkersonMinCut(graph, 0, 5);
        foreach (Tuple<int, int> cut in minCut.Cuts)
            Console.WriteLine($"{cut.Item1} - {cut.Item2}");

        return 0;
    }
}