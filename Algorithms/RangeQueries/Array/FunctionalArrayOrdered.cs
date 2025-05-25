using System.Runtime.CompilerServices;
using STType = int;

namespace Algorithms.RangeQueries;

public abstract class FunctionalArrayOrdered
{
    public abstract STType this[int index] { get; }

    public static FunctionalArrayOrdered Create(int length) => new Empty { Length = length };

    public abstract FunctionalArrayOrdered SetIndex(int index, STType value);

    protected class Empty : FunctionalArrayOrdered
    {
        public int Length;

        public override STType this[int index] => default;

        public override FunctionalArrayOrdered SetIndex(int index, STType value) =>
            new Singleton { Length = Length, Index = index, Value = value };
    }

    protected class Singleton : FunctionalArrayOrdered
    {
        public int Index;
        public int Length;
        public STType Value;

        public override STType this[int index] => Index == index ? Value : default;

        public override FunctionalArrayOrdered SetIndex(int index, STType value)
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

            int shift = 2;
            while (1 << (shift + 2) < Length) shift += 2;
            int len = 1 << shift;

            FunctionalArrayOrdered zero = Create(len);
            var clone = new Node();
            clone.Item0 = clone.Item1 = clone.Item2 = clone.Item3 = zero;
            clone.Shift = shift;
            clone.SetItem(Index >> shift, zero.SetIndex(Index & (len - 1), value));
            clone.SetItem(index >> shift, clone.GetItem(index >> shift).SetIndex(index & (len - 1), value));
            return clone;
        }
    }

    protected class Node : FunctionalArrayOrdered
    {
        public FunctionalArrayOrdered Item0, Item1, Item2, Item3;
        public int Shift;

        public override STType this[int index] {
            get
            {
                int i = index >> Shift;
                index &= (1 << Shift) - 1;
                return GetItem(i)[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FunctionalArrayOrdered GetItem(int index)
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
        public void SetItem(int index, FunctionalArrayOrdered value)
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

        public override FunctionalArrayOrdered SetIndex(int index, STType value)
        {
            var clone = (Node)MemberwiseClone();
            int i = index >> Shift;
            index &= (1 << Shift) - 1;
            clone.SetItem(i, clone.GetItem(i).SetIndex(index, value));
            return clone;
        }
    }

    protected class Leaf : FunctionalArrayOrdered
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

        public override FunctionalArrayOrdered SetIndex(int index, STType value)
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