using System.Runtime.CompilerServices;
using STType = int;

namespace Algorithms.RangeQueries;

public abstract class FunctionalArray8
{
    public abstract STType this[int index] { get; }

    public static FunctionalArray8 Create(int length) => new Empty { Length = length };

    public abstract FunctionalArray8 SetIndex(int index, STType value);

    protected class Empty : FunctionalArray8
    {
        public int Length;

        public override STType this[int index] => default;

        public override FunctionalArray8 SetIndex(int index, STType value) =>
            new Singleton { Length = Length, Index = index, Value = value };
    }

    protected class Singleton : FunctionalArray8
    {
        public int Index;
        public int Length;
        public STType Value;

        public override STType this[int index] => Index == index ? Value : default;

        public override FunctionalArray8 SetIndex(int index, STType value)
        {
            if (Index == index) {
                var node = (Singleton)MemberwiseClone();
                node.Value = value;
                return node;
            }

            if (Length <= 8) {
                var node = new Leaf();
                node.SetIndexX(Index, Value);
                node.SetIndexX(index, value);
                return node;
            }

            FunctionalArray8 zero = Create((Length + 7) >> 3);
            var clone = new Node();
            clone.Item0 = clone.Item1 =
                clone.Item2 = clone.Item3 = clone.Item4 = clone.Item5 = clone.Item6 = clone.Item7 = zero;
            clone.SetItem(Index & 7, zero.SetIndex(Index >> 3, value));
            clone.SetItem(index & 7, clone.GetItem(index & 7).SetIndex(index >> 3, value));
            return clone;
        }
    }

    protected class Node : FunctionalArray8
    {
        public FunctionalArray8 Item0, Item1, Item2, Item3, Item4, Item5, Item6, Item7;

        public override STType this[int index] => GetItem(index & 7)[index >> 3];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FunctionalArray8 GetItem(int index)
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
                case 4:
                    return Item4;
                case 5:
                    return Item5;
                case 6:
                    return Item6;
                case 7:
                    return Item7;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetItem(int index, FunctionalArray8 value)
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
                case 4:
                    Item4 = value;
                    break;
                case 5:
                    Item5 = value;
                    break;
                case 6:
                    Item6 = value;
                    break;
                case 7:
                    Item7 = value;
                    break;
            }
        }

        public override FunctionalArray8 SetIndex(int index, STType value)
        {
            var clone = (Node)MemberwiseClone();
            int i = index & 7;
            clone.SetItem(i, clone.GetItem(i).SetIndex(index >> 3, value));
            return clone;
        }
    }

    protected class Leaf : FunctionalArray8
    {
        STType Item0, Item1, Item2, Item3, Item4, Item5, Item6, Item7;

        public override STType this[int index] {
            get
            {
                switch (index) {
                    case 0: return Item0;
                    case 1: return Item1;
                    case 2: return Item2;
                    case 3: return Item3;
                    case 4: return Item4;
                    case 5: return Item5;
                    case 6: return Item6;
                    case 7: return Item7;
                    default: throw new Exception();
                }
            }
        }

        public override FunctionalArray8 SetIndex(int index, STType value)
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
                case 4:
                    Item4 = value;
                    break;
                case 5:
                    Item5 = value;
                    break;
                case 6:
                    Item6 = value;
                    break;
                case 7:
                    Item7 = value;
                    break;
            }
        }
    }
}