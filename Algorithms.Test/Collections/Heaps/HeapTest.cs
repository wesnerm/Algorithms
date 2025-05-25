namespace Algorithms.Collections;

using static Assert;

[TestFixture]
public class HeapTest
{
    [SetUp]
    public void Setup()
    {
        empty = new IndexedMinHeap<int>();
        sample = new IndexedMinHeap<int>();
        reverse = new IndexedMinHeap<int>
        {
            ReverseSort = true,
        };
        odds = new IndexedMinHeap<int>();
        evens = new IndexedMinHeap<int>();

        sample.AddRange(2, 6, 1, 3, 5, 4);
        reverse.AddRange(2, 6, 1, 3, 5, 4);
        odds.AddRange(9, 7, 5, 3, 1);
        evens.AddRange(2, 4, 6, 8, 10);
    }

    IndexedMinHeap<int> sample;
    IndexedMinHeap<int> reverse;
    IndexedMinHeap<int> odds;
    IndexedMinHeap<int> evens;
    IndexedMinHeap<int> empty;

    /// <summary>
    ///     Test for Clear()
    /// </summary>
    [Test]
    public void Clear()
    {
        sample.Clear();
        AreEqual(0, sample.Count);
    }

    /// <summary>
    ///     Test for Contains(T obj)
    /// </summary>
    [Test]
    public void Contains()
    {
        IsFalse(empty.Contains(1));
        IsFalse(evens.Contains(5));
        IsTrue(odds.Contains(5));
    }

    /// <summary>
    ///     Test for Count
    /// </summary>
    [Test]
    public void Count()
    {
        AreEqual(6, sample.Count);
        AreEqual(6, reverse.Count);
        AreEqual(5, odds.Count);
        AreEqual(5, evens.Count);
        AreEqual(0, empty.Count);
    }

    /// <summary>
    ///     Test for Dequeue()
    /// </summary>
    [Test]
    public void Dequeue()
    {
        AreEqual(1, sample.Dequeue());
        AreEqual(6, reverse.Dequeue());
        AreEqual(1, odds.Dequeue());
        AreEqual(2, evens.Dequeue());
    }

    /// <summary>
    ///     Test for Enqueue(T value)
    /// </summary>
    [Test]
    public void Enqueue()
    {
        sample.Enqueue(8);
        sample.Enqueue(0);
        AreEqual(8, sample.Count);
        AreEqual(0, sample.Top());
    }

    /// <summary>
    ///     Test for ExtractSortedList()
    /// </summary>
    [Test]
    public void ExtractSortedList()
    {
        AreEqual(new int[] { }, empty.ExtractSortedList().ToArray());
        AreEqual(new[] { 1, 2, 3, 4, 5, 6 }, sample.ExtractSortedList().ToArray());
        AreEqual(new[] { 6, 5, 4, 3, 2, 1 }, reverse.ExtractSortedList().ToArray());
        AreEqual(new[] { 1, 3, 5, 7, 9 }, odds.ExtractSortedList().ToArray());
        AreEqual(new[] { 2, 4, 6, 8, 10 }, evens.ExtractSortedList().ToArray());
        IsTrue(sample.IsEmpty);
    }

    /// <summary>
    ///     Test for Top()
    /// </summary>
    [Test]
    public void FindMinEmpty()
    {
        Throws<InvalidOperationException>(() => empty.Top());
    }

    /// <summary>
    ///     Test for IsEmpty
    /// </summary>
    [Test]
    public void IsEmpty()
    {
        IsFalse(sample.IsEmpty);
        IsFalse(reverse.IsEmpty);
        IsFalse(odds.IsEmpty);
        IsFalse(evens.IsEmpty);
        IsTrue(empty.IsEmpty);
    }

    /// <summary>
    ///     Test for Limit
    /// </summary>
    [Test]
    public void Limit()
    {
        var list = new IndexedMinHeap<int>
        {
            Limit = 3,
        };
        list.AddRange(3, 5, 1, 6, 2, 4);
        AreEqual(4, list.Top());
        int[] array = list.ExtractSortedList().ToArray();
        AreEqual(3, array.Length);
        AreEqual(new[] { 4, 5, 6 }, array);
    }

    /// <summary>
    ///     Test for Remove(T value)
    /// </summary>
    [Test]
    public void Remove()
    {
        IsFalse(evens.Remove(1));
        IsTrue(sample.Remove(5));
        IsTrue(sample.Remove(1));
        AreEqual(4, sample.Count);
        AreEqual(2, sample.Top());
        AreEqual(new[] { 2, 3, 4, 6 }, sample.ExtractSortedList().ToArray());
    }

    /// <summary>
    ///     Test for ReverseSort
    /// </summary>
    [Test]
    public void ReverseSort()
    {
        var list = new IndexedMinHeap<int>
        {
            ReverseSort = true,
        };
        list.AddRange(3, 5, 1, 6, 2, 4);
        AreEqual(6, list.Top());
        int[] array = list.ExtractSortedList().ToArray();
        AreEqual(new[] { 6, 5, 4, 3, 2, 1 }, array);
    }

    /// <summary>
    ///     Test for Top()
    /// </summary>
    [Test]
    public void Top()
    {
        AreEqual(1, sample.Top());
        AreEqual(6, reverse.Top());
        AreEqual(1, odds.Top());
        AreEqual(2, evens.Top());
    }
}