using AnsiVtConsole.NetCore.Component.Settings;

namespace AnsiVtConsole.NetCore.Component.Console
{
    public interface IAnsiVtConsole
    {
        public AnsiVtConsoleSettings Settings { get; }

        public WorkAreaSettings WorkAreaSettings { get; }


        ColorSettings Colors { get; set; }



        TextWriterWrapper StdErr { get; }

        TextReader In { get; set; }
        bool IsConsoleGeometryEnabled { get; }

        ConsoleTextWriterWrapper Out { get; set; }

        WorkArea WorkArea { get; }

        ActualWorkArea ActualWorkArea(bool fitToVisibleArea = true);

        bool CheckConsoleHasGeometry();

        static bool GetConsoleHasGeometry()
        {
            try
            {
                var x = System.Console.WindowLeft;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        void Error(string s = "");
        void Error(IEnumerable<string> ls);
        void Error(string s, bool lineBreak = false);
        void Errorln(string s = "");
        void Errorln(IEnumerable<string> ls);
        void Exit(int r = 0);
        void FixCoords(ref int x, ref int y);
        (int x, int y, int w, int h) GetCoords(int x, int y, int w, int h, bool fitToVisibleArea = true);
        int GetCursorX(object x);
        int GetCursorY(object x);
        void Infos();
        void Log(string s, bool enableForwardLogsToSystemDiagnostics = true);
        void LogError(Exception ex, bool enableForwardLogsToSystemDiagnostics = true);
        void LogError(string s, bool enableForwardLogsToSystemDiagnostics = true);
        void LogException(Exception ex, string message = "", bool enableForwardLogsToSystemDiagnostics = true);
        void LogWarning(string s, bool enableForwardLogsToSystemDiagnostics = true);
        string? Readln(string? prompt = null);
        void RedirectErr(string? filepath = null);
        void RedirectErr(TextWriter? sw);
        void RedirectOut(string? filepath = null);
        void RedirectOut(StreamWriter? sw);
        void SetCursorAtWorkAreaTop();
        void Warning(string s = "");
        void Warning(IEnumerable<string> ls);
        void Warning(string s, bool lineBreak = false);
        void Warningln(string s = "");
        void Warningln(IEnumerable<string> ls);
    }
}