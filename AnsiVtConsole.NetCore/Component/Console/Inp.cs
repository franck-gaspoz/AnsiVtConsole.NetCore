
using sc = System.Console;

namespace AnsiVtConsole.NetCore.Component.Console;

/// <summary>
/// input prompter
/// </summary>
public class Inp
{
    readonly ConsoleTextWriterWrapper _out;

    /// <summary>
    /// output stream to be used for the prompt
    /// </summary>
    /// <param name="out">a console text writer</param>
    public Inp(ConsoleTextWriterWrapper @out) => _out = @out;

    /// <summary>
    /// display a prompt and read a line from standard input
    /// </summary>
    /// <param name="prompt">prompt</param>
    /// <returns>inputed line if any</returns>
    public string? Readln(string? prompt = null)
    {
        lock (_out.Lock!)
        {
            if (prompt != null)
                _out.Write(prompt);
        }
        return sc.ReadLine();
    }
}
