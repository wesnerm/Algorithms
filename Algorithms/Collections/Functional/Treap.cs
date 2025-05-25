#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2006 - 2008, Wesner Moise.
/////////////////////////////////////////////////////////////////////////////

using System.Runtime.CompilerServices;

#endregion

namespace Algorithms.Collections;

abstract class Treap<K, T>
    where T : Treap<K, T>
{
    #region Variables

    const uint UnchangeableBit = 0x80000000;
    const uint CountMask = 0x00000fff;
    const uint HashMask = 0x7ffff000;
    const int HashShift = 12;
    readonly int _priority;
    uint _flags;
    T _left;
    T _right;

    #endregion

    #region Construction

    [DebuggerStepThrough]
    protected Treap(K key)
    {
        int hashcode = key.GetHashCode();
        _priority = BitTools.Reverse(hashcode);
        Key = key;
        // this.keyHashcode = key.GetHashCode();
    }

    protected abstract K Key { get; set; }

    protected T Left => _left;

    protected T Right => _right;

    // PERFORMANCE
    //Hashed, Individual 20.6 secs
    //Hashed, Group 5.3 secs
    //Sorted, Group 10 secs
    //Sorted, Individual 23 
    //SavedHash Group, 5.0,5.6,5.4  (normal,convert1, convert2)
    //SavedHash, Individual 17,18,19

    #endregion

    #region Properties

    // DONE
    [DebuggerNonUserCode]
    public T Clone()
    {
        var self = (T)MemberwiseClone();
        self._flags = 0;
        return self;
    }

    [DebuggerNonUserCode]
    public static void Seal(Treap<K, T> top)
    {
        if (top == null || IsSealed(top))
            return;

        uint hash = (uint)top.Key.GetHashCode() ^ (uint)top.NodeGetHashCode();
        hash ^= hash << HashShift;

        uint count = 1;
        uint tmpFlags;

        if (top._left != null) {
            Debug.Assert(top._left._priority <= top._priority);
            Seal(top._left);
            tmpFlags = top._left._flags;
            hash ^= tmpFlags;
            count += tmpFlags & CountMask;
        }

        if (top._right != null) {
            Debug.Assert(top._right._priority <= top._priority);
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

    protected virtual void OnSeal() { }

    // DONE
    [DebuggerNonUserCode]
    public static bool IsSealed(Treap<K, T> top)
    {
        if (top == null)
            return true;

        bool isSealed = top._flags >= UnchangeableBit;
        Debug.Assert(!isSealed || top._left == null || top._left._flags >= UnchangeableBit);
        Debug.Assert(!isSealed || top._right == null || top._right._flags >= UnchangeableBit);
        return isSealed;
    }

    [DebuggerNonUserCode]
    public static int TreeHashCode(Treap<K, T> top)
    {
        if (top == null) return 0;
        if (!IsSealed(top)) Seal(top);

        int hash = unchecked((int)top._flags);
        return hash ^ (hash >> HashShift);
    }

    [DebuggerNonUserCode]
    public static int Count(Treap<K, T> top)
    {
        if (top == null) return 0;
        if (!IsSealed(top)) Seal(top);
        return CountCore(top);
    }

    [DebuggerNonUserCode]
    static int CountCore(Treap<K, T> top)
    {
        if (top == null)
            return 0;

        int count = (int)(top._flags & CountMask);
        if (count == 0)
            count = CountCore(top._left) + CountCore(top._right) + 1;
        return count;
    }

    [DebuggerNonUserCode]
    protected static int Depth(T top)
    {
        if (top == null)
            return 0;

        int depth = Math.Max(Depth(top._left), Depth(top._right)) + 1;
        return depth;
    }

    [DebuggerNonUserCode]
    public static bool TreeEquals(T node1, T node2)
    {
        if (node1 == node2) return true;
        if (node1 == null || node2 == null) return false;
        if (!IsSealed(node1)) Seal(node1);
        if (!IsSealed(node2)) Seal(node2);
        return TreeEqualsCore(ref node1, ref node2);
    }

    [DebuggerNonUserCode]
    static bool TreeEqualsCore(ref T node1, ref T node2)
    {
        if (node1 == node2)
            return true;

        if (node1 != null
            && node2 != null
            && node1._priority == node2._priority
            && node1._flags == node2._flags
            && Equals(node1.Key, node2.Key)
            && node1.NodeEquals(node2)
            && TreeEqualsCore(ref node1._left, ref node2._left)
            && TreeEqualsCore(ref node1._right, ref node2._right)) {
            if (Unsafe.IsAddressLessThan(ref node1._flags, ref node2._flags)) // IsOlder
                node2 = node1;
            else
                node1 = node2;
            return true;
        }

        return false;
    }

    [DebuggerNonUserCode]
    public static int TreeCompare(T node1, T node2)
    {
        if (TreeEquals(node1, node2)) return 0;
        if (node1 == null) return -1;
        if (node2 == null) return 1;

        int cmp = Count(node1).CompareTo(Count(node2));
        if (cmp != 0)
            return cmp;

        return TreeCompareCore(node1, node2);
    }

    [DebuggerNonUserCode]
    static int TreeCompareCore(T node1, T node2)
    {
        if (node1 == node2) return 0;
        if (node1 == null) return -1;
        if (node2 == null) return 1;

        int cmp = TreeCompareCore(node1._left, node2._left);
        if (cmp != 0)
            return cmp;

        cmp = node1.CompareKey(node2.Key);
        if (cmp != 0)
            return cmp;

        cmp = node1.NodeCompare(node2);
        if (cmp != 0)
            return cmp;

        cmp = TreeCompareCore(node1._right, node2._right);
        return cmp;
    }

    public virtual int NodeCompare(T node2) => 0;

    public override int GetHashCode() => TreeHashCode(this);

    protected virtual bool NodeEquals(T map) => true;

    protected virtual int NodeGetHashCode() => 0;

    protected int CompareKey(K key) => CompareKey(key, key.GetHashCode());

    [DebuggerStepThrough]
    protected int CompareKey(K key, int keyHashcode)
    {
        int cmp = Key.GetHashCode().CompareTo(keyHashcode);
        return cmp != 0
            ? cmp
            : OnCompareKey(key);
    }

    protected virtual int OnCompareKey(K key) => Comparer<K>.Default.Compare(Key, key);

    #endregion

    #region Overrides

    public override bool Equals(object key) => TreeEquals((T)this, key as T);

    public override string ToString() => Key.ToString();

    #endregion

    #region Methods

    [DebuggerStepThrough]
    protected static T CloneLeft(T self, T left)
    {
        if (IsSealed(self)) {
            if (self._left == left)
                return self;
            self = self.Clone();
        }

        Debug.Assert(left == null || self._priority >= left._priority);
        self._left = left;
        return self;
    }

    [DebuggerStepThrough]
    protected static T CloneRight(T self, T right)
    {
        if (IsSealed(self)) {
            if (self._right == right)
                return self;
            self = self.Clone();
        }

        Debug.Assert(right == null || self._priority >= right._priority);
        self._right = right;
        return self;
    }

    [Conditional("DEBUG")]
    void Validate()
    {
        Debug.Assert(_left == null || _left._priority <= _priority);
        Debug.Assert(_right == null || _right._priority <= _priority);
        if (IsSealed(this)) {
            Debug.Assert(IsSealed(_left));
            Debug.Assert(IsSealed(_right));
        }
    }

    [DebuggerStepThrough]
    public static bool Contains(T top, K key) => GetValue(top, key) != null;

    [DebuggerStepThrough]
    public static T GetValue(T top, K key)
    {
        T? current = top;
        int keyHashcode = key.GetHashCode();
        // TODO: Possible optimization with priority -- key priority if high allows shortcircuiting
        while (current != null) {
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
        if (cmp > 0) {
            result = Construct(top,
                Insert(top._left, insertion),
                top._right);
        } else if (cmp < 0) {
            result = Construct(top,
                top._left,
                Insert(top._right, insertion));
        } else {
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
        if (cmp > 0) {
            result = Construct(top,
                InsertCore(top._left, insertion, func),
                top._right);
        } else if (cmp < 0) {
            result = Construct(top,
                top._left,
                InsertCore(top._right, insertion, func));
        } else {
            T added = func(top, insertion);
            if (added.NodeEquals(top))
                return top;
            result = Clone(added, top._left, top._right);
        }

        return result;
    }

    [DebuggerStepThrough]
    public static T Construct(T top, T leftNode, T rightNode)
    {
        int priority = top._priority;

        if (leftNode != null && priority < leftNode._priority)
            return Construct(leftNode,
                leftNode._left,
                Construct(top, leftNode.Right, top.Right));

        if (rightNode != null && priority <= rightNode._priority)
            return Construct(rightNode,
                Construct(top, top._left, rightNode._left),
                rightNode._right);

        return Clone(top, leftNode, rightNode);
    }

    public static T Clone(T top, T leftNode, T rightNode)
    {
        T self = top;
        if (IsSealed(top)) {
            if (top._left == leftNode && top._right == rightNode)
                return top;
            self = self.Clone();
        }

        Debug.Assert(leftNode == null || self._priority >= leftNode._priority);
        Debug.Assert(rightNode == null || self._priority > rightNode._priority);
        self._left = leftNode;
        self._right = rightNode;
        return self;
    }

    [DebuggerStepThrough]
    [Pure]
    public static T Remove(T top, K key)
    {
        T? result = RemoveCore(top, key, key.GetHashCode());
        if (result != null) result.Validate();
        Seal(result);
        return result;
    }

    static T RemoveCore(T top, K delete, int keyHashCode)
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
    public static T Filter(T map, Func<T, bool> filter)
    {
        if (map == null)
            return null;

        T left = Filter(map._left, filter);
        T right = Filter(map._right, filter);
        return filter(map) ? Join(left, right) : Clone(map, left, right);
    }

    [DebuggerNonUserCode]
    [Pure]
    protected static T Transform(T map, Func<T, T> filter)
    {
        T result = TransformCore(map, filter);
        Seal(result);
        return result;
    }

    [DebuggerNonUserCode]
    [Pure]
    public static T Transform(T map, Func<T, T> filter, Func<T, T, T> merge)
    {
        Debug.Assert(merge != null);
        List<T> bucket = null;
        T result = TransformCore(map, filter, ref bucket);

        if (bucket != null)
            foreach (T tmp in bucket)
                result = InsertCore(result, tmp, merge);

        Seal(result);
        return result;
    }

    [DebuggerNonUserCode]
    static T TransformCore(T map, Func<T, T> filter)
    {
        if (map == null)
            return null;

        T left = TransformCore(map._left, filter);
        T right = TransformCore(map._right, filter);
        T? result = filter(map);

        if (result != null)
            if (result == map || map.Key.Equals(result.Key))
                return Clone(result, left, right);

        return Join(left, right);
    }

    [DebuggerNonUserCode]
    static T TransformCore(T map, Func<T, T> filter, ref List<T> bucket)
    {
        if (map == null)
            return null;

        T left = TransformCore(map._left, filter, ref bucket);
        T right = TransformCore(map._right, filter, ref bucket);
        T? result = filter(map);

        if (result != null) {
            if (result == map || map.Key.Equals(result.Key))
                return Clone(result, left, right);
            if (bucket == null) bucket = new List<T>();
            bucket.Add(result);
        }

        return Join(left, right);
    }

    [DebuggerNonUserCode]
    public static void ForEach(T map, Action<T> action)
    {
        if (map == null) return;
        ForEach(map._left, action);
        action(map);
        ForEach(map._right, action);
    }

    [DebuggerNonUserCode]
    public static IEnumerable<V> Select<V>(T top, Func<T, V> func)
    {
        if (top == null) return ListTools.Empty<V>();
        return SelectCore(top, func);
    }

#line hidden
    [DebuggerNonUserCode]
    static IEnumerable<V> SelectCore<V>(T current, Func<T, V> func)
    {
        var stack = new Stack<T>();
        while (true) {
            while (current != null) {
                stack.Push(current);
                current = current._left;
            }

            if (stack.Count == 0)
                break;

            current = stack.Pop();
            yield return func(current);
            current = current._right;
        }
    }
#line default

    [DebuggerHidden] public List<T> Nodes => Select((T)this, node => node).ToList();

    #endregion

    #region SET TREAP OPERATIONS

    // http://www.cs.cmu.edu/afs/cs.cmu.edu/project/scandal/public/papers/treaps-spaa98.pdf

    public static T Split(out T less, out T gtr, T r, K key)
    {
        if (r == null) {
            less = gtr = null;
            return null;
        }

        T split;
        int cmp = r.CompareKey(key);
        if (cmp < 0) {
            less = r.Clone();
            split = Split(out less._right, out gtr, r._right, key);
            return split;
        }

        if (cmp > 0) {
            gtr = r.Clone();
            split = Split(out less, out gtr._left, r._left, key);
            return split;
        }

        less = r._left;
        gtr = r._right;
        if (!IsSealed(r)) Seal(r);
        return r;
    }

    // http://pavpanchekha.com/programming/treap.html
    //private static T AlternateSplit()
    //{
    //    // Insert with maximum priority / then take children
    //    // (def split (key root)
    //  //(let (ins (set key #0 root #:(p 1)))
    //    //    `(,ins.left ,ins.right)))
    //    return default(T);
    //}

    [Pure]
    public static T Split(out int index, T r, K key)
    {
        T? current = r;
        index = 0;

        while (current != null) {
            int cmp = r.CompareKey(key);
            if (cmp > 0) {
                current = current._left;
            } else {
                index += Count(current._left);

                if (cmp == 0)
                    return current;

                index++;
                current = current._right;
            }
        }

        return null;
    }

    public static T Join(T map1, T map2)
    {
        if (map1 == null) return map2;
        if (map2 == null) return map1;
        return map1._priority > map2._priority
            ? CloneRight(map1, Join(map1._right, map2))
            : CloneLeft(map2, Join(map1, map2._left));
    }

    [Pure]
    public static T Union(T map1, T map2)
    {
        if (map1 == null)
            return map2;
        if (map2 == null || map1 == map2)
            return map1;

        if (map1._priority <= map2._priority)
            Swap(ref map1, ref map2);

        T less, gtr;
        Split(out less, out gtr, map2, map1.Key);
        return Clone(map1,
            Union(map1._left, less),
            Union(map1._right, gtr));
    }

    [Pure]
    public static T Merge(T map1, T map2, Func<T, T, T> merge, bool optimize)
    {
        if (map1 == map2)
            if (map1 == null || optimize)
                return map1;

        T merged;
        T left, right;
        T duplicate;

        if (map2 == null || (map1 != null && map1._priority >= map2._priority)) {
            duplicate = Split(out left, out right, map2, map1.Key);
            merged = merge(map1, duplicate);
            left = Merge(map1._left, left, merge, optimize);
            right = Merge(map1._right, right, merge, optimize);
            //if (merged != null)
            //    merged.priority = map1.priority;
        } else {
            duplicate = Split(out left, out right, map1, map2.Key);
            merged = merge(duplicate, map2);
            left = Merge(left, map2._left, merge, optimize);
            right = Merge(right, map2._right, merge, optimize);
            //if (merged != null)
            //    merged.priority = map2.priority;
        }

        if (merged == null)
            return Join(left, right);

        return Clone(merged, left, right);
    }

    static T Pop(Stack<T> stack) => stack.Count != 0 ? stack.Pop() : null;

    static T Advance(Stack<T> stack, T set1)
    {
        while (set1 != null) {
            stack.Push(set1);
            set1 = set1.Left;
        }

        return Pop(stack);
    }

    static T Advance(Stack<T> stack, T set1, T set2)
    {
        if (set1 != null
            && set1._priority >= set2._priority
            && (stack.Count == 0 || stack.Peek().CompareKey(set2.Key) >= 0))
            for (T? current = set1; current != null; current = current.Left) {
                stack.Push(current);
                if (current.CompareKey(set2.Key) < 0)
                    break;
            }

        return Pop(stack);
    }

    [Pure]
    public static bool Subset(T subset, T parent)
    {
        if (subset == null || subset == parent)
            return true;
        if (parent == null
            || parent._priority < subset._priority
            || Count(subset) > Count(parent))
            return false;

        var stack = new Stack<T>();
        var stack2 = new Stack<T>();

        T? current = Advance(stack, subset);
        T? current2 = Advance(stack2, parent, subset);
        while (current != null) {
            // If current not found, return false
            if (current2 == null)
                return false;

            // Don't waste time, skip parents
            if (current == current2) {
                current = Pop(stack);
                current2 = Pop(stack2);
                continue;
            }

            int cmp = current.CompareKey(current2.Key);
            if (cmp > 0)
                // Search forward for current in current2
                current2 = Advance(stack2, current2.Right, current);
            else if (cmp == 0)
                // move to current next element
                current = Advance(stack, current.Right);
            else
                return false;
        }

        return true;
    }

    public static bool Overlaps(T set1, T set2)
    {
        if (set1 == null || set2 == null)
            return false;
        if (set1 == set2)
            return true;
        if (Count(set1) > Count(set2))
            Swap(ref set1, ref set2);

        var stack1 = new Stack<T>();
        var stack2 = new Stack<T>();
        T? current1 = Advance(stack1, set1, set2);
        T? current2 = Advance(stack2, set2, set1);
        while (current1 != null && current2 != null) {
            // Search forward for current in current2
            int cmp = current1.CompareKey(current2.Key);
            if (cmp > 0)
                current2 = Advance(stack2, current2.Right, current1);
            else if (cmp < 0)
                current1 = Advance(stack1, current1.Right, current2);
            else
                return true;
        }

        return false;
    }

    [Pure]
    public static T ExclusiveUnion(T map1, T map2)
    {
        if (map1 == null) return map2;
        if (map2 == null) return map1;
        if (map1 == map2) return null;

        if (map1._priority <= map2._priority)
            Swap(ref map1, ref map2);

        T less, gtr;
        T? duplicate = Split(out less, out gtr, map2, map1.Key);

        T left = ExclusiveUnion(map1._left, less);
        T right = ExclusiveUnion(map1._right, gtr);
        if (duplicate != null)
            return Join(left, right);
        return Clone(map1, left, right);
    }

    [Pure]
    public static T Intersection(T map1, T map2)
    {
        if (map1 == map2)
            return map1;

        if (map1 == null || map2 == null)
            return null;

        if (map1._priority < map2._priority)
            Swap(ref map1, ref map2);

        T less, gtr;
        T? duplicate = Split(out less, out gtr, map2, map1.Key);
        T left = Intersection(map1._left, less);
        T right = Intersection(map1._right, gtr);
        if (duplicate == null)
            return Join(left, right);

        return Clone(map1, left, right);
    }

    public static T Difference(T map1, T map2) => Diff(map1, map2, true);

    [DebuggerStepThrough]
    [Pure]
    static T Diff(T map1, T map2, bool map2IsSubtr)
    {
        if (map1 == null || map2 == null)
            return map2IsSubtr ? map1 : map2;
        if (map1 == map2) return null;

        if (map1._priority < map2._priority) {
            map2IsSubtr = !map2IsSubtr;
            Swap(ref map1, ref map2);
        }

        T less, gtr;
        T? duplicate = Split(out less, out gtr, map2, map1.Key);
        T left = Diff(map1._left, less, map2IsSubtr);
        T right = Diff(map1._right, gtr, map2IsSubtr);

        /* Keep map1 if no dupl. and subtracting r2 */
        if (duplicate == null && map2IsSubtr)
            return Clone(map1, left, right);

        /* Delete map1 */
        return Join(left, right);
    }

    #endregion
}