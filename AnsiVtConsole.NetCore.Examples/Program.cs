//#define Enable_Buffer
using System.Reflection;
using System.Text;

using AnsiVtConsole.NetCore;
using AnsiVtConsole.NetCore.Component.Console;
using AnsiVtConsole.NetCore.Component.Parser.ANSI;
using AnsiVtConsole.NetCore.Component.Widgets.Animatics;
using AnsiVtConsole.NetCore.Component.Widgets.Animatics.Animations;
using AnsiVtConsole.NetCore.Component.Widgets.Text.Raimbow;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;
using static AnsiVtConsole.NetCore.Component.Console.Unicode;
using static AnsiVtConsole.NetCore.Component.EchoDirective.Shortcuts;

var console = new AnsiVtConsole.NetCore.AnsiVtConsole();

#if Enable_Buffer
console.Out.EnableBuffer();
#endif

Title(console);

console.Out.WriteLine("(uon,bon)init:(br)");

console.Out.WriteLine("(f=blue)var (f=cyan)console (f=darkgray)= (f=blue)new (f=darkgray)AnsiVtConsole.NetCore.(f=green)AnsiVtConsole(f=white)();(br)");

console.Out.WriteLine("(uon,bon)console.Infos():(br)");
console.Infos();

console.Out.WriteLine("(br,uon,bon)console.Out.Echo(\"a string with echo directives tokens\"):(br)");
console.Out.Write("(f=blue)console(f=darkgray).(f=yellow)Out(f=darkgray).(f=green)Echo(f=white)(");
console.Out.Write("\"(b=red,f=yellow,uon)Hello, World!\"", true);
console.Out.WriteLine("(f=white))");
console.Out.WriteLine("(br,b=red,f=yellow,uon)Hello, World!");

console.Out.WriteLine("(br,uon,bon)console.Logger,console.Warn,console.Err:(br)");
console.Logger.Log("console.Logger.Log(\"log\")");
console.Logger.LogWarning("console.Logger.LogWarning(\"warning\")");
console.Logger.LogError("console.Logger.LogError(\"error\")");
console.Logger.LogError(
    new ArgumentException("bad argument exception"),
    "console.Logger.LogError(exception):");

console.Out.WriteLine("(br,uon,bon)ansi vt methods (ANSI,Shortcuts) - colors test:(br)");

AnsiColorTest(console);

console.Out.WriteLine();

console.Out.WriteLine("(br,uon,bon)echo directives:(br)");

const string PrintDocText =
@"(rdc)text can contains (uon)echo directives(tdoff) that changes the echo behavior. 
the echo directive syntax is formed according to these pattern:

(f=darkyellow)(printDirective) or (printDirective=printDirectiveValue)(rdc)

- multiple echo directives can be separated by a (f=darkyellow),(rdc) that can be grouped in a single text in parentheses: (f=darkyellow)(echoDirective1,echoDirective2=..,echoDirective3)(rdc)
- an echo directive value can be written inside a 'code' text block, depending on each echo directive, with the syntax: (f=darkyellow)[[...]](rdc)
- symbols of this grammar can be configured throught the class (uon,bon)AnsiVtConsole.NetCore.Component.Settings(tdoff)

current print directives are:

    (1) (uon)colorization:(tdoff)

    (f=yellow)f=(f=darkyellow)ConsoleColor(rdc)      : set foreground color
    (f=yellow)f8=(f=darkyellow)Int32(rdc)            : set foreground 8bit color index, where 0 <= index <= 255 
    (f=yellow)f24=(f=darkyellow)Int32:Int32:Int32(rdc) : set foreground 24bit color red:green:blue, where 0 <= red,green,blue <= 255 
    (f=yellow)f=(f=darkyellow)ConsoleColor(rdc)      : set foreground color
    (f=yellow)b=(f=darkyellow)ConsoleColor(rdc)      : set background color
    (f=yellow)b8=(f=darkyellow)Int32(rdc)            : set background 8bit color index, where 0 <= index <= 255
    (f=yellow)b24=(f=darkyellow)Int32:Int32:Int32(rdc) : set background 24bit color red:green:blue, where 0 <= red,green,blue <= 255 
    (f=yellow)df=(f=darkyellow)ConsoleColor(rdc)     : set default foreground
    (f=yellow)db=(f=darkyellow)ConsoleColor(rdc)     : set default background
    (f=yellow)bkf(rdc)                 : backup foreground color
    (f=yellow)bkb(rdc)                 : backup background color
    (f=yellow)rsf(rdc)                 : restore foreground color
    (f=yellow)rsb(rdc)                 : restore background color
    (f=yellow)rdc(rdc)                 : restore default colors
    
    (2) (uon)text decoration (vt100):(tdoff)

    (f=yellow)uon(rdc)                 : underline on
    (f=yellow)invon(rdc)               : inverted colors on
    (f=yellow)tdoff(rdc)               : text decoration off and reset default colors
    (f=yellow)lion(rdc)                : ligt colors
    (f=yellow)bon(rdc)                 : bold on
    (f=yellow)blon(rdc)                : blink on (not supported on Windows)

    (3) (uon)echo flow control:(tdoff)

    (f=yellow)cls(rdc)                 : clear screen
    (f=yellow)br(rdc)                  : jump begin of next line (line break)   
    (f=yellow)bkcr(rdc)                : backup cursor position
    (f=yellow)rscr(rdc)                : restore cursor position
    (f=yellow)crx=(f=darkyellow)Int32(rdc)           : set cursor x ((f=cyan)0<=x<=WindowWidth(rdc))
    (f=yellow)cry=(f=darkyellow)Int32(rdc)           : set cursor y ((f=cyan)0<=y<=WindowHeight(rdc))
    (f=yellow)cleft(rdc)               : move cursor left
    (f=yellow)cright(rdc)              : move cursor right
    (f=yellow)cup(rdc)                 : move cursor up
    (f=yellow)cdown(rdc)               : move cursor down
    (f=yellow)cnleft=(f=darkyellow)Int32(rdc)        : move cursor n characters left
    (f=yellow)cnright=(f=darkyellow)Int32(rdc)       : move cursor n characters right
    (f=yellow)cnup=(f=darkyellow)Int32(rdc)          : move cursor n lines up
    (f=yellow)cndown=(f=darkyellow)Int32(rdc)        : move cursor n lines down
    (f=yellow)cl(rdc)                  : clear line
    (f=yellow)clleft(rdc)              : clear line from cursor left
    (f=yellow)clright(rdc)             : clear line from cursor right
    (f=yellow)chome(rdc)               : move cursor to upper left corner
    (f=yellow)tab(rdc)                 : add a tab

    (4) (uon)script engine:(tdoff)

    (f=yellow)exec=(f=darkyellow)CodeBlock|[[CodeBlock]](rdc) : executes and echo result of a C# code block

    (5) (uon)application control:(tdoff)

    (f=yellow)exit(rdc)                : exit the current process

    (f=darkyellow)ConsoleColor := darkblue|darkgreen|darkcyan|darkred|darkmagenta|darkyellow|gray|darkgray|blue|green|cyan|red|magenta|yellow|white(rdc) (not case sensitive)
";

console.Out.WriteLine(PrintDocText);

console.Out.WriteLine();
console.Out.WriteLine("(br,uon,bon)console markup and Unicode:(br)");

console.Out.WriteLine($"it's the (bkf,f=red){Demi}(rsf) or the (bkf,f=yellow){Quar}(rsf) of it");

console.Out.WriteLine("(f=cyan,EdgeTopLeft,BarHorizontal,EdgeTopRight)");
console.Out.WriteLine("(f=cyan,BarVertical,f=blue,Box,f=cyan,BarVertical)");
console.Out.WriteLine("(f=cyan,EdgeBottomLeft,BarHorizontal,EdgeBottomRight)");

console.Out.WriteLine("(br,uon,bon)console markup and ANSI parsing settings:(br)");

var text = "(f=red,b=yellow,uon)a text written in red with a yellow foreground(f=white,b=black,tdoff)";
var text2 = $"{SGR_Underline}an underlined text defined with escaped characters (without markup){SGR_Reset}";

var ascii = new StringBuilder();
ascii.Append(ASCII.FF);
ascii.Append(ASCII.LF);
ascii.Append(ASCII.ACK);
ascii.Append(ASCII.BEL);
ascii.Append(ASCII.CR);
ascii.Append(ASCII.DC1);

console.Out.WriteLine(text);
console.Out.WriteLine(text2);
console.Out.WriteLine();
console.Out.WriteLine("(f=green)console.Settings.IsMarkupDisabled = true;");
console.Out.WriteLine();
console.Settings.IsMarkupDisabled = true;
console.Out.WriteLine(text);
console.Out.WriteLine(text2);

console.Settings.IsMarkupDisabled = false;
console.Out.WriteLine();
console.Out.WriteLine("(f=green)console.Settings.IsRawOutputEnabled = true;");
console.Out.WriteLine();
console.Settings.IsMarkupDisabled = false;
console.Settings.IsRawOutputEnabled = true;
console.Out.WriteLine(text);
console.Out.WriteLine(text2);

console.Settings.IsMarkupDisabled = false;
console.Settings.IsRawOutputEnabled = false;
console.Out.WriteLine();
console.Out.WriteLine("(f=green)console.Settings.IsRawOutputEnabled = true;");
console.Out.WriteLine("(f=green)console.Settings.ReplaceNonPrintableCharactersByTheirName = false;");
console.Out.WriteLine();
console.Settings.IsRawOutputEnabled = true;
console.Settings.ReplaceNonPrintableCharactersByTheirName = false;
console.Out.WriteLine(text);
console.Out.WriteLine(text2);

console.Settings.ReplaceNonPrintableCharactersByTheirName = true;
console.Settings.IsRawOutputEnabled = console.Settings.IsMarkupDisabled = false;

console.Out.WriteLine();
console.Out.WriteLine("(f=green)console.Out.GetText(str)");
console.Out.WriteLine();
console.Out.WriteLine(console.Out.GetText(text));
console.Out.WriteLine(console.Out.GetText(text2));

console.Out.WriteLine();
console.Out.WriteLine("(f=green)console.Out.GetRawText(str,false)");
console.Out.WriteLine();

console.Out.WriteLine(console.Out.GetRawText(text + ascii));
console.Out.WriteLine(console.Out.GetRawText(text2 + ascii));
console.Out.WriteLine(console.Out.GetRawText(text + ascii, false));
console.Out.WriteLine(console.Out.GetRawText(text2 + ascii, false));

console.Out.WriteLine();
console.Out.WriteLine("(f=green)console.Settings.RemoveANSISequences = true;");
console.Out.WriteLine();
console.Settings.RemoveANSISequences = true;
console.Out.WriteLine(ANSIParser.GetText(text));
console.Out.WriteLine(ANSIParser.GetText(text2));
console.Settings.RemoveANSISequences = false;

console.Out.WriteLine("(rdc)");

#if Enable_Buffer
console.Out.CloseBuffer();
#endif

void Title(IAnsiVtConsole console)
{
    var str = @"
   ___            _ __   __ _     ___                      _              _  _       _     ___                  
  /   \ _ _   ___(_)\ \ / /| |_  / __| ___  _ _   ___ ___ | | ___        | \| | ___ | |_  / __| ___  _ _  ___   
  | - || ' \ (_-/| | \   / |  _|| (__ / _ \| ' \ (_-// _ \| |/ -_)  _    | .  |/ -_)|  _|| (__ / _ \| '_|/ -_)  
  |_|_||_||_|/__/|_|  \_/   \__| \___|\___/|_||_|/__/\___/|_|\___| (_)   |_|\_|\___| \__| \___|\___/|_|  \___|  
";

    RaimbowText RaimbowText(string str)
        => new RaimbowText(str)
            .Origin(0, 0, 128)
            .CyclicGradient(4, 9, 14)
            .Add(console);

    var title = RaimbowText(str);

    RaimbowText($"  AnsiVtConsole.NetCore v{Assembly.GetExecutingAssembly().GetName().Version}");

    console.Out.WriteLine();

    var bar = RaimbowText("".PadLeft(113, '─'));

    var anims = new AnimationGroup(
        new IntAnimation(0, 255, 10000)
            .For(() => bar.OriginRGB.R))
         .Target(bar.OriginRGB);

    var anim = new Animation()
        .Add(
            new TimeLine()
                .Add(anims)
                .Update(bar)
            )
        .Start()
        .Wait();

    console.Out.WriteLine();
    console.Out.WriteLine();

    console.Exit();
}

void AnsiColorTest(IAnsiVtConsole console)
{
    // 3 bits colors (standard)
    var colw = 8;
    var totw = colw * 8 + 3 + 10;
    var hsep = "".PadLeft(totw, '-');
    var esc = (char)27;
    string r;
    var x2 = 0;

    console.Out.WriteLine("(f=blue)using static (f=darkgray)AnsiVtConsole.NetCore.Component.Console.(f=green)ANSI(f=darkgray);");
    console.Out.WriteLine("(f=blue)using static (f=darkgray)AnsiVtConsole.NetCore.Component.EchoDirective.(f=green)Shortcuts(f=darkgray);(br)");

    console.Out.WriteLine("(uon,bon)3 bits (8 color mode)");
    console.Out.WriteLine();
    console.Out.WriteLine("Background | Foreground colors");
    console.Out.WriteLine(hsep);
    for (var j = 0; j <= 7; j++)
    {
        var str1 = $" ESC[4{j}m   | {esc}[4{j}m";
        var str2 = $" ESC[10{j}m  | {esc}[10{j}m";
        for (var i = 0; i <= 7; i++)
        {
            str1 += Set4BitsColors(i, j | 0b1000) + $" [9{i}m   ";
            str2 += Set4BitsColors(i | 0b1000, j) + $" [3{i}m   ";
        }

        console.Out.WriteLine(str1 + "");
        console.Out.WriteLine(str2 + "");
        console.Out.WriteLine(hsep);
    }
    console.Out.WriteLine();

    // 8 bits colors
    console.Out.WriteLine("(uon,bon)8 bits (256 color mode)");
    console.Out.WriteLine();
    console.Out.WriteLine("216 colors: 16 + 36 × r + 6 × g + b (0 <= r, g, b <= 5)(br)");
    var n = 16;
    for (var y = 0; y < 6; y++)
    {
        r = "";
        for (var x = 16; x <= 51; x++)
        {
            if (x >= 34)
                r += Black;
            else
                r += White;
            r += $"{esc}[48;5;{n}m" + ((n + "").PadLeft(4, ' '));
            n++;
            x2++;
            if (x2 >= 6) { r += Br; x2 = 0; }
        }
        console.Out.Write(r);
    }

    console.Out.WriteLine();
    console.Out.WriteLine("(uon,bon)grayscale colors (24 colors) : 232 + l (0 <= l <= 24)(br)");
    r = White;
    x2 = 0;
    for (var x = 232; x <= 255; x++)
    {
        if (x >= 244)
            r += Black;
        r += $"{esc}[48;5;{x}m" + ((x + "").PadLeft(4, ' '));
        x2++;
        if (x2 >= 6) { r += console.Out.LNBRK; x2 = 0; }
    }
    console.Out.Write(r);

    console.Out.WriteLine();
    console.Out.WriteLine("(uon,bon)24 bits (16777216 colors): 0 <= r,g,b <= 255 (br) ");

    string cl(int r, int v, int b) =>
        esc + "[48;2;" + r + ";" + v + ";" + b + "m ";

    var stp = 4;
    r = "";
    int cr, cb = 0, cv = 0;
    for (cr = 0; cr < 255; cr += stp)
        r += cl(cr, cv, cb);
    console.Out.WriteLine(r);

    r = "";
    cr = 0;
    for (cv = 0; cv < 255; cv += stp)
        r += cl(cr, cv, cb);
    console.Out.WriteLine(r);

    cv = 0;
    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(cr, cv, cb);
    console.Out.WriteLine(r);

    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(cb, cb, 0);
    console.Out.WriteLine(r);

    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(cb, 0, cb);
    console.Out.WriteLine(r);

    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(0, cb, cb);
    console.Out.WriteLine(r);

    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(cb, cb, cb);
    console.Out.WriteLine(r);
}