#region Copyright

//  This source code may not be reviewed, copied, or redistributed without
//  the expressed permission of Wesner Moise.
//  
//  File: ListTools.cs
//  Created: 05/31/2012 
//  Modified: 09/26/2012
// 
//  Copyright (C) 2012 - 2012, Wesner Moise.

#endregion

#region Usings

using System.Collections;

#endregion

namespace Algorithms.Collections;

//[DebuggerStepThrough]
public static class ListTools
{
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> data)
    {
        foreach (var d in data)
            list.Add(d);
    }

    [DebuggerStepThrough]
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var v in enumerable)
            action(v);
    }

    /// <summary>
    ///     Performs a left rotation on a range of elements.
    ///     Specifically, rotate swaps the elements in the range[start, start+count) in such a way that the element
    ///     n_first becomes the first element of the new range and n_first - 1 becomes the last element.
    ///     A precondition of this function is that[start, newStart) and[newStart, start+count) are valid ranges.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="start">the beginning of the original range</param>
    /// <param name="count">the element that should appear at the beginning of the rotated range</param>
    /// <param name="newStart">the end of the original range</param>
    public static int Rotate<T>(this IList<T> list, int start, int count, int newStart)
    {
        var first = start;
        var newFirst = newStart;
        var end = start + count;
        var next = newFirst;
        T tmp;

        if (2*(newStart - start) == count)
        {
            // Eliminate branches for fast path
            for (var i = newStart - start; i >= 0; i--)
            {
                tmp = list[first];
                list[first++] = list[next];
                list[next++] = tmp;
            }
        }
        else
        {
            while (first != next)
            {
                tmp = list[first];
                list[first++] = list[next];
                list[next++] = tmp;

                if (next == end)
                {
                    next = newFirst;
                }
                else if (first == newFirst)
                {
                    newFirst = next;
                }
            }
        }

        var result = start + (end - newStart);
        return result;
    }

    public static int Rotate2<T>(this IList<T> list, int start, int count, int newStart)
    {
        var end = start + count;
        var result = start + (end - newStart);

        if (start == newStart)
            return result;

        if (2*(newStart - start) == count)
        {
            var p0 = start;
            var p1 = newStart;
            for (var i = newStart - start; i >= 0; i--)
            {
                var tmp = list[p0];
                list[p0++] = list[p1];
                list[p1++] = tmp;
            }
        }
        else
        {
            list.Reverse(start, newStart - start);
            list.Reverse(newStart, end - newStart);
            list.Reverse(start, count);
        }

        return result;
    }

    public static void Reverse<T>(this IList<T> list, int start, int count)
    {
        var first = start;
        var last = start + count - 1;

        while (first < last)
        {
            var tmp = list[first];
            list[first++] = list[last];
            list[last--] = tmp;
        }
    }

    private static int PositiveMod(int number, int modulus)
    {
        var result = number%modulus;
        return result >= 0 ? result : result + modulus;

        // Alternative: ((n%m) + m)%m
    }

    public static void Swap<T>(this IList<T> list, int index1, int index2)
    {
        var tmp = list[index1];
        list[index1] = list[index2];
        list[index2] = tmp;
    }

    [DebuggerStepThrough]
    [Pure]
    public static T ArgMin<T, V>(this IEnumerable<T> enumerable,
        Func<T, V> func,
        Comparison<V> compare = null)
    {
        if (compare == null)
            compare = Comparer<V>.Default.Compare;

        Debug.Assert(func != null);
        var inited = false;
        var result = default(T);
        var best = default(V);

        foreach (var e in enumerable)
        {
            var check = func(e);
            if (inited)
            {
                if (compare(check, best) < 0)
                {
                    best = check;
                    result = e;
                }
            }
            else
            {
                result = e;
                best = check;
                inited = true;
            }
        }
        return result;
    }

    public static void StableSort<T>(this IList<T> list, Comparison<T> comparison)
    {
        var count = list.Count;
        if (count < 2)
            return;

        var indices = new int[count];
        for (var k = count - 1; k >= 0; k--)
            indices[k] = k;

        Array.Sort(indices, (a, b) =>
        {
            var cmp = comparison(list[a], list[b]);
            return cmp != 0 ? cmp : a.CompareTo(b);
        });

        // Rotation
        for (var i = count - 1; i >= 0; i--)
        {
            // fast path: true for at least half of all elements in the list
            if (indices[i] == i) continue;

            int j, k;
            var swap = list[i];
            for (j = i; (k = indices[j]) != i; j = k)
            {
                Debug.Assert(j != k);
                list[j] = list[k];
                indices[j] = j;
            }
            list[j] = swap;
        }
    }

    public static void StableSort2<T>(this T[] list, Comparison<T> comparison)
    {
        var count = list.Length;
        var array = new T[count];
        list.CopyTo(array, 0);

        var indices = new int[count];
        for (var k = count - 1; k >= 0; k--)
            indices[k] = k;

        ((Span<int>) indices).Sort((Span<T>)array,
            (int a, int b) =>
            {
                var cmp = comparison(list[a], list[b]);
                return cmp != 0 ? cmp : a.CompareTo(b);
            });
    }

    [Pure]
    [return: NotNull]
    public static List<T> Sorted<T>(this IEnumerable<T> enumerable, Comparison<T> compare = null)
    {
        var list = enumerable.ToList();
        if (compare != null)
            list.Sort(compare);
        else
            list.Sort();
        return list;
    }

    [Pure]
    [return: NotNull]
    public static List<T> Sorted<T>(this IEnumerable<T> enumerable, IComparer<T> compare)
    {
        var list = enumerable.ToList();
        if (compare != null)
            list.Sort(compare);
        else
            list.Sort();
        return list;
    }

    [Pure]
    [return: NotNull]
    public static List<T> Sorted<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> compare)
    {
        var list = enumerable.ToList();
        list.SortBy(compare);
        return list;
    }

    public static void SortBy<T, TKey>(this List<T> list, Func<T, TKey> keyFunc)
    {
        var comparer = Comparer<TKey>.Default;
        list.Sort(((a,b)=>comparer.Compare(keyFunc(a), keyFunc(b))));
    }

    public static IEnumerable<T[]> Partition<T>(this IEnumerable<T> list, int count)
    {
        if (count <= 0)
            throw new InvalidOperationException();

        var array = new T[count];

        var i = 0;
        var en = list.GetEnumerator();
        while (true)
        {
            if (i == count)
            {
                yield return array;
                i = 0;
            }

            if (!en.MoveNext())
            {
                if (i != 0)
                    yield return array.CopyRange(0, i);
                yield break;
            }

            array[i++] = en.Current;
        }
    }

    public class LazyCollection<T> : ICollection<T>
    {
        #region Constructor

        public LazyCollection(IEnumerable<T> e)
        {
            _enumerable = e;
            _count = -1;
        }

        #endregion

        #region Variable

        private readonly IEnumerable<T> _enumerable;
        private int _count;

        #endregion

        #region Misc

        public IEnumerator<T> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(T item)
        {
            return _enumerable.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _enumerable.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException();
        }

        public int Count
        {
            get
            {
                if (_count < 0)
                    _count = _enumerable.Count();
                return _count;
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        #endregion
    }

    #region List Wrapper

    [DebuggerStepThrough]
    [System.Diagnostics.Contracts.Pure]
    public static List<T> Clone<T>(this List<T> list)
    {
        return list == null ? null : new List<T>(list);
    }

    [System.Diagnostics.Contracts.Pure]
    [return: NotNull]
    [DebuggerHidden]
    public static List<T> Map<T>([NotNull] this List<T> list, Func<T, T> func,
        IEqualityComparer<T> comparer = null)
    {
        if (list == null)
            return null;

        if (comparer == null)
            comparer = EqualityComparer<T>.Default;
        var count = list.Count;
        for (var i = 0; i < count; i++)
        {
            var elem = list[i];
            var newElem = func(list[i]);
            if (!comparer.Equals(elem, newElem))
            {
                var newArray = list.Clone();
                newArray[i] = newElem;
                for (i++; i < count; i++)
                    newArray[i] = func(newArray[i]);
                return newArray;
            }
        }
        return list;
    }

    [Pure]
    public static ICollection<T> ToCollection<T>(IEnumerable<T> collection)
    {
        var col = collection as ICollection<T>;
        if (col != null)
            return col;
        return new LazyCollection<T>(collection);
    }

    [Pure]
    public static ICollection<T> ForceCollection<T>(this IEnumerable<T> enumerable)
    {
        var collection = enumerable as ICollection<T>;
        if (collection != null)
            return collection;
        return enumerable.ToList();
    }

    [Pure]
    public static IList<T> ForceList<T>(this IEnumerable<T> enumerable)
    {
        var collection = enumerable as IList<T>;
        if (collection != null)
            return collection;
        return enumerable.ToList();
    }

    [Pure]
    [return: NotNull]
    [DebuggerStepThrough]
    public static IList<T> Empty<T>()
    {
        return (IList<T>) Array.Empty<T>();
    }

    [DebuggerStepThrough]
    public static IEnumerable<int> Range(int start, int end, int by = 1)
    {
        if (by > 0)
            for (var i = start; i <= end; i += by)
                yield return i;
        else
            for (var i = start; i >= end; i += by)
                yield return i;
    }

#line hidden
    public static IEnumerable<T> Singleton<T>(T singleton)
    {
        yield return singleton;
    }
#line default

    [DebuggerStepThrough]
    [Pure]
    [return: NotNull]
    public static IEnumerable<T> SingletonOrEmpty<T>(T singleton)
    {
        if (singleton == null)
            return Empty<T>();
        return Singleton(singleton);
    }

    public static void CopyTo<T>(this IEnumerable<T> list, T[] array, int position)
    {
        foreach (var item in list)
            array[position++] = item;
    }

#line default

    [DebuggerStepThrough]
    public static T RandomElement<T>(this IEnumerable<T> enumerable)
    {
        var random = new Random();
// ReSharper disable PossibleMultipleEnumeration
        var count = enumerable.Count();
// ReSharper restore PossibleMultipleEnumeration
        var index = random.Next(0, count - 1);
        return enumerable.ElementAt(index);
    }

    #endregion

    #region Composite

    [DebuggerHidden]
    [Pure]
    public static Func<T, IEnumerable<T>> IdentityEnumerable<T>() where T : IEnumerable<T>
    {
        return element => element;
    }

    [DebuggerStepThrough]
    [Pure]
    public static IEnumerable<T> GetDescendants<T>(this T element, bool dfs = true)
        where T : IEnumerable<T>
    {
        var id = IdentityEnumerable<T>();
        return dfs ? Search(element, id) : SearchBreadth(element, id);
    }

    [DebuggerStepThrough]
    [Pure]
    public static IEnumerable<T> Search<T>(IEnumerable<T> element, Func<T, IEnumerable<T>> enumerator)
    {
#line hidden
        var stack = new Stack<IEnumerator<T>>();

        try
        {
            stack.Push(element.GetEnumerator());
            while (stack.Count > 0)
            {
                var peek = stack.Peek();
                if (!peek.MoveNext())
                {
                    peek.Dispose();
                    stack.Pop();
                    continue;
                }

                var current = peek.Current;
                yield return current;
                stack.Push(enumerator(current).GetEnumerator());
            }
        }
        finally
        {
            while (stack.Count > 0)
                stack.Pop().Dispose();
        }
#line default
    }

    [Pure]
    public static IEnumerable<T> SearchBreadth<T>(T element, Func<T, IEnumerable<T>> enumerator)
    {
        var queue = new Queue<T>();
        while (true)
        {
            yield return element;
            foreach (var child in enumerator(element))
                queue.Enqueue(child);
            if (queue.Count == 0)
                break;
            element = queue.Dequeue();
        }
    }

    [Pure]
    public static IEnumerable<T> Subrange<T>(this IEnumerable<T> collection, int from, int to)
    {
        if (from > to)
            return Empty<T>();

        var list = collection as IList<T>;
        return list != null 
            ? Subrange(list, @from, to) 
            : SubrangeSlow(collection, @from, to);
    }

    public static IEnumerable<T> Subrange<T>(this IList<T> list, int from, int to)
    {
        for (var i = from; i <= to; i++)
            yield return list[i];
    }

    private static IEnumerable<T> SubrangeSlow<T>(this IEnumerable<T> collection, int from, int to)
    {
        var i = 0;
        foreach (var e in collection)
        {
            if (i >= from)
            {
                if (i > to)
                    yield break;
                yield return e;
            }
            i++;
        }
    }

    #endregion

    #region Copying

    public static void Copy<T>(this IList<T> source, int sourcePos, IList<T> dest, int destPos, int count)
    {
        if (source != dest || destPos <= sourcePos)
        {
            while (count-- > 0)
                dest[destPos++] = source[sourcePos++];
        }
        else
        {
            while (count-- > 0)
                dest[destPos + count] = source[sourcePos + count];
        }
    }

    [Pure]
    public static T[] CopyRange<T>(this IList<T> list, int position, int count)
    {
        NormalizeRange(list, ref position, ref count);

        if (count == 0)
            return Array.Empty<T>();

        var result = new T[count];
        if (list is T[] array)
            Array.Copy(array, position, result, 0, count);
        else
            for (var i = 0; i < result.Length; i++)
                result[i] = list[position + i];

        return result;
    }

    [Pure]
    public static IEnumerable<T> Flatten<T>(IEnumerable<object> args)
    {
        foreach (var arg in args)
        {
            if (arg is T)
            {
                yield return (T) arg;
                continue;
            }

            var en = arg as IEnumerable<T>
                     ?? Flatten<T>(((IEnumerable) arg).Cast<object>());

            foreach (var arg2 in en)
                yield return arg2;
        }
    }

    #endregion

    #region ArrayList Operations

    [DebuggerStepThrough]
    public static void Replace<T>(this List<T> list, int position, int count, IEnumerable<T> insert = null)
    {
        NormalizeRange(list, ref position, ref count);
        list.RemoveRange(position, count);
        if (insert != null)
            list.InsertRange(position, insert);
    }

    [DebuggerStepThrough]
    public static List<T> Cut<T>(this List<T> list, int position, int count)
    {
        NormalizeRange(list, ref position, ref count);
        var result = list.GetRange(position, count);
        list.Replace(position, count);
        return result;
    }

    [DebuggerStepThrough]
    public static T[] Repeat<T>(T element, int count)
    {
        if (count == 0) return Array.Empty<T>();
        var array = new T[count];
        for (var i = 0; i < array.Length; i++)
            array[i] = element;
        return array;
    }

    public static void Ensure<T>(this IList<T> list, int n)
    {
        for (var i = n - list.Count; i >= 0; i--)
            list.Add(default(T));
    }

    public static T EnsureNew<T>(this List<T> list, int n, bool all = false) where T : new()
    {
        for (var i = n - list.Count; i >= 0; i--)
            list.Add(all ? new T() : default(T));
        var result = list[n];
// ReSharper disable CompareNonConstrainedGenericWithNull
        if (result == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
            list[n] = result = new T();
        return result;
    }

    public static void NormalizeRange<T>(IList<T> list, ref int position, ref int count)
    {
        if (list == null)
        {
            position = 0;
            count = 0;
            return;
        }

        if (position < 0)
            position += list.Count;

        if (position < 0 || count < 0 || position + count > list.Count)
            throw new ArgumentOutOfRangeException();
    }

    #endregion

    #region Comparison

    public static bool AllSame<T, U>(this IEnumerable<T> list, [NotNull] Func<T, U> func)
    {
        using (var en = list.GetEnumerator())
        {
            if (!en.MoveNext())
                return true;

            var first = func(en.Current);
            var comparer = EqualityComparer<U>.Default;

            while (en.MoveNext())
            {
                if (!comparer.Equals(first, func(en.Current)))
                    return false;
            }
            return true;
        }
    }

    public static bool AllSame<T>(this IEnumerable<T> list)
    {
        return AllSame(list, x=>x);
    }

    public static bool ElementsIdentical<T>(T[] list1, T[] list2) where T : class
    {
        if (list1 == list2)
            return true;

        if (list1 == null || list2 == null)
            return false;

        if (list1.Length != list2.Length)
            return false;

// ReSharper disable LoopCanBeConvertedToQuery
        for (var i = 0; i < list1.Length; i++)
// ReSharper restore LoopCanBeConvertedToQuery
            if (list1[i] != list2[i])
                return false;

        return true;
    }

    #endregion

    #region Arrays

    [DebuggerStepThrough]
    public static void Push<T>(ref List<T> list, T element)
    {
        if (list == null)
            list = new List<T>();
        list.Add(element);
    }

    public static void Trim<T>(this List<T> list, int size)
    {
        if (list == null)
            return;

        var diff = list.Count - size;
        if (diff > 0)
            list.RemoveRange(size, diff);
    }

    [DebuggerStepThrough]
    public static T Pop<T>(this IList<T> list)
    {
        var count = list.Count;
        if (count == 0)
            return default(T);

        count--;
        var result = list[count];
        list.RemoveAt(count);
        return result;
    }

    [DebuggerStepThrough]
    public static T PopFront<T>(this IList<T> list)
    {
        var count = list.Count;
        if (count == 0)
            return default(T);

        var result = list[0];
        list.RemoveAt(0);
        return result;
    }

    [DebuggerStepThrough]
    public static void PushFront<T>(this IList<T> list, T item)
    {
        list.Insert(0, item);
    }

    [DebuggerStepThrough]
    [Pure]
    public static bool IsOrdered<T>(this T[] list) where T : IComparable<T>
    {
        for (var i = 1; i < list.Length; i++)
            if (list[i - 1].CompareTo(list[i]) > 0)
                return false;
        return true;
    }

    [DebuggerStepThrough]
    [Pure]
    public static bool IsNullOrEmpty<T>(this ICollection<T> list)
    {
        return list == null || list.Count == 0;
    }

    #endregion
}