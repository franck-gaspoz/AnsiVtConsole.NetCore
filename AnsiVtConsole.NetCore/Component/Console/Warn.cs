namespace AnsiVtConsole.NetCore.Component.Console;

public class Warn
{
    private readonly ConsoleTextWriterWrapper _out;
    private readonly Err _err;
    private readonly ColorSettings _colors;

    public Warn(
        ConsoleTextWriterWrapper outStream,
        Err err,
        ColorSettings colors)
    {
        _out = outStream;
        _err = err;
        _colors = colors;
    }

    public void Log(string s = "") => Log(s, false);

    public void Logln(string s = "") => Log(s, true);

    public void Logln(IEnumerable<string> ls)
    {
        foreach (var s in ls)
            _err.Logln(s);
    }

    public void Log(IEnumerable<string> ls)
    {
        foreach (var s in ls)
            _err.Log(s);
    }

    public void Log(string s, bool lineBreak = false)
    {
        lock (_out.Lock!)
        {
            _out.Echo($"{_colors.Warning}{s}{_colors.Default}", lineBreak);
        }
    }
}
