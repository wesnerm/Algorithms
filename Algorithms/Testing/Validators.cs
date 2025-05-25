using Algorithms.Collections;
using Algorithms.Graphs;

public static class Validators
{
    public static bool ValidTree(int[] tree)
    {
        var uf = new UnionFind(tree.Length);

        for (int i = 0; i < tree.Length; i++) {
            if (tree[i] < 0) continue;
            uf.Union(tree[i], i);
        }

        return uf.Count == 1 && GetTreeRoot(tree) != -1;
    }

    public static int EdgeCount(List<int>[] graph)
    {
        int count = 0;
        for (int i = 0; i < graph.Length; i++) {
            if (graph[i] == null) continue;
            count += graph[i].Count;
        }

        return count;
    }

    public static bool ValidTreeGraph(List<int>[] graph, int offset = 0)
    {
        var uf = new TreeGraph(graph, offset);
        int size = graph.Length - offset;
        return EdgeCount(graph) == size - 1 && uf.TreeSize != size;
    }

    public static bool ValidTreeDigraph(List<int>[] graph, int offset = 0)
    {
        var uf = new TreeGraph(graph, offset);
        int size = graph.Length - offset;
        if (EdgeCount(graph) == 2 * size - 1 && uf.TreeSize != size)
            return false;

        // TODO: Check for multiple edges
        for (int i = offset; i < graph.Length; i++) { }

        return true;
    }

    public static bool ConnectedGraph(List<int>[] graph, int offset = 0)
    {
        var uf = new TreeGraph(graph, offset);
        return uf.TreeSize == graph.Length - offset;
    }

    public static int GetTreeRoot(int[] tree)
    {
        int root = -1;
        for (int i = 0; i < tree.Length; i++)
            if (tree[i] < 0) {
                if (root != -1)
                    return -1;
                root = i;
            }

        return root;
    }

    public static int[] RemoveTreeOffset(int[] tree, int offset = 1)
    {
        int[] newTree = new int[tree.Length - offset];
        Array.Copy(tree, offset, newTree, 0, newTree.Length);
        for (int i = 0; i < newTree.Length; i++)
            if (newTree[i] >= offset)
                newTree[i] -= offset;
            else
                newTree[i] = -1;
        return newTree;
    }
}