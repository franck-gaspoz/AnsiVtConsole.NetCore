using System.Drawing;
using System.Runtime.CompilerServices;

using AnsiVtConsole.NetCore.Component.EchoDirective;
using AnsiVtConsole.NetCore.Component.Script;
using AnsiVtConsole.NetCore.Component.UI;
using AnsiVtConsole.NetCore.Lib;

using static AnsiVtConsole.NetCore.Component.EchoDirective.Shortcuts;
using static AnsiVtConsole.NetCore.Lib.Str;

using itpsrv = System.Runtime.InteropServices;
using sc = System.Console;

namespace AnsiVtConsole.NetCore.Component.Console
{
    public sealed class ConsoleTextWriterWrapper : TextWriterWrapper
    {
        #region attributes

        public override string ToString() => $"[console text writer wrapper - {base.ToString()}]";
        public bool IsMuteOrIsNotConsoleGeometryEnabled => IsMute || !Console.IsConsoleGeometryEnabled;
        public bool IsNotMuteAndIsConsoleGeometryEnabled => IsNotMute && Console.IsConsoleGeometryEnabled;

        public bool RedirectToErr = false;

        public IAnsiVtConsole Console;

        public ColorSettings ColorSettings;
        private int _cursorLeftBackup;
        private int _cursorTopBackup;
        private ConsoleColor _backgroundBackup = ConsoleColor.Black;
        private ConsoleColor _foregroundBackup = ConsoleColor.White;
        private EchoDirectiveProcessor EchoDirectiveProcessor;

        public static readonly string ESC = (char)27 + "";

        public string LNBRK =>
                // fix end of line remained filled with last colors
                EnableAvoidEndOfLineFilledWithBackgroundColor ?
                        $"{ANSI.RSTXTA}{ANSI.EL(ANSI.ELParameter.p0)}{ANSI.CRLF}{GetRestoreDefaultColors}"
                        : $"{ANSI.CRLF}";

        public CSharpScriptEngine? CSharpScriptEngine;

        #endregion

        #region console output settings

        public int CropX = -1;
        public bool EnableFillLineFromCursor = true;

        public bool EnableAvoidEndOfLineFilledWithBackgroundColor = true;

        #endregion

        #region console information cache

        private Point _cachedCursorPosition = Point.Empty;
        private Size _cachedBufferSize = Size.Empty;
        private ConsoleColor? _cachedForegroundColor;
        private ConsoleColor? _cachedBackgroundColor;

        #endregion

        #region init

#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.
        public ConsoleTextWriterWrapper(IAnsiVtConsole console) : base() => Init(console);

        public ConsoleTextWriterWrapper(IAnsiVtConsole console, TextWriter textWriter) : base(textWriter) => Init(console);

        public ConsoleTextWriterWrapper(IAnsiVtConsole console, CSharpScriptEngine cSharpScriptEngine) : base() => Init(console, cSharpScriptEngine);

        public ConsoleTextWriterWrapper(IAnsiVtConsole console, TextWriter textWriter, CSharpScriptEngine cSharpScriptEngine) : base(textWriter) => Init(console, cSharpScriptEngine);
#pragma warning restore CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

        /// <summary>
        /// console init + internal init
        /// </summary>
        private void Init(IAnsiVtConsole console, CSharpScriptEngine? cSharpScriptEngine = null)
        {
            Console = console;
            console.CheckConsoleHasGeometry();
            CSharpScriptEngine = cSharpScriptEngine ?? new CSharpScriptEngine(console);

            if (IsNotMute)
            {
                // TIP: dot not affect background color throught System.Console.Background to preserve terminal console background transparency
                Console.DefaultForeground = sc.ForegroundColor;
                _cachedForegroundColor = Console.DefaultForeground;
            }

            InitEchoDirectives();
        }

        private void InitEchoDirectives()
        {
#pragma warning disable CS8974 // Conversion d’un groupe de méthodes en type non-délégué
            // echo_directive => SimpleCommandDelegate, CommandDelegate, parameter
            var _drtvs = new Dictionary<string,
                    (EchoDirectiveProcessor.SimpleCommandDelegate? simpleCommand,
                    EchoDirectiveProcessor.CommandDelegate? command,
                    object? parameter)>() {
                { EchoDirectives.bkf+""   , (BackupForeground, null,null) },
                { EchoDirectives.bkb+""   , (BackupBackground, null,null) },
                { EchoDirectives.rsf+""   , (RestoreForeground, null,null) },
                { EchoDirectives.rsb+""   , (RestoreBackground, null,null) },

                { EchoDirectives.f+"="    , (null, SetForegroundColor,null) },
                { EchoDirectives.f8+"="   , (null, SetForegroundParse8BitColor,null) },
                { EchoDirectives.f24+"="  , (null, SetForegroundParse24BitColor,null) },
                { EchoDirectives.b+"="    , (null, SetBackgroundColor,null) },
                { EchoDirectives.b8+"="   , (null, SetBackgroundParse8BitColor,null) },
                { EchoDirectives.b24+"="  , (null, SetBackgroundParse24BitColor,null) },

                { EchoDirectives.df+"="   , (null, SetDefaultForeground,null) },
                { EchoDirectives.db+"="   , (null, SetDefaultBackground,null) },
                { EchoDirectives.rdc+""   , (RestoreDefaultColors, null,null) },

                { EchoDirectives.cls+""   , (ClearScreen, null,null) },

                { EchoDirectives.br+""    , (LineBreak, null,null) },
                { EchoDirectives.inf+""   , (Infos, null,null) },
                { EchoDirectives.bkcr+""  , (BackupCursorPos, null,null) },
                { EchoDirectives.rscr+""  , (RestoreCursorPos, null,null) },
                { EchoDirectives.crh+""   , (HideCur, null,null) },
                { EchoDirectives.crs+""   , (ShowCur, null,null) },
                { EchoDirectives.crx+"="  , (null, SetCursorX,null) },
                { EchoDirectives.cry+"="  , (null, SetCursorY,null) },
                { EchoDirectives.exit+""  , (null, Exit,null) },
                { EchoDirectives.exec+"=" , (null, ExecCSharp,null) },

                { EchoDirectives.invon+""   , (EnableInvert, null,null) },
                { EchoDirectives.lion+""    , (EnableLowIntensity, null,null) },
                { EchoDirectives.uon+""     , (EnableUnderline, null,null) },
                { EchoDirectives.bon+""     , (EnableBold, null,null) },
                { EchoDirectives.blon+""    , (EnableBlink, null,null) },
                { EchoDirectives.tdoff+""   , (DisableTextDecoration, null,null) },

                { EchoDirectives.cl+""          , (ClearLine, null,null) },
                { EchoDirectives.clright+""     , (ClearLineFromCursorRight, null,null) },
                { EchoDirectives.fillright+""   , (FillFromCursorRight, null,null) },
                { EchoDirectives.clleft+""      , (ClearLineFromCursorLeft, null,null) },

                { EchoDirectives.cup+""     , (MoveCursorTop, null,null) },
                { EchoDirectives.cdown+""   , (MoveCursorDown, null,null) },
                { EchoDirectives.cleft+""   , (MoveCursorLeft, null,null) },
                { EchoDirectives.cright+""  , (MoveCursorRight, null,null) },
                { EchoDirectives.chome+""   , (CursorHome, null,null) },

                { EchoDirectives.cnup+"="       , (null, MoveCursorTop,null) },
                { EchoDirectives.cndown+"="     , (null, MoveCursorDown,null) },
                { EchoDirectives.cnleft+"="     , (null, MoveCursorLeft,null) },
                { EchoDirectives.cnright+"="    , (null, MoveCursorRight,null) },

                // ANSI

                { EchoDirectives.ESC+"" , (null,ANSIToString,ANSI.ESC) },
                { EchoDirectives.CRLF+"" , (null,ANSIToString,ANSI.CRLF) },
                { EchoDirectives.CSI+"" , (null,ANSIToString,ANSI.CSI) },
                { EchoDirectives.DECSC+"" , (null,ANSIToString,ANSI.DECSC) },
                { EchoDirectives.DECRC+"" , (null,ANSIToString,ANSI.DECRC) },
                { EchoDirectives.RIS+"" , (null,ANSIToString,ANSI.RIS) },
                { EchoDirectives.RSTXTA+"" , (null,ANSIToString,ANSI.RSTXTA) },
                { EchoDirectives.RSCOLDEF+"" , (null,ANSIToString,ANSI.RSCOLDEF) },

                { EchoDirectives.CUU+"" , (null,ANSIToInt,ANSI.CUU) },
                { EchoDirectives.CUU+"=" , (null,ANSIToInt,ANSI.CUU) },
                { EchoDirectives.CUD+"" , (null,ANSIToInt,ANSI.CUD) },
                { EchoDirectives.CUD+"=" , (null,ANSIToInt,ANSI.CUD) },
                { EchoDirectives.CUF+"" , (null,ANSIToInt,ANSI.CUF) },
                { EchoDirectives.CUF+"=" , (null,ANSIToInt,ANSI.CUF) },
                { EchoDirectives.CUB+"" , (null,ANSIToInt,ANSI.CUB) },
                { EchoDirectives.CUB+"=" , (null,ANSIToInt,ANSI.CUB) },
                { EchoDirectives.CNL+"" , (null,ANSIToInt,ANSI.CNL) },
                { EchoDirectives.CNL+"=" , (null,ANSIToInt,ANSI.CNL) },
                { EchoDirectives.CPL+"" , (null,ANSIToInt,ANSI.CPL) },
                { EchoDirectives.CPL+"=" , (null,ANSIToInt,ANSI.CPL) },
                { EchoDirectives.CHA+"" , (null,ANSIToInt,ANSI.CHA) },
                { EchoDirectives.CHA+"=" , (null,ANSIToInt,ANSI.CHA) },
                { EchoDirectives.CUP+"" , (null,ANSITo2Int,ANSI.CUP) },
                { EchoDirectives.CUP+"=" , (null,ANSITo2Int,ANSI.CUP) },
                { EchoDirectives.SU+"" , (null,ANSIToInt,ANSI.SU) },
                { EchoDirectives.SU+"=" , (null,ANSIToInt,ANSI.SU) },
                { EchoDirectives.SD+"" , (null,ANSIToInt,ANSI.SD) },
                { EchoDirectives.SD+"=" , (null,ANSIToInt,ANSI.SD) },

                { EchoDirectives.DSR+"" , (null,ANSIToString,ANSI.DSR) },

                { EchoDirectives.ED+"=" , (null,ANSIToEDParameter,ANSI.ED) },
                { EchoDirectives.EL+"=" , (null,ANSIToELParameter,ANSI.EL) },

                { EchoDirectives.SGR_Reset+"" , (null,ANSIToString,ANSI.SGR_Reset) },
                { EchoDirectives.SGR_IncreasedIntensity+"" , (null,ANSIToString,ANSI.SGR_IncreasedIntensity) },
                { EchoDirectives.SGR_DecreaseIntensity+"" , (null,ANSIToString,ANSI.SGR_DecreaseIntensity) },
                { EchoDirectives.SGR_Italic+"" , (null,ANSIToString,ANSI.SGR_Italic) },
                { EchoDirectives.SGR_Underline+"" , (null,ANSIToString,ANSI.SGR_Underline) },
                { EchoDirectives.SGR_SlowBlink+"" , (null,ANSIToString,ANSI.SGR_SlowBlink) },
                { EchoDirectives.SGR_RapidBlink+"" , (null,ANSIToString,ANSI.SGR_RapidBlink) },
                { EchoDirectives.SGR_ReverseVideo+"" , (null,ANSIToString,ANSI.SGR_ReverseVideo) },
                { EchoDirectives.SGR_ItalicOff+"" , (null,ANSIToString,ANSI.SGR_ItalicOff) },
                { EchoDirectives.SGR_UnderlineOff+"" , (null,ANSIToString,ANSI.SGR_UnderlineOff) },
                { EchoDirectives.SGR_BlinkOff+"" , (null,ANSIToString,ANSI.SGR_BlinkOff) },
                { EchoDirectives.SGR_ReverseOff+"" , (null,ANSIToString,ANSI.SGR_ReverseOff) },
                { EchoDirectives.SGR_NotCrossedOut+"" , (null,ANSIToString,ANSI.SGR_NotCrossedOut) },
                { EchoDirectives.SGR_CrossedOut+"" , (null,ANSIToString,ANSI.SGR_CrossedOut) },
                { EchoDirectives.SGR_DoubleUnderline+"" , (null,ANSIToString,ANSI.SGR_DoubleUnderline) },
                { EchoDirectives.SGR_NormalIntensity+"" , (null,ANSIToString,ANSI.SGR_NormalIntensity) },

                { EchoDirectives.SGRF+"=" , (null,ANSI.SGRF,null) },
                { EchoDirectives.SGRF8+"=" , (null,ANSI.SGRF8,null) },
                { EchoDirectives.SGRF24+"=" , (null,ANSI.SGRF24,null) },
                { EchoDirectives.SGRB+"=" , (null,ANSI.SGRB,null) },
                { EchoDirectives.SGRB8+"=" , (null,ANSI.SGRB8,null) },
                { EchoDirectives.SGRB24+"=" , (null,ANSI.SGRB24,null) },

                // Unicode characters

                { EchoDirectives.Lire+"" , (null,UnicodeToString,Unicode.Lire) },
                { EchoDirectives.Yen+"" , (null,UnicodeToString,Unicode.Yen) },
                { EchoDirectives.None+"" , (null,UnicodeToString,Unicode.None) },
                { EchoDirectives.ARet+"" , (null,UnicodeToString,Unicode.ARet) },
                { EchoDirectives.Demi+"" , (null,UnicodeToString,Unicode.Demi) },
                { EchoDirectives.Quar+"" , (null,UnicodeToString,Unicode.Quar) },
                { EchoDirectives.ThreeQuar+"" , (null,UnicodeToString,Unicode.ThreeQuar) },
                { EchoDirectives.DoubleExclam+"" , (null,UnicodeToString,Unicode.DoubleExclam) },
                { EchoDirectives.Exp1+"" , (null,UnicodeToString,Unicode.Exp1) },
                { EchoDirectives.Exp2+"" , (null,UnicodeToString,Unicode.Exp2) },
                { EchoDirectives.Exp3+"" , (null,UnicodeToString,Unicode.Exp3) },
                { EchoDirectives.ExpRelease+"" , (null,UnicodeToString,Unicode.ExpRelease) },
                { EchoDirectives.Copyright+"" , (null,UnicodeToString,Unicode.Copyright) },
                { EchoDirectives.AE+"" , (null,UnicodeToString,Unicode.AE) },
                { EchoDirectives.AESmall+"" , (null,UnicodeToString,Unicode.AESmall) },
                { EchoDirectives.Bull+"" , (null,UnicodeToString,Unicode.Bull) },
                { EchoDirectives.ArrowThickUp+"" , (null,UnicodeToString,Unicode.ArrowThickUp) },
                { EchoDirectives.ArrowThickDown+"" , (null,UnicodeToString,Unicode.ArrowThickDown) },
                { EchoDirectives.ArrowThickLeft+"" , (null,UnicodeToString,Unicode.ArrowThickLeft) },
                { EchoDirectives.ArrowThickRight+"" , (null,UnicodeToString,Unicode.ArrowThickRight) },
                { EchoDirectives.ArrowUp+"" , (null,UnicodeToString,Unicode.ArrowUp) },
                { EchoDirectives.ArrowRight+"" , (null,UnicodeToString,Unicode.ArrowRight) },
                { EchoDirectives.ArrowDown+"" , (null,UnicodeToString,Unicode.ArrowDown) },
                { EchoDirectives.ArrowLeftRight+"" , (null,UnicodeToString,Unicode.ArrowLeftRight) },
                { EchoDirectives.ArrowUpDown+"" , (null,UnicodeToString,Unicode.ArrowUpDown) },
                { EchoDirectives.ArrowUpDownUnderline+"" , (null,UnicodeToString,Unicode.ArrowUpDownUnderline) },
                { EchoDirectives.MoreOrLess+"" , (null,UnicodeToString,Unicode.MoreOrLess) },
                { EchoDirectives.CornerBottomLeft+"" , (null,UnicodeToString,Unicode.CornerBottomLeft) },
                { EchoDirectives.BarSmallDottedVertical+"" , (null,UnicodeToString,Unicode.BarSmallDottedVertical) },
                { EchoDirectives.LeftChevron+"" , (null,UnicodeToString,Unicode.LeftChevron) },
                { EchoDirectives.RightChevron+"" , (null,UnicodeToString,Unicode.RightChevron) },
                { EchoDirectives.EdgeFlatTopRight+"" , (null,UnicodeToString,Unicode.EdgeFlatTopRight) },
                { EchoDirectives.BarHorizontal+"" , (null,UnicodeToString,Unicode.BarHorizontal) },
                { EchoDirectives.BarVertical+"" , (null,UnicodeToString,Unicode.BarVertical) },
                { EchoDirectives.EdgeTopLeft+"" , (null,UnicodeToString,Unicode.EdgeTopLeft) },
                { EchoDirectives.EdgeTopRight+"" , (null,UnicodeToString,Unicode.EdgeTopRight) },
                { EchoDirectives.EdgeBottomLeft+"" , (null,UnicodeToString,Unicode.EdgeBottomLeft) },
                { EchoDirectives.EdgeBottomRight+"" , (null,UnicodeToString,Unicode.EdgeBottomRight) },
                { EchoDirectives.EdgeRowLeft+"" , (null,UnicodeToString,Unicode.EdgeRowLeft) },
                { EchoDirectives.EdgeRowRight+"" , (null,UnicodeToString,Unicode.EdgeRowRight) },
                { EchoDirectives.EdgeColTop+"" , (null,UnicodeToString,Unicode.EdgeColTop) },
                { EchoDirectives.EdgeColBottom+"" , (null,UnicodeToString,Unicode.EdgeColBottom) },
                { EchoDirectives.EdgeRowColCross+"" , (null,UnicodeToString,Unicode.EdgeRowColCross) },
                { EchoDirectives.BarDoubleThickHorizontal+"" , (null,UnicodeToString,Unicode.BarDoubleThickHorizontal) },
                { EchoDirectives.BarDoubleHorizontal+"" , (null,UnicodeToString,Unicode.BarDoubleHorizontal) },
                { EchoDirectives.BarDoubleVertical+"" , (null,UnicodeToString,Unicode.BarDoubleVertical) },
                { EchoDirectives.EdgeDoubleTopLeft+"" , (null,UnicodeToString,Unicode.EdgeDoubleTopLeft) },
                { EchoDirectives.EdgeDoubleTopRight+"" , (null,UnicodeToString,Unicode.EdgeDoubleTopRight) },
                { EchoDirectives.EdgeDoubleBottomLeft+"" , (null,UnicodeToString,Unicode.EdgeDoubleBottomLeft) },
                { EchoDirectives.EdgeDoubleBottomRight+"" , (null,UnicodeToString,Unicode.EdgeDoubleBottomRight) },
                { EchoDirectives.EdgeDoubleRowLeft+"" , (null,UnicodeToString,Unicode.EdgeDoubleRowLeft) },
                { EchoDirectives.EdgeDoubleRowRight+"" , (null,UnicodeToString,Unicode.EdgeDoubleRowRight) },
                { EchoDirectives.EdgeDoubleColTop+"" , (null,UnicodeToString,Unicode.EdgeDoubleColTop) },
                { EchoDirectives.EdgeDoubleColBottom+"" , (null,UnicodeToString,Unicode.EdgeDoubleColBottom) },
                { EchoDirectives.EdgeDoubleRowColCross+"" , (null,UnicodeToString,Unicode.EdgeDoubleRowColCross) },
                { EchoDirectives.BoxHalfBottom+"" , (null,UnicodeToString,Unicode.BoxHalfBottom) },
                { EchoDirectives.BoxHalfTop+"" , (null,UnicodeToString,Unicode.BoxHalfTop) },
                { EchoDirectives.Box+"" , (null,UnicodeToString,Unicode.Box) },
                { EchoDirectives.BoxQuarLight+"" , (null,UnicodeToString,Unicode.BoxQuarLight) },
                { EchoDirectives.BoxTierLight+"" , (null,UnicodeToString,Unicode.BoxTierLight) },
                { EchoDirectives.BoxHalfLight+"" , (null,UnicodeToString,Unicode.BoxHalfLight) },
                { EchoDirectives.CardPic+"" , (null,UnicodeToString,Unicode.CardPic) },
                { EchoDirectives.CardTrefl+"" , (null,UnicodeToString,Unicode.CardTrefl) },
                { EchoDirectives.CardArt+"" , (null,UnicodeToString,Unicode.CardArt) },
                { EchoDirectives.CardCarro+"" , (null,UnicodeToString,Unicode.CardCarro) },

            };
#pragma warning restore CS8974 // Conversion d’un groupe de méthodes en type non-délégué

            EchoDirectiveProcessor = new EchoDirectiveProcessor(
                this,
                new CommandMap(_drtvs)
                );
        }

        #endregion

        #region commands impl. for echo directives map (avoiding lambdas in map)

        public delegate object Command1pEDParameterDelegate(ANSI.EDParameter n);
        public delegate object Command1pELParameterDelegate(ANSI.ELParameter n);

        private object? ANSIToString(object p) => p as string;

        private object UnicodeToString(object p) => ((char)p) + "";

        private object? ANSITo2Int(object parameters)
        {
            if (parameters is EchoDirectiveProcessor.Command2pIntDelegate com)
                return com.Invoke();
            if (parameters is ValueTuple<object, string> p)
            {
                try
                {
                    var command = (EchoDirectiveProcessor.Command2pIntDelegate)p.Item1;
                    var t = p.Item2.Split(":");
                    var x = Convert.ToInt32(t[0]);
                    var y = Convert.ToInt32(t[1]);
                    return command.Invoke(x, y);
                }
                catch (Exception)
                {
                    Error($"bad format and/or int value: {p.Item2} - attempted is {{Int}}:{{Int}}");
                }
            }
            return null;
        }

        private object? ANSIToInt(object parameters)
        {
            if (parameters is EchoDirectiveProcessor.Command1pIntDelegate com)
                return com.Invoke();
            if (parameters is ValueTuple<object, string> p)
            {
                try
                {
                    var command = (EchoDirectiveProcessor.Command1pIntDelegate)p.Item1;
                    return command.Invoke(Convert.ToInt32(p.Item2));
                }
                catch (Exception)
                {
                    Error($"bad Int value: {p.Item2}");
                }
            }
            return null;
        }

        private object? ANSIToEDParameter(object parameters)
        {
            if (parameters is ValueTuple<object, string> p)
            {
                try
                {
                    var command = (Command1pEDParameterDelegate)p.Item1;
                    return command.Invoke(Enum.Parse<ANSI.EDParameter>(p.Item2));
                }
                catch (Exception)
                {
                    Error($"bad EDParameter value: {p.Item2}");
                }
            }
            return null;
        }

        private object? ANSIToELParameter(object parameters)
        {
            if (parameters is ValueTuple<object, string> p)
            {
                try
                {
                    var command = (Command1pELParameterDelegate)p.Item1;
                    return command.Invoke(Enum.Parse<ANSI.ELParameter>(p.Item2));
                }
                catch (Exception)
                {
                    Error($"bad ELParameter value: {p.Item2}");
                }
            }
            return null;
        }

        private object Exit(object x)
        {
            Environment.Exit(0);
            return null;
        }

        private object? SetForegroundColor(object x)
        {
            SetForeground(TextColor.ParseColor(Console, x));
            return null;
        }

        private object? SetForegroundParse8BitColor(object x)
        {
            SetForeground(TextColor.Parse8BitColor(Console, x));
            return null;
        }

        private object? SetForegroundParse24BitColor(object x)
        {
            SetForeground(TextColor.Parse24BitColor(Console, x));
            return null;
        }

        private object? SetBackgroundColor(object x)
        {
            var c = TextColor.ParseColor(Console, x);
            if (c.HasValue)
                SetBackground(c.Value);
            return null;
        }

        private object? SetBackgroundParse8BitColor(object x)
        {
            SetBackground(TextColor.Parse8BitColor(Console, x));
            return null;
        }

        private object? SetBackgroundParse24BitColor(object x)
        {
            SetBackground(TextColor.Parse24BitColor(Console, x));
            return null;
        }

        private object? SetDefaultForeground(object x)
        {
            var c = TextColor.ParseColor(Console, x);
            if (c.HasValue)
                SetDefaultForeground(c.Value);
            return null;
        }

        private object? SetDefaultBackground(object x)
        {
            var c = TextColor.ParseColor(Console, x);
            if (c.HasValue)
                SetDefaultBackground(c.Value);
            return null;
        }

        private object? SetCursorX(object x)
        {
            CursorLeft = Console.GetCursorX(x);
            return null;
        }

        private object? SetCursorY(object x)
        {
            CursorTop = Console.GetCursorY(x);
            return null;
        }

        private object? ExecCSharp(object x)
            => CSharpScriptEngine!.ExecCSharp((string)x, this);

        private void MoveCursorTop() => MoveCursorTop(1);

        private void MoveCursorDown() => MoveCursorDown(1);

        private void MoveCursorLeft() => MoveCursorLeft(1);

        private void MoveCursorRight() => MoveCursorRight(1);

        private object? MoveCursorTop(object x)
        {
            MoveCursorTop(Convert.ToInt32(x));
            return null;
        }

        private object? MoveCursorDown(object x)
        {
            MoveCursorDown(Convert.ToInt32(x));
            return null;
        }

        private object? MoveCursorLeft(object x)
        {
            MoveCursorLeft(Convert.ToInt32(x));
            return null;
        }

        private object? MoveCursorRight(object x)
        {
            MoveCursorRight(Convert.ToInt32(x));
            return null;
        }

        #endregion

        #region buffering operations

        private void BackupCursorInformation()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            _cachedCursorPosition = CursorPos;
            _cachedBufferSize = new Size(sc.BufferWidth, sc.BufferHeight);
        }

        private void ClearCursorInformation()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            _cachedCursorPosition = Point.Empty;
            _cachedBufferSize = Size.Empty;
        }

        public override void EnableBuffer()
        {
            lock (Lock!)
            {
                if (!IsBufferEnabled)
                {
                    base.EnableBuffer();
                    BackupCursorInformation();
                }
            }
        }

        public override void CloseBuffer()
        {
            lock (Lock!)
            {
                if (IsBufferEnabled)
                {
                    base.CloseBuffer();
                    ClearCursorInformation();
                }
            }
        }

        #endregion

        #region console output operations

        public void Infos()
        {
            if (IsMute)
                return;
            var @out = Console.Out;
            var colors = Console.Colors;
            lock (@out.Lock!)
            {
                @out.Echoln($"OS={Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? "64" : "32")}bits plateform={RuntimeEnvironment.OSType}");
                @out.Echoln($"{Bkf}{colors.HighlightIdentifier}window:{Rsf} left={colors.Numeric}{sc.WindowLeft}{Rsf},top={colors.Numeric}{sc.WindowTop}{Rsf},width={colors.Numeric}{sc.WindowWidth}{Rsf},height={colors.Numeric}{sc.WindowHeight}{Rsf},largest width={colors.Numeric}{sc.LargestWindowWidth}{Rsf},largest height={colors.Numeric}{sc.LargestWindowHeight}{Rsf}");
                @out.Echoln($"{colors.HighlightIdentifier}buffer:{Rsf} width={colors.Numeric}{sc.BufferWidth}{Rsf},height={colors.Numeric}{sc.BufferHeight}{Rsf} | input encoding={colors.Numeric}{sc.InputEncoding.EncodingName}{Rsf} | output encoding={colors.Numeric}{sc.OutputEncoding.EncodingName}{Rsf}");
                @out.Echoln($"default background color={Bkf}{colors.KeyWord}{Console.DefaultBackground}{Rsf} | default foreground color={colors.KeyWord}{Console.DefaultForeground}{Rsf}");
                if (RuntimeEnvironment.OSType == itpsrv.OSPlatform.Windows)
                {
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                    @out.Echoln($"{Bkf}number lock={colors.Numeric}{sc.NumberLock}{Rsf} | capslock={colors.Numeric}{sc.CapsLock}{Rsf}");
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                    @out.Echo($"{Bkf}cursor visible={colors.Numeric}{sc.CursorVisible}{Rsf} | cursor size={colors.Numeric}{sc.CursorSize}");
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
                }
            }
        }

        public void RSTXTA()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write(ANSI.RSTXTA);
            }
        }

        public void CursorHome()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[H");
            }
        }

        public void ClearLineFromCursorRight()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[K");
            }
        }

        public void ClearLineFromCursorLeft()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[1K");
            }
        }

        public void ClearLine()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[2K");
            }
        }

        public void FillFromCursorRight()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                FillLineFromCursor(' ', false, false);
            }
        }

        public void EnableInvert()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[7m");
            }
        }

        public void EnableBlink()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                // TODO: not available on many consoles
                Write($"{(char)27}[5m");
            }
        }

        public void EnableLowIntensity()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                // TODO: not available on many consoles
                Write($"{(char)27}[2m");
            }
        }

        public void EnableUnderline()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[4m");
            }
        }

        public void EnableBold()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                // TODO: not available on many consoles
                Write($"{(char)27}[1m");
            }
        }

        public void DisableTextDecoration()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[0m");
            }
        }

        public void MoveCursorDown(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[{n}B");
            }
        }

        public void MoveCursorTop(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[{n}A");
            }
        }

        public void MoveCursorLeft(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[{n}D");
            }
        }

        public void MoveCursorRight(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[{n}C");
            }
        }

        public void ScrollWindowDown(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write(((char)27) + $"[{n}T");
            }
        }

        public void ScrollWindowUp(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write(((char)27) + $"[{n}S");
            }
        }

        /// <summary>
        /// backup the current 3bit foreground color
        /// </summary>
        public void BackupForeground()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                if (IsBufferEnabled)
                    throw new BufferedOperationNotAvailableException();
                _foregroundBackup = sc.ForegroundColor;
            }
        }

        /// <summary>
        /// backup the current 3bit background color
        /// </summary>
        public void BackupBackground()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                if (IsBufferEnabled)
                    throw new BufferedOperationNotAvailableException();
                _backgroundBackup = sc.BackgroundColor;
            }
        }

        public void RestoreForeground()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                SetForeground(Console.DefaultForeground);
            }
        }

        public void RestoreBackground()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                SetBackground(Console.DefaultBackground);
            }
        }

        /// <summary>
        /// set foreground color from a 3 bit palette color (ConsoleColor to ansi)
        /// </summary>
        /// <param name="c"></param>
        public void SetForeground(ConsoleColor? c)
        {
            if (IsMute)
                return;
            if (c == null)
                return;
            lock (Lock!)
            {
                _cachedForegroundColor = c;
                var s = ANSI.Set4BitsColorsForeground(ANSI.To4BitColorNum(c.Value));
                Write(s);
            }
        }

        public void SetBackground(ConsoleColor? c)
        {
            if (IsMute)
                return;
            if (c == null)
                return;
            lock (Lock!)
            {
                _cachedBackgroundColor = c;
                var s = ANSI.Set4BitsColorsBackground(ANSI.To4BitColorNum(c.Value));
                Write(s);
            }
        }

        /// <summary>
        /// set foreground color from a 8 bit palette color (vt/ansi)
        /// </summary>
        /// <param name="c"></param>
        public void SetForeground(int c)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[38;5;{c}m");
            }
        }

        /// <summary>
        /// set background color from a 8 bit palette color (vt/ansi)
        /// </summary>
        /// <param name="c"></param>
        public void SetBackground(int c)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[48;5;{c}m");
            }
        }

        /// <summary>
        /// set foreground color from a 24 bit palette color (vt/ansi)
        /// </summary>
        /// <param name="r">red from 0 to 255</param>
        /// <param name="g">green from 0 to 255</param>
        /// <param name="b">blue from 0 to 255</param>
        public void SetForeground(int r, int g, int b)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[38;2;{r};{g};{b}m");
            }
        }

        /// <summary>
        /// set background color from a 24 bit palette color (vt/ansi)
        /// </summary>
        /// <param name="r">red from 0 to 255</param>
        /// <param name="g">green from 0 to 255</param>
        /// <param name="b">blue from 0 to 255</param>
        public void SetBackground(int r, int g, int b)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write($"{(char)27}[48;2;{r};{g};{b}m");
            }
        }

        public void SetForeground((int r, int g, int b) color) => SetForeground(color.r, color.g, color.b);

        public void SetBackground((int r, int g, int b) color) => SetBackground(color.r, color.g, color.b);

        public void SetDefaultForeground(ConsoleColor c)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Console.DefaultForeground = c;
                sc.ForegroundColor = c;
            }
        }

        public void SetDefaultBackground(ConsoleColor c)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Console.DefaultBackground = c;
                sc.BackgroundColor = c;
            }
        }

        public void SetDefaultColors(ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                SetDefaultForeground(foregroundColor);
                SetDefaultBackground(backgroundColor);
            }
        }

        /// <summary>
        /// use RSTXTA to force colors set to defaults (avoid to reset to transparency colors)
        /// </summary>
        public void RestoreDefaultColors()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write(ANSI.RSTXTA);
                SetForeground(Console.DefaultForeground);
                SetBackground(Console.DefaultBackground);
                if (Console.DefaultForeground.HasValue)
                    sc.ForegroundColor = Console.DefaultForeground.Value;
            }
        }

        private string GetRestoreDefaultColors
        {
            get
            {
                var r = ANSI.RSTXTA;
                if (Console.DefaultForeground.HasValue)
                    r += ANSI.Set4BitsColorsForeground(ANSI.To4BitColorNum(Console.DefaultForeground.Value));
                if (Console.DefaultBackground.HasValue)
                    r += ANSI.Set4BitsColorsBackground(ANSI.To4BitColorNum(Console.DefaultBackground.Value));
                if (Console.DefaultForeground.HasValue)
                    sc.ForegroundColor = Console.DefaultForeground.Value;
                return r;
            }
        }

        public string DefaultColors => ANSI.Set4BitsColors(
            ANSI.To4BitColorNum(Console.DefaultForeground),
            ANSI.To4BitColorNum(Console.DefaultBackground));

        public void ClearScreen()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                if (IsBufferEnabled)
                    throw new BufferedOperationNotAvailableException();

                try
                {
                    WriteLine(ANSI.RSTXTA);         // reset text attr

                    System.Threading.Thread.Sleep(10);

                    sc.Write(ANSI.RIS);
                    sc.Clear();
                }
                catch (System.IO.IOException)
                {

                }
            }
        }

        public void LineBreak()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Write(LNBRK);
            }
        }

        public void ConsoleCursorPosBackup()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write(ANSI.DECSC);
            }
        }

        public void ConsoleCursorPosBackupAndRestore()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            ConsoleCursorPosBackup();
            ConsoleCursorPosRestore();
        }

        public void ConsoleCursorPosRestore()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write(ANSI.DECRC);
            }
        }

        public Task ConsoleCursorPosRestoreAsync()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return Task.CompletedTask;
            lock (Lock!)
            {
                return WriteAsync(ANSI.DECRC);
            }
        }

        public void BackupCursorPos()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                _cursorLeftBackup = CursorLeft;
                _cursorTopBackup = CursorTop;
            }
        }

        /// <summary>
        /// compat problem on low ansi
        /// </summary>
        public void RestoreCursorPos()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Write(ESC + "[2J" + ESC + $"[{_cursorTopBackup + 1};{_cursorLeftBackup + 1}H");
            }
        }

        /// <summary>
        /// get/set cursor column
        /// </summary>
        public int CursorLeft
        {
            get
            {
                if (IsMuteOrIsNotConsoleGeometryEnabled)
                    return 0;
                lock (Lock!)
                {
                    return IsBufferEnabled ? _cachedCursorPosition.X : sc.CursorLeft;
                }
            }
            set
            {
                if (IsMuteOrIsNotConsoleGeometryEnabled)
                    return;
                lock (Lock!)
                {
                    _cachedCursorPosition.X = value;
                    Write(ESC + "[" + (value + 1) + "G");
                }
            }
        }

        /// <summary>
        /// get/set cursor top
        /// </summary>
        public int CursorTop
        {
            get
            {
                if (IsMuteOrIsNotConsoleGeometryEnabled)
                    return 0;
                lock (Lock!)
                {
                    return IsBufferEnabled ? _cachedCursorPosition.X : sc.CursorTop;
                }
            }
            set
            {
                if (IsMuteOrIsNotConsoleGeometryEnabled)
                    return;
                lock (Lock!)
                {
                    _cachedCursorPosition.Y = value;
                    Write(ESC + $"[{value + 1};{CursorLeft + 1}H");
                }
            }
        }

        public Point CursorPos
        {
            get
            {
                if (IsMuteOrIsNotConsoleGeometryEnabled)
                    return new Point(0, 0);
                lock (Lock!)
                {
                    return new Point(CursorLeft, CursorTop);
                }
            }
            set
            {
                if (IsMuteOrIsNotConsoleGeometryEnabled)
                    return;
                lock (Lock!)
                {
                    Write(ESC + $"[{value.Y + 1};{value.X + 1}H");
                }
            }
        }

        public void SetCursorPos(Point p)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                var x = p.X;
                var y = p.Y;
                Console.FixCoords(ref x, ref y);
                if (IsBufferEnabled)
                {
                    _cachedCursorPosition.X = x;
                    _cachedCursorPosition.Y = y;
                }
                Write(ESC + $"[{y + 1};{x + 1}H");
            }
        }

        /// <summary>
        /// set cursor pos - @[y+1;x+1H
        /// </summary>
        /// <param name="x">x (origine 0)</param>
        /// <param name="y">y (origine 0)</param>
        public void SetCursorPos(int x, int y)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                Console.FixCoords(ref x, ref y);
                if (IsBufferEnabled)
                {
                    _cachedCursorPosition.X = x;
                    _cachedCursorPosition.Y = y;
                }
                Write(ESC + $"[{(y + 1)};{(x + 1)}H");
            }
        }

        public void HideCur()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                sc.CursorVisible = false;
            }
        }

        public void ShowCur()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                sc.CursorVisible = true;
            }
        }

        /// <summary>
        /// text only, no print directives, no ansi
        /// </summary>
        /// <param name="s">text to be filtered</param>
        /// <returns>text visible characters only</returns>
        public string GetText(string s)
        {
            var r = GetPrint(s, false, false);
            return r;
        }

        private string GetPrint(
            string s,
            bool lineBreak = false,
            bool ignorePrintDirectives = false,
            EchoSequenceList? printSequences = null)
        {
            lock (Lock!)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return s;
                var ms = new MemoryStream(s.Length * 4);
                var sw = new StreamWriter(ms);
                Console.RedirectOut(sw);
                var e = Console.EnableConstraintConsolePrintInsideWorkArea;
                Console.EnableConstraintConsolePrintInsideWorkArea = false;
                if (!ignorePrintDirectives)
                {
                    // directives are removed
                    s = ANSI.GetText(s);    // also removed ansi sequences
                    Echo(s, lineBreak, true, true, printSequences);
                }
                else
                {
                    // directives are keeped
                    Echo(s, lineBreak, false, true, printSequences, false, false);
                }
                ms.Position = 0;
                Console.EnableConstraintConsolePrintInsideWorkArea = e;
                sw.Flush();
                var rw = new StreamReader(ms);
                var txt = rw.ReadToEnd();
                rw.Close();
                Console.RedirectOut((StreamWriter?)null);
                return txt;
            }
        }

        public string GetPrintWithEscapeSequences(string s, bool lineBreak = false)
        {
            lock (Lock!)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return s;
                var ms = new MemoryStream(s.Length * 4);
                var sw = new StreamWriter(ms);
                Console.RedirectOut(sw);
                var e = Console.EnableConstraintConsolePrintInsideWorkArea;
                Console.EnableConstraintConsolePrintInsideWorkArea = false;
                Echo(s, lineBreak);
                Console.EnableConstraintConsolePrintInsideWorkArea = e;
                sw.Flush();
                ms.Position = 0;
                var rw = new StreamReader(ms);
                var txt = rw.ReadToEnd();
                rw.Close();
                Console.RedirectOut((StreamWriter?)null);
                return txt;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ConsolePrint(string s, bool lineBreak = false)
        {
            if (IsMute)
                return;
            // any print goes here...
            lock (Lock!)
            {
                if (CropX == -1)
                {
                    ConsoleSubPrint(s, lineBreak);
                }
                else
                {
                    var x = CursorLeft;
                    var mx = Math.Max(x, CropX);
                    if (mx > x)
                    {
                        var n = mx - x + 1;
                        if (s.Length <= n)
                            ConsoleSubPrint(s, lineBreak);
                        else
                            ConsoleSubPrint(s[..n], lineBreak);
                    }
                }
            }
        }

        /// <summary>
        /// debug echo to file
        /// </summary>
        /// <param name="s"></param>
        /// <param name="lineBreak"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerLineNumber"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override void EchoDebug(
            string s,
            bool lineBreak = false,
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (IsMute)
                return;
            if (!FileEchoDebugEnabled)
                return;
            if (FileEchoDebugDumpDebugInfo)
            {
                if (IsBufferEnabled)
                    DebugEchoStreamWriter?.Write($"x={CursorLeft},y={CursorTop},l={s.Length}, bw={_cachedBufferSize},bh={_cachedBufferSize},br={lineBreak} [{callerMemberName}:{callerLineNumber}] :");
                else
                    DebugEchoStreamWriter?.Write($"x={CursorLeft},y={CursorTop},l={s.Length},w={sc.WindowWidth},h={sc.WindowHeight},wtop={sc.WindowTop} bw={sc.BufferWidth},bh={sc.BufferHeight},br={lineBreak} [{callerMemberName}:{callerLineNumber}] :");
            }
            DebugEchoStreamWriter?.Write(s);
            if (lineBreak | FileEchoDebugAutoLineBreak)
                DebugEchoStreamWriter?.WriteLine(string.Empty);
            if (FileEchoDebugAutoFlush)
                DebugEchoStreamWriter?.Flush();
        }

        /// <summary>
        /// simplesystem.diagnostics.debug
        /// </summary>
        public void Debug(
            string s,
            bool lineBreak = false,
            [CallerFilePath] string callerFilePath = "",
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (IsMute)
                return;
            System.Diagnostics.Debug.Write($"{Path.GetFileName(callerFilePath)}:{callerLineNumber} | {callerMemberName} °°° {s}");
            if (lineBreak)
                System.Diagnostics.Debug.WriteLine(string.Empty);
        }

        public override void Write(string s)
        {
            if (RedirectToErr)
            {
                if (IsReplicationEnabled)
                    ReplicateStreamWriter!.Write(s);

                Console.Err.Write(s);
            }
            else
            {
                if (IsNotMute)
                    base.Write(s);
            }
        }

        public void Echoln(string s = "", bool ignorePrintDirectives = false) => Echo(s, true, !ignorePrintDirectives);
        public void Echoln(object s, bool ignorePrintDirectives = false) => Echo(s, true, !ignorePrintDirectives);

        public void Echo(
            string s = "",
            bool lineBreak = false,
            bool ignorePrintDirectives = false) => Echo(s, lineBreak, !ignorePrintDirectives);

        public void Echoln(char s, bool ignorePrintDirectives = false) => Echo(s + "", true, !ignorePrintDirectives);

        public void Echo(char s, bool lineBreak = false, bool ignorePrintDirectives = false) => Echo(s + "", lineBreak, !ignorePrintDirectives);

        public void Echo(
            object o,
            bool lineBreak = false)
            => Echo(o, lineBreak);

        /// <summary>
        /// output to stream
        /// </summary>
        /// <param name="o">object to output - is transform to string with ToText</param>
        /// <param name="lineBreak">if true, append a line break to output (call LineBreak()), default is false</param>
        /// <param name="parseCommands">if true, echo directives are parsed and evaluated, default is true</param>
        /// <param name="doNotEvalutatePrintDirectives">TODO: explain this parameter</param>
        /// <param name="printSequences">to store echo sequence objects when collected</param>
        /// <param name="avoidANSISequencesAndNonPrintableCharacters">if true and parseCommands=false, replace ansiseq and non printable chars by readable data</param>
        /// <param name="getNonPrintablesASCIICodesAsLabel">if true and parseCommands=false, replace ascii non printables chars by labels</param>
        private void Echo(
            object o,
            bool lineBreak = false,
            bool parseCommands = true,
            bool doNotEvalutatePrintDirectives = false,         // TODO: explain this
            EchoSequenceList? printSequences = null,
            bool avoidANSISequencesAndNonPrintableCharacters = true,
            bool getNonPrintablesASCIICodesAsLabel = true
            )
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                if (o == null)
                {
                    if (DumpNullStringAsText != null)
                        ConsolePrint(DumpNullStringAsText, false);
                }
                else
                {
                    if (parseCommands)
                    {
                        // call the EchoDirective component
                        EchoDirectiveProcessor.ParseTextAndApplyCommands(
                            o.ToString()!,
                            false,
                            "",
                            doNotEvalutatePrintDirectives,
                            printSequences);
                    }
                    else
                    {
                        var txt = o.ToString()!;
                        if (getNonPrintablesASCIICodesAsLabel)
                        {
                            txt = ASCII.GetNonPrintablesCodesAsLabel(
                                txt, false /* true: show all symbols */ );
                        }

                        if (avoidANSISequencesAndNonPrintableCharacters)
                            txt = ANSI.AvoidANSISequencesAndNonPrintableCharacters(txt);
                        ConsolePrint(txt, false);
                    }
                }

                if (lineBreak)
                    LineBreak();
            }
        }

        public void Warningln(string s) => Warning(s, true);

        public void Warning(string s, bool lineBreak = true)
        {
            if (IsNotMute)
                Console.Out.Echo($"{Console.Colors.Warning}{s}{Console.Colors.Default}", lineBreak);
        }

        public void Errorln(string s) => Error(s, true);

        public void Error(string s, bool lineBreak = true)
        {
            if (IsNotMute)
            {
                RedirectToErr = true;
                Console.Out.Echo($"{Console.Colors.Error}{s}{Console.Colors.Default}", lineBreak);
                RedirectToErr = false;
            }
            else
            {
                Console.Err.Write(s);
                if (lineBreak)
                    Console.Err.WriteLine(string.Empty);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ConsoleSubPrint(string s, bool lineBreak = false)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                if (Console.EnableConstraintConsolePrintInsideWorkArea)
                {
                    var (id, x, y, w, h) = Console.ActualWorkArea();
                    var x0 = CursorLeft;
                    var y0 = CursorTop;

                    var croppedLines = new List<string>();
                    var xr = x0 + s.Length - 1;
                    var xm = x + w - 1;
                    if (xr > xm)
                    {
                        while (xr > xm && s.Length > 0)
                        {
                            var left = s[..^(xr - xm)];
                            s = s.Substring(s.Length - (xr - xm), xr - xm);
                            croppedLines.Add(left);
                            xr = x + s.Length - 1;
                        }
                        if (s.Length > 0)
                            croppedLines.Add(s);
                        var curx = x0;
                        foreach (var line in croppedLines)
                        {
                            Write(line);
                            x0 += line.Length;
                            SetCursorPosConstraintedInWorkArea(ref x0, ref y0);
                            EchoDebug(line);
                        }
                        if (lineBreak)
                        {
                            x0 = x;
                            y0++;
                            SetCursorPosConstraintedInWorkArea(ref x0, ref y0);
                        }
                    }
                    else
                    {
                        Write(s);
                        x0 += s.Length;
                        SetCursorPosConstraintedInWorkArea(ref x0, ref y0);
                        EchoDebug(s);
                        if (lineBreak)
                        {
                            x0 = x;
                            y0++;
                            SetCursorPosConstraintedInWorkArea(ref x0, ref y0);
                        }
                    }
                }
                else
                {
                    Write(s);
                    EchoDebug(s);

                    if (lineBreak)
                    {
                        WriteLine(string.Empty);
                        EchoDebug(string.Empty, true);
                    }
                }
            }
        }

        private void FillLineFromCursor(char c = ' ', bool resetCursorLeft = true, bool useDefaultColors = true)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                if (!EnableFillLineFromCursor)
                    return;
                var f = _cachedForegroundColor;
                var b = _cachedForegroundColor;
                var aw = Console.ActualWorkArea();
                var nb = Math.Max(0, Math.Max(aw.Right, _cachedBufferSize.Width - 1) - CursorLeft - 1);
                var x = CursorLeft;
                var y = CursorTop;
                if (useDefaultColors)
                {
                    SetForeground(ColorSettings.Default.Foreground);
                    SetBackground(ColorSettings.Default.Background);
                }
                Write("".PadLeft(nb, c));   // TODO: BUG in WINDOWS: do not print the last character
                SetCursorPos(nb, y);
                Write(" ");
                if (useDefaultColors)
                {
                    SetForeground(f);
                    SetBackground(b);
                }
                if (resetCursorLeft)
                    CursorLeft = x;
            }
        }

        public int GetIndexInWorkAreaConstraintedString(
            string s,
            Point origin,
            Point cursorPos,
            bool forceEnableConstraintInWorkArea = false,
            bool fitToVisibleArea = true,
            bool doNotEvaluatePrintDirectives = true,
            bool ignorePrintDirectives = false
            )
            => GetIndexInWorkAreaConstraintedString(
                s,
                origin,
                cursorPos.X,
                cursorPos.Y,
                forceEnableConstraintInWorkArea,
                fitToVisibleArea,
                doNotEvaluatePrintDirectives,
                ignorePrintDirectives);

        public int GetIndexInWorkAreaConstraintedString(
            string s,
            Point origin,
            int cursorX,
            int cursorY,
            bool forceEnableConstraintInWorkArea = false,
            bool fitToVisibleArea = true,
            bool doNotEvaluatePrintDirectives = true,
            bool ignorePrintDirectives = false)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return 0;
            var r = GetWorkAreaStringSplits(
                s,
                origin,
                forceEnableConstraintInWorkArea,
                fitToVisibleArea,
                doNotEvaluatePrintDirectives,
                ignorePrintDirectives,
                cursorX,
                cursorY
                );
            return r.CursorIndex;
        }

        private LineSplitList GetIndexLineSplitsInWorkAreaConstraintedString(
            string s,
            Point origin,
            int cursorX,
            int cursorY,
            bool forceEnableConstraintInWorkArea = false,
            bool fitToVisibleArea = true,
            bool doNotEvaluatePrintDirectives = false,
            bool ignorePrintDirectives = false)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
            {
                return new LineSplitList(
                    new List<StringSegment>() { new StringSegment(s, 0, s.Length - 1) }, null, 0, 0
                );
            }

            var r = GetWorkAreaStringSplits(
                s,
                origin,
                forceEnableConstraintInWorkArea,
                fitToVisibleArea,
                doNotEvaluatePrintDirectives,
                ignorePrintDirectives,
                cursorX,
                cursorY
                );
            return r;
        }

        /// <summary>
        /// TODO: check for buffered mode
        /// </summary>
        /// <param name="s"></param>
        /// <param name="origin"></param>
        /// <param name="forceEnableConstraintInWorkArea"></param>
        /// <param name="fitToVisibleArea"></param>
        /// <param name="doNotEvaluatePrintDirectives"></param>
        /// <param name="ignorePrintDirectives"></param>
        /// <param name="cursorX"></param>
        /// <param name="cursorY"></param>
        /// <returns></returns>
        private LineSplitList GetWorkAreaStringSplits(
            string s,
            Point origin,
            bool forceEnableConstraintInWorkArea = false,
            bool fitToVisibleArea = true,
            bool doNotEvaluatePrintDirectives = false,
            bool ignorePrintDirectives = false,
            int cursorX = -1,
            int cursorY = -1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
            {
                return new LineSplitList(
                    new List<StringSegment>() { new StringSegment(s, 0, s.Length - 1) }, null, 0, 0
                );
            }

            var originalString = s;
            var r = new List<StringSegment>();
            EchoSequenceList? printSequences = null;

            if (cursorX == -1)
                cursorX = origin.X;
            if (cursorY == -1)
                cursorY = origin.Y;
            var cursorLineIndex = -1;
            var cursorIndex = -1;

            lock (Lock!)
            {
                var index = -1;
                var (id, x, y, w, h) = Console.ActualWorkArea(fitToVisibleArea);
                var x0 = origin.X;
                var y0 = origin.Y;

                var croppedLines = new List<StringSegment>();
                string? pds = null;
                var length = s.Length;
                if (doNotEvaluatePrintDirectives)
                {
                    pds = s;
                    printSequences = new EchoSequenceList();
                    s = GetPrint(s, false, ignorePrintDirectives, printSequences);
                }
                var xr = x0 + s.Length - 1;
                var xm = x + w - 1;

                if (xr >= xm)
                {
                    if (pds != null)
                    {
                        var lineSegments = new List<string>();
                        var currentLine = string.Empty;
                        var lastIndex = 0;

                        foreach (var ps in printSequences!)
                        {
                            if (!ps.IsText)
                            {
                                lineSegments.Add(ps.ToText());
                            }
                            else
                            {
                                currentLine += ps.Text;
                                xr = x0 + currentLine.Length - 1;
                                if (xr > xm && currentLine.Length > 0)
                                {
                                    while (xr > xm && currentLine.Length > 0)
                                    {
                                        var left = currentLine[..^(xr - xm)];
                                        currentLine = currentLine.Substring(currentLine.Length - (xr - xm), xr - xm);

                                        var truncLeft = left[lastIndex..];
                                        lineSegments.Add(truncLeft);
                                        croppedLines.Add(new StringSegment(string.Join("", lineSegments), 0, 0, lastIndex + truncLeft.Length));
                                        lineSegments.Clear();
                                        lastIndex = 0;

                                        xr = x + currentLine.Length - 1;
                                    }
                                    if (currentLine.Length > 0)
                                    {
                                        lineSegments.Add(currentLine);
                                        lastIndex = currentLine.Length;
                                    }
                                }
                                else
                                {
                                    lineSegments.Add(currentLine[lastIndex..]);
                                    lastIndex = currentLine.Length;
                                }
                            }
                        }

                        if (lineSegments.Count > 0)
                        {
                            var truncLeft = currentLine[lastIndex..];
                            lineSegments.Add(truncLeft);
                            croppedLines.Add(new StringSegment(string.Join("", lineSegments), 0, 0, lastIndex + truncLeft.Length));
                            lineSegments.Clear();
                            lastIndex = 0;
                        }
                    }
                    else
                    {
                        while (xr > xm && s.Length > 0)
                        {
                            var left = s[..^(xr - xm)];
                            s = s.Substring(s.Length - (xr - xm), xr - xm);
                            croppedLines.Add(new StringSegment(left, 0, 0, left.Length));
                            xr = x + s.Length - 1;
                        }
                        if (s.Length > 0)
                            croppedLines.Add(new StringSegment(s, 0, 0, s.Length));
                    }

                    var curx = x0;
                    var lineIndex = 0;
                    index = 0;
                    var indexFounds = false;
                    foreach (var line in croppedLines)
                    {
                        r.Add(new StringSegment(line.Text, x0, y0, line.Length));
                        if (!indexFounds && cursorY == y0)
                        {
                            index += cursorX - x0;
                            cursorIndex = index;
                            cursorLineIndex = lineIndex;
                            indexFounds = true;
                        }
                        x0 += line.Length;
                        index += line.Length;
                        SetCursorPosConstraintedInWorkArea(ref x0, ref y0, false, forceEnableConstraintInWorkArea, fitToVisibleArea);
                        lineIndex++;
                    }
                    if (!indexFounds)
                    {
                        cursorIndex = index;
                        cursorLineIndex = lineIndex;
                    }
                }
                else
                {
                    cursorIndex = cursorX - x0;
                    cursorLineIndex = 0;
                    if (pds != null)
                        r.Add(new StringSegment(pds, x0, y0, pds.Length));
                    else
                        r.Add(new StringSegment(s, x0, y0, s.Length));
                }
            }

            if (!doNotEvaluatePrintDirectives)
            {
                printSequences = new EchoSequenceList
                {
                    new EchoSequence(Console, (string?)null, 0, originalString.Length - 1, null, originalString)
                };
            }

            return new LineSplitList(r, printSequences, cursorIndex, cursorLineIndex);
        }

        public void SetCursorPosConstraintedInWorkArea(Point pos, bool enableOutput = true, bool forceEnableConstraintInWorkArea = false, bool fitToVisibleArea = true)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            var x = pos.X;
            var y = pos.Y;
            SetCursorPosConstraintedInWorkArea(ref x, ref y, enableOutput, forceEnableConstraintInWorkArea, fitToVisibleArea);
        }

        public void SetCursorPosConstraintedInWorkArea(int cx, int cy, bool enableOutput = true, bool forceEnableConstraintInWorkArea = false, bool fitToVisibleArea = true)
            => SetCursorPosConstraintedInWorkArea(ref cx, ref cy, enableOutput, forceEnableConstraintInWorkArea, fitToVisibleArea);

        /// <summary>
        /// TODO: check for buffered mode
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="enableOutput"></param>
        /// <param name="forceEnableConstraintInWorkArea"></param>
        /// <param name="fitToVisibleArea"></param>
        public void SetCursorPosConstraintedInWorkArea(ref int cx, ref int cy, bool enableOutput = true, bool forceEnableConstraintInWorkArea = false, bool fitToVisibleArea = true)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                var dx = 0;
                var dy = 0;

                if (Console.EnableConstraintConsolePrintInsideWorkArea || forceEnableConstraintInWorkArea)
                {
                    var (id, left, top, right, bottom) = Console.ActualWorkArea(fitToVisibleArea);
                    if (cx < left)
                    {
                        cx = right - 1;
                        cy--;
                    }
                    if (cx >= right)
                    {
                        cx = left;
                        cy++;
                    }

                    if (enableOutput && cy < top)
                    {
                        dy = top - cy;
                        cy += dy;
                        if (top + 1 <= bottom)
                        {
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                            sc.MoveBufferArea(      // TODO: not supported on linux (ubuntu 18.04 wsl)
                                left, top, right, bottom - top,
                                left, top + 1,
                                ' ',
                                Console.DefaultForeground ?? ConsoleColor.White, Console.DefaultBackground ?? ConsoleColor.Black);
                        }
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
                    }

                    if (enableOutput && cy > bottom /*- 1*/)
                    {
                        dy = bottom /*- 1*/ - cy;
                        cy += dy;
                        var nh = bottom - top + dy + 1;
                        if (nh > 0)
                        {
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                            sc.MoveBufferArea(      // TODO: not supported on linux (ubuntu 18.04 wsl)
                                left, top - dy, right, nh,
                                left, top,
                                ' ',
                                Console.DefaultForeground ?? ConsoleColor.White, Console.DefaultBackground ?? ConsoleColor.Black);
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
                        }
                    }
                }

                if (enableOutput)
                {
                    SetCursorPos(cx, cy);
                    if (dx != 0 || dy != 0)
                        Console.WorkAreaScrolled?.Invoke(null, new WorkAreaScrollEventArgs(0, dy));
                }
            }
        }

        #endregion

    }
}
