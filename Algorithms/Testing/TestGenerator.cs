// https://github.com/MikeMirzayanov/testlib/tree/master/generators
// http://spojtoolkit.com/TestCaseGenerator/

public static class TestGenerator
{
    public const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string Lower = "abcdefghijklmnopqrstuvwxyz";
    public const string Digit = "0123456789";
    public const string Alpha = Upper + Lower;
    public const string Word = Alpha + Digit;
    public const string Alpha_ = Alpha + "_";
    public const string Word_ = Word + "_";
    static readonly Random rand = new();

    public static void Shuffle<T>(IList<T> list, int start, int count)
    {
        for (int end = start + count; start < end; start++) {
            int x = rand.Next(start, end);
            T tmp = list[start];
            list[start] = list[x];
            list[x] = tmp;
        }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        Shuffle(list, 0, list.Count);
    }

    public static int[] IotaInclusive(int start, int end)
    {
        int[] result = new int[end - start + 1];
        for (int i = start; i <= end; i++)
            result[i - start] = i;
        return result;
    }

    public static int[] RandomArray(int size, int min, int max, bool distinct = false)
    {
        var set = new HashSet<int>();
        int[] result = new int[size];
        for (int i = 0; i < size; i++) {
            int v;
            do
                v = rand.Next(min, max + 1);
            while (distinct && !set.Add(v));

            result[i] = v;
        }

        return result;
    }

    public static string Join(this IEnumerable list, string delim = " ") => string.Join(delim, list);

    public static int[] RandomPermutation(int size, int start = 1)
    {
        int[] perm = new int[size];
        for (int i = 0; i < size; i++)
            perm[i] = i + start;
        Shuffle(perm);
        return perm;
    }

    public static int[][] RandomArrayOfPairs(int size,
        int min, int max, int minB, int maxB,
        Func<int, int, bool> constraint = null,
        bool distinct = false) =>
        throw new NotImplementedException();

    public static string RandomString(int n, string chars, bool distinct = false)
    {
        var set = new List<char>(chars);
        var sb = new StringBuilder();
        while (set.Count > 0 && sb.Length < n) {
            int i = rand.Next(set.Count);
            sb.Append(set[i]);
            if (distinct) {
                set[i] = set[set.Count - 1];
                set.RemoveAt(set.Count - 1);
            }
        }

        return sb.ToString();
    }

    public static char[] RandomCharArray(int n, string chars, bool distinct = false) =>
        RandomString(n, chars, distinct).ToCharArray();

    public static string RandomPattern(string s, int n = 0) => throw new NotImplementedException();

    public static string[] RandomStringMatrix(int rows, int cols, string pattern)
    {
        string[] result = new string[rows];
        for (int i = 0; i < result.Length; i++)
            result[i] = RandomString(cols, pattern);
        return result;
    }

    public static char[][] RandomCharMatrix(int rows, int cols, string pattern)
    {
        char[][] result = new char[rows][];
        for (int i = 0; i < result.Length; i++)
            result[i] = RandomCharArray(cols, pattern);
        return result;
    }

    public static int[] Relabel(int[] tree, int[] perm = null, int offset = 0)
    {
        perm = perm ?? RandomPermutation(tree.Length, 0);
        int[] result = new int[tree.Length];

        for (int i = 0; i < tree.Length; i++) {
            int v = tree[i];
            result[perm[i]] = v >= 0 ? perm[v] : -1;
        }

        return result;
    }

    public static int[] RandomTree(int n)
    {
        int[] result = new int[n];
        result[0] = -1;
        for (int i = 1; i < n; i++)
            result[i] = rand.Next(i);
        return Relabel(result);
    }

    public static List<int>[] RandomTreeGraph(int n, int offset = 1)
    {
        var graph = new List<int>[n + offset];
        for (int i = 0; i < n + offset; i++)
            graph[i] = new List<int>();

        for (int i = 0; i < n - 1; i++) {
            int u = i + 1 + offset;
            int v = rand.Next(i + 1) + offset;
            graph[u].Add(v);
            graph[v].Add(u);
        }

        return graph;
    }

    public static int[] BalancedTree(int n, int children = 2)
    {
        int[] result = new int[n];
        int parent = -1;
        int child = children;
        for (int i = 0; i < n; i++) {
            result[i] = parent;
            if (++child >= children) {
                parent++;
                child = 0;
            }
        }

        return Relabel(result);
    }

    public static int[] SimpleChain(int n)
    {
        int[] result = new int[n];
        for (int i = 0; i < n; i++)
            result[i] = i - 1;
        return Relabel(result);
    }

    public static int[] SimpleStar(int n, int rays = 1)
    {
        int[] result = new int[n];
        int brk0 = n / rays;
        int brk = brk0;
        for (int i = 0; i < n; i++)
            if (i < brk)
                result[i] = i - 1;
            else
                brk += brk0;
        return Relabel(result);
    }

    public static List<int>[] TreeToGraph(int[] tree)
    {
        int n = tree.Length;
        var graph = new List<int>[n];
        for (int i = 0; i < n; i++)
            graph[i] = new List<int>();

        for (int u = 0; u < tree.Length; u++) {
            int v = tree[u];
            if (v >= 0) {
                graph[u].Add(v);
                graph[v].Add(u);
            }
        }

        return graph;
    }

    public static List<int>[] RandomGraph(int n, int edges) => throw new NotImplementedException();

    public static List<int>[] RandomBipartiteGraph(int n, int edges) => throw new NotImplementedException();

    public static List<int>[] RandomDirectedGraph(int n, int edges) => throw new NotImplementedException();

    public static long NextLong(ref Random r)
    {
        long result = ((long)r.Next() << 16) ^ r.Next() ^ ((long)r.Next() << 34);
        return result & ~(1L << 63);
    }

    public static ulong NextULong(ref Random r)
    {
        ulong result = ((ulong)r.Next() << 16) ^ (ulong)r.Next() ^ ((ulong)r.Next() << 34);
        return result;
    }

    public static int NextWeightedRandom(int n, int type)
    {
        const int lim = 25;

        if (Math.Abs(type) < lim) {
            int result = rand.Next(n);

            for (int i = 0; i < +type; i++)
                result = Math.Max(result, rand.Next(n));

            for (int i = 0; i < -type; i++)
                result = Math.Min(result, rand.Next(n));

            return result;
        }

        double p = type > 0
            ? Math.Pow(rand.NextDouble(), 1.0 / (type + 1))
            : 1 - Math.Pow(rand.NextDouble(), 1.0 / (-type + 1));

        return (int)(n * p);
    }

    public static string TreeToString(int[] tree, int offset = 1)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < tree.Length; i++)
            if (tree[i] >= 0)
                sb.AppendLine($"{i + 1} {tree[i] + 1}");
        return sb.ToString();
    }
}