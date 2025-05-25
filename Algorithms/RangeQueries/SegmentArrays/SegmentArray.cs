namespace Algorithms.RangeQueries;

/// <summary>
///     Fast bottom up segment tree
/// </summary>
public struct SegmentArray
{
    readonly int[] _tree;

    public SegmentArray(int size) => _tree = new int[size * 2];

    public SegmentArray(int[] array)
        : this(array.Length)
    {
        int size = array.Length;
        Array.Copy(array, 0, _tree, size, array.Length);
        for (int i = size - 1; i > 0; i--)
            _tree[i] = _tree[i << 1] + _tree[(i << 1) | 1];
    }

    public void Update(int index, int value)
    {
        int i = index + (_tree.Length >> 1);
        _tree[i] += value;
        for (; i > 1; i >>= 1)
            _tree[i >> 1] = _tree[i] + _tree[i ^ 1];
    }

    public int this[int index] {
        get => _tree[index + (_tree.Length >> 1)];
        set
        {
            // Update(index, value - this[index]
            int i = index + (_tree.Length >> 1);
            _tree[i] = value;
            for (; i > 1; i >>= 1)
                _tree[i >> 1] = _tree[i] + _tree[i ^ 1];
        }
    }

    public int Query(int left, int right)
    {
        int size = _tree.Length >> 1;
        left += size;
        right += size;

        int leftResult = 0;
        int rightResult = 0;
        for (; left <= right; left >>= 1, right >>= 1) {
            if ((left & 1) == 1) leftResult = leftResult + _tree[left++];
            if ((right & 1) == 0) rightResult = _tree[right--] + rightResult;
        }

        return leftResult + rightResult;
    }

    public override string ToString() => string.Join(",", _tree);
}