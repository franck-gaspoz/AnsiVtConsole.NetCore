using AnsiVtConsole.NetCore.Component.EchoDirective;

using static AnsiVtConsole.NetCore.Component.EchoDirective.Shortcuts;

namespace AnsiVtConsole.NetCore.Component.Console
{
    /// <summary>
    /// text color
    /// </summary>
    public sealed class TextColor
    {
        /// <summary>
        /// default foreground
        /// </summary>
        public static ConsoleColor DefaultForeground { get; set; }

        /// <summary>
        /// default background
        /// </summary>
        public static ConsoleColor DefaultBackground { get; set; }

        private ConsoleColor? _foreground;

        /// <summary>
        /// foreground
        /// </summary>
        public ConsoleColor? Foreground
        {
            get => _foreground ?? DefaultForeground;
            set => _foreground = value;
        }

        private ConsoleColor? _background;

        /// <summary>
        /// background
        /// </summary>
        public ConsoleColor? Background
        {
            get => _background ?? DefaultBackground;
            set => _background = value;
        }

        private readonly string? _toStrPost = null;
        private readonly string? _toStrPre = null;

        /// <summary>
        /// text color
        /// </summary>
        public TextColor(ConsoleColor? foreground, ConsoleColor? background = null)
        {
            Foreground = foreground;
            Background = background;
        }

        /// <summary>
        /// text color
        /// </summary>
        public TextColor(ConsoleColor? foreground, ConsoleColor? background = null, string toStrPost = "", string toStrPre = "")
        {
            Foreground = foreground;
            Background = background;
            _toStrPost = toStrPost;
            _toStrPre = toStrPre;
        }

        /// <summary>
        /// Invert
        /// </summary>
        public TextColor Invert() => new TextColor(_background, _foreground);

        /// <summary>
        /// to string
        /// </summary>
        public override string ToString() =>
             _toStrPre + (!_foreground.HasValue ? "" : GetCmd(EchoDirectives.f + "", _foreground.Value.ToString().ToLower()))
             + (!_background.HasValue ? "" : GetCmd(EchoDirectives.b + "", _background.Value.ToString().ToLower())) + _toStrPost;

        /// <summary>
        /// ToStringForegroundOnly
        /// </summary>
        public string ToStringForegroundOnly() => (!_foreground.HasValue ? "" : GetCmd(EchoDirectives.f + "", _foreground.Value.ToString().ToLower()));

        /// <summary>
        /// ToString
        /// </summary>
        public string ToString(bool foregroundOnly) =>
             _toStrPre + (!_foreground.HasValue ? "" : GetCmd(EchoDirectives.f + "", _foreground.Value.ToString().ToLower()))
             + ((foregroundOnly) ? "" : (!_background.HasValue ? "" : GetCmd(EchoDirectives.b + "", _background.Value.ToString().ToLower()))) + _toStrPost;

        #region build and convert colors operations

        /// <summary>
        /// build a color from a name
        /// </summary>
        /// <param name="colorName">color name</param>
        /// <returns>console color</returns>
        public static ConsoleColor GetColor(string colorName) => (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colorName);

        /// <summary>
        /// parse a 4 bit color
        /// </summary>
        /// <param name="console">console</param>
        /// <param name="c">text of color name</param>
        public static ConsoleColor? ParseColor(IAnsiVtConsole console, object? c)
        {
            if (c == null)
                return null;
            var s = (string)c;
            if (string.IsNullOrWhiteSpace(s))
                return null;
            if (Enum.TryParse(s, true, out ConsoleColor r))
                return r;
            if (console.TraceCommandErrors)
                console.Error($"invalid color name: {c}");
            return ConsoleColor.Black;
        }

        /// <summary>
        /// parse a 8 bit color
        /// </summary>
        /// <param name="console">console</param>
        /// <param name="c">string representing an integer in range 0..255 (included)</param>
        /// <returns></returns>
        public static int Parse8BitColor(IAnsiVtConsole console, object c)
        {
            if (int.TryParse((string)c, out var r) && r >= 0 && r <= 255)
                return r;
            if (console.TraceCommandErrors)
                console.Error($"invalid 8 bit color number: {c}");
            return 255;
        }

        /// <summary>
        /// parse a 24 bit color
        /// </summary>
        /// <param name="console">console</param>
        /// <param name="c">string of format: r:g:b where 0&lt;=r,g,b &lt;255</param>
        /// <returns></returns>
        public static (int r, int g, int b) Parse24BitColor(IAnsiVtConsole console, object c)
        {
            var s = (string)c;
            var t = s.Split(':');
            if (t.Length == 3)
            {
                if (int.TryParse(t[0], out var r) && r >= 0 && r <= 255
                    && int.TryParse(t[1], out var g) && g >= 0 && g <= 255
                    && int.TryParse(t[2], out var b) && b >= 0 && b <= 255)
                {
                    return (r, g, b);
                }
            }
            if (console.TraceCommandErrors)
                console.Error($"invalid 24 bit color: {c}");
            return (255, 255, 255);
        }

        #endregion
    }
}
