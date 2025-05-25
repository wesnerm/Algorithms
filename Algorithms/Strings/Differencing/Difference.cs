namespace Algorithms.Strings;

public class Difference<T> where T : IEquatable<T>
{
    #region Variables

    readonly List<DiffEdit> _edits = new();
    readonly List<Card> _piles = new();
    IList<T> _src;
    IList<T> _dest;

    #endregion

    #region Methods

    public static List<DiffEdit> Diff(List<T> src, List<T> dest1, List<T> dest2)
    {
        var diff1 = new Difference<T>();
        var diff2 = new Difference<T>();
        List<DiffEdit> result1 = diff1.Diff(src, dest1);
        List<DiffEdit> result2 = diff2.Diff(src, dest2);
        return null;
    }

    public List<DiffEdit> Diff(IList<T> src, IList<T> dest)
    {
        _piles.Clear();
        _edits.Clear();
        _src = src;
        _dest = dest;
        DiffMain(0, src.Count, 0, dest.Count);
        return _edits;
    }

    void DiffMain(int srcStart, int srcCount, int destStart, int destCount)
    {
        if (srcCount <= 0 || destCount <= 0) {
            Debug.Assert(srcCount >= 0 && destCount >= 0);
            return;
        }

        int prefixCount = LengthOfCommonPrefix(_src, srcStart, srcCount, _dest, destStart, destCount);
        Emit(_edits, srcStart, destStart, prefixCount);

        srcStart += prefixCount;
        srcCount -= prefixCount;
        destStart += prefixCount;
        destCount -= prefixCount;

        int suffixCount = LengthOfCommonSuffix(_src, srcStart, srcCount, _dest, destStart, destCount);

        srcCount -= suffixCount;
        destCount -= suffixCount;

        DiffCore(srcStart, srcCount, destStart, destCount);
        Emit(_edits, srcStart + srcCount, destStart + destCount, suffixCount);
    }

    void DiffCore(int srcStart, int srcCount, int destStart, int destCount)
    {
        var uniqueSrc = new Dictionary<T, int>();
        var uniqueDest = new Dictionary<T, int>();

        int srcEnd = srcStart + srcCount;
        int destEnd = destStart + destCount;

        for (int i = srcStart; i < srcEnd; i++) {
            T e = _src[i];
            uniqueSrc[e] = uniqueSrc.ContainsKey(e) ? -1 : i;
        }

        for (int i = destStart; i < destEnd; i++) {
            T e = _dest[i];
            int index;
            if (uniqueSrc.TryGetValue(e, out index) && index != -1) {
                if (uniqueDest.ContainsKey(e)) {
                    uniqueSrc.Remove(e);
                    uniqueDest.Remove(e);
                } else {
                    uniqueDest[e] = i;
                }
            }
        }

        if (uniqueDest.Count == 0)
            return;

        int lastIndex = int.MinValue;
        foreach (KeyValuePair<T, int> pair in uniqueDest) {
            // pairs should arrived in order sorted by destination index
            int index = pair.Value;
            Debug.Assert(index > lastIndex);
            lastIndex = index;

            T e = pair.Key;
            int srcIndex = uniqueSrc[e];
            Place(srcIndex);
        }

        int[] lis = LargestIncreasingSubsequence();
        Debug.Assert(lis.Length > 0);

        int currentSrc = srcStart;
        int currentDest = destStart;

        foreach (int i in lis) {
            int srcIndex = i;
            T e = _src[i];
            int destIndex = uniqueDest[e];
            Debug.Assert(destIndex >= 0);

            DiffMain(currentSrc, srcIndex - currentSrc, currentDest, destIndex - currentDest);

            Emit(_edits, srcIndex, destIndex, 1);

            currentSrc = srcIndex + 1;
            currentDest = destIndex + 1;
        }

        DiffMain(currentSrc, srcEnd - currentSrc, currentDest, destEnd - currentDest);
    }

    static void Emit(List<DiffEdit> edits, int srcStart, int destStart, int len)
    {
        if (len <= 0) {
            Debug.Assert(len == 0);
            return;
        }

        int lastIndex = edits.Count - 1;
        if (lastIndex >= 0) {
            DiffEdit lastEdit = edits[lastIndex];
            int lastLength = lastEdit.Length;
            if (lastEdit.SourceOffset + lastLength == srcStart && lastEdit.DestOffset + lastLength == destStart) {
                lastEdit.Length = lastLength + len;
                return;
            }
        }

        edits.Add(new DiffEdit
        {
            SourceOffset = srcStart,
            DestOffset = srcStart,
            Length = len,
        });
    }

#if true
    public static List<DiffEdit> BitsToEditList(BitArray a, BitArray b)
    {
        var edits = new List<DiffEdit>();
        int indexA = 0;
        int indexB = 0;
        int countA = a.Count;
        int countB = b.Count;

        while (indexA < a.Count && indexB < b.Count) {
            int oldIndexA = indexA;
            int oldIndexB = indexB;

            while (indexA < countA && !a[indexA])
                indexA++;
            while (indexB < countB && !b[indexB])
                indexB++;

            // Recording changes
            if (indexA != oldIndexA) {
                if (indexB != oldIndexB) {
                    // replacement
                }
            } else if (indexB != oldIndexB) {
                // insertion
            }

            int newIndexA = indexA;
            int newIndexB = indexB;
            while (newIndexA < countA && newIndexB < countB && a[indexA] && b[indexB]) {
                newIndexA++;
                newIndexB++;
            }

            Emit(edits, indexA, indexB, newIndexA - newIndexB);

            indexA = newIndexA;
            indexB = newIndexB;
        }

        return edits;
    }

    public static void EditListToBitArray(IEnumerable<DiffEdit> edits, BitArray a, BitArray b)
    {
        foreach (DiffEdit e in edits) {
            int len = e.Length;
            int srcStart = e.SourceOffset;
            int destStart = e.DestOffset;
            Debug.Assert(len >= 0);
            for (int i = 0; i < len; i++) {
                // Assigning twice usally indicates an error
                Debug.Assert(!a[srcStart + i]);
                Debug.Assert(!b[destStart + i]);
                a[srcStart + i] = true;
                b[destStart + i] = true;
            }
        }
    }
#endif

    #endregion

    #region Helper Method

    public void Place(int c)
    {
        int index = FindPile(c);
        if (index > _piles.Count)
            _piles.Add(null);

        _piles[index] = new Card
        {
            // Previous = _piles[index],
            Value = c,
            Backreference = index > 0 ? _piles[index - 1] : null,
        };
    }

    int FindPile(int c)
    {
        int min = 0;
        int max = _piles.Count - 1;
        while (min <= max) {
            int mid = (min + max) >> 1;
            Card pile = _piles[mid];
            int top = pile.Value;

            if (c >= top)
                min = mid + 1;
            else if (mid > min && c < _piles[mid - 1].Value)
                max = mid - 1;
            else
                return mid;
        }

        return min;
    }

    public int[] LargestIncreasingSubsequence()
    {
        int count = _piles.Count;
        int[] list = new int[count];

        int i = count - 1;
        for (Card? card = count > 0 ? _piles[count - 1] : null; card != null; card = card.Backreference, i--)
            list[i] = card.Value;

        Debug.Assert(i == -1);
        return list;
    }

    static int LengthOfCommonPrefix(IList<T> src, int srcStart, int startCount, IList<T> dest, int destStart,
        int destCount)
    {
        int count = 0;
        int max = Math.Min(startCount, destCount);
        while (count < max && src[srcStart + count].Equals(dest[destStart + count]))
            count++;
        return count;
    }

    static int LengthOfCommonSuffix(IList<T> src, int srcStart, int srcCount, IList<T> dest, int destStart,
        int destCount)
    {
        int last = Math.Min(srcCount, destCount) - 1;
        int current = last;
        while (current >= 0 && src[srcStart + current].Equals(dest[destStart + current]))
            current--;
        return last - current;
    }

    #endregion

    #region Helpers

    public class DiffEdit
    {
        public int DestOffset;
        public int Length;
        public int SourceOffset; /* off into s1 if MATCH or DELETE but s2 if INSERT */

        public override string ToString() =>
            string.Format("Source={0} Dest={1} Length={2}", SourceOffset, DestOffset, Length);
    }

    class Card
    {
        // public Card Previous;
        public Card Backreference;
        public int Value;
    }

    #endregion
}