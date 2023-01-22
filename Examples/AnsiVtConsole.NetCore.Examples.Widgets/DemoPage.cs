namespace AnsiVtConsole.NetCore.Examples.Widgets;

abstract class DemoPage
{
#pragma warning disable CS8618
    protected IAnsiVtConsole _;
#pragma warning restore CS8618 
    readonly int _pageDelay = 3000;

    public void Run(IAnsiVtConsole console)
    {
        _ = console;
        Run();
    }

    public abstract void Run();

    protected static void Wait(int delay) => Thread.Sleep(delay);

    protected void WaitPage()
    {
        PleaseWait();
        Thread.Sleep(_pageDelay);
    }

    protected void SubTitle(string text)
        => _!.Out.WriteLine($"(bkf,f=white){text}(rsf,br)");

    protected void PleaseWait()
        => _!.Out.WriteLine($"(bkf,f=white)next page in {_pageDelay / 1000} seconds...(rsf,br)");
}
