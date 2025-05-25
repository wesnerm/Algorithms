namespace Algorithms.Mathematics.Combinatorics;

public class DigitDP
{
    readonly int MOD;
    readonly long[] tens;

    public DigitDP(int size, int mod = 1000 * 1000 * 1000 + 7)
    {
        tens = new long[size];
        MOD = mod;

        tens[0] = 1;
        for (int i = 1; i < size; i++)
            tens[i] = 10L * tens[i - 1] % MOD;
    }

    public long Solve(string number, bool include = true)
    {
        int length = number.Length;
        int[] digits = ConvertToLSDigitArray(number);

        long result = 0;
        // long upper = 0;
        for (int i = length - 1; i >= 0; i--) {
            long factor = tens[i];

            long top = digits[i] + (i == 0 && include ? 1 : 0);
            for (int j = 0; j < top; j++) {
                // ACCUMULATE SWATHS OF NUMBERS
                // result = (result + buffer[i, j] + upper * factor) % MOD;
            }

            int digitPrev = i + 1 >= length ? 10 : digits[i + 1];
            // UPDATE INFO ON BASED ON NEWEST DIGIT 
            //    upper = (upper + top * factor) % MOD;
        }

        return result;
    }

    /// <summary>
    ///     Use this to solve problems where leading zeros influences the results
    /// </summary>
    public long SolveLeading(string number, bool include = true)
    {
        int length = number.Length;
        int[] digits = ConvertToLSDigitArray(number);
        bool leadingZero = true;

        if (leadingZero)
            for (int len = length - 1; len > 0; len--) {
                // Incorporate all numbers whose leading digit is zero
            }

        long result = 0;
        // long upper = 0;
        for (int i = length - 1; i >= 0; i--) {
            long factor = tens[i];

            long top = digits[i] + (i == 0 && include ? 1 : 0);
            for (int j = 0; j < top; j++)
                if (leadingZero)
                    leadingZero = false;

            // ACCUMULATE SWATHS OF NUMBERS
            // result = (result + buffer[i, j] + upper * factor) % MOD;
            int digitPrev = i + 1 >= length ? 10 : digits[i + 1];
            // UPDATE INFO ON BASED ON NEWEST DIGIT 
            //    upper = (upper + top * factor) % MOD;
        }

        return result;
    }

    public static int[] ConvertToLSDigitArray(string number)
    {
        int length = number.Length;
        int[] digits = new int[length];
        for (int i = 0; i < length; i++)
            digits[i] = number[length - 1 - i] - '0';
        return digits;
    }

    public static string TrimZeroes(string num)
    {
        int zeroes = 0;
        for (int i = 0; i < num.Length && num[i] == '0'; i++)
            zeroes++;
        if (zeroes == num.Length) return "0";
        if (zeroes == 0) return num;
        return num.Remove(0, zeroes);
    }

    public static string Subtract1(string num)
    {
        var sb = new StringBuilder(num);
        int end = sb.Length - 1;

        for (; end > 0 && sb[end] == '0'; end--)
            sb[end] = '9';

        sb[end]--;
        return TrimZeroes(sb.ToString());
    }
}