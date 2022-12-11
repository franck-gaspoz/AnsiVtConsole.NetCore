namespace AnsiVtConsole.NetCore.Component.Console;

public class Logger
{
    private readonly ConsoleTextWriterWrapper _out;
    private readonly IAnsiVtConsole _console;
    private readonly Err _err;
    private readonly string[] _crlf = { Environment.NewLine };

    public Logger(
        IAnsiVtConsole console,
        ConsoleTextWriterWrapper @out,
        Err err
        )
    {
        _console = console;
        _out = @out;
        _err = err;
    }

    public void LogError(Exception ex, bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(ex + "");
        if (_console.Settings.DumpExceptions)
        {
            LogException(ex);
        }
        else
        {
            var msg = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                msg += _crlf + ex.Message;
            }
            var ls = msg.Split(_crlf, StringSplitOptions.None)
                .Select(x => _console.Colors.Error + x);
            _err.Logln(ls);
        }
    }

    public void LogException(Exception ex, string message = "", bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(message + _crlf + ex + "");
        var ls = new List<string>();
        if (_console.Settings.DumpExceptions)
        {
            ls = (ex + "").Split(_crlf, StringSplitOptions.None)
            .Select(x => _console.Colors.Error + x)
            .ToList();
            if (message != null)
                ls.Insert(0, $"{_console.Colors.Error}{message}");
        }
        else
        {
            ls.Insert(0, $"{_console.Colors.Error}{message}: {ex.Message}");
        }

        _err.Logln(ls);
    }

    public void LogError(string s, bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(s);
        var ls = (s + "").Split(_crlf, StringSplitOptions.None)
            .Select(x => _console.Colors.Error + x);
        _err.Logln(ls);
    }

    public void LogWarning(string s, bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(s);
        var ls = (s + "").Split(_crlf, StringSplitOptions.None)
            .Select(x => _console.Colors.Warning + x);
        _err.Logln(ls);
    }

    public void Log(string s, bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(s);
        var ls = (s + "").Split(_crlf, StringSplitOptions.None)
            .Select(x => _console.Colors.Log + x);
        foreach (var l in ls)
            _out.Echoln(l);
    }
}
