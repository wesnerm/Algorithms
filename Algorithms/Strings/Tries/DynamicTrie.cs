using Algorithms.Collections;

namespace Algorithms.Strings;

public class DynamicTrie : IEquatable<DynamicTrie>
{
    static readonly TrieData NullTrie = new();
    public int Hashcode;
    public bool IsWord;
    public DynamicTrie? Left, Middle, Right;
    public char Letter;

    public bool Equals(DynamicTrie? t)
    {
        if (t == this)
            return true;
        if (t == null)
            return false;

        return t.Letter == Letter
               && t.GetHashCode() == GetHashCode()
               && t.IsWord == IsWord
               && Equals(t.Left, Left)
               && Equals(t.Middle, Middle)
               && Equals(t.Right, Right);
    }

    public DynamicTrie Clone() => (DynamicTrie)MemberwiseClone();

    public static void Insert(ref DynamicTrie trie, string s)
    {
        if (s.Length != 0) Insert(ref trie, s, 0);
    }

    static bool Insert(ref DynamicTrie trie, string s, int pos)
    {
        if (pos >= s.Length)
            return false;

        char ch = char.ToUpper(s[pos]);
        bool changed = false;

        if (trie == null) {
            trie = new DynamicTrie { Letter = ch };
            changed = true;
        }

        if (trie.Letter == ch) {
            if (pos + 1 < s.Length) {
                changed |= Insert(ref trie.Middle, s, pos + 1);
            } else if (trie.IsWord) {
                return false;
            } else {
                changed = true;
                trie.IsWord = true;
            }
        } else if (trie.Letter < ch) {
            changed = Insert(ref trie.Left, s, pos);
        } else {
            changed = Insert(ref trie.Right, s, pos);
        }

        if (changed)
            trie.Hashcode = 0;
        return changed;
    }

    public override int GetHashCode() => GetHashCode(this);

    public override bool Equals(object? obj)
    {
        var t = obj as DynamicTrie;
        return Equals(t);
    }

    public static int GetHashCode(DynamicTrie trie)
    {
        if (trie == null)
            return 0;

        if (trie.Hashcode == 0) {
            int hashcode = GetHashCode(trie.Middle);
            hashcode = HashCode.Combine(hashcode, GetHashCode(trie.Left));
            hashcode = HashCode.Combine(hashcode, GetHashCode(trie.Right));
            hashcode = HashCode.Combine(hashcode, trie.Letter);
            hashcode = HashCode.Combine(hashcode, trie.IsWord ? 0 : 1);
            trie.Hashcode = hashcode == 0 ? 0x12345678 : hashcode;
        }

        return trie.Hashcode;
    }

    public static DynamicTrie Read(string path)
    {
        StreamReader file = File.OpenText(path);
        DynamicTrie trie = null;
        while (true) {
            string? line = file.ReadLine();
            if (line == null)
                return trie;
            Insert(ref trie, line);
        }
    }

    public static TrieData Compress(ref DynamicTrie t)
    {
        var hash = new Dictionary<DynamicTrie, TrieData>();
        TrieData result = Compress(ref t, hash);
        foreach (TrieData v in hash.Values) {
            v.Left = Request(v.Trie.Left, hash);
            v.Right = Request(v.Trie.Right, hash);
            v.Middle = Request(v.Trie.Middle, hash);
        }

        return result;
    }

    public static TrieData? Request(DynamicTrie t, Dictionary<DynamicTrie, TrieData> hash)
    {
        if (t == null)
            return null;
        return hash.GetValueOrDefault(t);
    }

    public static TrieData Compress(ref DynamicTrie t, Dictionary<DynamicTrie, TrieData> hash)
    {
        if (t == null)
            return NullTrie;

        TrieData? data = hash.GetValueOrDefault(t);
        if (data != null) {
            data.Instances++;
            return data;
        }

        hash[t] = data = new TrieData { Trie = t, Instances = 1 };

        TrieData dataLeft = Compress(ref t.Left, hash);
        TrieData dataRight = Compress(ref t.Right, hash);
        TrieData dataMiddle = Compress(ref t.Middle, hash);

        data.CountChildren = dataMiddle.Count;
        data.CountSiblings = 1 + dataLeft.CountSiblings + dataRight.CountSiblings;
        return data;
    }

    public class TrieData
    {
        public int CountChildren;
        public int CountSiblings;
        public int Index;
        public int Instances;
        public TrieData Left;
        public TrieData Middle;
        public TrieData Right;
        public DynamicTrie Trie;

        public int Count => CountSiblings + CountChildren;
    }
}