using System.Reflection;

using AnsiVtConsole.NetCore.Component.Widgets.Animatics;
using AnsiVtConsole.NetCore.Component.Widgets.Animatics.Animations;
using AnsiVtConsole.NetCore.Component.Widgets.Bars;
using AnsiVtConsole.NetCore.Component.Widgets.Texts.Coloring;

namespace AnsiVtConsole.NetCore.Examples.Widgets.Pages;

sealed class Title : DemoPage
{
    public Animation? Animation { get; private set; }

    public override void Run()
    {
        var str = @"
   ___            _ __   __ _     ___                      _              _  _       _     ___                  
  /   \ _ _   ___(_)\ \ / /| |_  / __| ___  _ _   ___ ___ | | ___        | \| | ___ | |_  / __| ___  _ _  ___   
  | - || ' \ (_-/| | \   / |  _|| (__ / _ \| ' \ (_-// _ \| |/ -_)  _    | .  |/ -_)|  _|| (__ / _ \| '_|/ -_)  
  |_|_||_||_|/__/|_|  \_/   \__| \___|\___/|_||_|/__/\___/|_|\___| (_)   |_|\_|\___| \__| \___|\___/|_|  \___|  
";
        Gradient Setup(Gradient raimbow)
            => raimbow
                .Origin(0, 0, 128)
                .CyclicGradient(4, 9, 14);

        Gradient RaimbowText(string str)
            => Setup(new Gradient(str));

        var title = RaimbowText(str).Add(_);

        RaimbowText($"  AnsiVtConsole.NetCore v{Assembly.GetExecutingAssembly().GetName().Version}").Add(_);

        _.Out.WriteLine();

        var bar = new GradientBar(113);
        Setup(bar.Gradient);
        bar.Add(_);

        _.Out.WriteLine();
        _.Out.WriteLine();

        var anims =
            new AnimationGroup(
                new IntAnimation(0, 255, 2000d)
                    .For(() => bar.Gradient.OriginRGB.R),
                new IntAnimation(0, 255, 2000d)
                    .For(() => bar.Gradient.OriginRGB.G),
                new IntAnimation(128, 255, 2000d)
                    .For(() => bar.Gradient.OriginRGB.B))
             .Target(bar.Gradient.OriginRGB);

        Animation = new Animation()
            .Add(
                new TimeLine()
                    .Loop()
                    .AutoReverse()
                    .Add(anims)
                    .Update(bar)
                )
            .Start();
    }
}
