#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Runtime.InteropServices;
using Algorithms.Mathematics;

#endregion

namespace Algorithms.Collections;

/// <summary>
///     Summary description for WeakMap.
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
public sealed class WeakMap<K, V> : ICloneable, IEnumerable<K>
    where K : class
{
    #region Variables

    const int SampleSize = 20;

    GCHandle bucketHandle;
    bucket[] buckets;
    float loadFactor;
    int version;

    struct bucket
    {
        public int code; // Store hash code; sign bit means there was a collision.
        public V data;
        public GCHandle key;
    }

    #endregion

    #region Constructors

    public WeakMap() : this(0, 1.0f) { }

    public WeakMap(int capacity) : this(capacity, 1.0f) { }

    public WeakMap(int capacity, float loadFactor)
    {
        if (capacity < 0)
            throw new ArgumentOutOfRangeException("capacity");
        if (!(loadFactor >= 0.1f && loadFactor <= 1.0f))
            throw new ArgumentOutOfRangeException("loadFactor");

        this.loadFactor = 0.72f * loadFactor;

        double rawsize = capacity / this.loadFactor;
        if (rawsize > int.MaxValue)
            throw new ArgumentException();

        int hashsize = NumberTheory.GetPrime((int)rawsize);
        buckets = new bucket[hashsize];
        bucketHandle = GCHandle.Alloc(buckets, GCHandleType.Normal);

        Capacity = (int)(this.loadFactor * hashsize);
        if (Capacity >= hashsize)
            Capacity = hashsize - 1;
    }

    public WeakMap(ICollection<KeyValuePair<K, V>> c)
        : this(c != null ? c.Count : 0)
    {
        if (c != null)
            foreach (KeyValuePair<K, V> entry in c)
                Add(entry.Key, entry.Value);
    }

    object ICloneable.Clone() => Clone();

    public WeakMap<K, V> Clone()
    {
        var map = new WeakMap<K, V>(Count);
        map.loadFactor = loadFactor;
        map.Count = 0;
        foreach (K key in this)
            map[key] = this[key];
        return map;
    }

    ~WeakMap()
    {
        for (int i = 0; i < buckets.Length; i++)
            SetKey(ref buckets[i], null);
        bucketHandle.Free();
    }

    #endregion

    #region Operations

    static uint InitHash(object key, int hashsize, out uint seed, out uint incr)
    {
        uint hashcode = (uint)key.GetHashCode() & 0x7FFFFFFF;
        seed = hashcode;
        incr = 1 + ((seed >> 5) + 1) % ((uint)hashsize - 1);
        return hashcode;
    }

    public void Clear()
    {
        if (Count == 0)
            return;

        for (int i = 0; i < buckets.Length; i++) {
            buckets[i].code = 0;
            SetKey(ref buckets[i], null);
        }

        Count = 0;
    }

    public bool Contains(K key)
    {
        if (key == null)
            throw new ArgumentNullException("key");

        uint seed;
        uint incr;
        // Take a snapshot of buckets, in case another thread resizes table
        bucket[] lbuckets = buckets;
        uint hashcode = InitHash(key, lbuckets.Length, out seed, out incr);
        int ntry = 0;

        int bucketNumber;
        do {
            bucketNumber = (int)(seed % (uint)lbuckets.Length);
            object? bucketKey = GetKey(ref lbuckets[bucketNumber]);
            if (bucketKey == null)
                return false;
            if ((lbuckets[bucketNumber].code & 0x7FFFFFFF) == hashcode && key.Equals(bucketKey))
                return true;
            seed += incr;
        } while (lbuckets[bucketNumber].code < 0 && ++ntry < lbuckets.Length);

        return false;
    }

    public void CopyTo(Array array, int arrayIndex)
    {
        bucket[] lbuckets = buckets;
        for (int i = lbuckets.Length; --i >= 0;) {
            object? keyv = GetKey(ref lbuckets[i]);
            if (keyv != null && keyv != buckets)
                array.SetValue(keyv, arrayIndex++);
        }
    }

    void Expand()
    {
        // Allocate new Array 
        int oldhashsize = buckets.Length;

        int rawsize = 1 + oldhashsize * 2;
        if (rawsize < 0)
            throw new ArgumentException();

        int hashsize = NumberTheory.GetPrime(rawsize);
        var newBuckets = new bucket[hashsize];

        // rehash table into new buckets
        for (int nb = 0; nb < oldhashsize; nb++) {
            object? bucketKey = GetKey(ref buckets[nb]);
            if (bucketKey != null && bucketKey != buckets)
                PutEntry(newBuckets, buckets[nb].key, buckets[nb].data, buckets[nb].code & 0x7FFFFFFF);
        }

        buckets = newBuckets;
        bucketHandle.Target = buckets;
        Capacity = (int)(loadFactor * hashsize);
        if (Capacity >= hashsize)
            Capacity = hashsize - 1;
    }

    void Sample()
    {
        int n = Math.Min(SampleSize, Capacity);
        int originalCount = 0;
        while (n-- > 0) {
            GetKey(ref buckets[n]);
            if (Count < originalCount)
                return;
        }
    }

    void RecalculateCounts()
    {
        int actualCount = 0;
        for (int i = 0; i < buckets.Length; i++) {
            object? o = GetKey(ref buckets[i]);
            if (o != null && o != buckets)
                actualCount++;
        }

        Debug.Assert(actualCount == Count);
        Count = actualCount;
    }

    public void Add(K key, V value)
    {
        Insert(key, value, true);
    }

    void Insert(K key, V value, bool add)
    {
        if (key == null)
            throw new ArgumentNullException("key");

        if (Count >= Capacity && !Contains(key)) {
            Sample();
            if (Count >= Capacity)
                Expand();
        }

        uint seed;
        uint incr;
        uint hashcode = InitHash(key, buckets.Length, out seed, out incr);
        int ntry = 0;
        int emptySlotNumber = -1;

        do {
            int bucketNumber = (int)(seed % (uint)buckets.Length);
            object? bucketKey = GetKey(ref buckets[bucketNumber]);
            if (emptySlotNumber == -1 && bucketKey == buckets && buckets[bucketNumber].code < 0)
                emptySlotNumber = bucketNumber;

            if (bucketKey == null ||
                (bucketKey == buckets && (buckets[bucketNumber].code & 0x80000000) == 0)) {
                if (emptySlotNumber != -1) // Reuse slot
                    bucketNumber = emptySlotNumber;

                buckets[bucketNumber].data = value;
                SetKey(ref buckets[bucketNumber], key);
                buckets[bucketNumber].code |= (int)hashcode;
                Count++;
                version++;
                return;
            }

            if ((buckets[bucketNumber].code & 0x7FFFFFFF) == hashcode &&
                key.Equals(bucketKey)) {
                if (add) throw new InvalidOperationException();
                buckets[bucketNumber].data = value;
                return;
            }

            if (emptySlotNumber == -1)
                buckets[bucketNumber].code |= unchecked((int)0x80000000);
            seed += incr;
        } while (++ntry < buckets.Length);

        if (emptySlotNumber != -1) {
            buckets[emptySlotNumber].data = value;
            SetKey(ref buckets[emptySlotNumber], key);
            buckets[emptySlotNumber].code |= (int)hashcode;
            Count++;
            version++;
            return;
        }

        throw new InvalidOperationException();
    }

    static void PutEntry(bucket[] newBuckets, GCHandle key, object value, int hashcode)
    {
        Debug.Assert(hashcode >= 0, "hashcode >= 0");

        uint seed = (uint)hashcode;
        uint incr = 1 + ((seed >> 5) + 1) % ((uint)newBuckets.Length - 1);

        do {
            int bucketNumber = (int)(seed % (uint)newBuckets.Length);
            if (!newBuckets[bucketNumber].key.IsAllocated) {
                newBuckets[bucketNumber].key = key;
                newBuckets[bucketNumber].code |= hashcode;
                newBuckets[bucketNumber].data = (V)value;
                return;
            }

            newBuckets[bucketNumber].code |= unchecked((int)0x80000000);
            seed += incr;
        } while (true);
    }

    public void Remove(K key)
    {
        if (key == null)
            throw new ArgumentNullException("key");

        uint seed;
        uint incr;
        uint hashcode = InitHash(key, buckets.Length, out seed, out incr);
        int ntry = 0;

        bucket b;
        int bn; // bucketNumber
        do {
            bn = (int)(seed % (uint)buckets.Length);
            b = buckets[bn];
            object bucketKey = GetKey(ref buckets[bn]);
            if ((b.code & 0x7FFFFFFF) == hashcode && key.Equals(bucketKey)) {
                buckets[bn].code &= unchecked((int)0x80000000);
                if (buckets[bn].code != 0)
                    SetKey(ref buckets[bn], buckets);
                else
                    SetKey(ref buckets[bn], null);
                Count--;
                return;
            }

            seed += incr;
        } while (buckets[bn].code < 0 && ++ntry < buckets.Length);
    }

    #endregion

    #region Properties

    public int Count { get; private set; }

    public int Capacity { get; private set; }

    public int ActualCount {
        get
        {
            int oldCount;
            do {
                oldCount = GC.CollectionCount(0);
                RecalculateCounts();
            } while (GC.CollectionCount(0) != oldCount);

            return Count;
        }
    }

    public V this[K key] {
        get
        {
            if (key == null)
                throw new ArgumentNullException("key");

            uint seed;
            uint incr;
            // Take a snapshot of buckets, in case another thread does a resize
            bucket[] lbuckets = buckets;
            uint hashcode = InitHash(key, lbuckets.Length, out seed, out incr);
            int ntry = 0;

            bucket b;
            do {
                int bucketNumber = (int)(seed % (uint)lbuckets.Length);
                b = lbuckets[bucketNumber];
                object? bucketKey = lbuckets[bucketNumber].key.Target;
                if (bucketKey == null)
                    return default;
                if ((b.code & 0x7FFFFFFF) == hashcode && key.Equals(bucketKey))
                    return lbuckets[bucketNumber].data;
                seed += incr;
            } while (b.code < 0 && ++ntry < lbuckets.Length);

            return default;
        }
        set => Insert(key, value, false);
    }

    #endregion

    #region Enumeration

    public IEnumerator<K> GetEnumerator()
    {
        int bucket = buckets.Length;
        int v = version;

        while (bucket-- > 0) {
            object? key = GetKey(ref buckets[bucket]);
            if (key != null && key != buckets) {
                if (v != version)
                    throw new InvalidOperationException();
                yield return (K)key;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region Bucket Operations

    object GetKey(ref bucket b)
    {
        if (!b.key.IsAllocated)
            return null;

        var obj = b.key.Target as K;
        if (obj != null)
            return obj;

        b.code &= unchecked((int)0x80000000);
        b.data = default;
        Count--;
        Debug.Assert(Count >= 0);

        if (b.code != 0) {
            b.key.Target = buckets;
            return buckets;
        }

        b.key.Free();
        return null;
    }

    static void SetKey(ref bucket b, object value)
    {
        if (value == null) {
            if (b.key.IsAllocated)
                b.key.Free();
            b.data = default;
            return;
        }

        if (b.key.IsAllocated) {
            b.key.Target = value;
            return;
        }

        b.key = GCHandle.Alloc(value, GCHandleType.Weak);
    }

    #endregion
}