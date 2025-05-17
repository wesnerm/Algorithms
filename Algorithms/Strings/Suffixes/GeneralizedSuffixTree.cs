using System.Text;

namespace Algorithms.Strings;

public class GeneralizedSuffixTree
{
    SuffixTree tree;
    List<StringSection> sections;

    public GeneralizedSuffixTree(params string [] strings)
    {
        sections = new List<StringSection>(strings.Length);
        int pos = 0;
        for (var index = 0; index < strings.Length; index++)
        {
            var s = strings[index];
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
        foreach(var s in strings)
        {
            sb.Append(s);
            sb.Append('#');
        }

        var text = sb.ToString();
        tree = new SuffixTree(text);
    }

    public class StringSection
    {
        public int Index;
        public string String;
        public int Start;
        public int End;
    }

}
