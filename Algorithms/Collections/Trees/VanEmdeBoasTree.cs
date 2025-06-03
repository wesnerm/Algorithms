using System.Numerics;

namespace Algorithms.Collections.Trees;

public class VanEmdeBoasTree : IEnumerable<int>
{
    const int MaxValue = int.MaxValue;
    readonly int m;
    readonly Dictionary<long, Node> map;
    readonly Node root;
    int counter;

    public VanEmdeBoasTree(int size, int capacity = 0)
    {
        m = int.Log2(size - 1) + 1;
        root = CreateNode(m, this);
        map = new Dictionary<long, Node>(capacity);
    }

    public int Count { get; private set; }

    public int Max => root.Max;

    public int Min => root.Min;

    public int[] Table => this.ToArray();

    public IEnumerator<int> GetEnumerator()
    {
        int i = -1;
        while ((i = Next(i)) < MaxValue)
            yield return i;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Next(int x) => root.Next(x, this);

    public int Previous(int x) => root.Previous(x, this);

    public bool Contains(int x) => root.Contains(x, this);

    public bool Insert(int x)
    {
        bool result = root.Insert(x, this);
        if (result) Count++;
        return result;
    }

    public bool Delete(int x)
    {
        bool result = root.Delete(x, this);
        if (result) Count--;
        return result;
    }

    Node Child(int id, int index, int newM = 0)
    {
        Node node;
        long key = ((long)id << m) + index;
        if (map.TryGetValue(key, out node))
            return node;

        if (newM != 0)
            map[key] = node = CreateNode(newM, this);
        return node;
    }

    static Node CreateNode(int m, VanEmdeBoasTree v) => m > 6 ? new VEBNode(m, v.counter++) : new LeafNode();

    abstract class Node
    {
        public abstract int Min { get; }
        public abstract int Max { get; }
        public abstract bool IsEmpty { get; }
        public abstract bool Contains(int x, VanEmdeBoasTree v);
        public abstract int Next(int x, VanEmdeBoasTree v);
        public abstract int Previous(int x, VanEmdeBoasTree v);
        public abstract bool Insert(int x, VanEmdeBoasTree v);
        public abstract bool Delete(int x, VanEmdeBoasTree v);

        public abstract int SameOrNext(int x, VanEmdeBoasTree v);
        public abstract int SameOrPrevious(int x, VanEmdeBoasTree v);
    }

    sealed class VEBNode : Node
    {
        readonly int id;
        readonly int m;
        Node aux;
        int min, max;

        public VEBNode(int m, int id)
        {
            this.id = id;
            this.m = m;
            min = MaxValue;
            max = -1;
        }

        public override int Min => min;

        public override int Max => max;

        public override bool IsEmpty => min > max;

        public override bool Contains(int x, VanEmdeBoasTree v)
        {
            if (x <= min) return x == min;
            if (x >= max) return x == max;
            int m2 = m >> 1;
            int lo = x & ((1 << m2) - 1);
            Node? child = v.Child(id, x >> m2);
            return child != null && child.Contains(lo, v);
        }

        public override int Next(int x, VanEmdeBoasTree v)
        {
            if (x < min) return min;
            if (x >= max) return MaxValue;

            int m2 = m >> 1;
            int i = x >> m2;
            int lo = x & ((1 << m2) - 1);

            Node? child = v.Child(id, i);
            if (child != null && lo < child.Max)
                return child.Next(lo, v) + (i << m2);

            int next = aux.Next(i, v);
            return v.Child(id, next).Min + (next << m2);
        }

        public override int Previous(int x, VanEmdeBoasTree v)
        {
            if (x > max) return max;
            if (x <= min) return -1;

            int m2 = m >> 1;
            int i = x >> m2;
            int lo = x & ((1 << m2) - 1);

            Node? child = v.Child(id, i);
            if (child != null && lo > child.Min)
                return child.Previous(lo, v) + (i << m2);

            int prev = aux.Previous(i, v);
            return prev >= 0 ? v.Child(id, prev).Max + (prev << m2) : min;
        }

        public override int SameOrNext(int x, VanEmdeBoasTree v)
        {
            if (x <= min) return min;
            if (x >= max) return x == max ? x : MaxValue;

            int m2 = m >> 1;
            int i = x >> m2;
            int lo = x & ((1 << m2) - 1);

            Node? child = v.Child(id, i);
            if (child != null)
                return child.SameOrNext(lo, v) + (i << m2);

            int next = aux.Next(i, v);
            return v.Child(id, next).Min + (next << m2);
        }

        public override int SameOrPrevious(int x, VanEmdeBoasTree v)
        {
            if (x >= max) return max;
            if (x <= min) return x == min ? x : -1;

            int m2 = m >> 1;
            int i = x >> m2;
            int lo = x & ((1 << m2) - 1);

            Node? child = v.Child(id, i);
            if (child != null)
                return child.SameOrPrevious(lo, v) + (i << m2);

            int prev = aux.Previous(i, v);
            return prev >= 0 ? v.Child(id, prev).Max + (prev << m2) : min;
        }

        public override bool Insert(int x, VanEmdeBoasTree v)
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

            if (aux == null)
                aux = CreateNode(m - m2, v);

            Node child = v.Child(id, i, m2);
            if (child.IsEmpty) aux.Insert(i, v);
            return child.Insert(x & ((1 << m2) - 1), v);
        }

        public override bool Delete(int x, VanEmdeBoasTree v)
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
                min = x = (aux.Min << m2) + v.Child(id, aux.Min).Min;

            int i = x >> m2;
            Node? child = v.Child(id, i);
            if (child == null || !child.Delete(x & ((1 << m2) - 1), v))
                return false;

            if (child.IsEmpty)
                aux.Delete(i, v);

            if (x == max)
                max = aux.IsEmpty ? Min : (aux.Max << m2) + v.Child(id, aux.Max).Max;
            return true;
        }
    }

    sealed class LeafNode : Node
    {
        long bitset;

        public override int Min => bitset != 0 ? (int)long.Log2(bitset & -bitset) : MaxValue;

        public override int Max => bitset != 0 ? (int)long.Log2(bitset) : -1;

        public override bool IsEmpty => bitset == 0;

        public override bool Insert(int x, VanEmdeBoasTree v)
        {
            if (!checked((ulong)x < 64) || (bitset & (1L << x)) != 0)
                return false;
            bitset |= 1L << x;
            return true;
        }

        public override bool Delete(int x, VanEmdeBoasTree v)
        {
            if (checked((ulong)x < 64) && (bitset & (1L << x)) == 0)
                return false;
            bitset &= ~(1L << x);
            return true;
        }

        public override int Next(int x, VanEmdeBoasTree v)
        {
            if (x >= 63)
                return MaxValue;
            long mask = x >= 0 ? -1L << (x + 1) : -1;
            mask &= bitset;
            return mask != 0 ? (int)long.Log2(mask & -mask) : MaxValue;
        }

        // ~(x-1) = -x
        public override int Previous(int x, VanEmdeBoasTree v)
        {
            if (x <= 0)
                return -1;
            long mask = x < 64 ? (1L << x) - 1 : -1;
            return Log2(bitset & mask);
        }

        public override int SameOrNext(int x, VanEmdeBoasTree v)
        {
            if (x > 63) return MaxValue;
            long mask = x > 0 ? -1L << x : -1;
            mask &= bitset;
            return mask != 0 ? (int)BitOperations.Log2((uint)(mask & -mask)) : MaxValue;
        }

        public override int SameOrPrevious(int x, VanEmdeBoasTree v)
        {
            if (x < 0) return -1;
            long mask = x < 63 ? (2L << x) - 1 : -1;
            return Log2(bitset & mask);
        }

        public override bool Contains(int x, VanEmdeBoasTree v) =>
            (bitset & (1L << x)) != 0 && unchecked((ulong)x < 64);

        private static int Log2(long size) => size > 0 ? BitOperations.Log2((ulong)size) : -1;

    }
}