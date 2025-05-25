namespace Algorithms.Strings;

public class GeneralizedSuffixTree
{
    readonly List<StringSection> sections;
    SuffixTree tree;

    public GeneralizedSuffixTree(params string[] strings)
    {
        sections = new List<StringSection>(strings.Length);
        int pos = 0;
        for (int index = 0; index < strings.Length; index++) {
            string s = strings[index];
            var section = new StringSection
            {
                Index = index,
                String = s,
                Start = pos,
                End = pos + s.Length - 1,
            };
            sections.Add(section);
            pos += s.Length + 1;
        }

        var sb = new StringBuilder(pos);
        foreach (string s in strings) {
            sb.Append(s);
            sb.Append('#');
        }

        string text = sb.ToString();
        tree = new SuffixTree(text);
    }

    public class StringSection
    {
        public int End;
        public int Index;
        public int Start;
        public string String;
    }
}