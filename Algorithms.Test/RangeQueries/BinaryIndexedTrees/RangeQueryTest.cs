namespace Algorithms.RangeQueries;

[TestFixture]
public class RangeQueryTest
{
    [Test]
    public void RangeUpdateBasicTest()
    {
        var bit = new RangeQuery(30);
        bit.AddInclusive(5, 15, 7);
        AreEqual(0, bit.SumInclusive(3, 3));
        AreEqual(7, bit.SumInclusive(5, 5));
        AreEqual(7, bit.SumInclusive(10, 10));
        AreEqual(7, bit.SumInclusive(15, 15));
        AreEqual(42, bit.SumInclusive(5, 10));
        AreEqual(42, bit.SumInclusive(3, 10));
        AreEqual(21, bit.SumInclusive(8, 10));
        AreEqual(77, bit.SumInclusive(5, 15));
        AreEqual(0, bit.SumInclusive(20, 20));
    }

    /// <summary>
    ///     Test for RangeUpdate(int i, int j, int v)
    /// </summary>
    [Test]
    public void RangeUpdateTest()
    {
        var bit = new RangeQuery(30);
        bit.AddInclusive(10, 20, 5);
        bit.AddInclusive(5, 15, 7);
        AreEqual(0, bit.SumInclusive(3, 3));
        AreEqual(7, bit.SumInclusive(5, 5));
        AreEqual(7, bit.SumInclusive(8, 8));
        AreEqual(12, bit.SumInclusive(10, 10));
        AreEqual(12, bit.SumInclusive(12, 12));
        AreEqual(12, bit.SumInclusive(15, 15));
        AreEqual(5, bit.SumInclusive(17, 17));
        AreEqual(5, bit.SumInclusive(20, 20));
        AreEqual(0, bit.SumInclusive(25, 25));
    }

    /// <summary>
    ///     Test for QueryRange(int b)
    /// </summary>
    [Test]
    [Ignore("QueryRange is not yet implemented")]
    public void QueryRangeTest()
    {
        // var obj = new RangeBit();
        // var expected = obj.QueryRange();
        // var actual = default(int);
        // Assert.AreEqual(expected, actual, "QueryRange");
        Fail();
    }

    /// <summary>
    ///     Test for QueryRange(int i, int j, int v)
    /// </summary>
    [Test]
    [Ignore("QueryRange2 is not yet implemented")]
    public void QueryRange2Test()
    {
        // var obj = new RangeBit();
        // var expected = obj.QueryRange();
        // var actual = default(int);
        // Assert.AreEqual(expected, actual, "QueryRange");
        Fail();
    }
    // private RangeBit sample;
}