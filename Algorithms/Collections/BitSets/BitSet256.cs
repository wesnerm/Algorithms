using System.Runtime.CompilerServices;

namespace Algorithms.Collections;

/// <summary>
///     Summary description for SmallBitSet.
/// </summary>
[DebuggerStepThrough]
public struct BitSet256
{
    [InlineArray(4)]
    struct Data
    {
        long data;
    }

    #region Variables

    Data data;

    #endregion

    #region Constructors

    BitSet256(long data0, long data1, long data2, long data3)
    {
        this.data[0] = data0;
        this.data[1] = data1;
        this.data[2] = data2;
        this.data[3] = data3;
    }

    #endregion

    #region Properties

    public static readonly BitSet256 FullSet = new(-1, -1, -1, -1);

    public bool IsEmpty => data[0] == 0 || data[1] == 0 || data[2] == 0 || data[3] == 0;

    public bool IsFull => data[0] == -1 && data[1] == -1 && data[2] == -1 && data[3] == -1;

    public bool this[int index] {
        get
        {
            int offset = (index >> 6) & 3;
            long bit = 1L << (index & 0x3f);
            return (data[offset] & bit) != 0;
        }
        set
        {
            int offset = (index >> 6) & 3;
            long bit = 1L << (index & 0x3f);
            if (value)
                data[offset] |= bit;
            else
                data[offset] &= ~bit;
        }
    }

    #endregion

    #region Methods

    public static bool operator ==(BitSet256 bits1, BitSet256 bits2) =>
        bits1.data[0] == bits2.data[0] && bits1.data[1] == bits2.data[1]
                                   && bits1.data[2] == bits2.data[2] && bits1.data[3] == bits2.data[3];

    public static bool operator !=(BitSet256 bits1, BitSet256 bits2) =>
        bits1.data[0] != bits2.data[0] || bits1.data[1] != bits2.data[1]
                                   || bits1.data[2] != bits2.data[2] || bits1.data[3] != bits2.data[3];

    public static BitSet256 operator |(BitSet256 bits1, BitSet256 bits2)
    {
        bits1.data[0] |= bits2.data[0];
        bits1.data[1] |= bits2.data[1];
        bits1.data[2] |= bits2.data[2];
        bits1.data[3] |= bits2.data[3];
        return bits1;
    }

    public void Union(ref BitSet256 bits2)
    {
        data[0] |= bits2.data[0];
        data[1] |= bits2.data[1];
        data[2] |= bits2.data[2];
        data[3] |= bits2.data[3];
    }

    public static BitSet256 operator &(BitSet256 bits1, BitSet256 bits2)
    {
        bits1.data[0] &= bits2.data[0];
        bits1.data[1] &= bits2.data[1];
        bits1.data[2] &= bits2.data[2];
        bits1.data[3] &= bits2.data[3];
        return bits1;
    }

    public static BitSet256 operator -(BitSet256 bits1, BitSet256 bits2)
    {
        bits1.data[0] &= ~bits2.data[0];
        bits1.data[1] &= ~bits2.data[1];
        bits1.data[2] &= ~bits2.data[2];
        bits1.data[3] &= ~bits2.data[3];
        return bits1;
    }

    public static BitSet256 operator ^(BitSet256 bits1, BitSet256 bits2)
    {
        bits1.data[0] ^= bits2.data[0];
        bits1.data[1] ^= bits2.data[1];
        bits1.data[2] ^= bits2.data[2];
        bits1.data[3] ^= bits2.data[3];
        return bits1;
    }

    public static BitSet256 operator ~(BitSet256 bits)
    {
        bits.data[0] = ~bits.data[0];
        bits.data[1] = ~bits.data[1];
        bits.data[2] = ~bits.data[2];
        bits.data[3] = ~bits.data[3];
        return bits;
    }

    #endregion

    #region Object Overrides

    public bool Contains(ref BitSet256 bits) =>
        (~data[0] & bits.data[0]) == 0
        && (~data[1] & bits.data[1]) == 0
        && (~data[2] & bits.data[2]) == 0
        && (~data[3] & bits.data[3]) == 0;

    public bool Contains(BitSet256 bits) =>
        (~data[0] & bits.data[0]) == 0
        && (~data[1] & bits.data[1]) == 0
        && (~data[2] & bits.data[2]) == 0
        && (~data[3] & bits.data[3]) == 0;

    public bool Overlaps(ref BitSet256 bits) =>
        (data[0] & bits.data[0]) != 0
        || (data[1] & bits.data[1]) != 0
        || (data[2] & bits.data[2]) != 0
        || (data[3] & bits.data[3]) != 0;

    public bool Overlaps(BitSet256 bits) =>
        (data[0] & bits.data[0]) != 0
        || (data[1] & bits.data[1]) != 0
        || (data[2] & bits.data[2]) != 0
        || (data[3] & bits.data[3]) != 0;

    public override string ToString() =>
        data[0].ToString("X8")
        + data[1].ToString("X8")
        + data[2].ToString("X8")
        + data[3].ToString("X8");

    /// <summary>
    ///     Determines whether two Object instances are equal.
    /// </summary>
    public override bool Equals(object obj) => obj is BitSet256 && Equals((BitSet256)obj);

    /// <summary>
    ///     Determines whether two SmallBitSet instances are equal.
    /// </summary>
    public bool Equals(BitSet256 obj) =>
        obj.data[0] == data[0] && obj.data[1] == data[1]
                           && obj.data[2] == data[2] && obj.data[3] == data[3];

    /// <summary>
    ///     Serves as a hash function for a particular type, suitable for use in
    ///     hashing algorithms and data structures like a hash table.
    /// </summary>
    public override int GetHashCode() => (data[0] ^ data[1] ^ data[2] ^ data[3]).GetHashCode();

    #endregion
}