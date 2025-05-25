namespace Algorithms.Collections;

public class Tree<TData, TNode>
    where TData : class
    where TNode : Tree<TData, TNode>
{
    protected TData Data;
    protected int Height;
    protected TNode Left;
    protected TNode Right;

    protected Tree() { }

    public abstract class BalancingAlgoritm
    {
        protected abstract TNode Construct(TData data, TNode left, TNode right);

        // O(log n - log m) where n is the larger subtree and m is the smaller subtree
        public TNode Create(TData data, TNode left, TNode right)
        {
            // if left and right tree satisfy the invariant
            if (data == null) {
                if (left == null) return right;
                if (right == null) return left;
            } else if (IsBalanced(left, right)) {
                return Construct(data, left, right);
            }

            if (Measure(left) < Measure(right)) {
                TNode rl = right.Left;
                TNode rr = right.Right;

                if (Measure(rl) <= Measure(rr))
                    return Create(right.Data, Create(data, left, rl), rr);

                // assert rl != null (because 1 <= GetHeight(rl) > GetHeight(rr))
                return Create(rl.Data,
                    Create(data, left, rl.Left),
                    Create(right.Data, rl.Right, rr));
            }

            TNode ll = left.Left;
            TNode lr = left.Right;

            if (Measure(ll) >= Measure(lr))
                return Create(left.Data, ll, Create(data, lr, right));

            // assert lr != null (because 1 <= GetHeight(lr) > GetHeight(ll))
            return Create(lr.Data,
                Create(left.Data, ll, lr.Left),
                Create(data, lr.Right, right));
        }

        public TNode Insert(TNode tree, TData data)
        {
            if (tree == null)
                return Construct(data, null, null);

            var comparer = Comparer<TData>.Default;
            int cmp = comparer.Compare(data, tree.Data);
            TNode result;

            if (cmp < 0) {
                result = Insert(tree.Left, data);
                if (result != tree.Left)
                    return Create(data, result, tree.Right);
            } else if (cmp > 0) {
                result = Insert(tree.Right, data);
                if (result != tree.Right)
                    return Create(data, tree.Left, result);
            }

            return tree;
        }

        bool IsBalanced(TNode left, TNode right)
        {
            int leftHeight = left.Height;
            int rightHeight = right.Height;

            return leftHeight > rightHeight
                ? leftHeight <= 2 * rightHeight
                : rightHeight <= 2 * leftHeight;
        }

        int Measure(TNode child) => child != null ? child.Height : 1;

        public TNode Delete(TNode tree, TData data)
        {
            if (tree == null)
                return null;

            var comparer = Comparer<TData>.Default;
            int cmp = comparer.Compare(data, tree.Data);
            TNode result;

            if (cmp < 0) {
                result = Delete(tree.Left, data);
                if (result != tree.Left)
                    return Create(data, result, tree.Right);
            } else if (cmp > 0) {
                result = Delete(tree.Right, data);
                if (result != tree.Right)
                    return Create(data, tree.Left, result);
            } else {
                return Create(null, tree.Left, tree.Right);

                // Alternative
                //TNode extremum;
                //var newRight = ExtractExtremum(tree.Right,
                //    out extremum, true);
                //return Create(extremum?.Data, tree.Left, newRight);
            }

            return tree;
        }

        public TNode FindExtremum(TNode tree, bool minimum)
        {
            while (tree != null) {
                TNode? node = minimum ? tree.Left : tree.Right;
                if (node == null)
                    return tree;
                tree = node;
            }

            return null;
        }

        public TNode ExtractExtremum(TNode tree,
            out TNode extracted,
            bool minimum)
        {
            if (tree != null) {
                TNode node;
                if (minimum) {
                    node = tree.Left;
                    if (node != null)
                        return Create(tree.Data,
                            ExtractExtremum(node, out extracted, true),
                            tree.Right);
                } else {
                    node = tree.Right;
                    if (node != null)
                        return Create(tree.Data,
                            tree.Left,
                            ExtractExtremum(node, out extracted, false));
                }
            }

            extracted = tree;
            return null;
        }

        public TNode InsertTree(TNode tree, TNode insertion)
        {
            if (tree == null)
                return insertion;
            if (insertion == null)
                return tree;

            TData data = insertion.Data;
            var comparer = Comparer<TData>.Default;
            int cmp = comparer.Compare(data, tree.Data);
            TNode result;

            if (cmp < 0) {
                result = Insert(tree.Left, data);
                if (result != tree.Left)
                    return Create(data, result, tree.Right);
            } else if (cmp > 0) {
                result = Insert(tree.Right, data);
                if (result != tree.Right)
                    return Create(data, tree.Left, result);
            }

            return tree;
        }

        public TNode Find(TNode top, TData data)
        {
            var comparer = Comparer<TData>.Default;
            TNode? current = top;
            while (current != null) {
                int cmp = comparer.Compare(data, current.Data);
                if (cmp < 0)
                    current = current.Left;
                else if (cmp > 0)
                    current = current.Right;
                else
                    return current;
            }

            return null;
        }

        public TNode Split(TNode tree, TData data,
            out TNode left, out TNode right)
        {
            if (tree == null) {
                left = right = null;
                return null;
            }

            var comparer = Comparer<TData>.Default;
            int cmp = comparer.Compare(data, tree.Data);
            TNode result;

            if (cmp < 0) {
                result = Split(tree.Left, data, out left, out right);
                right = Create(tree.Data, right, tree.Right);
            } else if (cmp > 0) {
                result = Split(tree.Right, data, out left, out right);
                left = Create(tree.Data, tree.Left, left);
            } else {
                result = tree;
                left = tree.Left;
                right = tree.Right;
            }

            return result;
        }

        public TNode Union(TNode node1, TNode node2)
        {
            if (node1 == null) return node2;
            if (node2 == null || node1 == node2) return node1;

            TNode splitL, splitR;
            Split(node2, node1.Data, out splitL, out splitR);
            return Create(node1.Data,
                Union(node1.Left, splitL),
                Union(node2.Right, splitR));
        }

        public TNode Intersection(TNode node1, TNode node2)
        {
            if (node1 == null || node2 == null) return null;
            if (node1 == node2) return node1;

            TNode splitL, splitR;

            // If one side of node1 is null, 
            // we don't need the split for that side of the intersection
            TNode? n = Split(node2, node1.Data, out splitL, out splitR);
            return Create(n == null ? null : node1.Data,
                Intersection(node1.Left, splitL),
                Intersection(node2.Right, splitR));
        }

        public TNode Difference(TNode node1, TNode node2)
        {
            if (node2 == null) return node1;
            if (node1 == null || node1 == node2) return null;

            TNode splitL, splitR;
            TNode? n = Split(node2, node1.Data, out splitL, out splitR);
            return Create(n == null ? node1.Data : null,
                Difference(node1.Left, splitL),
                Difference(node2.Right, splitR));
        }

        public TNode ExclusiveUnion(TNode node1, TNode node2)
        {
            if (node2 == null) return node1;
            if (node1 == null) return node2;
            if (node1 == node2) return null;

            TNode splitL, splitR;
            TNode? n = Split(node2, node1.Data, out splitL, out splitR);
            return Create(n == null ? node1.Data : null,
                ExclusiveUnion(node1.Left, splitL),
                ExclusiveUnion(node2.Right, splitR));
        }
    }
}