using System.Security.Cryptography;

namespace Algorithms.Benchmark;

public class Md5VsSha256
{
    const int N = 10000;
    readonly byte[] data;
    readonly MD5 md5 = MD5.Create();

    readonly SHA256 sha256 = SHA256.Create();

    public Md5VsSha256()
    {
        data = new byte[N];
        new Random(42).NextBytes(data);
    }

    [Benchmark]
    public byte[] Sha256() => sha256.ComputeHash(data);

    [Benchmark]
    public byte[] Md5() => md5.ComputeHash(data);
}