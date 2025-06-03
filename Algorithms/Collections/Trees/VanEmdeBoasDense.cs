using System.Numerics;

namespace Algorithms.Collections.Trees;

public class VanEmdeBoasDense : IEnumerable<int>
{
    const int MaxValue = int.MaxValue;
    readonly int m;
    readonly Node root;

    public VanEmdeBoasDense(int size)
    {
        m = Log2(size - 1) + 1;
        root = CreateNode(m);
    }

    public int Count { get; private set; }

    public int Max => root.Max;

    public int Min => root.Min;

    public int[] Table => this.ToArray();

    public static int Log2(long size) => size > 0 ? BitOperations.Log2((ulong)size) : -1;

    public IEnumerator<int> GetEnumerator()
    {
        int i = -1;
        while ((i = Next(i)) < MaxValue)
            yield return i;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Next(int x) => root.Next(x);

    public int Previous(int x) => root.Previous(x);

    public bool Contains(int x) => root.Contains(x);

    public bool Insert(int x)
    {
        bool result = root.Insert(x);
        if (result) Count++;
        return result;
    }

    public bool Delete(int x)
    {
        bool result = root.Delete(x);
        if (result) Count--;
        return result;
    }

    static Node CreateNode(int m) => m > 6 ? new VEBNode(m) : new LeafNode();

    abstract class Node
    {
        public abstract int Min { get; }
        public abstract int Max { get; }
        public abstract bool IsEmpty { get; }
        public abstract bool Contains(int x);
        public abstract int Next(int x);
        public abstract int Previous(int x);
        public abstract bool Insert(int x);
        public abstract bool Delete(int x);
    }

    sealed class VEBNode : Node
    {
        readonly int m;
        Node aux;
        Node[] children;
        int min, max;

        public VEBNode(int m)
        {
            this.m = m;
            min = MaxValue;
            max = -1;
        }

        public override int Min => min;

        public override int Max => max;

        public override bool IsEmpty => min > max;

        public override bool Contains(int x)
        {
            if (x <= min) return x == min;
            if (x >= max) return x == max;
            int m2 = m >> 1;
            int lo = x & ((1 << m2) - 1);
            Node? child = children[x >> m2];
            return child != null && child.Contains(lo);
        }

        public override int Next(int x)
        {
            if (x < min) return min;
            if (x >= max) return MaxValue;

            int m2 = m >> 1;
            int i = x >> m2;
            int lo = x & ((1 << m2) - 1);

            Node? child = children[i];
            if (child != null && lo < child.Max)
                return child.Next(lo) + (i << m2);

            int next = aux.Next(i);
            return children[next].Min + (next << m2);
        }

        public override int Previous(int x)
        {
            if (x > max) return max;
            if (x <= min) return -1;

            int m2 = m >> 1;
            int i = x >> m2;
            int lo = x & ((1 << m2) - 1);

            Node? child = children[i];
            if (child != null && lo > child.Min)
                return child.Previous(lo) + (i << m2);

            int prev = aux.Previous(i);
            return prev >= 0 ? children[prev].Max + (prev << m2) : min;
        }

        public override bool Insert(int x)
        {
            if (min > max) {
                min = max = x;
                return true;
            }

            if (x <= min) {
                if (x == min) return false;
                Swap(ref x, ref min);
            }

            if (x >= max) // Can't return here because item might not have been stored if max=min
                max = x;

            int m2 = m >> 1;
            int i = x >> m2;

            if (children == null) {
                children = new Node[1 << (m - m2)];
                aux = CreateNode(m - m2);
            }

            Node child = children[i] ?? (children[i] = CreateNode(m2));
            if (child.IsEmpty) aux.Insert(i);
            return child.Insert(x & ((1 << m2) - 1));
        }

        public override bool Delete(int x)
        {
            if (x < min)
                return false;

            if (min == max) {
                if (min != x) return false;
                min = MaxValue;
                max = -1;
                return true;
            }

            int m2 = m >> 1;
            if (x == min)
                min = x = (aux.Min << m2) + children[aux.Min].Min;

            int i = x >> m2;
            Node? child = children[i];
            if (child == null || !child.Delete(x & ((1 << m2) - 1)))
                return false;

            if (child.IsEmpty)
                aux.Delete(i);

            if (x == max)
                max = aux.IsEmpty ? Min : (aux.Max << m2) + children[aux.Max].Max;
            return true;
        }
    }

    sealed class LeafNode : Node
    {
        long bitset;

        public override int Min => bitset != 0 ? Log2(bitset & -bitset) : MaxValue;

        public override int Max => bitset != 0 ? Log2(bitset) : -1;

        public override bool IsEmpty => bitset == 0;

        public override bool Insert(int x)
        {
            if (!checked((ulong)x < 64) || (bitset & (1L << x)) != 0)
                return false;
            bitset |= 1L << x;
            return true;
        }

        public override bool Delete(int x)
        {
            if (checked((ulong)x < 64) && (bitset & (1L << x)) == 0)
                return false;
            bitset &= ~(1L << x);
            return true;
        }

        public override int Next(int x)
        {
            if (x >= 63)
                return MaxValue;
            long mask = x >= 0 ? -1L << (x + 1) : -1;
            mask &= bitset;
            return mask != 0 ? (int)BitOperations.Log2((uint)(mask & -mask)) : MaxValue;
        }

        // ~(x-1) = -x
        public override int Previous(int x)
        {
            if (x <= 0)
                return -1;
            long mask = x < 64 ? (1L << x) - 1 : -1;
            return Log2(bitset & mask);
        }

        public override bool Contains(int x) => (bitset & (1L << x)) != 0 && unchecked((ulong)x < 64);
    }
}