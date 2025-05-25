namespace Algorithms.Graphs;

public class Gabow
{
    readonly Stack<int> _gabowP = new();

    readonly Stack<int> _gabowS = new();
    readonly VData[] _vertices;
    int _index;
    int _maxP;
    int _maxS;

    public Gabow(Dictionary<int, HashSet<int>> g)
    {
        _index = 1;
        _vertices = new VData[g.Count + 1];
        Array.Clear(_vertices, 0, _vertices.Length);
        foreach (int v in g.Keys)
            if (_vertices[v].Visited == false)
                GabowSCC(g, v);

        Console.WriteLine("MaxP = {0}", _maxP);
        Console.WriteLine("MaxS = {0}", _maxS);

        IEnumerable<int> list = _vertices.Select(x => x.Count).OrderByDescending(x => x).Take(5);
        foreach (int elem in list)
            Console.WriteLine(elem);
    }

    public void GabowSCC(Dictionary<int, HashSet<int>> g, int v)
    {
        // Set the depth index for v to the smallest unused index
        _vertices[v].Index = _index;
        //_vertices[v].Pushed = true;
        _vertices[v].Visited = true;
        _index++;
        _gabowS.Push(v);
        _gabowP.Push(v);

        _maxP = Math.Max(_gabowP.Count, _maxP);
        _maxS = Math.Max(_gabowS.Count, _maxS);

        // Consider successors of v
        if (g.ContainsKey(v))
            foreach (int w in g[v])
                if (_vertices[w].Visited == false)
                    // Successor w has not yet been visited; recurse on it
                    GabowSCC(g, w);
                else if (_vertices[w].LowLink == 0)
                    while (_gabowP.Count > 0 && _vertices[_gabowP.Peek()].Index > _vertices[w].Index)
                        _gabowP.Pop();

        // If v is a root node, pop the stack and generate an SCC
        if (_gabowP.Count > 0 && _gabowP.Peek() == v) {
            int w;
            // start a new strongly connected component
            do {
                w = _gabowS.Pop();
                _vertices[w].LowLink = v;
                _vertices[v].Count++;
                // add w to current strongly connected component
            } while (w != v);

            _gabowP.Pop();
            // output the current strongly connected component
        }
    }

    struct VData
    {
        public int Index;
        public int LowLink;
        public int Count;
        public bool Visited;
    }
}