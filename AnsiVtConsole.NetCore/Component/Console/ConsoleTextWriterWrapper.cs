#define Ignore_BufferedOperationNotAvailableException

using System.Drawing;
using System.Runtime.CompilerServices;

using AnsiVtConsole.NetCore.Component.EchoDirective;
using AnsiVtConsole.NetCore.Component.Parser.ANSI;
using AnsiVtConsole.NetCore.Component.Script;
using AnsiVtConsole.NetCore.Component.UI;
using AnsiVtConsole.NetCore.Lib;

using static AnsiVtConsole.NetCore.Component.EchoDirective.Shortcuts;
using static AnsiVtConsole.NetCore.Lib.Str;

using itpsrv = System.Runtime.InteropServices;
using sc = System.Console;

namespace AnsiVtConsole.NetCore.Component.Console
{
    /// <summary>
    /// ansi vt console text writer wrapping a text writer wrapper
    /// </summary>
    public sealed class ConsoleTextWriterWrapper : TextWriterWrapper
    {
        #region attributes

        /// <summary>
        /// text info about the console
        /// </summary>
        /// <returns>text</returns>
        public override string ToString() => $"[console text writer wrapper - {base.ToString()}]";

        /// <summary>
        /// is mute and is console geometry enabled
        /// </summary>
        public bool IsMuteOrIsNotConsoleGeometryEnabled => IsMute || !Console.WorkArea.IsConsoleGeometryEnabled;

        /// <summary>
        /// is not mute and is console geometry enabled
        /// </summary>
        public bool IsNotMuteAndIsConsoleGeometryEnabled => IsNotMute && Console.WorkArea.IsConsoleGeometryEnabled;

        /// <summary>
        /// is redirected to Err
        /// </summary>
        public bool RedirectToErr { get; set; } = false;

        /// <summary>
        /// ansi vt console
        /// </summary>
        public IAnsiVtConsole Console { get; private set; }

        /// <summary>
        /// console colors
        /// </summary>
        public ColorSettings ColorSettings { get; private set; }

        int _cursorLeftBackup;
        int _cursorTopBackup;
        ConsoleColor? _foregroundBackup;
        ConsoleColor? _backgroundBackup;
        EchoDirectiveProcessor _echoDirectiveProcessor;

        /// <summary>
        /// the Escape character
        /// </summary>
        public static readonly string ESC = (char)27 + "";

        /// <summary>
        /// line break that fix end of line that may remained filled with current background color
        /// </summary>
        public string LNBRK =>
                // fix end of line remained filled with last colors
                EnableAvoidEndOfLineFilledWithBackgroundColor ?
                        $"{ANSI.RSTXTA}{ANSI.EL(ANSI.ELParameter.p0)}{ANSI.CRLF}{_restoreDefaultColorsAnsiSequence}"
                        : $"{ANSI.CRLF}";

        /// <summary>
        /// csharp script engine
        /// </summary>
        public CSharpScriptEngine? CSharpScriptEngine { get; private set; }

        #endregion

        #region console output settings

        /// <summary>
        /// crop x coordinate
        /// </summary>
        public int CropX = -1;

        /// <summary>
        /// auto fill lines from cursor to the right
        /// </summary>
        public bool EnableFillLineFromCursor = true;

        /// <summary>
        /// enable the trick for old console to avoid the end of line to be filled with line background color
        /// </summary>
        public bool EnableAvoidEndOfLineFilledWithBackgroundColor = true;

        #endregion

        #region console information cache

        Point _cachedCursorPosition = Point.Empty;
        Size _cachedBufferSize = Size.Empty;
        ConsoleColor? _cachedForegroundColor;
        ConsoleColor? _cachedBackgroundColor;

        #endregion

        #region init

#pragma warning disable CS8618 

        /// <summary>
        /// text writer for a console
        /// </summary>
        /// <param name="console">console</param>
        public ConsoleTextWriterWrapper(IAnsiVtConsole console) : base() => Init(console);

        /// <summary>
        /// text writer for a console build over a text wrapper
        /// </summary>
        /// <param name="console">console</param>
        /// <param name="textWriter">text wrapper</param>
        public ConsoleTextWriterWrapper(IAnsiVtConsole console, TextWriter textWriter) : base(textWriter) => Init(console);

        /// <summary>
        /// text writer for a console
        /// </summary>
        /// <param name="console">console</param>
        /// <param name="cSharpScriptEngine">a csharp script engine used for C# script handling in markup</param>
        public ConsoleTextWriterWrapper(IAnsiVtConsole console, CSharpScriptEngine cSharpScriptEngine) : base() => Init(console, cSharpScriptEngine);

        /// <summary>
        /// text writer for a console build over a text wrapper
        /// </summary>
        /// <param name="console">console</param>
        /// <param name="textWriter">text writer</param>
        /// <param name="cSharpScriptEngine">a csharp script engine used for C# script handling in markup</param>
        public ConsoleTextWriterWrapper(IAnsiVtConsole console, TextWriter textWriter, CSharpScriptEngine cSharpScriptEngine) : base(textWriter) => Init(console, cSharpScriptEngine);
#pragma warning restore CS8618

        /// <summary>
        /// console init + internal init
        /// </summary>
        void Init(IAnsiVtConsole console, CSharpScriptEngine? cSharpScriptEngine = null)
        {
            Console = console;
            console.WorkArea.CheckConsoleHasGeometry();
            CSharpScriptEngine = cSharpScriptEngine ?? new CSharpScriptEngine(console);

            if (IsNotMute)
            {
                // TIP: dot not affect background color throught System.Console.Background to preserve terminal console background transparency
                Console.Settings.DefaultForeground = sc.ForegroundColor;
                _cachedForegroundColor = Console.Settings.DefaultForeground;
            }

            InitEchoDirectives();
        }

        void InitEchoDirectives()
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

                { EchoDirectives.DoubleUnderline+"" , (null,UnicodeToString,Unicode.DoubleUnderline) },
                { EchoDirectives.Cross+"" , (null,UnicodeToString,Unicode.Cross) },
                { EchoDirectives.OutlinedCross+"" , (null,UnicodeToString,Unicode.OutlinedCross) },
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

            _echoDirectiveProcessor = new EchoDirectiveProcessor(
                this,
                new CommandMap(_drtvs)
                );
        }

        #endregion

        #region commands impl. for echo directives map (avoiding lambdas in map)

        delegate object Command1pEDParameterDelegate(ANSI.EDParameter n);

        delegate object Command1pELParameterDelegate(ANSI.ELParameter n);

        object? ANSIToString(object p) => p as string;

        object UnicodeToString(object p) => ((char)p) + "";

        object? ANSITo2Int(object parameters)
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

        object? ANSIToInt(object parameters)
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

        object? ANSIToEDParameter(object parameters)
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

        object? ANSIToELParameter(object parameters)
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

        /// <summary>
        /// exit the console client
        /// </summary>
        /// <param name="x">not used</param>
        object Exit(object x)
        {
            Environment.Exit(0);
            return null;
        }

        /// <summary>
        /// set foreground color from parsed from a console color (3bit,console color name) text descriptioh
        /// </summary>
        /// <param name="x">color</param>
        object? SetForegroundColor(object x)
        {
            SetForeground(TextColor.ParseColor(Console, x));
            return null;
        }

        /// <summary>
        /// set foreground color from parsed from a 8bits text descriptioh
        /// </summary>
        /// <param name="x">color</param>
        object? SetForegroundParse8BitColor(object x)
        {
            SetForeground(TextColor.Parse8BitColor(Console, x));
            return null;
        }

        /// <summary>
        /// set foreground color from parsed from a 24bits text descriptioh
        /// </summary>
        /// <param name="x">color</param>
        object? SetForegroundParse24BitColor(object x)
        {
            SetForeground(TextColor.Parse24BitColor(Console, x));
            return null;
        }

        /// <summary>
        /// set background color from parsed from a console color (3bit,console color name) text descriptioh
        /// </summary>
        /// <param name="x">color</param>
        object? SetBackgroundColor(object x)
        {
            var c = TextColor.ParseColor(Console, x);
            if (c.HasValue)
                SetBackground(c.Value);
            return null;
        }

        /// <summary>
        /// set background color from parsed from a 8bits text descriptioh
        /// </summary>
        /// <param name="x">color</param>
        object? SetBackgroundParse8BitColor(object x)
        {
            SetBackground(TextColor.Parse8BitColor(Console, x));
            return null;
        }

        /// <summary>
        /// set background color from parsed from a 24bits text descriptioh
        /// </summary>
        /// <param name="x">color</param>
        object? SetBackgroundParse24BitColor(object x)
        {
            SetBackground(TextColor.Parse24BitColor(Console, x));
            return null;
        }

        /// <summary>
        /// set default foreground color
        /// </summary>
        /// <param name="x">color</param>
        object? SetDefaultForeground(object x)
        {
            var c = TextColor.ParseColor(Console, x);
            if (c.HasValue)
                SetDefaultForeground(c.Value);
            return null;
        }

        /// <summary>
        /// Set default background color
        /// </summary>
        /// <param name="x">color</param>
        object? SetDefaultBackground(object x)
        {
            var c = TextColor.ParseColor(Console, x);
            if (c.HasValue)
                SetDefaultBackground(c.Value);
            return null;
        }

        /// <summary>
        /// set cursor x
        /// </summary>
        /// <param name="x">x</param>
        object? SetCursorX(object x)
        {
            CursorLeft = Console.Cursor.GetCursorX(x);
            return null;
        }

        /// <summary>
        /// set cursor y
        /// </summary>
        /// <param name="x">y</param>
        object? SetCursorY(object x)
        {
            CursorTop = Console.Cursor.GetCursorY(x);
            return null;
        }

        /// <summary>
        /// executes a csharp scrit
        /// </summary>
        /// <param name="x">script test</param>
        object? ExecCSharp(object x)
            => CSharpScriptEngine!.ExecCSharp((string)x, this);

        /// <summary>
        /// moves cursor 1 line top
        /// </summary>
        void MoveCursorTop() => MoveCursorTop(1);

        /// <summary>
        /// moves cursor 1 line down
        /// </summary>
        void MoveCursorDown() => MoveCursorDown(1);

        /// <summary>
        /// moves cursor 1 column left
        /// </summary>
        void MoveCursorLeft() => MoveCursorLeft(1);

        /// <summary>
        /// moves cursor 1 column right
        /// </summary>
        void MoveCursorRight() => MoveCursorRight(1);

        /// <summary>
        /// moves cursor top
        /// </summary>
        /// <param name="x">lines count</param>
        object? MoveCursorTop(object x)
        {
            MoveCursorTop(Convert.ToInt32(x));
            return null;
        }

        /// <summary>
        /// moves cursor down
        /// </summary>
        /// <param name="x">lines count</param>
        object? MoveCursorDown(object x)
        {
            MoveCursorDown(Convert.ToInt32(x));
            return null;
        }

        /// <summary>
        /// moves cursor left
        /// </summary>
        /// <param name="x">columns count</param>
        object? MoveCursorLeft(object x)
        {
            MoveCursorLeft(Convert.ToInt32(x));
            return null;
        }

        /// <summary>
        /// moves cursor right
        /// </summary>
        /// <param name="x">columns count</param>
        object? MoveCursorRight(object x)
        {
            MoveCursorRight(Convert.ToInt32(x));
            return null;
        }

        #endregion

        #region buffering operations

        void BackupCursorInformation()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            _cachedCursorPosition = CursorPos;
            _cachedBufferSize = new Size(sc.BufferWidth, sc.BufferHeight);
        }

        void ClearCursorInformation()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            _cachedCursorPosition = Point.Empty;
            _cachedBufferSize = Size.Empty;
        }

        /// <summary>
        /// enable output to a buffer
        /// </summary>
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

        /// <summary>
        /// close the output buffer
        /// </summary>
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

        /// <summary>
        /// write info about the console
        /// </summary>
        public void Infos()
        {
            if (IsMute)
                return;
            var @out = Console.Out;
            var colors = Console.Colors;
            lock (@out.Lock!)
            {
                @out.WriteLine($"OS={Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? "64" : "32")}bits plateform={RuntimeEnvironment.OSType}");
                @out.WriteLine($"{Bkf}{colors.HighlightIdentifier}window:{Rsf} left={colors.Numeric}{sc.WindowLeft}{Rsf},top={colors.Numeric}{sc.WindowTop}{Rsf},width={colors.Numeric}{sc.WindowWidth}{Rsf},height={colors.Numeric}{sc.WindowHeight}{Rsf},largest width={colors.Numeric}{sc.LargestWindowWidth}{Rsf},largest height={colors.Numeric}{sc.LargestWindowHeight}{Rsf}");
                @out.WriteLine($"{colors.HighlightIdentifier}buffer:{Rsf} width={colors.Numeric}{sc.BufferWidth}{Rsf},height={colors.Numeric}{sc.BufferHeight}{Rsf} | input encoding={colors.Numeric}{sc.InputEncoding.EncodingName}{Rsf} | output encoding={colors.Numeric}{sc.OutputEncoding.EncodingName}{Rsf}");
                @out.WriteLine($"default background color={Bkf}{colors.KeyWord}{Console.Settings.DefaultBackground}{Rsf} | default foreground color={colors.KeyWord}{Console.Settings.DefaultForeground}{Rsf}");
                if (RuntimeEnvironment.OSType == itpsrv.OSPlatform.Windows)
                {
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                    @out.WriteLine($"{Bkf}number lock={colors.Numeric}{sc.NumberLock}{Rsf} | capslock={colors.Numeric}{sc.CapsLock}{Rsf}");
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
                    @out.Write($"{Bkf}cursor visible={colors.Numeric}{sc.CursorVisible}{Rsf} | cursor size={colors.Numeric}{sc.CursorSize}");
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
                }
            }
        }

        /// <summary>
        /// reset text attributes
        /// </summary>
        public void RSTXTA()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                WriteStream(ANSI.RSTXTA);
            }
        }

        /// <summary>
        /// set cursor at home
        /// </summary>
        public void CursorHome()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[H");
            }
        }

        /// <summary>
        /// clear line from cursor right
        /// </summary>
        public void ClearLineFromCursorRight()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[K");
            }
        }

        /// <summary>
        /// clear line from cursor left
        /// </summary>
        public void ClearLineFromCursorLeft()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[1K");
            }
        }

        /// <summary>
        /// clear line
        /// </summary>
        public void ClearLine()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[2K");
            }
        }

        /// <summary>
        /// fille lin from cursor right
        /// </summary>
        public void FillFromCursorRight()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                FillLineFromCursor(' ', false, false);
            }
        }

        /// <summary>
        /// enable invert
        /// </summary>
        public void EnableInvert()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[7m");
            }
        }

        /// <summary>
        /// enable blink
        /// </summary>
        public void EnableBlink()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                // TODO: not available on many consoles
                WriteStream($"{(char)27}[5m");
            }
        }

        /// <summary>
        /// enable low intensity
        /// </summary>
        public void EnableLowIntensity()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                // TODO: not available on many consoles
                WriteStream($"{(char)27}[2m");
            }
        }

        /// <summary>
        /// enable underline
        /// </summary>
        public void EnableUnderline()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[4m");
            }
        }

        /// <summary>
        /// enable bold
        /// </summary>
        public void EnableBold()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                // TODO: not available on many consoles
                WriteStream($"{(char)27}[1m");
            }
        }

        /// <summary>
        /// disale text decoration (reset to default)
        /// </summary>
        public void DisableTextDecoration()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[0m");
            }
        }

        /// <summary>
        /// moves cursor down
        /// </summary>
        /// <param name="n">lines count</param>
        public void MoveCursorDown(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[{n}B");
            }
        }

        /// <summary>
        /// moves cursor top
        /// </summary>
        /// <param name="n">lines count</param>
        public void MoveCursorTop(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[{n}A");
            }
        }

        /// <summary>
        /// moves cursor left
        /// </summary>
        /// <param name="n">columns count</param>
        public void MoveCursorLeft(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[{n}D");
            }
        }

        /// <summary>
        /// moves cursor right
        /// </summary>
        /// <param name="n">columns count</param>
        public void MoveCursorRight(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream($"{(char)27}[{n}C");
            }
        }

        /// <summary>
        /// scroll window down
        /// </summary>
        /// <param name="n">lines count</param>
        public void ScrollWindowDown(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream(((char)27) + $"[{n}T");
            }
        }

        /// <summary>
        /// scroll window up
        /// </summary>
        /// <param name="n">lines count</param>
        public void ScrollWindowUp(int n = 1)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream(((char)27) + $"[{n}S");
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
#if !Ignore_BufferedOperationNotAvailableException
                if (IsBufferEnabled)
                    throw new BufferedOperationNotAvailableException();
#endif
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
#if !Ignore_BufferedOperationNotAvailableException
                if (IsBufferEnabled)
                    throw new BufferedOperationNotAvailableException();
#endif
                _backgroundBackup = sc.BackgroundColor;
            }
        }

        /// <summary>
        /// resore backuped foreground color
        /// </summary>
        public void RestoreForeground()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                SetForeground(_foregroundBackup);
            }
        }

        /// <summary>
        /// restore backuped backround color
        /// </summary>
        public void RestoreBackground()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                SetBackground(_backgroundBackup);
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
                WriteStream(s);
            }
        }

        /// <summary>
        /// set 3bit background color from console color
        /// </summary>
        /// <param name="c"></param>
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
                WriteStream(s);
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
                WriteStream($"{(char)27}[38;5;{c}m");
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
                WriteStream($"{(char)27}[48;5;{c}m");
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
                WriteStream($"{(char)27}[38;2;{r};{g};{b}m");
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
                WriteStream($"{(char)27}[48;2;{r};{g};{b}m");
            }
        }

        /// <summary>
        /// set foreground 24bit color from r/g/b
        /// </summary>
        /// <param name="color"></param>
        public void SetForeground((int r, int g, int b) color) => SetForeground(color.r, color.g, color.b);

        /// <summary>
        /// set background 24bit color froms r/g/b
        /// </summary>
        /// <param name="color"></param>
        public void SetBackground((int r, int g, int b) color) => SetBackground(color.r, color.g, color.b);

        /// <summary>
        /// set default foreground color
        /// </summary>
        /// <param name="c">color</param>
        public void SetDefaultForeground(ConsoleColor c)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Console.Settings.DefaultForeground = c;
                sc.ForegroundColor = c;
            }
        }

        /// <summary>
        /// set default background color
        /// </summary>
        /// <param name="c">color</param>
        public void SetDefaultBackground(ConsoleColor c)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                Console.Settings.DefaultBackground = c;
                sc.BackgroundColor = c;
            }
        }

        /// <summary>
        /// set default colors
        /// </summary>
        /// <param name="foregroundColor">foreground color</param>
        /// <param name="backgroundColor">background color</param>
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
                WriteStream(ANSI.RSTXTA);
                SetForeground(Console.Settings.DefaultForeground);
                SetBackground(Console.Settings.DefaultBackground);
                if (Console.Settings.DefaultForeground.HasValue)
                    sc.ForegroundColor = Console.Settings.DefaultForeground.Value;
            }
        }

        /// <summary>
        /// returns restore default colors directive
        /// </summary>
        string _restoreDefaultColorsAnsiSequence
        {
            get
            {
                var r = ANSI.RSTXTA;
                if (Console.Settings.DefaultForeground.HasValue)
                    r += ANSI.Set4BitsColorsForeground(ANSI.To4BitColorNum(Console.Settings.DefaultForeground.Value));
                if (Console.Settings.DefaultBackground.HasValue)
                    r += ANSI.Set4BitsColorsBackground(ANSI.To4BitColorNum(Console.Settings.DefaultBackground.Value));
                if (Console.Settings.DefaultForeground.HasValue)
                    sc.ForegroundColor = Console.Settings.DefaultForeground.Value;
                return r;
            }
        }

        /// <summary>
        /// default colors
        /// </summary>
        public string DefaultColors => ANSI.Set4BitsColors(
            ANSI.To4BitColorNum(Console.Settings.DefaultForeground),
            ANSI.To4BitColorNum(Console.Settings.DefaultBackground));

        /// <summary>
        /// clear screen
        /// </summary>
        /// <exception cref="BufferedOperationNotAvailableException">operation not available in buffered mode</exception>
        public void ClearScreen()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
#if !Ignore_BufferedOperationNotAvailableException
                if (IsBufferEnabled)
                    throw new BufferedOperationNotAvailableException();
#endif
                try
                {
                    WriteLineStream(ANSI.RSTXTA);         // reset text attr

                    System.Threading.Thread.Sleep(10);

                    sc.Write(ANSI.RIS);
                    sc.Clear();
                }
                catch (System.IO.IOException)
                {

                }
            }
        }

        /// <summary>
        /// line break
        /// </summary>
        public void LineBreak()
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                WriteStream(LNBRK);
            }
        }

        /// <summary>
        /// backup cursor pos
        /// </summary>
        public void ConsoleCursorPosBackup()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream(ANSI.DECSC);
            }
        }

        /// <summary>
        /// backup and restore cursor pos
        /// </summary>
        public void ConsoleCursorPosBackupAndRestore()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            ConsoleCursorPosBackup();
            ConsoleCursorPosRestore();
        }

        /// <summary>
        /// restore cursor pos
        /// </summary>
        public void ConsoleCursorPosRestore()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream(ANSI.DECRC);
            }
        }

        /// <summary>
        /// restore cursor pos async
        /// </summary>
        public Task ConsoleCursorPosRestoreAsync()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return Task.CompletedTask;
            lock (Lock!)
            {
                return WriteAsync(ANSI.DECRC);
            }
        }

        /// <summary>
        /// backup the cursor pos
        /// </summary>
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
        /// restore the cursor pos. compat problem on low ansi
        /// </summary>
        public void RestoreCursorPos()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                WriteStream(ESC + "[2J" + ESC + $"[{_cursorTopBackup + 1};{_cursorLeftBackup + 1}H");
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
                    WriteStream(ESC + "[" + (value + 1) + "G");
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
                    WriteStream(ESC + $"[{value + 1};{CursorLeft + 1}H");
                }
            }
        }

        /// <summary>
        /// returns cursor pos as a Point
        /// </summary>
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
                    WriteStream(ESC + $"[{value.Y + 1};{value.X + 1}H");
                }
            }
        }

        /// <summary>
        /// set cursor pos
        /// </summary>
        /// <param name="p">pos as a Point</param>
        public void SetCursorPos(Point p)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                var x = p.X;
                var y = p.Y;
                Console.Cursor.FixCoords(ref x, ref y);
                if (IsBufferEnabled)
                {
                    _cachedCursorPosition.X = x;
                    _cachedCursorPosition.Y = y;
                }
                WriteStream(ESC + $"[{y + 1};{x + 1}H");
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
                Console.Cursor.FixCoords(ref x, ref y);
                if (IsBufferEnabled)
                {
                    _cachedCursorPosition.X = x;
                    _cachedCursorPosition.Y = y;
                }
                WriteStream(ESC + $"[{(y + 1)};{(x + 1)}H");
            }
        }

        /// <summary>
        /// hides the cursor
        /// </summary>
        public void HideCur()
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                sc.CursorVisible = false;
            }
        }

        /// <summary>
        /// shows the cursor
        /// </summary>
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
        /// get text only without print directives and without ansi
        /// </summary>
        /// <param name="s">text to be filtered</param>
        /// <returns>text visible characters only</returns>
        public string GetText(string s)
        {
            var r = GetPrint(s, true, true);
            return r;
        }

        /// <summary>
        /// get text with or without print directives and ansi
        /// </summary>
        /// <param name="s">text to be filtered</param>
        /// <param name="removeANSI">if set remove ansi sequences</param>
        /// <param name="ignorePrintDirectives">if true remove print directives</param>
        /// <param name="lineBreak">if true append a line break to the end</param>
        /// <param name="printSequences">if not null is filled with the parsed sequences</param>
        /// <returns>text visible characters only</returns>
        public string GetPrint(
            string s,
            bool removeANSI = true,
            bool ignorePrintDirectives = true,
            bool lineBreak = false,
            EchoSequenceList? printSequences = null)
        {
            lock (Lock!)
            {
                if (string.IsNullOrWhiteSpace(s))
                    return s;
                var ms = new MemoryStream(s.Length * 4);
                var sw = new StreamWriter(ms);
                Console.RedirectOut(sw);
                var e = Console.WorkAreaSettings.EnableConstraintConsolePrintInsideWorkArea;
                Console.WorkAreaSettings.EnableConstraintConsolePrintInsideWorkArea = false;

                if (removeANSI)
                    s = ANSI.GetText(s);    // also removed ansi sequences

                if (ignorePrintDirectives)
                {
                    // directives are removed                    
                    WriteInternal(s, lineBreak, true, true, printSequences);
                }
                else
                {
                    // directives are keeped
                    WriteInternal(s, lineBreak, false, true, printSequences, false, false);
                }
                Console.WorkAreaSettings.EnableConstraintConsolePrintInsideWorkArea = e;

                sw.Flush();
                ms.Position = 0;

                var rw = new StreamReader(ms);
                var txt = rw.ReadToEnd();
                rw.Close();
                Console.RedirectOut((StreamWriter?)null);
                return txt;
            }
        }

        /// <summary>
        /// get raw text - substitue non printables characters by their representation
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="escapableOnly">just handle known escapable characters</param>
        /// <param name="useHexa">unix hexa notation</param>
        /// <returns></returns>
        public string GetRawText(
            string text,
            bool escapableOnly = true,
            bool useHexa = true)
        {
            var s = GetPrint(text, false);
            s = ASCII.GetNonPrintablesCodesAsLabel(s, false, escapableOnly);
            if (useHexa)
                s = ANSI.AvoidANSISequencesAndNonPrintableCharacters(s);
            return s;
        }

        /// <summary>
        /// get the output of a write operation
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="lineBreak">line break</param>
        /// <returns>text</returns>
        public string GetPrint(string text, bool lineBreak = false)
        {
            lock (Lock!)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return text;
                var ms = new MemoryStream(text.Length * 4);
                var sw = new StreamWriter(ms);
                Console.RedirectOut(sw);
                var e = Console.WorkAreaSettings.EnableConstraintConsolePrintInsideWorkArea;
                Console.WorkAreaSettings.EnableConstraintConsolePrintInsideWorkArea = false;
                Write(text, lineBreak);
                Console.WorkAreaSettings.EnableConstraintConsolePrintInsideWorkArea = e;
                sw.Flush();
                ms.Position = 0;
                var rw = new StreamReader(ms);
                var txt = rw.ReadToEnd();
                rw.Close();
                Console.RedirectOut((StreamWriter?)null);
                return txt;
            }
        }

        /// <summary>
        /// write a text to the console
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="lineBreak">line break</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ConsolePrint(string text, bool lineBreak = false)
        {
            if (IsMute)
                return;
            // any print goes here...
            lock (Lock!)
            {
                if (CropX == -1)
                {
                    ConsoleSubPrint(text, lineBreak);
                }
                else
                {
                    var x = CursorLeft;
                    var mx = Math.Max(x, CropX);
                    if (mx > x)
                    {
                        var n = mx - x + 1;
                        if (text.Length <= n)
                            ConsoleSubPrint(text, lineBreak);
                        else
                            ConsoleSubPrint(text[..n], lineBreak);
                    }
                }
            }
        }

        /// <summary>
        /// debug output traces to a file
        /// </summary>
        /// <param name="s">text</param>
        /// <param name="lineBreak">line break</param>
        /// <param name="callerMemberName">caller member name</param>
        /// <param name="callerLineNumber">caller line number</param>
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
        /// debug output traces to a System.Diagnostics.Debug
        /// </summary>
        /// <param name="s">text</param>
        /// <param name="lineBreak">line break</param>
        /// <param name="callerFilePath">caller file path</param>
        /// <param name="callerMemberName">caller member name</param>
        /// <param name="callerLineNumber">caller line number</param>
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

        /// <inheritdoc/>
        public override void WriteStream(string s)
        {
            if (RedirectToErr)
            {
                if (IsReplicationEnabled)
                    ReplicateStreamWriter!.Write(s);

                Console.StdErr.WriteStream(s);
            }
            else
            {
                if (IsNotMute)
                    base.WriteStream(s);
            }
        }

        /// <summary>
        /// write a line
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="ignorePrintDirectives">ignore markup</param>
        public void WriteLine(string text = "", bool ignorePrintDirectives = false) => WriteInternal(text, true, !ignorePrintDirectives);

        /// <summary>
        /// write a line
        /// </summary>
        /// <param name="obj">object</param>
        /// <param name="ignorePrintDirectives">ignore markup</param>
        public void WriteLine(object obj, bool ignorePrintDirectives = false) => WriteInternal(obj, true, !ignorePrintDirectives);

        /// <summary>
        /// write
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="ignorePrintDirectives">ignore markup</param>
        public void Write(
            string text = "",
            bool ignorePrintDirectives = false) => WriteInternal(text, false, !ignorePrintDirectives);

        /// <summary>
        /// write
        /// </summary>
        /// <param name="character">character</param>
        /// <param name="ignorePrintDirectives">ignore markup</param>
        public void WriteLine(char character, bool ignorePrintDirectives = false) => WriteInternal(character + "", true, !ignorePrintDirectives);

        /// <summary>
        /// write
        /// </summary>
        /// <param name="character">character</param>
        /// <param name="ignorePrintDirectives">ignore markup</param>
        public void Write(char character, bool ignorePrintDirectives = false) => WriteInternal(character + "", false, !ignorePrintDirectives);

        /// <summary>
        /// write
        /// </summary>
        /// <param name="obj">object</param>
        public void Write(object obj) => WriteInternal(obj, false);

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
        void WriteInternal(
            object o,
            bool lineBreak = false,
            bool parseCommands = true,
            bool doNotEvalutatePrintDirectives = false,         // TODO: explain this
            EchoSequenceList? printSequences = null,
            bool avoidANSISequencesAndNonPrintableCharacters = true,
            bool getNonPrintablesASCIICodesAsLabel = true
            )
        {
            doNotEvalutatePrintDirectives |= Console.Settings.IsMarkupDisabled;
            parseCommands &= !Console.Settings.IsRawOutputEnabled;
            getNonPrintablesASCIICodesAsLabel &= !(Console.Settings.IsRawOutputEnabled
                && !Console.Settings.ReplaceNonPrintableCharactersByTheirName);

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
                    var txt = o.ToString()!;
                    if (Console.Settings.RemoveANSISequences)
                        txt = ANSIParser.GetText(txt);

                    if (parseCommands)
                    {
                        // call the EchoDirective component
                        _echoDirectiveProcessor.ParseTextAndApplyCommands(
                            txt,
                            false,
                            "",
                            doNotEvalutatePrintDirectives,
                            printSequences);
                    }
                    else
                    {
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

        /// <summary>
        /// write a line in a warning style
        /// </summary>
        /// <param name="s">text</param>
        public void Warningln(string s) => Warning(s, true);

        /// <summary>
        /// write a text in a warning style
        /// </summary>
        /// <param name="s"></param>
        /// <param name="lineBreak"></param>
        public void Warning(string s, bool lineBreak = true)
        {
            if (IsNotMute)
                Console.Out.Write($"{Console.Colors.Warning}{s}{Console.Colors.Default}", lineBreak);
        }

        /// <summary>
        /// write a text in an error style
        /// </summary>
        /// <param name="s">text</param>
        public void Errorln(string s) => Error(s, true);

        /// <summary>
        /// write a text in an error style
        /// </summary>
        /// <param name="s">text</param>
        /// <param name="lineBreak">line break</param>
        public void Error(string s, bool lineBreak = true)
        {
            if (IsNotMute)
            {
                RedirectToErr = true;
                Console.Out.Write($"{Console.Colors.Error}{s}{Console.Colors.Default}", lineBreak);
                RedirectToErr = false;
            }
            else
            {
                Console.StdErr.WriteStream(s);
                if (lineBreak)
                    Console.StdErr.WriteLineStream(string.Empty);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ConsoleSubPrint(string s, bool lineBreak = false)
        {
            if (IsMute)
                return;
            lock (Lock!)
            {
                if (Console.WorkAreaSettings.EnableConstraintConsolePrintInsideWorkArea)
                {
                    var (id, x, y, w, h) = Console.WorkArea.ActualWorkArea();
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
                            WriteStream(line);
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
                        WriteStream(s);
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
                    WriteStream(s);
                    EchoDebug(s);

                    if (lineBreak)
                    {
                        WriteLineStream(string.Empty);
                        EchoDebug(string.Empty, true);
                    }
                }
            }
        }

        void FillLineFromCursor(char c = ' ', bool resetCursorLeft = true, bool useDefaultColors = true)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                if (!EnableFillLineFromCursor)
                    return;
                var f = _cachedForegroundColor;
                var b = _cachedForegroundColor;
                var aw = Console.WorkArea.ActualWorkArea();
                var nb = Math.Max(0, Math.Max(aw.Right, _cachedBufferSize.Width - 1) - CursorLeft - 1);
                var x = CursorLeft;
                var y = CursorTop;
                if (useDefaultColors)
                {
                    SetForeground(ColorSettings.Default.Foreground);
                    SetBackground(ColorSettings.Default.Background);
                }
                WriteStream("".PadLeft(nb, c));   // TODO: BUG in WINDOWS: do not print the last character
                SetCursorPos(nb, y);
                WriteStream(" ");
                if (useDefaultColors)
                {
                    SetForeground(f);
                    SetBackground(b);
                }
                if (resetCursorLeft)
                    CursorLeft = x;
            }
        }

        /// <summary>
        /// compute the index inside a work area of a string
        /// </summary>
        /// <param name="s">string</param>
        /// <param name="origin">origin</param>
        /// <param name="cursorPos">cursor position</param>
        /// <param name="forceEnableConstraintInWorkArea">force or not the constraint</param>
        /// <param name="fitToVisibleArea">fit to visible area</param>
        /// <param name="doNotEvaluatePrintDirectives">preserve markup</param>
        /// <param name="ignorePrintDirectives">ignore markup</param>
        /// <returns></returns>
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

        /// <summary>
        /// compute the index inside a work area of a string from a position
        /// </summary>
        /// <param name="s">string</param>
        /// <param name="origin">porigin</param>
        /// <param name="cursorX">cursor x</param>
        /// <param name="cursorY">cursor y</param>
        /// <param name="forceEnableConstraintInWorkArea">force or not the constraint</param>
        /// <param name="fitToVisibleArea">fit to visible area</param>
        /// <param name="doNotEvaluatePrintDirectives">preserve markup</param>
        /// <param name="ignorePrintDirectives">ignore markup</param>
        /// <returns></returns>
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

        LineSplitList GetIndexLineSplitsInWorkAreaConstraintedString(
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
        LineSplitList GetWorkAreaStringSplits(
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
                var (id, x, y, w, h) = Console.WorkArea.ActualWorkArea(fitToVisibleArea);
                var x0 = origin.X;
                var y0 = origin.Y;

                var croppedLines = new List<StringSegment>();
                string? pds = null;
                var length = s.Length;
                if (doNotEvaluatePrintDirectives)
                {
                    pds = s;
                    printSequences = new EchoSequenceList();
                    s = GetPrint(s, !ignorePrintDirectives, !ignorePrintDirectives, false, printSequences);
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

        /// <summary>
        /// set the cursor position constrainted to the work area
        /// </summary>
        /// <param name="pos">new cursor position</param>
        /// <param name="enableOutput">enable output</param>
        /// <param name="forceEnableConstraintInWorkArea">force constraint</param>
        /// <param name="fitToVisibleArea">fir to visible area</param>
        public void SetCursorPosConstraintedInWorkArea(
            Point pos,
            bool enableOutput = true,
            bool forceEnableConstraintInWorkArea = false,
            bool fitToVisibleArea = true)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            var x = pos.X;
            var y = pos.Y;
            SetCursorPosConstraintedInWorkArea(ref x, ref y, enableOutput, forceEnableConstraintInWorkArea, fitToVisibleArea);
        }

        /// <summary>
        /// set the cursor position constrainted to the work area
        /// </summary>
        /// <param name="cx">cursor x</param>
        /// <param name="cy">cursor y</param>
        /// <param name="enableOutput">enable output</param>
        /// <param name="forceEnableConstraintInWorkArea">force constraint</param>
        /// <param name="fitToVisibleArea">fir to visible area</param>
        public void SetCursorPosConstraintedInWorkArea(
            int cx,
            int cy,
            bool enableOutput = true,
            bool forceEnableConstraintInWorkArea = false,
            bool fitToVisibleArea = true)
            => SetCursorPosConstraintedInWorkArea(ref cx, ref cy, enableOutput, forceEnableConstraintInWorkArea, fitToVisibleArea);

        /// <summary>
        /// set the cursor position constrainted to the work area
        /// </summary>
        /// <param name="cx">cursor x</param>
        /// <param name="cy">cursor y</param>
        /// <param name="enableOutput">enable output</param>
        /// <param name="forceEnableConstraintInWorkArea">force constraint</param>
        /// <param name="fitToVisibleArea">fir to visible area</param>
        public void SetCursorPosConstraintedInWorkArea(ref int cx, ref int cy, bool enableOutput = true, bool forceEnableConstraintInWorkArea = false, bool fitToVisibleArea = true)
        {
            if (IsMuteOrIsNotConsoleGeometryEnabled)
                return;
            lock (Lock!)
            {
                var dx = 0;
                var dy = 0;

                if (Console.WorkAreaSettings.EnableConstraintConsolePrintInsideWorkArea || forceEnableConstraintInWorkArea)
                {
                    var (id, left, top, right, bottom) = Console.WorkArea.ActualWorkArea(fitToVisibleArea);
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
                                Console.Settings.DefaultForeground ?? ConsoleColor.White,
                                Console.Settings.DefaultBackground ?? ConsoleColor.Black);
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
                                Console.Settings.DefaultForeground ?? ConsoleColor.White,
                                Console.Settings.DefaultBackground ?? ConsoleColor.Black);
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme
                        }
                    }
                }

                if (enableOutput)
                {
                    SetCursorPos(cx, cy);
                    if (dx != 0 || dy != 0)
                        Console.WorkAreaSettings.WorkAreaScrolled?.Invoke(null, new WorkAreaScrollEventArgs(0, dy));
                }
            }
        }

        #endregion

    }
}
