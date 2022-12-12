namespace AnsiVtConsole.NetCore.Component.Console;

public sealed class Err
{
    private readonly ConsoleTextWriterWrapper _out;
    private readonly ColorSettings _colors;

    public Err(
        ConsoleTextWriterWrapper outStream,
        ColorSettings colors)
    {
        _out = outStream;
        _colors = colors;
    }

    public void Log(string s = "") => Log(s, false);

    public void Logln(string s = "") => Log(s, true);

    public void Logln(IEnumerable<string> ls)
    {
        foreach (var s in ls)
            Logln(s);
    }

    public void Log(IEnumerable<string> ls)
    {
        foreach (var s in ls)
            Log(s);
    }

    public void Log(string s, bool lineBreak = false)
    {
        lock (_out.Lock!)
        {
            _out.RedirectToErr = true;
            _out.Write($"{_colors.Error}{s}{_colors.Default}", lineBreak);
            _out.RedirectToErr = false;
        }
    }
}
