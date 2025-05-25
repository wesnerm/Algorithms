#region

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2006 - 2008, Wesner Moise.
/////////////////////////////////////////////////////////////////////////////

using System.Runtime.Serialization;

#endregion

namespace Algorithms.Collections;

[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(SortedDictionaryDebugView<,>))]
[Serializable]
public struct Map<TK, TV> : IDictionary<TK, TV>, ISerializable
    where TK : IComparable<TK>, IEquatable<TK>
{
    #region Variables

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TreapNode _node;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static readonly Map<TK, TV> Empty = new((TreapNode)null);

    #endregion

    #region Constructor

    [DebuggerStepThrough]
    Map(TreapNode node)
    {
        _node = node;
        TreapNode.Seal(node);
    }

    public Map(IEnumerable<KeyValuePair<TK, TV>> collection)
    {
        _node = null;
        foreach (KeyValuePair<TK, TV> item in collection)
            _node = TreapNode.Insert(_node,
                new TreapNode(item.Key, item.Value));
        TreapNode.Seal(_node);
    }

    public Map(IEnumerable<TK> col1, IEnumerable<TV> col2)
    {
        _node = null;
        Insert(col1, col2);
        TreapNode.Seal(_node);
    }

    Map<TK, TV> Construct(TreapNode node)
    {
        if (node == _node) return this;
        if (node == null) return Empty;
        var map = new Map<TK, TV>(node);
        return map;
    }

    void Insert(IEnumerable<TK> col1, IEnumerable<TV> col2)
    {
        IEnumerator<TK> enk = col1.GetEnumerator();
        IEnumerator<TV> env = col2.GetEnumerator();

        while (true) {
            bool b1 = enk.MoveNext();
            bool b2 = env.MoveNext();

            if (b1 != b2)
                throw new InvalidOperationException();
            if (!b1)
                break;

            _node = TreapNode.Insert(_node,
                new TreapNode(enk.Current, env.Current));
        }
    }

    #endregion

    #region Methods

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Count {
        get
        {
            int count = TreapNode.Count(_node);
            return count;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsEmpty => _node == null;

    bool Exchange(TreapNode newNode, TreapNode originalNode)
    {
        if (newNode == originalNode) return true;
        TreapNode actualOriginal = Interlocked.CompareExchange(
            ref _node,
            newNode,
            originalNode);
        return actualOriginal == originalNode;
    }

    public void RemoveInPlace(TK key)
    {
        while (true) {
            TreapNode? startVal = _node;
            TreapNode desiredVal = TreapNode.Remove(startVal, key);
            TreapNode.Seal(desiredVal);
            if (Exchange(desiredVal, startVal))
                return;
        }
    }

    public void AddInPlace(TK key, TV value)
    {
        while (true) {
            TreapNode? startVal = _node;
            TreapNode desiredVal = TreapNode.Insert(startVal, new TreapNode(key, value));
            TreapNode.Seal(desiredVal);
            if (Exchange(desiredVal, startVal))
                return;
        }
    }

    [DebuggerStepThrough]
    [Pure]
    public Map<TK, TV> Add(TK key, TV value)
    {
        TreapNode n = TreapNode.Insert(_node, new TreapNode(key, value));
        return Construct(n);
    }

    [DebuggerStepThrough]
    [Pure]
    public Map<TK, TV> AddRange(IEnumerable<KeyValuePair<TK, TV>> collection)
    {
        TreapNode? n = _node;
        foreach (KeyValuePair<TK, TV> item in collection)
            n = TreapNode.Insert(n, new TreapNode(item.Key, item.Value));
        return Construct(n);
    }

    [DebuggerStepThrough]
    [Pure]
    public Map<TK, TV> Remove(TK key)
    {
        TreapNode n = TreapNode.Remove(_node, key);
        return Construct(n);
    }

    [DebuggerStepThrough]
    [Pure]
    public Map<TK, TV> Update(TK key, TV value) =>
        value == null
            ? Remove(key)
            : Add(key, value);

    [DebuggerStepThrough]
    public void ForEach(Action<TK, TV> action)
    {
        TreapNode.ForEach(_node, action);
    }

    /// <summary>
    /// </summary>
    /// <param name="filter">returns true to delete</param>
    /// <returns></returns>
    [DebuggerStepThrough]
    [Pure]
    public Map<TK, TV> Filter(Func<TK, TV, bool> filter) => Construct(TreapNode.Filter(_node, filter));

    [DebuggerStepThrough]
    [Pure]
    public Map<TK, TV> Transform(Func<TK, TV, TV> mapFunc) => Construct(TreapNode.Transform(_node, mapFunc));

    [Pure]
    public Map<TK, TV> Transform(Func<TV, TV> mapFunc) => Construct(TreapNode.Transform(_node, mapFunc));

    [Pure]
    public Map<TK, TV> Transform(Func<(TK, TV), (TK, TV)> mapFunc) => Construct(TreapNode.Transform(_node, mapFunc));

    #endregion

    #region Node

    [DebuggerNonUserCode]
    internal class TreapNode : Treap<TK, TreapNode>
    {
        #region Variables

        public readonly TV Value;

        #endregion

        #region Construction

        public TreapNode(TK key, TV value) : base(key) => Value = value;

        #endregion

        #region Overrides

        protected override TK Key { get; set; }

        protected override bool NodeEquals(TreapNode map) => EqualityComparer<TV>.Default.Equals(Value, map.Value);

        public override int NodeCompare(TreapNode map) => Comparer<TV>.Default.Compare(Value, map.Value);

        protected override int NodeGetHashCode() => Value.GetHashCode();

        [DebuggerStepThrough]
        [Pure]
        public static TreapNode Transform(TreapNode node, Func<TK, TV, TV> mapFunc)
        {
            return Transform(node,
                n => {
                    TV v = mapFunc(n.Key, n.Value);
                    if (v == null)
                        return null;
                    if (v.Equals(n.Value))
                        return n;
                    return new TreapNode(n.Key, v);
                });
        }

        [Pure]
        public static TreapNode Transform(TreapNode node, Func<TV, TV> mapFunc)
        {
            return Transform(node,
                n => {
                    TV v = mapFunc(n.Value);
                    if (v == null)
                        return null;
                    if (v.Equals(n.Value))
                        return n;
                    return new TreapNode(n.Key, v);
                });
        }

        [Pure]
        public static TreapNode Transform(TreapNode node, Func<(TK, TV), (TK, TV)> mapFunc)
        {
            return Transform(node,
                n => {
                    (TK key, TV value) = mapFunc((n.Key, n.Value));
                    if (value == null)
                        return null;
                    if ((object)key == (object)n.Key
                        &&
                        (object)value == (object)n.Value)
                        return n;
                    return new TreapNode(key, value);
                });
        }

        [DebuggerStepThrough]
        public static void ForEach(TreapNode node, Action<TK, TV> action)
        {
            ForEach(node, n => action(n.Key, n.Value));
        }

        /// <summary>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="filter">returns true to delete</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static TreapNode Filter(TreapNode node, Func<TK, TV, bool> filter)
        {
            return Transform(node, n => filter(n.Key, n.Value) ? null : n);
        }

        [DebuggerStepThrough]
        public static IEnumerable<TK> GetKeys(TreapNode node)
        {
            return Select(node, n => n.Key);
        }

        [DebuggerStepThrough]
        public static IEnumerable<TV> GetValues(TreapNode node)
        {
            return Select(node, n => n.Value);
        }

        [DebuggerStepThrough]
        public static IEnumerable<KeyValuePair<TK, TV>> GetKeyValues(TreapNode node)
        {
            return Select(node, n => new KeyValuePair<TK, TV>(n.Key, n.Value));
        }

        public override string ToString() => Key + " = " + Value;

        #endregion
    }

    #endregion

    #region Helpers

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    public IEnumerable<TK> Keys => TreapNode.GetKeys(_node);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    public IEnumerable<TV> Values => TreapNode.GetValues(_node);

    [Pure]
    public TV this[TK key] {
        get
        {
            TreapNode? n = TreapNode.GetValue(_node, key);
            return n != null ? n.Value : default;
        }
    }

    [DebuggerNonUserCode]
    [Pure]
    public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator() => TreapNode.GetKeyValues(_node).GetEnumerator();

    [DebuggerStepThrough]
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void IDictionary<TK, TV>.Add(TK key, TV value)
    {
        throw new InvalidOperationException();
    }

    [DebuggerStepThrough]
    [Pure]
    public bool ContainsKey(TK key) => TreapNode.Contains(_node, key);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    ICollection<TK> IDictionary<TK, TV>.Keys => ListTools.ToCollection(Keys);

    bool IDictionary<TK, TV>.Remove(TK key) => throw new InvalidOperationException();

    [DebuggerStepThrough]
    public bool TryGetValue(TK key, out TV value)
    {
        TreapNode? n = TreapNode.GetValue(_node, key);
        value = n != null ? n.Value : default;
        return n != null;
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Pure]
    ICollection<TV> IDictionary<TK, TV>.Values => ListTools.ToCollection(Values);

    TV IDictionary<TK, TV>.this[TK key] {
        get => this[key];
        set => throw new InvalidOperationException();
    }

    void ICollection<KeyValuePair<TK, TV>>.Add(KeyValuePair<TK, TV> item)
    {
        throw new InvalidOperationException();
    }

    void ICollection<KeyValuePair<TK, TV>>.Clear()
    {
        throw new InvalidOperationException();
    }

    [Pure]
    public bool Contains(KeyValuePair<TK, TV> item)
    {
        TreapNode? n = TreapNode.GetValue(_node, item.Key);
        if (n == null)
            return false;
        return EqualityComparer<TV>.Default.Equals(n.Value, item.Value);
    }

    public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
    {
        ListTools.CopyTo(this, array, arrayIndex);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int ICollection<KeyValuePair<TK, TV>>.Count => TreapNode.Count(_node);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<KeyValuePair<TK, TV>>.IsReadOnly => true;

    bool ICollection<KeyValuePair<TK, TV>>.Remove(KeyValuePair<TK, TV> item) => throw new InvalidOperationException();

    #endregion

    #region Object Overrides

    public bool Equals(Map<TK, TV> map) => TreapNode.TreeEquals(_node, map._node);

    public override int GetHashCode()
    {
        int hashcode = TreapNode.TreeHashCode(_node);
        return hashcode;
    }

    //public override bool Equals(Exp exp)
    //{
    //    return Equals(exp as Hash<K, V>);
    //}

    //protected override int CompareToOverride(Exp other)
    //{
    //    var m = other as Hash<K, V>;
    //    if (m != null)
    //        return Treap<K, HashNode>.TreeCompare(_hashNode, m._hashNode);
    //    return base.CompareToOverride(other);
    //}

    #endregion

    #region Evaluation

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //public override EvalFlags Evaluation
    //{
    //    [DebuggerStepThrough]
    //    get
    //    {
    //        // POSTPONE: Selectively scan only those nodes that are interesting
    //        if (_hashNode == null)
    //            return EvalFlags.Constant | EvalFlags.Evaluated;

    //        Debug.Assert(Treap<K, HashNode>.IsSealed(_hashNode));
    //        return _hashNode.Flags ^ EvalFlags.AndOperators;
    //    }
    //}

    //public override Exp Eval()
    //{
    //    if (IsEvaluated) return this;
    //    return EvalCore();
    //}

    //private Exp EvalCore()
    //{
    //    // POSTPONE: A thunk may be needed / also selectively ignore branchs
    //    var result = Map(0, ExpTools.EvalFunc);
    //    return result;
    //}

    #endregion

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Keys", Keys.ToArray());
        info.AddValue("Values", Values.ToArray());
    }

    public Map(SerializationInfo info, StreamingContext context)
    {
        var keys = (TK[])info.GetValue("Keys", typeof(TK[]));
        var values = (TV[])info.GetValue("Values", typeof(TV[]));
        this = new Map<TK, TV>(keys, values);
    }

    #endregion
}