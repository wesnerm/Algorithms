namespace Algorithms.RangeQueries.Succinct;

[TestFixture]
public class WaveletTreeTest
{
    [Test]
    public void SimpleTest()
    {
        int[] array = { 1, 2, 3, 4, 5 };

        var wt = new WaveletTree(array);

        AreEqual(2, wt.CountLessEqual(0, 2, 2));
        AreEqual(4, wt.Kth(0, 4, 4));
        AreEqual(1, wt.Count(0, 4, 3));
    }
}