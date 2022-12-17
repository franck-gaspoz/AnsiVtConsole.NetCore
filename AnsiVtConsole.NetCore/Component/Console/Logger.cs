namespace AnsiVtConsole.NetCore.Component.Console;

/// <summary>
/// logger
/// <para>log to console, can forwards to System.Diagnostics.Debug</para>
/// </summary>
public sealed class Logger
{
    private readonly ConsoleTextWriterWrapper _out;
    private readonly IAnsiVtConsole _console;
    private readonly Error _err;
    private readonly Warn _warn;
    private readonly string[] _crlf = { Environment.NewLine };

    internal Logger(
        IAnsiVtConsole console,
        ConsoleTextWriterWrapper @out,
        Error err,
        Warn warn
        )
    {
        _console = console;
        _out = @out;
        _err = err;
        _warn = warn;
    }

    /// <summary>
    /// log an error
    /// </summary>
    /// <param name="exception">exception</param>
    /// <param name="enableForwardLogsToSystemDiagnostics">enable or disable to replicate log to System.Diagnostics.Debug</param>
    public void LogError(Exception exception, bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(exception + "");
        if (_console.Settings.DumpExceptions)
        {
            LogError(exception, string.Empty, enableForwardLogsToSystemDiagnostics);
        }
        else
        {
            var msg = exception.Message;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                msg += _crlf + exception.Message;
            }
            var ls = msg.Split(_crlf, StringSplitOptions.None)
                .Select(x => _console.Colors.Error + x);
            _err.Logln(ls);
        }
    }

    /// <summary>
    /// log an error
    /// </summary>
    /// <param name="exception">exception</param>
    /// <param name="message">message</param>
    /// <param name="enableForwardLogsToSystemDiagnostics">enable or disable to replicate log to System.Diagnostics.Debug</param>
    public void LogError(Exception exception, string message = "", bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(message + _crlf + exception + "");
        var ls = new List<string>();
        if (_console.Settings.DumpExceptions)
        {
            ls = (exception + "").Split(_crlf, StringSplitOptions.None)
            .Select(x => _console.Colors.Error + x)
            .ToList();
            if (message != null)
                ls.Insert(0, $"{_console.Colors.Error}{message}");
        }
        else
        {
            ls.Insert(0, $"{_console.Colors.Error}{message}: {exception.Message}");
        }

        _err.Logln(ls);
    }

    /// <summary>
    /// log an error
    /// </summary>
    /// <param name="message">message</param>
    /// <param name="enableForwardLogsToSystemDiagnostics">enable or disable to replicate log to System.Diagnostics.Debug</param>
    public void LogError(string message, bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(message);
        var ls = (message + "").Split(_crlf, StringSplitOptions.None)
            .Select(x => _console.Colors.Error + x);
        _err.Logln(ls);
    }

    /// <summary>
    /// log a warning
    /// </summary>
    /// <param name="message">message</param>
    /// <param name="enableForwardLogsToSystemDiagnostics">enable or disable to replicate log to System.Diagnostics.Debug</param>
    public void LogWarning(string message, bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(message);
        var ls = (message + "").Split(_crlf, StringSplitOptions.None)
            .Select(x => _console.Colors.Warning + x);
        _warn.Logln(ls);
    }

    /// <summary>
    /// log a message
    /// </summary>
    /// <param name="message">message</param>
    /// <param name="enableForwardLogsToSystemDiagnostics">enable or disable to replicate log to System.Diagnostics.Debug</param>
    public void Log(string message, bool enableForwardLogsToSystemDiagnostics = true)
    {
        if (_console.Settings.ForwardLogsToSystemDiagnostics && enableForwardLogsToSystemDiagnostics)
            System.Diagnostics.Debug.WriteLine(message);
        var ls = (message + "").Split(_crlf, StringSplitOptions.None)
            .Select(x => _console.Colors.Log + x);
        foreach (var l in ls)
            _out.WriteLn(l);
    }
}
