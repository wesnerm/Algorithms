using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestGenerator;

public static class TestHelpers
{

    public static void Echo(object obj=null)
    {
        Console.Error.WriteLine(obj);
    }

    public static void EchoRaw(object obj=null)
    {
        Console.Error.Write(obj);
    }

    public static void EchoTree(int n, int offset=1)
    {
        var tree = RandomTree(n);
        EchoRaw(TreeToString(tree));
    }

}
