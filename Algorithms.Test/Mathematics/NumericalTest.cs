using static Algorithms.Mathematics.EuclidAlgo;

namespace Algorithms.Mathematics;

[TestFixture]
public class NumericalTest
{
    [Test]
    public void EuclidTest()
    {
        AreEqual(2, EuclidAlgo.Gcd(14, 30));

        // expected: 2 -2 1
        int x, y;
        int d = ExtendedEuclid(14, 30, out x, out y);
        AreEqual(2, d);
        AreEqual(-2, x);
        AreEqual(1, y);

        // expected: 95 45
        List<int> sols = ModularLinearEquationSolver(14, 30, 100);
        AreEqual(2, sols.Count);
        AreEqual(95, sols[0]);
        AreEqual(45, sols[1]);

        // expected: 8
        AreEqual(8, ModularMath.ModInverse(8, 9));

        // expected: 23 56
        //           11 12
        var xs = new List<int> { 3, 5, 7, 4, 6 };
        var a = new List<int> { 2, 3, 2, 3, 5 };
        (int, int) ret = ChineseRemainderTheorem(xs.GetRange(0, 3), a.GetRange(0, 3));
        AreEqual(23, ret.Item1);
        AreEqual(56, ret.Item2);
        ret = ChineseRemainderTheorem(xs.GetRange(3, 2), a.GetRange(3, 2));
        AreEqual(11, ret.Item1);
        AreEqual(12, ret.Item2);

        // expected: 5 -15
        LinearDiophantine(7, 2, 5, out x, out y);
        AreEqual(5, x);
        AreEqual(-15, y);
    }

    /*
    [Test]
    [Ignore("")]
    public static void FftTest()
    {
        Console.Write("If rows come in identical pairs, then everything works.\n");

        var a = new[] { 0, 1, new Complex(1, 3), new Complex(0, 5), 1, 0, 2, 0 };
        var b = new[] { 1, new Complex(0, -2), new Complex(0, 1), 3, -1, -3, 1, -2 };
        var A = new Complex[8];
        var B = new Complex[8];
        FastFourierTransform.Transform(a, A, 8, 1, 1, 0, 0);
        FastFourierTransform.Transform(b, B, 8, 1, 1, 0, 0);

        for (var i = 0; i < 8; i++)
        {
            Console.Write(A[i] + " ");
        }
        Console.WriteLine();
        for (var i = 0; i < 8; i++)
        {
            var Ai = new Complex(0, 0);
            for (var j = 0; j < 8; j++)
            {
                Ai = Ai + a[j] * Complex.Exp(j * i * 2 * Math.PI / 8);
            }
            Console.Write(Ai + " ");
        }
        Console.WriteLine();

        var AB = new Complex[8];
        for (var i = 0; i < 8; i++)
            AB[i] = A[i] * B[i];
        var aconvb = new Complex[8];
        FastFourierTransform.Transform(AB, aconvb, 8, -1, 1, 0, 0);
        for (var i = 0; i < 8; i++)
            aconvb[i] = aconvb[i] / 8;
        for (var i = 0; i < 8; i++)
        {
            Console.Write(aconvb[i] + " ");
        }
        Console.WriteLine();

        for (var i = 0; i < 8; i++)
        {
            Complex aconvbi = 0;
            for (var j = 0; j < 8; j++)
                aconvbi = aconvbi + a[j] * b[(8 + i - j) % 8];

            Console.Write(aconvbi + " ");
        }
        Console.WriteLine();

    }*/

    [Test]
    [Ignore("")]
    public void GaussJordanTest()
    {
        const int n = 4;
        const int m = 2;
        double[,] a = new double[,]
        {
            { 1, 2, 3, 4 },
            { 1, 0, 1, 0 },
            { 5, 3, 2, 4 },
            { 6, 1, 4, 6 },
        };

        double[,] b = new double[,]
        {
            { 1, 2 },
            { 4, 3 },
            { 5, 6 },
            { 8, 7 },
        };

        double det = LinearAlgebra.GaussJordan(a, b);

        // expected: 60  
        Console.WriteLine("Determinant: " + det);

        // expected: -0.233333 0.166667 0.133333 0.0666667
        //           0.166667 0.166667 0.333333 -0.333333
        //           0.233333 0.833333 -0.133333 -0.0666667
        //           0.05 -0.75 -0.1 0.2
        Console.WriteLine("Inverse: ");
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++)
                Console.Write(a[i, j] + " ");
            Console.WriteLine();
        }

        // expected: 1.63333 1.3
        //           -0.166667 0.5
        //           2.36667 1.7
        //           -1.85 -1.35
        Console.WriteLine("Solution: ");
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < m; j++)
                Console.Write(b[i, j] + " ");
            Console.WriteLine();
        }
    }

    [Test]
    public void SimplexTest()
    {
        double[,] A = new double[,]
        {
            { 6, -1, 0 },
            { -1, -5, 0 },
            { 1, 5, 1 },
            { -1, -5, -1 },
        };
        double[] b = new double[] { 10, -4, 5, -5 };
        double[] c = new double[] { 1, -1, 0 };

        var solver = new LpSolver(A, b, c);
        double value = solver.Answer;
        double[] x = solver.OptimalVariables;

        // VALUE: 1.29032
        AreEqual(1.29032, value);

        // SOLUTION: 1.74194 0.451613 1
        AreEqual(3, x.Length);
        AreEqual(1.74194, x[0]);
        AreEqual(0.451613, x[1]);
        AreEqual(1, x[2]);
    }

    [Test]
    [Ignore("")]
    public void RrefTest()
    {
        double[,] a = new double[,]
        {
            { 16, 2, 3, 13 },
            { 5, 11, 10, 8 },
            { 9, 7, 6, 12 },
            { 4, 14, 15, 1 },
            { 13, 21, 21, 13 },
        };

        int rank = LinearAlgebra.ReducedRowEchelonForm(a);

        // expected: 3
        AreEqual(3, rank);

        double[,] expected = new[,]
        {
            { 1, 0, 0, 1 },
            { 0, 1, 0, 3 },
            { 0, 0, 1, -3 },
            { 0, 0, 0, 3.10862e-15 },
            { 0, 0, 0, 2.22045e-15 },
        };

        for (int i = 0; i < 5; i++)
        for (int j = 0; j < 4; j++)
            AreEqual(expected[i, j], a[i, j]);
    }
}