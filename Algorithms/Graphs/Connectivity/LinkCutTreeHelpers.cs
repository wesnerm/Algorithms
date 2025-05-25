namespace Algorithms.Graphs.TreeGraphs;

public partial class LinkCutTree
{
    // fr.Query(to)
    // fr and to will be at most a distance of two apart
    // because to will be root and fr will have been rotated out of root
    // fr will be the rightmost child of to
    // every element on the left subtree will be on the path between child and parent

    public LinkCutTree FindKth(LinkCutTree to, int k)
    {
        LinkCutTree fr = this;

        // Don't try to optimized these lines -- breaks easily
        int q = fr.Query(to);
        if (q < k) return null;

        while (fr != to) {
            q = fr.Query(to);
            if (k == 1 && fr.Value == 1) return fr;
            if (k == q && to.Value == 1) return to;

            int check;
            LinkCutTree mid = fr._parent;
            if (mid == to) {
                mid = fr._left;
                check = mid.Query(fr);
            } else {
                check = mid._subTreeValue;
            }

            if (k <= check) {
                to = mid;
            } else {
                fr = mid;
                k = k - check + mid.Value;
            }
        }

        return fr.Value == 1 ? fr : null;
    }
}