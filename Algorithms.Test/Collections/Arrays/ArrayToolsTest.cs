namespace Algorithms.Collections;

[TestFixture]
public class ArrayToolsTest
{
    /// <summary>
    ///     Test for NewArray(int n, int m)
    /// </summary>
    [Test]
    [Ignore("NewArray is not yet implemented")]
    public void NewArrayTest()
    {
        // var obj = new ArrayTools();
        // var expected = obj.NewArray();
        // var actual = default(T[][]);
        // Assert.AreEqual(expected, actual, "NewArray");
        Fail();
    }

    /// <summary>
    ///     Test for ConvertArrayForm(this T[,] array)
    /// </summary>
    [Test]
    [Ignore("ConvertArrayForm is not yet implemented")]
    public void ConvertArrayFormTest()
    {
        // var obj = new ArrayTools();
        // var expected = obj.ConvertArrayForm();
        // var actual = default(T[][]);
        // Assert.AreEqual(expected, actual, "ConvertArrayForm");
        Fail();
    }

    /// <summary>
    ///     Test for ConvertArrayForm(this T[][] array)
    /// </summary>
    [Test]
    [Ignore("ConvertArrayForm2 is not yet implemented")]
    public void ConvertArrayForm2Test()
    {
        // var obj = new ArrayTools();
        // var expected = obj.ConvertArrayForm();
        // var actual = default(T[,]);
        // Assert.AreEqual(expected, actual, "ConvertArrayForm");
        Fail();
    }

    /// <summary>
    ///     Test for SwapRow(this T[,] array, int r1, int r2)
    /// </summary>
    [Test]
    [Ignore("SwapRow is not yet implemented")]
    public void SwapRowTest()
    {
        // var obj = new ArrayTools();
        // var expected = obj.SwapRow();
        // var actual = default(void);
        // Assert.AreEqual(expected, actual, "SwapRow");
        Fail();
    }

    /// <summary>
    ///     Test for CloneArray(this T[] list)
    /// </summary>
    [Test]
    public void CloneArrayTest()
    {
        IsNull(ArrayTools.CloneArray<object>(null));
        AreEqual(new[] { 1, 2, 3 }.CloneArray(), new[] { 1, 2, 3 });
    }

    /// <summary>
    ///     Test for Map(this T[] list, Func<T, T> func, IEqualityComparer<T> comparer = null)
    /// </summary>
    [Test]
    public void MapTest()
    {
        int[] array = new[]
        {
            1, 2, 3,
        };

        IsNull(((object[])null).Map(x => x));
        AreSame(array, array.Map(x => x));
        CollectionAssert.AreEqual(new[] { 2, 3, 4 }, array.Map(x => x + 1));
        AreNotSame(array, array.Map(x => x + 1));

        var list = new List<int>
        {
            1,
            2,
            3,
        };

        IsNull(((List<object>)null).Map(x => x));
        AreSame(list, list.Map(x => x));
        CollectionAssert.AreEqual(new[] { 2, 3, 4 }, list.Map(x => x + 1));
        AreNotSame(list, list.Map(x => x + 1));
    }
    // private ArrayTools sample;
}