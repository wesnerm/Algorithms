public class CarryLessMultiplication
{
    // http://bitmath.blogspot.com/2013/05/carryless-multiplicative-inverse.html
    static uint MultiplicativeInverse2(uint d)
    {
        uint x = 1;
        for (int i = 0; i < 5; i++) x = Multiply(x, Multiply(x, d));
        return x;
    }

    static uint MultiplicativeInverse(uint x)
    {
        uint inv = 1;
        uint rem = x;
        for (int i = 1; i < 32; i++)
            if (((rem >> i) & 1) != 0) {
                rem ^= x << i;
                inv |= 1u << i;
            }

        return inv;
    }

    static uint Multiply(uint a, uint b)
    {
        uint r = 0;
        while (b != 0) {
            if ((a & 1) != 0)
                r ^= b; // carryless addition is xor
            a >>= 1;
            b <<= 1;
        }

        return r;
    }

    public static ulong[] BuildBitSet(int[] data, int m)
    {
        ulong[] bitset = BuildBitSet(m);
        foreach (int d in data) bitset[d >> 6] |= 1ul << (d & 63);
        return bitset;
    }

    public static ulong[] BuildBitSet(int m)
    {
        ulong[] bitset = new ulong[(m + 63 + 64) >> 6];
        return bitset;
    }

    public static long GetSection(ulong[] bitset, int index, int length)
    {
        ulong result = 0;
        int end = index + length;
        int startbit = index & 63;
        int startword = index >> 6;
        int startlen = 64 - startbit;
        int endword = end >> 6;

        result = bitset[startword] >> startbit;
        if (startlen < 64) result &= (1ul << startlen) - 1;
        if (endword > startword) result |= bitset[endword] << (64 - startbit);
        result &= (1ul << length) - 1;
        return (long)result;
    }

    public static bool Get(ulong[] bitset, int d) => (bitset[d >> 6] & (1ul << (d & 63))) != 0;

    public static void Set(ulong[] bitset, int d)
    {
        bitset[d >> 6] |= 1ul << (d & 63);
    }

    public static void Clear(ulong[] bitset, int d)
    {
        bitset[d >> 6] &= ~(1ul << (d & 63));
    }
}