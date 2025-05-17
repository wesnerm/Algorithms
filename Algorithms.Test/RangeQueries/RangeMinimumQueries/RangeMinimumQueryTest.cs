
namespace Algorithms.RangeQueries;

using T = System.Int32;

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
        for (int j = i; j < rmq.Length; j++)
        {
            var expected = T.MaxValue;
            var expectedIndex = -1;
            for (int k = i; k <= j; k++)
            {
                var v = rmq.Array[k];
                if (v < expected)
                {
                    expected = v;
                    expectedIndex = k;
                }
            }

            Assert.AreEqual(expected, rmq.GetMin(i, j));
            Assert.AreEqual(expectedIndex, rmq.GetArgMin(i, j));
        }
    }

    [Test]
    public void RMQTest()
    {
        var rmq = Sample();
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
        CheckRanges(5,1);
    }

}
