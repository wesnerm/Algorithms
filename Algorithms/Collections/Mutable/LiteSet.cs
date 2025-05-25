#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using Algorithms.Mathematics;

#endregion

namespace Algorithms.Collections;

/// <summary>
///     Summary description for Set.
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
[DebuggerStepThrough]
public class LiteSet<T> : ICloneable, ICollection<T>
//where T : class
{
    #region Collection Properties

    bool ICollection<T>.IsReadOnly => false;

    #endregion

    #region Constants

    const int HashMask = 0x7fffffff;
    const int NullKey = 0x00000000;
    const int DeletedKey = unchecked((int)0x80000000);
    const int CollidedBit = unchecked((int)0x80000000);

    #endregion

    #region Variables

    protected struct Bucket
    {
        public int Code; // Store hash code; sign bit means there was a collision.
        public T Key;
    }

    Bucket[] _buckets;
    const double LoadFactor = 0.72;
    int _version;

    #endregion

    #region Constructors

    [DebuggerStepThrough]
    public LiteSet() : this(0) { }

    [DebuggerStepThrough]
    public LiteSet(int capacity)
    {
        if (capacity <= 0) {
            Capacity = 0;
            _buckets = Array.Empty<Bucket>();
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");
            return;
        }

        double rawsize = capacity / LoadFactor;
        if (rawsize > int.MaxValue)
            throw new ArgumentException();

        int hashsize = NumberTheory.GetPrime((int)rawsize);
        _buckets = new Bucket[hashsize];

        Capacity = (int)(LoadFactor * hashsize);
        if (Capacity >= hashsize)
            Capacity = hashsize - 1;
    }

    public LiteSet(IEnumerable<T> c)
        : this(0)
    {
        foreach (T o in c)
            Add(o);
    }

    public LiteSet(ICollection<T> c)
        : this(c.Count)
    {
        foreach (T o in c)
            Add(o);
    }

    object ICloneable.Clone() => Clone();

    public LiteSet<T> Clone()
    {
        var ht = new LiteSet<T>(Count);
        foreach (T o in this)
            ht.Add(o);
        return ht;
    }

    #endregion

    #region Operations

    [DebuggerStepThrough]
    static uint InitHash(int hash, int hashsize, out uint seed, out uint incr)
    {
        uint hashcode = (uint)(hash & HashMask);
        if (hashcode == 0)
            hashcode = 0x55555555;
        seed = hashcode;
        incr = unchecked(1 + ((seed >> 5) + 1) % ((uint)hashsize - 1));
        return hashcode;
    }

    public void Clear()
    {
        if (Count == 0)
            return;
        Array.Clear(_buckets, 0, _buckets.Length);
        Count = 0;
    }

    public bool Contains(T key)
    {
        uint seed;
        uint incr;

        // Take a snapshot of buckets, in case another thread resizes table
        Bucket[] lbuckets = _buckets;
        if (lbuckets.Length == 0)
            return false;

        uint hashcode = InitHash(key.GetHashCode(), lbuckets.Length, out seed, out incr);
        int ntry = 0;

        Bucket b;
        do {
            int bucketNumber = (int)(seed % (uint)lbuckets.Length);
            b = lbuckets[bucketNumber];
            if (b.Code == NullKey)
                return false;
            if ((b.Code & HashMask) == hashcode && key.Equals(b.Key))
                return true;
            seed += incr;
        } while (b.Code < 0 && ++ntry < lbuckets.Length);

        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Bucket[] lbuckets = _buckets;
        for (int i = lbuckets.Length; --i >= 0;) {
            Bucket b = lbuckets[i];
            if ((b.Code & HashMask) != 0)
                array[arrayIndex++] = b.Key;
        }
    }

    void Expand()
    {
        // Allocate new Array 
        int oldhashsize = _buckets.Length;

        int rawsize = 1 + oldhashsize * 2;
        if (rawsize < 0)
            throw new ArgumentException();

        int hashsize = NumberTheory.GetPrime(rawsize);
        var newBuckets = new Bucket[hashsize];

        // rehash table into new buckets
        for (int nb = 0; nb < oldhashsize; nb++) {
            Bucket b = _buckets[nb];
            if ((b.Code & HashMask) != 0)
                PutEntry(newBuckets, b.Key, b.Code & HashMask);
        }

        _buckets = newBuckets;

        // recalculate counts
        Capacity = (int)(LoadFactor * hashsize);
        if (Capacity >= hashsize)
            Capacity = hashsize - 1;
    }

    public void AddRange(T[] range)
    {
        foreach (T o in range)
            Add(o);
    }

    public void RemoveRange(T[] range)
    {
        foreach (T o in range)
            Remove(o);
    }

    public void AddRange(IEnumerable<T> range)
    {
        foreach (T o in range)
            Add(o);
    }

    public void RemoveRange(IEnumerable<T> range)
    {
        foreach (T o in range)
            Remove(o);
    }

    void ICollection<T>.Add(T key)
    {
        Add(key);
    }

    public T Add(T key)
    {
        if (key == null)
            throw new ArgumentNullException("key");

        if (Count >= Capacity && !Contains(key))
            Expand();
        uint seed;
        uint incr;
        uint hashcode = InitHash(key.GetHashCode(), _buckets.Length, out seed, out incr);
        int ntry = 0;
        int emptySlotNumber = -1;

        do {
            int bucketNumber = (int)(seed % (uint)_buckets.Length);
            Bucket b = _buckets[bucketNumber];
            if (emptySlotNumber == -1 && b.Code == DeletedKey)
                emptySlotNumber = bucketNumber;

            if (b.Code == 0) {
                if (emptySlotNumber != -1) // Reuse slot
                    bucketNumber = emptySlotNumber;

                _buckets[bucketNumber].Key = key;
                _buckets[bucketNumber].Code |= (int)hashcode;
                Count++;
                _version++;
                return key;
            }

            if ((b.Code & HashMask) == hashcode && key.Equals(b.Key))
                return b.Key;
            if (emptySlotNumber == -1)
                _buckets[bucketNumber].Code |= unchecked((int)0x80000000);
            seed += incr;
        } while (++ntry < _buckets.Length);

        if (emptySlotNumber != -1) {
            _buckets[emptySlotNumber].Key = key;
            _buckets[emptySlotNumber].Code |= (int)hashcode;
            Count++;
            _version++;
            return key;
        }

        throw new InvalidOperationException();
    }

    static void PutEntry(Bucket[] newBuckets, T key, int hashcode)
    {
        Debug.Assert(hashcode >= 0, "hashcode >= 0");

        uint seed = (uint)hashcode;
        uint incr = 1 + ((seed >> 5) + 1) % ((uint)newBuckets.Length - 1);

        do {
            int bucketNumber = (int)(seed % (uint)newBuckets.Length);
            Bucket b = newBuckets[bucketNumber];
            if ((b.Code & HashMask) == 0) {
                newBuckets[bucketNumber].Key = key;
                newBuckets[bucketNumber].Code |= hashcode;
                return;
            }

            newBuckets[bucketNumber].Code |= unchecked((int)0x80000000);
            seed += incr;
        } while (true);
    }

    public bool Remove(T key)
    {
        if (key == null)
            throw new ArgumentNullException("key");

        uint seed;
        uint incr;
        uint hashcode = InitHash(key.GetHashCode(), _buckets.Length, out seed, out incr);
        int ntry = 0;

        Bucket b;
        do {
            int bn = (int)(seed % (uint)_buckets.Length); // bucketNumber
            b = _buckets[bn];
            if ((b.Code & HashMask) == hashcode && key.Equals(b.Key)) {
                _buckets[bn] = new Bucket();
                _buckets[bn].Code = b.Code & CollidedBit;
                Count--;
                return true;
            }

            seed += incr;
        } while (b.Code < 0 && ++ntry < _buckets.Length);

        return false;
    }

    #endregion

    #region Properties

    public int Count { get; private set; }

    public int Capacity { get; private set; }

    public T this[T key] {
        get
        {
            if (key == null)
                throw new ArgumentNullException("key");
            uint seed;
            uint incr;
            // Take a snapshot of buckets, in case another thread does a resize
            Bucket[] lbuckets = _buckets;
            uint hashcode = InitHash(key.GetHashCode(), lbuckets.Length, out seed, out incr);
            int ntry = 0;

            Bucket b;
            do {
                if (ntry >= lbuckets.Length)
                    break;

                int bucketNumber = (int)(seed % (uint)lbuckets.Length);
                b = lbuckets[bucketNumber];
                if (b.Code == 0)
                    return default;
                if ((b.Code & HashMask) == hashcode && key.Equals(b.Key))
                    return b.Key;
                seed += incr;
                ++ntry;
            } while (b.Code < 0);

            return default;
        }
    }

    [DebuggerStepThrough]
    public T GetItemFromHashCode(int id)
    {
        uint seed;
        uint incr;
        // Take a snapshot of buckets, in case another thread does a resize
        Bucket[] lbuckets = _buckets;
        uint hashcode = InitHash(id, lbuckets.Length, out seed, out incr);
        int ntry = 0;

        Bucket b;
        do {
            int bucketNumber = (int)(seed % (uint)lbuckets.Length);
            b = lbuckets[bucketNumber];
            if (b.Code == 0)
                break;
            if ((b.Code & HashMask) == hashcode)
                return b.Key;
            seed += incr;
        } while (b.Code < 0 && ++ntry < lbuckets.Length);

        return default;
    }

    #endregion

    #region Enumeration

#line hidden
    [DebuggerStepThrough]
    public IEnumerator<T> GetEnumerator()
    {
#if DEBUG
        int v = _version;
#endif
        for (int i = _buckets.Length - 1; i >= 0; i--) {
#if DEBUG
            if (v != _version)
                throw new InvalidOperationException();
#endif
            Bucket b = _buckets[i];
            if ((b.Code & HashMask) != 0)
                yield return b.Key;
        }
    }
#line default
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Object Overrides

    public bool Equals(LiteSet<T> s) => s.Count == Count && ContainsSubset(s);

    public override bool Equals(object obj)
    {
        var s = obj as LiteSet<T>;
        if (s == null)
            return false;
        return Equals(s);
    }

    public override int GetHashCode()
    {
        int hashCode = Count;
        foreach (T o in this)
            hashCode ^= o.GetHashCode();
        return hashCode;
    }

    #endregion

    #region Set Operations

    public void MergeSet(IEnumerable<T> s)
    {
        foreach (T o in s)
            Add(o);
    }

    static void Reorder(ref LiteSet<T> s1, ref LiteSet<T> s2)
    {
        if (s1.Count > s2.Count) {
            LiteSet<T> tmp = s1;
            s1 = s2;
            s2 = tmp;
        }
    }

    public bool ContainsSubset(LiteSet<T> s)
    {
        if (s.Count > Count)
            return false;

        foreach (T o in s)
            if (!Contains(o))
                return false;
        return true;
    }

    public bool IntersectsWith(LiteSet<T> s2)
    {
        LiteSet<T> s1 = this;
        Reorder(ref s1, ref s2);

        foreach (T o in s1)
            if (s2.Contains(o))
                return true;
        return false;
    }

    public static LiteSet<T> Union(LiteSet<T> s1, IEnumerable<T> s2)
    {
        LiteSet<T> s = s1.Clone();
        s.MergeSet(s2);
        return s;
    }

    public static LiteSet<T> Intersection(LiteSet<T> s1, LiteSet<T> s2)
    {
        var s = new LiteSet<T>();
        foreach (T o in s1)
            if (s2.Contains(o))
                s.Add(o);
        return s;
    }

    public static LiteSet<T> Intersection(LiteSet<T> s1, ICollection<T> s2)
    {
        var s = new LiteSet<T>();
        foreach (T o in s2)
            if (s1.Contains(o))
                s.Add(o);
        return s;
    }

    public static LiteSet<T> Difference(LiteSet<T> s1, LiteSet<T> s2)
    {
        var s = new LiteSet<T>(Math.Max(0, s1.Count - s2.Count));
        foreach (T o in s1)
            if (!s2.Contains(o))
                s.Add(o);
        return s;
    }

    public static LiteSet<T> ExclusiveUnion(LiteSet<T> s1, LiteSet<T> s2)
    {
        var s = new LiteSet<T>();
        foreach (T o in s1)
            if (!s2.Contains(o))
                s.Add(o);
        foreach (T o in s2)
            if (!s1.Contains(o))
                s.Add(o);
        return s;
    }

    public static LiteSet<T> operator &(LiteSet<T> s1, LiteSet<T> s2) => Intersection(s1, s2);

    public static LiteSet<T> operator |(LiteSet<T> s1, LiteSet<T> s2) => Union(s1, s2);

    public static LiteSet<T> operator ^(LiteSet<T> s1, LiteSet<T> s2) => ExclusiveUnion(s1, s2);

    public static LiteSet<T> operator -(LiteSet<T> s1, LiteSet<T> s2) => Difference(s1, s2);

    #endregion
}