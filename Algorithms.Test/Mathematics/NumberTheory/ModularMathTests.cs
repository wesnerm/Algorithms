
namespace Algorithms.Mathematics;

public class ModularMathTests
{

    [Test]
    public void InversesTest()
    {
        var mod = 1000000007;
        var inverses = ModularMath.Inverses(50, mod);

        for (int i = 1; i < inverses.Length; i++)
            Assert.AreEqual(1L, 1L * inverses[i] * i % mod);
    }

}
