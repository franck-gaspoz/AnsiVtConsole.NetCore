___

# ![AnsiVtConsole.NetCore](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/ascii-icon.png "AnsiVtConsole.NetCore") AnsiVtConsole.NetCore

___

**AnsiVtConsole.NetCore library** provides ANSI VT support (cursor,colors,screen size) for multi-plateform (windows, linux, osx, arm) console applications using C# and .NET Core 6

[![licence mit](https://img.shields.io/badge/licence-MIT-blue.svg)](license.md) This project is licensed under the terms of the MIT license: [LICENSE.md](LICENSE.md)  
![last commit](https://img.shields.io/github/last-commit/franck-gaspoz/AnsiVtConsole.NetCore?style=plastic)
![version](https://img.shields.io/github/v/tag/franck-gaspoz/AnsiVtConsole.NetCore?style=plastic)
___

![title](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/title.png "title")  

# Features

The library provides functionalities needed to build console applications running in a terminal (WSL/WSL2, cmd.exe, ConEmu, bash, ...) with text interface. That includes:
- **a text printer engine** that supports **print directives** (markup) allowing to manage console functionalities from text itself, as html would do but with a simplest grammar (that can be configured). That makes possible colored outputs, cursor control, text scrolling and also dynamic C# execution (scripting), based on **System.Console** and **ANSI VT100 / VT52 (VT100 type Fp or 3Fp, Fs, CSI, SGR)** 

- A ANSI Parser that can identify/remove escape sequences in a text

- **widgets** : visual elements with live update, animations, thread safe, combinables together 
    - raimbow text,bar
    - animated text, text timer, type writer
    - image (provided in a separate package: [AnsiVtConsole.NetCore.Imaging](https://www.nuget.org/packages/AnsiVtConsole.NetCore/))

- The console output can be controlled by:
    - tokens in a string (print directives)
    - as string shortcuts (dynamic ansi vt strings) powered by **SkiaSharp**
    - throught API methods

# Index

- [Usage](#usage)
    - [using the markup](#using-the-markup)
    - [using the string shortcuts](#using-the-string-shortcuts)
    - [using the ANSI sequences](#using-the-ansi-sequences)
    - [using the widgets](#using-the-widgets)
- [Print directives (markup)](#print-directives-markup)
    - [1. Colorization with SGR (Select Graphic Rendition)](#colorization-with-sgr-select-graphic-rendition)
    - [2. Text decoration (vt100) with SGR (Select Graphic Rendition)](#text-decoration-vt100-with-sgr-select-graphic-rendition)
    - [3. CSI (Control Sequence Introducer)](#csi-control-sequence-introducer)
    - [4. Script engine](#script-engine)
    - [5. Application control](#application-control)
    - [6. ANSI Sequences](#ansi-sequences)
    - [7. Unicode characters](#unicode-characters)
- [Command line interface tool for your shells](#command-line-interface-tool-for-your-shells)
- [Examples](#examples)
    - [project AnsiConsole.NetCore.Examples.Widgets](#project-ansiconsole.netcore.examples.widgets) 
    - [project AnsiConsole.NetCore.Examples.ANSI](#project-ansiconsole.netcore.examples.ansi)
        - [Colorisation](#colorisation)
        - [AnsiVtConsole markup and Ansi/Vt parsing](#ansivtconsole-markup-and-ansivt-parsing)
- [Version history](#versions-history)

# Usage

download the nuget from command line or add it from Visual Studio

``` dos
@rem version 1.0.21 or any new one
dotnet add package AnsiVtConsole.NetCore --version 1.0.21
```

> **Note**
>
> When installing the package, the following files are copied into your project:
> - LICENSE.md
> - README.md
> - Component/Parser/ANSI/ansi-seq-patterns.txt
> - assets/example1.png
> - assets/example2.png
> - assets/example3.png
> - assets/example4.png
> - assets/example5.png
> - assets/example6.png
> - assets/output.png
> - assets/ascii-icon.png
>
> you can delete these files **EXCEPT `ansi-seq-patterns.txt`** (ANSI grammar) that is required for the ANSI parser to work
>
> these files are set as `Content` and are copied to output folder on build


``` csharp
// get the ansi vt console
var console = new AnsiVtConsole.NetCore.AnsiVTConsole();
```

## 1. using the markup :

``` csharp
console.Write("(br,f=yellow,b=red)yellow text on red background(br)(f=cyan)current time is: (exec=System.DateTime.Now,br)");
```

## 2. using the string shortcuts :

``` csharp
using static AnsiVtConsole.NetCore.Component.EchoDirective;

System.Console.Out.Writeline($"{Br}{Yellow}{BRed}yellow text on red background{Br}{Cyan}current time is: {System.DateTime.Now}{Br}");
```

## 3. using the ANSI sequences:

``` csharp
using static AnsiVtConsole.NetCore.Component.Console.ANSI;

System.Console.Out.Writeline($"{CRLF}{SGRF("Yellow")}{SGRB("Red")}yellow text on red background{CRLF}{SGRF("Cyan")}current time is: {System.DateTime.Now}{CRLF}");
```

### all outputs:

![output](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/output.png "output")

## 4. using the widgets

``` csharp
/* type writer example
this adds a 'type writer' widget at current cursor location, 
maintains it in place and produces an animation of the text until
the end of the animation
*/
using AnsiVtConsole.NetCore.Component.Widgets.Texts.TypeWriting;

TypeWriter(text)
    .Add(console)   // add to a console and render the widget
    .Wait();        // animated update until end of animation
```

example output:

![widget output](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/ansivtconsole.netcore.widget.gif "output")

# Print directives (markup):

text can contains echo directives that changes the echo behavior. the echo directive syntax is formed according to this pattern: `(printDirective) or (printDirective=printDirectiveValue)`

- multiple echo directives can be separated by a , that can be grouped in a single text in parentheses: `(echoDirective1,echoDirective2=..,echoDirective3)`
- an echo directive value can be written inside a 'code' text block, depending on each echo directive, with the syntax: `[[...]]`
- symbols of this grammar can be configured throught the class:
    `AnsiVtConsole.NetCore.Component.Settings`
- alternatively to the print directives you can use the strings shortcuts from the class: `AnsiVtConsole.NetCore.Component.EchoDirective`

Available echo directives are defined in the class ``

## 1. Colorization with SGR (Select Graphic Rendition)

```yaml
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
rdc                 : restore default colors```

ConsoleColor := darkblue|darkgreen|darkcyan|darkred|darkmagenta|darkyellow|gray|darkgray|blue|green|cyan|red|magenta|yellow|white (not case sensitive)
```

## 2. Text decoration (vt100) with SGR (Select Graphic Rendition)

```yaml
uon                 : underline on
invon               : inverted colors on
tdoff               : text decoration off and reset default colors
lion                : ligtht colors
bon                 : bold on
blon                : blink on
```

## 3. CSI (Control Sequence Introducer)

```yaml
cls                 : clear screen
br                  : jump begin of next line (line break)
bkcr                : backup cursor position
rscr                : restore cursor position
crx=Int32           : set cursor x (0<=x<=WindowWidth)
cry=Int32           : set cursor y (0<=y<=WindowHeight)
crh                 : hide cursor
crs                 : show cursor
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
fillright           : fill line from cursor right
chome               : move cursor to upper left corner
tab                 : add a tab
```

## 4. Script engine

```yaml
exec=CodeBlock|[[CodeBlock]] : executes and echo result of a C# code block
```

```csharp
using static AnsiVtConsole.NetCore.Component.Parser.ANSI;
using cons=AnsiVtConsole.NetCore;
// get the ansi vt console
var console = new cons.AnsiVTConsole();

console.Out.WriteLine("current date is: (exec=System.DateTime.Now.Date)");
```

## 5. Application control

```yaml
info                : output infos about the console
exit                : exit the current process
```

## 6. ANSI Sequences

ANSI sequences are defined in `AnsiVtConsole.NetCore.Component.Console` and can be used directly to build ANSI strings

```csharp
using static AnsiVtConsole.NetCore.Component.Parser.ANSI;
using cons=AnsiVtConsole.NetCore;
// get the ansi vt console
var console = new cons.AnsiVTConsole();

var str = $"{SGR_Underline}{SGR_CrossedOut}my text{SGR_SetBackgroundColor24bits(15,152,123)}in color";
System.Console.WriteLine(str);

// is equivalent to:

cons.Out.WriteLine("(SGR_Underline,SGR_SlowBlink)my text(SGRB24=15:152:123)in color");

// or to:

cons.Out.WriteLine("(uon,blon)my text(f24=15:152:123)in color");

```

## 7. Unicode characters

Unicode characters are defined in the class `AnsiVtConsole.NetCore.Component.Console`.

```csharp

using static AnsiVtConsole.NetCore.Component.Console.Unicode;
using cons=AnsiVtConsole.NetCore;
// get the ansi vt console
var console = new cons.AnsiVTConsole();

console.Out.WriteLine($"it's the (bkf,f=red){Demi}(rsf) or the (bkf,f=yellow){Quar}(rsf) of it");

console.Out.WriteLine("(f=cyan,EdgeTopLeft,BarHorizontal,EdgeTopRight)");
console.Out.WriteLine("(f=cyan,BarVertical,f=blue,Box,f=cyan,BarVertical)");
console.Out.WriteLine("(f=cyan,EdgeBottomLeft,BarHorizontal,EdgeBottomRight)");
```
![example6](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/example6.png "example6")

# Command line interface tool for your shells

The project `AnsiVtConsole.NetCore.CommandLine` build a **command line tool** that calls the **AnsiVtConsole `WriteLine`** method.
With that you can add to you shell scripts the outputs provided by AnsiVtConsole:

```dos
out.exe "(br,f=yellow,b=red)yellow text on red background(br)(f=cyan)current time is: (exec=System.DateTime.Now,br)"
```

outputs:

![output](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/output.png "output")

```dos
out.exe "(br,f=yellow,b=red)yellow text on red background(br)(f=cyan)current time is: (exec=System.DateTime.Now,br)" --raw
```

outputs:

`\e[4m\e[0m\e[0K\r\n\e[4m\e[0m\e[37m\e[93m\e[101myellow text on red background\e[4m\e[0m\e[0K\r\n\e[4m\e[0m\e[37m\e[96mcurrent time is: 13/06/2020 06:17:15\e[4m\e[0m\e[0K\r\n\e[4m\e[0m\e[37m`

this tool accepts these arguments:

```dos
out.exe <text> [--raw [--esc-only] [--hexa] ] 
```

- if `--err` outputs to standard error stream instead of standard output stream
- if `--raw`, parse ANSI and non printable characters to show them by their names or representations
- if `--raw` these options are avalaibles :
    - `--esc-only` : disable only knowns non printable characters (from ASCII)
    - `--hexa` : use the hexa unix format for non printable characters

> **Note**
>
> the command line tool is built upon the library **`CommandLine.NetCore`**. For more information for this, please refers to:
> - [CommandLine.NetCore nuget](https://www.nuget.org/packages/CommandLine.NetCore/#readme-body-tab)
> - [CommandLine.NetCore GitHub repository](https://github.com/franck-gaspoz/CommandLine.NetCore)

# Examples

## project `AnsiConsole.NetCore.Examples.Widgets`

this project once compiled provides a **live demo in the console** of **widgets** implemented in the current version of the library.
To try these examples, compile and run the project **AnsiVtConsole.NetCore.Examples.Widgets**:

![title](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/title.png "title")  

example of an **Image** widget. This widget is delivered in the package: [AnsiVtConsole.NetCore.Imaging](https://www.nuget.org/packages/AnsiVtConsole.NetCore/)

![example7](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/example7.png "example7")



## project `AnsiConsole.NetCore.Examples.ANSI`

### Colorization

To try these examples, compile and run the project **AnsiVtConsole.NetCore.Examples.ANSI**:

![example1](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/example1.png "example1")
![example2](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/example2.png "example2")

### AnsiVtConsole markup and Ansi/Vt parsing

![example5](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/example5.png "example5")

# Versions history

`1.0.21,1.0.22,1.0.23,1.0.24,1.0.25,1.0.26` - 2-16-2024
- fix nuget content files

`1.0.20` - 1-23-2023
- animated widgets
- properties animator
- widgets: text,type writer,gradient,gradient bar,text timer
- widget image in a separated project AnsiVtConsole.NetCore.Imaging using SkiaSharp
- examples projects
- fluent methods for ContextTextWriterWrapper
- doc update

`1.0.19` - 1-17-2023
- add **widgets** feature + RaimbowText widget
- add example
- doc update

`1.0.18` - 1-1-2023
- add symbols and sources in package
- new editorconfig and code clean up
- command `out` for a shell available in `AnsiVtConsole.NetCore.CommandLine.Out` that compiles to `out.exe`
- doc update

`1.0.17` - 10-1-2023
- add setting that make it possible to disable ansi/vt in console ouputs: `AnsiVtConsole.NetCore.Component.Settings` : `IsMarkupDisabled`,`IsRawOutputEnabled`,`ReplaceNonPrintableCharactersByTheirName`,`RemoveANSISequences`
- add methods to get output text in various formats (without ansi,with unparsed markup,in shell escaped characters) : `GetText`,`GetRawText`,`ANSIParser.GetText` 
- add grammar file for ANSI parser
- enable buffering mode for any print directive
- update doc

`1.0.16` - 5-1-2022
- fix nupkg. add deployment of documentation files
- update doc

`1.0.14,1.0.15` - 12-17-2022
- fix LogError,LogWarning

`1.0.13` - 12-17-2022
- fix refactoring Write,WriteLine
- add default empty string to log methods

`1.0.12` - 12-17-2022
- rename Writeln methods by WriteLine
- suppress Log,Logln &amp; logger refactoring
- add code documentation

**AnsiVtConsole.NetCore.Imaging**

`1.0.0` - 21-1-2023
- inital version. provide widget image to AnsiConsole.NetCore (package [AnsiVtConsole.NetCore.Imaging](https://www.nuget.org/packages/AnsiVtConsole.NetCore/))

___

