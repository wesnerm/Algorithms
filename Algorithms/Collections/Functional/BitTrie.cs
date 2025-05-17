#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2015, Wesner Moise.
/////////////////////////////////////////////////////////////////////////////

#endregion

#if false
namespace Algorithms.Collections;

internal abstract class BitTrie<K, T>
    where T : BitTrie<K, T>, IEnumerable<K>
{
#region Variables

    private const uint UnchangeableBit = 0x80000000;
    private const uint CountMask = 0x00000fff;
    private const uint HashMask = 0x7ffff000;
    private const int HashShift = 12;
    private uint _flags;

#endregion

#region Construction

    [DebuggerStepThrough]
    protected BitTrie()
    {
    }

    protected virtual TrieType Type { get { return 0; } }

    protected virtual int Hashcode { get { return 0; } }

    protected virtual int Bit { get { return 0;  } }

#endregion

#region Properties

    // DONE
    [DebuggerNonUserCode]
    public T Clone()
    {
        var self = (T) MemberwiseClone();
        self._flags = 0;
        return self;
    }

    [DebuggerNonUserCode]
    public static void Seal(BitTrie<K, T> top)
    {
        if (top == null || IsSealed(top))
            return;

        var hash = (uint)top.Key.GetHashCode() ^ (uint)top.NodeGetHashCode();
        hash ^= hash << HashShift;

        uint count = 1;
        uint tmpFlags;

        if (top._left != null)
        {
            Seal(top._left);
            tmpFlags = top._left._flags;
            hash ^= tmpFlags;
            count += tmpFlags & CountMask;
        }

        if (top._right != null)
        {
            Seal(top._right);
            tmpFlags = top._right._flags;
            hash ^= tmpFlags;
            count += tmpFlags & CountMask;
        }

        if (count >= CountMask)
            count = 0;

        // REVIEW: The quickly varying bits are all at the top

        // Fill counts and hash code
        top._flags = UnchangeableBit
                    | (hash & HashMask)
                    | (count & CountMask);

        Debug.Assert(IsSealed(top));
        top.OnSeal();
    }

    protected virtual void OnSeal()
    {
    }

    // DONE
    [DebuggerNonUserCode]
    public static bool IsSealed(BitTrie<K, T> top)
    {
        if (top == null)
            return true;

        bool isSealed = top._flags >= UnchangeableBit;
        Debug.Assert(!isSealed || top._left == null || top._left._flags >= UnchangeableBit);
        Debug.Assert(!isSealed || top._right == null || top._right._flags >= UnchangeableBit);
        return isSealed;
    }

    [DebuggerNonUserCode]
    public static int TreeHashCode(BitTrie<K, T> top)
    {
        if (top == null) return 0;
        if (!IsSealed(top)) Seal(top);

        var hash = unchecked((int) top._flags);
        return hash ^ (hash >> HashShift);
    }

    [DebuggerNonUserCode]
    public static int Count(BitTrie<K, T> top)
    {
        if (top == null) return 0;
        if (!IsSealed(top)) Seal(top);
        return CountCore(top);
    }

    [DebuggerNonUserCode]
    private static int CountCore(BitTrie<K, T> top)
    {
        if (top == null)
            return 0;

        var count = (int) (top._flags & CountMask);
        if (count == 0)
            count = CountCore(top._left) + CountCore(top._right) + 1;
        return count;
    }

    [DebuggerNonUserCode]
    public override int GetHashCode()
    {
        return TreeHashCode(this);
    }

#endregion

#region Overrides

    [DebuggerStepThrough]
    public override bool Equals(object key)
    {
        return TreeEquals((T) this, key as T);
    }

    [DebuggerStepThrough]
    public override string ToString()
    {
        return string.Join(this, ",");
    }

#endregion

#region Methods

    [DebuggerStepThrough]
    protected static T CloneLeft(T self, T left)
    {
        if (IsSealed(self))
        {
            if (self._left == left)
                return self;
            self = self.Clone();
        }
        self._left = left;
        return self;
    }

    [DebuggerStepThrough]
    protected static T CloneRight(T self, T right)
    {
        if (IsSealed(self))
        {
            if (self._right == right)
                return self;
            self = self.Clone();
        }
        self._right = right;
        return self;
    }

    [Conditional("DEBUG")]
    private void Validate()
    {
        if (IsSealed(this))
        {
            Debug.Assert(IsSealed(_left));
            Debug.Assert(IsSealed(_right));
        }
    }

    [DebuggerStepThrough]
    public static bool Contains(T top, K key)
    {
        return GetValue(top, key) != null;
    }

    [DebuggerStepThrough]
    public static T GetValue(T top, K key)
    {
        T current = top;
        int keyHashcode = key.GetHashCode();
        while (current != null)
        {
            int cmp = current.CompareKey(key, keyHashcode);
            if (cmp > 0)
                current = current._left;
            else if (cmp < 0)
                current = current._right;
            else
                return current;
        }
        return null;
    }

    [DebuggerStepThrough]
    [Pure]
    public static T Insert(T top, T insertion)
    {
        if (top == null)
            return insertion;

        top.Validate();

        T result;
        int cmp = top.CompareKey(insertion.Key);
        if (cmp > 0)
        {
            result = Clone(top, Insert(top._left, insertion), top._right);
        }
        else if (cmp < 0)
        {
            result = Clone(top, top._left, Insert(top._right, insertion));
        }
        else
        {
            if (top.NodeEquals(insertion))
                return top;
            Debug.Assert(!IsSealed(insertion));
            insertion._left = top._left;
            insertion._right = top._right;
            insertion.Validate();
            return insertion;
        }
        return result;
    }

    public static T InsertCore(T top, T insertion, Func<T, T, T> func)
    {
        if (top == null)
            return insertion;

        top.Validate();

        T result;
        int cmp = top.CompareKey(insertion.Key);
        if (cmp > 0)
        {
            result = Clone(top, InsertCore(top._left, insertion, func), top._right);
        }
        else if (cmp < 0)
        {
            result = Clone(top, top._left, InsertCore(top._right, insertion, func));
        }
        else
        {
            T added = func(top, insertion);
            if (added.NodeEquals(top))
                return top;
            result = Clone(added, top._left, top._right);
        }
        return result;
    }

    public static T Clone(T top, T leftNode, T rightNode)
    {
        var self = top;
        if (IsSealed(top))
        {
            if (top._left == leftNode && top._right == rightNode)
                return top;
            self = self.Clone();
        }
        self._left = leftNode;
        self._right = rightNode;
        return self;
    }

    [DebuggerStepThrough]
    [Pure]
    public static T Remove(T top, K key)
    {
        T result = RemoveCore(top, key, key.GetHashCode());
        if (result != null) result.Validate();
        Seal(result);
        return result;
    }

    private static T RemoveCore(T top, K delete, int keyHashCode)
    {
        if (top == null)
            return top;
        int cmp = top.CompareKey(delete, keyHashCode);
        if (cmp > 0)
            return CloneLeft(top, RemoveCore(top._left, delete, keyHashCode));
        if (cmp < 0)
            return CloneRight(top, RemoveCore(top._right, delete, keyHashCode));
        return Join(top._left, top._right);
    }

    [DebuggerStepThrough]
    [Pure]
    public static T Filter(T map, Func<K, bool> filter)
    {
        if (map == null)
            return null;

        T left = Filter(map._left, filter);
        T right = Filter(map._right, filter);
        return filter(map.Key) ? Join(left, right) : Clone(map, left, right);
    }

    [DebuggerStepThrough]
    public static BitTrie<K,T> Filter(T map, Func<T, bool> filter)
    {
        if (map == null)
            return null;

        T left = Filter(map._left, filter);
        T right = Filter(map._right, filter);
        return filter(map) ? Join(left, right) : Clone(map, left, right);
    }

    public static BitTrie<K,T> Join(T map1, T map2)
    {
        if (map1 == null) return map2;
        if (map2 == null) return map1;
        if (map1.Hashcode > map2.Hashcode)
            Swap(ref map1, ref map2);
        return new TrieNode
        {
            Left = map1,
            Right = map2,
        };
    }

    [DebuggerNonUserCode]
    private IEnumerable<K> GetEnumerator()
    {
        BitTrie<K,T> current = this;
        var stack = new Stack<BitTrie<K,T>>();
        while (true)
        {
            
            var node = current as TrieNode;
            if (node != null)
            {
                stack.Push(node.Right);
                current = node.Left;
                continue;
            }

            var leaf = current as TrieLeaf;
            if (leaf != null)
                yield return leaf.Key;

            if (stack.Count == 0)
                yield break;

            current = stack.Pop();
        }
    }

#endregion

#region SET OPERATIONS
    private static int FindHighestBitDifference(int hashcode1, int hashcode2)
    {
        int xor = hashcode1 ^ hashcode2;
        return Utility.Log2(xor);
    }

    private static TrieNode MakeNode(BitTrie<K,T> t1, BitTrie<K,T> t2, int bit, int hashcode)
    {
        Debug.Assert(bit>=-1);
        hashcode &= -1 << (bit + 1);
        return new TrieNode
        {
            Left = t1,
            Right = t2,
        };
    }

    private static TrieNode MakeNode(BitTrie<K,T> t1, BitTrie<K,T> t2)
    {
        var hashcode = t1.Hashcode;
        int bit = FindHighestBitDifference(hashcode, t2.Hashcode);
        return MakeNode(t1, t2, bit, hashcode);
    }

    static int Compare(TrieLeaf t1, TrieLeaf t2)
    {
        throw new NotImplementedException();
    }

    [Pure]
    public static BitTrie<K, T> Union(BitTrie<K,T> trie1, BitTrie<K,T> trie2)
    {
        if (trie1 == null)
            return trie2;

        var type1 = trie1.Type;
        if (type1 == TrieType.Empty)
            return trie2;

        if (trie2 == null)
            return trie1;

        var type2 = trie2.Type;
        if (type1 > type2)
            return Union(trie2, trie1);

        int hashcode1 = trie1.Hashcode;
        int hashcode2 = trie2.Hashcode;

        // leaf leaf
        if (type2 == TrieType.Leaf)
        {
            var cmp = (long) hashcode1 - (long) hashcode2; 
            if (cmp == 0)
            {
                cmp = Compare(trie1, trie2);
                if (cmp == 0)
                    return trie1;
            }
            return cmp <= 0 ? MakeNode(trie1, trie2) : MakeNode(trie2, trie1);
        }

        // leaf node
        int bit = FindHighestBitDifference(hashcode1, hashcode2);
        var node2 = (TrieNode)(BitTrie<K, T>)trie2;
        if (type1 == TrieType.Leaf)
        {
            if (bit < 0)
            {
                if (Compare((TrieLeaf) (BitTrie<K,T>) trie1, (TrieLeaf) node2.Left) <= 0)
                    return MakeNode(trie1, trie2, bit, hashcode1);
                return MakeNode(node2.Left, Union(trie1, node2.Right), bit, hashcode1);
            }

            int bit2 = node2.Bit;
            if (bit <= bit2)
            {
                // use same bit and hashcode2
                if ((trie1.Hashcode & (1 << bit)) == 0)
                    return MakeNode(Union(trie1, node2.Left), node2.Right, bit2, hashcode2);
                return MakeNode(node2.Left, Union(trie1, node2.Right), bit2, hashcode2);
            }

            if ((trie1.Hashcode & (1 << bit)) == 0)
                return MakeNode(trie1, trie2, bit, hashcode1);
            return MakeNode(trie2, trie1, bit, hashcode1);
        }

        // Two nodes
        var node1 = (TrieNode) trie1;
        if (hashcode2 < hashcode1 || hashcode1 == hashcode2 && node1.Bit > node2.Bit)
            Swap(ref node1, ref node2);

        if (bit <= node1.Bit && bit <= node2.Bit)
        {
            // Already sorted
            return MakeNode(node1, node2, bit, hashcode1);
        }

        if (bit < node2.Bit)
        {
            if ((hashcode2 & (1<<bit)) == 0)
                return MakeNode(Union(node1.Left, node2), node1.Right, bit, hashcode1);
            return MakeNode(node1.Left, Union(node1.Right, node2), bit, hashcode1);
        }

        var result = MakeNode(Union(node1.Left, node2.Left), Union(node1.Right, node2.Right), bit, hashcode1);
        if (result.Left == node1.Left && result.Right == node1.Right)
            result = node1;
        else if (result.Left == node2.Left && result.Right == node2.Right)
            result = node2;

        return result;
    }

    [Pure]
    public static BitTrie<K, T> Merge(BitTrie<K, T> trie1, BitTrie<K, T> trie2,
        Func<BitTrie<K, T>, BitTrie<K, T>, BitTrie<K, T>> merge, bool swapped = false)
    {
        if (trie1==trie2 || trie1 == null || trie2 == null || trie1.Type == TrieType.Empty)
            return swapped ? merge(trie2, trie1) : merge(trie1, trie2);

        var type1 = trie1.Type;
        var type2 = trie2.Type;
        if (type1 > type2)
            return Merge(trie2, trie1, merge, !swapped);

        int hashcode1 = trie1.Hashcode;
        int hashcode2 = trie2.Hashcode;
        int bit = FindHighestBitDifference(trie1.Hashcode, trie2.Hashcode);

        // leaf leaf
        if (type2 == TrieType.Leaf)
        {
            if (hashcode1 > hashcode2 || bit < 0 && Compare(trie1, trie2) > 0)
                Swap(ref trie1, ref trie2);
            return MakeNode(trie1, trie2, bit, hashcode1);
        }

        // leaf node
        var node2 = (TrieNode)(BitTrie<K, T>)trie2;
        if (type1 == TrieType.Leaf)
        {
            if (bit < 0)
            {
                if (Compare(trie1, node2.Left) <= 0)
                    return MakeNode(trie1, trie2, bit, hashcode1);
                return MakeNode(node2.Left, Union(trie1, node2.Right), bit, hashcode1);
            }

            int bit2 = node2.Bit;
            if (bit <= bit2)
            {
                // use same bit and hashcode2
                if ((trie1.Hashcode & (1 << bit)) == 0)
                    return MakeNode(Union(trie1, node2.Left), node2.Right, bit2, hashcode2);
                return MakeNode(node2.Left, Union(trie1, node2.Right), bit2, hashcode2);
            }

            if ((trie1.Hashcode & (1 << bit)) == 0)
                return MakeNode(trie1, trie2, bit, hashcode1);
            return MakeNode(trie2, trie1, bit, hashcode1);
        }

        // Two nodes
        var node1 = (TrieNode)(BitTrie<K, T>)trie1;
        if (hashcode2 < hashcode1 || hashcode1 == hashcode2 && node1.Bit > node2.Bit)
            Swap(ref node1, ref node2);

        if (bit <= node1.Bit && bit <= node2.Bit)
        {
            // Already sorted
            return MakeNode(node1, node2, bit, hashcode1);
        }

        if (bit < node2.Bit)
        {
            if ((hashcode2 & (1 << bit)) == 0)
                return MakeNode(Union(node1.Left, node2), node1.Right, bit, hashcode1);
            return MakeNode(node1.Left, Union(node1.Right, node2), bit, hashcode1);
        }

        var result = MakeNode(Union(node1.Left, node2.Left), Union(node1.Right, node2.Right), bit, hashcode1);
        if (result.Left == node1.Left && result.Right == node1.Right)
            result = node1;
        else if (result.Left == node2.Left && result.Right == node2.Right)
            result = node2;

        return result;
    }

    [Pure]
    public static bool Subset(BitTrie<K, T> subset, BitTrie<K, T> parent)
    {
        throw new NotImplementedException();
    }

    public static bool Overlaps(BitTrie<K, T> subset, BitTrie<K, T> parent)
    {
        throw new NotImplementedException();
    }

    [Pure]
    public static T ExclusiveUnion(BitTrie<K, T> map1, BitTrie<K, T> map2)
    {
        throw new NotImplementedException();
    }

    [Pure]
    public static T Intersection(BitTrie<K, T> map1, BitTrie<K, T> map2)
    {
        throw new NotImplementedException();
    }

    [DebuggerStepThrough]
    public static T Difference(BitTrie<K, T> map1, BitTrie<K, T> map2)
    {
        throw new NotImplementedException();
    }
#endregion

    private class TrieEmpty : BitTrie<K,T>
    {
    }

    private class TrieNode : BitTrie<K,T>
    {
        private int _hashCode;
        private sbyte _bits;
        private int _count;
        public BitTrie<K, T> Left;
        public BitTrie<K, T> Right;

        public TrieNode(BitTrie<K, T> t1, BitTrie<K, T> t2, int bit)
        {
            Debug.Assert(bit >= -1);
            _bits = unchecked( (sbyte)bit );
            _hashCode = t1.Hashcode -1 << (bit + 1);
            Left = t1;
            Right = t2;
        }

        public TrieNode(BitTrie<K, T> t1, BitTrie<K, T> t2)
            : this(t1, t2, FindHighestBitDifference(t1.Hashcode, t2.Hashcode))
        {
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    private class TrieLeaf : BitTrie<K,T>
    {
        private int _hashcode;
        public K Key;

        public override int GetHashCode()
        {
            return _hashcode;
        }

    }

    protected enum TrieType
    {
        Empty,
        Leaf,
        Node,
    }
}
#endif