namespace AnsiVtConsole.NetCore.Component.Settings;

/// <summary>
/// ansi vt console settings
/// </summary>
public class AnsiVtConsoleSettings
{
    /// <summary>
    /// console id
    /// </summary>
    public int ID { get; internal set; }

    /// <summary>
    /// is error redirected
    /// </summary>
    public bool IsErrorRedirected { get; internal set; } = false;

    /// <summary>
    /// is output redirect
    /// </summary>
    public bool IsOutputRedirected { get; internal set; } = false;

    /// <summary>
    /// if true trace command errors
    /// </summary>
    public bool TraceCommandErrors { get; set; } = true;

    /// <summary>
    /// if true dump exception traces else use log error
    /// </summary>
    public bool DumpExceptions { get; set; } = true;

    /// <summary>
    /// default foreground color
    /// </summary>
    public ConsoleColor? DefaultForeground { get; set; }

    /// <summary>
    /// default background color
    /// </summary>
    public ConsoleColor? DefaultBackground { get; set; }

    /// <summary>
    /// markup print directive pattern begin char
    /// </summary>
    public char CommandBlockBeginChar { get; set; } = '(';

    /// <summary>
    /// markup print directive pattern end char
    /// </summary>
    public char CommandBlockEndChar { get; set; } = ')';

    /// <summary>
    /// markup print directive separator char
    /// </summary>
    public char CommandSeparatorChar { get; set; } = ',';

    /// <summary>
    /// markup print directive value assignation char
    /// </summary>
    public char CommandValueAssignationChar { get; set; } = '=';

    /// <summary>
    /// markup code block pattern begin
    /// </summary>
    public string CodeBlockBegin { get; set; } = "[[";

    /// <summary>
    /// markup code block end begin
    /// </summary>
    public string CodeBlockEnd { get; set; } = "]]";

    /// <summary>
    /// if true forwards logs to system diagnostics
    /// </summary>
    public bool ForwardLogsToSystemDiagnostics { get; set; } = true;

    /// <summary>
    /// tab length
    /// </summary>
    public int TabLength { get; set; } = 7;

    /// <summary>
    /// if set markup (print directives) is removed from outputs
    /// </summary>
    public bool IsMarkupDisabled { get; set; } = false;

    /// <summary>
    /// if set markup is not handled and outputed
    /// </summary>
    public bool IsRawOutputEnabled { get; set; } = false;

    /// <summary>
    /// if set and IsRawOutputEnabled, non printable characters are replaced by their name
    /// </summary>
    public bool ReplaceNonPrintableCharactersByTheirName { get; set; } = true;

    /// <summary>
    /// if set, any ansi sequence is removed from output
    /// </summary>
    public bool RemoveANSISequences { get; set; } = false;
}
