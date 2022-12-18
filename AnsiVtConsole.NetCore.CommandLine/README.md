___

# ![AnsiVtConsole.NetCore](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/ascii-icon.png "AnsiVtConsole.NetCore") AnsiVtConsole.NetCore

___

**AnsiVtConsole.NetCore library** provides ANSI VT support (cursor,colors,screen size) for multi-plateform (windows, linux, osx, arm) console applications using C# and .NET Core 6

[![licence mit](https://img.shields.io/badge/licence-MIT-blue.svg)](license.md) This project is licensed under the terms of the MIT license: [LICENSE.md](LICENSE.md)  
![last commit](https://img.shields.io/github/last-commit/franck-gaspoz/AnsiVtConsole.NetCore?style=plastic)
![version](https://img.shields.io/github/v/tag/franck-gaspoz/AnsiVtConsole.NetCore?style=plastic)
___

# Features

The library provides functionalities needed to build console applications running in a terminal (WSL/WSL2, cmd.exe, ConEmu, bash, ...) with text interface. That includes:
- **a text printer engine** that supports **print directives** allowing to manage console functionalities from text itself, as html would do but with a simplest grammar (that can be configured). That makes possible colored outputs, cursor control, text scrolling and also dynamic C# execution (scripting), based on **System.Console** and **ANSI VT100 / VT52 (VT100 type Fp or 3Fp, Fs, CSI, SGR)** 

- The console output can be controlled by:
    - tokens in a string (print directives)
    - as string shortcuts (dynamic ansi vt strings)
    - throught API methods

# Usage

``` csharp
using cons=AnsiVtConsole.NetCore;
// get the ansi vt console
var console = new cons.AnsiVTConsole();
```

## 1. using the text parser:

``` csharp
console.Write("(br,f=yellow,b=red)yellow text on red background(br)(f=cyan)current time is: (exec=System.DateTime.Now,br)");
```

## 2. using the string shortcuts :

``` csharp
using static AnsiVtConsole.NetCore.Component.EchoDirective;

System.Console.Out.Writeline($"{Br}{Yellow}{BRed}yellow text on red background{Br}{Cyan}current time is: {System.DateTime.Now}{Br}");
```

### both outputs:

![output](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/output.png "output")

# Print directives:

text can contains echo directives that changes the echo behavior. the echo directive syntax is formed according to this pattern: `(printDirective) or (printDirective=printDirectiveValue)`

- multiple echo directives can be separated by a , that can be grouped in a single text in parentheses: `(echoDirective1,echoDirective2=..,echoDirective3)`
- an echo directive value can be written inside a 'code' text block, depending on each echo directive, with the syntax: `[[...]]`
- symbols of this grammar can be configured throught the class:
    `AnsiVtConsole.NetCore.Component.Settings`
- alternatively to the print directives you can use the strings shortcuts from the class: `AnsiVtConsole.NetCore.Component.EchoDirective`

## 1. Colorization

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

## 2. Text decoration (vt100)

```yaml
uon                 : underline on
invon               : inverted colors on
tdoff               : text decoration off and reset default colors
lion                : ligtht colors
bon                 : bold on
blon                : blink on (not supported on Windows)
```

## 3. Echo flow control

```yaml
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
```

## 4. Script engine

```yaml
exec=CodeBlock|[[CodeBlock]] : executes and echo result of a C# code block
```

## 5. Application control

```yaml
info                : output infos about the console
exit                : exit the current process
```

# Examples

To try these examples, compile and run the project **AnsiVtConsole.NetCore.Examples**:

![example1](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/example1.png "example1")
![example1](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/example2.png "example2")
![example1](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/example3.png "example3")
![example1](https://raw.githubusercontent.com/franck-gaspoz/AnsiVtConsole.NetCore/main/AnsiVtConsole.NetCore/assets/example4.png "example4")

# Version history

1.0.14,1.0.15 - 12-17-2022

- fix LogError,LogWarning

1.0.13 - 12-17-2022

- fix refactoring Write,WriteLine
- add default empty string to log methods

1.0.12 - 12-17-2022

- rename Writeln methods by WriteLine
- suppress Log,Logln & logger refactoring
- add code documentation
___

