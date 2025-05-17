#region Using

using System.Collections;
using System.Diagnostics.Contracts;

#endregion

namespace Algorithms.Collections;

/// <summary>
///     Summary description for PArray
/// </summary>
[DebuggerStepThrough]
public class PMap<K, V> : Persistent<Dictionary<K, V>>, IEnumerable<KeyValuePair<K, V>>
    where K : IEquatable<K>
    where V : class
{
    #region Helper Classes

    private class Diff : Change
    {
        private readonly K Key;
        private V Value;

        public Diff(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public override Change Apply(Dictionary<K, V> a)
        {
            var key = Key;
            V vp;
            a.TryGetValue(key, out vp);

            if (Value != null)
                a[key] = Value;
            else
                a.Remove(key);

            Value = vp;
            return this;
        }
    }

    #endregion

    #region Constructor

    public PMap(Dictionary<K, V> array)
        : base(array)
    {
    }

    public PMap()
        : this(new Dictionary<K, V>())
    {
    }

    private PMap(Change change, PMap<K, V> p)
        : base(change, p)
    {
    }

    #endregion

    #region Properties

    public int Count
    {
        get { return Data.Count; }
    }

    [Pure]
    public V this[K key]
    {
        get
        {
            V value;
            Data.TryGetValue(key, out value);
            return value;
        }
    }

    [Pure]
    public bool TryGetValue(K key, out V value)
    {
        return Data.TryGetValue(key, out value);
    }

    [Pure]
    public PMap<K, V> Set(K key, V element)
    {
        var arr = Data;
        V old;
        var found = arr.TryGetValue(key, out old);
        if (found && element.Equals(old))
            return this;

        return new PMap<K, V>(new Diff(key, element), this);
    }

    [Pure]
    public PMap<K, V> Add(K key, V element)
    {
        var arr = Data;
        if (arr.ContainsKey(key))
            throw new InvalidOperationException();

        return new PMap<K, V>(new Diff(key, element), this);
    }

    [Pure]
    public PMap<K, V> Remove(K key)
    {
        var arr = Data;
        V old;
        if (!arr.TryGetValue(key, out old))
            return this;
        return new PMap<K, V>(new Diff(key, default(V)), this);
    }

    #endregion

    #region Methods

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IEnumerable<V> Values
    {
        get { return Data.Values; }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public IEnumerable<K> Keys
    {
        get { return Data.Keys; }
    }

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        return Data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool ContainsKey(K key)
    {
        return Data.ContainsKey(key);
    }

    public override string ToString()
    {
        return Data.ToString();
    }

    #endregion
}