#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

namespace Algorithms.Collections;

public static class SetTools
{
    public static IEnumerable<T> Union<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
        var set = new HashSet<T>();

        foreach (T tmp in first) {
            if (set.Contains(tmp))
                continue;
            set.Add(tmp);
            yield return tmp;
        }

        foreach (T tmp in second) {
            if (set.Contains(tmp))
                continue;
            set.Add(tmp);
            yield return tmp;
        }
    }

    public static IEnumerable<T> Intersection<T>(IEnumerable<T> first, IEnumerable<T> second)
        where T : class
    {
        var set = new HashSet<T>();

        foreach (T tmp in second)
            set.Add(tmp);

        foreach (T tmp in first)
            if (set.Contains(tmp)) {
                set.Remove(tmp);
                yield return tmp;
            }
    }

    [DebuggerStepThrough]
    public static bool Intersects<T>(this IEnumerable<T> first, IList<T> second) => first.Any(second.Contains);

    public static IEnumerable<T> Difference<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
        var set = new HashSet<T>();

        foreach (T tmp in second)
            set.Add(tmp);

        return first.Where(tmp => !set.Contains(tmp));
    }

    [DebuggerStepThrough]
    public static IEnumerable<T> Union<T>(this IEnumerable<IEnumerable<T>> listOfLists) =>
        listOfLists.Aggregate(Enumerable.Union);

    [DebuggerStepThrough]
    public static IEnumerable<T> Intersection<T>(this IEnumerable<IEnumerable<T>> listOfLists) =>
        listOfLists.Aggregate(Enumerable.Intersect);

    [DebuggerStepThrough]
    public static HashSet<T> ToSet<T>(this IEnumerable<T> collection) => new(collection);

    [DebuggerStepThrough]
    public static HashSet<U> ToSet<T, U>(this IEnumerable<T> collection, Func<T, U> func)
    {
        var newSet = new HashSet<U>();
        foreach (T item in collection) {
            U newItem = func(item);
            if (newItem != null)
                newSet.Add(newItem);
        }

        return newSet;
    }

    [DebuggerStepThrough]
    public static HashSet<T> Clone<T>(this HashSet<T> collection) => new(collection);
}