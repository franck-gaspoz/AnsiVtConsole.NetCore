﻿using cons = AnsiVtConsole.NetCore;

var console = new cons.AnsiVtConsole();

console.Out.Echo("(b=red,f=yellow)Hello, World!(br)");

console.StdErr.WriteLine("hello world");
