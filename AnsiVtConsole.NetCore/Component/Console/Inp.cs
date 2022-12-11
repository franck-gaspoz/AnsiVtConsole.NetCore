
using sc = System.Console;

namespace AnsiVtConsole.NetCore.Component.Console;

public class Inp
{
    private readonly ConsoleTextWriterWrapper _out;

    public Inp(ConsoleTextWriterWrapper @out) => _out = @out;

    public string? Readln(string? prompt = null)
    {
        lock (_out.Lock!)
        {
            if (prompt != null)
                _out.Echo(prompt);
        }
        return sc.ReadLine();
    }
}
