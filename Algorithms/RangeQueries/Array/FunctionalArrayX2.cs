using System.Runtime.CompilerServices;
using STType = int;

namespace Algorithms.RangeQueries;

public abstract class FunctionalArrayX2
{
    public abstract STType this[int index] { get; }

    public static FunctionalArrayX2 Create() => new Empty();

    public abstract FunctionalArrayX2 SetIndex(int index, STType value);

    public FunctionalArrayX2 Join(int index0, STType value0, int index1, STType value1)
    {
        var couple = new Couple();
        if (index0 < index1) {
            couple.Index0 = index0;
            couple.Index1 = index1;
            couple.Value0 = value0;
            couple.Value1 = value1;
        } else {
            couple.Index1 = index0;
            couple.Index0 = index1;
            couple.Value1 = value0;
            couple.Value0 = value1;
        }

        return couple;
    }

    class Empty : FunctionalArrayX2
    {
        public override STType this[int index] => default;

        public override FunctionalArrayX2 SetIndex(int index, STType value) =>
            new Singleton { Index = index, Value = value };
    }

    class Singleton : FunctionalArrayX2
    {
        public int Index;
        public STType Value;

        public override STType this[int index] => Index == index ? Value : default;

        public override FunctionalArrayX2 SetIndex(int index, STType value)
        {
            if (Index == index) {
                var node = (Singleton)MemberwiseClone();
                node.Value = value;
                return node;
            }

            return Join(Index, Value, index, value);
        }
    }

    class Couple : FunctionalArrayX2
    {
        public int Index0, Index1;
        public STType Value0, Value1;

        public override STType this[int index] {
            get
            {
                if (index == Index0) return Value0;
                if (index == Index1) return Value1;
                return default;
            }
        }

        public override FunctionalArrayX2 SetIndex(int index, STType value)
        {
            int xor0 = index ^ Index0;
            if (xor0 == 0) {
                var couple = (Couple)MemberwiseClone();
                couple.Value0 = value;
                return couple;
            }

            int xor1 = index ^ Index1;
            if (xor1 == 0) {
                var couple = (Couple)MemberwiseClone();
                couple.Value1 = value;
                return couple;
            }

            // We don't have the higher bit
            if (Index1 < (xor1 & ~Index1))
                return new Node { Item0 = this, Item1 = new Singleton { Index = index, Value = value } };

            if (xor0 < (Index0 & ~xor0))
                return new Node { Item1 = this, Item0 = new Singleton { Index = index, Value = value } };

            return xor0 > xor1
                ? new Node
                {
                    Item0 = new Singleton { Index = Index0, Value = Value0 },
                    Item1 = Join(Index1, Value1, index, value),
                }
                : new Node
                {
                    Item0 = Join(Index0, Value0, index, value),
                    Item1 = new Singleton { Index = Index1, Value = Value1 },
                };
        }
    }

    class Node : FunctionalArrayX2
    {
        public FunctionalArrayX2 Item0, Item1;
        public int Mask;

        public override STType this[int index] => GetItem(index & 1)[index >> 1];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FunctionalArrayX2 GetItem(int index) => index == 0 ? Item0 : index == 1 ? Item1 : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetItem(int index, FunctionalArrayX2 value)
        {
            if (index == 0) Item0 = value;
            else if (index == 1) Item1 = value;
        }

        public override FunctionalArrayX2 SetIndex(int index, STType value)
        {
            var clone = (Node)MemberwiseClone();
            int i = index & 1;
            clone.SetItem(i, clone.GetItem(i).SetIndex(index >> 1, value));
            return clone;
        }
    }
}