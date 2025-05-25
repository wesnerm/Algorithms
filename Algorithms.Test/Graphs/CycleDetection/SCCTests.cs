using System.Text.RegularExpressions;

namespace Algorithms.Graphs;

using Graph2 = Dictionary<int, HashSet<int>>;

public class TarjanSCC
{
    public void Run(string file = @"d:\test\SCC.txt")
    {
        var g = new Graph2();

        Console.WriteLine("Reading....");
        foreach (string line in File.ReadLines(file)) {
            int[] split = Regex.Matches(line, @"\d+").Select(x => int.Parse(x.Value)).ToArray();
            int head = split[0];
            int tail = split[1];
            AddEdge(g, head, tail);
        }

        int max = g.Keys.Max() + 1;
        var g2 = new List<int>[max];
        for (int i = 0; i < max; i++)
            g2[i] = new List<int>();
        foreach (KeyValuePair<int, HashSet<int>> v in g)
            g2[v.Key].AddRange(v.Value);

        var t = new Thread(() => {
            Console.WriteLine("\nGabow Method");
            new Gabow(g);

            Console.WriteLine("\nTarjan Method");
            new Tarjan(g2);
        }, 0x10000000);
        t.Start();
        t.Join();

        Console.WriteLine("\nPress any key.");
        Console.ReadKey();
    }

    static HashSet<int> Get(Graph2 graph, int head, bool force = true)
    {
        if (graph.TryGetValue(head, out HashSet<int>? dict))
            return dict;
        if (force)
            graph[head] = dict = new HashSet<int>();
        return dict;
    }

    static void AddEdge(Graph2 g, int head, int tail)
    {
        HashSet<int> dict = Get(g, head);
        dict.Add(tail);
    }

    public static Graph2 Invert(Graph2 dict)
    {
        var graph = new Graph2();
        foreach (KeyValuePair<int, HashSet<int>> pair in dict)
        foreach (int tail in pair.Value)
            AddEdge(graph, tail, pair.Key);
        return graph;
    }
}