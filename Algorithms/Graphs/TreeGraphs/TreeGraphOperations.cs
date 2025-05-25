namespace Algorithms.Graphs;

public static class TreeGraphOperations
{
    // This is useful for reordering a tree so that the w and its descendants are processed last
    public static int[] TraceWithWLast(TreeGraph tree, int w)
    {
        int[] newtrace = new int[tree.TreeSize];
        int itrace = 0, inew = 0;
        int iw = tree.Begin[w];
        while (itrace >= 0) {
            int u = tree.Trace[itrace];
            newtrace[inew++] = u;
            int ifound = -1, vSize;
            for (int iv = itrace + 1, end = itrace + tree.Sizes[u]; iv < end; iv += vSize) {
                int v = tree.Trace[iv];
                vSize = tree.Sizes[v];
                if (iv <= iw && iw < iv + vSize) {
                    ifound = iv;
                } else {
                    Array.Copy(tree.Trace, iv, newtrace, inew, vSize);
                    inew += vSize;
                }
            }

            itrace = ifound;
        }

        return newtrace;
    }
}