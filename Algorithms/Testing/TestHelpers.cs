using static TestGenerator;

public static class TestHelpers
{
    public static void Echo(object obj = null)
    {
        Console.Error.WriteLine(obj);
    }

    public static void EchoRaw(object obj = null)
    {
        Console.Error.Write(obj);
    }

    public static void EchoTree(int n, int offset = 1)
    {
        int[] tree = RandomTree(n);
        EchoRaw(TreeToString(tree));
    }
}