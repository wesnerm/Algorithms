namespace Algorithms.Strings;

public class SequenceHasher64
{
    // This gives us the best range of 32-bit values with the benefits of primality
    const long LoPrime = int.MaxValue; // Largest 32-bit prime
    const long HiPrime = int.MaxValue; // 0x7FFFFFED; // 2nd Largest 31-bit prime

    const int LoFactor = 307;
    const int HiFactor = 2309;
    static SequenceHasher lohasher;
    static SequenceHasher hihasher;

    public SequenceHasher64(StringBuilder s)
    {
        lohasher = new SequenceHasher(s, LoFactor, LoPrime);
        hihasher = new SequenceHasher(s, HiFactor, HiPrime);
    }

    public long ComputeBackHash(int rightIndex, int count)
    {
        long lo = lohasher.ComputeBackHash(rightIndex, count);
        long hi = hihasher.ComputeBackHash(rightIndex, count);

        // This gives us a 62 bit hash value
        return lo | (hi << 31);
    }

    public long ComputeForwardHash(int leftIndex, int count) => ComputeBackHash(leftIndex + count - 1, count);
}

public class SequenceHasher
{
    static long[] _hashes;
    static long[] _factors;
    public readonly long HashFactor;
    public readonly long HashMod;

    // Modular arithmetic does not mix well with unsigned integers 
    // -- Avoid uint and ulong
    // Negative numbers are not congruent to their unsigned counterparts (ie, -k != 2^n - k mod m)

    // Avoid bit shift operations because of high collision rate especially on comp prog test cases

    public SequenceHasher(StringBuilder s, int hashFactor = 307, long hashMod = int.MaxValue)
    {
        HashMod = hashMod;
        HashFactor = hashFactor;

        long hash = 0;
        _hashes = new long[s.Length];
        for (int i = 0; i < s.Length; i++) {
            hash = (hash * HashFactor + s[i]) % HashMod;
            _hashes[i] = hash;
        }

        long factor = 1;
        _factors = new long[s.Length];
        for (int i = 0; i < _factors.Length; i++) {
            _factors[i] = factor;
            factor = factor * HashFactor % HashMod;
        }
    }

    public long ComputeBackHash(int rightIndex, int count)
    {
        long hashEnd = _hashes[rightIndex];
        long hashStart = rightIndex >= count ? Mult(_hashes[rightIndex - count], _factors[count]) : 0;
        return (hashEnd + HashMod - hashStart) % HashMod;
    }

    public long ComputeForwardHash(int leftIndex, int count) => ComputeBackHash(leftIndex + count - 1, count);

    public long Advance(long hash, int ch) => (hash * HashFactor + ch) % HashMod;

    public long Mult(long a, long b) => a * b % HashMod;
}