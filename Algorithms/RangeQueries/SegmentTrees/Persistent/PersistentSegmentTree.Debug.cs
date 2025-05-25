using static System.Math;

#if DEBUG

namespace Algorithms.RangeQueries;

[DebuggerDisplay("Length = {Length}")]
public partial class PersistentSegmentTree
{
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
        string cover = Covering > 0 ? " " + LazyValue : "";
        Debug.WriteLine($"{indent} #{ID} {start}:{start + Length} ={Sum} {cover}{readOnly}");
        Left?.Dump(sb, indent + " ", start);
        Right?.Dump(sb, indent + " ", start + Left.Length);
    }

    #region Constructor

    partial void IncrementId()
    {
        ID = counter++;
    }

    #endregion

    #region Debug

    public DebugNode DebugRoot => new(this, 0);

    [DebuggerDisplay("{Start}..{End} ({Root.Length}) -> {Root}")]
    public struct DebugNode
    {
        readonly PersistentSegmentTree Root;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int Start;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int End => Start + (Root?.Length ?? 1) - 1;

        public DebugNode Left => new(Root?.Left, Start);
        public DebugNode Right => new(Root?.Right, Min(End, ((Start + End) >> 1) + 1));

        public DebugNode(PersistentSegmentTree node, int start)
        {
            Start = start;
            Root = node;
        }
    }

    #endregion
}
#endif