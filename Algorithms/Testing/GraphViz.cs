namespace Algorithms.Graphs;

public class Dot
{
    readonly StringBuilder builder = new();
    public bool GoogleCharts;

    public Dot Id(object nameObj)
    {
        string? name = nameObj.ToString();
        if (IsId(name)) {
            builder.Append(name);
        } else {
            builder.Append('"');
            builder.Append(Escaped(name));
            builder.Append('"');
        }

        return this;
    }

    public Dot Append(object text)
    {
        builder.Append(text);
        return this;
    }

    public bool IsId(string name) => true;

    public string Escaped(string name) => name;

    public override string ToString()
    {
        if (GoogleCharts) {
            builder.Insert(0, "http://chart.apis.google.com/chart?cht=gv:dot&chl=digraph{");
            builder.Append("}");
        } else {
            var preamble = new StringBuilder();
            preamble.AppendLine("digraph{");
            preamble.AppendLine("graph[rankdir=LR];");
            preamble.AppendLine("node[shape=circle];");
            builder.Insert(0, preamble);
            builder.AppendLine("}");
        }

        return builder.ToString();
    }
}