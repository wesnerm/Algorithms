#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2010-2011, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Text.RegularExpressions;

#endregion

namespace Algorithms.Collections;

[Pure]
public static class Link
{
    #region Constructor

    public static Link<object> Empty => Link<object>.Empty;

    [DebuggerStepThrough]
    public static Link<T> From<T>(IEnumerable<T> link) => Link<T>.From(link);

    [DebuggerStepThrough]
    public static Link<object> New(object first, Link<object> rest = null) => new(first, rest);

    public static Link<object> New(params object[] list)
    {
        var result = (Link<object>)list[list.Length - 1];
        for (int i = list.Length - 2; i >= 0; i--)
            result = New(list[i], result);
        return result;
    }

    [DebuggerStepThrough]
    public static Link<T> ListOf<T>(IEnumerable<T> list) => From(list);

    [DebuggerStepThrough]
    public static Link<T> ListOf<T>(params T[] list) => ListOf((IEnumerable<T>)list);

    [DebuggerStepThrough]
    public static Link<object> List(IEnumerable<object> list) => From(list);

    [DebuggerStepThrough]
    public static Link<object> List(params object[] list) => Link<object>.From(list);

    #endregion

    #region Helper Methods

    public static bool HasElements<T>(this Link<T> link) => link != null && link.Count > 0;

    public static T Hash<T>(T data, LiteSet<object> hash = null)
    {
        if (data == null)
            return default;

        if (hash == null)
            hash = new LiteSet<object>();

        object? find = hash[data];
        if (find != null)
            return (T)find;

        var link = data as Link<object>;
        if (link != null) {
            if (link.Count == 0)
                return data;
            link.First = Hash(link.First, hash);
            link.Rest = Hash(link.Rest, hash);
        }

        hash.Add(data);
        return data;
    }

    public static IEnumerable<Link<object>> Descendents(this Link<object> top)
    {
        if (top == null)
            yield break;

        var stack = new Stack<Link<object>>();
        var list = new Stack<Link<object>>();
        stack.Push(top);

        while (stack.Count != 0) {
            Link<object> pop = stack.Pop();
            yield return pop;

            foreach (object child in pop) {
                var childList = child as Link<object>;
                if (childList != null)
                    list.Push(childList);
            }

            while (list.Count > 0)
                stack.Push(list.Pop());
        }
    }

    public static IEnumerable<object> DescendentsAll(this Link<object> top)
    {
        return top.Descendents().SelectMany(o => o);
    }

    #endregion

    #region Tools

    public static Link<object> Merge(this Link<object> list, Link<object> src, bool checkHeads = true)
    {
        if (list == null)
            return src;
        if (src == null || src.IsEmpty)
            return list;

        bool changed = false;
        List<object> result = list.ToList();

        Debug.Assert(Equals(list[0], src[0]) || !checkHeads, "Heads are not equal");

        for (Link<object> current = src; current.IsNotEmpty; current = current.Rest) {
            object elem = current.First;

            // Test containment
            bool good = result.Any(line => Equals(line, elem));
            if (good)
                continue;

            var elemList = elem as Link<object>;
            if (elemList != null) {
                object head = elemList.First;
                for (int j = 0; j < result.Count; j++) {
                    object elem2 = result[j];
                    var elemList2 = elem2 as Link<object>;
                    if (elemList2 != null && head.Equals(elemList2.First)) {
                        Link<object> merge = elemList2.Merge(elemList);
                        if (merge != result[j])
                            changed = true;
                        result[j] = merge;
                        good = true;
                        break;
                    }
                }

                if (good)
                    continue;
            }

            result.Add(elem);
            changed = true;
        }

        if (changed)
            return List(result);

        return list;
    }

    public static object Recurse(object lisp,
        Func<object, object> code, bool all = false)
    {
        Func<object, object> func = null;
        func = e => {
            object elem = e;

            var elemList = elem as Link<object>;
            if (elemList != null)
                elem = elemList.Convert(func);

            elemList = elem as Link<object>;
            if (!all && elemList == null)
                return elem;

            return code(elem);
        };
        return func(lisp);
    }

    public static void Recurse(object lisp, Action<object> code, bool all = false)
    {
        Recurse(lisp, x => {
            code(x);
            return x;
        }, all);
    }

    public static Link<object> RemoveList(Link<object> lisp, object item)
    {
        return (Link<object>)Recurse(lisp, line => {
            var lineList = line as Link<object>;
            if (lineList != null && item.Equals(lineList.First))
                return null;
            return line;
        });
    }

    public static object Index(this Link<object> list, params int[] indices)
    {
        object current = list;
        for (int i = 0; i < indices.Length; i++) {
            var link = current as Link<object>;
            if (link == null)
                return null;
            current = link[indices[i]];
        }

        return current;
    }

    [DebuggerStepThrough]
    public static bool IsNullOrEmpty<T>(this Link<T> link) => link == null || link.IsEmpty;

    #endregion

    #region File IO

    public static Link<object> GetList(string file = null)
    {
        Link<object> list = ReadList(file);
        if (list.Count == 1) {
            var link = list.First as Link<object>;
            if (link != null)
                return link;
        }

        return list;
    }

    public static Link<object> ReadList(string file = null)
    {
        TextReader reader = file == null ? Console.In : new StreamReader(file, Encoding.UTF8);
        using (reader) return ReadElements(reader);
    }

    public static Link<object> Parse(string data) => ReadElements(new StringReader(data));

    static Link<object> ReadElements(TextReader newReader) => ReadElements(ToTokens(newReader));

    static Link<object> ReadElements(IEnumerator<string> en)
    {
        var link = Link<object>.Empty;

        while (en.MoveNext()) {
            object element;
            string s = en.Current;
            if (s == "(")
                element = ReadElements(en);
            else if (s == ")")
                break;
            else
                element = s;
            Debug.Assert(element != null);
            link = new Link<object>(element, link);
        }

        link = Link<object>.ReverseX(link);
        return link;
    }

    public static IEnumerator<string> ToTokens(TextReader reader)
    {
        while (true) {
            string? line = reader.ReadLine();
            if (line == null)
                break;

            foreach (Match t in
                     Regex.Matches(line, @"[()]|;.*|""[^""]*""|[^\s()"";]+")) {
                string? token = t.Value;
                if (token[0] != ';' && token[0] != ',') {
                    Debug.Assert(token != null);
                    yield return token;
                }
            }
        }
    }

    public static Link<object> Sorted(this Link<object> link) => Link<object>.From(ListTools.Sorted(link));

    public static int Compare(object obj1, object obj2)
    {
        if (Equals(obj1, obj2))
            return 0;
        if (obj1 == null)
            return -1;
        if (obj2 == null)
            return 1;

        Type type = obj1.GetType();
        Type type2 = obj2.GetType();
        if (type == type2) {
            var icomparer = obj1 as IComparable;
            if (icomparer != null)
                return icomparer.CompareTo(obj2);
        }

        Type linkobj = typeof(Link<object>);
        if (type == linkobj)
            return 1;
        if (type2 == linkobj)
            return -1;

        return string.Compare(type.Name, type2.Name, StringComparison.Ordinal);
    }

    #endregion
}