namespace Algorithms.Graphs;
public partial class DynamicGraph
{
    static int _keyCounter;

    public class Edge
    {
        public object Label;
        public readonly int Key;
        public Vertex S;
        public Vertex T;
        public int Level;
        public List<EulerHalfEdge> Euler;

        public Edge(Vertex s, Vertex t, object label)
        {
            Label = label;
            Key = _keyCounter++;
            S = s;
            T = t;
        }

        bool Visit(TreapNode node)
        {
            if (node.Flag)
            {
                var v = (Vertex) ((EulerVertex)node.Value).Label;
                var adj = v.Adjacent;
                for (var ptr = LevelIndex(adj, Level); ptr < adj.Count && adj[ptr].Level == Level; ++ptr)
                {
                    var e = adj[ptr];
                    var es = e.S;
                    var et = e.T;
                    if (es.Euler[Level].Connected(et.Euler[Level]))
                    {
                        RaiseLevel(e);
                        ptr -= 1;
                    }
                    else
                    {
                        //Found the edge, relink components
                        Link(e);
                        return true;
                    }
                }
            }

            if (node.Left != null && node.Left.FlagAggregate)
            {
                if (Visit(node.Left))
                    return true;
            }

            if (node.Right != null && node.Right.FlagAggregate)
            {
                if (Visit(node.Right))
                    return true;
            }

            return false;
        }

        public void Cut()
        {
            //Don't double cut an edge
            if (S == null)
                return;

            //Search over tv for edge connecting to tw

            RemoveEdge(S, this);
            RemoveEdge(T, this);
            if (Euler != null)
            {
                //Cut edge from tree
                for (var i = 0; i < Euler.Count; ++i)
                    Euler[i].Cut();

                //Find replacement, looping over levels
                for (var i = Level; i >= 0; --i)
                {
                    var tv = S.Euler[i].Node.Root();
                    var tw = T.Euler[i].Node.Root();
                    Visit(tv.Count > tw.Count ? tw : tv);
                }
            }

            S = T = null;
            Euler = null;
            Level = 32;
        }

        public override string ToString() => $"Edge {S.Label}-{T.Label} - '{Label}'";
    }

    public class Vertex 
    {
        public object Label;
        public readonly List<EulerVertex> Euler = new List<EulerVertex>();
        public readonly List<Edge> Adjacent = new List<Edge>();

        public Vertex(object label = null)
        {
            Label = label;
            Euler.Add(new EulerVertex(this));
        }

        public bool Connected(Vertex other)
        {
            return Euler[0].Connected(other.Euler[0]);
        }

        public Edge Link(Vertex other, object value = null)
        {
            var e = new Edge(this, other, value);
            if (!Euler[0].Connected(other.Euler[0]))
                DynamicGraph.Link(e);

            Euler[0].SetFlag(true);
            other.Euler[0].SetFlag(true);
            Insert(Adjacent, e);
            Insert(other.Adjacent, e);
            return e;
        }

        /// <summary>Returns the number of vertices in this connected component</summary>
        public int ComponentSize()
        {
            return Euler[0].Count;
        }

        /// <summary>Removes the vertex from the graph</summary>
        public void Cut()
        {
            while (Adjacent.Count > 0)
                Adjacent[Adjacent.Count - 1].Cut();
        }

        public IEnumerable<Vertex> Component()
        {
            var node = Euler[0].Node.First();
            while (node != null)
            {
                if (node.Value is EulerVertex v)
                    yield return (Vertex)v.Label;
                node = node.Next;
            }
        }

        public override string ToString() => Label.ToString() ?? "Vertex";
    }

    #region Edge List

    static int CompareEdges(Edge a, Edge b)
    {
        var d = a.Level - b.Level;
        if (d!=0)
            return d;
        return a.Key - b.Key;
    }

    static int Index(List<Edge> list, Edge e)
    {
        int left = 0;
        int right = list.Count - 1;

        while (left <= right)
        {
            int mid = left + right >> 1;
            int cmp = CompareEdges(e,list[mid]);
            if (cmp > 0)
                left = mid + 1;
            else
                right = mid - 1;
        }

        if (left < list.Count && list[left].Key == e.Key)
            return left;
        return -1;
    }

    static int Insert(List<Edge> list, Edge e) //insertEdge
    {
        int left = 0;
        int right = list.Count - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            int cmp = CompareEdges(e, list[mid]);
            if (cmp >= 0)
                left = mid + 1;
            else
                right = mid - 1;
        }

        list.Insert(left, e);
        return left;
    }

    public static int LevelIndex(List<Edge> list, int i) // levelIndex
    {
        int left = 0;
        int right = list.Count - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            int cmp = i - list[mid].Level; // GOOD
            if (cmp > 0)
                left = mid + 1;
            else
                right = mid - 1;
        }

        //var alt = left < list.Count ? list[left].Level : list[left-1].Level+1;
        return left;
    }

    //Raise the level of an edge, optionally inserting into higher level trees
    static void RaiseLevel(Edge edge)
    {
        var s = edge.S;
        var t = edge.T;

        //Update position in edge lists
        RemoveEdge(s, edge);
        RemoveEdge(t, edge);
        edge.Level += 1;
        Insert(s.Adjacent, edge);
        Insert(t.Adjacent, edge);

        //Update flags for s
        if (s.Euler.Count <= edge.Level)
            s.Euler.Add(new EulerVertex(s));

        var es = s.Euler[edge.Level];
        es.SetFlag(true);

        //Update flags for t
        if (t.Euler.Count <= edge.Level)
        {
            t.Euler.Add(new EulerVertex(t));
        }

        var et = t.Euler[edge.Level];
        et.SetFlag(true);

        //Relink if necessary
        edge.Euler?.Add(es.Link(et, edge));
    }

    //Remove edge from list and update flags
    static void RemoveEdge(Vertex vertex, Edge edge)
    {
        var adj = vertex.Adjacent;
        var idx = Index(adj, edge);
        adj.RemoveAt(idx);
        //Check if flag needs to be updated
        if (!(idx < adj.Count && adj[idx].Level == edge.Level
              || idx > 0 && adj[idx - 1].Level == edge.Level))
        {
            vertex.Euler[edge.Level].SetFlag(false);
        }
    }

    //Add an edge to all spanning forests with level <= edge.level
    static void Link(Edge edge)
    {
        var es = edge.S.Euler;
        var et = edge.T.Euler;
        int size = edge.Level + 1;

        var euler = new List<EulerHalfEdge>(size);

        for (var i = 0; i < size; ++i)
        {
            if (es.Count <= i)
                es.Add(new EulerVertex(edge.S));

            if (et.Count <= i)
                et.Add(new EulerVertex(edge.T));

            euler.Add(es[i].Link(et[i], edge));
        }

        edge.Euler = euler;
    }
    #endregion
}