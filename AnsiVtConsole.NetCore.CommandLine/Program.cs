using cons = AnsiVtConsole.NetCore;

namespace AnsiVtConsole.NetCore.CommandLine;

public class Program
{
    public static int Main(string[] args)
    {
        var console = new cons.AnsiVtConsole();
        console.Out.WriteLine(args[0]);

        return 0;
    }
}