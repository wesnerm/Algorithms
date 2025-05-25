namespace Algorithms.Graphs;

// SOURCE: STANFORD

/// <summary>
///     Adjacency matrix implementation of Stoer-Wagner Math.Min cut algorithm.
///     Running time:
///     O(|V|^3)
/// </summary>
public class StoerWagnerMinVertexCut
{
    /// <summary>
    ///     (min cut value, nodes in half of Math.Min cut)
    /// </summary>
    public List<int> BestCut;

    public int BestWeight;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StoerWagnerMinCut" /> class.
    /// </summary>
    /// <param name="weights">- graph, constructed using AddEdge()</param>
    public StoerWagnerMinVertexCut(int[][] weights)
    {
        int n = weights.Length;
        var used = new BitArray(n);

        var cut = new List<int>();
        var bestCut = new List<int>();

        int bestWeight = -1;

        for (int phase = n - 1; phase >= 0; phase--) {
            int[] w = weights[0];
            var added = (BitArray)used.Clone();
            int last = 0;
            for (int i = 0; i < phase; i++) {
                int prev = last;
                last = -1;
                for (int j = 1; j < n; j++)
                    if (!added[j] && (last == -1 || w[j] > w[last]))
                        last = j;

                if (i == phase - 1) {
                    for (int j = 0; j < n; j++)
                        weights[prev][j] += weights[last][j];
                    for (int j = 0; j < n; j++)
                        weights[j][prev] = weights[prev][j];

                    used[last] = true;
                    cut.Add(last);
                    if (bestWeight == -1 || w[last] < bestWeight) {
                        bestCut = cut;
                        bestWeight = w[last];
                    }
                } else {
                    for (int j = 0; j < n; j++)
                        w[j] += weights[last][j];
                    added[last] = true;
                }
            }
        }

        BestWeight = bestWeight;
        BestCut = bestCut;
    }
}