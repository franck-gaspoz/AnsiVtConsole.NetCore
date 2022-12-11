using AnsiVtConsole.NetCore.Component.Settings;

namespace AnsiVtConsole.NetCore.Component.Console
{
    public interface IAnsiVtConsole
    {
        public AnsiVtConsoleSettings Settings { get; }

        public WorkAreaSettings WorkAreaSettings { get; }

        public Err Err { get; }

        public Warn Warn { get; }

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

        void RedirectErr(TextWriter? sw);

        void RedirectOut(string? filepath = null);

        void RedirectOut(StreamWriter? sw);
    }
}