using System.Runtime.CompilerServices;

namespace Algorithms;

public static class BitTools
{
    public static int BranchlessSignExtend(int n) => n >> 31;

    public static long BranchlessSignExtend(long n) => n >> 63;

    public static long LeastPowerOfTwoGreaterOrEqualTo(long n)
    {
        long bits = n;
        while ((bits & (bits - 1)) != 0)
            bits &= bits - 1;
        if (n > bits) bits <<= 1;
        return bits;
    }

    public static int LeastPowerOfTwoGreaterOrEqualTo(int n)
    {
        int bits = n;
        while ((bits & (bits - 1)) != 0)
            bits &= bits - 1;
        if (n > bits) bits <<= 1;
        return bits;
    }

    public static bool HasOneBit(long value)
    {
        unchecked {
            return (value & (value - 1)) == 0 && value != 0;
        }
    }

    public static long MaxBits(long bits)
    {
        unchecked {
            int log = Log2(bits);
            if (log < 0) return 0;
            return (1 << log) - 1;
        }
    }

    [DebuggerStepThrough]
    public static uint RotateLeft(uint bits, int n = 1)
    {
        int n1 = n & 31;
        return (bits << n1) | (bits >> (32 - n1));
    }

    [DebuggerStepThrough]
    public static uint RotateRight(uint bits, int n = 1)
    {
        int n1 = n & 31;
        return (bits << (32 - n1)) | (bits >> n1);
    }

    [DebuggerStepThrough]
    public static int RotateLeft(int bits, int n = 1) => unchecked((int)RotateLeft((uint)bits, n));

    [DebuggerStepThrough]
    public static int RotateRight(int bits, int n = 1) => unchecked((int)RotateLeft((uint)bits, n));

    [DebuggerStepThrough]
    public static int RotateLeft31(int bits, int n = 1)
    {
        unchecked {
            int n1;
            if ((uint)n < 31) {
                n1 = n;
            } else {
                n1 = n % 31;
                if (n1 < 0)
                    n1 = 31 + n;
            }

            int b = bits & int.MaxValue;
            return ((b << n1) | (b >> (31 - n1))) & int.MaxValue;
        }
    }

    [DebuggerStepThrough]
    public static int RotateRight31(int bits, int n = 1) => RotateLeft31(bits, 31 - n);

    [DebuggerStepThrough]
    public static ulong RotateLeft(ulong bits, int n = 1)
    {
        int n1 = n & 31;
        return (bits << n1) | (bits >> (32 - n1));
    }

    [DebuggerStepThrough]
    public static ulong RotateRight(ulong bits, int n = 1)
    {
        int n1 = n & 31;
        return (bits << (32 - n1)) | (bits >> n1);
    }

    [DebuggerStepThrough]
    public static long RotateLeft(long bits, int n = 1) => unchecked((long)RotateLeft((ulong)bits, n));

    [DebuggerStepThrough]
    public static long RotateRight(long bits, int n = 1) => unchecked((long)RotateRight((ulong)bits, n));

    public static int Reverse(int value)
    {
        unchecked {
            uint n = unchecked((uint)value);
            n = (n >> 16) | (n << 16);
            n = ((n >> 0x8) & 0x00ff00ff) | ((n << 0x8) & 0xff00ff00);
            n = ((n >> 0x4) & 0x0f0f0f0f) | ((n << 0x4) & 0xf0f0f0f0);
            n = ((n >> 0x2) & 0x33333333) | ((n << 0x2) & 0xcccccccc);
            n = ((n >> 0x1) & 0x55555555) | ((n << 0x1) & 0xaaaaaaaa);
            return unchecked((int)n);
        }
    }

    public static long GetUnsigned(long data, int pos, int bits)
    {
        int chop = 64 - bits;
        unchecked {
            return (long)(((ulong)data << (chop - pos)) >> chop);
        }
    }

    public static long GetSigned(long data, int pos, int bits)
    {
        int chop = 64 - bits;
        return (data << (chop - pos)) >> chop;
    }

    public static int GetUnsigned(int data, int pos, int bits)
    {
        int chop = 32 - bits;
        unchecked {
            return (int)(((uint)data << (chop - pos)) >> chop);
        }
    }

    public static int GetSigned(int data, int pos, int bits)
    {
        int chop = 32 - bits;
        return (data << (chop - pos)) >> chop;
    }

    public static long Set(long data, int pos, int bits, long val)
    {
        long mask = (1L << bits) - 1;
        mask <<= pos;
        val <<= pos;
        return (data & ~mask) | (val & mask);
    }

    public static int Set(int data, int pos, int bits, int val)
    {
        int mask = (1 << bits) - 1;
        mask <<= pos;
        val <<= pos;
        return (data & ~mask) | (val & mask);
    }

    [DebuggerStepThrough]
    public static int RemoveLastBit(int value) => value & (value - 1);

    #region Bits

    const ulong M1 = 0x5555555555555555; //binary: 0101...
    const ulong M2 = 0x3333333333333333; //binary: 00110011..
    const ulong M4 = 0x0f0f0f0f0f0f0f0f; //binary:  4 zeros,  4 ones ...
    const ulong M8 = 0x00ff00ff00ff00ff; //binary:  8 zeros,  8 ones ...
    const ulong M16 = 0x0000ffff0000ffff; //binary: 16 zeros, 16 ones ...
    const ulong M32 = 0x00000000ffffffff; //binary: 32 zeros, 32 ones
    const ulong Hff = 0xffffffffffffffff; //binary: all ones
    const ulong H01 = 0x0101010101010101; //the sum of 256 to the power of 0,1,2,3...

    //http://en.wikipedia.org/wiki/Hamming_weight#Processor_support

    //This is better when most bits in x are 0
    //It uses 3 arithmetic operations and one comparison/branch per "1" bit in x.

    public static int BitCount(long x)
    {
        int count;
        ulong y = unchecked((ulong)x);
        for (count = 0; y != 0; count++)
            y &= y - 1;
        return count;
    }

    //This is a naive implementation, shown for comparison,
    //and to help in understanding the better functions.
    //It uses 24 arithmetic operations (shift, add, and).

    public static int Count1(ulong x)
    {
        x = (x & M1) + ((x >> 1) & M1); //put count of each  2 bits into those  2 bits 
        x = (x & M2) + ((x >> 2) & M2); //put count of each  4 bits into those  4 bits 
        x = (x & M4) + ((x >> 4) & M4); //put count of each  8 bits into those  8 bits 
        x = (x & M8) + ((x >> 8) & M8); //put count of each 16 bits into those 16 bits 
        x = (x & M16) + ((x >> 16) & M16); //put count of each 32 bits into those 32 bits 
        x = (x & M32) + ((x >> 32) & M32); //put count of each 64 bits into those 64 bits 
        return unchecked((int)x);
    }

    //This uses fewer arithmetic operations than any other known  
    //implementation on machines with slow multiplication.
    //It uses 17 arithmetic operations.

    public static int Count2(ulong x)
    {
        x -= (x >> 1) & M1; //put count of each 2 bits into those 2 bits
        x = (x & M2) + ((x >> 2) & M2); //put count of each 4 bits into those 4 bits 
        x = (x + (x >> 4)) & M4; //put count of each 8 bits into those 8 bits 
        x += x >> 8; //put count of each 16 bits into their lowest 8 bits
        x += x >> 16; //put count of each 32 bits into their lowest 8 bits
        x += x >> 32; //put count of each 64 bits into their lowest 8 bits
        return unchecked((int)(x & 0x7f));
    }

    //This uses fewer arithmetic operations than any other known  
    //implementation on machines with fast multiplication.
    //It uses 12 arithmetic operations, one of which is a multiply.

    public static int Count3(ulong x)
    {
        x -= (x >> 1) & M1; //put count of each 2 bits into those 2 bits
        x = (x & M2) + ((x >> 2) & M2); //put count of each 4 bits into those 4 bits 
        x = (x + (x >> 4)) & M4; //put count of each 8 bits into those 8 bits 
        return unchecked((int)((x * H01) >> 56)); //returns left 8 bits of x + (x<<8) + (x<<16) + (x<<24) + ... 
    }

    // Competition Performance including fixed overhead
    // BitCount(ulong) - 5.76
    // Count1 - 3.53
    // Count2 - 3.30
    // Count3 - 3.19

    // taken from Hacker's Delight, chapter 5, pp66
    // Failed in competition
    public static uint CountOptimised(uint i)
    {
        i = i - ((i >> 1) & 0x55555555); // adjacent bits grouped
        i = (i & 0x33333333) + ((i >> 2) & 0x33333333); // adjacent 2-bit groups paired
        i = i + ((i >> 4) & 0x0F0F0F0F); // adjacent 4-bit groups paired
        i = i + (i >> 8); // adjacent 8-bit groups paired
        i = i + (i >> 16); // adjacent 16-bit groups pair
        return i & 0x0000003F; // a 32-bit unsigned int can have at most 32 set bits, 
        // which in decimal needs on 6 bits to represent
    }

    // http://www.necessaryandsufficient.net/2009/04/optimising-bit-counting-using-iterative-data-driven-development/

    // BEST: works by counting adjacent 4-bits

    public static uint CountHackMem(uint i)
    {
        uint n = (i >> 1) & 0x77777777;
        i = i - n;
        n = (n >> 1) & 0x77777777;
        i = i - n;
        n = (n >> 1) & 0x77777777;
        i = i - n;
        i = (i + (i >> 4)) & 0xF0F0F0F;
        i = i * 0x01010101;
        return i >> 24;
    }

    // To turn off the rightmost 1-bit in x:          x & (x-1)
    //To isolate the rightmost bit in x:                x & 1
    //To isolate the rightmost 1-bit in x:            x & (-x) 
    //To isolate the rightmost 0-bit in x:            x & (x+1)
    //To create a mask for all trailing zeros:       ~x & (x-1)     (~ is the one's complement, aka bitwise NOT)
    //To turn off the k-th bit for the right in x:   x &  ~(1<<k)
    //To check if x is a power of 2:                      x & (x - 1) == 0
    //To toggle multiple bits at one: cretae a mask for the bits to toggle, M, and use:  x ^ M
    //To find the max value of a data type T, cast zero as T then complement:    ~(T)0

    #endregion

    #region Bit Arithmetic

    public static int Negate(int bits, int mod = 32)
    {
        int result = 0;
        for (int i = 0; i < mod; i++)
            if ((bits & (1 << i)) != 0)
                result |= i == 0 ? 1 : 1 << (mod - i);
        return result;
    }

    public static int Add(int bits1, int bits2)
    {
        int mask = bits2;
        int result = 0;
        while (mask != 0) {
            int bit = mask & -mask;
            result |= bits1 * bit;
            mask &= ~bit;
        }

        return result;
    }

    public static int Mod(int bits, int mod)
    {
        uint c = (uint)bits;
        c = (c | (c >> mod)) & ((1U << mod) - 1);
        return (int)c;
    }

    public static long Negate(long bits, int mod = 64)
    {
        long result = 0;
        for (int i = 0; i < mod; i++)
            if ((bits & (1 << i)) != 0)
                result |= i == 0 ? 1L : 1L << (mod - i);
        return result;
    }

    public static long Add(long bits1, long bits2)
    {
        long mask = bits2;
        long result = 0L;
        while (mask != 0) {
            long bit = mask & -mask;
            result |= bits1 * bit;
            mask &= ~bit;
        }

        return result;
    }

    public static long Mod(long bits, int mod)
    {
        ulong c = (ulong)bits;
        c = (c | (c >> mod)) & ((1UL << mod) - 1);
        return (long)c;
    }

    #endregion

    #region Logarithm

    public static int EstimateTreeDepth(int leafCount)
    {
        // Red-black tree is at most 2 lg n
        // Treap is on average 1.7 lg n (3.7 average worst case)
        int depth = int.Log2(leafCount);
        return depth + depth + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(long value)
    {
        return value != 0 ? (int)long.Log2(value) : -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int Log2(ulong value)
    {
        return value != 0 ? (int)ulong.Log2(value) : -1;
    }

    public static int Log2Original(int value)
    {
        unchecked {
            int log = 0;
            if ((uint)value >= 0x10000U) {
                log += 16;
                value = (int)((uint)value >> 16);
            }

            if (value >= 0x100) {
                log += 8;
                value >>= 8;
            }

            if (value >= 0x10) {
                log += 4;
                value >>= 4;
            }

            if (value >= 0x4) {
                log += 2;
                value >>= 2;
            }

            if (value >= 0x2) log += 1;
            return log;
        }
    }

    public static int Log2Slow(int value)
    {
        unchecked {
            // TESTED
            int log = 0;
            if ((uint)value >= 1U << 12) {
                log = 12;
                value = (int)((uint)value >> 12);
                if (value >= 1 << 12) {
                    log += 12;
                    value >>= 12;
                }
            }

            if (value >= 1 << 6) {
                log += 6;
                value >>= 6;
            }

            if (value >= 1 << 3) {
                log += 3;
                value >>= 3;
            }

            return log + ((value >> 1) & (~value >> 2));
        }
    }

    public static int AlternateLog2(long value)
    {
        // UNTESTED
        int log = 0;
        if ((ulong)value >= 1UL << 24) {
            if ((ulong)value >= 1UL << 48) {
                log = 48;
                value = (long)((ulong)value >> 48);
            } else {
                log = 24;
                value >>= 24;
            }
        }

        if (value >= 1 << 12) {
            log += 12;
            value >>= 12;
        }

        if (value >= 1 << 6) {
            log += 6;
            value >>= 6;
        }

        if (value >= 1 << 3) {
            log += 3;
            value >>= 3;
        }

        return log + (int)((value >> 1) & (~value >> 2));
    }

    //~0 = -(0+1)
    //~-1 = -(-1+1)
    //~2 = -(2+1)
    public static int Log2Naive(long value)
    {
        if (value <= 0)
            return value == 0 ? -1 : 63;

        int log = 0;
        if (value >= 0x100000000L) {
            log += 32;
            value >>= 32;
        }

        if (value >= 0x10000) {
            log += 16;
            value >>= 16;
        }

        if (value >= 0x100) {
            log += 8;
            value >>= 8;
        }

        if (value >= 0x10) {
            log += 4;
            value >>= 4;
        }

        if (value >= 0x4) {
            log += 2;
            value >>= 2;
        }

        if (value >= 0x2) log += 1;
        return log;
    }

    public static int Log2Slow(ulong value)
    {
        int log = 0;
        if (value >= 1UL << 24) {
            if (value >= 1UL << 48) {
                log = 48;
                value = value >> 48;
            } else {
                log = 24;
                value >>= 24;
            }
        }

        if (value >= 1 << 12) {
            log += 12;
            value >>= 12;
        }

        if (value >= 1 << 6) {
            log += 6;
            value >>= 6;
        }

        if (value >= 1 << 3) {
            log += 3;
            value >>= 3;
        }

        return log + (int)((value >> 1) & (~value >> 2));
    }

    public static int NumberOfTrailingZeros(int v)
    {
        int lastBit = v & -v;
        return lastBit != 0 ? Log2(lastBit) : 32;
    }

    public static int NumberOfTrailingZeros(long v)
    {
        long lastBit = v & -v;
        return lastBit != 0 ? Log2(lastBit) : 64;
    }

    public static int NumberOfTrailingZeros(ulong v)
    {
        ulong lastBit = v & unchecked((ulong)-(long)v);
        return lastBit != 0 ? Log2(lastBit) : 64;
    }

    public static int NumberOfLeadingZeros(int n) => 32 - 1 - Log2(n);

    public static int NumberOfLeadingZeros(long n) => 64 - 1 - Log2(n);

    public static int NumberOfLeadingZeros(ulong n) => 64 - 1 - Log2(n);

    public static ulong LowestOneBit(ulong n) => n & unchecked((ulong)-(long)n);

    public static long LowestOneBit(long n) => unchecked(n & -n);

    public static int LowestOneBit(int n) => unchecked(n & -n);

    public static ulong HighestOneBit(ulong n) => n != 0 ? 1UL << Log2(n) : 0;

    public static long HighestOneBit(long n) => n != 0 ? 1L << Log2(n) : 0;

    public static int HighestOneBit(int n) => n != 0 ? 1 << Log2(n) : 0;

    public static int ShiftRight(int n, int shift = 1) => unchecked((int)((uint)n >> shift));

    public static long ShiftRight(long n, int shift = 1) => unchecked((long)((ulong)n >> shift));

    static class DeBruijnUntested
    {
        static readonly int[] _positions =
        {
            0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
            31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9,
        };

        /// <summary>
        ///     Returns the first set bit (FFS), or 0 if no bits are set.
        /// </summary>
        public static int TrailingZeros2(int number)
        {
            uint res = unchecked((uint)(number & -number) * 0x077CB531U) >> 27;
            return _positions[res];
        }

        public static int TrailingZeros(int number)
        {
            const long low = 0x67A2CDDF05CAC984L;
            const long med = unchecked((long)0xA64E417B92D72F30);
            const long hi = 0x57B3754L;
            return (int)(((low >> (number << 1)) & 3) | (((med >> (number << 1)) & 3) << 2) |
                         (((hi >> number) & 1) << 4));
        }
    }

    /*    long[] _positions =
    {
        0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
        31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
    };

    long value1 = 0;
    long value2 = 0;
    long value3 = 0;
    for (int i=0; i<32; i++)
    {
        value1 |= (_positions[i]>>0&3)<<i*2;
        value2 |= (_positions[i]>>2&3)<<i*2;
        value3 |= (_positions[i]>>4&1)<<i;
    }

    Console.WriteLine("low {0:X}", value1);
    Console.WriteLine("med {0:X}", value2);
    Console.WriteLine("hi  {0:X}", value3);

      for (int i=0; i<32; i++)
      {
        long v = (value1>>(i<<1)&3) | (value2>>(i<<1)&3)<<2 | (value3>>i&1)<<4;
        Console.Write(v + " ");
      }
        Console.WriteLine();

      const long low = 0x67A2CDDF05CAC984L;
      const long med = unchecked((long)0xA64E417B92D72F30);
      const long hi = 0x57B3754L;
      for (int i=0; i<32; i++)
      {
        long v = (low>>(i<<1)&3) | (med>>(i<<1)&3)<<2 | (hi>>i&1)<<4;
        Console.Write(v + " ");
      }
        Console.WriteLine();
    }
    */
    // http://bitmath.blogspot.com/2012/09/divisibility-and-modular-multiplication.html

    static bool IsDivisibleByOdd(uint x, uint divisor)
    {
        if ((divisor & 1) == 0)
            throw new ArgumentException("divisor must be odd");
        uint d_inv = MultiplicativeInverse(divisor);
        uint biggest = uint.MaxValue / divisor; // problem right here
        return x * d_inv <= biggest;
    }

    static uint MultiplicativeInverse(uint d)
    {
        // see Hacker's Delight,
        // Computing the Multiplicative Inverse by Newton's Method
        // use extra iteration when extending to 64 bits
        uint x = d * d + d - 1;
        uint t = d * x;
        x *= 2 - t;
        t = d * x;
        x *= 2 - t;
        t = d * x;
        x *= 2 - t;
        return x;
    }

    //UNTESTED
    public static int Log(long n, long b)
    {
        if (n < 2) return n == 1 ? 0 : -1;

        int pow = 0;
        long k = 1;
        while (true) {
            long newK = k * b;
            if (newK > n || newK < k) return pow;
            k = newK;
            pow++;
        }
    }

    //UNTESTED
    public static int Log(int n, int b)
    {
        if (n < 2) return n == 1 ? 0 : -1;
        int pow = 0;
        for (long k = b; k <= n; k *= b)
            pow++;
        return pow;
    }
}

#endregion