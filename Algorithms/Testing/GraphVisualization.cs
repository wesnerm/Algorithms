using Algorithms.Strings;

namespace Algorithms.Graphs;

public static class GraphVisualization
{
    static void Emit(StringBuilder sb, IEnumerable nodeLabels)
    {
        if (nodeLabels == null) return;
        int i = 0;
        foreach (object? obj in nodeLabels)
            sb.AppendLine($"{i++} [label=\"{obj}\"]");
    }

    public static string DrawParents(int[] parents, IEnumerable nodeLabels = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph {");
        Emit(sb, nodeLabels);
        for (int i = 0; i < parents.Length; i++) {
            int v = parents[i];
            if (v >= 0)
                sb.AppendLine($"{v} -> {i};");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string DrawDirectedGraph(IList<int>[] graph, IEnumerable nodeLabels = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph {");
        Emit(sb, nodeLabels);
        for (int i = 0; i < graph.Length; i++)
            foreach (int v in graph[i])
                sb.AppendLine($"{i} -> {v};");
        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string DrawGraph(IList<int>[] graph, string prefix = "", IEnumerable nodeLabels = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph {");
        Emit(sb, nodeLabels);
        for (int i = 0; i < graph.Length; i++)
            foreach (int v in graph[i])
                if (v >= i)
                    sb.AppendLine($"{prefix}{i} -- {prefix}{v};");
        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string DrawGraphInner(IList<int>[] graph, string prefix = "", IEnumerable nodeLabels = null)
    {
        var sb = new StringBuilder();
        Emit(sb, nodeLabels);
        for (int i = 0; i < graph.Length; i++)
            foreach (int v in graph[i])
                if (v >= i)
                    sb.AppendLine($"{prefix}{i} -- {prefix}{v};");
        return sb.ToString();
    }

    public static string DrawWeightedGraph(IList<int>[] graph, IList<int>[] edgeWeights, IEnumerable nodeLabels = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph {");
        Emit(sb, nodeLabels);
        for (int i = 0; i < graph.Length; i++)
        for (int j = 0; j < graph[i].Count; j++) {
            int v = graph[i][j];
            int w = edgeWeights[i][j];
            if (v >= i) sb.AppendLine($"{i} -- {v} [label=\"{w}\"];");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string DrawWeightedDigraph(IList<int>[] graph, IList<int>[] edgeWeights,
        IEnumerable nodeLabels = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph {");
        Emit(sb, nodeLabels);
        for (int i = 0; i < graph.Length; i++)
        for (int j = 0; j < graph[i].Count; j++) {
            int v = graph[i][j];
            int w = edgeWeights[i][j];
            sb.AppendLine($"{i} -> {v} [label=\"{w}\"];");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string PrintGraph(IList<int>[] graph, bool directed = false)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < graph.Length; i++)
            foreach (int v in graph[i])
                if (directed || v >= i)
                    sb.AppendLine($"{i} {v}");
        return sb.ToString();
    }

    public static string Join<T>(this IList<T> array, string join = " ") => string.Join(join, array);

    public static string ToDot(SuffixAutomaton suffixAutomaton, bool next, bool suffixLink, bool googleCharts)
    {
        var dot = new Dot();
        foreach (var n in suffixAutomaton.GetNodes())
            if (n != null) {
                if (suffixLink && n.Link != null)
                    dot.Id(suffixAutomaton.Label(n))
                        .Append("->")
                        .Id(suffixAutomaton.Label(n.Link))
                        .Append("[style=dashed];\n");
                // Google Charts
                // .Append("],");
                if (next && n.Next != null)
                    for (int i = 0; i < n.NextCount; i++)
                        dot.Id(suffixAutomaton.Label(n))
                            .Append("->")
                            .Id(suffixAutomaton.Label(n.Next[i]))
                            .Append("[label=")
                            .Id(n.Next[i].Key)
                            .Append("];\n");
                // Google Charts                                 
                // .Append("],");
            }

        dot.GoogleCharts = googleCharts;
        return dot.ToString();
    }
}