namespace Algorithms.Graphs;

[TestFixture]
public class EulerianPathTest
{
    [Test]
    public void DirectedOddTest()
    {
        var euler = new EulerianPath(3, true);
        euler.AddEdge(0, 1);
        euler.AddEdge(1, 2);

        List<int>? path = euler.FindPath(0);
        AreEqual(new[] { 0, 1, 2 }, path);

        path = euler.FindPath(1);
        Null(path);

        path = euler.FindPath(2);
        Null(path);
    }

    [Test]
    public void DirectedEvenTest()
    {
        var euler = new EulerianPath(3, true);
        euler.AddEdge(0, 1);
        euler.AddEdge(1, 2);
        euler.AddEdge(2, 0);

        List<int> path = euler.FindPath(0);
        AreEqual(new[] { 0, 1, 2, 0 }, path);

        path = euler.FindPath(1);
        AreEqual(new[] { 1, 2, 0, 1 }, path);

        path = euler.FindPath(2);
        AreEqual(new[] { 2, 0, 1, 2 }, path);
    }

    [Test]
    public void UndirectedOddTest()
    {
        var euler = new EulerianPath(3);
        euler.AddEdge(0, 1);
        euler.AddEdge(1, 2);

        List<int>? path = euler.FindPath(0);
        AreEqual(new[] { 0, 1, 2 }, path);

        path = euler.FindPath(1);
        Null(path);

        path = euler.FindPath(2);
        AreEqual(new[] { 2, 1, 0 }, path);
    }

    [Test]
    public void UndirectedEvenTest()
    {
        var euler = new EulerianPath(3);
        euler.AddEdge(0, 1);
        euler.AddEdge(1, 2);
        euler.AddEdge(2, 0);

        List<int> path = euler.FindPath(0);
        AreEqual(new[] { 0, 1, 2, 0 }, path);

        path = euler.FindPath(1);
        AreEqual(new[] { 1, 0, 2, 1 }, path);

        path = euler.FindPath(2);
        AreEqual(new[] { 2, 1, 0, 2 }, path);
    }
}