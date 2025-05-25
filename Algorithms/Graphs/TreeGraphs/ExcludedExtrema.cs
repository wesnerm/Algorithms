namespace Algorithms.Graphs;

public struct ExcludedExtrema
{
    public long Maximum;
    public long Maximum2;
    public long Minimum;
    public long Minimum2;

    public static ExcludedExtrema Empty => new()
    {
        Maximum = long.MinValue,
        Maximum2 = long.MinValue,
        Minimum = long.MaxValue,
        Minimum2 = long.MaxValue,
    };

    public void Add(long v)
    {
        if (v > Maximum2) {
            Maximum2 = v;
            if (Maximum2 > Maximum) Swap(ref Maximum, ref Maximum2);
        }

        if (v < Minimum2) {
            Minimum2 = v;
            if (Minimum2 < Minimum) Swap(ref Minimum, ref Minimum2);
        }
    }

    public long ExcludedMinimum(long exclude) => exclude != Minimum ? Minimum : Minimum2;

    public long ExcludedMaximum(long exclude) => exclude != Maximum ? Maximum : Maximum2;
}

public class ExcludedMask
{
    public long Mask;
    public long Mask2;

    public ExcludedMask(long v) => Mask = v;

    public void Add(long v)
    {
        Mask2 |= Mask & v;
        Mask |= v;
    }

    public long Exclude(long exclude) => (Mask & ~exclude) | Mask2;
}