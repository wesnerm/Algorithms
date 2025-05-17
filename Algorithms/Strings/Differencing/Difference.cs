using System.Collections;

namespace Algorithms.Strings;

public class Difference<T> where T : IEquatable<T>
{
    #region Constructor

    #endregion

    #region Variables

    private readonly List<DiffEdit> _edits = new List<DiffEdit>();
    private readonly List<Card> _piles = new List<Card>();
    private IList<T> _src;
    private IList<T> _dest;

    #endregion

    #region Methods

    public static List<DiffEdit> Diff(List<T> src, List<T> dest1, List<T> dest2)
    {
        var diff1 = new Difference<T>();
        var diff2 = new Difference<T>();
        var result1 = diff1.Diff(src, dest1);
        var result2 = diff2.Diff(src, dest2);
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

    private void DiffMain(int srcStart, int srcCount, int destStart, int destCount)
    {
        if (srcCount <= 0 || destCount <= 0)
        {
            Debug.Assert(srcCount >= 0 && destCount >= 0);
            return;
        }

        var prefixCount = LengthOfCommonPrefix(_src, srcStart, srcCount, _dest, destStart, destCount);
        Emit(_edits, srcStart, destStart, prefixCount);

        srcStart += prefixCount;
        srcCount -= prefixCount;
        destStart += prefixCount;
        destCount -= prefixCount;

        var suffixCount = LengthOfCommonSuffix(_src, srcStart, srcCount, _dest, destStart, destCount);

        srcCount -= suffixCount;
        destCount -= suffixCount;

        DiffCore(srcStart, srcCount, destStart, destCount);
        Emit(_edits, srcStart + srcCount, destStart + destCount, suffixCount);
    }

    private void DiffCore(int srcStart, int srcCount, int destStart, int destCount)
    {
        var uniqueSrc = new Dictionary<T, int>();
        var uniqueDest = new Dictionary<T, int>();

        var srcEnd = srcStart + srcCount;
        var destEnd = destStart + destCount;

        for (var i = srcStart; i < srcEnd; i++)
        {
            var e = _src[i];
            uniqueSrc[e] = uniqueSrc.ContainsKey(e) ? -1 : i;
        }

        for (var i = destStart; i < destEnd; i++)
        {
            var e = _dest[i];
            int index;
            if (uniqueSrc.TryGetValue(e, out index) && index != -1)
            {
                if (uniqueDest.ContainsKey(e))
                {
                    uniqueSrc.Remove(e);
                    uniqueDest.Remove(e);
                }
                else
                    uniqueDest[e] = i;
            }
        }

        if (uniqueDest.Count == 0)
            return;

        var lastIndex = int.MinValue;
        foreach (var pair in uniqueDest)
        {
            // pairs should arrived in order sorted by destination index
            var index = pair.Value;
            Debug.Assert(index > lastIndex);
            lastIndex = index;

            var e = pair.Key;
            var srcIndex = uniqueSrc[e];
            Place(srcIndex);
        }

        var lis = LargestIncreasingSubsequence();
        Debug.Assert(lis.Length > 0);

        var currentSrc = srcStart;
        var currentDest = destStart;

        foreach (var i in lis)
        {
            var srcIndex = i;
            var e = _src[i];
            var destIndex = uniqueDest[e];
            Debug.Assert(destIndex >= 0);

            DiffMain(currentSrc, srcIndex - currentSrc, currentDest, destIndex - currentDest);

            Emit(_edits, srcIndex, destIndex, 1);

            currentSrc = srcIndex + 1;
            currentDest = destIndex + 1;
        }

        DiffMain(currentSrc, srcEnd - currentSrc, currentDest, destEnd - currentDest);
    }

    private static void Emit(List<DiffEdit> edits, int srcStart, int destStart, int len)
    {
        if (len <= 0)
        {
            Debug.Assert(len == 0);
            return;
        }

        var lastIndex = edits.Count - 1;
        if (lastIndex >= 0)
        {
            var lastEdit = edits[lastIndex];
            var lastLength = lastEdit.Length;
            if (lastEdit.SourceOffset + lastLength == srcStart && lastEdit.DestOffset + lastLength == destStart)
            {
                lastEdit.Length = lastLength + len;
                return;
            }
        }

        edits.Add(new DiffEdit
        {
            SourceOffset = srcStart,
            DestOffset = srcStart,
            Length = len
        });
    }

#if true
    public static List<DiffEdit> BitsToEditList(BitArray a, BitArray b)
    {
        var edits = new List<DiffEdit>();
        var indexA = 0;
        var indexB = 0;
        var countA = a.Count;
        var countB = b.Count;

        while (indexA < a.Count && indexB < b.Count)
        {
            var oldIndexA = indexA;
            var oldIndexB = indexB;

            while (indexA < countA && !a[indexA])
                indexA++;
            while (indexB < countB && !b[indexB])
                indexB++;

            // Recording changes
            if (indexA != oldIndexA)
            {
                if (indexB != oldIndexB)
                {
                    // replacement
                }
            }
            else if (indexB != oldIndexB)
            {
                // insertion
            }

            var newIndexA = indexA;
            var newIndexB = indexB;
            while (newIndexA < countA && newIndexB < countB && a[indexA] && b[indexB])
            {
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
        foreach (var e in edits)
        {
            var len = e.Length;
            var srcStart = e.SourceOffset;
            var destStart = e.DestOffset;
            Debug.Assert(len >= 0);
            for (var i = 0; i < len; i++)
            {
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
        var index = FindPile(c);
        if (index > _piles.Count)
            _piles.Add(null);

        _piles[index] = new Card
        {
            // Previous = _piles[index],
            Value = c,
            Backreference = index > 0 ? _piles[index - 1] : null
        };
    }

    private int FindPile(int c)
    {
        var min = 0;
        var max = _piles.Count - 1;
        while (min <= max)
        {
            var mid = (min + max) >> 1;
            var pile = _piles[mid];
            var top = pile.Value;

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
        var count = _piles.Count;
        var list = new int[count];

        var i = count - 1;
        for (var card = count > 0 ? _piles[count - 1] : null; card != null; card = card.Backreference, i--)
            list[i] = card.Value;

        Debug.Assert(i == -1);
        return list;
    }

    private static int LengthOfCommonPrefix(IList<T> src, int srcStart, int startCount, IList<T> dest, int destStart,
        int destCount)
    {
        var count = 0;
        var max = Math.Min(startCount, destCount);
        while (count < max && src[srcStart + count].Equals(dest[destStart + count]))
            count++;
        return count;
    }

    private static int LengthOfCommonSuffix(IList<T> src, int srcStart, int srcCount, IList<T> dest, int destStart,
        int destCount)
    {
        var last = Math.Min(srcCount, destCount) - 1;
        var current = last;
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

        public override string ToString()
        {
            return string.Format("Source={0} Dest={1} Length={2}", SourceOffset, DestOffset, Length);
        }
    }

    private class Card
    {
        // public Card Previous;
        public Card Backreference;
        public int Value;
    }

    #endregion
}