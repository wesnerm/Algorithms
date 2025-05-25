using System.Runtime.CompilerServices;
using STType = int;

namespace Algorithms.RangeQueries;

public abstract class FunctionalArray4
{
    public abstract STType this[int index] { get; }

    public static FunctionalArray4 Create(int length) => new Empty { Length = length };

    public abstract FunctionalArray4 SetIndex(int index, STType value);

    protected class Empty : FunctionalArray4
    {
        public int Length;

        public override STType this[int index] => default;

        public override FunctionalArray4 SetIndex(int index, STType value) =>
            new Singleton { Length = Length, Index = index, Value = value };
    }

    protected class Singleton : FunctionalArray4
    {
        public int Index;
        public int Length;
        public STType Value;

        public override STType this[int index] => Index == index ? Value : default;

        public override FunctionalArray4 SetIndex(int index, STType value)
        {
            if (Index == index) {
                var node = (Singleton)MemberwiseClone();
                node.Value = value;
                return node;
            }

            if (Length <= 4) {
                var node = new Leaf();
                node.SetIndexX(Index, Value);
                node.SetIndexX(index, value);
                return node;
            }

            FunctionalArray4 zero = Create((Length + 3) >> 2);
            var clone = new Node();
            clone.Item0 = clone.Item1 = clone.Item2 = clone.Item3 = zero;
            clone.SetItem(Index & 3, zero.SetIndex(Index >> 2, value));
            clone.SetItem(index & 3, clone.GetItem(index & 3).SetIndex(index >> 2, value));
            return clone;
        }
    }

    protected class Node : FunctionalArray4
    {
        public FunctionalArray4 Item0, Item1, Item2, Item3;

        public override STType this[int index] => GetItem(index & 3)[index >> 2];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FunctionalArray4 GetItem(int index)
        {
            switch (index) {
                case 0:
                    return Item0;
                case 1:
                    return Item1;
                case 2:
                    return Item2;
                case 3:
                    return Item3;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetItem(int index, FunctionalArray4 value)
        {
            switch (index) {
                case 0:
                    Item0 = value;
                    break;
                case 1:
                    Item1 = value;
                    break;
                case 2:
                    Item2 = value;
                    break;
                case 3:
                    Item3 = value;
                    break;
            }
        }

        public override FunctionalArray4 SetIndex(int index, STType value)
        {
            var clone = (Node)MemberwiseClone();
            int i = index & 3;
            clone.SetItem(i, clone.GetItem(i).SetIndex(index >> 2, value));
            return clone;
        }
    }

    protected class Leaf : FunctionalArray4
    {
        STType Item0, Item1, Item2, Item3;

        public override STType this[int index] {
            get
            {
                switch (index) {
                    case 0: return Item0;
                    case 1: return Item1;
                    case 2: return Item2;
                    case 3: return Item3;
                    default: throw new Exception();
                }
            }
        }

        public override FunctionalArray4 SetIndex(int index, STType value)
        {
            var clone = (Leaf)MemberwiseClone();
            clone.SetIndexX(index, value);
            return clone;
        }

        public void SetIndexX(int index, STType value)
        {
            switch (index) {
                case 0:
                    Item0 = value;
                    break;
                case 1:
                    Item1 = value;
                    break;
                case 2:
                    Item2 = value;
                    break;
                case 3:
                    Item3 = value;
                    break;
            }
        }
    }
}