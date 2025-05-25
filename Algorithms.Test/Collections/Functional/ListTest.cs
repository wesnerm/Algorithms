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

    CharList _compressedList;
    CharList _emptyList;
    CharList _shortList;
    CharList _longList;
    const string LongText = "The quick brown fox jumps over the lazy dog";

    [Test]
    public void AddCharList()
    {
        AreEqual(0, _emptyList.Count);
        _emptyList.Add('a');
        AreEqual(1, _emptyList.Count);
        AreEqual('a', _emptyList[0]);
        _emptyList.Add('b');
        AreEqual(2, _emptyList.Count);
        AreEqual('a', _emptyList[0]);
        AreEqual('b', _emptyList[1]);
        AreEqual("ab", _emptyList.ToString());
        _emptyList.AddRange("cdefghijklm".ToCharArray());
        AreEqual(13, _emptyList.Count);
        AreEqual('c', _emptyList[2]);
        AreEqual('m', _emptyList[12]);
        AreEqual("abcdefghijklm", _emptyList.ToString());
    }

    [Test]
    public void BinarySearch() { }

    [Test]
    public void ClearList()
    {
        IsTrue(_longList.Count > 0);
        _longList.Clear();
        AreEqual(0, _longList.Count);
    }

    [Test]
    public void CloneList() { }

    [Test]
    public void CompareTo() { }

    [Test]
    public void Compress() { }

    [Test]
    public void Construction()
    {
        AreEqual(LongText, _longList.ToString());
        AreEqual(0, _emptyList.Count);
        var newlist = new CharList("abcdefghijklm".ToCharArray());
        AreEqual(13, newlist.Count);
        AreEqual("abcdefghijklm", newlist.ToString());
    }

    [Test]
    public void ContainsList()
    {
        foreach (char ch in "abcdefghijklmnopqrstuvwxyz")
            IsTrue(_longList.Contains(ch));

        IsFalse(_longList.Contains('@'));
    }

    [Test]
    public void Copy()
    {
        Rope<char> list = _longList.Copy(1, 10);
        AreEqual(LongText.Substring(1, 10), list.ToString());
        AreEqual(LongText.Length, _longList.Count);
    }

    [Test]
    public void CopyTo()
    {
        char[] charArray = new char[2 + LongText.Length];
        _longList.CopyTo(charArray, 1);
        AreEqual('\0', charArray[0]);
        AreEqual('\0', charArray[LongText.Length + 1]);
        AreEqual(LongText, new string(charArray, 1, LongText.Length));
    }

    [Test]
    public void Cut()
    {
        Rope<char> list = _longList.Cut(1, 10);
        AreEqual(LongText.Substring(1, 10), list.ToString());
        AreEqual(LongText.Substring(0, 1) + LongText.Substring(11), _longList.ToString());
        AreEqual(LongText.Length - 10, _longList.Count);
    }

    [Test]
    public void Dump() { }

    [Test]
    public void Ensure() { }

    /// <summary>  Test </summary>
    [Test]
    public void EqualsList() { }

    /// <summary>  Test </summary>
    [Test]
    public void GetEnumerator()
    {
        int i = 0;
        foreach (char ch in _longList)
            AreEqual(ch, _longList[i++]);
        AreEqual(i, _longList.Count);
    }

    /// <summary>  Test </summary>
    [Test]
    public void IndexOfList()
    {
        foreach (char ch in "abcdefghijklmnopqrstuvwxyz") {
            int i = _longList.IndexOf(ch);
            IsTrue(i >= 0);
            AreEqual(ch, _longList[i]);
        }

        AreEqual(-1, _longList.IndexOf('@'));
    }

    /// <summary>  Test </summary>
    [Test]
    public void InsertList()
    {
        _longList.Insert(0, 5, 'x');
        AreEqual("xxxxx" + LongText, _longList.ToString());
        _longList.InsertRange(1, 'a', 'b', 'c');
        AreEqual("xabcxxxx" + LongText, _longList.ToString());
        _longList.InsertRange(5, "xy".ToCharArray());
        AreEqual("xabcxxyxxx" + LongText, _longList.ToString());
    }

    /// <summary>  Test </summary>
    [Test]
    public void NextRun() { }

    /// <summary>  Test </summary>
    [Test]
    public void PreviousRun() { }

    /// <summary>  Test </summary>
    [Test]
    public void RemoveAtList()
    {
        _longList.RemoveAt(13);
        string text = LongText;
        text = text.Substring(0, 13) + text.Substring(14);
        AreEqual(text, _longList.ToString());
    }

    /// <summary>  Test </summary>
    [Test]
    public void RemoveList() { }

    /// <summary>  Test </summary>
    [Test]
    public void RemoveRange()
    {
        _longList.RemoveRange(13, 5);
        string text = LongText;
        text = text.Substring(0, 13) + text.Substring(13 + 5);
        AreEqual(text, _longList.ToString());
    }

    /// <summary>  Test </summary>
    [Test]
    public void SetRange()
    {
        _longList.SetRange(13, 5, 'x');
        string text = LongText;
        text = text.Substring(0, 13) + "xxxxx" + text.Substring(13 + 5);
        AreEqual(text, _longList.ToString());
        AreEqual('x', _longList[13]);
        AreEqual('x', _longList[14]);
        AreEqual('x', _longList[15]);
        AreEqual('x', _longList[16]);
        AreEqual('x', _longList[17]);
    }

    /// <summary>  Test </summary>
    [Test]
    public void Sort() { }

    /// <summary>  Test </summary>
    [Test]
    public void ToArray()
    {
        char[] charArray = _emptyList.ToArray();
        AreEqual(0, charArray.Length);

        charArray = _longList.ToArray();
        AreEqual(LongText.Length, charArray.Length);
        AreEqual(LongText, new string(charArray));
    }
}