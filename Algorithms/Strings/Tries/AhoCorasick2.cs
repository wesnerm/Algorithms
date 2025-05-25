namespace Algorithms.Strings;

// http://www.geeksforgeeks.org/aho-corasick-algorithm-pattern-searching/

/// <summary>
///     Implementation of Aho Corasick algorithm for string matching
/// </summary>
public class AhoCorasick2
{
    // Max number of states in the matching machine.
    // Should be equal to the sum of the length of all keywords.
    const int MaxS = 500;

    // Maximum number of characters in input alphabet
    const int MaxC = 26;

    // FAILURE FUNCTION IS IMPLEMENTED USING f[]
    readonly int[] _fail = new int[MaxS];

    // GOTO FUNCTION (OR TRIE) IS IMPLEMENTED USING g[][]
    readonly int[,] _goto = new int[MaxS, MaxC];
    readonly int _length;

    // OUTPUT FUNCTION IS IMPLEMENTED USING outLink[]
    // Bit i in this mask is one if the word with index i
    // appears when the machine enters this state.
    readonly long[] _outIndices = new long[MaxS];
    readonly int _states;

    readonly string[] _words;

    // Builds the string matching machine.
    // arr -   array of words. The index of each keyword is important:
    //         "outLink[state] & (1 << i)" is > 0 if we just found word[i]
    //         in the text.
    // Returns the number of states that the built machine has.
    // States are numbered 0 up to the return value - 1, inclusive.
    public AhoCorasick2(string[] words)
    {
        _words = words;
        _length = words.Length;

        // Initialize all values in goto function as -1.
        for (int i = 0; i < _goto.GetLength(0); i++)
        for (int j = 0; j < _goto.GetLength(1); j++)
            _goto[i, j] = -1;

        // Initially, we just have the 0 state
        int states = 1;

        // Construct values for goto function, i.e., fill g[][]
        // This is same as building a Trie for arr[]
        for (int i = 0; i < _length; ++i) {
            string word = words[i];
            int currentState = 0;

            // Insert all characters of current word in arr[]
            for (int j = 0; j < word.Length; ++j) {
                int ch = word[j] - 'a';

                // Allocate a new node (create a new state) if a
                // node for ch doesn't exist.
                if (_goto[currentState, ch] == -1)
                    _goto[currentState, ch] = states++;

                currentState = _goto[currentState, ch];
            }

            // Add current word in output function
            _outIndices[currentState] |= 1L << i;
        }

        // For all characters which don't have an edge from
        // root (or state 0) in Trie, add a goto edge to state
        // 0 itself
        for (int ch = 0; ch < MaxC; ++ch)
            if (_goto[0, ch] == -1)
                _goto[0, ch] = 0;

        // Now, let's build the failure function

        // Initialize values in fail function
        for (int i = 0; i < _fail.Length; i++)
            _fail[i] = -1;

        // Failure function is computed in breadth first order
        // using a queue
        var q = new Queue<int>();

        // Iterate over every possible input
        for (int ch = 0; ch < MaxC; ++ch)
            // All nodes of depth 1 have failure function value
            // as 0. For example, in above diagram we move to 0
            // from states 1 and 3.
            if (_goto[0, ch] != 0) {
                _fail[_goto[0, ch]] = 0;
                q.Enqueue(_goto[0, ch]);
            }

        // Now queue has states 1 and 3
        while (q.Count > 0) {
            // Remove the front state from queue
            int state = q.Dequeue();

            // For the removed state, find failure function for
            // all those characters for which goto function is
            // not defined.
            for (int ch = 0; ch <= MaxC; ++ch)
                // If goto function is defined for character 'ch'
                // and 'state'
                if (_goto[state, ch] != -1) {
                    // Find failure state of removed state
                    int failure = _fail[state];

                    // Find the deepest node labeled by proper
                    // suffix of string from root to current
                    // state.
                    while (_goto[failure, ch] == -1)
                        failure = _fail[failure];

                    failure = _goto[failure, ch];
                    _fail[_goto[state, ch]] = failure;

                    // Merge output values
                    _outIndices[_goto[state, ch]] |= _outIndices[failure];

                    // Insert the next level node (of Trie) in Queue
                    q.Enqueue(_goto[state, ch]);
                }
        }

        _states = states;
    }

    // Returns the next state the machine will transition to using goto
    // and failure functions.
    // currentState - The current state of the machine. Must be between
    //                0 and the number of states - 1, inclusive.
    // nextInput - The next character that enters into the machine.
    int FindNextState(int currentState, char nextInput)
    {
        int answer = currentState;
        int ch = nextInput - 'a';

        // If goto is not defined, use failure function
        while (_goto[answer, ch] == -1)
            answer = _fail[answer];

        return _goto[answer, ch];
    }

    // This function finds all occurrences of all array words
    // in text.
    IEnumerable<Tuple<int, int>> SearchWords(string text)
    {
        // Initialize current state
        int currentState = 0;

        // Traverse the text through the nuilt machine to find
        // all occurrences of words in arr[]
        for (int i = 0; i < text.Length; ++i) {
            currentState = FindNextState(currentState, text[i]);
            if (_outIndices[currentState] == 0)
                continue;

            for (int j = 0; j < _length; ++j)
                if ((_outIndices[currentState] & (1L << j)) != 0)
                    yield return new Tuple<int, int>(i, j);
        }
    }

    // Driver program to test above
    public static void Main()
    {
        string[] arr = { "he", "she", "hers", "his" };
        string text = "ahishers";

        var trie = new AhoCorasick2(arr);
        foreach (Tuple<int, int> s in trie.SearchWords(text)) {
            int i = s.Item1;
            int j = s.Item2;

            Console.WriteLine($"Word {arr[j]} appears from {i - arr[j].Length + 1} to {i}");
        }
    }
}