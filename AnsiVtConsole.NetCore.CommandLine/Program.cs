using CommandLine.NetCore.Services.CmdLine;

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
        => new CommandLineInterfaceBuilder()
                .Build(args)
                .Run();
}