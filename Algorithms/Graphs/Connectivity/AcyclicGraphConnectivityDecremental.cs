using Algorithms.Collections;

namespace Algorithms.Graphs.LinkCutTrees;

// Optimized for incremental and decremental connectivity
public class AcyclicGraphConnectivityDecremental
{
    const int DefaultHashCutoff = 10;
    ICollection<int>[] _g;
    int[] _touch;
    int time;
    int _hashCutoff;
    readonly DisjointSet[] _ds;
    int _count;

    public AcyclicGraphConnectivityDecremental(int size, int hashCutoff = DefaultHashCutoff)
    {
        _g = new ICollection<int>[size];
        _ds = new DisjointSet[size];
        _touch = new int[size];
        for (int i=0; i<size; i++)
            _ds[i] = new DisjointSet();
        _hashCutoff = hashCutoff;
        Clear();
    }

    public int Count => _count;

    public void Clear()
    {
        _count = _ds.Length;
        for (int i = 0; i < _ds.Length; i++)
            _ds[i].Delete();
    }

    void AddOneEdge(int x, int y)
    {
        if (_g[x] == null) _g[x] = new List<int>();
        _g[x].Add(y);
        if (_g[x].Count == 10 && _g[x] is List<int>)
            _g[x] = new HashSet<int>(_g[x]);
    }

    public bool AddEdge(int x, int y)
    {
        if (_g[x] != null && _g[x].Contains(y))
            return false;

        AddOneEdge(x, y);
        AddOneEdge(y, x);

        if (_ds[x].Union(_ds[y]))
            _count--;
        return true;
    }

    public bool RemoveEdge(int x, int y)
    {
        if (_g[x] == null || !_g[x].Contains(y))
            return false;

        _g[x].Remove(x);
        _g[y].Remove(y);

        time++;
        int relabel;
        var en1 = Dfs(x).GetEnumerator();
        var en2 = Dfs(y).GetEnumerator();
        while (true)
        {
            if (!en1.MoveNext())
            {
                relabel = x;
                break;
            }

            if (!en2.MoveNext())
            {
                relabel = y;
                break;
            }
        }

        // TODO: Can improve this to work with cycles

        time++;
        var ds = _ds[relabel];
        ds.Delete();
        foreach (var v in Dfs(relabel))
            _ds[v].AssignToSet(ds);

        return true;
    }

    IEnumerable<int> Dfs(int x)
    {
        var stack = new Stack<int>();
        stack.Push(x);
        _touch[x] = time;
        while (stack.Count > 0)
        {
            var pop = stack.Pop();
            foreach (var v in _g[pop])
            {
                if (_touch[v] >= time) continue;
                yield return v;
                _touch[v] = time;
                stack.Push(v);
            }
        }
    }

    public bool Connected(int x, int y)
    {
        return _ds[x].Connected(_ds[y]);
    }

    public int GetCount(int x)
    {
        return _ds[x].Count;
    }

    public IEnumerable<List<int>> Components()
    {
        for (int i = _ds.Length - 1; i >= 0; i--)
            _ds[i].Label = i;

        var comp = new Dictionary<int, List<int>>();
        for (int i = 0; i < _ds.Length; i++)
        {
            int label = _ds[i].Label;
            if (!comp.TryGetValue(label, out List<int> list))
                comp[label] = list = new List<int>();
            list.Add(i);
        }

        return comp.Values;
    }
}
