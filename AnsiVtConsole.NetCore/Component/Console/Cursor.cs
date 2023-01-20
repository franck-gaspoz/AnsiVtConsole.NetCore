
using sc = System.Console;
namespace AnsiVtConsole.NetCore.Component.Console;

/// <summary>
/// cursor control
/// </summary>
public class Cursor
{
    readonly IAnsiVtConsole _console;

    /// <summary>
    /// a cursor attached to a console and an output stream
    /// </summary>
    /// <param name="console">console</param>
    public Cursor(IAnsiVtConsole console) => _console = console;

    /// <summary>
    /// fix coordianates according to the console buffer size
    /// </summary>
    /// <param name="x">x</param>
    /// <param name="y">y</param>
    public void FixCoords(ref int x, ref int y)
    {
        lock (_console.Out.Lock!)
        {
            x = Math.Max(0, Math.Min(x, sc.BufferWidth - 1));
            y = Math.Max(0, Math.Min(y, sc.BufferHeight - 1));
        }
    }

    /// <summary>
    /// get an int value from a x string coordinate or the real cursor position if no or not valid parameter
    /// </summary>
    /// <param name="x">x</param>
    /// <returns>x</returns>
    public int GetCursorX(object? x = null)
    {
        if (x != null && x is string s && !string.IsNullOrWhiteSpace(s))
        {
            if (int.TryParse(s, out var v))
                return v;
            else
            {
                if (_console.Settings.TraceCommandErrors)
                    _console.Logger.LogError($"wrong cursor x: {x}");
            }
        }
        if (!_console.WorkArea.IsConsoleGeometryEnabled)
            return 0;

        lock (_console.Out.Lock!)
        {
            return sc.CursorLeft;
        }
    }

    /// <summary>
    /// get an int value from a y string coordinate or the real cursor position if no or not valid parameter
    /// </summary>
    /// <param name="x">y</param>
    /// <returns>y</returns>
    public int GetCursorY(object? x = null)
    {
        if (x != null && x is string s && !string.IsNullOrWhiteSpace(s))
        {
            if (int.TryParse(s, out var v))
                return v;
            else
            {
                if (_console.Settings.TraceCommandErrors)
                    _console.Logger.LogError($"wrong cursor y: {x}");
            }
        }
        if (!_console.WorkArea.IsConsoleGeometryEnabled)
            return 0;

        lock (_console.Out.Lock!)
        {
            return sc.CursorTop;
        }
    }

    /// <summary>
    /// hide cursor (not thread safe)
    /// </summary>
    public void Hide() => sc.CursorVisible = false;

    /// <summary>
    /// show cursor (not thread safe)
    /// </summary>
    public void Show() => sc.CursorVisible = true;
}
