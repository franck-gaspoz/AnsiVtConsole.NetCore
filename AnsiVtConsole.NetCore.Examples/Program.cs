using System.Reflection;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;
using static AnsiVtConsole.NetCore.Component.EchoDirective.Shortcuts;

using cons = AnsiVtConsole.NetCore;

var console = new cons.AnsiVtConsole();

var title = $"| AnsiVtConsole.NetCore v{Assembly.GetExecutingAssembly().GetName().Version} |";
var sep = "".PadLeft(title.Length, '-');

console.Out.Echoln($"(bon,f=cyan){sep}");
console.Out.Echoln($"(bon,f=cyan){title}");
console.Out.Echoln($"(bon,f=cyan){sep}(br)");

console.Out.Echoln("(uon,bon)init:(br)");

console.Out.Echoln("(f=blue)using (f=darkgray)cons = AnsiVtConsole.NetCore;");
console.Out.Echoln("(f=blue)var (f=cyan)console (f=darkgray)= (f=blue)new (f=darkgray)cons.(f=green)AnsiVtConsole(f=white)();(br)");

console.Out.Echoln("(uon,bon)console.Infos():(br)");
console.Infos();

console.Out.Echoln("(br,uon,bon)console.Out.Echo():");
console.Out.Echoln("(br,b=red,f=yellow,uon)Hello, World!");

console.Out.Echoln("(br,uon,bon)console.Logger,console.Warn,console.Err:(br)");
console.Logger.Log("console.Logger.Log(\"log\")");
console.Logger.LogWarning("console.Logger.LogWarning(\"warning\")");
console.Logger.LogError("console.Logger.LogError(\"error\")");
console.Logger.LogError(
    new ArgumentException("bad argument exception"),
    "console.Logger.LogError(exception):");
console.Warn.Logln("console.Warn.Log(\"err\")");
console.Err.Logln("console.Err.Log(\"err\")");

console.Out.Echoln("(br,uon,bon)ansi vt methods (ANSI,Shortcuts) - colors test:(br)");

AnsiColorTest(console);

console.Out.Echoln();

console.Out.Echoln("(br,uon,bon)echo directives:(br)");

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
    (f=yellow)invon(rdc)               : inverse on

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

console.Out.Echoln(PrintDocText);

console.Out.Echoln();

void AnsiColorTest(cons.IAnsiVtConsole console)
{
    // 3 bits colors (standard)
    var colw = 8;
    var totw = colw * 8 + 3 + 10;
    var hsep = "".PadLeft(totw, '-');
    var esc = (char)27;
    string r;
    var x2 = 0;

    console.Out.Echoln("(f=blue)using static (f=darkgray)AnsiVtConsole.NetCore.Component.Console.(f=green)ANSI(f=darkgray);");
    console.Out.Echoln("(f=blue)using static (f=darkgray)AnsiVtConsole.NetCore.Component.EchoDirective.(f=green)Shortcuts(f=darkgray);(br)");

    console.Out.Echoln("(uon,bon)3 bits (8 color mode)");
    console.Out.Echoln();
    console.Out.Echoln("Background | Foreground colors");
    console.Out.Echoln(hsep);
    for (var j = 0; j <= 7; j++)
    {
        var str1 = $" ESC[4{j}m   | {esc}[4{j}m";
        var str2 = $" ESC[10{j}m  | {esc}[10{j}m";
        for (var i = 0; i <= 7; i++)
        {
            str1 += Set4BitsColors(i, j | 0b1000) + $" [9{i}m   ";
            str2 += Set4BitsColors(i | 0b1000, j) + $" [3{i}m   ";
        }

        console.Out.Echoln(str1 + "");
        console.Out.Echoln(str2 + "");
        console.Out.Echoln(hsep);
    }
    console.Out.Echoln();

    // 8 bits colors
    console.Out.Echoln("(uon,bon)8 bits (256 color mode)");
    console.Out.Echoln();
    console.Out.Echoln("216 colors: 16 + 36 × r + 6 × g + b (0 <= r, g, b <= 5)(br)");
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
        console.Out.Echo(r);
    }

    console.Out.Echoln();
    console.Out.Echoln("(uon,bon)grayscale colors (24 colors) : 232 + l (0 <= l <= 24)(br)");
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
    console.Out.Echo(r);

    console.Out.Echoln();
    console.Out.Echoln("(uon,bon)24 bits (16777216 colors): 0 <= r,g,b <= 255 (br) ");

    string cl(int r, int v, int b) =>
        esc + "[48;2;" + r + ";" + v + ";" + b + "m ";

    var stp = 4;
    r = "";
    int cr, cb = 0, cv = 0;
    for (cr = 0; cr < 255; cr += stp)
        r += cl(cr, cv, cb);
    console.Out.Echoln(r);

    r = "";
    cr = 0;
    for (cv = 0; cv < 255; cv += stp)
        r += cl(cr, cv, cb);
    console.Out.Echoln(r);

    cv = 0;
    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(cr, cv, cb);
    console.Out.Echoln(r);

    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(cb, cb, 0);
    console.Out.Echoln(r);

    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(cb, 0, cb);
    console.Out.Echoln(r);

    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(0, cb, cb);
    console.Out.Echoln(r);

    r = "";
    for (cb = 0; cb < 255; cb += stp)
        r += cl(cb, cb, cb);
    console.Out.Echoln(r);
}