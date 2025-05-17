using System.Text;
using System.Threading.Tasks;

namespace Algorithms.Collections;

/// <summary>
/// Disjoint Set with O(1) clearance
/// </summary>
public class UnionFindTimestamped
{
    int[] _ds;
    int[] _timestamp;
    int time;
    int _count;

    public UnionFindTimestamped(int size)
    {
        _ds = new int[size];
        _timestamp = new int[size + 1];
        Clear();
    }

    public int Count => _count;

    public void Clear()
    {
        time++;
        _count = _ds.Length;
    }

    public bool Union(int x, int y)
    {
        var rx = Find(x);
        var ry = Find(y);
        if (rx == ry) return false;

        if (_ds[rx] <= _ds[ry])
        {
            _ds[rx] += _ds[ry];
            _ds[ry] = rx;
        }
        else
        {
            _ds[ry] += _ds[rx];
            _ds[rx] = ry;
        }
        _count--;
        return true;
    }

    public int Find(int x)
    {
        if (_timestamp[x] < time)
        {
            _ds[x] = -1;
            _timestamp[x] = time;
        }
        var root = _ds[x];
        return root < 0
            ? x
            : (_ds[x] = Find(root));
    }

    public int GetCount(int x)
    {
        var c = _ds[Find(x)];
        return c >= 0 ? 1 : -c;
    }

    public IEnumerable<int> Roots()
    {
        for (var i = 0; i < _ds.Length; i++)
            if (Find(i) == i)
                yield return i;
    }

    public IEnumerable<List<int>>Components()
    {
        var comp = new Dictionary<int, List<int>>();
        foreach (var c in Roots())
            comp[c] = new List<int>(GetCount(c));
        for (int i = 0; i < _ds.Length; i++)
            comp[Find(i)].Add(i);
        return comp.Values;
    }
}
