namespace AnsiVtConsole.NetCore.Component.Console;

internal class Warn
{
    private readonly ConsoleTextWriterWrapper _out;
    private readonly Error _err;

    public Warn(
        ConsoleTextWriterWrapper outStream,
        Error err)
    {
        _out = outStream;
        _err = err;
    }

    public void Log(string text = "") => LogInternal(text, false);

    public void LogLine(string text = "") => LogInternal(text, true);

    public void LogLine(IEnumerable<string> texts)
    {
        foreach (var s in texts)
            _err.LogLine(s);
    }

    public void Log(IEnumerable<string> texts)
    {
        foreach (var s in texts)
            _err.Log(s);
    }

    private void LogInternal(string text, bool lineBreak = false)
    {
        lock (_out.Lock!)
        {
            _out.Write(text, lineBreak);
        }
    }
}
