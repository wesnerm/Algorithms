using System.Numerics;

namespace Algorithms.RangeQueries;

public class BitSegmentTree
{
    readonly ulong[][] _bits;
    readonly int _capacity;

    public BitSegmentTree(int capacity)
    {
        _capacity = capacity;
        int d = 1;
        for (int m = capacity; m > 1; m >>= 6, d++) ;

        _bits = new ulong[d][];
        for (int i = 0, m = capacity >> 6; i < d; i++, m >>= 6)
            _bits[i] = new ulong[m + 1];
        Size = 0;
    }

    public int Size { get; private set; }

    public bool this[int pos] {
        get => pos >= 0 && pos < _capacity && (long)(_bits[0][pos >> 6] << (~pos & 63)) < 0;
        set
        {
            if (value) {
                if (pos >= 0 && pos < _capacity) {
                    if (!this[pos]) Size++;
                    for (int i = 0; i < _bits.Length; i++, pos >>= 6)
                        _bits[i][pos >> 6] |= 1UL << pos;
                }
            } else {
                if (pos >= 0 && pos < _capacity) {
                    if (this[pos]) Size--;
                    for (int i = 0; i < _bits.Length && (i == 0 || _bits[i - 1][pos] == 0L); i++, pos >>= 6)
                        _bits[i][pos >> 6] &= ~(1UL << pos);
                }
            }
        }
    }

    public BitSegmentTree SetRangeExclusive(int r)
    {
        for (int i = 0; i < _bits.Length; i++, r = (r + 63) >> 6) {
            for (int j = 0; j < r >> 6; j++)
                _bits[i][j] = ulong.MaxValue;
            if ((r & 63) != 0) _bits[i][r >> 6] |= (1UL << r) - 1;
        }

        return this;
    }

    // [0,r)
    public BitSegmentTree UnsetRange(int r)
    {
        if (r >= 0)
            for (int i = 0; i < _bits.Length; i++, r = (r + 63) >> 6) {
                for (int j = 0; j < (r + 63) >> 6; j++)
                    _bits[i][j] = 0;
                if ((r & 63) != 0) _bits[i][r >> 6] &= ~((1UL << r) - 1);
            }

        return this;
    }

    public int Prev(int pos)
    {
        for (int i = 0; i < _bits.Length && pos >= 0; i++, pos >>= 6, pos--) {
            int pre = Prev(_bits[i][pos >> 6], pos & 63);
            if (pre != -1) {
                pos = ((pos >> 6) << 6) | pre;
                while (i > 0) pos = (pos << 6) | (63 - BitOperations.LeadingZeroCount(_bits[--i][pos]));
                return pos;
            }
        }

        return -1;
    }

    public int Next(int pos)
    {
        for (int i = 0; i < _bits.Length && pos >> 6 < _bits[i].Length; i++, pos >>= 6, pos++) {
            int nex = Next(_bits[i][pos >> 6], pos & 63);
            if (nex != -1) {
                pos = ((pos >> 6) << 6) | nex;
                while (i > 0) pos = (pos << 6) | BitOperations.TrailingZeroCount(_bits[--i][pos]);
                return pos;
            }
        }

        return -1;
    }

    static int Prev(ulong set, int n)
    {
        ulong h = HighestOneBit(set << ~n);
        if (h == 0L) return -1;
        return BitOperations.TrailingZeroCount(h) - (63 - n);
    }

    static int Next(ulong set, int n)
    {
        ulong h = LowestOneBit(set >> n);
        if (h == 0L) return -1;
        return BitOperations.TrailingZeroCount(h) + n;
    }

    public override string ToString()
    {
        var list = new List<int>();
        for (int pos = Next(0); pos != -1; pos = Next(pos + 1))
            list.Add(pos);
        return string.Join(" ", list);
    }

    public static ulong LowestOneBit(ulong n) => n & unchecked((ulong)-(long)n);

    public static ulong HighestOneBit(ulong n) => unchecked(n != 0 ? 1UL << Log2((long)n) : 0);

    private static int Log2(long size) => size > 0 ? BitOperations.Log2((ulong)size) : -1;

}