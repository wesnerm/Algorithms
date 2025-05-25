namespace Algorithms.Collections;

/// <summary>
///     Summary description for SmallBitSet.
/// </summary>
[DebuggerStepThrough]
public unsafe struct BitSet256
{
    #region Variables

    long long1, long2, long3, long4;

    #endregion

    #region Constructors

    BitSet256(long long1, long long2, long long3, long long4)
    {
        this.long1 = long1;
        this.long2 = long2;
        this.long3 = long3;
        this.long4 = long4;
    }

    #endregion

    #region Properties

    public static readonly BitSet256 FullSet = new(-1, -1, -1, -1);

    public bool IsEmpty => long1 == 0 || long2 == 0 || long3 == 0 || long4 == 0;

    public bool IsFull => long1 == -1 && long2 == -1 && long3 == -1 && long4 == -1;

    public bool this[int index] {
        get
        {
            int offset = (index >> 6) & 3;
            long bit = 1L << (index & 0x3f);
            fixed (long* p = &long1) {
                return (p[offset] & bit) != 0;
            }
        }
        set
        {
            int offset = (index >> 6) & 3;
            long bit = 1L << (index & 0x3f);

            fixed (long* p = &long1) {
                if (value)
                    p[offset] |= bit;
                else
                    p[offset] &= ~bit;
            }
        }
    }

    public void SetItem(int index, bool value)
    {
        int offset = (index >> 6) & 3;
        long bit = 1L << (index & 0x3f);

        fixed (long* p = &long1) {
            if (value)
                p[offset] |= bit;
            else
                p[offset] &= ~bit;
        }
    }

    #endregion

    #region Methods

    public static bool operator ==(BitSet256 bits1, BitSet256 bits2) =>
        bits1.long1 == bits2.long1 && bits1.long2 == bits2.long2
                                   && bits1.long3 == bits2.long3 && bits1.long4 == bits2.long4;

    public static bool operator !=(BitSet256 bits1, BitSet256 bits2) =>
        bits1.long1 != bits2.long1 || bits1.long2 != bits2.long2
                                   || bits1.long3 != bits2.long3 || bits1.long4 != bits2.long4;

    public static BitSet256 operator |(BitSet256 bits1, BitSet256 bits2)
    {
        bits1.long1 |= bits2.long1;
        bits1.long2 |= bits2.long2;
        bits1.long3 |= bits2.long3;
        bits1.long4 |= bits2.long4;
        return bits1;
    }

    public void Union(ref BitSet256 bits2)
    {
        long1 |= bits2.long1;
        long2 |= bits2.long2;
        long3 |= bits2.long3;
        long4 |= bits2.long4;
    }

    public static BitSet256 operator &(BitSet256 bits1, BitSet256 bits2)
    {
        bits1.long1 &= bits2.long1;
        bits1.long2 &= bits2.long2;
        bits1.long3 &= bits2.long3;
        bits1.long4 &= bits2.long4;
        return bits1;
    }

    public static BitSet256 operator -(BitSet256 bits1, BitSet256 bits2)
    {
        bits1.long1 &= ~bits2.long1;
        bits1.long2 &= ~bits2.long2;
        bits1.long3 &= ~bits2.long3;
        bits1.long4 &= ~bits2.long4;
        return bits1;
    }

    public static BitSet256 operator ^(BitSet256 bits1, BitSet256 bits2)
    {
        bits1.long1 ^= bits2.long1;
        bits1.long2 ^= bits2.long2;
        bits1.long3 ^= bits2.long3;
        bits1.long4 ^= bits2.long4;
        return bits1;
    }

    public static BitSet256 operator ~(BitSet256 bits)
    {
        bits.long1 = ~bits.long1;
        bits.long2 = ~bits.long2;
        bits.long3 = ~bits.long3;
        bits.long4 = ~bits.long4;
        return bits;
    }

    #endregion

    #region Object Overrides

    public bool Contains(ref BitSet256 bits) =>
        (~long1 & bits.long1) == 0
        && (~long2 & bits.long2) == 0
        && (~long3 & bits.long3) == 0
        && (~long4 & bits.long4) == 0;

    public bool Contains(BitSet256 bits) =>
        (~long1 & bits.long1) == 0
        && (~long2 & bits.long2) == 0
        && (~long3 & bits.long3) == 0
        && (~long4 & bits.long4) == 0;

    public bool Overlaps(ref BitSet256 bits) =>
        (long1 & bits.long1) != 0
        || (long2 & bits.long2) != 0
        || (long3 & bits.long3) != 0
        || (long4 & bits.long4) != 0;

    public bool Overlaps(BitSet256 bits) =>
        (long1 & bits.long1) != 0
        || (long2 & bits.long2) != 0
        || (long3 & bits.long3) != 0
        || (long4 & bits.long4) != 0;

    public override string ToString() =>
        long1.ToString("X8")
        + long2.ToString("X8")
        + long3.ToString("X8")
        + long4.ToString("X8");

    /// <summary>
    ///     Determines whether two Object instances are equal.
    /// </summary>
    public override bool Equals(object obj) => obj is BitSet256 && Equals((BitSet256)obj);

    /// <summary>
    ///     Determines whether two SmallBitSet instances are equal.
    /// </summary>
    public bool Equals(BitSet256 obj) =>
        obj.long1 == long1 && obj.long2 == long2
                           && obj.long3 == long3 && obj.long4 == long4;

    /// <summary>
    ///     Serves as a hash function for a particular type, suitable for use in
    ///     hashing algorithms and data structures like a hash table.
    /// </summary>
    public override int GetHashCode() => (long1 ^ long2 ^ long3 ^ long4).GetHashCode();

    #endregion
}