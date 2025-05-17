using System.Text;

// SOURCE: https://www.hackerrank.com/rest/contests/w27/challenges/how-many-substrings/hackers/uwi/download_solution
// SOURCE: https://saisumit.wordpress.com/2016/01/26/suffix-automaton/
// SOURCE: https://www37.atwiki.jp/uwicoder/pages/2842.html#id_bb5a1c5a
// SOURCE: http://e-maxx.ru/algo/suffix_automata

namespace Algorithms.Strings;

public partial class SuffixAutomaton
{
    public Node Start;
    public Node End;
    public int NodeCount;
    public IEnumerable<char> Text;

    Node[] _nodes;
    SummarizedState[] _summary;

    private SuffixAutomaton()
    {
        Start = new Node();
        End = Start;
        NodeCount = 1;
    }

    /// <summary>
    /// Constructs an automaton from the string
    /// </summary>
    /// <param name="s"></param>
    public SuffixAutomaton(IEnumerable<char> s) : this()
    {
        Text = s;

        foreach (var c in s)
            Extend(c);

        for (var p = End; p != Start; p = p.Link)
            p.IsTerminal = true;
    }

    /// <summary>
    /// Extends an automaton by one character
    /// </summary>
    /// <param name="c"></param>
    public void Extend(char c)
    {
        var node = new Node
        {
            Key = c,
            Len = End.Len + 1,
            Link = Start,
            Index = NodeCount,
        };
        NodeCount++;

        Node p;
        for (p = End; p != null && p[c] == null; p = p.Link)
            p[c] = node;
        End = node;

        if (p == null) return;

        var q = p[c];
        if (p.Len + 1 == q.Len)
            node.Link = q;
        else
        {
            var clone = q.Clone();
            clone.Len = p.Len + 1;
            clone.Index = NodeCount;
            NodeCount++;

            for (; p != null && p[c] == q; p = p.Link)
                p[c] = clone;

            q.Link = node.Link = clone;
        }
    }

    /// <summary>
    /// Indicates whether the substring is contained with automaton
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public bool ContainsSubstring(string s)
    {
        return FindNode(s) != null;
    }

    /// <summary>
    /// Lazily constructs a list of nodes
    /// </summary>
    /// <returns></returns>
    public Node[] GetNodes()
    {
        if (_nodes != null && NodeCount == _nodes.Length)
            return _nodes;

        var nodes = _nodes = new Node[NodeCount];
        int stack = 0;
        int idx = NodeCount;

        nodes[stack++] = Start;
        while (stack > 0)
        {
            var current = nodes[--stack];

            if (current.Index > 0)
                current.Index = 0;

            current.Index--;
            var index = current.NextCount + current.Index;
            if (index >= 0)
            {
                stack++;

                var child = current.Next[index];
                if (child.Index >= -child.NextCount)
                    nodes[stack++] = current.Next[index];
            }
            else if (index == -1)
            {
                nodes[--idx] = current;
            }
            Debug.Assert(idx >= stack);
        }

        if (idx != 0)
        {
            Debug.Assert(idx == 0, "NodeCount smaller than number of nodes");
            NodeCount -= idx;
            _nodes = new Node[NodeCount];
            Array.Copy(nodes, idx, _nodes, 0, NodeCount);
        }

        UpdateNodeIndices();
        return _nodes;
    }

    /// <summary>
    /// Iterates through nodes in bottom-up fashion
    /// </summary>
    public IEnumerable<Node> NodesBottomUp()
    {
        var nodes = GetNodes();
        for (int i = NodeCount - 1; i >= 0; i--)
            yield return nodes[i];
    }

    void UpdateNodeIndices()
    {
        var nodes = _nodes;
        for (int i = 0; i < NodeCount; i++)
            nodes[i].Index = i;
    }

    /// <summary>
    /// Goes through a node given a string
    /// </summary>
    /// <param name="pattern">string to search for</param>
    /// <param name="index">start of substring in pattern to search for</param>
    /// <param name="count">length of substring</param>
    /// <returns>returns node representing string or null if failed</returns>

    public Node FindNode(string pattern, int index, int count)
    {
        var node = Start;
        for (int i = 0; i < count; i++)
        {
            node = node[pattern[index + i]];
            if (node == null) return null;
        }
        return node;
    }

    public Node FindNode(string pattern)
    {
        return FindNode(pattern, 0, pattern.Length);
    }

    /// <summary>
    /// Provides a compressed view of the automaton, so that depth-first search of an automaton
    /// can be accomplished in O(n) instead of O(n^2) time.
    /// </summary>
    /// <returns></returns>
    public SummarizedState[] SummarizedAutomaton()
    {
        if (_summary != null)
            return _summary;

        var summary = new SummarizedState[NodeCount];
        foreach (var n in NodesBottomUp())
        {

            if (n.NextCount == 1 && !n.IsTerminal)
            {
                var c = summary[n.Next[0].Index];
                summary[n.Index] = new SummarizedState { Node = c.Node, Length = c.Length + 1 };
            }
            else
            {
                summary[n.Index] = new SummarizedState { Node = n, Length = 1 };
            }
        }

        _summary = summary;
        return summary;
    }

    /// <summary>
    /// A state in the compressed automaton
    /// </summary>
    public struct SummarizedState
    {
        /// <summary> the end node of a labeled multicharacter edge </summary>
        public Node Node;
        /// <summary> the number of characters to advance to reach the state </summary>
        public int Length;
        public override string ToString() => $"Node={Node?.Index} Length={Length}";
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var n in GetNodes())
        {
            sb.Append($"{{id:{0}, len:{n.Len}, link:{n.Link?.Index ?? -1}, cloned:{n.IsCloned}, Next:{{");
            sb.Append(string.Join(",", n.Children.Select(c => c.Key + ":" + c.Index)));
            sb.AppendLine("}}");
        }
        return sb.ToString();
    }

    public class Node 
    {
        public char Key;
        public bool IsTerminal;
        public byte NextCount;
        int _keyMask;
        public Node[] Next;

        public int Len;
        public int Index;
        public Node Link;
        public Node Original;
        public static readonly Node[] Empty = new Node[0];

        public Node()
        {
            Next = Empty;
        }

        public int FirstOccurrence => Original?.Len ?? this.Len;

        public bool IsCloned => Original != null;

        public Node Clone()
        {
            var node = (Node)base.MemberwiseClone();
            node.Original = Original ?? this;
            node.Next = (Node[])node.Next.Clone();
            return node;
        }

        public Node this[char ch]
        {
            get
            {
                if ((_keyMask << ~ch) < 0)
                {
                    int left = 0;
                    int right = NextCount - 1;
                    while (left <= right)
                    {
                        int mid = (left + right) >> 1;
                        var val = Next[mid];
                        int cmp = val.Key - ch;
                        if (cmp < 0)
                            left = mid + 1;
                        else if (cmp > 0)
                            right = mid - 1;
                        else
                            return val;
                    }
                }
                return null;
            }
            set
            {
                int left = 0;
                int right = NextCount - 1;
                while (left <= right)
                {
                    int mid = (left + right) >> 1;
                    var val = Next[mid];
                    int cmp = val.Key - ch;
                    if (cmp < 0)
                        left = mid + 1;
                    else if (cmp > 0)
                        right = mid - 1;
                    else
                    {
                        Next[mid] = value;
                        return;
                    }
                }

                if (NextCount >= Next.Length)
                    Array.Resize(ref Next, Math.Max(2, NextCount * 2));
                if (NextCount > left)
                    Array.Copy(Next, left, Next, left + 1, NextCount - left);
                NextCount++;
                Next[left] = value;
                _keyMask |= 1 << ch;
            }
        }

        /// <summary>
        /// Return child nodes
        /// </summary>
        public IEnumerable<Node> Children
        {
            get
            {
                for (int i = 0; i < NextCount; i++)
                    yield return Next[i];
            }
        }

    }
}