using AnsiVtConsole.NetCore.Component.Widgets.Texts;
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
            10000,
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
    {
        TypeWrite($"(f=white){text}");
        _.Out.WriteLine();
    }

    protected void PleaseWait(int waitForNextPage)
    {
        _.Out.WriteLine().WriteLine();

        var tipsAnim = TipsAnim();

        var tt = new TextTimer(
                tipsAnim.RightX + 1,
                tipsAnim.BottomY,
                "(bkf,f=yellow)This page will automatically update in {0}(f=yellow) seconds...(rsf)",
                2,
                TimeSpan.FromSeconds(waitForNextPage * 100),
                (duration) => GetDurationText(duration))
            .Add(_!);
        tt
            .OnStop += (o, e) => tipsAnim.Stop();
        tt.Wait();
    }

    protected AnimatedText TipsAnim()
        => new AnimatedText(5, () => GetTipsFrame())
            .Add(_!);

    int _tipIndex = 0;
    static readonly string[] _tipsFrames = new string[]
    {
        "►  ",
        " ► ",
        "  ►"
    };

    string GetTipsFrame()
    {
        var tip = _tipsFrames[_tipIndex++];
        _tipIndex %= 3;
        return tip;
    }

    static string GetDurationText(TimeSpan duration)
    {
        var seconds = duration.TotalSeconds;
        var text = seconds.ToString();
        var col = "";
        if (seconds <= 5) col = "(f=darkyellow)";
        if (seconds <= 3) col = "(f=red)";
        return col + text;
    }
}
