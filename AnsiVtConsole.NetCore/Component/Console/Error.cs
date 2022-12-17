namespace AnsiVtConsole.NetCore.Component.Console;

internal sealed class Error
{
    private readonly ConsoleTextWriterWrapper _out;

    public Error(ConsoleTextWriterWrapper outStream)
        => _out = outStream;

    public void Log(string text = "") => LogInternal(text, false);

    public void LogLine(string text = "") => LogInternal(text, true);

    public void LogLine(IEnumerable<string> texts)
    {
        foreach (var s in texts)
            LogLine(s);
    }

    public void Log(IEnumerable<string> texts)
    {
        foreach (var s in texts)
            Log(s);
    }

    private void LogInternal(string text, bool lineBreak = false)
    {
        lock (_out.Lock!)
        {
            _out.RedirectToErr = true;
            if (!lineBreak)
                _out.Write(text);
            else
                _out.WriteLine(text);
            _out.RedirectToErr = false;
        }
    }
}
