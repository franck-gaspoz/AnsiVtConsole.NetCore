using AnsiVtConsole.NetCore.Component.Console;
using AnsiVtConsole.NetCore.Component.Settings;

namespace AnsiVtConsole.NetCore;

/// <summary>
/// AnsiVtConsole interface
/// </summary>
public interface IAnsiVtConsole
{
    public AnsiVtConsoleSettings Settings { get; }

    public WorkAreaSettings WorkAreaSettings { get; }

    public Inp Inp { get; }

    ColorSettings Colors { get; }

    TextWriterWrapper StdErr { get; }

    TextReader In { get; }

    Cursor Cursor { get; }

    ConsoleTextWriterWrapper Out { get; }

    WorkArea WorkArea { get; }

    Logger Logger { get; }

    void Exit(int r = 0);

    void Infos();

    void RedirectErr(string? filepath = null);

    void RedirectErr(StreamWriter? sw);

    void RedirectOut(string? filepath = null);

    void RedirectOut(StreamWriter? sw);
}
