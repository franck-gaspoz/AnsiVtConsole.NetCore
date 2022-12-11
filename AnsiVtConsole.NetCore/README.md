<hr>

# AnsiVtConsole.NetCore

<hr>

<b>AnsiVtConsole.NetCore library</b> helps build fastly nice multi-plateform (windows, linux, osx, arm) console applications using C# and .NET Core 6

[![licence mit](https://img.shields.io/badge/licence-MIT-blue.svg)](license.md) This project is licensed under the terms of the MIT license: [LICENSE.md](LICENSE.md)  

![last commit](https://img.shields.io/github/last-commit/franck-gaspoz/AnsiVtConsole.NetCore?style=plastic)
![version](https://img.shields.io/github/v/tag/franck-gaspoz/AnsiVtConsole.NetCore?style=plastic)
<hr>

# Features

The toolkit provides functionalities needed to build console applications running in a terminal (WSL/WSL2, cmd.exe, ConEmu, bash, ...) with text interface. That includes:
- <b>a text printer engine </b>that supports <b>print directives</b> allowing to manage console functionalities from text itself, as html would do but with a simplest grammar (that can be configured). That makes possible colored outputs, cursor control, text scrolling and also dynamic C# execution (scripting), based on <b>System.Console</b> and <b> ANSI VT100 / VT52 (VT100 type Fp or 3Fp, Fs, CSI, SGR)</b>. 
- The print directives can be used:

    - as tokens in a string
    - as methods

# Usage

``` csharp
using AnsiVtConsole.NetCore;
// get the ansi vt console
var console = new AnsiVTConsole();
```

## 1. using the text parser:
``` csharp
console.Echo("(br,f=yellow,b=red)yellow text on red background(br)(f=cyan)current time is: (exec=System.DateTime.Now,br)");
```

## 2. using the methods :

``` csharp
using static AnsiVtConsole.NetCore.Component.EchoDirective;
System.Console.Out.Writeline($"{Br}{Yellow}{BRed}yellow text on red background{Br}{Cyan}current time is: {System.DateTime.Now}{Br}");
```

<br>

# echo directives:

text can contains echo directives that changes the echo behavior.
the echo directive syntax is formed according to these pattern:

(printDirective) or (printDirective=printDirectiveValue)

- multiple echo directives can be separated by a , that can be grouped in a single text in parentheses: (echoDirective1,echoDirective2=..,echoDirective3)
- an echo directive value can be written inside a 'code' text block, depending on each echo directive, with the syntax: [[...]]
- symbols of this grammar can be configured throught the class AnsiVtConsole.NetCore.Component.Settings

## current print directives are:

### (1) colorization:

    f=ConsoleColor      : set foreground color
    f8=Int32            : set foreground 8bit color index, where 0 <= index <= 255
    f24=Int32:Int32:Int32 : set foreground 24bit color red:green:blue, where 0 <= red,green,blue <= 255
    f=ConsoleColor      : set foreground color
    b=ConsoleColor      : set background color
    b8=Int32            : set background 8bit color index, where 0 <= index <= 255
    b24=Int32:Int32:Int32 : set background 24bit color red:green:blue, where 0 <= red,green,blue <= 255
    df=ConsoleColor     : set default foreground
    db=ConsoleColor     : set default background
    bkf                 : backup foreground color
    bkb                 : backup background color
    rsf                 : restore foreground color
    rsb                 : restore background color
    rdc                 : restore default colors

### (2) text decoration (vt100):

    uon                 : underline on
    invon               : inverted colors on
    tdoff               : text decoration off and reset default colors
    lion                : ligt colors
    bon                 : bold on
    blon                : blink on (not supported on Windows)

### (3) echo flow control:

    cls                 : clear screen
    br                  : jump begin of next line (line break)
    bkcr                : backup cursor position
    rscr                : restore cursor position
    crx=Int32           : set cursor x (0<=x<=WindowWidth)
    cry=Int32           : set cursor y (0<=y<=WindowHeight)
    cleft               : move cursor left
    cright              : move cursor right
    cup                 : move cursor up
    cdown               : move cursor down
    cnleft=Int32        : move cursor n characters left
    cnright=Int32       : move cursor n characters right
    cnup=Int32          : move cursor n lines up
    cndown=Int32        : move cursor n lines down
    cl                  : clear line
    clleft              : clear line from cursor left
    clright             : clear line from cursor right
    chome               : move cursor to upper left corner
    tab                 : add a tab

### (4) script engine:

    exec=CodeBlock|[[CodeBlock]] : executes and echo result of a C# code block

### (5) application control:

    exit                : exit the current process

    ConsoleColor := darkblue|darkgreen|darkcyan|darkred|darkmagenta|darkyellow|gray|darkgray|blue|green|cyan|red|magenta|yellow|white (not case sensitive)