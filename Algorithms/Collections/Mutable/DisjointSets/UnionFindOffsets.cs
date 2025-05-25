namespace Algorithms.Collections;

public class UnionFindOffsets
{
    public readonly int[] Array;
    public readonly int[] Shifts;

    public UnionFindOffsets(int size)
    {
        Array = new int[size];
        Shifts = new int[size];
        Clear();
    }

    public int Count { get; private set; }

    public bool Connected(int x, int y) => Find(x) == Find(y);

    public int GetCount(int x) => -Array[Find(x)];

    void Clear()
    {
        Count = Array.Length;
        for (int i = 0; i < Array.Length; i++)
            Array[i] = -1;
    }

    public bool Union(int x, int y, int shift)
    {
        int rx = Find(x);
        int ry = Find(y);
        if (rx == ry) return false;

        //if (_ds[rx] > _ds[ry])
        //{
        //    Swap(ref rx, ref ry);
        //    shift = -shift;
        //}

        Array[rx] += Array[ry];
        Array[ry] = rx;
        Shifts[ry] = shift - Shifts[y] + Shifts[x];

        Count--;
        return true;
    }

    public int Find(int x)
    {
        int root = Array[x];
        if (root < 0)
            return x;

        Array[x] = Find(root);
        Shifts[x] += Shifts[root];
        return Array[x];
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