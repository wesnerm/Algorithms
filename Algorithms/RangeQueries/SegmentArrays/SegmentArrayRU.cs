namespace Algorithms.RangeQueries;

/// <summary>
///     Fast bottom up segment tree
/// </summary>
public struct SegmentArrayRU
{
    readonly int[] _tree;

    public SegmentArrayRU(int size) => _tree = new int[size * 2];

    public SegmentArrayRU(int[] array)
        : this(array.Length)
    {
        int size = array.Length;
        Array.Copy(array, 0, _tree, size, array.Length);
    }

    public int this[int index] {
        get => Query(index);
        set => Add(index, index, value - this[index]);
    }

    public void Add(int left, int right, int value)
    {
        int size = _tree.Length >> 1;
        left += size;
        right += size;
        for (; left <= right; left >>= 1, right >>= 1) {
            if ((left & 1) == 1) _tree[left++] += value;
            if ((right & 1) == 0) _tree[right--] += value;
        }
    }

    public int Query(int index)
    {
        int result = 0;
        int i = (index + _tree.Length) >> 1;
        for (; i > 0; i >>= 1) result += _tree[i];
        return result;
    }

    public override string ToString() => string.Join(",", _tree);
}