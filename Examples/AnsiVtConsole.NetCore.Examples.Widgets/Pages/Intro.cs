namespace AnsiVtConsole.NetCore.Examples.Widgets.Pages;

sealed class Intro : DemoPage
{
    public override void Run()
    {
        TypeWriter("Welcome to the demonstration program of AnsiVtCore.NetCore")
            .Add(_)
            .Wait()
            .Wait(500);

        _.Out.WriteLine();

        TypeWrite("This program show examples of using (bon,b=green,f=white)WIDGETS components(tdoff)");

        _.Out.WriteLine();

        TypeWrite("Widgets are elements drawn on console with:(br)");

        _.Out.WriteLine();

        var dot = "(f=yellow)►(f=white)";
        TypeWrite($"  {dot} live update");
        TypeWrite($"  {dot} ANSI markup and sequences");
        TypeWrite($"  {dot} animations");
        TypeWrite($"  {dot} functionalities");

        _.Out.WriteLine();

        var remCol = "(tdoff,f=darkcyan)";
        var higCol = "(uon,bon,f=magenta)";
        TypeWrite($"{remCol}░ Widgets are composable together and the {higCol}Animator{remCol} class can animate any of their properties");
        TypeWrite($"{remCol}░ Widgets mecanism is {higCol}thread safe{remCol} thus several widgets can play simultaneously");

        _.Out.WriteLine().WriteLine();

        TypeWrite("░ this demo will auto play, juste have a seat and watch. Let's go now");
    }
}
