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

    public bool IsAnsiVtDisabled { get; set; } = false;
}
