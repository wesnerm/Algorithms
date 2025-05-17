namespace Algorithms.Graphs;

using Algorithms.Properties;
using static FloydWarshall;

[TestFixture]
public class FloydWarshallTest
{
    #region Tests

    [Test]
    public void FloydWarshall1Test()
    {
        var table = ReadTable(@"FWData/g1.txt");
        bool hasCycles = HasNegativeCycles(table);
        Assert.IsTrue(hasCycles);
    }

    [Test]
    public void FloydWarshall2Test()
    {
        var table = ReadTable(@"FWData/g2.txt");
        bool hasCycles = HasNegativeCycles(table);
        Assert.IsTrue(hasCycles);
    }

    [Test]
    public void FloydWarshall3Test()
    {
        var table = ReadTable(@"FWData/g3.txt");
        bool hasCycles = HasNegativeCycles(table);
        var spath = ShortestPath(table);
        Assert.IsFalse(hasCycles);
        Assert.AreEqual(spath, -19);
    }

    public long[,] ReadTable(string s)
    {
        using (var file = AssemblyInfo.LoadEmbeddedText(s))
        {
            string? line = file.ReadLine();
            string[] split = line!.Split();
            int n = int.Parse(split[0]);
            int nEdges = int.Parse(split[1]);

            var edges = new List<Edge>(nEdges);
            while ((line = file.ReadLine()) != null)
            {
                split = line.Split();
                var edge = new Edge
                {
                    V1 = int.Parse(split[0]) - 1,
                    V2 = int.Parse(split[1]) - 1,
                    Cost = int.Parse(split[2])
                };
                edges.Add(edge);
            }

            return FindAllPairsShortestPath(edges, n);
        }
    }

    #endregion
}