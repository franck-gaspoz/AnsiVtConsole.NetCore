using AnsiVtConsole.NetCore.Component.Widgets.Texts.Timers;
using AnsiVtConsole.NetCore.Component.Widgets.Texts.TypeWriting;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;

namespace AnsiVtConsole.NetCore.Examples.Widgets;

abstract class DemoPage
{
#pragma warning disable CS8618
    protected IAnsiVtConsole _;
#pragma warning restore CS8618

    public void Run(IAnsiVtConsole console)
    {
        _ = console;
        Run();
    }

    protected static TypeWriter TypeWriter(string text)
        => new(
            $"{SGR_SetForegroundColor4bits(SGR_4BitsColors.White, true)}{text}",
            40,
            $"{SGR_SetForegroundColor4bits(SGR_4BitsColors.Green, true)}█(rsf)");

    protected TypeWriter TypeWrite(string text)
        => TypeWriter(text).Add(_).Wait();

    public abstract void Run();

    protected static void Wait(int delay) => Thread.Sleep(delay);

    protected void WaitPage(int waitForNextPage)
    {
        PleaseWait(waitForNextPage);
        Thread.Sleep(waitForNextPage * 1000);
    }

    protected void SubTitle(string text)
        => _!.Out.WriteLine($"(bkf,f=white){text}(rsf,br)");

    protected void PleaseWait(int waitForNextPage)
        => new TextTimer(
            "(br,br,bkf,f=yellow)►►► This page will automatically update in {0} seconds...(rsf)",
            2,
            TimeSpan.FromSeconds(waitForNextPage),
            (duration) => duration.Seconds.ToString())
            .Add(_!);

    protected void PleaseWait0(int waitForNextPage)
        => _!.Out.WriteLine($"(br,br,bkf,f=yellow)►►► This page will automatically update in {waitForNextPage} seconds...(rsf)");
}
