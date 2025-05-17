namespace Algorithms.Graphs;

public class CoordinateCompression
{
    private Dictionary<int, int> map = new Dictionary<int, int>();
    private int id;
    private int maxId;

    public CoordinateCompression(int startId, int maxId = int.MaxValue)
    {
        this.id = startId;
        this.maxId = maxId;
    }

    public int Compress(int value)
    {
        int result;
        return map.TryGetValue(value, out result) ? result : (map[value] = id++);
    }

    public int[] Compress(int[] values)
    {
        return Array.ConvertAll(values, Compress);
    }

    public List<int> Compress(List<int> values)
    {
        return values.ConvertAll(Compress);
    }
    
    public static int[] CompressX(int[] a)
    {
        int n = a.Length;
        var ret = (int[])a.Clone();
        int[] ind = new int[n];
        for (int i = 0; i < n; i++)
            ind[i] = i;

        Array.Sort(ret, ind);

        int p = 0;
        for (int i = 0; i < n; i++)
        {
            if (i == 0 || ret[i] != ret[i - 1]) ret[p++] = ret[i];
            a[ind[i]] = p - 1;
        }

        Array.Resize(ref ret, p);
        return ret;
    }

    public static long[] CompressX(long[] a)
    {
        int n = a.Length;
        var ret = (long[]) a.Clone();
        int[] ind = new int[n];
        for (int i = 0; i < n; i++)
            ind[i] = i;

        Array.Sort(ret, ind);

        int p = 0;
        for (int i = 0; i < n; i++)
        {
            if (i == 0 || ret[i] != ret[i - 1]) ret[p++] = ret[i];
            a[ind[i]] = p - 1;
        }

        Array.Resize(ref ret, p);
        return ret;
    }
}
