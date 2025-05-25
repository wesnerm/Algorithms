namespace Algorithms.Collections;

/// <summary>
///     Summary description for PGraph
/// </summary>
[Pure]
public sealed class PGraph<T> : Persistent<Graph<T>>
    where T : IEquatable<T>
{
    #region Helper Classes

    [DebuggerStepThrough]
    class Diff : Change
    {
        readonly T _vertex1;
        readonly T _vertex2;
        bool _insert;

        public Diff(bool insert, T e1, T e2)
        {
            _insert = insert;
            _vertex1 = e1;
            _vertex2 = e2;
        }

        public override Change Apply(Graph<T> core)
        {
            if (_insert) core.AddEdge(_vertex1, _vertex2);
            else core.RemoveEdge(_vertex1, _vertex2);
            _insert = !_insert;
            return this;
        }
    }

    //[DebuggerStepThrough]
    //private class VertexDiff : Change
    //{

    //}

    #endregion

    #region Constructor

    [DebuggerStepThrough]
    public PGraph()
        : base(new Graph<T>()) { }

    [DebuggerStepThrough]
    public PGraph(Graph<T> graph)
        : base(graph) { }

    [DebuggerStepThrough]
    PGraph(Change change, PGraph<T> prev)
        : base(change, prev) { }

    #endregion

    #region Properties

    public int EdgeCount => Data.EdgeCount;

    public int VertexCount => Data.VertexCount;

    public PGraph<T> AddEdge(T e1, T e2) => new(new Diff(true, e1, e2), this);

    public PGraph<T> RemoveEdge(T e1, T e2) => new(new Diff(false, e1, e2), this);

    #endregion
}