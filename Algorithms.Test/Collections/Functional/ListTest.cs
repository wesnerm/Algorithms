
using CharList = Algorithms.Collections.Rope<char>.List;

namespace Algorithms.Collections;

/// <summary>
///     Summary description for TestClass.
/// </summary>
[TestFixture]
public class ListTest 
{
    [SetUp]
    public void Setup()
    {
        _emptyList = new CharList();
        _compressedList = new CharList();
        _shortList = new CharList();
        _longList = new CharList();

        _compressedList.Insert(0, 100, 'x');
        _compressedList.SetRange(25, 50, 'o');

        _shortList.AddRange("The Lord of the Rings".ToCharArray());

        _longList.AddRange(LongText.ToCharArray());

    }

    private CharList _compressedList;
    private CharList _emptyList;
    private CharList _shortList;
    private CharList _longList;
    private const string LongText = "The quick brown fox jumps over the lazy dog";

    [Test]
    public void AddCharList()
    {
        Assert.AreEqual(0, _emptyList.Count);
        _emptyList.Add('a');
        Assert.AreEqual(1, _emptyList.Count);
        Assert.AreEqual('a', _emptyList[0]);
        _emptyList.Add('b');
        Assert.AreEqual(2, _emptyList.Count);
        Assert.AreEqual('a', _emptyList[0]);
        Assert.AreEqual('b', _emptyList[1]);
        Assert.AreEqual("ab", _emptyList.ToString());
        _emptyList.AddRange("cdefghijklm".ToCharArray());
        Assert.AreEqual(13, _emptyList.Count);
        Assert.AreEqual('c', _emptyList[2]);
        Assert.AreEqual('m', _emptyList[12]);
        Assert.AreEqual("abcdefghijklm", _emptyList.ToString());
    }

    [Test]
    public void BinarySearch()
    {
    }

    [Test]
    public void ClearList()
    {
        Assert.IsTrue(_longList.Count > 0);
        _longList.Clear();
        Assert.AreEqual(0, _longList.Count);
    }

    [Test]
    public void CloneList()
    {
    }

    [Test]
    public void CompareTo()
    {
    }

    [Test]
    public void Compress()
    {
    }

    [Test]
    public void Construction()
    {
        Assert.AreEqual(LongText, _longList.ToString());
        Assert.AreEqual(0, _emptyList.Count);
        var newlist = new CharList("abcdefghijklm".ToCharArray());
        Assert.AreEqual(13, newlist.Count);
        Assert.AreEqual("abcdefghijklm", newlist.ToString());
    }

    [Test]
    public void ContainsList()
    {
        foreach (var ch in "abcdefghijklmnopqrstuvwxyz")
            Assert.IsTrue(_longList.Contains(ch));

        Assert.IsFalse(_longList.Contains('@'));
    }

    [Test]
    public void Copy()
    {
        var list = _longList.Copy(1, 10);
        Assert.AreEqual(LongText.Substring(1, 10), list.ToString());
        Assert.AreEqual(LongText.Length, _longList.Count);
    }

    [Test]
    public void CopyTo()
    {
        var charArray = new char[2 + LongText.Length];
        _longList.CopyTo(charArray, 1);
        Assert.AreEqual('\0', charArray[0]);
        Assert.AreEqual('\0', charArray[LongText.Length + 1]);
        Assert.AreEqual(LongText, new string(charArray, 1, LongText.Length));
    }

    [Test]
    public void Cut()
    {
        var list = _longList.Cut(1, 10);
        Assert.AreEqual(LongText.Substring(1, 10), list.ToString());
        Assert.AreEqual(LongText.Substring(0, 1) + LongText.Substring(11), _longList.ToString());
        Assert.AreEqual(LongText.Length - 10, _longList.Count);
    }

    [Test]
    public void Dump()
    {
    }

    [Test]
    public void Ensure()
    {
    }

    /// <summary>  Test </summary>
    [Test]
    public void EqualsList()
    {
    }

    /// <summary>  Test </summary>
    [Test]
    public void GetEnumerator()
    {
        var i = 0;
        foreach (var ch in _longList)
            Assert.AreEqual(ch, _longList[i++]);
        Assert.AreEqual(i, _longList.Count);
    }

    /// <summary>  Test </summary>
    [Test]
    public void IndexOfList()
    {
        foreach (var ch in "abcdefghijklmnopqrstuvwxyz")
        {
            var i = _longList.IndexOf(ch);
            Assert.IsTrue(i >= 0);
            Assert.AreEqual(ch, _longList[i]);
        }

        Assert.AreEqual(-1, _longList.IndexOf('@'));
    }

    /// <summary>  Test </summary>
    [Test]
    public void InsertList()
    {
        _longList.Insert(0, 5, 'x');
        Assert.AreEqual("xxxxx" + LongText, _longList.ToString());
        _longList.InsertRange(1, 'a', 'b', 'c');
        Assert.AreEqual("xabcxxxx" + LongText, _longList.ToString());
        _longList.InsertRange(5, "xy".ToCharArray());
        Assert.AreEqual("xabcxxyxxx" + LongText, _longList.ToString());
    }

    /// <summary>  Test </summary>
    [Test]
    public void NextRun()
    {
    }

    /// <summary>  Test </summary>
    [Test]
    public void PreviousRun()
    {
    }

    /// <summary>  Test </summary>
    [Test]
    public void RemoveAtList()
    {
        _longList.RemoveAt(13);
        var text = LongText;
        text = text.Substring(0, 13) + text.Substring(14);
        Assert.AreEqual(text, _longList.ToString());
    }

    /// <summary>  Test </summary>
    [Test]
    public void RemoveList()
    {
    }

    /// <summary>  Test </summary>
    [Test]
    public void RemoveRange()
    {
        _longList.RemoveRange(13, 5);
        var text = LongText;
        text = text.Substring(0, 13) + text.Substring(13 + 5);
        Assert.AreEqual(text, _longList.ToString());
    }

    /// <summary>  Test </summary>
    [Test]
    public void SetRange()
    {
        _longList.SetRange(13, 5, 'x');
        var text = LongText;
        text = text.Substring(0, 13) + "xxxxx" + text.Substring(13 + 5);
        Assert.AreEqual(text, _longList.ToString());
        Assert.AreEqual('x', _longList[13]);
        Assert.AreEqual('x', _longList[14]);
        Assert.AreEqual('x', _longList[15]);
        Assert.AreEqual('x', _longList[16]);
        Assert.AreEqual('x', _longList[17]);
    }

    /// <summary>  Test </summary>
    [Test]
    public void Sort()
    {
    }

    /// <summary>  Test </summary>
    [Test]
    public void ToArray()
    {
        var charArray = _emptyList.ToArray();
        Assert.AreEqual(0, charArray.Length);

        charArray = _longList.ToArray();
        Assert.AreEqual(LongText.Length, charArray.Length);
        Assert.AreEqual(LongText, new string(charArray));
    }
}