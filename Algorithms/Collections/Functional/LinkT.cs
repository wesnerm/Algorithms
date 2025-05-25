#region Copyright

//  This source code may not be reviewed, copied, or redistributed without
//  the expressed permission of Wesner Moise.
//  
//  File: LispLink.cs
//  Created: 10/13/2012 
//  Modified: 10/13/2012
// 
//  Copyright (C) 2012 - 2012, Wesner Moise.

#endregion

namespace Algorithms.Collections;

[Pure]
[DebuggerDisplay("{ToString(\"200\")}")]
public class Link<T> : IList<T>,
    IEquatable<Link<T>>,
    IComparable,
    IComparable<Link<T>>,
    IFormattable
{
    int IComparable.CompareTo(object obj)
    {
        if (obj == null)
            return 1;
        return CompareTo((Link<T>)obj);
    }

    public int CompareTo(Link<T> other)
    {
        Link<T>? current1 = this;
        Link<T>? current2 = other;

        var comparer = Comparer<T>.Default;
        // ReSharper disable RedundantCast
        while (current1 != current2)
            // ReSharper restore RedundantCast
        {
            if (current1 == null)
                return -1;
            if (current2 == null)
                return 1;
            int cmp = comparer.Compare(current1.First, current2.First);
            if (cmp != 0)
                return cmp;

            current1 = current1.Rest;
            current2 = current2.Rest;
        }

        return 0;
    }

    public bool Equals(Link<T> other)
    {
#pragma warning disable 252,253
        if (other == null)
            return false;
        if (other == this)
#pragma warning restore 252,253
            return true;

        if (Count != other.Count || _hashCode != other._hashCode)
            return false;

        Link<T> current1 = this;
        Link<T> current2 = other;

        // ReSharper disable RedundantCast
        var comparer = EqualityComparer<T>.Default;
        while ((object)current1 != (object)current2
               && current1.IsNotEmpty)
            // ReSharper restore RedundantCast
        {
            if (!comparer.Equals(current1.First, current2.First))
                return false;

            current1 = current1.Rest;
            current2 = current2.Rest;
        }

        return true;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        return Equals(obj as Link<T>);
    }

    public override int GetHashCode() => _hashCode;

    public Link<T> ChangeTail(Link<T> newTail, int fromEnd = 0)
    {
        Debug.Assert(newTail != null);

        int count = Count;
        if (count <= fromEnd) {
            Debug.Assert(count == fromEnd);
            return newTail;
        }

        if (fromEnd == 0 && newTail.IsEmpty)
            return this;

        Link<T> reversed = Reverse(this, fromEnd);
        return ReverseX(reversed, newTail);
    }

    static Link<T> Reverse(Link<T> link, int fromEnd = 0)
    {
        Link<T> reversed = null;
        int skip = link.Count - fromEnd;
        for (Link<T> current = link; skip > 0; skip--, current = current._rest)
            reversed = new Link<T>(current.First, reversed);
        return reversed ?? Empty;
    }

    public Link<T> Reverse()
    {
        if (Count <= 1)
            return this;
        return Reverse(this);
    }

    public static Link<T> ReverseX(Link<T> link, Link<T> tail = null)
    {
        Link<T> current = link ?? Empty;
        Link<T> reversed = tail ?? Empty;

        while (current.IsNotEmpty) {
            Link<T> tmp = current.Rest;
            current.Rest = reversed;
            reversed = current;
            current = tmp;
        }

        return reversed;
    }

    [Pure]
    public Link<T> Add(T element) => Append(new Link<T>(element));

    [Pure]
    public Link<T> Append(Link<T> link) => ChangeTail(link);

    [Pure]
    public Link<T> Prepend(Link<T> collection) => Prepend(this, collection);

    [Pure]
    public Link<T> Replace(int position, int count, params T[] insertData) => ReplaceRange(position, count, insertData);

    [Pure]
    public Link<T> ReplaceRange(int position, int count, IEnumerable<T> lisp = null)
    {
        Link<T> start = Drop(position);
        Link<T> end = start.Drop(count);
        return ChangeTail(Prepend(end, lisp), start.Count);
    }

    [Pure]
    public Link<T> RemoveRange(int position, int count) => ReplaceRange(position, count);

    [Pure]
    public Link<T> InsertAt(int position, T element) => Replace(position, 0, element);

    [Pure]
    public Link<T> ReplaceAt(int position, T element) => Replace(position, 1, element);

    [Pure]
    public static Link<T> Prepend(Link<T> link, IEnumerable<T> collection)
    {
        if (collection == null)
            return link;

        Link<T> current;

        if (link.Count == 0) {
            current = collection as Link<T>;
            if (current != null)
                return current;
        }

        current = Empty;
        foreach (T item in collection)
            if (item != null)
                current = new Link<T>(item, current);
        return ReverseX(current, link);
    }

    [Pure]
    public Link<T> Remove(Func<T, bool> func)
    {
        return Convert(x => func(x) ? default : x);
    }

    [Pure]
    public Link<T> Convert(Func<T, T> func)
    {
        Link<T> tally = null;
        Link<T> tail = null;
        Link<T> firstChange = null;
        var comparer = EqualityComparer<T>.Default;
        for (Link<T> current = this; current.IsNotEmpty; current = current.Rest) {
            T f = func(current.First);
            if (!comparer.Equals(f, current.First)) {
                if (firstChange == null)
                    firstChange = current;
                else
                    for (; tail != current; tail = tail.Rest)
                        tally = new Link<T>(tail.First, tally);

                tally = f == null ? tally : new Link<T>(f, tally);
                tail = current.Rest;
            }
        }

        if (firstChange == null)
            return this;
        tally = ReverseX(tally, tail);
        return ChangeTail(tally, firstChange.Count);
    }

    [Pure]
    public Link<T> ConvertAt(int pos, Func<T, T> func)
    {
        Link<T> locate = Drop(pos);
        if (locate.IsEmpty)
            return this;
        T newFirst = func(locate.First);
        if (EqualityComparer<T>.Default.Equals(newFirst, locate.First))
            return this;
        return ChangeTail(new Link<T>(newFirst, locate.Rest), locate.Count);
    }

    public Link<T> Drop(int count)
    {
        if (count >= Count)
            return Empty;

        Link<T> current = this;
        while (count-- > 0 && current.IsNotEmpty)
            current = current.Rest;
        return current;
    }

    [Pure]
    public Link<T> Take(int count) => Subrange(0, count);

    [Pure]
    public Link<T> Subrange(int position, int count)
    {
        if (count == 0)
            return Empty;

        Link<T> start = Drop(position);
        if (start.IsEmpty)
            return start;
        Link<T> end = start.Drop(count);
        return start.ChangeTail(Empty, end.Count);
    }

    public Link<T> Sort(Comparison<T> sortFunc = null) => From(this.Sorted(sortFunc));

    public static Link<T> operator |(T head, Link<T> rest) => new(head, rest);

    public static Link<T> operator +(Link<T> first, Link<T> second) => first.Append(second);

    #region Variables

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Link<T> _rest;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    T _first;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    int _hashCode;

    #endregion

    #region Constructor

    [DebuggerStepThrough]
    public static Link<T> From(params object[] data) => From(Flatten(data));

    [DebuggerStepThrough]
    public static Link<T> From(IEnumerable<T> link) => Prepend(Empty, link);

    [DebuggerStepThrough]
    public Link(T first, Link<T> next = null)
    {
        Debug.Assert(first != null);
        _first = first;
        _rest = next ?? Empty;
        Recalculate();
    }

    [DebuggerStepThrough]
    Link()
    {
        _rest = this;
        Recalculate();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public static readonly Link<T> Empty = new();

    #endregion

    #region Overrides

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T First {
        get => _first;
        set
        {
            _first = value;
            Recalculate();
        }
    }

    [DebuggerStepThrough]
    void Recalculate()
    {
        Link<T> next = _rest;

        int resthash;
        if (next == this) {
            Count = 0;
            resthash = unchecked((int)0xdeadbeef);
        } else {
            Count = next.Count + 1;
            resthash = next.GetHashCode();
        }

        int firsthash = _first != null ? _first.GetHashCode() : 0;
        _hashCode = HashCode.Combine(firsthash, resthash);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Link<T> Rest {
        get => _rest;
        set
        {
            _rest = value;
            Recalculate();
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsEmpty => Count <= 0;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsNotEmpty => Count > 0;

    #endregion

    #region List Members

    public IEnumerator<T> GetEnumerator()
    {
        for (Link<T> current = this; current.IsNotEmpty; current = current.Rest)
            yield return current.First;
    }

    void ICollection<T>.Add(T item)
    {
        throw new InvalidOperationException();
    }

    void ICollection<T>.Clear()
    {
        throw new InvalidOperationException();
    }

    public bool Contains(T item) => IndexOf(item) >= 0;

    public void CopyTo(T[] array, int arrayIndex)
    {
        ListTools.CopyTo(this, array, arrayIndex);
    }

    bool ICollection<T>.Remove(T item) => throw new InvalidOperationException();

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Count { get; private set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly => true;

    public int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        int count = 0;
        for (Link<T> current = this; current.IsNotEmpty; current = current.Rest) {
            if (comparer.Equals(item, current.First))
                return count;
            count++;
        }

        return -1;
    }

    void IList<T>.Insert(int index, T item)
    {
        throw new InvalidOperationException();
    }

    void IList<T>.RemoveAt(int index)
    {
        throw new InvalidOperationException();
    }

    public T this[int index] {
        get
        {
            int count = 0;
            for (Link<T> current = this; current.IsNotEmpty; current = current.Rest) {
                if (count == index)
                    return current.First;
                count++;
            }

            return default;
        }
        set => throw new InvalidOperationException();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] DebugList => this.ToArray();

    #endregion

    #region To String

    public string ToString(string format, IFormatProvider formatProvider = null)
    {
        bool indent = false;
        int limit = int.MaxValue;

        if (!string.IsNullOrEmpty(format)) {
            int newLimit;
            if (format == "I")
                indent = true;
            else if (char.IsDigit(format[0]) && int.TryParse(format, out newLimit))
                limit = newLimit;
        }

        var builder = new StringBuilder();
        ToStringGuts(builder, indent, limit);
        string result = builder.ToString();
        return result;
    }

    public override string ToString() => ToString("");

    void ToStringGuts(StringBuilder builder, bool doIndent = true, int limit = int.MaxValue)
    {
        int indent = 2;
        int oldLength = builder.Length;

        if (doIndent)
            for (int pos = builder.Length - 1; pos > 0; pos--) {
                if (builder[pos] == '\n')
                    break;
                indent++;
            }

        builder.Append("(");
        bool newline = false;

        int ii = 0;
        for (Link<T> node = this; node.IsNotEmpty; node = node.Rest) {
            if (builder.Length > limit)
                return;

            if (ii != 0) {
                if (newline) {
                    builder.AppendLine();
                    builder.Append(' ', indent);
                } else {
                    builder.Append(' ');
                }
            }

            object? nodeData = node.First;
            var listNode = nodeData as Link<T>;
            if (listNode != null) {
                listNode.ToStringGuts(builder, doIndent, limit);
                if (doIndent)
                    newline = true;
            } else {
                if (nodeData != null)
                    builder.Append(nodeData);
            }

            if (!newline && ii == 0)
                indent += builder.Length - oldLength - 1;
            ii++;
        }

        builder.Append(")");
    }

    #endregion

    #region Misc

    [Pure]
    public static IEnumerable<T> Flatten(IEnumerable<object> args)
    {
        if (typeof(T) == typeof(object))
            return (IEnumerable<T>)FlattenObject(args);
        return ListTools.Flatten<T>(args);
    }

    [Pure]
    static IEnumerable<object> FlattenObject(IEnumerable<object> args)
    {
        foreach (object arg in args) {
            var en = arg as IEnumerable<object>;
            if (en == null || arg is Link<object>) {
                yield return arg;
                continue;
            }

            foreach (object arg2 in en)
                yield return arg2;
        }
    }

    #endregion
}