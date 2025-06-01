using Algorithms.Mathematics.Multiplication.FFT;

namespace Algorithms.Mathematics.Multiplication;

[TestFixture]
public class Conv64Test : FftTestBase
{

    public Conv64Test() : base()
    {
    }

    protected override long[] Multiply(long[] a, long[] b)
    {
        var c = new Conv64();
        return c.Multiply(a, b);
    }

    //[Test]
    //public void CorrectnessTest()
    //{
    //    // Correctness test
    //    long[] p = new long[] { 1, 2, 3 };
    //    long[] q = new long[] { 4, 5, 6 };
    //    long[] expected = new long[] { 4, 13, 28, 27, 18 };
    //    long[] result = Multiply(p, q);

    //    Check(p, q, result); // Check the result
    //}

    //[Test]
    //[TestCase(100000)]
    //public void BenchmarkTest(int size)
    //{
    //    var sw = Stopwatch.StartNew();
    //    long[] in1 = new long[size];
    //    long[] in2 = new long[size];
    //    for (int i = 0; i < size; ++i) {
    //        in1[i] = i % 2;
    //        in2[i] = (i + 1) % 2;
    //    }

    //    long[] benchResult = Multiply(in1, in2);
    //    sw.Stop();

    //    Console.WriteLine(
    //        $"Benchmark completed in {sw.Elapsed.TotalSeconds:F2} seconds. Result length: {benchResult.Length}");
    //    WriteResults(benchResult);
    //}

    //[Test]
    //[TestCase(0)]
    //[TestCase(1)]
    //[TestCase(2)]
    //[TestCase(3)]
    //[TestCase(10)]
    //[TestCase(100)]
    //[TestCase(1094)]
    //[TestCase(1095)]
    //[TestCase(5000)]
    //[TestCase(10000)]
    //public void SequenceTest(int size)
    //{
    //    long[] in1 = new long[size]; // Smaller for quicker test
    //    long[] in2 = new long[size];

    //    for (int i = 0; i < size; ++i) {
    //        in1[i] = i % 2;
    //        in2[i] = (i + 1) % 2;
    //    }

    //    Console.WriteLine("Inputs prepared. Starting multiplication...");
    //    var sw = Stopwatch.StartNew();
    //    long[] res = Multiply(in1, in2);
    //    sw.Stop();
    //    Console.WriteLine($"Multiplication took {sw.ElapsedMilliseconds} ms.");

    //    // Output a few values for verification
    //    Console.WriteLine($"Result length: {res.Length} (expected {in1.Length + in2.Length - 1})");
    //    WriteResults(res);

    //    Check(in1, in2, res); // Check the result
    //}

    //[Test]
    //[TestCase(1)]
    //[TestCase(2)]
    //[TestCase(3)]
    //[TestCase(10)]
    //[TestCase(100)]
    //[TestCase(1094)]
    //[TestCase(1095)]
    //[TestCase(5000)]
    //[TestCase(10000)]
    //public void RandomTest(int size)
    //{
    //    long[] in1 = new long[size]; // Smaller for quicker test
    //    long[] in2 = new long[size];

    //    var r = new Random(0x123456);
    //    for (int i = 0; i < size; ++i) {
    //        in1[i] = r.NextInt64(-long.MaxValue, long.MaxValue);
    //        in2[i] = r.NextInt64(-long.MaxValue, long.MaxValue);
    //    }

    //    Console.WriteLine("Inputs prepared. Starting multiplication...");
    //    var sw = Stopwatch.StartNew();
    //    long[] res = Multiply(in1, in2);
    //    sw.Stop();
    //    Console.WriteLine($"Multiplication took {sw.ElapsedMilliseconds} ms.");

    //    // Output a few values for verification
    //    Console.WriteLine($"Result length: {res.Length} (expected {in1.Length + in2.Length - 1})");
    //    WriteResults(res);

    //    Check(in1, in2, res); // Check the result
    //}

    //void WriteResults(long[] res, int limit = 20)
    //{
    //    limit = Math.Min(limit, res.Length);
    //    Console.Write("Result preview: ");
    //    for (int i = 0; i < limit; ++i) Console.Write(res[i] + " ");
    //    Console.WriteLine(limit < res.Length ? "..." : "");
    //}

    //[Test]
    //public void Test1()
    //{
    //    // Example simple test case: (1 + x) * (1 + x) = 1 + 2x + x^2
    //    long[] p_test = { 1, 1 };
    //    long[] q_test = { 1, 1 };
    //    long[] res_test = Multiply(p_test, q_test);
    //    Console.Write("Test (1+x)*(1+x): ");
    //    foreach (long val in res_test) Console.Write(val + " "); // Expected: 1 2 1

    //    Check(p_test, q_test, res_test); // Check the result
    //    AreEqual(1, res_test[0]);
    //    AreEqual(2, res_test[1]);
    //    AreEqual(1, res_test[2]);
    //    AreEqual(3, GetLength(res_test)); // Length should be 3
    //}

    //[Test]
    //public void Test2()
    //{
    //    // Example (1 + 2x) * (3 + 4x) = 3 + 4x + 6x + 8x^2 = 3 + 10x + 8x^2
    //    long[] p_test = new long[] { 1, 2 };
    //    long[] q_test = new long[] { 3, 4 };
    //    long[] res_test = Multiply(p_test, q_test);
    //    Console.Write("Test (1+2x)*(3+4x): ");
    //    foreach (long val in res_test) Console.Write(val + " "); // Expected: 3 10 8

    //    Check(p_test, q_test, res_test); // Check the result
    //    AreEqual(3, res_test[0]);
    //    AreEqual(10, res_test[1]);
    //    AreEqual(8, res_test[2]);
    //    AreEqual(3, GetLength(res_test));
    //}

    //[Test]
    //public void Test3()
    //{
    //    // Test from C++ main
    //    // in1[i] = i % 2; (0,1,0,1,...)
    //    // in2[i] = (i+1) % 2; (1,0,1,0,...)
    //    // (0+x+0x^2+x^3...) * (1+0x+1x^2+0x^3...)
    //    // If in1 = {0,1}, in2 = {1,0} -> x * 1 = x -> {0,1}
    //    // If in1 = {0,1,0}, in2 = {1,0,1} -> x * (1+x^2) = x+x^3 -> {0,1,0,1}
    //    // For {0,1} and {1,0}:
    //    long[] p_test = new long[] { 0, 1 }; // x
    //    long[] q_test = new long[] { 1, 0 }; // 1
    //    long[] res_test = Multiply(p_test, q_test); // Expected: 0 1 (0 + 1x)
    //    Console.Write("Test (x)*(1): ");
    //    foreach (long val in res_test) Console.Write(val + " ");

    //    Check(p_test, q_test, res_test); // Check the result
    //    AreEqual(0, res_test[0]);
    //    AreEqual(1, res_test[1]);
    //    AreEqual(2, GetLength(res_test));
    //}

    //[Test]
    //public void Test4()
    //{
    //    // For {0,1,0,1} and {1,0,1,0}:
    //    long[] p_test = new long[] { 0, 1, 0, 1 }; // x+x^3
    //    long[] q_test = new long[] { 1, 0, 1, 0 }; // 1+x^2
    //    long[] res_test = Multiply(p_test, q_test); // (x+x^3)(1+x^2) = x+x^3+x^3+x^5 = x+2x^3+x^5 -> {0,1,0,2,0,1}
    //    Console.Write("Test (x+x^3)*(1+x^2): ");
    //    foreach (long val in res_test) Console.Write(val + " ");

    //    Check(p_test, q_test, res_test); // Check the result
    //    AreEqual(0, res_test[0]);
    //    AreEqual(1, res_test[1]);
    //    AreEqual(0, res_test[2]);
    //    AreEqual(2, res_test[3]);
    //    AreEqual(0, res_test[4]);
    //    AreEqual(1, res_test[5]);
    //    AreEqual(6, GetLength(res_test));
    //}

    //int GetLength(long[] res_test)
    //{
    //    int length = res_test.Length;
    //    while (length > 0 && res_test[length - 1] == 0) length--;
    //    return length;
    //}

    //void Check(long[] poly1, long[] poly2, long[] res_test)
    //{
    //    long[] expected = Polynomial.MultiplyPolynomials(poly1, poly2);
    //    int lengthExpected = GetLength(expected);
    //    int lengthRes = GetLength(res_test);

    //    AreEqual(lengthExpected, lengthRes);
    //    for (int i = 0; i < expected.Length; i++) AreEqual(expected[i], res_test[i]);
    //}
}