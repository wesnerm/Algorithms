namespace Algorithms.Collections.Mutable;

public struct BitSet512 : IEnumerable<int>
{
    const int NumberOfWords = 8;
    public long w0, w1, w2, w3, w4, w5, w6, w7;

    public static BitSet512 operator |(BitSet512 b1, BitSet512 b2) =>
        new()
        {
            w0 = b1.w0 | b2.w0,
            w1 = b1.w1 | b2.w1,
            w2 = b1.w2 | b2.w2,
            w3 = b1.w3 | b2.w3,
            w4 = b1.w4 | b2.w4,
            w5 = b1.w5 | b2.w5,
            w6 = b1.w6 | b2.w6,
            w7 = b1.w7 | b2.w7,
        };

    public static BitSet512 operator &(BitSet512 b1, BitSet512 b2) =>
        new()
        {
            w0 = b1.w0 & b2.w0,
            w1 = b1.w1 & b2.w1,
            w2 = b1.w2 & b2.w2,
            w3 = b1.w3 & b2.w3,
            w4 = b1.w4 & b2.w4,
            w5 = b1.w5 & b2.w5,
            w6 = b1.w6 & b2.w6,
            w7 = b1.w7 & b2.w7,
        };

    public static BitSet512 operator ^(BitSet512 b1, BitSet512 b2) =>
        new()
        {
            w0 = b1.w0 ^ b2.w0,
            w1 = b1.w1 ^ b2.w1,
            w2 = b1.w2 ^ b2.w2,
            w3 = b1.w3 ^ b2.w3,
            w4 = b1.w4 ^ b2.w4,
            w5 = b1.w5 ^ b2.w5,
            w6 = b1.w6 ^ b2.w6,
            w7 = b1.w7 ^ b2.w7,
        };

    public static BitSet512 operator -(BitSet512 b1, BitSet512 b2) =>
        new()
        {
            w0 = b1.w0 & ~b2.w0,
            w1 = b1.w1 & ~b2.w1,
            w2 = b1.w2 & ~b2.w2,
            w3 = b1.w3 & ~b2.w3,
            w4 = b1.w4 & ~b2.w4,
            w5 = b1.w5 & ~b2.w5,
            w6 = b1.w6 & ~b2.w6,
            w7 = b1.w7 & ~b2.w7,
        };

    public static BitSet512 operator ~(BitSet512 b1) =>
        new()
        {
            w0 = ~b1.w0,
            w1 = ~b1.w1,
            w2 = ~b1.w2,
            w3 = ~b1.w3,
            w4 = ~b1.w4,
            w5 = ~b1.w5,
            w6 = ~b1.w6,
            w7 = ~b1.w7,
        };

    public static bool Equal(ref BitSet512 b1, ref BitSet512 b2) =>
        b1.w0 == b2.w0
        && b1.w1 == b2.w1
        && b1.w2 == b2.w2
        && b1.w3 == b2.w3
        && b1.w4 == b2.w4
        && b1.w5 == b2.w5
        && b1.w6 == b2.w6
        && b1.w7 == b2.w7;

    public int Count =>
        +BitCount(w0)
        + BitCount(w1)
        + BitCount(w2)
        + BitCount(w3)
        + BitCount(w4)
        + BitCount(w5)
        + BitCount(w6)
        + BitCount(w7);

    public bool IsEmpty => (w0 | w1 | w2 | w3 | w4 | w5 | w6 | w7) == 0;

    public bool IsFull => (w0 & w1 & w2 & w3 & w4 & w5 & w6 & w7) == -1;

    public int LastElement {
        get
        {
            if (w7 != 0) return Log2(w7) + 7 * 64;
            if (w6 != 0) return Log2(w6) + 6 * 64;
            if (w5 != 0) return Log2(w5) + 5 * 64;
            if (w4 != 0) return Log2(w4) + 4 * 64;
            if (w3 != 0) return Log2(w3) + 3 * 64;
            if (w2 != 0) return Log2(w2) + 2 * 64;
            if (w1 != 0) return Log2(w1) + 1 * 64;
            if (w0 != 0) return Log2(w0);
            return -1;
        }
    }

    public int FirstElement {
        get
        {
            if (w0 != 0) return Log2(unchecked(w0 & -w0));
            if (w1 != 0) return Log2(unchecked(w1 & -w1)) + 1 * 64;
            if (w2 != 0) return Log2(unchecked(w2 & -w2)) + 2 * 64;
            if (w3 != 0) return Log2(unchecked(w3 & -w3)) + 3 * 64;
            if (w4 != 0) return Log2(unchecked(w4 & -w4)) + 4 * 64;
            if (w5 != 0) return Log2(unchecked(w5 & -w4)) + 5 * 64;
            if (w6 != 0) return Log2(unchecked(w6 & -w4)) + 6 * 64;
            if (w7 != 0) return Log2(unchecked(w7 & -w4)) + 7 * 64;
            return -1;
        }
    }

    public IEnumerator<int> GetEnumerator()
    {
        for (int i = 0; i < NumberOfWords; i++)
        for (long w = GetWord(i); w != 0; w &= w - 1)
            yield return Log2(unchecked(w & -w)) + i * 64;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private unsafe long GetWordUnsafe(int word)
    //{
    //    fixed (long* p = &w0) return p[word];
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private unsafe void SetWordUnsafe(int word, long value)
    //{
    //    fixed (long* p = &w0) p[word] = value;
    //}

    long GetWord(int word)
    {
        switch (word) {
            case 0: return w0;
            case 1: return w1;
            case 2: return w2;
            case 3: return w3;
            case 4: return w4;
            case 5: return w5;
            case 6: return w6;
            case 7: return w7;
            default: return 0;
        }
    }

    void SetWord(int word, long value)
    {
        switch (word) {
            case 0:
                w0 = value;
                return;
            case 1:
                w1 = value;
                return;
            case 2:
                w2 = value;
                return;
            case 3:
                w3 = value;
                return;
            case 4:
                w4 = value;
                return;
            case 5:
                w5 = value;
                return;
            case 6:
                w6 = value;
                return;
            case 7:
                w7 = value;
                return;
        }
    }

    public bool Contains(int index) => this[index];

    public bool Contains(BitSet512 bitset) => (bitset - this).IsEmpty;

    public void Add(int index)
    {
        int word = index >> 6;
        long bit = 1L << (index & 63);
        switch (word) {
            case 0:
                w0 |= bit;
                return;
            case 1:
                w1 |= bit;
                return;
            case 2:
                w2 |= bit;
                return;
            case 3:
                w3 |= bit;
                return;
            case 4:
                w4 |= bit;
                return;
            case 5:
                w5 |= bit;
                return;
            case 6:
                w6 |= bit;
                return;
            case 7:
                w7 |= bit;
                return;
            default: throw new ArgumentOutOfRangeException("index");
        }
    }

    public void Remove(int index)
    {
        long bit = 1L << (index & 63);
        switch (index >> 6) {
            case 0:
                w0 &= ~bit;
                return;
            case 1:
                w1 &= ~bit;
                return;
            case 2:
                w2 &= ~bit;
                return;
            case 3:
                w3 &= ~bit;
                return;
            case 4:
                w4 &= ~bit;
                return;
            case 5:
                w5 &= ~bit;
                return;
            case 6:
                w6 &= ~bit;
                return;
            case 7:
                w7 &= ~bit;
                return;
        }
    }

    public bool this[int index] {
        get => (GetWord(index >> 6) & (1L << (index & 63))) != 0;
        set
        {
            if (value) Add(index);
            else Remove(index);
        }
    }

    public override string ToString() => string.Join(", ", this);
}