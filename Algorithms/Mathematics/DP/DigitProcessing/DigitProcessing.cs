namespace Algorithms.Mathematics.Combinatorics;

public class DigitProcessing
{
    public static int FindNthDigit(long n)
    {
        if (n == 0)
            return 0;

        int digits = 1;
        long factor = 1;
        n--;
        while (true) {
            long addend = 9 * digits * factor;
            if (n < addend) break;
            n -= addend;
            factor *= 10;
            digits++;
        }

        long mod = n % digits;
        n /= digits;
        n += factor;
        while (mod + 1 < digits) {
            mod++;
            n /= 10;
        }

        return (int)(n % 10);
    }

    public static long MaskRepeated(long n)
    {
        long mask = 0;
        while (n > 0) {
            long d = n % 10;
            n /= 10;
            long bit = 1L << (6 * (int)d);
            while ((mask & bit) != 0) bit <<= 1;
            mask |= bit;
        }

        return mask;
    }

    public static int DigitMask(long n)
    {
        int mask = 0;
        while (n > 0) {
            long d = n % 10;
            n /= 10;
            int bit = 1 << (int)d;
            mask |= bit;
        }

        return mask;
    }

    public static long FingerPrint(long n)
    {
        // Permutations of the same number have the same fingerprint
        long result = 0;
        while (n > 0) {
            int digit = (int)(n % 10);
            n /= 10;
            result += 1L << (5 * digit);
        }

        return result;
    }

    public static int PandigitMask(long n)
    {
        int mask = 0;
        while (n > 0) {
            long d = n % 10;
            n /= 10;
            int bit = 1 << (int)d;
            if ((mask & bit) != 0) return 0;
            mask |= bit;
        }

        return mask;
    }

    public static int[] NumberToDigits(long number)
    {
        string s = number.ToString();
        int[] digits = new int[s.Length];
        for (int i = 0; i < s.Length; i++)
            digits[i] = s[i] - '0';
        return digits;
    }
}