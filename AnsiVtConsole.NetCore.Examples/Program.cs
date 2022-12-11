using System.Reflection;

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

console.Out.Echoln("(br,uon,bon)vt methods:");


console.Out.Echoln();
