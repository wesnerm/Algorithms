namespace Algorithms.Graphs;

//////////////////////////////////////////////////////////////////////
// Min cost bipartite matching via shortest augmenting paths
//
// This is an O(n^3) implementation of a shortest augmenting path
// algorithm for finding min cost perfect matchings in dense
// graphs.  In practice, it solves 1000x1000 problems in around 1
// second.
//
//   cost[i][j] = cost for pairing left node i with right node j
//   Lmate[i] = index of right node that left node i pairs with
//   Rmate[j] = index of left node that right node j pairs with
//
// The values in cost[i][j] may be positive or negative.  To perform
// maximization, simply negate the cost[][] matrix.
//////////////////////////////////////////////////////////////////////

public class MinCostMatching
{
    public int[] lmate;
    public double MinCost;
    public int[] rmate;

    public MinCostMatching(double[][] cost)
    {
        int n = cost.Length;

        // construct dual feasible solution
        double[] u = new double[n];
        double[] v = new double[n];
        for (int i = 0; i < n; i++) {
            u[i] = cost[i][0];
            for (int j = 1; j < n; j++)
                u[i] = Math.Min(u[i], cost[i][j]);
        }

        for (int j = 0; j < n; j++) {
            v[j] = cost[0][j] - u[0];
            for (int i = 1; i < n; i++)
                v[j] = Math.Min(v[j], cost[i][j] - u[i]);
        }

        // construct primal solution satisfying complementary slackness
        lmate = Repeat(n, -1);
        rmate = Repeat(n, -1);
        int mated = 0;
        for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++) {
            if (rmate[j] != -1) continue;
            if (Math.Abs(cost[i][j] - u[i] - v[j]) < 1e-10) {
                lmate[i] = j;
                rmate[j] = i;
                mated++;
                break;
            }
        }

        double[] dist = new double[n];
        int[] dad = new int[n];
        var seen = new BitArray(n);

        // repeat until primal solution is feasible
        while (mated < n) {
            // find an unmatched left node
            int s = 0;
            while (lmate[s] != -1) s++;

            // initialize Dijkstra
            for (int i = 0; i < dad.Length; i++)
                dad[i] = -1;
            seen.SetAll(false);
            for (int k = 0; k < n; k++)
                dist[k] = cost[s][k] - u[s] - v[k];

            int j;
            while (true) {
                // find closest
                j = -1;
                for (int k = 0; k < n; k++) {
                    if (seen[k]) continue;
                    if (j == -1 || dist[k] < dist[j]) j = k;
                }

                seen[j] = true;

                // termination condition
                if (rmate[j] == -1) break;

                // relax neighbors
                int i = rmate[j];
                for (int k = 0; k < n; k++) {
                    if (seen[k]) continue;
                    double newDist = dist[j] + cost[i][k] - u[i] - v[k];
                    if (dist[k] > newDist) {
                        dist[k] = newDist;
                        dad[k] = j;
                    }
                }
            }

            // update dual variables
            for (int k = 0; k < n; k++) {
                if (k == j || !seen[k]) continue;
                int i = rmate[k];
                v[k] += dist[k] - dist[j];
                u[i] -= dist[k] - dist[j];
            }

            u[s] += dist[j];

            // augment along path
            while (dad[j] >= 0) {
                int d = dad[j];
                rmate[j] = rmate[d];
                lmate[rmate[j]] = j;
                j = d;
            }

            rmate[j] = s;
            lmate[s] = j;

            mated++;
        }

        double value = 0;
        for (int i = 0; i < n; i++)
            value += cost[i][lmate[i]];

        MinCost = value;
    }

    static int[] Repeat(int n, int v)
    {
        int[] array = new int[n];
        for (int i = 0; i < n; i++)
            array[i] = v;
        return array;
    }
}