namespace Algorithms.Strings.Tries;

// SOURCE: http://codeforces.com/blog/entry/14854
// SOURCE: http://ideone.com/J1XjX6
class AhoCorasick3
{
    const int letterCount = 26;
    readonly int[] fail;
    readonly int[] len;

    readonly int[] term;
    readonly int[,] to;
    int states = 1;

    public AhoCorasick3(int maxN)
    {
        term = new int[maxN];
        len = new int[maxN];
        to = new int[maxN, letterCount];
        fail = new int[maxN];
    }

    void AddString(string s)
    {
        int current = 0;
        foreach (char c in s) {
            if (to[current, c - 'a'] == 0) {
                to[current, c - 'a'] = states++;
                len[to[current, c - 'a']] = len[current] + 1;
            }

            current = to[current, c - 'a'];
        }

        term[current] = current;
    }

    void PushLinks()
    {
        int[] queue = new int[states];
        int start = 0, end = 1;
        queue[0] = 0;
        while (start < end) {
            int state = queue[start++];
            int failure = fail[state];
            if (term[state] == 0) term[state] = term[failure];
            for (int ch = 0; ch < letterCount; ch++)
                if (to[state, ch] != 0) {
                    fail[to[state, ch]] = state != 0 ? to[failure, ch] : 0;
                    queue[end++] = to[state, ch];
                } else {
                    to[state, ch] = to[failure, ch];
                }
        }
    }
}