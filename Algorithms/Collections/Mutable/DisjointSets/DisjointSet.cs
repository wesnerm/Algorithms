#pragma warning disable 660, 661
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace Algorithms.Collections;

public class DisjointSet
{
    #region Variables

    Node _ds;

    #endregion

    #region Constructor

    public DisjointSet() => _ds = new Node();

    #endregion

    #region Helpers

    class Node
    {
        public int Count = 1;
        public int Label;
        public Node Parent;
    }

    #endregion

    #region Methods

    public bool Union(DisjointSet node)
    {
        Node rx = Find(ref _ds);
        Node ry = Find(ref node._ds);
        if (rx == ry) return false;
        if (rx.Count >= ry.Count) {
            rx.Count += ry.Count;
            node._ds = ry.Parent = rx;
        } else {
            ry.Count += rx.Count;
            _ds = rx.Parent = ry;
        }

        return true;
    }

    Node Find() => Find(ref _ds);

    static Node Find(ref Node x) => x.Parent == null ? x : x = Find(ref x.Parent);

    public bool Connected(DisjointSet set) => Find() == set.Find();

    public void Delete()
    {
        if (Find().Count == 1) return;
        _ds.Count--;
        _ds = new Node();
    }

    public void AssignToSet(DisjointSet set)
    {
        _ds.Count--;
        _ds = set._ds;
        _ds.Count++;
    }

    public int Count => Find().Count;

    public int Label {
        get => Find().Label;
        set => Find().Label = value;
    }

    #endregion

    #region Equality

    public static bool operator ==(DisjointSet node1, DisjointSet node2)
    {
        if ((object)node1 == node2)
            return true;
        if ((object)node1 == null || (object)node2 == null)
            return false;

        return node1.Find() == node2.Find();
    }

    public static bool operator !=(DisjointSet node1, DisjointSet node2) => !(node1 == node2);

    public override bool Equals(object obj) => Equals(obj as DisjointSet);

    public bool Equals(DisjointSet set) => (object)set != null && Find() == set.Find();

    #endregion
}