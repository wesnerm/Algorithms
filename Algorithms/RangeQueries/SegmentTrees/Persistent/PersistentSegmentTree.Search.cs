namespace Algorithms.RangeQueries;

using STType = long;

public partial class PersistentSegmentTree
{
    #region Search

    public int FindGreater(STType k, int start = 0)
    {
        if (k >= Sum) return -1;
        if (Length == 1) return start;

        LazyPropagate();
        long leftSum = Left.Sum;
        return k < leftSum
            ? Left.FindGreater(k, start)
            : Right.FindGreater(k - leftSum, start + Left.Length);
    }

    public int Next(STType k, int start = 0)
    {
        if (k >= start + Length - 1 || Sum <= 0) return -1;
        if (Length == 1) return start;

        LazyPropagate();
        int result = Left.Next(k);
        if (result < 0) result = Right.Next(k, start + Left.Length);
        return result;
    }

    public int Previous(STType k, int start = 0)
    {
        if (k <= start || Sum <= 0) return -1;
        if (Length == 1) return start;

        LazyPropagate();
        int result = Right.Previous(k, start + Left.Length);
        if (result < 0) result = Left.Previous(k);
        return result;
    }

    #endregion
}