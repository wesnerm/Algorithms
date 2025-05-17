#region Using

using System.Collections;
using System.Diagnostics.Contracts;

#endregion

namespace Algorithms.Collections;

/// <summary>
///     Summary description for PSet
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof (CollectionDebugView<>))]
public class PSet<K> : Persistent<LiteSet<K>>, IEnumerable<K>
    where K : class
{
    #region Helper Classes

    private class Diff : Change
    {
        private readonly K Key;
        private bool Insert;

        public Diff(bool insert, K key)
        {
            Insert = insert;
            Key = key;
        }

        public override Change Apply(LiteSet<K> a)
        {
            if (Insert) a.Add(Key);
            else a.Remove(Key);
            Insert = !Insert;
            return this;
        }
    }

    #endregion

    #region Constructor

    [DebuggerStepThrough]
    public PSet(LiteSet<K> array)
        : base(array)
    {
    }

    public PSet()
        : base(new LiteSet<K>())
    {
    }

    [DebuggerStepThrough]
    private PSet(Change change, PSet<K> p)
        : base(change, p)
    {
    }

    #endregion

    #region Properties

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    public int Count
    {
        get { return Data.Count; }
    }

    [Pure]
    public K this[K key]
    {
        get { return Data[key]; }
    }

    [Pure]
    public PSet<K> Add(K key)
    {
        var arr = Data;
        if (arr.Contains(key))
            return this;

        arr.Add(key);
        return new PSet<K>(new Diff(true, key), this);
    }

    [Pure]
    public PSet<K> Remove(K key)
    {
        var arr = Data;
        if (!arr.Remove(key))
            return this;

        return new PSet<K>(new Diff(false, key), this);
    }

    #endregion

    #region Methods

#line hidden
    public IEnumerator<K> GetEnumerator()
    {
        return Data.GetEnumerator();
    }
#line default

    public bool Contains(K key)
    {
        return Data.Contains(key);
    }

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #endregion
}