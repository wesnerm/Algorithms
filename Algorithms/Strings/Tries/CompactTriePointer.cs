namespace Algorithms.Strings;

public unsafe struct CompactTriePointer
{
    #region Statics

    public static CompactTrie* Address;

    #endregion

    #region Variables

    CompactTrie* _current;
    byte _item;
    byte _len;

    #endregion

    #region Construction

    CompactTriePointer(CompactTrie* current, int id)
    {
        _current = current;
        LemmaID = id;
        _item = 0;
        _len = (byte)current->Length;
    }

    #endregion

    #region Properties

    public static CompactTriePointer WordList => new(Address, 1);

    public static CompactTriePointer Null => new();

    public bool IsNull => _current == null;

    public int Index => (int)(_current - Address);

    public int Subindex => _item;

    public char Character => _current->Item(_item);

    public bool MoreSibling => _item == 0 && _current != null && !_current->End;

    public bool EndOfWord => _current->StringEnd(_item);

    public CompactTriePointer Children {
        get
        {
            CompactTriePointer ptr = this;
            if (ptr.MoveDown()) return ptr;
            return new CompactTriePointer();
        }
    }

    public int WordCount => CompactTrie.GetCount(Address, _current, _item);

    public CompactTriePointer Next {
        get
        {
            CompactTriePointer ptr = this;
            if (ptr.MoveNext()) return ptr;
            return new CompactTriePointer();
        }
    }

    public int LemmaID { get; private set; }

    #endregion

    #region Enumeration

    public bool MoveNext()
    {
        if (_item != 0 || _current == null || _current->End)
            return false;
        LemmaID += CompactTrie.GetCount(Address, _current, 0);
        _current += CompactTrie.EstimateSize(_len);
        _len = (byte)_current->Length;
        return true;
    }

    public bool MoveDown()
    {
        if (_current == null)
            return false;

        if (_current->StringEnd(_item))
            LemmaID++;

        if (_current->Compact) {
            _item++;
            if (_item < _len)
                return true;
            _item = 0;
        }

        int index = _current->Child;
        if (index == 0)
            return false;

        _current = &Address[index];
        _len = (byte)_current->Length;
        return true;
    }

    public bool MoveDown2() => _item + 1 < _len && MoveDown();

    public bool MoveTo(char ch)
    {
        while (true) {
            char currentCh = Character;
            if (ch == currentCh)
                return true;
            if (ch < currentCh || !MoveNext())
                return false;
        }
    }

    #endregion

    #region Methods

    public static int Find(string text)
    {
        if (string.IsNullOrEmpty(text))
            return 0;
        return Find(text, 0, text.Length);
    }

    public static int Find(string text, int start, int count)
    {
        if (start == text.Length)
            return 0;

        CompactTriePointer ptr = WordList;

        while (true) {
            if (!ptr.MoveTo(char.ToLower(text[start])))
                return -1;

            start++;
            count--;

            if (count == 0)
                return ptr.EndOfWord ? ptr.LemmaID : -1;

            if (!ptr.MoveDown())
                return -1;
        }
    }

    public static IEnumerable<int> GetWordBreaks(string text, bool bothSides)
    {
        CompactTriePointer ptr = WordList;
        int i = 0;
        while (true) {
            if (!ptr.MoveTo(char.ToLower(text[i])))
                yield break;

            i++;
            if (ptr.EndOfWord)
                if (!bothSides || Find(text, i, text.Length - i) > 0)
                    yield return i;

            if (i == text.Length || !ptr.MoveDown())
                yield break;
        }
    }

    public static string GetString(int id)
    {
        if (id < 1)
            return null;

        var s = new StringBuilder(11);
        int n = id - 1;

        CompactTriePointer current = WordList;
        while (true) {
            Debug.Assert(n >= 0);
            int count = current.WordCount;
            if (n < count) {
                do {
                    s.Append(current.Character);
                    if (current.EndOfWord && n-- == 0)
                        return s.ToString();
                } while (current.MoveDown2());

                Debug.Assert(n >= 0);
                bool good = current.MoveDown();
                Debug.Assert(good);
            } else {
                n -= count;
                Debug.Assert(n >= 0);
                if (!current.MoveNext())
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    #endregion

    #region Object Overrides

    public override bool Equals(object obj) => obj is CompactTriePointer && Equals((CompactTriePointer)obj);

    public bool Equals(CompactTriePointer pointer) => pointer._current == _current && pointer._item == _item;

    public static bool operator ==(CompactTriePointer a, CompactTriePointer b) => a.Equals(b);

    public static bool operator !=(CompactTriePointer a, CompactTriePointer b) => !a.Equals(b);

    public override int GetHashCode()
    {
        if (_current == null)
            return 0;
        return _current->GetHashCode() ^ _item;
    }

    public override string ToString() => _current == null ? "" : _current->ToString();

    #endregion
}