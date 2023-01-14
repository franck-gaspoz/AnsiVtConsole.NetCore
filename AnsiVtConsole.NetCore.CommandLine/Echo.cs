
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;

namespace AnsiVtConsole.NetCore.CommandLine;

class Echo : Command
{
    public Echo(Dependencies dependencies) : base(dependencies)
    {
    }

    protected override CommandResult Execute(ArgSet args) =>

        // echo <text> --err

        For(Param())
        .Do(() => DoEcho)
        .Options(Opt("err"))
        .With(args);

    void DoEcho(Param<string> textParam, Opt errOpt)
    {
        var console = new AnsiVtConsole();
        var text = textParam.Value!;

        if (errOpt.IsSet)
            console.Out.WriteLine(text);
        else
            console.Logger.LogError(text);
    }
}