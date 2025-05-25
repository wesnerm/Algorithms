namespace Algorithms.RangeQueries.SegmentTrees;

//https://www.hackerrank.com/contests/womens-codesprint-4/challenges/lexicographically-smaller-or-equal-strings/editorial
// Finds the number of values in an array from L to R that are less than or equal to value
public class MergeSortTree
{
    readonly int[] array;
    readonly int n;
    public int maxSize;
    public int[][] tree;

    public MergeSortTree(int[] array, int maxSize = int.MaxValue)
    {
        this.array = array;
        n = array.Length;
        this.maxSize = maxSize;
        tree = new int[4 * n][];
        Build(1, 0, n - 1);
    }

    void Build(int node, int start, int end)
    {
        if (start == end) {
            tree[node] = new[] { array[start] };
        } else {
            int l = 2 * node, r = l + 1, mid = (start + end) / 2;
            Build(l, start, mid);
            Build(r, mid + 1, end);
            tree[node] = Merge(tree[l], tree[r]);
        }
    }

    int Query(int node, int start, int end, int i, int j, int value)
    {
        if (start > j || end < i) return 0;

        if (start >= i && end <= j) {
            int answer = Bound(tree[node], value, true);
            return answer;
        }

        int l = 2 * node, r = l + 1, mid = (start + end) >> 1;
        int answer1 = Query(l, start, mid, i, j, value);
        int answer2 = Query(r, mid + 1, end, i, j, value);
        return answer1 + answer2;
    }

    public int Query(int i, int j, int value) => Query(1, 0, n - 1, i, j, value);

    public static int Bound<T>(T[] array, T value, bool upper = false)
        where T : IComparable<T>
    {
        int left = 0;
        int right = array.Length - 1;

        while (left <= right) {
            int mid = left + ((right - left) >> 1);
            int cmp = value.CompareTo(array[mid]);
            if (cmp > 0 || (cmp == 0 && upper))
                left = mid + 1;
            else
                right = mid - 1;
        }

        return left;
    }

    int[] QueryList(int node, int start, int end, int i, int j, int size)
    {
        if (start > j || end < i) return null;

        if (start >= i && end <= j)
            return tree[node];

        int l = 2 * node, r = l + 1, mid = (start + end) >> 1;
        int[] answer1 = QueryList(l, start, mid, i, j, size);
        int[] answer2 = QueryList(r, mid + 1, end, i, j, size);
        return Merge(answer1, answer2);
    }

    public int[] QueryList(int i, int j, int size) => QueryList(1, 0, n - 1, i, j, size);

    void UpdateList(int node, int start, int end, int x, int value)
    {
        if (start > x || end < x) return;

        if (start == end) {
            tree[node][0] = value;
            return;
        }

        int l = 2 * node, r = l + 1, mid = (start + end) >> 1;
        UpdateList(l, start, mid, x, value);
        UpdateList(r, mid + 1, end, x, value);
        tree[node] = Merge(tree[l], tree[r]);
    }

    public void UpdateList(int x, int value)
    {
        UpdateList(1, 0, n - 1, x, value);
    }

    int[] Merge(int[] array1, int[] array2)
    {
        if (array1 == null) return array2;
        if (array2 == null) return array1;

        int size = Math.Min(maxSize, array1.Length + array2.Length);
        int[] buffer = new int[size];

        int i = 0;
        int j = 0;
        int k = 0;
        while (k < size)
            buffer[k++] = j >= array2.Length || (i < array1.Length && array1[i].CompareTo(array2[j]) <= 0)
                ? array1[i++]
                : array2[j++];

        return buffer;
    }
}