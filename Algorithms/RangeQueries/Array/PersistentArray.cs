using System.Runtime.CompilerServices;

namespace Algorithms.RangeQueries;

using T = int;

public class PersistentArray
{
    public class Node
    {
        #region Other Operations

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Node UpdateNode(Node left, Node right)
        {
            if (ReadOnly)
                return new Node(left, right);
            Left = left;
            Right = right;
            T x = Left.Value;
            T y = Right.Value;
            Value = x == y ? x : int.MinValue;
            return this;
        }

        #endregion

        #region Variables

        public T Value;
        public Node Left;
        public Node Right;
        public bool ReadOnly;
        public int Length;

#if DEBUG
        static int counter;
        int ID = counter++;

        public string Dump()
        {
            var sb = new StringBuilder();
            Dump(sb, " ");
            return sb.ToString();
        }

        void Dump(StringBuilder sb, string indent, int start = 0)
        {
            string readOnly = ReadOnly ? " readonly" : "";
            Debug.WriteLine($"{indent} #{ID} {start}:{start + Length} ={Value} {readOnly}");
            Left?.Dump(sb, indent + " ", start);
            Right?.Dump(sb, indent + " ", start + Left.Length);
        }
#endif

        #endregion

        #region Constructor

        public Node(int length, int defaultValue)
        {
            Length = length;
            ReadOnly = true;
            Value = defaultValue;
            if (length >= 2) {
                int half = (length + 1) >> 1;
                Left = new Node(half, defaultValue);
                Right = (length & 1) == 0 ? Left : new Node(length - half, defaultValue);
            }
        }

        public Node(T[] array)
            : this(array, 0, array.Length) { }

        public Node(T[] array, int start, int length)
        {
            Length = length;
            ReadOnly = true;
            if (length >= 2) {
                int half = (length + 1) >> 1;
                Left = new Node(array, start, half);
                Right = new Node(array, start + half, length - half);
                T x = Left.Value;
                T y = Right.Value;
                Value = x == y ? x : int.MinValue;
            } else {
                Value = array[start];
            }
        }

        Node(Node left, Node right)
        {
            Length = left.Length + right.Length;
            Left = left;
            Right = right;

            T x = Left.Value;
            T y = Right.Value;
            Value = x == y ? x : int.MinValue;
        }

        public Node Clone()
        {
            if (!ReadOnly)
                MakeReadOnly();
            return this;
        }

        void MakeReadOnly()
        {
            if (ReadOnly) return;
            ReadOnly = true;
            Left?.MakeReadOnly();
            Right?.MakeReadOnly();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Node MutableNode()
        {
            if (!ReadOnly) return this;
            var node = (Node)MemberwiseClone();
            node.ReadOnly = false;
#if DEBUG
            node.ID = counter++;
#endif
            return node;
        }

        #endregion

        #region Core Operations

        public T Query(int start)
        {
            if (Value != int.MinValue)
                return Value;

            if (start < Left.Length)
                return Left.Query(start);
            return Right.Query(start - Left.Length);
        }

        public Node Cover(int start, T value)
        {
            if (start >= Length || start < 0)
                return this;

            if (start == 0 && Length == 1) {
                Node node = MutableNode();
                node.Value = value;
                return node;
            }

            return UpdateNode(
                Left.Cover(start, value),
                Right.Cover(start - Left.Length, value));
        }

        #endregion

        #region Misc

        public override string ToString() => $"Sum={Value} Length={Length}";

        public void FillTable(T[] table, int start = 0)
        {
            if (Length == 1) {
                table[start] = Value;
                return;
            }

            Left.FillTable(table, start);
            int rightStart = start + Left.Length;
            if (rightStart < table.Length) Right.FillTable(table, rightStart);
        }

        #endregion
    }

    #region Variables

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Node Root;

    public int Length;

    #endregion

    #region PersistentArray

    public PersistentArray(int length, T defaultValue)
    {
        Root = new Node(Math.Max(1, length), defaultValue);
        Length = length;
    }

    public PersistentArray(T[] array, int start = 0, int length = int.MaxValue)
    {
        if (length > array.Length - start)
            length = array.Length - start;
        Root = new Node(array, start, length);
        Length = length;
    }

    public PersistentArray Clone()
    {
        var clone = (PersistentArray)MemberwiseClone();
        clone.Root = clone.Root.Clone();
        return clone;
    }

    #endregion

    #region Properties

    public T this[int index] {
        get => Root.Query(index);
        set => Root = Root.Cover(index, value);
    }

    public T[] Table {
        get
        {
            T[] result = new T[Length];
            Root.FillTable(result);
            return result;
        }
    }

    #endregion

    #region Debug

    // Optimized setoperations -- union, set, delete
#if DEBUG
    public DebugNode DebugRoot => new(Root, 0);

    [DebuggerDisplay("{Start}..{End} ({Root.Length}) -> {Root}")]
    public struct DebugNode
    {
        readonly Node Root;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Start;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int End => Start + (Root?.Length ?? 1) - 1;

        public DebugNode Left => new(Root?.Left, Start);
        public DebugNode Right => new(Root?.Right, Math.Min(End, ((Start + End) >> 1) + 1));

        public DebugNode(Node node, int start)
        {
            Start = start;
            Root = node;
        }
    }
#endif

    #endregion
}