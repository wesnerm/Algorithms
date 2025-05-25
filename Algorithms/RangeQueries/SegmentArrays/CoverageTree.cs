using T = int;

namespace Algorithms.RangeQueries;

/// <summary>
///     Determines number of ranges
/// </summary>
public class CoverageTree
{
    #region Union Area

    public static long UnionArea(IList<int[]> rectangles)
    {
        unchecked {
            const int X1 = 0, Y1 = 1, X2 = 2, Y2 = 3;

            int n = rectangles.Count;
            long[] events = new long[2 * n];
            int[] y = new int[2 * n];
            for (int i = 0; i < n; i++) {
                // rect is width [x1,x2) [y1,y2) real interval
                int[] rect = rectangles[i];
                int x1 = rect[X1];
                int y1 = rect[Y1];
                int x2 = rect[X2];
                int y2 = rect[Y2];
                events[2 * i] = ((long)x1 << 32) + i;
                events[2 * i + 1] = ((long)x2 << 32) | (~i & 0xFFFFFFFFL);
                y[2 * i] = y1;
                y[2 * i + 1] = y2;
            }

            Array.Sort(events);
            Array.Sort(y);

            var t = new CoverageTree(y);
            long area = 0;
            int lastX = (int)(events[0] >> 32);
            foreach (long e in events) {
                int x = (int)(e >> 32);
                int i = (int)(e & 0xFFFFFFFFL);
                bool inside = i >= 0;
                if (!inside) i = ~i;
                int dx = x - lastX;
                T dy = t.QueryAll();
                area += (long)dx * dy;
                lastX = x;
                int y1 = rectangles[i][Y1];
                int y2 = rectangles[i][Y2];
                t.UpdateX(y1, y2, inside ? 1 : -1);
            }

            return area;
        }
    }

    #endregion

    #region Variables

    readonly int[] Count;
    readonly T[] Len;
    public readonly T[] X;
    public int Size;

    #endregion

    #region Constructor

    public CoverageTree(T[] xSorted) : this(xSorted.Length) { }

    public CoverageTree(int size)
    {
        int len = 1;
        while (len < size) len <<= 1;
        len = 2 * len;

        Count = new int[len];
        Len = new T[len];
        Size = size;
    }

    #endregion

    #region Utilities

    public static int[] SortAndRemoveDupes(T[] array)
    {
        var list = new List<T>(array);
        list.Sort();
        int write = 0;
        for (int read = 0; read < list.Count; read++) {
            T e = list[read];
            if (write == 0 || e != list[write - 1])
                list[write++] = e;
        }

        list.RemoveRange(write, list.Count - write);
        return list.ToArray();
    }

    public int XToIndex(T x)
    {
        if (X == null) return x;
        int left = 0;
        int right = Size - 1;
        while (left <= right) {
            int mid = (left + right) >> 1;
            int cmp = x - X[mid];
            if (cmp > 0)
                left = mid + 1;
            else if (cmp < 0 || (mid > left && x == X[mid - 1]))
                right = mid - 1;
            else
                return mid;
        }

        return right;
    }

    #endregion

    #region Updates

    public void UpdateX(T x0, T x1, int delta)
    {
        UpdateIndex(XToIndex(x0), XToIndex(x1) - 1, delta);
    }

    public void UpdateIndex(int from, int to, int delta)
    {
        UpdateIndex(0, from, to, delta, 0, Size - 1);
    }

    void UpdateIndex(int root, int from, int to, int delta, int nodeLeft, int nodeRight)
    {
        if (from == nodeLeft && to == nodeRight) {
            Count[root] += delta;
        } else {
            int mid = (nodeLeft + nodeRight) >> 1;
            if (from <= mid)
                UpdateIndex(2 * root + 1, from, Math.Min(to, mid), delta, nodeLeft, mid);
            if (to > mid)
                UpdateIndex(2 * root + 2, Math.Max(from, mid + 1), to, delta, mid + 1, nodeRight);
        }

        Len[root] = Count[root] != 0
            ? X != null ? X[nodeRight + 1] - X[nodeLeft] : nodeRight + 1 - nodeLeft
            : nodeRight > nodeLeft
                ? Len[2 * root + 1] + Len[2 * root + 2]
                : 0;
    }

    #endregion

    #region Queries

    public T QueryAll() => Len[0];

    public T QueryX(T from, T to) =>
        // TODO: Endpoints need to be adjusted in results if they are in between indices
        QueryIndex(XToIndex(from), XToIndex(to));

    public T QueryIndex(int from, int to)
    {
        int right = Size - 1;
        if (from < 0) from = 0;
        if (to > right) to = right;
        return QueryIndex(0, from, to, 0, right);
    }

    T QueryIndex(int root, int from, int to, int nodeLeft, int nodeRight)
    {
        if (from == nodeLeft && to == nodeRight)
            return Len[root];

        if (Count[root] != 0)
            return X != null ? X[to + 1] - X[from] : to + 1 - from;

        int mid = (nodeLeft + nodeRight) >> 1;
        int result = 0;
        if (from <= mid)
            result += QueryIndex(2 * root + 1, from, Math.Min(to, mid), nodeLeft, mid);
        if (to > mid)
            result += QueryIndex(2 * root + 2, Math.Max(from, mid + 1), to, mid + 1, nodeRight);

        return result;
    }

    #endregion

    #region Find

    public int NextIndex(int index, bool on = true) => NextIndex(0, index + 1, 0, Size - 1, on);

    int NextIndex(int root, int index, int nodeLeft, int nodeRight, bool on)
    {
        if (Count[root] != 0)
            return on ? index : nodeRight + 1;

        if (Len[root] == 0)
            return on ? nodeRight + 1 : index;

        Debug.Assert(nodeLeft < nodeRight);

        int mid = (nodeLeft + nodeRight) >> 1;
        if (index <= mid) {
            int result = NextIndex(2 * root + 1, index, nodeLeft, mid, on);
            if (result <= mid)
                return result;
            index = result;
        }

        return NextIndex(2 * root + 2, index, mid + 1, nodeRight, on);
    }

    public int PreviousIndex(int index, bool on = true) => PreviousIndex(0, index - 1, 0, Size - 1, on);

    int PreviousIndex(int root, int index, int nodeLeft, int nodeRight, bool on)
    {
        if (Count[root] != 0)
            return on ? index : nodeLeft - 1;

        if (Len[root] == 0)
            return on ? nodeLeft - 1 : index;

        Debug.Assert(nodeLeft < nodeRight);

        int mid = (nodeLeft + nodeRight) >> 1;

        if (index > mid) {
            int result = PreviousIndex(2 * root + 2, index, mid + 1, nodeRight, on);
            if (result > mid)
                return result;
            index = result;
        }

        return PreviousIndex(2 * root + 1, index, nodeLeft, mid, on);
    }

    public bool StateOfIndex(int index) => StateOfIndex(0, index, Size);

    bool StateOfIndex(int root, int index, int size)
    {
        if (Count[root] != 0) return true;
        if (Len[root] == 0) return false;

        int half = (size + 1) >> 1;
        return index < half
            ? StateOfIndex(2 * root + 1, index, half)
            : StateOfIndex(2 * root + 2, index - half, size - half);
    }

    #endregion

    #region Properties

    public bool this[int x] => StateOfIndex(XToIndex(x));

    public bool[] Table {
        get
        {
            var list = new List<bool>();
            for (int i = 0; i < Size; i++)
                list.Add(StateOfIndex(i));
            return list.ToArray();
        }
    }

    #endregion
}