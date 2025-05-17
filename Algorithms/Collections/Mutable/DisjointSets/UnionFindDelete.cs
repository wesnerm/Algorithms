
namespace Algorithms.Collections;

public class UnionFindDelete
{
    readonly Node[] _ds;
    int _count;

    public UnionFindDelete(int size)
    {
        _ds = new Node[size];
        Clear();
    }

    public int Count => _count;

    public void Clear()
    {
        _count = _ds.Length;
        for (int i = 0; i < _ds.Length; i++)
            if (_ds[i]==null || _ds[i].Count != 1)
                _ds[i] = new Node {Id = i};
    }

    public bool Union(int x, int y)
    {
        var rx = Find(ref _ds[x]);
        var ry = Find(ref _ds[y]);
        if (rx == ry) return false;
        if (rx.Count >= ry.Count)
        {
            rx.Count += ry.Count;
            _ds[y] = ry.Parent = rx;
        }
        else
        {
            ry.Count += rx.Count;
            _ds[x] = rx.Parent = ry;
        }
        _count--;
        return true;
    }

    public int Find(int x)
    {
        var r = Find(ref _ds[x]);
        if (r.Id < 0) r.Id = x;
        return r.Id;
    }

    static Node Find(ref Node x)
    {
        return x.Parent == null ? x : (x = Find(ref x.Parent));
    }

    public void Delete(int x)
    {
        var n = Find(ref _ds[x]);
        if (n.Count == 1) return;
        if (n.Id == x) n.Id = -1;
        n.Count--;
        _count++;
        _ds[x] = new Node {Id = x}; 
    }

    public int GetCount(int x)
    {
        return _ds[Find(x)].Count;
    }

    public IEnumerable<int> Roots()
    {
        for (int i = 0; i < _ds.Length; i++)
            if (Find(i) == i)
                yield return i;
    }

    public IEnumerable<List<int>> Components()
    {
        var comp = new Dictionary<int, List<int>>();
        foreach (var c in Roots())
            comp[c] = new List<int>(GetCount(c));

        for (int i = 0; i < _ds.Length; i++)
            comp[Find(i)].Add(i);
        return comp.Values;
    }

    class Node
    {
        public Node Parent;
        public int Count = 1;
        public int Id;
    }
}
