namespace Algorithms.Mathematics;

// Based on Dual numbers  and Automatic Differentiation
// http://blog.demofox.org/2014/12/30/dual-numbers-automatic-differentiation/
public class DualNumber
{
    DualNumber(double real = 0.0f, double dual = 0.0f)
    {
        Real = real;
        Dual = dual;
    }

    public double Real { get; }

    public double Dual { get; }

    //----------------------------------------------------------------------
    // Math Operations
    //----------------------------------------------------------------------
    public static DualNumber operator +(DualNumber a, DualNumber b) => new(a.Real + b.Real, a.Dual + b.Dual);

    public static DualNumber operator -(DualNumber a, DualNumber b) => new(a.Real - b.Real, a.Dual - b.Dual);

    public static DualNumber operator *(DualNumber a, DualNumber b) =>
        new(
            a.Real * b.Real,
            a.Real * b.Dual + a.Dual * b.Real
        );

    public static DualNumber operator /(DualNumber a, DualNumber b) =>
        new(
            a.Real / b.Real,
            (a.Dual * b.Real - a.Real * b.Dual) / (b.Real * b.Real)
        );

    public static DualNumber Sqrt(DualNumber a)
    {
        double sqrtReal = Math.Sqrt(a.Real);
        return new DualNumber(
            sqrtReal,
            0.5f * a.Dual / sqrtReal
        );
    }

    public static DualNumber Pow(DualNumber a, double y) =>
        new(
            Math.Pow(a.Real, y),
            y * a.Dual * Math.Pow(a.Real, y - 1.0f)
        );

    public static DualNumber Sin(DualNumber a) =>
        new(
            Math.Sin(a.Real),
            a.Dual * Math.Cos(a.Real)
        );

    public static DualNumber Cos(DualNumber a) =>
        new(
            Math.Cos(a.Real),
            -a.Dual * Math.Sin(a.Real)
        );

    public static DualNumber Tan(DualNumber a) =>
        new(
            Math.Tan(a.Real),
            a.Dual / (Math.Cos(a.Real) * Math.Cos(a.Real))
        );

    public static DualNumber Atan(DualNumber a) =>
        new(
            Math.Atan(a.Real),
            a.Dual / (1.0f + a.Real * a.Real)
        );

    public static DualNumber SmoothStep(DualNumber x) =>
        // f(x) = 3x^2 - 2x^3
        // f'(x) = 6x - 6x^2
        x * x * (new DualNumber(3) - new DualNumber(2) * x);

    //----------------------------------------------------------------------
    // Test Functions
    //----------------------------------------------------------------------

    void TestSmoothStep(double x)
    {
        DualNumber y = SmoothStep(new DualNumber(x, 1.0f));
        Console.WriteLine("smoothstep 3x^2-2x^3({0:F4}) = {1:F4}\n", x, y.Real);
        Console.WriteLine("smoothstep 3x^2-2x^3'({0:F4}) = {1:F4}\n\n", x, y.Dual);
    }

    void TestTrig(double x)
    {
        DualNumber y = Sin(new DualNumber(x, 1.0f));
        Console.WriteLine("sin({0:F4}) = {1:F4}\n", x, y.Real);
        Console.WriteLine("sin'({0:F4}) = {1:F4}\n\n", x, y.Dual);

        y = Cos(new DualNumber(x, 1.0f));
        Console.WriteLine("Math.Cos({0:F4}) = {1:F4}\n", x, y.Real);
        Console.WriteLine("Math.Cos'({0:F4}) = {1:F4}\n\n", x, y.Dual);

        y = Tan(new DualNumber(x, 1.0f));
        Console.WriteLine("tan({0:F4}) = {1:F4}\n", x, y.Real);
        Console.WriteLine("tan'({0:F4}) = {1:F4}\n\n", x, y.Dual);

        y = Atan(new DualNumber(x, 1.0f));
        Console.WriteLine("atan({0:F4}) = {1:F4}\n", x, y.Real);
        Console.WriteLine("atan'({0:F4}) = {1:F4}\n\n", x, y.Dual);
    }

    void TestSimple(double x)
    {
        DualNumber y = new DualNumber(3.0f) / Sqrt(new DualNumber(x, 1.0f));
        Console.WriteLine("3/sqrt({0:F4}) = {1:F4}\n", x, y.Real);
        Console.WriteLine("3/sqrt({0:F4})' = {1:F4}\n\n", x, y.Dual);

        y = Pow(new DualNumber(x, 1.0f) + new DualNumber(1.0f), 1.337f);
        Console.WriteLine("({0:F4})^1.337 = {1:F4}\n", x, y.Real);
        Console.WriteLine("({0:F4})^1.337' = {1:F4}\n\n", x, y.Dual);
    }
}