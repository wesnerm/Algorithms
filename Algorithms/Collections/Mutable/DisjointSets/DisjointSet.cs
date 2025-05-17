
#pragma warning disable 660, 661
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

namespace Algorithms.Collections;

public class DisjointSet
{
    #region Variables
    Node _ds;
    #endregion

    #region Constructor
    public DisjointSet()
    {
        _ds = new Node();
    }
    #endregion

    #region Methods
    public bool Union(DisjointSet node)
    {
        var rx = Find(ref _ds);
        var ry = Find(ref node._ds);
        if (rx == ry) return false;
        if (rx.Count >= ry.Count)
        {
            rx.Count += ry.Count;
            node._ds = ry.Parent = rx;
        }
        else
        {
            ry.Count += rx.Count;
            _ds = rx.Parent = ry;
        }
        return true;
    }

    Node Find()
    {
        return Find(ref _ds);
    }

    static Node Find(ref Node x)
    {
        return x.Parent == null ? x : (x = Find(ref x.Parent));
    }

    public bool Connected(DisjointSet set)
    {
        return Find() == set.Find();
    }

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

    public int Label
    {
        get { return Find().Label; }
        set { Find().Label = value; }
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

    public static bool operator !=(DisjointSet node1, DisjointSet node2)
    {
        return !(node1 == node2);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as DisjointSet);
    }

    public bool Equals(DisjointSet set)
    {
        return (object)set != null && Find() == set.Find();
    }
    #endregion

    #region Helpers
    class Node
    {
        public Node Parent;
        public int Count = 1;
        public int Label;
    }
    #endregion
}
