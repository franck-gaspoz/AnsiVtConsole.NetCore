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
    /// <para>
    /// slowness due to:
    /// - many system calls on both linux (ConsolePal.Unix.cs) and windows (ConsolePal.Windows.cs)
    /// - the .net core make use of interop for each console method call in windows (ConsolePal.Windows.cs)
    /// </para>
    /// </summary>
    [DebuggerDisplay("[Console : ID={Settings.ID} Out={Out} In={In} Err={StdErr}]")]
    public sealed class AnsiVtConsole : IAnsiVtConsole
    {
        #region public properties

        public AnsiVtConsoleSettings Settings { get; private set; } = new();

        public ColorSettings Colors { get; private set; }

        #region work area        

        /// <summary>
        /// cursor
        /// </summary>
        public Cursor Cursor { get; private set; }

        public WorkArea WorkArea { get; private set; }

        public WorkAreaSettings WorkAreaSettings { get; private set; } = new();

        #endregion

        #region streams

        /// <summary>
        /// ansi vt std redirectable/bufferable stream
        /// </summary>
        public ConsoleTextWriterWrapper Out { get; private set; }

        /// <summary>
        /// ansi vr error output stream
        /// </summary>
        public Err Err { get; private set; }

        /// <summary>
        /// ansi vr warning output stream
        /// </summary>
        public Warn Warn { get; private set; }

        /// <summary>
        /// system standard err stream wrapper
        /// </summary>
        public TextWriterWrapper StdErr { get; private set; } = new(sc.Error);

        /// <summary>
        /// standard input stream
        /// </summary>
        public TextReader In { get; private set; } = System.Console.In;

        /// <summary>
        /// input stream
        /// </summary>
        public Inp Inp { get; private set; }

        /// <summary>
        /// logger
        /// </summary>
        public Logger Logger { get; private set; }

        #endregion

        #endregion

        #region private attributes

        private static int _instanceCounter = 1000;

        private static readonly object _instanceLock = new object();

        private TextWriter? _errorWriter;

        private StreamWriter? _errorStreamWriter;

        private FileStream? _errorFileStream;

        private TextWriter? _outputWriter;

        private StreamWriter? _outputStreamWriter;

        private FileStream? _outputFileStream;

        #endregion

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
            Err = new(Out, Colors);
            Warn = new(Out, Err, Colors);
            Inp = new(Out);
            Cursor = new(this, Out);
            Logger = new(this, Out, Err);
            Shortcuts.Initialize(this);
        }

        #region operations

        public void Infos()
        {
            lock (Out.Lock!)
            {
                Out.Echoln($"OS={Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? "64" : "32")}bits plateform={RuntimeEnvironment.OSType}");
                Out.Echoln($"{White}{Bkf}{Colors.HighlightIdentifier}window:{Rsf} left={Colors.Numeric}{sc.WindowLeft}{Rsf},top={Colors.Numeric}{sc.WindowTop}{Rsf},width={Colors.Numeric}{sc.WindowWidth}{Rsf},height={Colors.Numeric}{sc.WindowHeight}{Rsf},largest width={Colors.Numeric}{sc.LargestWindowWidth}{Rsf},largest height={Colors.Numeric}{sc.LargestWindowHeight}{Rsf}");
                Out.Echoln($"{Colors.HighlightIdentifier}buffer:{Rsf} width={Colors.Numeric}{sc.BufferWidth}{Rsf},height={Colors.Numeric}{sc.BufferHeight}{Rsf} | input encoding={Colors.Numeric}{sc.InputEncoding.EncodingName}{Rsf} | output encoding={Colors.Numeric}{sc.OutputEncoding.EncodingName}{Rsf}");
                Out.Echoln($"{White}default background color={Bkf}{Colors.KeyWord}{Settings.DefaultBackground}{Rsf} | default foreground color={Colors.KeyWord}{Settings.DefaultForeground}{Rsf}");
                if (RuntimeEnvironment.OSType == OSPlatform.Windows)
                {
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                    Out.Echoln($"number lock={Colors.Numeric}{sc.NumberLock}{Rsf} | capslock={Colors.Numeric}{sc.CapsLock}{Rsf}");
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                    Out.Echoln($"cursor visible={Colors.Numeric}{sc.CursorVisible}{Rsf} | cursor size={Colors.Numeric}{sc.CursorSize}");
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
                }
            }
        }

        /// <summary>
        /// terminates current process
        /// </summary>
        /// <param name="r">return code</param>
        public void Exit(int r = 0) => Environment.Exit(r);

        #endregion

        #region stream methods

        public void RedirectOut(StreamWriter? sw)
        {
            if (sw != null)
            {
                Out.Redirect(sw);
                _outputWriter = sc.Out;
                sc.SetOut(sw);
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

        public void RedirectErr(TextWriter? sw)
        {
            if (sw != null)
            {
                StdErr.Redirect(sw);
                _errorWriter = sc.Error;
                sc.SetError(sw);
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

        public void RedirectOut(string? filepath = null)
        {
            if (filepath != null)
            {
                _outputWriter = sc.Out;
                _outputFileStream = new FileStream(filepath, FileMode.Append, FileAccess.Write);
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

        public void RedirectErr(string? filepath = null)
        {
            if (filepath != null)
            {
                _errorWriter = sc.Error;
                _errorFileStream = new FileStream(filepath, FileMode.Append, FileAccess.Write);
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
