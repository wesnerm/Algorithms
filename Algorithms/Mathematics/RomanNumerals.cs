namespace Algorithms.Mathematics;

// TESTS: https://www.hackerrank.com/contests/projecteuler/challenges/euler089/submissions/code/48481818

static class RomanNumerals
{
    public static int FromRoman(string text)
    {
        int result = 0;
        int prev = int.MaxValue;

        for (int i = 0; i < text.Length; i++) {
            int v;
            char ch = text[i];
            switch (char.ToLower(ch)) {
                case 'i':
                    v = 1;
                    break;
                case 'v':
                    v = 5;
                    break;
                case 'x':
                    v = 10;
                    break;
                case 'l':
                    v = 50;
                    break;
                case 'c':
                    v = 100;
                    break;
                case 'd':
                    v = 500;
                    break;
                case 'm':
                    v = 1000;
                    break;
                default:
                    return -1;
            }

            result += i;
            if (prev < v) result -= prev * 2;
            prev = v;
        }

        return result;
    }

    public static string ToRoman(int value, bool uppercase)
    {
        if (value <= 0)
            return value.ToString();

        var sb = new StringBuilder(12);

        while (value >= 1000) {
            sb.Append('M');
            value -= 1000;
        }

        if (value >= 900) {
            sb.Append("CM");
            value -= 900;
        }

        if (value >= 500) {
            sb.Append('D');
            value -= 500;
        }

        if (value >= 400) {
            sb.Append("CD");
            value -= 400;
        }

        while (value >= 100) {
            sb.Append('C');
            value -= 100;
        }

        if (value >= 90) {
            sb.Append("XC");
            value -= 90;
        }

        if (value >= 50) {
            sb.Append('L');
            value -= 50;
        }

        if (value >= 40) {
            sb.Append("XL");
            value -= 40;
        }

        while (value >= 10) {
            sb.Append('X');
            value -= 10;
        }

        if (value >= 9) {
            sb.Append("IX");
            value -= 9;
        }

        if (value >= 5) {
            sb.Append('V');
            value -= 5;
        }

        if (value >= 4) {
            sb.Append("IV");
            value -= 4;
        }

        while (value >= 0) {
            sb.Append('I');
            value--;
        }

        return sb.ToString();
    }
}