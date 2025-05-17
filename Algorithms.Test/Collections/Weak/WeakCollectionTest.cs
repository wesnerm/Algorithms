
using System.Diagnostics;
using WeakCollection = Algorithms.Collections.WeakCollection<object>;

namespace Algorithms.Collections.Weak;

/// <summary>
/// Summary description for TestClass.
/// </summary>
[TestFixture]
public class WeakCollectionTest
{
    #region Variables
    object[] strongOdds = new object [] { 1, 3, 5, 7, 9 };
    object[] strongStrings = new string[] 
        {
            "Alpha", "Beta", "Gamma", "Delta", "Epsilon",
            "Iota", "Kappa", "Lambda", "Mu" 
        };

    WeakCollection<object> numbers ;
    WeakCollection<object> numbers37 ;
    WeakCollection<object> strings ;
    WeakCollection<object> allWeak ;
    WeakCollection<object> allStrong ;
    WeakCollection<object>[] basket;

    #endregion

    #region Construction
    public WeakCollectionTest()
    {
    }
    #endregion

    #region WeakListup And Teardown
    [SetUp]
    public void Setup()
    {
        numbers = new WeakCollection();
        numbers37 = new WeakCollection();
        strings = new WeakCollection();
        allWeak = new WeakCollection(50);
        allStrong = new WeakCollection(7);

        numbers.AddRange( strongOdds );
        numbers.AddRange( new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } );
        numbers37.AddRange( new object[] { 1, 2, 4, 5, 6, 8, 9, 10 } );
        numbers37.AddRange( strongOdds );
        strings.AddRange(  strongStrings );
        allWeak.AddRange( new object[] { 1, 2, 3, 4, 5, 6 } );
        allStrong.AddRange( strongOdds);

        basket = new WeakCollection[] { numbers, numbers37, strings, allWeak, allStrong };

    }            

    #endregion

    #region Test

    [Test]
    public void Construction()
    {
        WeakCollection<object> set1 = new WeakCollection();
        Assert.IsTrue(set1.Count == 0);
        Assert.IsTrue(set1.ActualCount == 0);
    }

    [Test]
    public void Clone()
    {
        WeakCollection clone = allStrong.Clone();
        Assert.IsTrue(clone.Count == allStrong.Count);

        foreach (object o in clone)
            Assert.IsTrue(allStrong.Contains(o));
    }
    
    [Test]
    public void Clear()
    {
        foreach (WeakCollection ws in basket)
        {
            ws.Clear();
            Assert.AreEqual(0, ws.Count);
            Assert.AreEqual(0, ws.ActualCount);
        }
    }

    [Test]
    public void Contains()
    {
        foreach (object i in strongOdds)
        {
            Assert.IsTrue(numbers.Contains(i));
            Assert.IsTrue(numbers.Contains((int)i));
        }

        Assert.IsTrue(!numbers.Contains(0));
    }

    [Test]
    public void Properties()
    {
        foreach (WeakCollection ws in basket)
        {
            Assert.IsTrue(ws.Count >= ws.ActualCount);
        }
    }

    [Test]
    public void Enumerators()
    {
        int count = 0;
        foreach (object o in numbers)
        {
            Assert.IsNotNull(o);
            count++;
        }
        Assert.IsTrue(count >= numbers.Count);
        count = numbers.Count;
        Assert.IsTrue(count >= numbers.ActualCount);
    }

    #endregion
}
