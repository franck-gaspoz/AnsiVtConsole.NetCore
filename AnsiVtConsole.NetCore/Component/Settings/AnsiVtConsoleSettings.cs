namespace AnsiVtConsole.NetCore.Component.Settings;

/// <summary>
/// ansi vt console settings
/// </summary>
public class AnsiVtConsoleSettings
{
    public int ID { get; set; }

    public bool IsErrorRedirected { get; set; } = false;

    public bool IsOutputRedirected { get; set; } = false;

    public bool TraceCommandErrors { get; set; } = true;

    public bool DumpExceptions { get; set; } = true;

    public ConsoleColor? DefaultForeground { get; set; }

    public ConsoleColor? DefaultBackground { get; set; }

    public char CommandBlockBeginChar { get; set; } = '(';

    public char CommandBlockEndChar { get; set; } = ')';

    public char CommandSeparatorChar { get; set; } = ',';

    public char CommandValueAssignationChar { get; set; } = '=';

    public string CodeBlockBegin { get; set; } = "[[";

    public string CodeBlockEnd { get; set; } = "]]";

    public bool ForwardLogsToSystemDiagnostics { get; set; } = true;

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
