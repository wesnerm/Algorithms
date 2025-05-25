namespace Algorithms.RangeQueries;

using T = int;

[TestFixture]
public class RangeMinimumQueryTest
{
    RangeMinimumQuery Sample()
    {
        int[] a = { 7, 2, 3, 0, 5, 10, 3, 12, 18 };
        return new RangeMinimumQuery(a);
    }

    void CheckRanges(params T[] array)
    {
        var rmq = new RangeMinimumQuery(array);

        for (int i = 0; i < rmq.Length; i++)
        for (int j = i; j < rmq.Length; j++) {
            int expected = T.MaxValue;
            int expectedIndex = -1;
            for (int k = i; k <= j; k++) {
                int v = rmq.Array[k];
                if (v < expected) {
                    expected = v;
                    expectedIndex = k;
                }
            }

            AreEqual(expected, rmq.GetMin(i, j));
            AreEqual(expectedIndex, rmq.GetArgMin(i, j));
        }
    }

    [Test]
    public void RMQTest()
    {
        RangeMinimumQuery rmq = Sample();
        CheckRanges(rmq.Array);
    }

    [Test]
    public void Ones()
    {
        CheckRanges(1);
    }

    [Test]
    public void Twos()
    {
        CheckRanges(5, 1);
    }
}