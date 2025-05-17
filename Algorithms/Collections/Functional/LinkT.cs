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

#region Usings

using System.Collections;
using System.Diagnostics.Contracts;
using System.Text;
using Algorithms.Collections;

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
        return CompareTo((Link<T>) obj);
    }

    public int CompareTo(Link<T> other)
    {
        var current1 = this;
        var current2 = other;

        var comparer = Comparer<T>.Default;
        // ReSharper disable RedundantCast
        while (current1 != current2)
            // ReSharper restore RedundantCast
        {
            if (current1 == null)
                return -1;
            if (current2 == null)
                return 1;
            var cmp = comparer.Compare(current1.First, current2.First);
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

        if (_count != other._count || _hashCode != other._hashCode)
            return false;

        var current1 = this;
        var current2 = other;

        // ReSharper disable RedundantCast
        var comparer = EqualityComparer<T>.Default;
        while ((object) current1 != (object) current2
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

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        return Equals(obj as Link<T>);
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    public Link<T> ChangeTail(Link<T> newTail, int fromEnd = 0)
    {
        Debug.Assert(newTail != null);

        var count = _count;
        if (count <= fromEnd)
        {
            Debug.Assert(count == fromEnd);
            return newTail;
        }

        if (fromEnd == 0 && newTail.IsEmpty)
            return this;

        var reversed = Reverse(this, fromEnd);
        return ReverseX(reversed, newTail);
    }

    private static Link<T> Reverse(Link<T> link, int fromEnd = 0)
    {
        Link<T> reversed = null;
        var skip = link.Count - fromEnd;
        for (var current = link; skip > 0; skip--, current = current._rest)
            reversed = new Link<T>(current.First, reversed);
        return reversed ?? Empty;
    }

    public Link<T> Reverse()
    {
        if (_count <= 1)
            return this;
        return Reverse(this);
    }

    public static Link<T> ReverseX(Link<T> link, Link<T> tail = null)
    {
        var current = link ?? Empty;
        var reversed = tail ?? Empty;

        while (current.IsNotEmpty)
        {
            var tmp = current.Rest;
            current.Rest = reversed;
            reversed = current;
            current = tmp;
        }

        return reversed;
    }

    [Pure]
    public Link<T> Add(T element)
    {
        return Append(new Link<T>(element));
    }

    [Pure]
    public Link<T> Append(Link<T> link)
    {
        return ChangeTail(link);
    }

    [Pure]
    public Link<T> Prepend(Link<T> collection)
    {
        return Prepend(this, collection);
    }

    [Pure]
    public Link<T> Replace(int position, int count, params T[] insertData)
    {
        return ReplaceRange(position, count, insertData);
    }

    [Pure]
    public Link<T> ReplaceRange(int position, int count, IEnumerable<T> lisp = null)
    {
        var start = Drop(position);
        var end = start.Drop(count);
        return ChangeTail(Prepend(end, lisp), start.Count);
    }

    [Pure]
    public Link<T> RemoveRange(int position, int count)
    {
        return ReplaceRange(position, count);
    }

    [Pure]
    public Link<T> InsertAt(int position, T element)
    {
        return Replace(position, 0, element);
    }

    [Pure]
    public Link<T> ReplaceAt(int position, T element)
    {
        return Replace(position, 1, element);
    }

    [Pure]
    public static Link<T> Prepend(Link<T> link, IEnumerable<T> collection)
    {
        if (collection == null)
            return link;

        Link<T> current;

        if (link.Count == 0)
        {
            current = collection as Link<T>;
            if (current != null)
                return current;
        }

        current = Empty;
        foreach (var item in collection)
        {
            if (item != null)
                current = new Link<T>(item, current);
        }
        return ReverseX(current, link);
    }

    [Pure]
    public Link<T> Remove(Func<T, bool> func)
    {
        return Convert(x => func(x) ? default(T) : x);
    }

    [Pure]
    public Link<T> Convert(Func<T, T> func)
    {
        Link<T> tally = null;
        Link<T> tail = null;
        Link<T> firstChange = null;
        var comparer = EqualityComparer<T>.Default;
        for (var current = this; current.IsNotEmpty; current = current.Rest)
        {
            var f = func(current.First);
            if (!comparer.Equals(f, current.First))
            {
                if (firstChange == null)
                {
                    firstChange = current;
                }
                else
                {
                    for (; tail != current; tail = tail.Rest)
                        tally = new Link<T>(tail.First, tally);
                }

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
        var locate = Drop(pos);
        if (locate.IsEmpty)
            return this;
        var newFirst = func(locate.First);
        if (EqualityComparer<T>.Default.Equals(newFirst, locate.First))
            return this;
        return ChangeTail(new Link<T>(newFirst, locate.Rest), locate.Count);
    }

    public Link<T> Drop(int count)
    {
        if (count >= _count)
            return Empty;

        var current = this;
        while (count-- > 0 && current.IsNotEmpty)
            current = current.Rest;
        return current;
    }

    [Pure]
    public Link<T> Take(int count)
    {
        return Subrange(0, count);
    }

    [Pure]
    public Link<T> Subrange(int position, int count)
    {
        if (count == 0)
            return Empty;

        var start = Drop(position);
        if (start.IsEmpty)
            return start;
        var end = start.Drop(count);
        return start.ChangeTail(Empty, end.Count);
    }

    public Link<T> Sort(Comparison<T> sortFunc = null)
    {
        return From(this.Sorted(sortFunc));
    }

    public static Link<T> operator |(T head, Link<T> rest)
    {
        return new Link<T>(head, rest);
    }

    public static Link<T> operator +(Link<T> first, Link<T> second)
    {
        return first.Append(second);
    }

    #region Variables

    [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Link<T> _rest;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)] private T _first;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)] private int _count;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)] private int _hashCode;

    #endregion

    #region Constructor

    [DebuggerStepThrough]
    public static Link<T> From(params object[] data)
    {
        return From(Flatten(data));
    }

    [DebuggerStepThrough]
    public static Link<T> From(IEnumerable<T> link)
    {
        return Prepend(Empty, link);
    }

    [DebuggerStepThrough]
    public Link(T first, Link<T> next = null)
    {
        Debug.Assert(first != null);
        _first = first;
        _rest = next ?? Empty;
        Recalculate();
    }

    [DebuggerStepThrough]
    private Link()
    {
        _rest = this;
        Recalculate();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)] public static readonly Link<T> Empty = new Link<T>();

    #endregion

    #region Overrides

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public T First
    {
        get { return _first; }
        set
        {
            _first = value;
            Recalculate();
        }
    }

    [DebuggerStepThrough]
    private void Recalculate()
    {
        var next = _rest;

        int resthash;
        if (next == this)
        {
            _count = 0;
            resthash = unchecked((int) 0xdeadbeef);
        }
        else
        {
            _count = next.Count + 1;
            resthash = next.GetHashCode();
        }

        var firsthash = _first != null ? _first.GetHashCode() : 0;
        _hashCode = HashCode.Combine(firsthash, resthash);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Link<T> Rest
    {
        get { return _rest; }
        set
        {
            _rest = value;
            Recalculate();
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsEmpty
    {
        get { return _count <= 0; }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public bool IsNotEmpty
    {
        get { return _count > 0; }
    }

    #endregion

    #region List Members

    public IEnumerator<T> GetEnumerator()
    {
        for (var current = this; current.IsNotEmpty; current = current.Rest)
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

    public bool Contains(T item)
    {
        return IndexOf(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ListTools.CopyTo(this, array, arrayIndex);
    }

    bool ICollection<T>.Remove(T item)
    {
        throw new InvalidOperationException();
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Count
    {
        get { return _count; }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    bool ICollection<T>.IsReadOnly
    {
        get { return true; }
    }

    public int IndexOf(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        var count = 0;
        for (var current = this; current.IsNotEmpty; current = current.Rest)
        {
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

    public T this[int index]
    {
        get
        {
            var count = 0;
            for (var current = this; current.IsNotEmpty; current = current.Rest)
            {
                if (count == index)
                    return current.First;
                count++;
            }
            return default(T);
        }
        set { throw new InvalidOperationException(); }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public T[] DebugList
    {
        get { return this.ToArray(); }
    }

    #endregion

    #region To String

    public string ToString(string format, IFormatProvider formatProvider = null)
    {
        var indent = false;
        var limit = int.MaxValue;

        if (!string.IsNullOrEmpty(format))
        {
            int newLimit;
            if (format == "I")
                indent = true;
            else if (char.IsDigit(format[0]) && int.TryParse(format, out newLimit))
                limit = newLimit;
        }

        var builder = new StringBuilder();
        ToStringGuts(builder, indent, limit);
        var result = builder.ToString();
        return result;
    }

    public override string ToString()
    {
        return ToString("");
    }

    private void ToStringGuts(StringBuilder builder, bool doIndent = true, int limit = int.MaxValue)
    {
        var indent = 2;
        var oldLength = builder.Length;

        if (doIndent)
            for (var pos = builder.Length - 1; pos > 0; pos--)
            {
                if (builder[pos] == '\n')
                    break;
                indent++;
            }

        builder.Append("(");
        var newline = false;

        var ii = 0;
        for (var node = this; node.IsNotEmpty; node = node.Rest)
        {
            if (builder.Length > limit)
                return;

            if (ii != 0)
            {
                if (newline)
                {
                    builder.AppendLine();
                    builder.Append(' ', indent);
                }
                else
                {
                    builder.Append(' ');
                }
            }

            var nodeData = (object) node.First;
            var listNode = nodeData as Link<T>;
            if (listNode != null)
            {
                listNode.ToStringGuts(builder, doIndent, limit);
                if (doIndent)
                    newline = true;
            }
            else
            {
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
        if (typeof (T) == typeof (object))
            return (IEnumerable<T>) FlattenObject(args);
        return ListTools.Flatten<T>(args);
    }

    [Pure]
    private static IEnumerable<object> FlattenObject(IEnumerable<object> args)
    {
        foreach (var arg in args)
        {
            var en = arg as IEnumerable<object>;
            if (en == null || arg is Link<object>)
            {
                yield return arg;
                continue;
            }

            foreach (var arg2 in en)
                yield return arg2;
        }
    }

    #endregion
}