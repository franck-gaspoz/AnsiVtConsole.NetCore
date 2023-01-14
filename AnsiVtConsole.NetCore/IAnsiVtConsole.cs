using AnsiVtConsole.NetCore.Component.Console;
using AnsiVtConsole.NetCore.Component.Settings;

namespace AnsiVtConsole.NetCore;

/// <summary>
/// AnsiVtConsole interface
/// </summary>
public interface IAnsiVtConsole
{
    /// <summary>
    /// settings of the console
    /// </summary>
    public AnsiVtConsoleSettings Settings { get; }

    /// <summary>
    /// work area settings
    /// </summary>
    public WorkAreaSettings WorkAreaSettings { get; }

    /// <summary>
    /// input prompter
    /// </summary>
    public Inp Inp { get; }

    /// <summary>
    /// default colors and predefined colors sets
    /// </summary>
    ColorSettings Colors { get; }

    /// <summary>
    /// system standard err stream wrapper
    /// </summary>
    TextWriterWrapper StdErr { get; }

    /// <summary>
    /// standard input stream
    /// </summary>
    TextReader In { get; }

    /// <summary>
    /// cursor controler
    /// </summary>
    Cursor Cursor { get; }

    /// <summary>
    /// output stream
    /// </summary>
    ConsoleTextWriterWrapper Out { get; }

    /// <summary>
    /// work area
    /// </summary>
    WorkArea WorkArea { get; }

    /// <summary>
    /// logger
    /// </summary>
    Logger Logger { get; }

    /// <summary>
    /// terminates current process
    /// </summary>
    /// <param name="r">return code</param>
    void Exit(int r = 0);

    /// <summary>
    /// output infos about the system and the console host
    /// </summary>
    void Infos();

    /// <summary>
    /// redirects outputs to a file
    /// </summary>
    /// <param name="filepath">file path - set null to disable redirect</param>
    void RedirectErr(string? filepath = null);

    /// <summary>
    /// redirects errors to a stream writer
    /// </summary>
    /// <param name="sw">stream writer - set null to disable redirect</param>
    void RedirectErr(StreamWriter? sw);

    /// <summary>
    /// redirects outputs to a file
    /// </summary>
    /// <param name="filepath">file path - set null to disable redirect</param>
    void RedirectOut(string? filepath = null);

    /// <summary>
    /// redirects outputs to a stream writer
    /// </summary>
    /// <param name="sw">stream writer - set null to disable redirect</param>
    void RedirectOut(StreamWriter? sw);
}
