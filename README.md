<hr>

# <img src="Doc/Images/game-ascii-icon-200x200.png" width="72" valign="middle"/> AnsiVtConsole.NetCore

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
using cons=AnsiVtConsole.NetCore;
// get the ansi vt console
var console = new cons.AnsiVTConsole();
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

<image src="Doc/Images/2020-06-13 06_18_08-Window.png"/>     

# Examples

To see these examples, compile and run the project **AnsiVtConsole.NetCore.Examples**:

<image src="Doc/Images/example1.png"/>    
<image src="Doc/Images/example2.png"/>    
<image src="Doc/Images/example3.png"/>    
<image src="Doc/Images/example4.png"/>    

<hr>
