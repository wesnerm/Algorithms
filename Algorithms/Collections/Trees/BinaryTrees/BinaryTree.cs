namespace Algorithms.Collections.Trees;

public abstract class BinaryTree<T, V>
    where T : BinaryTree<T, V>, new()
    where V : IComparable<V>
{
    #region Constructors

    protected BinaryTree() => Left = Right = (T)this;

    protected BinaryTree(V key)
    {
        Key = key;
        Count = 1;
        Left = Right = Empty;
    }

    protected BinaryTree(V key, T left, T right)
    {
        Key = key;
        Count = left.Count + right.Count + 1;
        Left = left;
        Right = right;
    }

    protected abstract T Construct(V key);

    protected abstract T Construct(V key, T right);

    public static T Join(T left, V key, T right)
    {
        int compare = left.Compare(right);
        if (compare == 0)
            return left.Construct(key, right);

        if (compare > 0) {
            T ll = left.Left;
            T lr = left.Right;
            if (ll.Count >= lr.Count)
                return ll.Construct(right.Key, Join(lr, key, right));
            return ll.Construct(left.Key, lr.Left)
                .Construct(lr.Key, Join(lr.Right, key, right));
        }

        T rl = right.Left;
        T rr = right.Right;
        if (rl.Count <= rr.Count)
            return Join(left, key, rl).Construct(right.Key, rr);
        return Join(left, key, rl.Left)
            .Construct(rl.Key, rl.Right.Construct(right.Key, rr));
    }

    public static T Join(T left, T right)
    {
        if (left.IsEmpty) return right;
        if (right.IsEmpty) return left;

        if (left.Compare(right) > 0) {
            T ll = left.Left;
            T lr = left.Right;
            if (ll.Count >= lr.Count)
                return ll.Construct(right.Key, Join(lr, right));
            return ll.Construct(left.Key, lr.Left)
                .Construct(lr.Key, Join(lr.Right, right));
        }

        T rl = right.Left;
        T rr = right.Right;
        if (rl.Count <= rr.Count)
            return Join(left, rl).Construct(right.Key, rr);
        return Join(left, rl.Left)
            .Construct(rl.Key, rl.Right.Construct(right.Key, rr));
    }

    public T JoinChildren(T left, T right) => Left != left || Right != right ? Join(left, Key, right) : (T)this;

    #endregion

    #region Properties

    public readonly V Key;
    public readonly int Count;
    public readonly T Left, Right;
    public static readonly T Empty = new();

    public bool IsEmpty => Left == this;

    protected virtual int Compare(T other)
    {
        int leftCount = Count + 1;
        int rightCount = other.Count + 1;
        if (leftCount > 3 * (rightCount + 1)) return 1;
        if (rightCount > 3 * (leftCount + 1)) return -1;
        return 0;
    }

    public V[] Table(int start = 0, int count = -1)
    {
        if (count < 0) count += Count + 1 - start;
        var table = new V[count];
        int index = 0;
        FillTable(table, ref index, start, start + count - 1);
        return table;
    }

    void FillTable(V[] table, ref int index, int start, int end)
    {
        if (IsEmpty) return;
        int leftCount = Left.Count;
        if (start < leftCount)
            Left.FillTable(table, ref index, start, end);
        if (start <= leftCount && leftCount <= end)
            table[index++] = Key;
        if (end > leftCount)
            Right.FillTable(table, ref index, start - (leftCount + 1), end - (leftCount + 1));
    }

    #endregion

    #region Set Operations

    public T Insert(V key)
    {
        if (IsEmpty)
            return Construct(key);
        int compare = key.CompareTo(Key);
        if (compare < 0)
            return JoinChildren(Left.Insert(key), Right);
        if (compare > 0)
            return JoinChildren(Left, Right.Insert(key));
        return (T)this;
    }

    public T Remove(V key)
    {
        if (!IsEmpty) {
            int compare = key.CompareTo(Key);
            if (compare < 0)
                return JoinChildren(Left.Remove(key), Right);
            if (compare > 0)
                return JoinChildren(Left, Right.Remove(key));
        }

        return (T)this;
    }

    public static T Union(T left, T right)
    {
        if (left == right) return left;
        if (left.Count < right.Count) return Union(right, left);
        if (left.IsEmpty) return right;
        if (right.IsEmpty) return left;
        (T left, T right) split = right.Split(left.Key);
        return left.JoinChildren(Union(left.Left, split.left), Union(left.Right, split.right));
    }

    public static T Intersection(T left, T right)
    {
        if (left == right) return left;
        if (left.IsEmpty || right.IsEmpty) return Empty;
        (T left, T right) split = right.Split(left.Key);
        bool exists = split.left.Count + split.right.Count != right.Count;
        if (exists)
            return left.JoinChildren(Intersection(left.Left, split.left), Intersection(left.Right, split.right));
        return Join(Intersection(left.Left, split.left), Intersection(left.Right, split.right));
    }

    public static T Difference(T left, T right)
    {
        if (left == right || left.IsEmpty) return Empty;
        if (right.IsEmpty) return left;
        (T left, T right) split = right.Split(left.Key);
        bool exists = split.left.Count + split.right.Count != right.Count;
        if (!exists)
            return left.JoinChildren(Difference(left.Left, split.left), Difference(left.Right, split.right));
        return Join(Difference(left.Left, split.left), Difference(left.Right, split.right));
    }

    public static T ExclusiveUnion(T left, T right)
    {
        if (left == right) return Empty;
        if (left.IsEmpty) return right;
        if (right.IsEmpty) return left;
        if (left.Count < right.Count) return ExclusiveUnion(right, left);
        (T left, T right) split = right.Split(left.Key);
        bool exists = split.left.Count + split.right.Count != right.Count;
        if (exists)
            return Join(ExclusiveUnion(left.Left, split.left), ExclusiveUnion(left.Right, split.right));
        return left.JoinChildren(ExclusiveUnion(left.Left, split.left), ExclusiveUnion(left.Right, split.right));
    }

    public (T left, T right) Split(V key, bool removeKey = true)
    {
        if (IsEmpty) return (Empty, Empty);

        int compare = key.CompareTo(Key);
        if (compare < 0) {
            (T left, T right) pair = Left.Split(key, removeKey);
            return (pair.left, JoinChildren(pair.right, Right));
        }

        if (compare > 0) {
            (T left, T right) pair = Right.Split(key, removeKey);
            return (JoinChildren(Left, pair.left), pair.right);
        }

        return (Left, removeKey ? Right : Join(Empty, key, Right));
    }

    public int Rank(int key)
    {
        BinaryTree<T, V> node = this;
        int start = 0;

        while (!node.IsEmpty) {
            int compare = key.CompareTo(Key);
            if (compare < 0) {
                node = node.Left;
            } else {
                int leftCount = Left.Count;
                if (compare == 0)
                    return start + leftCount;
                start += leftCount + 1;
                node = node.Right;
            }
        }

        return ~start;
    }

    #endregion

    #region List Operations

    public V SelectAt(int index, V defaultValue = default)
    {
        BinaryTree<T, V> node = this;
        while (!node.IsEmpty) {
            int leftCount = node.Left.Count;
            if (index < leftCount) {
                node = node.Left;
            } else if (index > leftCount) {
                node = node.Right;
                index -= leftCount + 1;
            } else if (index == leftCount) {
                return node.Key;
            }
        }

        return defaultValue;
    }

    public T InsertAt(int position, V key) => InsertAt(position, Construct(key));

    public T InsertAt(int position, T node)
    {
        if (IsEmpty)
            return node;

        int leftCount = Left.Count;
        return position <= leftCount
            ? JoinChildren(Left.InsertAt(position, node), Right)
            : JoinChildren(Left, Right.InsertAt(position - leftCount - 1, node));
    }

    public T ReplaceAt(int position, V key)
    {
        if (!IsEmpty) {
            int leftCount = Left.Count;
            if (position < leftCount)
                return JoinChildren(Left.ReplaceAt(position, key), Right);
            if (position > leftCount)
                return JoinChildren(Left, Right.ReplaceAt(position, key));
        }

        return Join(Left, key, Right);
    }

    public T RemoveAt(int position)
    {
        if (IsEmpty)
            return (T)this;

        int leftCount = Left.Count;
        if (position < leftCount)
            return JoinChildren(Left.RemoveAt(position), Right);
        if (position > leftCount)
            return JoinChildren(Left, Right.RemoveAt(position - leftCount - 1));
        return Join(Left, Right);
    }

    public T RemoveRange(int start, int count)
    {
        if (count <= 0 || IsEmpty)
            return (T)this;

        int end = start + count;
        int leftCount = Left.Count;
        T leftPart = start < leftCount ? Left.RemoveRange(start, count) : Left;
        int rightStart = Math.Max(start, leftCount + 1);
        T rightPart = end - 1 > leftCount ? Right.RemoveRange(rightStart, end - rightStart) : Right;

        return leftCount >= start && leftCount < end
            ? JoinChildren(leftPart, rightPart)
            : Join(leftPart, rightPart);
    }

    public T CopyRange(int start, int count)
    {
        int end = start + count;
        if (end <= 0 || start >= Count)
            return Empty;

        if (start <= 0 && end >= Count)
            return (T)this;

        int mid = Left.Count;
        T leftPart = Left.CopyRange(start, count);
        T rightPart = Right.CopyRange(start - (mid + 1), count);

        return start <= mid && mid <= end
            ? JoinChildren(leftPart, rightPart)
            : Join(leftPart, rightPart);
    }

    public T Remove(Predicate<V> keep)
    {
        if (IsEmpty)
            return (T)this;
        if (keep(Key))
            return JoinChildren(Left.Remove(keep), Right.Remove(keep));
        return Join(Left.Remove(keep), Right.Remove(keep));
    }

    public (T left, T right) SplitAt(int pos)
    {
        if (IsEmpty) return (Empty, Empty);

        int leftCount = Left.Count;
        if (pos <= leftCount) {
            (T left, T right) split = Left.SplitAt(pos);
            return (split.left, JoinChildren(split.right, Right));
        } else {
            (T left, T right) split = Right.SplitAt(pos - (leftCount + 1));
            return (JoinChildren(Left, split.left), split.right);
        }
    }

    #endregion
}