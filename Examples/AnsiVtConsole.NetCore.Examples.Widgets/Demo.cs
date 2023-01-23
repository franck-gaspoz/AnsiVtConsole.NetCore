using AnsiVtConsole.NetCore.Examples.Widgets.Pages;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;

namespace AnsiVtConsole.NetCore.Examples.Widgets;

sealed class Demo : DemoPage
{
    readonly Title _title;

    readonly List<(DemoPage demo, int waitForNextPage)> _demos = new();

    readonly bool _isAutomatic = true;
    int _cursorTop;
    const int DefWait = 5;

    public Demo()
    {
        _title = new Title();
        _demos.AddRange(new List<(DemoPage, int)>()
        {
            (_title,0),
            (new Intro(),8),
            (new Images(),DefWait)
        });
    }

    public override void Run()
    {
        _.Out.ClearScreen();
        _.Out.HideCur();

        var lastDemo = _demos.Last().demo;
        do
        {
            foreach (var (demo, waitForNextPage) in _demos)
            {
                demo.Run(_);
                if (_cursorTop == 0)
                    _cursorTop = _.Out.CursorTop;

                if (_isAutomatic)
                    WaitBeforeNextPage(waitForNextPage);
                else
                    WaitKeyBeforeNextPage();
            }

            _title.Animation!.Stop();
            _.Out.CursorHome();
        }
        while (true);
    }

    void WaitBeforeNextPage(int waitForNextPage)
    {
        _.Out.Write(DECTCEMShow);
        if (waitForNextPage > 0)
            WaitPage(waitForNextPage);
        NextPage();
    }

    void WaitKeyBeforeNextPage()
    {
        _.Out.Write(DECTCEMShow);
        _.Inp.WaitKeyPress();
        NextPage();
    }

    void NextPage() => _.Out.Write(
            CUP(1, _cursorTop + 1)
            + ED(0)
            + CUP(1, _cursorTop + 1)
            + DECTCEMShow);

}
