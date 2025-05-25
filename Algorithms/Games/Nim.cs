namespace Algorithms.Games;

public static class Nim
{
    /// <summary>
    ///     Minimum Excludant.
    /// </summary>
    /// <param name="set">The set.</param>
    /// <returns></returns>
    public static int Mex(ISet<int> set)
    {
        int n = 0;
        while (set.Contains(n)) n++;
        return n;
    }

    /// <summary>
    ///     Minimum Excludant.
    /// </summary>
    /// <param name="set">The set.</param>
    /// <returns></returns>
    public static int Mex(BitArray set)
    {
        int n = 0;
        while (set[n]) n++;
        return n;
    }

    public static int NimGame(IEnumerable<int> piles)
    {
        return piles.Aggregate(0, (a, b) => a ^ b);
    }

    public static int NimGame(IEnumerable<int> piles, Func<int, int> func)
    {
        return piles.Select(func).Aggregate(0, (a, b) => a ^ b);
    }

    public static int StaircaseNim(IEnumerable<int> piles)
    {
        bool odd = true;
        int grundy = 0;
        foreach (int v in piles) {
            if (odd)
                grundy ^= v;
            odd = !odd;
        }

        return grundy;
    }

    public static int MisereNim(IEnumerable<int> piles)
    {
        int grundy = NimGame(piles);
        if (piles.Max() > 1)
            return grundy;
        return grundy == 0 ? 1 : 0;
    }

    //https://www.hackerrank.com/contests/ncr-codesprint/challenges/game-of-numbers/copy-from/7720003

    /// <summary>
    ///     In the subtraction game, players wins by removing coins up to and past zero.
    ///     Players can only move lo to hi coins.
    /// </summary>
    /// <param name="hi">The hi.</param>
    /// <param name="lo">The lo.</param>
    /// <param name="k">The k.</param>
    /// <returns></returns>
    public static bool SubtractionGame(int hi, int lo, int k)
    {
        if (k <= 0) return false;
        if (k <= hi) return true;
        int kk = k - (hi + 1);
        kk %= lo + hi;
        return kk >= lo;
    }

    public static IEnumerable<int[]> WythoffsGamePPositions()
    {
        double gr = (1 + Math.Sqrt(5)) / 2;
        for (int n = 0;; n++) {
            int c0 = (int)Math.Floor(n * gr);
            int c1 = (int)Math.Floor(n * gr * gr);
            yield return new[] { c0, c1 };
        }
    }

    /// <summary>
    ///     With a rooted tree, use the colon principle:
    ///     A vertex with several stalks is replaced by the nimsum
    ///     of the lengths of the stalks
    /// </summary>
    /// <param name="e">The edges</param>
    /// <param name="v">The root</param>
    /// <returns></returns>
    // https://www.hackerrank.com/contests/5-days-of-game-theory/challenges/deforestation/copy-from/7904155
    static int GreenHackenbush(HashSet<int>[] e, int v)
    {
        int grundy = 0;
        foreach (int v2 in e[v])
            grundy ^= GreenHackenbush(e, v2) + 1;
        return grundy;
    }
}