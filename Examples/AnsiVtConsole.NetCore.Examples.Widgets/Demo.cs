using AnsiVtConsole.NetCore.Examples.Widgets.Pages;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;

namespace AnsiVtConsole.NetCore.Examples.Widgets;

sealed class Demo : DemoPage
{
    readonly Title _title;

    readonly List<(DemoPage demo, bool waitForNextPage)> _demos = new();

    readonly bool _isAutomatic = true;
    int _cursorTop;

    public Demo()
    {
        _title = new Title();
        _demos.AddRange(new List<(DemoPage, bool)>()
        {
            (_title,true),
            (new Images(),true)
        });
    }

    public override void Run()
    {
        _.Out.ClearScreen();

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

    void WaitBeforeNextPage(bool waitForNextPage = true)
    {
        _.Out.Write(DECTCEMShow);
        if (waitForNextPage)
            WaitPage();
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
