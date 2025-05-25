using WeakCollection = Algorithms.Collections.WeakCollection<object>;

namespace Algorithms.Collections.Weak;

/// <summary>
///     Summary description for TestClass.
/// </summary>
[TestFixture]
public class WeakCollectionTest
{
    [SetUp]
    public void Setup()
    {
        numbers = new WeakCollection();
        numbers37 = new WeakCollection();
        strings = new WeakCollection();
        allWeak = new WeakCollection(50);
        allStrong = new WeakCollection(7);

        numbers.AddRange(strongOdds);
        numbers.AddRange(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        numbers37.AddRange(new object[] { 1, 2, 4, 5, 6, 8, 9, 10 });
        numbers37.AddRange(strongOdds);
        strings.AddRange(strongStrings);
        allWeak.AddRange(new object[] { 1, 2, 3, 4, 5, 6 });
        allStrong.AddRange(strongOdds);

        basket = new[] { numbers, numbers37, strings, allWeak, allStrong };
    }

    readonly object[] strongOdds = new object[] { 1, 3, 5, 7, 9 };

    readonly object[] strongStrings =
    {
        "Alpha", "Beta", "Gamma", "Delta", "Epsilon",
        "Iota", "Kappa", "Lambda", "Mu",
    };

    WeakCollection<object> numbers;
    WeakCollection<object> numbers37;
    WeakCollection<object> strings;
    WeakCollection<object> allWeak;
    WeakCollection<object> allStrong;
    WeakCollection<object>[] basket;

    [Test]
    public void Construction()
    {
        var set1 = new WeakCollection();
        IsTrue(set1.Count == 0);
        IsTrue(set1.ActualCount == 0);
    }

    [Test]
    public void Clone()
    {
        WeakCollection clone = allStrong.Clone();
        IsTrue(clone.Count == allStrong.Count);

        foreach (object o in clone)
            IsTrue(allStrong.Contains(o));
    }

    [Test]
    public void Clear()
    {
        foreach (WeakCollection ws in basket) {
            ws.Clear();
            AreEqual(0, ws.Count);
            AreEqual(0, ws.ActualCount);
        }
    }

    [Test]
    public void Contains()
    {
        foreach (object i in strongOdds) {
            IsTrue(numbers.Contains(i));
            IsTrue(numbers.Contains((int)i));
        }

        IsTrue(!numbers.Contains(0));
    }

    [Test]
    public void Properties()
    {
        foreach (WeakCollection ws in basket) IsTrue(ws.Count >= ws.ActualCount);
    }

    [Test]
    public void Enumerators()
    {
        int count = 0;
        foreach (object o in numbers) {
            IsNotNull(o);
            count++;
        }

        IsTrue(count >= numbers.Count);
        count = numbers.Count;
        IsTrue(count >= numbers.ActualCount);
    }
}