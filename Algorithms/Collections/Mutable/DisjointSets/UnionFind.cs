namespace Algorithms.Collections;

public class UnionFind
{
    public readonly int[] Array;

    public UnionFind(int size)
    {
        Array = new int[size];
        Clear();
    }

    public int Count { get; private set; }

    public bool Connected(int x, int y) => Find(x) == Find(y);

    public int GetCount(int x) => -Array[Find(x)];

    public void Clear()
    {
        Count = Array.Length;
        for (int i = 0; i < Array.Length; i++)
            Array[i] = -1;
    }

    public bool Union(int x, int y, bool deterministic = false)
    {
        int rx = Find(x);
        int ry = Find(y);
        if (rx == ry) return false;

        if (Array[rx] <= Array[ry] || deterministic) {
            Array[rx] += Array[ry];
            Array[ry] = rx;
        } else {
            Array[ry] += Array[rx];
            Array[rx] = ry;
        }

        Count--;
        return true;
    }

    public int Find(int x)
    {
        int root = Array[x];
        return root < 0 ? x : Array[x] = Find(root);
    }

    public IEnumerable<int> Roots()
    {
        for (int i = 0; i < Array.Length; i++)
            if (Array[i] < 0)
                yield return i;
    }

    public IEnumerable<List<int>> Components()
    {
        var comp = new Dictionary<int, List<int>>();
        foreach (int c in Roots())
            comp[c] = new List<int>(GetCount(c));
        for (int i = 0; i < Array.Length; i++)
            comp[Find(i)].Add(i);
        return comp.Values;
    }
}