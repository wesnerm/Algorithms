#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2006 - 2008, Wesner Moise.
/////////////////////////////////////////////////////////////////////////////

using System.Collections;

#endregion

namespace Algorithms.Collections;

public class BloomFilter
{
    #region Variables

    private int[] array;
    private int count;

    #endregion

    #region Construction

    public BloomFilter()
    {
        array = new int[256];
    }

    public BloomFilter(IEnumerable en)
        : this()
    {
        AddRange(en);
    }

    public BloomFilter Clone()
    {
        var clone = (BloomFilter) MemberwiseClone();
        clone.array = (int[]) array.Clone();
        return clone;
    }

    #endregion

    #region Methods

    private void Ensure(int length)
    {
        if (length <= array.Length)
            return;
        Array.Resize(ref array, length);
    }

    public void AddItem(object o)
    {
        var hashcode = o.GetHashCode();
        SetIndex(hashcode);
        count++;
    }

    public void AddRange(IEnumerable range)
    {
        foreach (var o in range)
            AddItem(o);
    }

    public bool ContainsItem(object o)
    {
        var hashcode = o.GetHashCode();
        return GetIndex(hashcode);
    }

    public void Clear()
    {
        array.Initialize();
        count = 0;
    }

    private bool GetIndex(int index)
    {
        unchecked
        {
            var bit = 1 << (index & 31);
            var loc = index >> 5;
            return (array[(uint) loc%array.Length] & bit) != 0;
        }
    }

    private void SetIndex(int index)
    {
        var bit = 1 << (index & 31);
        var loc = index >> 5;
        array[loc%array.Length] |= bit;
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

        for (var i = 0; i < array.Length; i++)
            if (array[i] != bloom.array[i])
                return false;

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = base.GetHashCode();
        return hashCode;
    }

    #endregion
}