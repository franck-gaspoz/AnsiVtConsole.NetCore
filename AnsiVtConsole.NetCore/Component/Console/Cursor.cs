
using sc = System.Console;
namespace AnsiVtConsole.NetCore.Component.Console;

public class Cursor
{
    private readonly ConsoleTextWriterWrapper _out;
    private readonly IAnsiVtConsole _console;

    public Cursor(
        IAnsiVtConsole console,
        ConsoleTextWriterWrapper @out)
    {
        _out = @out;
        _console = console;
    }

    public void FixCoords(ref int x, ref int y)
    {
        lock (_out.Lock!)
        {
            x = Math.Max(0, Math.Min(x, sc.BufferWidth - 1));
            y = Math.Max(0, Math.Min(y, sc.BufferHeight - 1));
        }
    }

    public int GetCursorX(object x)
    {
        if (x != null && x is string s && !string.IsNullOrWhiteSpace(s)
            && int.TryParse(s, out var v))
        {
            return v;
        }

        if (_console.Settings.TraceCommandErrors)
            _console.Logger.LogError($"wrong cursor x: {x}");
        if (!_console.WorkArea.IsConsoleGeometryEnabled)
            return 0;
        lock (_out.Lock!)
        {
            return sc.CursorLeft;
        }
    }

    public int GetCursorY(object x)
    {
        if (x != null && x is string s && !string.IsNullOrWhiteSpace(s)
            && int.TryParse(s, out var v))
        {
            return v;
        }

        if (_console.Settings.TraceCommandErrors)
            _console.Logger.LogError($"wrong cursor y: {x}");
        if (!_console.WorkArea.IsConsoleGeometryEnabled)
            return 0;
        lock (_out.Lock!)
        {
            return sc.CursorTop;
        }
    }
}
