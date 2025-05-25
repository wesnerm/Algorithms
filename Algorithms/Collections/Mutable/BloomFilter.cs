#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2006 - 2008, Wesner Moise.
/////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Collections;

public class BloomFilter
{
    #region Variables

    int[] array;
    int count;

    #endregion

    #region Construction

    public BloomFilter() => array = new int[256];

    public BloomFilter(IEnumerable en)
        : this()
    {
        AddRange(en);
    }

    public BloomFilter Clone()
    {
        var clone = (BloomFilter)MemberwiseClone();
        clone.array = (int[])array.Clone();
        return clone;
    }

    #endregion

    #region Methods

    void Ensure(int length)
    {
        if (length <= array.Length)
            return;
        Array.Resize(ref array, length);
    }

    public void AddItem(object o)
    {
        int hashcode = o.GetHashCode();
        SetIndex(hashcode);
        count++;
    }

    public void AddRange(IEnumerable range)
    {
        foreach (object? o in range)
            AddItem(o);
    }

    public bool ContainsItem(object o)
    {
        int hashcode = o.GetHashCode();
        return GetIndex(hashcode);
    }

    public void Clear()
    {
        array.Initialize();
        count = 0;
    }

    bool GetIndex(int index)
    {
        unchecked {
            int bit = 1 << (index & 31);
            int loc = index >> 5;
            return (array[(uint)loc % array.Length] & bit) != 0;
        }
    }

    void SetIndex(int index)
    {
        int bit = 1 << (index & 31);
        int loc = index >> 5;
        array[loc % array.Length] |= bit;
    }

    public override bool Equals(object o)
    {
        var bloom = o as BloomFilter;
        if (bloom == null) return false;

        if (bloom.array == array)
            return true;

        if (bloom.count != count)
            return false;

        if (array.Length != bloom.array.Length)
            return false;

        for (int i = 0; i < array.Length; i++)
            if (array[i] != bloom.array[i])
                return false;

        return true;
    }

    public override int GetHashCode()
    {
        int hashCode = base.GetHashCode();
        return hashCode;
    }

    #endregion
}