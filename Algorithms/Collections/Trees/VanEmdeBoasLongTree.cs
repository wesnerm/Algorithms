namespace Algorithms.Collections.Trees;

public class VanEmdeBoasLongTree : IEnumerable<long>
{
    const long MaxValue = long.MaxValue;
    readonly int m;
    readonly Dictionary<long, Node> map;
    readonly Node root;
    int counter;

    public VanEmdeBoasLongTree(int size, int capacity = 0)
    {
        m = Log2(size - 1) + 1;
        root = CreateNode(m, this);
        map = new Dictionary<long, Node>(capacity);
    }

    public int Count { get; private set; }

    public long Max => root.Max;

    public long Min => root.Min;

    public long[] Table => this.ToArray();

    public IEnumerator<long> GetEnumerator()
    {
        long i = -1;
        while ((i = Next(i)) < MaxValue)
            yield return i;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public long Next(long x) => root.Next(x, this);

    public long Previous(long x) => root.Previous(x, this);

    public bool Contains(long x) => root.Contains(x, this);

    public bool Insert(long x)
    {
        bool result = root.Insert(x, this);
        if (result) Count++;
        return result;
    }

    public bool Delete(long x)
    {
        bool result = root.Delete(x, this);
        if (result) Count--;
        return result;
    }

    Node Child(int id, long index, int newM = 0)
    {
        Node node;
        long key = ((long)id << m) + index;
        if (map.TryGetValue(key, out node))
            return node;

        if (newM != 0)
            map[key] = node = CreateNode(newM, this);
        return node;
    }

    static Node CreateNode(int m, VanEmdeBoasLongTree v) => m > 6 ? new VEBNode(m, v.counter++) : new LeafNode();

    abstract class Node
    {
        public abstract long Min { get; }
        public abstract long Max { get; }
        public abstract bool IsEmpty { get; }
        public abstract bool Contains(long x, VanEmdeBoasLongTree v);
        public abstract long Next(long x, VanEmdeBoasLongTree v);
        public abstract long Previous(long x, VanEmdeBoasLongTree v);
        public abstract bool Insert(long x, VanEmdeBoasLongTree v);
        public abstract bool Delete(long x, VanEmdeBoasLongTree v);

        public abstract long SameOrNext(long x, VanEmdeBoasLongTree v);
        public abstract long SameOrPrevious(long x, VanEmdeBoasLongTree v);
    }

    sealed class VEBNode : Node
    {
        readonly int id;
        readonly int m;
        Node aux;
        long min, max;

        public VEBNode(int m, int id)
        {
            this.id = id;
            this.m = m;
            min = MaxValue;
            max = -1;
        }

        public override long Min => min;

        public override long Max => max;

        public override bool IsEmpty => min > max;

        public override bool Contains(long x, VanEmdeBoasLongTree v)
        {
            if (x <= min) return x == min;
            if (x >= max) return x == max;
            int m2 = m >> 1;
            long lo = x & ((1 << m2) - 1);
            Node? child = v.Child(id, x >> m2);
            return child != null && child.Contains(lo, v);
        }

        public override long Next(long x, VanEmdeBoasLongTree v)
        {
            if (x < min) return min;
            if (x >= max) return MaxValue;

            int m2 = m >> 1;
            long i = x >> m2;
            long lo = x & ((1 << m2) - 1);

            Node? child = v.Child(id, i);
            if (child != null && lo < child.Max)
                return child.Next(lo, v) + (i << m2);

            long next = aux.Next(i, v);
            return v.Child(id, next).Min + (next << m2);
        }

        public override long Previous(long x, VanEmdeBoasLongTree v)
        {
            if (x > max) return max;
            if (x <= min) return -1;

            int m2 = m >> 1;
            long i = x >> m2;
            long lo = x & ((1 << m2) - 1);

            Node? child = v.Child(id, i);
            if (child != null && lo > child.Min)
                return child.Previous(lo, v) + (i << m2);

            long prev = aux.Previous(i, v);
            return prev >= 0 ? v.Child(id, prev).Max + (prev << m2) : min;
        }

        public override long SameOrNext(long x, VanEmdeBoasLongTree v)
        {
            if (x <= min) return min;
            if (x >= max) return x == max ? x : MaxValue;

            int m2 = m >> 1;
            long i = x >> m2;
            long lo = x & ((1 << m2) - 1);

            Node? child = v.Child(id, i);
            if (child != null)
                return child.SameOrNext(lo, v) + (i << m2);

            long next = aux.Next(i, v);
            return v.Child(id, next).Min + (next << m2);
        }

        public override long SameOrPrevious(long x, VanEmdeBoasLongTree v)
        {
            if (x >= max) return max;
            if (x <= min) return x == min ? x : -1;

            int m2 = m >> 1;
            long i = x >> m2;
            long lo = x & ((1 << m2) - 1);

            Node? child = v.Child(id, i);
            if (child != null)
                return child.SameOrPrevious(lo, v) + (i << m2);

            long prev = aux.Previous(i, v);
            return prev >= 0 ? v.Child(id, prev).Max + (prev << m2) : min;
        }

        public override bool Insert(long x, VanEmdeBoasLongTree v)
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
            long i = x >> m2;

            if (aux == null)
                aux = CreateNode(m - m2, v);

            Node child = v.Child(id, i, m2);
            if (child.IsEmpty) aux.Insert(i, v);
            return child.Insert(x & ((1 << m2) - 1), v);
        }

        public override bool Delete(long x, VanEmdeBoasLongTree v)
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

            long i = x >> m2;
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

        public override long Min => bitset != 0 ? Log2(bitset & -bitset) : MaxValue;

        public override long Max => bitset != 0 ? Log2(bitset) : -1;

        public override bool IsEmpty => bitset == 0;

        public override bool Insert(long x, VanEmdeBoasLongTree v)
        {
            if (!checked((ulong)x < 64) || (bitset & (1L << (int)x)) != 0)
                return false;
            bitset |= 1L << (int)x;
            return true;
        }

        public override bool Delete(long x, VanEmdeBoasLongTree v)
        {
            if (checked((ulong)x < 64) && (bitset & (1L << (int)x)) == 0)
                return false;
            bitset &= ~(1L << (int)x);
            return true;
        }

        public override long Next(long x, VanEmdeBoasLongTree v)
        {
            if (x >= 63)
                return MaxValue;
            long mask = x >= 0 ? -1L << ((int)x + 1) : -1;
            mask &= bitset;
            return mask != 0 ? Log2(mask & -mask) : MaxValue;
        }

        // ~(x-1) = -x
        public override long Previous(long x, VanEmdeBoasLongTree v)
        {
            if (x <= 0)
                return -1;
            long mask = x < 64 ? (1L << (int)x) - 1 : -1;
            return Log2(bitset & mask);
        }

        public override long SameOrNext(long x, VanEmdeBoasLongTree v)
        {
            if (x > 63) return MaxValue;
            long mask = x > 0 ? -1L << (int)x : -1;
            mask &= bitset;
            return mask != 0 ? Log2(mask & -mask) : MaxValue;
        }

        public override long SameOrPrevious(long x, VanEmdeBoasLongTree v)
        {
            if (x < 0) return -1;
            long mask = x < 63 ? (2L << (int)x) - 1 : -1;
            return Log2(bitset & mask);
        }

        public override bool Contains(long x, VanEmdeBoasLongTree v) =>
            (bitset & (1L << (int)x)) != 0 && unchecked((ulong)x < 64);
    }
}