#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

#endregion

using System.Reflection;

namespace Algorithms.Collections;

public static class DictionaryTools
{
    [DebuggerStepThrough]
    public static Dictionary<K, V> Clone<K, V>(IDictionary<K, V> dict) => new(dict);

    [DebuggerNonUserCode]
    public static Dictionary<V, K> Invert<K, V>(this IDictionary<K, V> dict)
    {
        var reverse = new Dictionary<V, K>(dict.Count);
        foreach (KeyValuePair<K, V> pair in dict)
            reverse.Add(pair.Value, pair.Key);
        return reverse;
    }

    public static Dictionary<V, List<K>> Invert<K, V>(this Dictionary<K, List<V>> dict)
    {
        var dict2 = new Dictionary<V, List<K>>();
        foreach (KeyValuePair<K, List<V>> pair in dict)
        foreach (V value in pair.Value)
            dict2.AddMany(value, pair.Key);
        return dict2;
    }

    [DebuggerNonUserCode]
    public static Dictionary<V, List<K>> InvertMany<K, V>(this Dictionary<K, V> dict)
    {
        var dict2 = new Dictionary<V, List<K>>();
        foreach (KeyValuePair<K, V> pair in dict)
            dict2.AddMany(pair.Value, pair.Key);
        return dict2;
    }

    [DebuggerNonUserCode]
    public static Dictionary<K, V> ConstructMap<K, V>(K[] keys, V[] values)
    {
        if (keys == null || values == null)
            return null;

        var dict = new Dictionary<K, V>(keys.Length);
        for (int i = 0; i < keys.Length; i++) {
            if (i >= values.Length)
                break;
            dict[keys[i]] = values[i];
        }

        return dict;
    }

    [DebuggerNonUserCode]
    public static bool Equivalent<K, V>(IDictionary<K, V> dict1,
        IDictionary<K, V> dict2,
        IEqualityComparer<V> comparer = null)
    {
        if (dict1 == dict2)
            return true;

        if (dict1 == null || dict2 == null
                          || dict1.Count != dict2.Count)
            return false;

        comparer = comparer ?? EqualityComparer<V>.Default;
        foreach (KeyValuePair<K, V> pair in dict1) {
            V value;
            if (!dict2.TryGetValue(pair.Key, out value)
                || !comparer.Equals(pair.Value, value))
                return false;
        }

        return true;
    }

    [DebuggerNonUserCode]
    public static int GetHashCode<K, V>(IDictionary<K, V> dict1)
    {
        int hashcode = 0x1D1C4103;

        if (dict1 != null) {
            hashcode ^= dict1.Count;
            foreach (KeyValuePair<K, V> pair in dict1)
                hashcode ^= GetHashCode(pair);
            if (hashcode == 0)
                return 0x1D1C4104;
        }

        return hashcode;
    }

    public static int GetHashCode<K, V>(KeyValuePair<K, V> pair)
    {
        int hashcode = pair.Key.GetHashCode();
        if (pair.Value != null)
            hashcode ^= pair.Value.GetHashCode();
        return hashcode;
    }

    public static bool IsSubsetOf<K, V>(IDictionary<K, V> dict1, IDictionary<K, V> dict2)
    {
        if (dict1 == dict2)
            return true;

        if (dict1 == null || dict2 == null
                          || dict1.Count > dict2.Count)
            return false;

        var comparer = EqualityComparer<V>.Default;
        foreach (KeyValuePair<K, V> pair in dict1) {
            V value;
            if (!dict2.TryGetValue(pair.Key, out value)
                || !comparer.Equals(pair.Value, value))
                return false;
        }

        return true;
    }

    //public static V GetValue<K, V>(this Dictionary<K, V> dict, K key)
    //{
    //    V value;
    //    dict.TryGetValue(key, out value);
    //    return value;
    //}

    public static V Get<K, V>(this Dictionary<K, V> dict, K key)
    {
        V value;
        if (dict != null && dict.TryGetValue(key, out value))
            return value;
        return default;
    }

    public static V GetOr<K, V>(this Dictionary<K, V> dict,
        K key,
        V defaultValue = default)
    {
        V value;
        if (dict != null && dict.TryGetValue(key, out value))
            return value;
        return defaultValue;
    }

    public static V Get<K, K2, V>(this Dictionary<K, Dictionary<K2, V>> dict,
        K key,
        K2 key2)
    {
        Dictionary<K2, V> dict2;
        if (dict == null || !dict.TryGetValue(key, out dict2))
            return default;
        return Get(dict2, key2);
    }

    public static V Get<K, K2, K3, V>(this Dictionary<K, Dictionary<K2, Dictionary<K3, V>>> dict,
        K key, K2 key2, K3 key3)
    {
        Dictionary<K2, Dictionary<K3, V>> dict2;
        if (dict == null || !dict.TryGetValue(key, out dict2))
            return default;
        return Get(dict2, key2, key3);
    }

    public static V Get<K, K2, K3, K4, V>(this Dictionary<K, Dictionary<K2, Dictionary<K3, Dictionary<K4, V>>>> dict,
        K key, K2 key2, K3 key3, K4 key4)
    {
        Dictionary<K2, Dictionary<K3, Dictionary<K4, V>>> dict2;
        if (dict == null || !dict.TryGetValue(key, out dict2))
            return default;
        return Get(dict2, key2, key3, key4);
    }

    public static bool Remove<K, K2, V>(this Dictionary<K, Dictionary<K2, V>> dict,
        K key,
        K2 key2)
    {
        Dictionary<K2, V> dict2;
        if (dict == null || !dict.TryGetValue(key, out dict2))
            return false;
        return dict2.Remove(key2);
    }

    public static bool Remove<K, K2, K3, V>(this Dictionary<K, Dictionary<K2, Dictionary<K3, V>>> dict,
        K key, K2 key2, K3 key3)
    {
        Dictionary<K2, Dictionary<K3, V>> dict2;
        if (dict == null || !dict.TryGetValue(key, out dict2))
            return false;
        return Remove(dict2, key2, key3);
    }

    public static bool Remove<K, K2, K3, K4, V>(
        this Dictionary<K, Dictionary<K2, Dictionary<K3, Dictionary<K4, V>>>> dict,
        K key, K2 key2, K3 key3, K4 key4)
    {
        Dictionary<K2, Dictionary<K3, Dictionary<K4, V>>> dict2;
        if (dict == null || !dict.TryGetValue(key, out dict2))
            return false;
        return Remove(dict2, key2, key3, key4);
    }

    public static V? GetValueNull<K, V>(this Dictionary<K, V> dict, K key)
        where V : struct
    {
        V value;
        if (dict != null && dict.TryGetValue(key, out value))
            return value;
        return null;
    }

    public static V Get<K, V>(this Dictionary<K, V> dict, K key, Func<V> func)
    {
        V value;
        if (dict != null && dict.TryGetValue(key, out value))
            return value;
        return func();
    }

    public static V Get<K, V>(this Dictionary<K, V> dict, K key, Func<K, V> func)
    {
        V value;
        if (dict != null && dict.TryGetValue(key, out value))
            return value;
        return func(key);
    }

    public static void Set<K, V>(this Dictionary<K, V> dict, K key, V value)
    {
        if (value == null)
            dict.Remove(key);
        else
            dict[key] = value;
    }

    public static void Set<K, K2, V>(this Dictionary<K, Dictionary<K2, V>> dict,
        K key, K2 key2, V value)
    {
        if (value == null)
            dict.Remove(key, key2);
        else
            dict.Force(key)[key2] = value;
    }

    public static void Set<K, K2, K3, V>(this Dictionary<K, Dictionary<K2, Dictionary<K3, V>>> dict,
        K key, K2 key2, K3 key3, V value)
    {
        if (value == null)
            dict.Remove(key, key2, key3);
        else
            dict.Force(key, key2)[key3] = value;
    }

    public static void Set<K, K2, K3, K4, V>(this Dictionary<K, Dictionary<K2, Dictionary<K3, Dictionary<K4, V>>>> dict,
        K key, K2 key2, K3 key3, K4 key4, V value)
    {
        if (value == null)
            dict.Remove(key, key2, key3, key4);
        else
            dict.Force(key, key2, key3)[key4] = value;
    }

    public static void Set<K, V>(this Dictionary<K, V> dict, K key, Func<V, V> func)
    {
        V v = func(Get(dict, key));
        Set(dict, key, v);
    }

    public static void Set<K, K2, V>(this Dictionary<K, Dictionary<K2, V>> dict,
        K key, K2 key2, Func<V, V> func)
    {
        V v = func(Get(dict, key, key2));
        Set(dict, key, key2, v);
    }

    public static void Set<K, K2, K3, V>(this Dictionary<K, Dictionary<K2, Dictionary<K3, V>>> dict,
        K key, K2 key2, K3 key3, Func<V, V> func)
    {
        V v = func(Get(dict, key, key2, key3));
        Set(dict, key, key2, key3, v);
    }

    public static void Set<K, K2, K3, K4, V>(this Dictionary<K, Dictionary<K2, Dictionary<K3, Dictionary<K4, V>>>> dict,
        K key, K2 key2, K3 key3, K4 key4, Func<V, V> func)
    {
        V v = func(Get(dict, key, key2, key3, key4));
        Set(dict, key, key2, key3, key4, v);
    }

    [DebuggerStepThrough]
    public static Dictionary<K, V> New<K, V>() => new();

    [DebuggerStepThrough]
    public static Dictionary<K, Dictionary<K2, V>> New<K, K2, V>() => new();

    [DebuggerStepThrough]
    public static Dictionary<K, Dictionary<K2, Dictionary<K3, V>>> New<K, K2, K3, V>() => new();

    [DebuggerStepThrough]
    public static Dictionary<K, Dictionary<K2, Dictionary<K3, Dictionary<K4, V>>>> New<K, K2, K3, K4, V>() => new();

    public static T GetInterned<T>(Dictionary<T, T> dict, T key)
    {
        T value;
        if (dict.TryGetValue(key, out value))
            return value;
        dict[key] = key;
        return key;
    }

    [DebuggerNonUserCode]
    public static bool SameKeys<K, V>(IDictionary<K, V> dict1, IDictionary<K, V> dict2)
    {
        if (dict1 == dict2)
            return true;

        if (dict1 == null || dict2 == null
                          || dict1.Count != dict2.Count)
            return false;

        foreach (K k in dict1.Keys)
            if (!dict2.ContainsKey(k))
                return false;

        return true;
    }

    public static int GetHashCode<K, V>(IDictionary<K, V> dictionary, bool checkValues)
    {
        int hashcode = 0;
        foreach (KeyValuePair<K, V> pair in dictionary) {
            int tmp = pair.Key.GetHashCode();
            if (checkValues && pair.Value != null)
                tmp = HashCode.Combine(tmp, pair.Value.GetHashCode());
            hashcode ^= tmp;
        }

        return hashcode;
    }

    public static IEnumerable<K> SetIntersection<K>(ICollection<K> set1, ICollection<K> set2)
    {
        if (set1 == null || set2 == null)
            yield break;

        foreach (K key in set1)
            if (set2.Contains(key))
                yield return key;
    }

    public static IEnumerable<K> SetUnion<K>(ICollection<K> set1, ICollection<K> set2)
    {
        foreach (K key in set1)
            yield return key;

        foreach (K key in set2)
            if (!set1.Contains(key))
                yield return key;
    }

    public static IEnumerable<K> SetExclusiveUnion<K>(ICollection<K> set1, ICollection<K> set2)
    {
        foreach (K key in set1)
            if (!set2.Contains(key))
                yield return key;

        foreach (K key in set2)
            if (!set1.Contains(key))
                yield return key;
    }

    public static IEnumerable<K> SetDifference<K>(ICollection<K> set1, ICollection<K> set2)
    {
        foreach (K key in set1)
            if (!set2.Contains(key))
                yield return key;
    }

    public static IEnumerable<T> UnionCollection<T>(ICollection<T> col1, ICollection<T> col2)
    {
        foreach (T t in col1)
            yield return t;
        foreach (T t in col2)
            if (!col1.Contains(t))
                yield return t;
    }

    public static void AddMany<T1, T2, T3>(this Dictionary<T1, T2> dict, T1 key, T3 value)
        where T2 : ICollection<T3>, new()
    {
        T2 set;
        if (!dict.TryGetValue(key, out set)) {
            set = new T2();
            dict[key] = set;
        }

        set.Add(value);
    }

    public static bool RemoveMany<T1, T2, T3>(this Dictionary<T1, T2> dict, T1 key, T3 value)
        where T2 : ICollection<T3>
    {
        T2 set;
        if (!dict.TryGetValue(key, out set))
            return false;

        bool result = set.Remove(value);
        if (set.Count == 0)
            dict.Remove(key);
        return result;
    }

    public static int Increment<T>(this Dictionary<T, int> dict, T key, int value = 1)
    {
        int result = dict.Get(key) + value;
        dict[key] = result;
        return result;
    }

    public static int Increment<T, T2>(this Dictionary<T, Dictionary<T2, int>> dict, T key, T2 key2, int value = 1) =>
        dict.Force(key).Increment(key2, value);

    public static int Increment<T, T2, T3>(this Dictionary<T, Dictionary<T2, Dictionary<T3, int>>> dict, T key, T2 key2,
        T3 key3, int value = 1) =>
        dict.Force(key).Force(key2).Increment(key3, value);

    public static int Decrement<T>(this Dictionary<T, int> dict, T key) => Increment(dict, key, -1);

    public static V Force<K, V>(this Dictionary<K, V> dict, K key)
        where V : new()
    {
        V value;
        if (dict.TryGetValue(key, out value))
            return value;

        var result = new V();
        dict.Add(key, result);
        return result;
    }

    public static V Force<T1, T2, V>(this Dictionary<T1, Dictionary<T2, V>> dict, T1 key, T2 key2)
        where V : new() =>
        Force(Force(dict, key), key2);

    public static V Force<T1, T2, T3, V>(this Dictionary<T1, Dictionary<T2, Dictionary<T3, V>>> dict, T1 key, T2 key2,
        T3 key3)
        where V : new() =>
        Force(Force(Force(dict, key), key2), key3);

    public static bool Remove2<T1, TOut>(this Dictionary<T1, TOut> dict, T1 key)
        where TOut : new()
    {
        if (dict == null)
            return false;
        return dict.Remove(key);
    }

    public static IDictionary ToDictionary(object obj)
    {
        var dict = obj as IDictionary;
        if (dict != null)
            return dict;

        var hash = new Dictionary<object, object>();
        Type type = obj.GetType();

        var en = obj as IEnumerable;
        if (en != null)
            foreach (object? item in en) {
                DictionaryEntry entry = ToEntry(item);
                hash[entry.Key] = entry.Value;
            }
        else
            foreach (PropertyInfo prop in type.GetProperties())
                hash[prop.Name] = prop.GetValue(obj, null);

        return hash;
    }

    public static DictionaryEntry ToEntry(object item)
    {
        if (item is DictionaryEntry)
            return (DictionaryEntry)item;

        Type type = item.GetType();
        if (type.IsGenericType) {
            Type? gen = type.GetGenericTypeDefinition();
            if (gen != null) {
#if MONO
                if (gen == typeof(KeyValuePair<,>))
                    return new DictionaryEntry(item.InvokeMember("Key"), item.InvokeMember("Value"));

                if (gen == typeof(Tuple<,>))
                    return new DictionaryEntry(item.InvokeMember("Item1"), item.InvokeMember("Item2"));
#else
                dynamic dyn = item;
                if (gen == typeof(KeyValuePair<,>))
                    return new DictionaryEntry(dyn.Key, dyn.Value);

                if (gen == typeof(Tuple<,>))
                    return new DictionaryEntry(dyn.Item1, dyn.Item2);
#endif
            }
        }

        return new DictionaryEntry(item.ToString(), item);
    }
}