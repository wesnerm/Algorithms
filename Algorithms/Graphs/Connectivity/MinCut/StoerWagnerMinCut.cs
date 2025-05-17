using System.Collections;

namespace Algorithms.Graphs;

// SOURCE: STANFORD

/// <summary>
/// Adjacency matrix implementation of Stoer-Wagner Math.Min cut algorithm.
///
/// Running time:
///     O(|V|^3)
///
/// </summary>
/// 

public class StoerWagnerMinVertexCut
{
    /// <summary>
    /// (min cut value, nodes in half of Math.Min cut)        
    /// </summary>
    public List<int> BestCut;
    public int BestWeight;

    /// <summary>
    /// Initializes a new instance of the <see cref="StoerWagnerMinCut"/> class.
    /// </summary>
    /// <param name="weights">- graph, constructed using AddEdge()</param>
    public StoerWagnerMinVertexCut(int[][] weights)
    {
        var n = weights.Length;
        var used = new BitArray(n);

        List<int> cut = new List<int>();
        List<int> bestCut = new List<int>();

        var bestWeight = -1;

        for (var phase = n - 1; phase >= 0; phase--)
        {
            var w = weights[0];
            var added = (BitArray) used.Clone();
            var last = 0;
            for (var i = 0; i < phase; i++)
            {
                var prev = last;
                last = -1;
                for (var j = 1; j < n; j++)
                    if (!added[j] && (last == -1 || w[j] > w[last])) last = j;

                if (i == phase - 1)
                {
                    for (var j = 0; j < n; j++)
                        weights[prev][j] += weights[last][j];
                    for (var j = 0; j < n; j++)
                        weights[j][prev] = weights[prev][j];

                    used[last] = true;
                    cut.Add(last);
                    if (bestWeight == -1 || w[last] < bestWeight)
                    {
                        bestCut = cut;
                        bestWeight = w[last];
                    }
                }
                else
                {
                    for (var j = 0; j < n; j++)
                        w[j] += weights[last][j];
                    added[last] = true;
                }
            }
        }

        BestWeight = bestWeight;
        BestCut = bestCut;
    }
}