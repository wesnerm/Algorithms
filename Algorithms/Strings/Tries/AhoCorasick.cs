namespace Algorithms.Strings;
#if false
    partial class AhoCorasick
    {

        static SimpleTrie root = new SimpleTrie();

        // https://www.hackerrank.com/challenges/two-two/submissions/code/33967065

        static AhoCorasick()
        {
            var twos = new List<string>();
            for (int i = 0; i <= 800; i++)
                twos.Add(BigInteger.Pow(2, i).ToString());
            twos.Sort((a, b) => a.Length - b.Length);

            foreach (var v in twos)
                root.Insert(v);

            foreach (var v in twos)
            {
                var trie1 = new HashSet<SimpleTrie>() { root };
                var trie2 = new HashSet<SimpleTrie>();
                var main = root;

                char lastc = '\0';
                foreach (var c in v)
                {
                    trie2.Add(root);

                    var best = (lastc != 0 ? root.MoveNext(lastc) : null) ?? root;
                    int count = 0;
                    var matches = new List<string>();
                    foreach (var n in trie1)
                    {
                        if (n != main && best.Depth < n.Depth)
                            best = n;

                        var node = n.MoveNext(c);
                        if (node != null)
                        {
                            trie2.Add(node);
                            if (node.word != null)
                            {
                                matches.Add(node.word);
                                count++;
                            }
                        }
                    }

                    main.fail = best;
                    main = main.MoveNext(c);
                    main.counts = count;
                    main.matches = matches;
                    count = 0;

                    var tmp = trie1;
                    trie1 = trie2;
                    trie2 = tmp;
                    trie2.Clear();
                    lastc = c;
                }

                var best2 = root;
                foreach (var n in trie1)
                {
                    if (n != main && best2.Depth < n.Depth)
                        best2 = n;
                }
                main.fail = best2;
            }
        }

        static void Main(String[] args)
        {
            int tc = int.Parse(Console.ReadLine());

            var trie1 = new HashSet<SimpleTrie>() { root };
            var trie2 = new HashSet<SimpleTrie>();
            var dict = new Dictionary<string, int>();
            var dict2 = new Dictionary<string, int>();
            while (tc-- > 0)
            {
                var s = Console.ReadLine();
                int count = 0;
                dict.Clear();
                dict2.Clear();

                var main = root;
                foreach (var c in s)
                {
                    if (Verbose) Console.WriteLine($"{main} applying {c}");
                    while (true)
                    {
                        var node = main.MoveNext(c);
                        if (node != null)
                        {
                            if (Verbose) Console.WriteLine($"{main} Advancing to {node}");
                            main = node;
                            break;
                        }
                        if (main == root)
                            break;
                        if (main.fail == null) Console.WriteLine($"{main.fail} has no fail");
                        main = main.fail ?? root;
                        if (Verbose) Console.WriteLine($"Failing to {main}");
                    }
                    if (Verbose && main.counts > 0) Console.WriteLine($"Counting: {main}->{string.Join(",", main.matches)}");

                    if (main.counts > 0)
                    {
                        /*
                        foreach(var m in main.matches)
                        {
                            if (m=="64") Console.WriteLine($"64 Found in {main}");
                            dict2[m] = dict2.ContainsKey(m) ? dict2[m]+1 : 1;
                        }*/
                    }

                    count += main.counts;
                }

                Console.WriteLine(count);

                /*
                count = 0;
                trie1.Clear();
                trie2.Clear();
                trie1.Add(root);
                foreach(var c in s)
                {
                    trie2.Add(root);
                    foreach(var n in trie1)
                    {
                        var node = n.MoveNext(c);
                        if (node != null)
                        {
                            trie2.Add(node);
                            if (node.word != null) 
                            {
                                count++;
                                dict[node.word] = dict.ContainsKey(node.word) ? dict[node.word]+1 : 1;
                            }
                        }
                    }

                    var tmp = trie1;
                    trie1 = trie2;
                    trie2 = tmp;
                    trie2.Clear();
                }

                foreach(var v in dict2.Keys.OrderBy(x=>x))
                {
                    if (dict[v] != dict2[v])
                        Console.WriteLine($"{v}->{dict[v]} {dict2[v]}");
                }
                Console.WriteLine(count);
                */
            }
        }

        public static bool Verbose = false;

        // Test case
        // 1
        // 12089258196146291747061764
        // Answer: 15
    }
#endif