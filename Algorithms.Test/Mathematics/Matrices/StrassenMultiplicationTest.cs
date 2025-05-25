using static Algorithms.Collections.ArrayTools;

namespace Algorithms.Mathematics.Matrices;

[TestFixture]
public class StrassenMultiplicationTest
{
    [Test]
    public void MultiplicationTest()
    {
        long[,] matA = new long[3, 3];
        matA[0, 0] = 1;
        matA[0, 1] = 2;
        matA[0, 2] = 3;
        matA[1, 0] = 4;
        matA[1, 1] = 5;
        matA[1, 2] = 6;

        long[,] matB = new long[3, 3];
        matB[0, 0] = 7;
        matB[0, 1] = 8;
        matB[1, 0] = 9;
        matB[1, 1] = 10;
        matB[2, 0] = 11;
        matB[2, 1] = 12;

        long[][] matC = StrassenMultiplication.Strassen(ConvertArrayForm(matA), ConvertArrayForm(matB), 2, 3, 2);
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) Console.Write("{0:D} ", matC[i][j]);
            Console.Write("\n");
        }
    }
}