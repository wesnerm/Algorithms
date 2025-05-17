namespace Algorithms.Collections;

// Improvements
// 1) Inline persistent array code in disjoint set
// 2) Remove count

public struct UnionFindPersistent
{
    PArray<int> _ds;

    public UnionFindPersistent(UnionFind set)
    {
        _ds = new PArray<int>((int[])set.Array.Clone());
    }

    public UnionFindPersistent(int size)
    {
        var array = new int[size];
        for (var i = 0; i < size; i++)
            array[i] = -1;

        _ds = new PArray<int>(array);
    }

    UnionFindPersistent(PArray<int> ds)
    {
        _ds = ds;
    }

    public UnionFindPersistent Union(int x, int y)
    {
        var rx = Find(x);
        var ry = Find(y);
        if (rx == ry) return this;

        PArray<int> ds;
        var count = _ds[rx] + _ds[ry];
        ds = _ds[rx] <= _ds[ry]
            ? _ds.Set(rx, count).Set(ry, rx)
            : _ds.Set(ry, count).Set(rx, ry);
        return new UnionFindPersistent(ds);
    }

    public int Find(int x)
    {
#if !UNCOMPRESSED_DS
        var root = _ds[x];
        if (root < 0) return x;

        var f = Find(root);
        _ds = _ds.Set(x, f);
        return f;
#else
        var root = _ds[x];
        while (root >= 0)
        {
            x = root;
            root = _ds[x];
        }
        return x;
#endif
    }

    public int GetCount(int x)
    {
        var rx = Find(x);
        return -_ds[rx];
    }

    public int GetCount()
    {
        _ds.Sync();
        var array = _ds.Data;
        int count = 0;
        for (int i = 0; i < array.Length; i++)
            if (array[i] < 0)
                count++;
        return count;
    }

    public IEnumerable<int> Roots()
    {
        _ds.Sync();
        var array = _ds.Data;
        for (int i = 0; i < array.Length; i++)
            if (array[i] < 0)
                yield return i;
    }

    public IEnumerable<List<int>> Components()
    {
        _ds.Sync();
        var array = _ds.Data;
        var comp = new Dictionary<int, List<int>>();
        for (int i = 0; i < array.Length; i++)
            if (array[i] < 0)
                comp[i] = new List<int>(-array[i]);
        for (int i = 0; i < array.Length; i++)
                comp[Find(i)].Add(i);
        return comp.Values;
    }
}