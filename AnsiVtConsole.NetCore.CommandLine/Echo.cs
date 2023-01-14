
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;

namespace AnsiVtConsole.NetCore.CommandLine;

class Echo : Command
{
    public Echo(Dependencies dependencies) : base(dependencies)
    {
    }

    protected override CommandResult Execute(ArgSet args) =>
        For(Param())
        .Do(() => DoEcho)
        .With(args);

    void DoEcho(Param<string> text)
    {

    }
}