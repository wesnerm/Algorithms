namespace Algorithms.Graphs;

public class FloydWarshall
{
    public static long[,] FindAllPairsShortestPath(IList<Edge> edges, int n)
    {
        var table = new long[n, n];
        for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                table[i, j] = i == j ? 0 : int.MaxValue;

        foreach (var e in edges)
            table[e.V1, e.V2] = e.Cost;

        return FindAllPairsShortestPath(table);
    }

    public static long[,] FindAllPairsShortestPath(long[,] table)
    {
        int n = table.GetLength(0);
        for (var k = 0; k < n; k++)
            for (var i = 0; i < n; i++)
            {
                var dik = table[i, k];
                for (var j = 0; j < n; j++)
                {
                    var d = dik + table[k, j];
                    if (table[i, j] > d) table[i, j] = d;
                }
            }
        return table;
    }

    public static int[,] FindAllPairsShortestPath(int[,] table)
    {
        int n = table.GetLength(0);
        for (var k = 0; k < n; k++)
            for (var i = 0; i < n; i++)
            {
                long dik = table[i, k];
                for (var j = 0; j < n; j++)
                {
                    var d = dik + table[k, j];
                    if (d < table[i, j]) table[i, j] = (int)d;
                }
            }

        return table;
    }

    public static long[,] FindAllPairsShortestPath(IList<int>[] edges)
    {
        int n = edges.Length;
        var table = new long[n, n];
        for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                table[i, j] = i == j ? 0 : int.MaxValue;

        for (var i = 0; i < edges.Length; i++)
            foreach (var v in edges[i])
                table[i, v] = 1;

        return FindAllPairsShortestPath(table);
    }

    public static bool HasNegativeCycles(long[,] table)
    {
        int n = table.GetLength(0);
        for (var i = 0; i < n; i++)
        {
            if (table[i, i] < 0)
                return true;
        }
        return false;
    }

    public static long ShortestPath(long[,] table)
    {
        int n = table.GetLength(0);
        long shortestpath = long.MaxValue;
        for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
                if (i != j) shortestpath = Math.Min(shortestpath, table[i, j]);
        return shortestpath;
    }

    public class Edge
    {
        public int Cost;
        public int V1;
        public int V2;
    }
}
