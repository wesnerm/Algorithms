namespace Algorithms.Collections;

public class UnionFindDelete
{
    readonly Node[] _ds;

    public UnionFindDelete(int size)
    {
        _ds = new Node[size];
        Clear();
    }

    public int Count { get; private set; }

    public void Clear()
    {
        Count = _ds.Length;
        for (int i = 0; i < _ds.Length; i++)
            if (_ds[i] == null || _ds[i].Count != 1)
                _ds[i] = new Node { Id = i };
    }

    public bool Union(int x, int y)
    {
        Node rx = Find(ref _ds[x]);
        Node ry = Find(ref _ds[y]);
        if (rx == ry) return false;
        if (rx.Count >= ry.Count) {
            rx.Count += ry.Count;
            _ds[y] = ry.Parent = rx;
        } else {
            ry.Count += rx.Count;
            _ds[x] = rx.Parent = ry;
        }

        Count--;
        return true;
    }

    public int Find(int x)
    {
        Node r = Find(ref _ds[x]);
        if (r.Id < 0) r.Id = x;
        return r.Id;
    }

    static Node Find(ref Node x) => x.Parent == null ? x : x = Find(ref x.Parent);

    public void Delete(int x)
    {
        Node n = Find(ref _ds[x]);
        if (n.Count == 1) return;
        if (n.Id == x) n.Id = -1;
        n.Count--;
        Count++;
        _ds[x] = new Node { Id = x };
    }

    public int GetCount(int x) => _ds[Find(x)].Count;

    public IEnumerable<int> Roots()
    {
        for (int i = 0; i < _ds.Length; i++)
            if (Find(i) == i)
                yield return i;
    }

    public IEnumerable<List<int>> Components()
    {
        var comp = new Dictionary<int, List<int>>();
        foreach (int c in Roots())
            comp[c] = new List<int>(GetCount(c));

        for (int i = 0; i < _ds.Length; i++)
            comp[Find(i)].Add(i);
        return comp.Values;
    }

    class Node
    {
        public int Count = 1;
        public int Id;
        public Node Parent;
    }
}