namespace Algorithms.Collections;

/// <summary>
///     Disjoint Set
/// </summary>
public class UnionFindRollback
{
    #region Constructor

    public UnionFindRollback(int size, int bufferSize = -1)
    {
        Array = new int[size];
        if (bufferSize == -1) bufferSize = size;
        _values = new int[bufferSize];
        _indices = new int[bufferSize];
        Clear();
    }

    #endregion

    #region Variables

    public readonly int[] Array;
    int[] _indices;
    int[] _values;
    int _time;

    #endregion

    #region Properties

    public int Count { get; private set; }

    public int Time {
        get => _time;
        set
        {
            if (value < 0) {
                _time = -1;
                return;
            }

            while (value < _time) {
                _time--;
                Array[_indices[_time]] = _values[_time];
                // _values[_time] = default(T);
            }

            if (_time < 0)
                _time = 0;
        }
    }

    #endregion

    #region Methods

    int SetIndex(int index, int value)
    {
        int time = _time;
        if (time >= 0) {
            if (time >= _indices.Length) {
                int newSize = Math.Max(4, time * 2);
                System.Array.Resize(ref _values, newSize);
                System.Array.Resize(ref _indices, newSize);
            }

            _indices[time] = index;
            _values[time] = Array[index];
            _time++;
        }

        Array[index] = value;
        return value;
    }

    public void Clear()
    {
        Count = Array.Length;
        for (int i = 0; i < Array.Length; i++)
            Array[i] = -1;
        _time = 0;
    }

    public bool Union(int x, int y)
    {
        int rx = Find(x);
        int ry = Find(y);
        if (rx == ry) return false;

        if (Array[rx] <= Array[ry]) {
            SetIndex(rx, Array[ry] + Array[rx]);
            SetIndex(ry, rx);
        } else {
            SetIndex(ry, Array[ry] + Array[rx]);
            SetIndex(rx, ry);
        }

        Count--;
        return true;
    }

    public bool Connected(int x, int y) => Find(x) == Find(y);

    public int Find(int x)
    {
        int root = Array[x];
        return root < 0
            ? x
            : SetIndex(x, Find(root));
        //return root < 0
        //    ? x
        //    : Find(root);
    }

    public int GetCount(int x)
    {
        int c = Array[Find(x)];
        return c >= 0 ? 1 : -c;
    }

    public IEnumerable<int> Roots()
    {
        int length = Array.Length;
        for (int i = 0; i < length; i++)
            if (Array[i] < 0)
                yield return i;
    }

    public IEnumerable<List<int>> Components()
    {
        var comp = new Dictionary<int, List<int>>();
        foreach (int c in Roots())
            comp[c] = new List<int>(GetCount(c));
        int length = Array.Length;
        for (int i = 0; i < length; i++)
            comp[Find(i)].Add(i);
        return comp.Values;
    }

    #endregion
}