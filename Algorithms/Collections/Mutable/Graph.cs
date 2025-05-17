
namespace Algorithms.Collections;

[DebuggerStepThrough]
public class Graph<T> where T : IEquatable<T>
{
    #region Variables

    private readonly Dictionary<T, HashSet<T>> _dict;
    public bool RetainVertices = false;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)] private int _edgeCount;

    #endregion

    #region Constructor

    public Graph()
    {
        _dict = new Dictionary<T, HashSet<T>>();
    }

    public Graph(int capacity)
    {
        _dict = new Dictionary<T, HashSet<T>>(capacity);
    }

    #endregion

    #region Properties

    public int EdgeCount
    {
        get { return _edgeCount; }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int VertexCount
    {
        get { return _dict.Count; }
    }

    public IEnumerable<KeyValuePair<T, T>> Edges
    {
        get
        {
            foreach (var pair in _dict)
                foreach (var e2 in pair.Value)
                    yield return new KeyValuePair<T, T>(pair.Key, e2);
        }
    }

    public IEnumerable<T> Vertices
    {
        get { return _dict.Keys; }
    }

    //[DebuggerStepThrough]
    //public IEnumerable<KeyValuePair<T, T>> GetEnumerator()
    //{
    //    return Edges;
    //}

    //public bool this[T e1, T e2]
    //{
    //    [DebuggerStepThrough]
    //    get { return HasEdge(e1, e2); }
    //    set
    //    {
    //        if (value)
    //            AddEdge(e1, e2);
    //        else
    //            RemoveEdge(e1, e2);
    //    }
    //}

    #endregion

    #region Methods

    //public void SetEdges(T e1, HashSet<T> set)
    //{
    //    if (set == null)
    //        dict.Remove(e1);
    //    else
    //        dict[e1] = set;
    //}

    public HashSet<T> GetEdges(T e1)
    {
        HashSet<T> edges;
        _dict.TryGetValue(e1, out edges);
        return edges;
    }

    public bool RemoveEdges(T e1)
    {
        return _dict.Remove(e1);
    }

    public bool AddEdge(T e1, T e2)
    {
        HashSet<T> set;
        if (!_dict.TryGetValue(e1, out set))
        {
            set = new HashSet<T> {e2};
            _dict.Add(e1, set);
        }

        var oldCount = set.Count;
        set.Add(e2);
        if (oldCount == set.Count)
            return false;
        _edgeCount++;
        return true;
    }

    //[DebuggerStepThrough]
    //public bool AddUndirectedEdge(T e1, T e2)
    //{
    //    return AddEdge(e1, e2) | AddEdge(e2, e1);
    //}

    public bool RemoveEdge(T e1, T e2)
    {
        HashSet<T> set;
        var oldCount = _edgeCount;
        if (_dict.TryGetValue(e1, out set)
            && set.Remove(e2))
        {
            _edgeCount--;
            if (set.Count == 0 && !RetainVertices)
                _dict.Remove(e1);
        }
        return _edgeCount < oldCount;
    }

    [DebuggerStepThrough]
    public bool HasEdge(T e1, T e2)
    {
        HashSet<T> set;
        return _dict.TryGetValue(e1, out set)
               && set.Contains(e2);
    }

    [DebuggerStepThrough]
    public bool HasStartingVertex(T v)
    {
        return _dict.ContainsKey(v);
    }

    //[DebuggerStepThrough]
    //public bool HasEndingVertex(T v2)
    //{
    //    foreach (var pair in dict)
    //        if (pair.Value.Contains(v2))
    //            return true;
    //    return false;
    //}

    //public IEnumerable<T> GetEndingVertices(T v)
    //{
    //    HashSet<T> set;
    //    if (dict.TryGetValue(v, out set))
    //        return set;
    //    return ListTools.Empty<T>();
    //}

    //public IEnumerable<T> GetStartingVertices(T v2)
    //{
    //    foreach (var pair in dict)
    //        if (pair.Value.Contains(v2))
    //            yield return pair.Key;
    //}

    //[DebuggerStepThrough]
    //public Graph<T> GetInverse()
    //{
    //    var g = new Graph<T>();
    //    foreach (var pair in Edges)
    //        g.AddEdge(pair.Value, pair.Key);
    //    return g;
    //}

    #endregion
}