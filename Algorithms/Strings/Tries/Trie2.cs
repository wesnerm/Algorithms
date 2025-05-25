using Algorithms.Collections;

namespace Algorithms.Strings;

public class Trie2 : IEquatable<Trie2>
{
    int _hashCode;
    public char Character;
    public Trie2 Children;
    public int Count;

    public Trie2 Next;

    public int SeqNo;
    public bool Shared;
    public bool Sibling;
    public bool StringEnd;

    public bool Equals(Trie2 trie)
    {
        if (trie == null
            || GetHashCode() != trie.GetHashCode())
            return false;

        if (Character != trie.Character
            || StringEnd != trie.StringEnd)
            return false;

        if (Children != trie.Children)
            if (Children == null || !Children.Equals(trie.Children))
                return false;

        if (Next != trie.Next)
            if (Next == null || !Next.Equals(trie.Next))
                return false;

        return true;
    }

    public override bool Equals(object obj) => Equals(obj as Trie2);

    public override int GetHashCode()
    {
        if (_hashCode == 0) {
            int hashCode = Character;
            if (StringEnd)
                hashCode ^= 0x12121212;
            if (Children != null)
                hashCode = HashCode.Combine(hashCode, Children.GetHashCode());
            if (Next != null)
                hashCode = HashCode.Combine(hashCode, Next.GetHashCode());
            _hashCode = hashCode != 0 ? hashCode : 0x23232323;
        }

        return _hashCode;
    }

    public static List<Trie2> Transfer(Trie2 trie,
        Dictionary<Trie2, int> map)
    {
        var array = new List<Trie2>(map.Count);
        Transfer(trie, array, map);
        return array;
    }

    static void Transfer(Trie2 trie,
        List<Trie2> array,
        Dictionary<Trie2, int> map)
    {
        for (Trie2? current = trie; current != null; current = current.Next) {
            int sn = map[current];
            array.Ensure(sn);
            Debug.Assert(array[sn] == null || array[sn] == current,
                "Error: array slot " + sn + " already preoccupied");
            array[sn] = current;
            Transfer(current.Children, array, map);
        }
    }

    public static int CountAll(Trie2 trie)
    {
        int count = 0;

        for (Trie2? current = trie; current != null; current = current.Next) {
            int currentCount = CountAll(current.Children);
            if (current.StringEnd) currentCount++;
            current.Count = currentCount;
            count += currentCount;
        }

        return count;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// Dump Trie -- dumps all the words from the trie into an array
    /// for diagnostic purposes
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public List<string> Dump(string prefix = "")
    {
        var list = new List<string>();
        DumpGuts(this, list, prefix);
        return list;
    }

    static void DumpGuts(Trie2 trie, List<string> list, string prefix)
    {
        for (; trie != null; trie = trie.Next) {
            string word = prefix + trie.Character;
            if (trie.StringEnd) list.Add(word);
            DumpGuts(trie.Children, list, word);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Insert - inserts a new word into the trie
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public static void Insert(ref Trie2 trie, string word, int i)
    {
        char ch = word[i];
        Trie2 previous = null;
        Trie2? current = trie;
        Trie2 newTrie;

        for (; current != null; previous = current, current = current.Next)
            if (current.Character >= ch)
                break;

        if (current != null && current.Character == ch) {
            newTrie = current;
        } else {
            newTrie = new Trie2 { Character = ch, Next = current };
            if (previous != null)
                previous.Next = newTrie;
            else
                trie = newTrie;
        }

        if (i + 1 < word.Length)
            Insert(ref newTrie.Children, word, i + 1);
        else
            newTrie.StringEnd = true;
    }

    public class Builder
    {
        #region Variables

        readonly Dictionary<Trie2, Trie2> _hash = new();

        #endregion

        #region Helper Methods

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // GetSequenceLength - computes the length of all the trie,
        //    first by initializing, then by adding an id to each new
        //   trie encountered, that was never sequenced
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Dictionary<Trie2, int> Sequence(Trie2 trie)
        {
            var map = new Dictionary<Trie2, int>(ReferenceEqualityComparer.Instance);
            SequenceGuts(trie, map);
            return map;
        }

        public void SequenceGuts(Trie2 trie, Dictionary<Trie2, int> map)
        {
            for (Trie2? current = trie; current != null; current = current.Next)
                if (!map.ContainsKey(current))
                    map[current] = map.Count;

            for (Trie2? current = trie; current != null; current = current.Next) SequenceGuts(current.Children, map);
        }

        public Dictionary<Trie2, int> FixSequence(Trie2 trie,
            Action<int> report)
        {
            Dictionary<Trie2, int> seq = Sequence(trie);
            report(seq.Count);

            while (true) {
                Compress(trie);
                Dictionary<Trie2, int> newSequence = Sequence(trie);
                if (seq.Count == newSequence.Count) break;
                seq = newSequence;
                report(seq.Count);
            }

            return seq;
        }

        public void Compress(Trie2 trie)
        {
            if (trie == null) return;

            for (Trie2? current = trie; current != null; current = current.Next) {
                Trie2? child = current.Children;
                if (child != null) {
                    Compress(child);
                    current.Children = Add(child);
                }
            }
        }

        Trie2 Add(Trie2 trie)
        {
            if (trie == null)
                return null;

            if (_hash.ContainsKey(trie)) {
                Trie2 oldtrie = trie;
                trie = _hash[trie];
                if (trie != oldtrie)
                    trie.Shared = true;
            } else {
                _hash[trie] = trie;
            }

            return trie;
        }

        //private void CompressText(Trie2 trie)
        //{
        //    for (; trie != null; trie = trie.Next)
        //    {
        //        var child = trie.Children;
        //        if (child == null) continue;
        //        CompressText(child);

        //        if (child.Shared) continue;
        //        if (trie.StringEnd) continue;
        //        if (child.Next != null) continue;
        //        if (child.Count != trie.Count) continue;

        //        trie.Character += child.Character;
        //        trie.Children = child.Children;
        //        trie.StringEnd = child.StringEnd;
        //    }
        //}

        #endregion
    }
}