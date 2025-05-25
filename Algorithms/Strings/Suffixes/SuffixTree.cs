namespace Algorithms.Strings;

// SOURCE: http://www.geeksforgeeks.org/ukkonens-suffix-tree-construction-part-6/

public class SuffixTree
{
    readonly int _charRange;

    readonly int _charStart;
    readonly EndHolder _leafEnd = new(-1);
    public readonly Node Root;
    public readonly string Text;

    int _activeEdge;
    int _activeLength;
    Node _activeNode;

    Node _lastNewNode;

    int _remainingSuffixCount;
    int _size;
    EndHolder _splitEnd;

    public SuffixTree(string text)
    {
        Text = text;

        _charStart = text.Length == 0 ? 0 : int.MaxValue;
        int charEnd = 0;
        foreach (char c in text) {
            _charStart = Math.Min(_charStart, c);
            charEnd = Math.Max(charEnd, c);
        }

        _charRange = charEnd - _charStart + 1;

        _activeEdge = -1;
        _size = text.Length;
        _activeNode = Root = new Node(this, -1, new EndHolder(-1));
        for (int i = 0; i < _size; i++)
            ExtendSuffixTree(i);
        int labelHeight = 0;
        SetSuffixIndexByDfs(Root, labelHeight);
    }

    bool WalkDown(Node currNode)
    {
        if (_activeLength < currNode.EdgeLength) return false;
        _activeEdge += currNode.EdgeLength;
        _activeLength -= currNode.EdgeLength;
        _activeNode = currNode;
        return true;
    }

    void CloseTree() { }

    void ExtendSuffixTree(int pos)
    {
        _leafEnd.Value = pos;

        _remainingSuffixCount++;
        _lastNewNode = null;

        while (_remainingSuffixCount > 0) {
            if (_activeLength == 0)
                _activeEdge = pos;

            if (_activeNode[Text[_activeEdge]] == null) {
                //Extension Rule 2 (A new leaf edge gets created)
                _activeNode[Text[_activeEdge]] = new Node(this, pos, _leafEnd);

                if (_lastNewNode != null) {
                    _lastNewNode.Link = _activeNode;
                    _lastNewNode = null;
                }
            } else {
                Node next = _activeNode[Text[_activeEdge]];
                if (WalkDown(next))
                    continue;

                if (Text[next.Start + _activeLength] == Text[pos]) {
                    if (_lastNewNode != null && _activeNode != Root) {
                        _lastNewNode.Link = _activeNode;
                        _lastNewNode = null;
                    }

                    _activeLength++;
                    break;
                }

                _splitEnd = new EndHolder(next.Start + _activeLength - 1);

                var split = new Node(this, next.Start, _splitEnd);
                _activeNode[Text[_activeEdge]] = split;

                split[Text[pos]] = new Node(this, pos, _leafEnd);
                next.Start += _activeLength;
                split[Text[next.Start]] = next;

                if (_lastNewNode != null)
                    _lastNewNode.Link = split;

                _lastNewNode = split;
            }

            _remainingSuffixCount--;
            if (_activeNode == Root && _activeLength > 0) //APCFER2C1
            {
                _activeLength--;
                _activeEdge = pos - _remainingSuffixCount + 1;
            } else if (_activeNode != Root) //APCFER2C2
            {
                _activeNode = _activeNode.Link;
            }
        }
    }

    void SetSuffixIndexByDfs(Node n, int labelHeight)
    {
        if (n == null)
            return;

        bool leaf = true;
        foreach (var c in n.Children)
            if (c != null) {
                leaf = false;
                SetSuffixIndexByDfs(c, labelHeight + c.EdgeLength);
            }

        if (leaf)
            //for (int i = n.Start; i <= n.End; i++)
            //{
            //    if (Text[i] == '#') //Trim unwanted characters
            //        n.End = i;
            //}
            n.SuffixIndex = _size - labelHeight;
    }

    public void OutputSuffixTree(Node n, StringBuilder builder)
    {
        if (n == null)
            return;

        if (n.Start != -1)
            builder.Append($"{n.Start} {n.End} ");

        bool leaf = true;
        foreach (var c in n.Children)
            if (c != null) {
                leaf = false;
                OutputSuffixTree(c, builder);
            }

        if (leaf)
            builder.AppendLine($" {n.SuffixIndex}");
    }

    /// <summary>
    ///     Returns a view of the trie
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var builder = new StringBuilder();
        OutputSuffixTree(Root, builder);
        return builder.ToString();
    }

    /// <summary>
    ///     Determines whether substring is contain in the trie string
    /// </summary>
    /// <param name="substring"></param>
    /// <returns></returns>
    public bool ContainsSubstring(string substring)
    {
        int index;
        return FindNode(substring, out index) != null;
    }

    /// <summary>
    ///     Finds the node containing the substring and returns ending position+1
    /// </summary>
    /// <param name="substring"></param>
    /// <param name="index">1+the position of ending char in the node</param>
    /// <returns>the found node</returns>
    public Node FindNode(string substring, out int index)
    {
        index = 0;

        Node? n = Root;
        int i = 0;
        while (n != null) {
            if (n.Start != -1) {
                int j = i;
                int k = n.Start;
                for (; k <= n.End && j < substring.Length; k++, j++)
                    if (Text[k] != substring[j])
                        return null;

                if (j == substring.Length) {
                    index = k;
                    return n;
                }
            }

            i = i + n.EdgeLength;
            n = n[substring[i]];
        }

        return null;
    }

    void Dfs(Node n, Func<Node, bool> predicate)
    {
        var stack = new Stack<Node>();
        stack.Push(n);

        while (stack.Count > 0) {
            Node pop = stack.Pop();
            if (predicate(pop))
                return;
            PushChildren(stack, pop);
        }
    }

    void PushChildren(Stack<Node> stack, Node n)
    {
        Node[] children = n.Next;
        for (int i = n.NextCount - 1; i >= 0; i--) {
            Node? c = children[i];
            if (c != null)
                stack.Push(c);
        }
    }

    /// <summary>
    ///     Returns the start indices of all occurrences of the substring
    /// </summary>
    /// <param name="substring"></param>
    /// <returns></returns>
    public IEnumerable<int> IndicesOf(string substring)
    {
        int index;
        Node? n = FindNode(substring, out index);
        if (n == null) yield break;

        var stack = new Stack<Node>();
        stack.Push(n);
        while (stack.Count > 0) {
            Node pop = stack.Pop();
            if (pop.SuffixIndex >= 0) {
                yield return pop.SuffixIndex;
                continue;
            }

            PushChildren(stack, pop);
        }
    }

    void DfsRepeatedSubstring(Node n, int length, ref int start, ref int maxLength)
    {
        if (n == null)
            return;

        if (n.SuffixIndex == -1) //If it is internal node
        {
            foreach (var c in n.Children)
                if (c != null)
                    DfsRepeatedSubstring(c, length + c.EdgeLength, ref start, ref maxLength);
        } else if (n.SuffixIndex >= 0 && maxLength < length - n.EdgeLength) {
            maxLength = length - n.EdgeLength;
            start = n.SuffixIndex;
        }
    }

    /// <summary>
    ///     Finds the longest repeated substring in text
    /// </summary>
    /// <param name="start">position of some longest repeated string </param>
    /// <param name="maxLength">length of longest repeated string or 0 if none</param>
    public void LongestRepeatedSubstring(out int start, out int maxLength)
    {
        start = 0;
        maxLength = 0;
        DfsRepeatedSubstring(Root, 0, ref start, ref maxLength);
    }

    #region Build Suffix Array

    void DfsSuffixArray(Node n, int[] suffixArray, ref int idx)
    {
        if (n == null)
            return;

        if (n.SuffixIndex == -1) //If it is internal node
        {
            foreach (var c in n.Children)
                if (c != null)
                    DfsSuffixArray(c, suffixArray, ref idx);
        }
        //If it is Leaf node other than "$" label
        else if (n.SuffixIndex > -1 && n.SuffixIndex < _size) {
            suffixArray[idx++] = n.SuffixIndex;
        }
    }

    public int[] GetSuffixArray()
    {
        _size--;
        int[] suffixArray = new int[_size];
        for (int i = 0; i < _size; i++)
            suffixArray[i] = -1;
        int idx = 0;
        DfsSuffixArray(Root, suffixArray, ref idx);
        _size++;
        return suffixArray;
    }

    #endregion

    #region Helpers

    public class Node : TrieNode<Node>
    {
        public readonly SuffixTree Tree;
        EndHolder _endHolder;
        public Node Link;
        public int Start;

        public int SuffixIndex = -1;

        public Node(SuffixTree tree, int start, EndHolder endHolder)
        {
            Tree = tree;
            Link = tree.Root;
            Start = start;
            _endHolder = endHolder;
        }

        public int End {
            get => _endHolder.Value;
            set => _endHolder = new EndHolder(value);
        }

        public int EdgeLength => this == Tree.Root ? 0 : _endHolder.Value - Start + 1;
    }

    public class EndHolder
    {
        public int Value;

        public EndHolder(int value) => Value = value;
    }

    #endregion
}