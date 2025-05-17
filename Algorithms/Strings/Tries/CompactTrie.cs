#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.IO;
using System.Text;

#endregion

namespace Algorithms.Strings;

/// <summary>
///     Summary description for CompactTrie.
/// </summary>
public unsafe struct CompactTrie
{
    #region Variables

    private ushort _loChild;
    private byte _hiChild;
    private byte _character;
    private short _count;

    #endregion

    #region Properties

    public bool IsEmpty
    {
        get { return _count == 0 && _loChild == 0 && _hiChild == 0 && _character == 0; }
    }

    public char Character
    {
        get { return (char) (_character & ~0x80); }
        set { _character = (byte) value; }
    }

    [DebuggerStepThrough]
    public char Item(int i)
    {
        fixed (byte* p = &_character)
            return (char) (p[i] & ~0x80);
    }

    [DebuggerStepThrough]
    public bool StringEnd(int index)
    {
        fixed (byte* p = &_character)
            return 0 != (p[index] & 0x80);
    }

    public void AppendTo(StringBuilder sb)
    {
        var len = Length;
        sb.EnsureCapacity(sb.Length + len);

        fixed (byte* p = &_character)
        {
            var pWrite = p;
            while (len-- > 0)
                sb.Append((char) *pWrite++);
        }
    }

    public string Text
    {
        get
        {
            var len = Length;
            var sb = new StringBuilder(len);
            fixed (byte* p = &_character)
            {
                for (var i = 0; i < len; i++)
                    sb.Append((char) (p[i] & ~0x80));
            }
            return sb.ToString();
        }
        set
        {
            var len = value.Length;
            Length = len;
            if (len == 1)
            {
                _character = (byte) value[0];
                Debug.Assert(!Compact);
                return;
            }

            fixed (CompactTrie* trie = &this)
            {
                var p = &trie->_character;
                Debug.Assert(&trie->_character < (byte*) &trie->_count);
                for (var i = 0; i < len; i++)
                {
                    p[i] = (byte) value[i];
                    Debug.Assert(p[i] != 0);
                }
                Debug.Assert(Compact);
                if (len >= 32)
                    p[len] = 0;
            }
        }
    }

    public int Size
    {
        get
        {
            if (Compact)
                return EstimateSize(Length);
            Debug.Assert(Length == 1);
            return 1;
        }
    }

    public static int EstimateSize(int length)
    {
        if (length == 1)
            return 1;

        if (length >= 32)
            length++;

        var bytes = sizeof (CompactTrie) + length - 3;
        var size = bytes/sizeof (CompactTrie);
        if (bytes%sizeof (CompactTrie) != 0)
            size++;
        return size;
    }

    public int Child
    {
        get { return _loChild + ((_hiChild & 0x3) << 16); }
        set
        {
            _loChild = unchecked((ushort) (value & 0xffff));
            _hiChild = unchecked((byte) ((value >> 16) & 0x3));
        }
    }

    public bool Compact
    {
        get { return (_hiChild & (0x1f << 2)) != 0; }
    }

    public int Length
    {
        get
        {
            var length = 1 + ((_hiChild >> 2) & 0x1f);
            if (length < 32)
                return length;

            fixed (byte* p = &_character)
            {
                var pEnd = p + 1;
                while (*pEnd != 0) pEnd++;
                return (int) (pEnd - p);
            }
        }
        set
        {
            var length = value;
            if (length > 32) length = 32;
            _hiChild = (byte) ((_hiChild & ~0x7c) | ((length - 1 & 0x1f) << 2));
        }
    }

    public bool End
    {
        get { return (_hiChild & 0x80) != 0; }
        set
        {
            if (value) _hiChild |= 0x80;
            else _hiChild &= unchecked((byte) ~0x80);
        }
    }

    public static byte* GetCharacters(CompactTrie* trie)
    {
        return &trie->_character;
    }

    //        public static int GetCount(Trie *trie, int index)
    //        {
    //            return GetCount(trie, trie+index, 0);
    //        }

    private static int _trace;

    public static int GetCount(CompactTrie* start, CompactTrie* trie)
    {
        _trace++;
        if (_trace > 20) Debugger.Break();
        try
        {
            return GetCount(start, trie, 0);
        }
        finally
        {
            _trace--;
        }
    }

    public static int GetCount(CompactTrie* start, CompactTrie* trie, int startIndex)
    {
        if (!trie->Compact)
            return trie->_count;

        var count = 0;
        var p = &trie->_character;
        var len = trie->Length;
        while (len-- > startIndex)
            if (0 != (*p++ & 0x80))
                count++;

        if (trie->Child != 0)
        {
            var child = start + trie->Child;
            while (true)
            {
                count += GetCount(start, child);
                if (child->End) break;
                child += child->Size;
            }
        }

        return count;
    }

    public int CountValue
    {
        set
        {
            Debug.Assert(value < 32768);
            if (!Compact)
                _count = (short) value;
        }
    }

    #endregion

    #region Load/Save

    public static void WriteToStream(Stream stream, byte* p, int bytes)
    {
        while (bytes-- > 0)
            stream.WriteByte(*p++);
    }

    public static int ReadFromStream(Stream stream, byte* pDest, int bytes)
    {
        var remainingBytes = bytes;
        while (remainingBytes > 0)
        {
            var data = stream.ReadByte();
            if (data == -1)
                break;
            *pDest++ = (byte) data;
            remainingBytes--;
        }
        return bytes - remainingBytes;
    }

    public static void WriteToDisk(string filename, CompactTrie[] trie)
    {
        File.Delete(filename);
        var file = File.OpenWrite(filename);

        Debug.Assert(sizeof (CompactTrie) == 6);
        fixed (CompactTrie* pTrie = trie)
            WriteToStream(file, (byte*) pTrie, trie.Length*sizeof (CompactTrie));

        file.Close();
    }

    public static CompactTrie[] ReadFromDisk(string filename)
    {
        var file = File.OpenRead(filename);

        var attr = File.GetAttributes(filename);

        var fileSize = (int) file.Length;
        Debug.Assert(fileSize%sizeof (CompactTrie) == 0);
        var trieSize = fileSize/sizeof (CompactTrie);
        var trie = new CompactTrie[trieSize];

        fixed (CompactTrie* pTrie = trie)
            ReadFromStream(file, (byte*) pTrie, fileSize);

        file.Close();
        return trie;
    }

    #endregion

    #region Comparison

    public static bool Equals(CompactTrie[] trie1, CompactTrie[] trie2)
    {
        if (trie1.Length != trie2.Length)
            return false;

        for (var i = 0; i < trie1.Length; i++)
        {
            if (!trie1[i].Equals(trie2[i]))
                return false;
        }

        return true;
    }

    public bool Equals(CompactTrie trie)
    {
        return _count == trie._count
               && _character == trie._character
               && _loChild == trie._loChild
               && _hiChild == trie._hiChild;
    }

    #endregion

    #region Miscellaneous

    public static int WordCount(CompactTrie* trie)
    {
        var count = 0;
        for (var i = 0;; i++)
        {
            var current = &trie[i];
            count += GetCount(trie, current, 0);
            if (current->End)
                break;
        }
        return count;
    }

    public override string ToString()
    {
        var sb = new StringBuilder(16);
        var len = Text.Length;
        for (var i = 0; i < len; i++)
        {
            var ch = Item(i);
            if (StringEnd(i))
                sb.AppendFormat("({0})", ch);
            else
                sb.Append(ch);
        }

        if (_count != 1) sb.AppendFormat(" ({0}x)", _count);
        if (End) sb.Append(" END");
        if (Child > 0) sb.AppendFormat(" -> {0}", Child);
        return sb.ToString();
    }

    #endregion

    #region Searches

    public static int FindPartial(CompactTrie* trie, string text, Action<int> action)
    {
        if (text == null)
            return 0;

        var current = trie;
        var currentIndex = 0;
        var len = text.Length;
        if (len == 0)
            return 0;

        var id = 1;
        for (var i = 0; i < len; i++)
        {
            var ch = char.ToLower(text[i]);

            while (ch != current->Character)
            {
                var count = GetCount(trie, current, 0);
                id += count;
                if (ch < current->Character || current->End)
                    return -1;
                var size = current->Size;
                currentIndex += size;
                current += current->Size;
            }

            if (current->StringEnd(0))
            {
                action(i + 1);
                if (i + 1 == len) return id;
                id++;
            }

            var nodeLen = current->Length;
            for (var j = 1; j < nodeLen; j++)
            {
                i++;
                if (i >= len) return -1;
                ch = char.ToLower(text[i]);
                if (current->Item(j) != ch) return -1;
                if (current->StringEnd(j))
                {
                    action(i + 1);
                    if (i + 1 == len) return id;
                    id++;
                }
            }

            if (current->Child == 0)
                return -1;
            currentIndex = current->Child;
            current = &trie[currentIndex];
        }
        return -1;
    }

    public static int Find(CompactTrie* trie, string text)
    {
        if (text == null)
            return 0;

        var current = trie;
        var currentIndex = 0;
        var len = text.Length;
        if (len == 0)
            return 0;

        var id = 1;
        for (var i = 0; i < len; i++)
        {
            var ch = char.ToLower(text[i]);

            while (ch != current->Character)
            {
                var count = GetCount(trie, current, 0);
                id += count;
                if (ch < current->Character || current->End)
                    return -1;
                var size = current->Size;
                currentIndex += size;
                current += current->Size;
            }

            if (current->StringEnd(0))
            {
                if (i + 1 == len) return id;
                id++;
            }

            var nodeLen = current->Length;
            for (var j = 1; j < nodeLen; j++)
            {
                i++;
                if (i >= len) return -1;
                ch = char.ToLower(text[i]);
                if (current->Item(j) != ch) return -1;
                if (current->StringEnd(j))
                {
                    if (i + 1 == len) return id;
                    id++;
                }
            }

            if (current->Child == 0)
                return -1;
            currentIndex = current->Child;
            current = &trie[currentIndex];
        }
        return -1;
    }

    public static string GetString(CompactTrie* trie, int id)
    {
        if (id < 1)
            return "";

        var s = new StringBuilder(11);
        var n = id - 1;
        var currentIndex = 0;
        var current = trie;
        while (true)
        {
            Debug.Assert(n >= 0);
            var count = GetCount(trie, current);
            if (n < count)
            {
                var len = current->Length;
                for (var j = 0; j < len; j++)
                {
                    s.Append(current->Item(j));
                    if (current->StringEnd(j))
                    {
                        if (n == 0)
                            return s.ToString();
                        n--;
                    }
                }

                Debug.Assert(n >= 0);
                currentIndex = current->Child;
                Debug.Assert(currentIndex != 0);
                current = &trie[currentIndex];
            }
            else
            {
                n -= count;
                Debug.Assert(n >= 0);
                if (current->End)
                    throw new ArgumentOutOfRangeException();
                var size = current->Size;
                current += size;
                currentIndex += size;
            }
        }
    }

    #endregion
}