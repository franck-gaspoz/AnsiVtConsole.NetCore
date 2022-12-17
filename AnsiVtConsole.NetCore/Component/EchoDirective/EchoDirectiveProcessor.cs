using System.Text;

using AnsiVtConsole.NetCore.Component.Console;

namespace AnsiVtConsole.NetCore.Component.EchoDirective
{
    /// <summary>
    /// echo directives processor
    /// <para>&#9989; drives directly the writer (simple approach to execute print directives)</para>
    /// </summary>
    internal sealed class EchoDirectiveProcessor
    {
        public delegate object? Command1pIntDelegate(int n = 1);
        public delegate object? Command2pIntDelegate(int x = 1, int y = 1);
        public delegate object? Command2pDelegate(object parameter, object argument);
        public delegate object? CommandDelegate(object x);
        public delegate void SimpleCommandDelegate();
        public readonly ConsoleTextWriterWrapper Writer;
        public readonly CommandMap CommandMap;
        private readonly IAnsiVtConsole _console;

        public EchoDirectiveProcessor(
            ConsoleTextWriterWrapper writer,
            CommandMap commandMap
        )
        {
            _console = writer.Console;
            Writer = writer;
            CommandMap = commandMap;
        }

        private readonly StringBuilder _tmpsb = new(100000);

        public void ParseTextAndApplyCommands(
            string s,
            bool lineBreak = false,
            string tmps = "",
            bool doNotEvalutatePrintDirectives = false,
            EchoSequenceList? printSequences = null,
            int startIndex = 0)
        {
            lock (Writer.Lock!)
            {
                var i = 0;
                KeyValuePair<string,
                    (SimpleCommandDelegate? simpleCommand,
                    CommandDelegate? command,
                    object? parameter)>? cmd = null;
                var n = s.Length;
                var isAssignation = false;
                var cmdindex = -1;

                _tmpsb.Clear();
                _tmpsb.Append(tmps);

                while (cmd == null && i < n)
                {
                    if (s[i] == _console.Settings.CommandBlockBeginChar)
                    {
                        foreach (var ccmd in CommandMap.Map!)
                        {
                            if (s.IndexOf(_console.Settings.CommandBlockBeginChar + ccmd.Key, i) == i)
                            {
                                cmd = ccmd;
                                cmdindex = i;
                                isAssignation = ccmd.Key.EndsWith("=");
                            }
                        }
                        if (cmd == null)
                            _tmpsb.Append(s[i]);
                    }
                    else
                    {
                        _tmpsb.Append(s[i]);
                    }

                    i++;
                }

                var stmps = _tmpsb.ToString();

                if (cmd == null)
                {
                    Writer.ConsolePrint(stmps, false);

                    printSequences?.Add(
                        new EchoSequence(
                            _console,
                            (string?)null,
                            0,
                            i - 1,
                            null,
                            stmps,
                            startIndex));
                    return;
                }
                else
                {
                    i = cmdindex;
                }

                if (!string.IsNullOrEmpty(stmps))
                {
                    Writer.ConsolePrint(stmps);

                    printSequences?.Add(
                        new EchoSequence(
                            _console,
                            (string?)null,
                            0,
                            i - 1,
                            null,
                            stmps,
                            startIndex));
                }

                _tmpsb.Clear();

                var firstCommandEndIndex = 0;
                var k = -1;
                string? value = null;
                if (isAssignation)
                {
                    firstCommandEndIndex = s.IndexOf(_console.Settings.CommandValueAssignationChar, i + 1);
                    if (firstCommandEndIndex > -1)
                    {
                        firstCommandEndIndex++;
                        var subs = s[firstCommandEndIndex..];
                        if (subs.StartsWith(_console.Settings.CodeBlockBegin))
                        {
                            firstCommandEndIndex += _console.Settings.CodeBlockBegin.Length;
                            k = s.IndexOf(_console.Settings.CodeBlockEnd, firstCommandEndIndex);
                            if (k > -1)
                            {
                                value = s[firstCommandEndIndex..k];
                                k += _console.Settings.CodeBlockEnd.Length;
                            }
                            else
                            {
                                Writer.ConsolePrint(s);

                                printSequences?.Add(
                                    new EchoSequence(
                                        _console,
                                        (string?)null,
                                        i,
                                        s.Length - 1,
                                        null,
                                        s,
                                        startIndex));
                                return;
                            }
                        }
                    }
                }

                var j = i + cmd.Value.Key.Length;
                var inCmt = false;
                var firstCommandSeparatorCharIndex = -1;
                while (j < s.Length)
                {
                    if (inCmt && s.IndexOf(_console.Settings.CodeBlockEnd, j) == j)
                    {
                        inCmt = false;
                        j += _console.Settings.CodeBlockEnd.Length - 1;
                    }
                    if (!inCmt && s.IndexOf(_console.Settings.CodeBlockBegin, j) == j)
                    {
                        inCmt = true;
                        j += _console.Settings.CodeBlockBegin.Length - 1;
                    }
                    if (!inCmt && s.IndexOf(_console.Settings.CommandSeparatorChar, j) == j && firstCommandSeparatorCharIndex == -1)
                        firstCommandSeparatorCharIndex = j;
                    if (!inCmt && s.IndexOf(_console.Settings.CommandBlockEndChar, j) == j)
                        break;
                    j++;
                }
                if (j == s.Length)
                {
                    Writer.ConsolePrint(s);

                    printSequences?.Add(
                        new EchoSequence(
                            _console,
                            (string?)null,
                            i,
                            j,
                            null,
                            s,
                            startIndex));
                    return;
                }

                var cmdtxt = s[i..j];
                if (firstCommandSeparatorCharIndex > -1)
                    cmdtxt = cmdtxt[..(firstCommandSeparatorCharIndex - i)/*-1*/];

                object? result = null;
                if (isAssignation)
                {
                    if (value == null)
                    {
                        var t = cmdtxt.Split(_console.Settings.CommandValueAssignationChar);
                        value = t[1];
                    }

                    // ❎ --> exec echo directive command : with ASSIGNATION
                    if (!doNotEvalutatePrintDirectives)
                    {
                        if (cmd.Value.Value.command != null)
                        {
                            if (cmd.Value.Value.parameter == null)
                            {
                                try
                                {
                                    result = cmd.Value.Value.command(value);    // CommandDelegate
                                }
                                catch (Exception ex)
                                {
                                    if (_console.Settings.TraceCommandErrors)
                                        _console.Logger.LogError(ex.Message);
                                }
                            }
                            else
                            {
                                try
                                {
                                    result = cmd.Value.Value.command((cmd.Value.Value.parameter, value));
                                }
                                catch (Exception ex)
                                {
                                    if (_console.Settings.TraceCommandErrors)
                                        _console.Logger.LogError(ex.Message);
                                }
                            }
                        }
                        else
                            if (cmd.Value.Value.simpleCommand != null)
                        {
                            try
                            {
                                cmd.Value.Value.simpleCommand();
                            }
                            catch (Exception ex)
                            {
                                if (_console.Settings.TraceCommandErrors)
                                    _console.Logger.LogError(ex.Message);
                            }
                            result = null;
                        }
                        // else: no command: do nothing
                    }
                    // <--

                    if (Writer.FileEchoDebugEnabled && Writer.FileEchoDebugCommands)
                        Writer.EchoDebug(_console.Settings.CommandBlockBeginChar + cmd.Value.Key + value + _console.Settings.CommandBlockEndChar);

                    printSequences?.Add(new EchoSequence(_console, cmd.Value.Key[0..^1], i, j, value, null, startIndex));
                }
                else
                {
                    // ❎ --> exec echo directive command : NO ASSIGNATION
                    if (!doNotEvalutatePrintDirectives)
                    {
                        if (cmd.Value.Value.command != null)
                        {
                            try
                            {
                                result = cmd.Value.Value.command(cmd.Value.Value.parameter!);
                            }
                            catch (Exception ex)
                            {
                                if (_console.Settings.TraceCommandErrors)
                                    _console.Logger.LogError(ex.Message);
                            }
                        }
                        else
                        {
                            if (cmd.Value.Value.simpleCommand != null)
                            {
                                try
                                {
                                    cmd.Value.Value.simpleCommand();
                                }
                                catch (Exception ex)
                                {
                                    if (_console.Settings.TraceCommandErrors)
                                        _console.Logger.LogError(ex.Message);
                                }
                                result = null;
                            }
                            // else: no command: do nothing
                        }
                    }
                    // <--

                    if (Writer.FileEchoDebugEnabled && Writer.FileEchoDebugCommands)
                        Writer.EchoDebug(_console.Settings.CommandBlockBeginChar + cmd.Value.Key + _console.Settings.CommandBlockEndChar);

                    printSequences?.Add(new EchoSequence(_console, cmd.Value.Key, i, j, value, null, startIndex));
                }
                if (result != null)
                    Writer.Write(result);    // recurse

                if (firstCommandSeparatorCharIndex > -1)
                {
                    s = _console.Settings.CommandBlockBeginChar + s[(firstCommandSeparatorCharIndex + 1) /*+ i*/ ..];
                    startIndex += firstCommandSeparatorCharIndex + 1;
                }
                else
                {
                    if (j + 1 < s.Length)
                    {
                        s = s[(j + 1)..];
                        startIndex += j + 1;
                    }
                    else
                    {
                        s = string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(s))
                    ParseTextAndApplyCommands(s, lineBreak, "", doNotEvalutatePrintDirectives, printSequences, startIndex);
            }
        }
    }
}