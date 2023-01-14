
using CommandLine.NetCore.Services.CmdLine.Arguments;
using CommandLine.NetCore.Services.CmdLine.Commands;

namespace AnsiVtConsole.NetCore.CommandLine;

class Out : Command
{
    public Out(Dependencies dependencies) : base(dependencies)
    {
    }

    protected override CommandResult Execute(ArgSet args) =>

        // echo <text> [--err]

        For(Param())
            .Do(() => OutAnsi)

        // echo <text> --raw [--esc-only] [--hexa] [--err]

        .For(Param(), Opt("raw"), Opt("esc-only", true), Opt("hexa", true))
            .Do(() => OutRaw)

        .Options(Opt("err"))

        .With(args);


    void OutAnsi(Param<string> textParam, Opt errOpt)
        => DoOut(textParam.Value!, errOpt.IsSet, false, false, false);

    void OutRaw(Param<string> textParam, Opt errOpt, Opt escOpt, Opt hexaOpt)
        => DoOut(textParam.Value!, errOpt.IsSet, true, escOpt.IsSet, hexaOpt.IsSet);

    void DoOut(string text, bool err, bool raw, bool esc, bool hexa)
    {
        var console = new AnsiVtConsole();

        if (raw)
            text = console.Out.GetRawText(text, esc, hexa);

        if (!err)
            console.Out.WriteLine(text);
        else
            console.Logger.LogError(text);
    }
}