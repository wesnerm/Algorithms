namespace Algorithms.Graphs;

[TestFixture]
public class EulerTourTest
{
    EulerTour Sample(bool middle)
    {
        //graph G {
        //    1--9
        //    4--8
        //    9--7
        //    2--4
        //    3--6
        //    1--2
        //    9--10
        //    2--3
        //    4--5
        // }

        return new EulerTour(new[]
        {
            new List<int>(),
            new List<int> { 2, 9 },
            new List<int> { 1, 3, 4 },
            new List<int> { 2, 6 },
            new List<int> { 2, 5, 8 },
            new List<int> { 4 },
            new List<int> { 3 },
            new List<int> { 9 },
            new List<int> { 4 },
            new List<int> { 1, 7, 10 },
            new List<int> { 9 },
        }, 1, middle);
    }

    [Test]
    public void AaTest()
    {
        // Prevents startup time contamination with test times
    }

    [Test]
    public void MiddleTest()
    {
        EulerTour tree = Sample(true);
        AreEqual(new[] { 1, 2, 3, 6, 3, 2, 4, 5, 4, 8, 4, 2, 1, 9, 7, 9, 10, 9, 1, 0, 0 },
            tree.Trace);
    }

    [Test]
    public void MiddleParentTest()
    {
        EulerTour tree = Sample(true);
        AreEqual(new[] { 0, -1, 1, 2, 2, 4, 3, 9, 4, 1, 9 }, tree.Parent);
    }

    [Test]
    public void MiddleDepthTest()
    {
        EulerTour tree = Sample(true);
        AreEqual(new[] { 0, 0, 1, 2, 2, 3, 3, 2, 3, 1, 2 }, tree.Depth);
    }

    [Test]
    public void MiddleBeginEndTest()
    {
        EulerTour tree = Sample(true);
        CheckBeginEnd(tree);
    }

    void CheckBeginEnd(EulerTour tree)
    {
        for (int i = 1; i <= 10; i++) {
            AreEqual(i, tree.Trace[tree.Begin[i]]);
            AreEqual(i, tree.Trace[tree.End[i]]);
            for (int j = 0; j < tree.Begin[i]; j++)
                AreNotEqual(i, tree.Trace[j]);
            for (int j = tree.End[i] + 1; j < tree.Trace.Length; j++)
                AreNotEqual(i, tree.Trace[j]);
        }
    }

    [Test]
    public void DoublingTest()
    {
        EulerTour tree = Sample(false);
        AreEqual(new[] { 1, 2, 3, 6, 6, 3, 4, 5, 5, 8, 8, 4, 2, 9, 7, 7, 10, 10, 9, 1, 0, 0 },
            tree.Trace);
    }

    [Test]
    public void DoublingBeginTest()
    {
        EulerTour tree = Sample(false);
        CheckBeginEnd(tree);
    }

    [Test]
    public void DoublingParentTest()
    {
        EulerTour tree = Sample(true);
        AreEqual(new[] { 0, -1, 1, 2, 2, 4, 3, 9, 4, 1, 9 }, tree.Parent);
    }

    [Test]
    public void DoublingDepthTest()
    {
        EulerTour tree = Sample(true);
        AreEqual(new[] { 0, 0, 1, 2, 2, 3, 3, 2, 3, 1, 2 }, tree.Depth);
    }
}