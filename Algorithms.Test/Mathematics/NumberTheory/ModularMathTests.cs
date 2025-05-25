namespace Algorithms.Mathematics;

public class ModularMathTests
{
    [Test]
    public void InversesTest()
    {
        int mod = 1000000007;
        int[] inverses = ModularMath.Inverses(50, mod);

        for (int i = 1; i < inverses.Length; i++)
            AreEqual(1L, 1L * inverses[i] * i % mod);
    }
}