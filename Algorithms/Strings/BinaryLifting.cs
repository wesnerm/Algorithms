namespace Algorithms.Strings;

public class BinaryLifting
{
    public static int[,] BinaryLift<T>(int[] array)
    {
        int bits = 1;
        while (1 << bits < array.Length)
            bits++;
        int[,] result = new int[array.Length, bits];
        for (int i = 0; i < array.Length; i++) {
            result[i, 0] = array[i];
            for (int j = 1; j < bits; j++)
                result[i, j] = result[result[i, j - 1], j - 1];
        }

        return result;
    }

    public static int[,] BinaryLift(int[] array, Func<int, int, bool> jump)
    {
        int bits = 1;
        while (1 << bits < array.Length)
            bits++;
        int[,] result = new int[array.Length, bits];
        for (int i = 0; i < array.Length; i++)
            result[i, 0] = array[i];
        for (int j = 1; j < bits; j++)
        for (int i = 0; i < array.Length; i++)
            // TODO: Figure this out
            result[i, j] = jump(array[i], array[i - 1])
                ? result[i, j - 1]
                : result[result[i, j - 1], j - 1];

        return result;
    }
}