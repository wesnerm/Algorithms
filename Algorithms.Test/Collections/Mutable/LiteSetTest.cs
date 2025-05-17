using Set = Algorithms.Collections.LiteSet<object>;

namespace Algorithms.Collections;

/// <summary>
///     Summary description for TestClass.
/// </summary>
[TestFixture]
public class LiteSetTest
{
    HashSet<Set> Objects => new ();

    [SetUp]
    public void Setup()
    {
        numbers = new Set();
        numbers37 = new Set();
        strings = new Set();
        allWeak = new Set(50);
        allStrong = new Set(7);

        numbers.AddRange(strongOdds);
        numbers.AddRange(new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
        numbers37.AddRange(new object[] { 1, 2, 4, 5, 6, 8, 9, 10 });
        numbers37.AddRange(strongOdds);

        strings.AddRange(strongStrings);

        allWeak.AddRange(new object[] { 1, 2, 3, 4, 5, 6 });
        allStrong.AddRange(strongOdds);

        basket = new[] { numbers, numbers37, strings, allWeak, allStrong };

        empty = new Set();
        sample = new Set
        {
            1,
            0,
            -1
        };
        sample2 = new Set();
        sample2.AddRange(new object[] { 1, -1 });
        sample3 = new Set();
        sample3.AddRange(new object[] { -1, 2, 3 });
        sample4 = new Set();
        sample4.AddRange(new object[] { 4, 5, 6 });

    }

    private readonly object[] strongOdds = { 1, 3, 5, 7, 9 };

    private readonly object[] strongStrings =
    {
        "Alpha", "Beta", "Gamma", "Delta", "Epsilon",
        "Iota", "Kappa", "Lambda", "Mu"
    };

    private Set empty;
    private Set sample;
    private Set sample2;
    private Set sample3;
    private Set sample4;

    private Set numbers;
    private Set numbers37;
    private Set strings;
    private Set allWeak;
    private Set allStrong;
    private Set[] basket;

    public void Validate(Set set)
    {
        var count = set.Count;
        var actualCount = set.Count;
        Assert.IsTrue(count >= actualCount);
        Assert.IsTrue(actualCount >= 0);
    }

    [Test]
    public void Add()
    {
    }

    [Test]
    public void Adhoc()
    {
        var col = new Set();

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

        for (var i = 1; i < 30; i++)
            col.Add(i);
    }

    [Test]
    public void Clear()
    {
        foreach (var ws in basket)
        {
            ws.Clear();
            AreEqual(0, ws.Count);
            AreEqual(0, ws.Count);
        }
    }

    [Test]
    public void Clone()
    {
        var clone = allStrong.Clone();
        IsTrue(clone.Count == allStrong.Count);

        foreach (var o in clone)
            IsTrue(allStrong.Contains(o));
    }

    [Test]
    public void Construction()
    {
        var set1 = new Set();
        IsTrue(set1.Count == 0);
        IsTrue(set1.Count == 0);
    }

    [Test]
    public void Contains()
    {
        foreach (var i in strongOdds)
        {
            IsTrue(numbers.Contains(i));
            IsTrue(numbers.Contains((int)i));
            IsTrue(numbers[(int)i] == i);
            IsTrue(numbers[i] == i);
        }

        IsTrue(!numbers.Contains(0));
        IsTrue(numbers[0] == null);
    }

    [Test]
    public void ContainsSubset()
    {
        foreach (var set in Objects)
        {
            IsTrue(set.ContainsSubset(empty));
            if (set.Count > 0)
                IsFalse(empty.ContainsSubset(set));
        }

        foreach (var set1 in Objects)
            foreach (var set2 in Objects)
            {
                if (set1.ContainsSubset(set2))
                {
                    IsTrue(set1.Count == set2.Count || !set2.ContainsSubset(set1));

                    var tmp = Set.Difference(set1, set2);
                    AreEqual(set1.Count - set2.Count, tmp.Count);

                    tmp = Set.Union(set1, set2);
                    AreEqual(set1.Count, tmp.Count);

                    tmp = Set.Intersection(set1, set2);
                    AreEqual(set2.Count, tmp.Count);

                    tmp = Set.ExclusiveUnion(set1, set2);
                    AreEqual(set1.Count - set2.Count, tmp.Count);

                    AreEqual(set1.Count == set2.Count, set1.Equals(set2));
                }
                else
                {
                    IsFalse(ReferenceEquals(set1, set2));
                }
            }
    }

    [Test]
    public void Difference()
    {
        foreach (var set in Objects)
        {
            AreEqual(Set.Difference(set, empty).OrderBy(x => x), set.OrderBy(x => x));
            AreEqual(Set.Difference(empty, set), empty);
        }

        foreach (var set1 in Objects)
            foreach (var set2 in Objects)
            {
                var result = Set.Difference(set1, set2);

                foreach (var n in result)
                    IsTrue(set1.Contains(n) && !set2.Contains(n));

                foreach (var n in set1)
                    IsTrue(result.Contains(n) != set2.Contains(n));

                foreach (var n in set2)
                    IsTrue(!result.Contains(n));
            }
    }

    [Test]
    public void Enumerators()
    {
        var count = 0;
        foreach (var o in numbers)
        {
            IsNotNull(o);
            count++;
        }
        Assert.IsTrue(count >= numbers.Count);
        count = numbers.Count;
        Assert.IsTrue(count >= numbers.Count);
    }

    [Test]
    public void ExclusiveUnion()
    {
        foreach (var set in Objects)
            SetAreEqual(Set.ExclusiveUnion(set, empty), set);

        foreach (var set1 in Objects)
            foreach (var set2 in Objects)
            {
                var result = Set.ExclusiveUnion(set1, set2);
                SetAreEqual(result, Set.ExclusiveUnion(set2, set1));

                foreach (var n in result)
                    IsTrue(set1.Contains(n) != set2.Contains(n));

                foreach (var n in set1)
                    IsTrue(result.Contains(n) != set2.Contains(n));

                foreach (var n in set2)
                    IsTrue(result.Contains(n) != set1.Contains(n));
            }
    }

    [Test]
    public void Intersection()
    {
        foreach (var set in Objects)
            AreEqual(Set.Intersection(set, empty), empty);

        foreach (var set1 in Objects)
            foreach (var set2 in Objects)
            {
                var result = Set.Intersection(set1, set2);
                AreEqual(result, Set.Intersection(set2, set1));

                foreach (var n in result)
                    IsTrue(set1.Contains(n) && set2.Contains(n));

                foreach (var n in set1)
                    IsTrue(result.Contains(n) == set2.Contains(n));

                foreach (var n in set2)
                    IsTrue(result.Contains(n) == set1.Contains(n));
            }
    }

    [Test]
    public void IntersectsWith()
    {
        foreach (var set in Objects)
        {
            IsFalse(set.IntersectsWith(empty));
            IsFalse(empty.IntersectsWith(set));
        }

        foreach (var set1 in Objects)
            foreach (var set2 in Objects)
            {
                if (set1.IntersectsWith(set2))
                {
                    IsTrue(set2.IntersectsWith(set1));
                    var tmp = Set.Difference(set1, set2);
                    IsTrue(tmp.Count < set1.Count);

                    tmp = Set.Union(set1, set2);
                    IsTrue(tmp.Count < set1.Count + set2.Count);
                    IsTrue(tmp.Count >= set1.Count);

                    tmp = Set.Intersection(set1, set2);
                    IsTrue(tmp.Count >= 1);

                    tmp = Set.ExclusiveUnion(set1, set2);
                    IsTrue(tmp.Count < set1.Count + set2.Count);
                }
                else
                {
                    IsFalse(ReferenceEquals(set1, set2) && set1.Count != 0);

                    IsFalse(set2.IntersectsWith(set1));
                    var tmp = Set.Difference(set1, set2);
                    IsTrue(tmp.Count == set1.Count);

                    tmp = Set.Union(set1, set2);
                    IsTrue(tmp.Count == set1.Count + set2.Count);

                    tmp = Set.Intersection(set1, set2);
                    IsTrue(tmp.Count == 0);

                    tmp = Set.ExclusiveUnion(set1, set2);
                    IsTrue(tmp.Count == set1.Count + set2.Count);
                }
            }
    }

    [Test]
    public void Remove()
    {
        AreEqual(10, numbers.Count);
        numbers.RemoveRange(strongOdds);
        AreEqual(5, numbers.Count);
    }

    [Test]
    public void Union()
    {
        foreach (var set in Objects)
            SetAreEqual(Set.Union(set, empty), set);

        foreach (var set1 in Objects)
            foreach (var set2 in Objects)
            {
                var result = Set.Union(set1, set2);
                SetAreEqual(result, Set.Union(set2, set1));

                foreach (var n in result)
                    IsTrue(set1.Contains(n) || set2.Contains(n));

                foreach (var n in set1)
                    IsTrue(result.Contains(n));

                foreach (var n in set2)
                    IsTrue(result.Contains(n));
            }
    }

    private void SetAreEqual<T>(LiteSet<T> set1, LiteSet<T> set2)
    {
        AreEqual(set1.Count, set2.Count);

        var set = new HashSet<T>(set1);
        set.ExceptWith(set2);
        AreEqual(0, set.Count);
    }
}