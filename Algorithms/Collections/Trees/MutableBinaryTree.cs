namespace Algorithms.Collections;

public class MutableBinaryTree
{
    public MutableBinaryTree(int key, MutableBinaryTree left = null, MutableBinaryTree right = null)
    {
        Key = key;
        Dup = 1;
        NodeCount = 1;
        Left = left;
        Right = right;
    }

    void Recalc()
    {
        NodeCount = Dup + Count(Left) + Count(Right);
    }

    public static int Count(MutableBinaryTree ost) => ost?.NodeCount ?? 0;

    public static void Insert(ref MutableBinaryTree root, int key)
    {
        if (root == null)
            root = new MutableBinaryTree(key);
        else if (key < root.Key)
            Insert(ref root.Left, key);
        else if (key > root.Key)
            Insert(ref root.Right, key);
        else
            root.Dup++;
        root.Recalc();
    }

    public static int Rank(MutableBinaryTree root, int key, bool upper = false)
    {
        if (root == null)
            return 0;

        if (key < root.Key)
            return Rank(root.Left, key, upper);

        int bound = Count(root.Left);
        if (key == root.Key)
            return bound + (upper ? root.Dup : 0);
        return bound + root.Dup + Rank(root.Right, key, upper);
    }

    public static MutableBinaryTree Select(MutableBinaryTree root, int index)
    {
        while (root != null) {
            int count = Count(root.Left);
            if (index < count) {
                root = root.Left;
            } else {
                index -= count - root.Dup;
                if (index < 0)
                    return root;
                root = root.Right;
            }
        }

        return null;
    }

    public static void Update(MutableBinaryTree root, int index, int value) { }

    public MutableBinaryTree Rebalance(ref MutableBinaryTree root)
    {
        if (root == null)
            return null;

        MutableBinaryTree left = root.Left;
        MutableBinaryTree right = root.Right;

        int compare = Compare(left, right);
        if (compare > 0) {
            MutableBinaryTree ll = left.Left;
            MutableBinaryTree lr = left.Right;
            if (Count(ll) >= Count(lr))
                root = Reconstruct(left, ll, Reconstruct(root, lr, right));
            else
                root = Reconstruct(lr,
                    Reconstruct(left, ll, lr.Left),
                    Reconstruct(root, lr.Right, right));
        } else if (compare < 0) {
            MutableBinaryTree rl = right.Left;
            MutableBinaryTree rr = right.Right;
            if (Count(rl) <= Count(rr))
                root = Reconstruct(right, Reconstruct(root, left, rl), rr);
            else
                root = Reconstruct(rl,
                    Reconstruct(root, left, rl.Left),
                    Reconstruct(right, rl.Right, rr));
        }

        return root;
    }

    int Compare(MutableBinaryTree left, MutableBinaryTree right)
    {
        int leftCount = Count(left) + 1;
        int rightCount = Count(right) + 1;
        if (leftCount > 3 * (rightCount + 1)) return 1;
        if (rightCount > 3 * (leftCount + 1)) return -1;
        return 0;
    }

    public MutableBinaryTree Reconstruct(MutableBinaryTree parent, MutableBinaryTree left, MutableBinaryTree right)
    {
        parent.Left = left;
        parent.Right = right;
        return parent;
    }

    #region Variables

    public MutableBinaryTree Left;
    public MutableBinaryTree Right;
    public int Dup;
    public int NodeCount;
    public int Key;

    #endregion
}