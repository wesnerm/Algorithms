using Algorithms.Properties;

namespace Algorithms.Graphs;

using static FloydWarshall;

[TestFixture]
public class FloydWarshallTest
{
    [Test]
    public void FloydWarshall1Test()
    {
        long[,] table = ReadTable(@"FWData/g1.txt");
        bool hasCycles = HasNegativeCycles(table);
        IsTrue(hasCycles);
    }

    [Test]
    public void FloydWarshall2Test()
    {
        long[,] table = ReadTable(@"FWData/g2.txt");
        bool hasCycles = HasNegativeCycles(table);
        IsTrue(hasCycles);
    }

    [Test]
    public void FloydWarshall3Test()
    {
        long[,] table = ReadTable(@"FWData/g3.txt");
        bool hasCycles = HasNegativeCycles(table);
        long spath = ShortestPath(table);
        IsFalse(hasCycles);
        AreEqual(spath, -19);
    }

    public long[,] ReadTable(string s)
    {
        using (TextReader file = AssemblyInfo.LoadEmbeddedText(s)) {
            string? line = file.ReadLine();
            string[] split = line!.Split();
            int n = int.Parse(split[0]);
            int nEdges = int.Parse(split[1]);

            var edges = new List<Edge>(nEdges);
            while ((line = file.ReadLine()) != null) {
                split = line.Split();
                var edge = new Edge
                {
                    V1 = int.Parse(split[0]) - 1,
                    V2 = int.Parse(split[1]) - 1,
                    Cost = int.Parse(split[2]),
                };
                edges.Add(edge);
            }

            return FindAllPairsShortestPath(edges, n);
        }
    }
}