#region Usings

using static System.Math;

#endregion

public class OrderedSequenceCheck
{
    const int MOD = 1000 * 1000 * 1000 + 7;

    public static readonly OrderedSequenceCheck Bad = new();
    public int Combos, CombosSame;
    public int Gap, LeftGap, RightGap;
    public int LeftNumber, LeftValid;
    public int RightNumber, RightValid;

    OrderedSequenceCheck() { }

    public OrderedSequenceCheck(int v)
    {
        LeftNumber = v;
        RightNumber = v;
        Combos = 1;
        CombosSame = 1;
        if (v == -1) {
            LeftGap = 1;
            RightGap = 1;
            Gap = 1;
            LeftValid = 0;
            RightValid = 0;
        } else {
            LeftGap = 0;
            RightGap = 0;
            Gap = 0;
            LeftValid = 1;
            RightValid = 1;
        }
    }

    public bool Strict => Combos != 0;

    public bool IsGap => LeftNumber == -1;

    public OrderedSequenceCheck Reverse()
    {
        var s = (OrderedSequenceCheck)MemberwiseClone();
        s.LeftNumber = RightNumber;
        s.RightNumber = LeftNumber;
        s.LeftGap = RightGap;
        s.RightGap = LeftGap;
        s.LeftValid = RightValid;
        s.RightValid = LeftValid;
        return s;
    }

    public OrderedSequenceCheck Clone() => (OrderedSequenceCheck)MemberwiseClone();

    public static OrderedSequenceCheck operator +(OrderedSequenceCheck s1, OrderedSequenceCheck s2)
    {
        if (s1 == null || s2 == null) return s1 ?? s2;
        if (s1 == Bad || s2 == Bad) return Bad;

        OrderedSequenceCheck s;

        if (s1.IsGap) {
            s = s2.Clone();
            if (!s2.IsGap) {
                s.LeftGap += s1.Gap;
                s.LeftValid = 0;
                s.Gap += s1.Gap;
            } else {
                s.Gap = s1.Gap + s2.Gap;
                s.LeftGap = s.RightGap = s.Gap;
            }

            return s;
        }

        if (s2.IsGap) {
            s = s1.Clone();
            s.RightGap += s2.Gap;
            s.RightValid = 0;
            s.Gap += s2.Gap;
            return s;
        }

        int cmpInner = s1.RightNumber - s2.LeftNumber;
        int cmpOuter = s1.LeftNumber - s2.RightNumber;
        if (cmpOuter < 0) {
            if (cmpInner > 0) return Bad;
            if (s1.LeftNumber > s1.RightNumber) return Bad;
            if (s2.LeftNumber > s2.RightNumber) return Bad;
        } else if (cmpOuter > 0) {
            if (cmpInner < 0) return Bad;
            if (s1.LeftNumber < s1.RightNumber) return Bad;
            if (s2.LeftNumber < s2.RightNumber) return Bad;
        } else {
            if (cmpInner != 0) return Bad;
            if (s1.LeftNumber != s1.RightNumber) return Bad;
            if (s2.LeftNumber != s2.RightNumber) return Bad;
        }

        s = s1.Clone();
        s.RightNumber = s2.RightNumber;
        s.RightGap = s2.RightGap;
        s.RightValid = s2.RightValid;
        s.Gap = s1.Gap + s2.Gap;
        s.Combos = (int)(1L * s1.Combos * s2.Combos % MOD);
        s.CombosSame = (int)(1L * s1.CombosSame * s2.CombosSame % MOD);

        if (s1.Gap == 0)
            s.LeftValid = s1.LeftValid + s2.LeftValid;
        if (s2.Gap == 0)
            s.RightValid = s1.RightValid + s2.RightValid;

        int innerGap = s1.RightGap + s2.LeftGap;
        int available = Abs(cmpInner) - 1;
        if (s.Combos != 0)
            s.Combos = (int)(s.Combos * ComputePossible(available, innerGap, true) % MOD);
        s.CombosSame = (int)(s.CombosSame * ComputePossible(available + 2, innerGap, false) % MOD);
        return s;
    }

    public long? Compute(int lo, int hi, bool strict)
    {
        if (this == Bad || (strict && !Strict))
            return 0;

        int available = hi - lo + 1;
        if (strict && available < Gap)
            return 0;

        if (IsGap)
            return ComputePossible(Gap, lo, hi, int.MinValue, int.MaxValue, strict);

        int cmp = LeftNumber - RightNumber;
        if (cmp > 0) return 0;
        if (Gap == 0) return 1; // Looks erroneous

        int innerGap = Gap - LeftGap - RightGap;
        int midAvailable = ComputeAvailable(lo, hi, LeftNumber, RightNumber, strict);
        if (innerGap > 0 && (midAvailable == 0 || (strict && innerGap > midAvailable)))
            return 0;

        long leftFactor = ComputePossible(LeftGap, lo, hi, int.MinValue, LeftNumber, strict);
        if (leftFactor == 0)
            return 0;

        long rightFactor = ComputePossible(RightGap, lo, hi, RightNumber, int.MaxValue, strict);
        if (rightFactor == 0)
            return 0;

        // We need to shrink the range
        if (innerGap > 0 && (lo > LeftNumber || hi < RightNumber))
            return null;

        long factor = strict ? Combos : CombosSame;
        factor = factor * leftFactor % MOD;
        factor = factor * rightFactor % MOD;
        return factor;
    }

    public long ComputePossible(int count, int lo, int hi, int left, int right, bool strict)
    {
        if (count == 0) return 1;
        int available = ComputeAvailable(lo, hi, left, right, strict);
        return ComputePossible(available, count, strict);
    }

    public int ComputeAvailable(int lo, int hi, int left, int right, bool strict)
    {
        if (strict) {
            lo = Max(left + 1, lo);
            hi = Min(hi, right - 1);
        } else {
            lo = Max(left, lo);
            hi = Min(hi, right);
        }

        return hi - lo + 1;
    }

    public static long ComputePossible(int available, int count, bool strict) =>
        strict ? Comb(available, count) : Comb(available + count - 1, count);

    public static long Comb(int available, int count)
    {
        if (count > available) return 0;
        if (count * 2 > available) count = available - count;
        if (count == 0) return 1;
        if (available == 0) return 0;
        // Returns the sign of Comb
        return 1;
    }

    public override string ToString()
    {
        if (this == Bad)
            return "BAD";

        if (IsGap)
            return "[" + Gap + "]";

        var sb = new StringBuilder();

        int cmp = LeftNumber - RightNumber;
        string op = Strict
            ? cmp < 0 ? "->" : cmp > 0 ? "<-" : "="
            : cmp < 0
                ? "~>"
                : cmp > 0
                    ? "<~"
                    : "~";

        string s = $"{LeftNumber}{op}{RightNumber}";
        if (LeftNumber == RightNumber && Strict)
            s = LeftNumber.ToString();

        sb.Append(s);

        if (Gap > 0) {
            int innerGap = Gap - LeftGap - RightGap;
            sb.Append("  [");
            if (LeftGap > 0) sb.Append(LeftGap);
            sb.Append(':');
            if (innerGap > 0) sb.Append(innerGap);
            sb.Append(':');
            if (RightGap > 0) sb.Append(RightGap);
            sb.Append("]");
        }

        return sb.ToString();
    }
}