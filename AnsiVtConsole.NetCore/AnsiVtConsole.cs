using System.Diagnostics;
using System.Runtime.InteropServices;

using AnsiVtConsole.NetCore.Component.Console;
using AnsiVtConsole.NetCore.Component.EchoDirective;
using AnsiVtConsole.NetCore.Component.Settings;

using static AnsiVtConsole.NetCore.Component.EchoDirective.Shortcuts;

using RuntimeEnvironment = AnsiVtConsole.NetCore.Lib.RuntimeEnvironment;
using sc = System.Console;

namespace AnsiVtConsole.NetCore
{
    /// <summary>
    /// AnsiVtConsole.NetCore helps build fastly nice console applications
    /// </summary>
    [DebuggerDisplay("[Console : ID={Settings.ID} Out={Out} In={In} Err={StdErr}]")]
    public sealed class AnsiVtConsole : IAnsiVtConsole
    {
        /*
         * slowness due to:
        - many system calls on both linux (ConsolePal.Unix.cs) and windows (ConsolePal.Windows.cs)
        - the .net core make use of interop for each console method call in windows (ConsolePal.Windows.cs)
        - markup is slow: it will be optimized soon
        */

        #region public properties

        /// <inheritdoc/>
        public AnsiVtConsoleSettings Settings { get; private set; } = new();

        /// <inheritdoc/>
        public ColorSettings Colors { get; private set; }

        #region work area        

        /// <inheritdoc/>
        public Cursor Cursor { get; private set; }

        /// <inheritdoc/>
        public WorkArea WorkArea { get; private set; }

        /// <inheritdoc/>
        public WorkAreaSettings WorkAreaSettings { get; private set; } = new();

        #endregion

        #region streams

        /// <inheritdoc/>
        public ConsoleTextWriterWrapper Out { get; private set; }

        /// <inheritdoc/>
        readonly Error _error;

        /// <inheritdoc/>
        readonly Warn _warn;

        /// <inheritdoc/>
        public TextWriterWrapper StdErr { get; private set; } = new(sc.Error);

        /// <inheritdoc/>
        public TextReader In { get; private set; } = System.Console.In;

        /// <inheritdoc/>
        public Inp Inp { get; private set; }

        /// <inheritdoc/>
        public Logger Logger { get; private set; }

        #endregion

        #endregion

        #region private attributes

        static int _instanceCounter = 1000;

        static readonly object _instanceLock = new();

        TextWriter? _errorWriter;

        StreamWriter? _errorStreamWriter;

        FileStream? _errorFileStream;

        TextWriter? _outputWriter;

        StreamWriter? _outputStreamWriter;

        FileStream? _outputFileStream;

        #endregion

        /// <summary>
        /// new instance of an ansi vt console
        /// </summary>
        public AnsiVtConsole()
        {
            lock (_instanceLock)
            {
                Settings.ID = _instanceCounter;
                _instanceCounter++;
            }
            WorkArea = new(this);
            Out = new ConsoleTextWriterWrapper(this, sc.Out);
            Colors = new ColorSettings(this);
            _error = new(Out);
            _warn = new(Out, _error);
            Inp = new(Out);
            Cursor = new(this);
            Logger = new(this, Out, _error, _warn);
            Shortcuts.Initialize(this);
        }

        #region operations

        /// <inheritdoc/>
        public void Infos()
        {
            lock (Out.Lock!)
            {
                Out.WriteLine($"OS={Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? "64" : "32")}bits plateform={RuntimeEnvironment.OSType}");
                Out.WriteLine($"{White}{Bkf}{Colors.HighlightIdentifier}window:{Rsf} left={Colors.Numeric}{sc.WindowLeft}{Rsf},top={Colors.Numeric}{sc.WindowTop}{Rsf},width={Colors.Numeric}{sc.WindowWidth}{Rsf},height={Colors.Numeric}{sc.WindowHeight}{Rsf},largest width={Colors.Numeric}{sc.LargestWindowWidth}{Rsf},largest height={Colors.Numeric}{sc.LargestWindowHeight}{Rsf}");
                Out.WriteLine($"{Colors.HighlightIdentifier}buffer:{Rsf} width={Colors.Numeric}{sc.BufferWidth}{Rsf},height={Colors.Numeric}{sc.BufferHeight}{Rsf} | input encoding={Colors.Numeric}{sc.InputEncoding.EncodingName}{Rsf} | output encoding={Colors.Numeric}{sc.OutputEncoding.EncodingName}{Rsf}");
                Out.WriteLine($"{White}default background color={Bkf}{Colors.KeyWord}{Settings.DefaultBackground}{Rsf} | default foreground color={Colors.KeyWord}{Settings.DefaultForeground}{Rsf}");
                if (RuntimeEnvironment.OSType == OSPlatform.Windows)
                {
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                    Out.WriteLine($"number lock={Colors.Numeric}{sc.NumberLock}{Rsf} | capslock={Colors.Numeric}{sc.CapsLock}{Rsf}");
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                    Out.WriteLine($"cursor visible={Colors.Numeric}{sc.CursorVisible}{Rsf} | cursor size={Colors.Numeric}{sc.CursorSize}");
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
                }
            }
        }

        /// <inheritdoc/>
        public void Exit(int r = 0) => Environment.Exit(r);

        #endregion

        #region stream methods

        /// <inheritdoc/>
        public void RedirectOut(StreamWriter? streamWriter)
        {
            if (streamWriter != null)
            {
                Out.Redirect(streamWriter);
                _outputWriter = sc.Out;
                sc.SetOut(streamWriter);
                Settings.IsOutputRedirected = true;
            }
            else
            {
                Out.Redirect((TextWriter?)null);
                sc.SetOut(_outputWriter!);
                _outputWriter = null;
                Settings.IsOutputRedirected = false;
            }
        }

        /// <inheritdoc/>
        public void RedirectErr(StreamWriter? streamWriter)
        {
            if (streamWriter != null)
            {
                StdErr.Redirect(streamWriter);
                _errorWriter = sc.Error;
                sc.SetError(streamWriter);
                Settings.IsErrorRedirected = true;
            }
            else
            {
                StdErr.Redirect((TextWriter?)null);
                sc.SetError(_errorWriter!);
                _errorWriter = null;
                Settings.IsErrorRedirected = false;
            }
        }

        /// <inheritdoc/>
        public void RedirectOut(string? filePath = null)
        {
            if (filePath != null)
            {
                _outputWriter = sc.Out;
                _outputFileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                _outputStreamWriter = new StreamWriter(_outputFileStream);
                sc.SetOut(_outputStreamWriter);
                Out.Redirect(_outputStreamWriter);
            }
            else
            {
                _outputStreamWriter!.Flush();
                _outputStreamWriter.Close();
                _outputStreamWriter = null;
                sc.SetOut(_outputWriter!);
                _outputWriter = null;
                Out.Redirect((string?)null);
            }
        }

        /// <inheritdoc/>
        public void RedirectErr(string? filePath = null)
        {
            if (filePath != null)
            {
                _errorWriter = sc.Error;
                _errorFileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
                _errorStreamWriter = new StreamWriter(_errorFileStream);
                sc.SetOut(_errorStreamWriter);
                StdErr.Redirect(_errorStreamWriter);
            }
            else
            {
                _errorStreamWriter!.Flush();
                _errorStreamWriter.Close();
                _errorStreamWriter = null;
                sc.SetOut(_errorWriter!);
                _errorWriter = null;
                StdErr.Redirect((string?)null);
            }
        }

        #endregion

    }
}
