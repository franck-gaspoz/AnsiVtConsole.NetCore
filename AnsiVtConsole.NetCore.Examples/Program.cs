using System.Reflection;

using cons = AnsiVtConsole.NetCore;

var console = new cons.AnsiVtConsole();

console.Out.Echoln($"(bon,f=cyan)AnsiVtConsole.NetCore v{Assembly.GetExecutingAssembly().GetName().Version}(br)");

console.Out.Echoln("(uon,bon)init:(br)");

console.Out.Echoln("(f=blue)using (f=darkgray)cons = AnsiVtConsole.NetCore;");
console.Out.Echoln("(f=blue)var (f=cyan)console (f=darkgray)= (f=blue)new (f=darkgray)cons.(f=green)AnsiVtConsole(f=white)()(br)");

console.Out.Echoln("(uon,bon)console.Infos():(br)");
console.Infos();

console.Out.Echoln("(br,uon,bon)console.Out.Echo():");
console.Out.Echoln("(br,b=red,f=yellow,uon)Hello, World!");

console.Out.Echoln("(br,uon,bon)console.Logger:(br)");
console.Logger.Log("console.Logger.Log()");
console.Logger.LogWarning("console.Logger.LogWarning()");
console.Logger.LogError("console.Logger.LogError()");
console.Logger.LogException(
    new ArgumentException("bad argument exception"),
    "console.Logger.LogException():");

console.Out.Echoln();
