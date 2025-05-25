using WeakSet = Algorithms.Collections.WeakSet<object>;

namespace Algorithms.Collections.Weak;

/// <summary>
///     Summary description for TestClass.
/// </summary>
[TestFixture]
public class WeakSetTest
{
    [SetUp]
    public void Setup()
    {
        _numbers = new WeakSet();
        _numbers37 = new WeakSet();
        _strings = new WeakSet();
        _allWeak = new WeakSet(50);
        _allStrong = new WeakSet(7);

        _numbers.AddRange(_strongOdds);
        _numbers.AddRange(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        _numbers37.AddRange(new object[] { 1, 2, 4, 5, 6, 8, 9, 10 });
        _numbers37.AddRange(_strongOdds);
        _strings.AddRange(_strongStrings);
        _allWeak.AddRange(new object[] { 1, 2, 3, 4, 5, 6 });
        _allStrong.AddRange(_strongOdds);

        _basket = new[] { _numbers, _numbers37, _strings, _allWeak, _allStrong };
    }

    readonly object[] _strongOdds = { 1, 3, 5, 7, 9 };

    readonly object[] _strongStrings =
    {
        "Alpha", "Beta", "Gamma", "Delta", "Epsilon",
        "Iota", "Kappa", "Lambda", "Mu",
    };

    WeakSet _numbers;
    WeakSet _numbers37;
    WeakSet _strings;
    WeakSet _allWeak;
    WeakSet _allStrong;
    WeakSet[] _basket;

    public void Validate(WeakSet set)
    {
        int count = set.Count;
        int actualCount = set.ActualCount;
        IsTrue(count >= actualCount);
        IsTrue(actualCount >= 0);
    }

    [Test]
    public void Add() { }

    [Test]
    public void Adhoc()
    {
        var col = new WeakSet();

        object one = 1;
        object two = 2;
        object three = 3;

        object test;

        test = col.Add(one);
        IsTrue(test == one);

        test = col.Add(two);
        IsTrue(test == two);

        test = col.Add(2);
        IsTrue(test == two);

        test = col.Add(three);
        IsTrue(test == three);

        col.Remove(2);

        IsTrue(col.Contains(one));
        IsTrue(col.Contains(1));
        IsTrue(!col.Contains(two));
        IsTrue(col[one] == one);
        IsTrue(col[two] == null);
        IsTrue(col[1] == one);

        for (int i = 1; i < 30; i++)
            col.Add(i);

        GC.Collect();

        AreEqual(2, col.ActualCount);
    }

    [Test]
    public void Clear()
    {
        foreach (WeakSet ws in _basket) {
            ws.Clear();
            AreEqual(0, ws.Count);
            AreEqual(0, ws.ActualCount);
        }
    }

    [Test]
    public void Clone()
    {
        WeakSet clone = _allStrong.Clone();
        IsTrue(clone.Count == _allStrong.Count);

        foreach (object o in clone)
            IsTrue(_allStrong.Contains(o));
    }

    [Test]
    public void Construction()
    {
        var set1 = new WeakSet();
        IsTrue(set1.Count == 0);
        IsTrue(set1.ActualCount == 0);
    }

    [Test]
    public void Contains()
    {
        foreach (object i in _strongOdds) {
            IsTrue(_numbers.Contains(i));
            IsTrue(_numbers.Contains((int)i));
            IsTrue(_numbers[(int)i] == i);
            IsTrue(_numbers[i] == i);
        }

        IsTrue(!_numbers.Contains(0));
        IsTrue(_numbers[0] == null);
    }

    [Test]
    public void Enumerators()
    {
        int count = 0;
        foreach (object o in _numbers) {
            IsNotNull(o);
            count++;
        }

        IsTrue(count >= _numbers.Count);
        count = _numbers.Count;
        IsTrue(count >= _numbers.ActualCount);
    }

    [Test]
    public void Properties()
    {
        foreach (WeakSet ws in _basket) IsTrue(ws.Count >= ws.ActualCount);
    }

    [Test]
    public void Remove()
    {
        GC.Collect();
        AreEqual(_strongOdds.Length, _numbers.ActualCount);
        _numbers.RemoveRange(_strongOdds);
        AreEqual(0, _numbers.ActualCount);
    }

    [Test]
    public void WeakRefs()
    {
        GC.Collect();
        AreEqual(_strongOdds.Length, _numbers.ActualCount);
        AreEqual(2, _numbers37.ActualCount);
        IsTrue(_numbers37[3] != null);
        IsTrue(_numbers37.Contains(3));
        IsTrue(_numbers37[7] != null);
        IsTrue(_numbers37.Contains(7));
        IsTrue(_numbers37[1] == null);
        IsTrue(!_numbers37.Contains(1));
        AreEqual(_strongOdds.Length, _allStrong.ActualCount);
        AreEqual(0, _allWeak.ActualCount);
    }
}