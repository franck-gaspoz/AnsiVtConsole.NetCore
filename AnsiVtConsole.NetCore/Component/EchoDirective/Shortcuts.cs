#pragma warning disable CS1591

namespace AnsiVtConsole.NetCore.Component.EchoDirective
{
    /// <summary>
    /// shortcuts to echo directives commands
    /// <para>
    /// naming conventions:<br/>
    /// &#9889; echo directive commands shortcuts are CamelCase<br/>
    /// </para>
    /// </summary>
    public static class Shortcuts
    {
        public static IAnsiVtConsole? Console { get; set; }

        public static void Initialize(IAnsiVtConsole console) => Console = console;

        public static string GetCmd(string cmd, string? value = null)
        {
            if (value != null)
                return $"{Console!.Settings.CommandBlockBeginChar}{cmd}{Console!.Settings.CommandValueAssignationChar}{value}{Console!.Settings.CommandBlockEndChar}";
            return $"{Console!.Settings.CommandBlockBeginChar}{cmd}{Console!.Settings.CommandBlockEndChar}";
        }

        public static string GetCmd(EchoDirectives cmd, string? value = null)
        {
            if (value != null)
                return $"{Console!.Settings.CommandBlockBeginChar}{cmd}{Console!.Settings.CommandValueAssignationChar}{value}{Console!.Settings.CommandBlockEndChar}";
            return $"{Console!.Settings.CommandBlockBeginChar}{cmd}{Console!.Settings.CommandBlockEndChar}";
        }

        #region commands shortcuts

        /// <summary>
        /// clear line from cursor left - @[1K
        /// </summary>
        public static string Clleft => GetCmd(EchoDirectives.clleft);

        /// <summary>
        /// clear line from cursor right - @[K
        /// </summary>
        public static string Clright => GetCmd(EchoDirectives.clright);

        /// <summary>
        /// fill current line to the right - shell impl. (@TODO: remove)
        /// </summary>
        public static string Fillright => GetCmd(EchoDirectives.fillright);

        /// <summary>
        /// clear entire line - @[2K
        /// </summary>
        public static string Cl => GetCmd(EchoDirectives.cl);

        /// <summary>
        /// cursor home (top left 0,0) - @[H
        /// </summary>
        public static string Chome => GetCmd(EchoDirectives.chome);

        /// <summary>
        /// light colors - sgr
        /// </summary>
        public static string Lion => GetCmd(EchoDirectives.lion);

        /// <summary>
        /// bold on - sgr
        /// </summary>
        public static string Bon => GetCmd(EchoDirectives.bon);

        /// <summary>
        /// blink on - sgr
        /// </summary>
        public static string Blon => GetCmd(EchoDirectives.blon);

        /// <summary>
        /// cursor left - @[1D
        /// </summary>
        public static string Cleft => GetCmd(EchoDirectives.cleft);

        /// <summary>
        /// cursor right- @[1C
        /// </summary>
        public static string Cright => GetCmd(EchoDirectives.cright);

        /// <summary>
        /// cursor up - @[1A
        /// </summary>
        public static string Cup => GetCmd(EchoDirectives.cup);

        /// <summary>
        /// cursor down - @[1B
        /// </summary>
        public static string Cdown => GetCmd(EchoDirectives.cdown);

        /// <summary>
        /// cursor n cells left - @[{n}D
        /// </summary>
        /// <param name="n">nb cells</param>
        public static string Cnleft(int n) => GetCmd(EchoDirectives.cleft + "", n + "");

        /// <summary>
        /// cursor n cells right- @[{n}C
        /// </summary>
        /// <param name="n">nb cells</param>
        public static string Cnright(int n) => GetCmd(EchoDirectives.cright + "", n + "");

        /// <summary>
        /// cursor n lines up - @[1{n}A 
        /// </summary>
        /// <param name="n">nb cells</param>
        public static string Cnup(int n) => GetCmd(EchoDirectives.cup + "", n + "");

        /// <summary>
        /// cursor n lines down - @[{n}B
        /// </summary>
        /// <param name="n">nb cells</param>
        public static string Cndown(int n) => GetCmd(EchoDirectives.cdown + "", n + "");

        /// <summary>
        /// invert/reverse on - sgr
        /// </summary>
        public static string Invon => GetCmd(EchoDirectives.invon);

        /// <summary>
        /// underline on - sgr
        /// </summary>
        public static string Uon => GetCmd(EchoDirectives.uon);

        /// <summary>
        /// text decoration off - sgr
        /// </summary>
        public static string Tdoff => GetCmd(EchoDirectives.tdoff);

        /// <summary>
        /// setup default background color
        /// </summary>
        public static string DefaultBackgroundCmd => GetCmd(EchoDirectives.b + "", Console!.Settings.DefaultBackground!.Value.ToString().ToLower());

        /// <summary>
        /// setup default foreground color
        /// </summary>
        public static string DefaultForegroundCmd => GetCmd(EchoDirectives.f + "", Console!.Settings.DefaultForeground!.Value.ToString().ToLower());

        /// <summary>
        /// set colors to defaults from shell  default foreground and background colors
        /// </summary>
        public static string Rdc => GetCmd(EchoDirectives.rdc);

        /// <summary>
        /// background black
        /// </summary>
        public static string Bblack => GetCmd(EchoDirectives.b + "", "black");

        /// <summary>
        /// background darkblue
        /// </summary>
        public static string Bdarkblue => GetCmd(EchoDirectives.b, "darkblue");

        /// <summary>
        /// background darkgreen
        /// </summary>
        public static string Bdarkgreen => GetCmd(EchoDirectives.b, "darkgreen");

        /// <summary>
        /// background darkcyan
        /// </summary>
        public static string Bdarkcyan => GetCmd(EchoDirectives.b, "darkcyan");

        /// <summary>
        /// background darkred
        /// </summary>
        public static string Bdarkred => GetCmd(EchoDirectives.b, "darkred");

        /// <summary>
        /// background darkmagenta
        /// </summary>
        public static string Bdarkmagenta => GetCmd(EchoDirectives.b, "darkmagenta");

        /// <summary>
        /// background darkyellow
        /// </summary>
        public static string Bdarkyellow => GetCmd(EchoDirectives.b, "darkyellow");

        /// <summary>
        /// background gray
        /// </summary>
        public static string Bgray => GetCmd(EchoDirectives.b, "gray");

        /// <summary>
        /// background darkgray
        /// </summary>
        public static string Bdarkgray => GetCmd(EchoDirectives.b, "darkgray");

        /// <summary>
        /// background blue
        /// </summary>
        public static string Bblue => GetCmd(EchoDirectives.b, "blue");

        /// <summary>
        /// background green
        /// </summary>
        public static string Bgreen => GetCmd(EchoDirectives.b, "green");

        /// <summary>
        /// background cyan
        /// </summary>
        public static string Bcyan => GetCmd(EchoDirectives.b, "cyan");

        /// <summary>
        /// background red
        /// </summary>
        public static string Bred => GetCmd(EchoDirectives.b, "red");

        /// <summary>
        /// background magenta
        /// </summary>
        public static string Bmagenta => GetCmd(EchoDirectives.b, "magenta");

        /// <summary>
        /// background yellow
        /// </summary>
        public static string Byellow => GetCmd(EchoDirectives.b, "yellow");

        /// <summary>
        /// background white
        /// </summary>
        public static string Bwhite => GetCmd(EchoDirectives.b, "white");

        /// <summary>
        /// foreground Black
        /// </summary>
        public static string Black => GetCmd(EchoDirectives.f, "black");

        /// <summary>
        /// foreground Darkblue
        /// </summary>
        public static string Darkblue => GetCmd(EchoDirectives.f, "darkblue");

        /// <summary>
        /// foreground Darkgreen
        /// </summary>
        public static string Darkgreen => GetCmd(EchoDirectives.f, "darkgreen");

        /// <summary>
        /// foreground Darkcyan
        /// </summary>
        public static string Darkcyan => GetCmd(EchoDirectives.f, "darkcyan");

        /// <summary>
        /// foreground Darkred
        /// </summary>
        public static string Darkred => GetCmd(EchoDirectives.f, "darkred");

        /// <summary>
        /// foreground Darkmagenta
        /// </summary>
        public static string Darkmagenta => GetCmd(EchoDirectives.f, "darkmagenta");

        /// <summary>
        /// foreground Darkyellow
        /// </summary>
        public static string Darkyellow => GetCmd(EchoDirectives.f, "darkyellow");

        /// <summary>
        /// foreground Gray
        /// </summary>
        public static string Gray => GetCmd(EchoDirectives.f, "gray");

        /// <summary>
        /// foreground Darkgray
        /// </summary>
        public static string Darkgray => GetCmd(EchoDirectives.f, "darkgray");

        /// <summary>
        /// foreground Blue
        /// </summary>
        public static string Blue => GetCmd(EchoDirectives.f, "blue");

        /// <summary>
        /// foreground Green
        /// </summary>
        public static string Green => GetCmd(EchoDirectives.f, "green");

        /// <summary>
        /// foreground Cyan
        /// </summary>
        public static string Cyan => GetCmd(EchoDirectives.f, "cyan");

        /// <summary>
        /// foreground Red
        /// </summary>
        public static string Red => GetCmd(EchoDirectives.f, "red");

        /// <summary>
        /// foreground Magenta
        /// </summary>
        public static string Magenta => GetCmd(EchoDirectives.f, "magenta");

        /// <summary>
        /// foreground Yellow
        /// </summary>
        public static string Yellow => GetCmd(EchoDirectives.f, "yellow");

        /// <summary>
        /// foreground White
        /// </summary>
        public static string White => GetCmd(EchoDirectives.f, "white");

        /// <summary>
        /// backup foreground in the shell backup memory
        /// </summary>
        public static string Bkf => GetCmd(EchoDirectives.bkf);

        /// <summary>
        /// restore foreground from the shell default foreground setting
        /// </summary>
        public static string Rsf => GetCmd(EchoDirectives.rsf);

        /// <summary>
        /// backup background in the shell backup memory
        /// </summary>
        public static string Bkb => GetCmd(EchoDirectives.bkb);

        /// <summary>
        /// restore background from shell default background setting
        /// </summary>
        public static string Rsb => GetCmd(EchoDirectives.rsb);

        /// <summary>
        /// clear screen - @Uses ConsoleTextWriterWrapper
        /// </summary>
        public static string Cls => GetCmd(EchoDirectives.cls);

        /// <summary>
        /// line break - @Uses ConsoleTextWrapper.LineBreak
        /// </summary>
        public static string Br => GetCmd(EchoDirectives.br);

        /// <summary>
        /// set background from 4 bits palette  : b=red (from ConsoleColor not case sensitive values)
        /// </summary>
        /// <param name="c">console color</param>
        public static string B(ConsoleColor c) => GetCmd(EchoDirectives.b, c + "");

        /// <summary>
        /// set background from 8 bits palette : b8=0&lt;=n&lt;=255
        /// </summary>
        /// <param name="c">console color</param>
        public static string B8(ConsoleColor c) => GetCmd(EchoDirectives.b8, c + "");

        /// <summary>
        /// set background from 24 bits color : b24=r:g:b with 0&lt;=r,g,b&lt;=255
        /// </summary>
        /// <param name="c">console color</param>
        public static string B24(ConsoleColor c) => GetCmd(EchoDirectives.b24, c + "");

        /// <summary>
        /// set foreground from 4 bits palette : f=red (from ConsoleColor not case sensitive values)
        /// </summary>
        /// <param name="c">console color</param>
        public static string F(ConsoleColor c) => GetCmd(EchoDirectives.f, c + "");

        /// <summary>
        /// set foreground from 8 bits palette : f8=0&lt;=n&lt;=255 
        /// </summary>
        /// <param name="c">console color</param>
        public static string F8(ConsoleColor c) => GetCmd(EchoDirectives.f8, c + "");

        /// <summary>
        /// set foreground from 24 bits color : f24=r:g:b with 0&lt;=r,g,b&lt;=255
        /// </summary>
        /// <param name="c">console color</param>
        public static string F24(ConsoleColor c) => GetCmd(EchoDirectives.f24, c + "");

        /// <summary>
        /// backup cursor pos in shell memory - @Uses system console cursor position
        /// </summary>
        public static string Bkcr => GetCmd(EchoDirectives.bkcr);

        /// <summary>
        /// restore cursor pos from shell memory - @Uses @2J@[{top+1};{left+1})H
        /// </summary>
        public static string Rscr => GetCmd(EchoDirectives.rscr);

        /// <summary>
        /// set cursor left (x) - @Uses @[{x+1}G (get @uses system console)
        /// </summary>
        /// <param name="x">x</param>
        public static string Crx(int x) => GetCmd(EchoDirectives.crx, x + "");

        /// <summary>
        /// set cursor top (y) - @Uses @2J@[{top+1};{left+1})H (get @uses system console)
        /// </summary>
        /// <param name="y">y</param>
        public static string Cry(int y) => GetCmd(EchoDirectives.cry, y + "");

        /// <summary>
        /// set cursor left (x) - @Uses @[{x+1}G (get @uses system console) + set cursor top (y) - @Uses @2J@[{top+1};{left+1})H (get @uses system console)
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        public static string Cr(int x, int y) => $"{GetCmd(EchoDirectives.crx, x + "")}{GetCmd(EchoDirectives.cry, y + "")}";

        /// <summary>
        /// exec csharp code from text
        /// </summary>
        /// <param name="csharpText">csharp text</param>
        /// <returns></returns>
        public static string Exec(string csharpText) => GetCmd(EchoDirectives.exec, csharpText);

        /// <summary>
        /// tab
        /// </summary>
        public static string Tab => "".PadLeft(Console!.Settings.TabLength, ' ');

        #endregion
    }
}