using cons = AnsiVtConsole.NetCore;

namespace AnsiVtConsole.NetCore.CommandLine;

/// <summary>
/// main class
/// </summary>
public class Program
{
    /// <summary>
    /// command line
    /// </summary>
    /// <param name="args">arguments</param>
    /// <returns>exit code</returns>
    public static int Main(string[] args)
    {
        var console = new cons.AnsiVtConsole();

        var text = args.Length > 0 ? args[0] : string.Empty;
        console.Out.WriteLine(text);

        return 0;
    }
}