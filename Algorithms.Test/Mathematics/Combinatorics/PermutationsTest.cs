namespace Algorithms.Mathematics;

using static Permutations;

[TestFixture]
public class PermutationRankTest
{
    /// <summary>
    ///     Test for Main()
    /// </summary>
    [Test]
    public void GetRankTest()
    {
        CheckFunction(GetRank);
    }

    public void CheckFunction(Func<string, int> func)
    {
        AreEqual(2, func("bac"));
        AreEqual(0, func("aaa"));
        AreEqual(2, func("abba"));
        AreEqual(60, func("caabbc"));
        AreEqual(352781740, func("axaelixedhtshsixbuzouqtjrkpyafthezfuehcovcqlbvmkbrwxhzrxymricmehktxepyxomxcx"));
    }

    [Test]
    public void NextCombinationTest2()
    {
        for (int n = 1; n <= 5; n++)
        for (int k = 0; k <= n; k++) {
            long v = InitCombination(n, k);
            AreEqual(k, BitTools.BitCount(v));

            int count = 1;
            while ((v = NextCombination(v)) != 0) {
                AreEqual(k, BitTools.BitCount(v));
                count++;
            }

            AreEqual(Comb(n, k), count, $"Comb({n},{k}) != {count}");
        }
    }
    // private PermutationRank sample;
}