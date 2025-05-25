#region Using

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
[DebuggerTypeProxy(typeof(SortedCollectionDebugView<>))]
[Serializable]
public struct Set<K> : ICollection<K>, ISerializable
    where K : IComparable<K>, IEquatable<K>
{
    #region Variables

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    TreapNode _node;

    #endregion

    #region Constructor

    //[DebuggerStepThrough]
    Set(TreapNode node)
    {
        _node = node;
        TreapNode.Seal(node);
    }

    Set<K> Construct(TreapNode node)
    {
        if (node == _node) return this;
        if (node == null) return Empty;
        return new Set<K>(node);
    }

    public static readonly Set<K> Empty = new((TreapNode)null);

    public Set(IEnumerable<K> collection)
    {
        _node = null;
        foreach (K item in collection)
            _node = TreapNode.Insert(_node, new TreapNode(item));
        TreapNode.Seal(_node);
    }

    public Set(SerializationInfo info)
        : this((K[])info.GetValue("Values", typeof(K[]))) { }

    void ISerializable.GetObjectData(SerializationInfo info,
        StreamingContext context)
    {
        info.AddValue("Values", this.ToArray());
    }

    #endregion

    #region Properties

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //public override EvalFlags Evaluation
    //{
    //    [DebuggerStepThrough]
    //    get
    //    {
    //        if (_setNode == null)
    //            return EvalFlags.AndOperators;

    //        return _setNode.Flags ^ EvalFlags.AndOperators;
    //    }
    //}

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //public override Exp Head
    //{
    //    [DebuggerStepThrough]
    //    get { return Symbols.Set; }
    //}

    //[DebuggerBrowsable(DebuggerBrowsableState.Never)]
    //public override SymbolId HeadId
    //{
    //    [DebuggerStepThrough]
    //    get { return SymbolId.Set; }
    //}

    #endregion

    #region Methods

    bool Exchange(TreapNode newNode, TreapNode originalNode)
    {
        if (newNode == originalNode) return true;
        TreapNode actualOriginal = Interlocked.CompareExchange(
            ref _node, newNode, originalNode);
        return actualOriginal == originalNode;
    }

    public void RemoveInPlace(K key)
    {
        while (true) {
            TreapNode? startVal = _node;
            TreapNode desiredVal = TreapNode.Remove(startVal, key);
            if (Exchange(desiredVal, startVal))
                return;
        }
    }

    public void AddInPlace(K key)
    {
        while (true) {
            TreapNode? startVal = _node;
            TreapNode desiredVal = TreapNode.Insert(startVal, new TreapNode(key));
            if (Exchange(desiredVal, startVal))
                return;
        }
    }

    [DebuggerStepThrough]
    public Set<K> AddAndCopy(K key)
    {
        TreapNode n = TreapNode.Insert(_node, new TreapNode(key));
        return Construct(n);
    }

    [DebuggerStepThrough]
    public Set<K> AddRangeAndCopy(IEnumerable<K> collection)
    {
        TreapNode? n = _node;
        foreach (K item in collection)
            n = TreapNode.Insert(n, new TreapNode(item));
        return Construct(n);
    }

    [DebuggerStepThrough]
    public Set<K> RemoveAndCopy(K key)
    {
        TreapNode n = TreapNode.Remove(_node, key);
        return Construct(n);
    }

    //public override Exp Map(MapOptions options, Func<Exp, Exp> func)
    //{
    //    var set = this as Set<Exp>;
    //    if (set == null)
    //        return this;

    //    {
    //        var mapFunc = func;
    //        var newNode =
    //            Treap<Exp, ExpNode>.Transform(set._setNode, n=>
    //            {
    //                var v = mapFunc(n.Key);
    //                if (v == null) return null;
    //                return v.Equals(n.Key) ? n : new ExpNode(v);
    //            });
    //        return set.Construct(newNode);
    //    }
    //}

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

    [DebuggerStepThrough]
    public void ForEach(Action<K> action)
    {
        if (_node != null)
            _node.ForEach(action);
    }

    [DebuggerStepThrough]
    public Set<K> Filter(Func<K, bool> filter)
    {
        TreapNode result = TreapNode.Filter(_node, filter);
        return Construct(result);
    }

    #endregion

    #region Helpers

    #region IEnumerable<K> Members

#line hidden
    public IEnumerator<K> GetEnumerator()
    {
        if (_node == null)
            return Enumerable.Empty<K>().GetEnumerator();
        return _node.GetEnumerator();
    }
#line default

    [DebuggerStepThrough]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion

    #region ICollection<K> Members

    [DebuggerStepThrough]
    void ICollection<K>.Add(K item)
    {
        throw new InvalidOperationException();
    }

    [DebuggerStepThrough]
    void ICollection<K>.Clear()
    {
        throw new InvalidOperationException();
    }

    [DebuggerStepThrough]
    public bool Contains(K item) => TreapNode.Contains(_node, item);

    [DebuggerStepThrough]
    public void CopyTo(K[] array, int arrayIndex)
    {
        ListTools.CopyTo(this, array, arrayIndex);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int ICollection<K>.Count => TreapNode.Count(_node);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<K>.IsReadOnly => true;

    [DebuggerStepThrough]
    bool ICollection<K>.Remove(K item) => throw new InvalidOperationException();

    #endregion

    #endregion

    #region Node

    [DebuggerNonUserCode]
    internal class TreapNode : Treap<K, TreapNode>
    {
        #region Construction

        public TreapNode(K key)
            : base(key) { }

        #endregion

        protected override K Key { get; set; }

        public TreapNode Transform(Func<K, K> mapFunc)
        {
            return Transform(this, n => {
                K? k = mapFunc(n.Key);
                if (k == null)
                    return null;
                if (k.Equals(n.Key))
                    return n;
                return new TreapNode(k);
            });
        }

        public IEnumerator<K> GetEnumerator()
        {
            return Select(this, n => n.Key).GetEnumerator();
        }

        [DebuggerStepThrough]
        public void ForEach(Action<K> action)
        {
            ForEach(this, n => action(n.Key));
        }
    }

    #endregion

    #region Set Operations

    [DebuggerStepThrough]
    static Set<K> Construct(Set<K> s1, Set<K> s2, TreapNode node)
    {
        if (node == null) return Empty;
        if (node == s1._node) return s1;
        if (node == s2._node) return s2;
        TreapNode.Seal(node);
        return new Set<K>(node);
    }

    [DebuggerStepThrough]
    public Set<K> Union(Set<K> set) => Construct(this, set, TreapNode.Union(_node, set._node));

    [DebuggerStepThrough]
    public Set<K> Intersection(Set<K> set) => Construct(this, set, TreapNode.Intersection(_node, set._node));

    [DebuggerStepThrough]
    public Set<K> Difference(Set<K> set) => Construct(this, set, TreapNode.Difference(_node, set._node));

    [DebuggerStepThrough]
    public Set<K> ExclusiveUnion(Set<K> set) => Construct(this, set, TreapNode.ExclusiveUnion(_node, set._node));

    public Set<K> Transform(Func<K, K> mapFunc)
    {
        TreapNode? newNode = _node?.Transform(mapFunc);
        return Construct(newNode);
    }

    #endregion

    #region Object Overrides

    public bool Equals(Set<K> set) => TreapNode.TreeEquals(_node, set._node);

    public override int GetHashCode()
    {
        int hashcode = TreapNode.TreeHashCode(_node);
        if (hashcode == 0) hashcode = 0x53453453;
        return hashcode;
    }

    //public override bool Equals(Exp exp)
    //{
    //    return Equals(exp as Set<K>);
    //}

    //protected override int CompareToOverride(Exp other)
    //{
    //    var m = other as Set<K>;
    //    if (m != null)
    //        return SetNode.TreeCompare(_setNode, m._setNode);

    //    // This will never occur because only sets have Id.Set
    //    return base.CompareToOverride(other);
    //}

    #endregion
}