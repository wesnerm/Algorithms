using Algorithms.Collections;

namespace Algorithms.Graphs;

public class RandomizedMinCut
{
    public void Run()
    {
        var graph = new Graph();

        foreach (string line in File.ReadAllLines(@"d:\test\KargerAdj.txt")) {
            string[] split = line.Split();
            int v = int.Parse(split[0]);
            Debug.Assert(split.Length > 0);
            for (int i = 1; i < split.Length; i++)
                graph.AddEdge(v, int.Parse(split[i]));
        }

        int minCut = int.MaxValue;

        int max = graph.edgeCount;
        max *= 4 * max;

        Debug.Assert(graph.Edges.Count() == graph.edgeCount);
        for (int i = 0; i < max; i++) {
            Graph g = DoMinCut(graph.Clone());
            if (minCut > g.edgeCount) {
                minCut = g.edgeCount;
                Console.WriteLine("Minimum Cut - " + minCut);
                foreach (KeyValuePair<int, int> edge in g.Edges)
                    Console.WriteLine(edge);
            }
        }
    }

    Graph DoMinCut(Graph g)
    {
        while (g.vertexCount > 2) {
            KeyValuePair<int, int> edge = g.Edges.RandomElement();
            int v1 = edge.Key;
            int v2 = edge.Value;
            g.MergeVertex(v1, v2);
        }

        return g;
    }

    public class Graph
    {
        public int[,] array = new int[41, 41];
        public int edgeCount;

        bool flip;
        public int vertexCount;
        public int[] vertexEdges = new int[41];

        public IEnumerable<KeyValuePair<int, int>> Edges {
            get
            {
                for (int i = 0; i < 41; i++)
                    //if (vertexEdges[i] == 0)
                    //    continue;
                for (int j = i; j < 41; j++)
                for (int k = array[i, j] - 1; k >= 0; k--)
                    yield return new KeyValuePair<int, int>(i, j);
            }
        }

        public Graph Clone()
        {
            var g = (Graph)MemberwiseClone();
            g.array = (int[,])g.array.Clone();
            g.vertexEdges = (int[])g.vertexEdges.Clone();
            return g;
        }

        public void AddEdge(int x, int y)
        {
            if (x == y)
                return;
            if (x > y)
                Swap(ref x, ref y);
            array[x, y]++;
            edgeCount++;
            if (++vertexEdges[x] == 1)
                vertexCount++;
            if (++vertexEdges[y] == 1)
                vertexCount++;
        }

        public void RemoveEdge(int x, int y)
        {
            if (x == y)
                return;
            if (x > y)
                Swap(ref x, ref y);
            if (array[x, y] > 0) {
                array[x, y]--;
                edgeCount--;
                if (--vertexEdges[x] == 0)
                    vertexCount--;
                if (--vertexEdges[y] == 0)
                    vertexCount--;
            }
        }

        public IEnumerable<int> ToVertex(int x)
        {
            for (int i = 0; i < 41; i++)
            for (int j = Get(i, x) - 1; j >= 0; j--)
                yield return i;
        }

        public int Get(int x, int y)
        {
            if (x < y)
                return array[x, y];
            return array[y, x];
        }

        public void MergeVertex(int start, int end)
        {
            flip = !flip;
            if (flip)
                Swap(ref start, ref end);
            foreach (int v in ToVertex(start)) {
                RemoveEdge(v, start);
                if (v != start)
                    AddEdge(v, end);
            }
        }
    }
}