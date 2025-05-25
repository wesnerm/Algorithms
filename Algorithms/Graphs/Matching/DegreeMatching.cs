namespace Algorithms.Graphs.Matching;

public static class DegreeMatching
{
    /// <summary>
    ///     Find a pairing between objects
    /// </summary>
    /// <param name="degrees"></param>
    /// <param name="connected"></param>
    /// <returns></returns>
    /// <remarks>
    ///     https://www.codechef.com/JUNE20A/problems/CONVAIR
    /// </remarks>
    public static IEnumerable<Tuple<int, int>> MatchingFromDegree(int[] degrees, bool connected = false)
    {
        degrees = degrees.ToArray();

        int[] sorted = new int[degrees.Length];
        for (int i = 0; i < degrees.Length; i++)
            sorted[i] = i;
        Array.Sort(degrees, sorted);

        int index = 0;
        int end = sorted.Length - 1;
        int maxStart = end;

        // Skip over zeros if not connected
        if (!connected)
            while (index <= end && degrees[index] <= 0)
                index++;

        while (index < end) {
            int maxValue = degrees[end];
            if (maxStart <= index) maxStart = index + 1;
            while (maxStart > index + 1 && degrees[maxStart - 1] >= maxValue) maxStart--;
            while (maxStart < end && degrees[maxStart + 1] < maxValue) maxStart++;

            for (int i = maxStart; i <= end; i++) {
                degrees[i]++;
                degrees[index]++;
                yield return Tuple.Create(sorted[index], sorted[i]);
                if (degrees[index] <= 0)
                    index++;
            }
        }
    }
}