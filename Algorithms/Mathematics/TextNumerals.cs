// TESTS: https://www.hackerrank.com/contests/projecteuler/challenges/euler017

namespace Algorithms.Mathematics;

public static class TextNumerals
{
    public static readonly string[] Cardinals =
    {
        "zero",
        "one", "two", "three", "four", "five",
        "six", "seven", "eight", "nine", "ten",
        "eleven", "twelve", "thirteen", "fourteen", "fifteen",
        "sixteen", "seventeen", "eighteen", "nineteen",
    };

    public static readonly string[] Cardinals2 =
    {
        "twenty", "thirty", "forty", "fifty", "sixty",
        "seventy", "eighty", "ninety",
    };

    public static readonly string[] Cardinals3 =
    {
        "thousand", "million",
        "billion", "trillion", "quadrillion", "quintillion",
        "sextillion", "octillion", "nonillion", "decillion",
    };

    public static readonly string[] Ordinals =
    {
        "zeroth",
        "first", "second", "third", "fourth", "fifth",
        "sixth", "seventh", "eighth", "ninth", "tenth",
        "eleventh", "twelfth", "thirteenth", "fourteenth", "fifteenth",
        "sixteenth", "seventeenth", "eighteenth", "nineteenth",
    };

    public static readonly string[] Ordinals2 =
    {
        "twentieth", "thirtieth", "fortieth", "fiftieth",
        "sixtieth", "seventieth", "eightieth", "ninetieth",
    };

    static bool GetThousands(ref long value,
        out int thousands,
        out int factor)
    {
        long test = value;
        long location = 1;

        thousands = 0;
        while (test >= 1000) {
            test /= 1000;
            location *= 1000;
            thousands++;
        }

        factor = (int)test;
        if (thousands > 0) {
            value -= location * factor;
            return true;
        }

        return false;
    }

    static void AppendNumber(StringBuilder sb, long value, bool cardinal)
    {
        int factor;
        int thousands;
        while (GetThousands(ref value, out thousands, out factor)) {
            AppendNumber(sb, factor, true);
            sb.Append(Cardinals3[thousands - 1]);

            if (value > 0 || cardinal)
                sb.Append(' ');
            else
                sb.Append("th ");
        }

        if (value >= 100) {
            AppendNumber(sb, value / 100, true);
            value %= 100;
            if (value > 0 || cardinal)
                sb.Append("hundred ");
            else
                sb.Append("hundredth ");
        }

        if (value >= 20) {
            int n = (int)(value / 10);
            value %= 10;
            if (value > 0 || cardinal)
                sb.Append(Cardinals2[n - 2]);
            else
                sb.Append(Ordinals2[n - 2]);

            sb.Append(value > 0 ? '-' : ' ');
        }

        if (value > 0) {
            if (cardinal)
                sb.Append(Cardinals[(int)value]);
            else
                sb.Append(Ordinals[(int)value]);
            sb.Append(' ');
        }
    }

    public static string ToText(long value, bool cardinal)
    {
        var buffer = new StringBuilder();
        AppendNumber(buffer, value, cardinal);
        return buffer.ToString();
    }
}