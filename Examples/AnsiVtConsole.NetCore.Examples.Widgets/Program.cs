using System.Reflection;

using AnsiVtConsole.NetCore;
using AnsiVtConsole.NetCore.Component.Widgets.Animatics;
using AnsiVtConsole.NetCore.Component.Widgets.Animatics.Animations;
using AnsiVtConsole.NetCore.Component.Widgets.Text.Raimbow;
using AnsiVtConsole.NetCore.Imaging.Component.Widgets.Images;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;

var console = new AnsiVtConsole.NetCore.AnsiVtConsole();

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

Title(console); WaitKey(console);

Images(console); WaitKey(console, false);

console.Exit();

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

void Title(IAnsiVtConsole console)
{
    var str = @"
   ___            _ __   __ _     ___                      _              _  _       _     ___                  
  /   \ _ _   ___(_)\ \ / /| |_  / __| ___  _ _   ___ ___ | | ___        | \| | ___ | |_  / __| ___  _ _  ___   
  | - || ' \ (_-/| | \   / |  _|| (__ / _ \| ' \ (_-// _ \| |/ -_)  _    | .  |/ -_)|  _|| (__ / _ \| '_|/ -_)  
  |_|_||_||_|/__/|_|  \_/   \__| \___|\___/|_||_|/__/\___/|_|\___| (_)   |_|\_|\___| \__| \___|\___/|_|  \___|  
";

    RaimbowText RaimbowText(string str)
        => new RaimbowText(str)
            .Origin(0, 0, 128)
            .CyclicGradient(4, 9, 14)
            .Add(console);

    var title = RaimbowText(str);

    RaimbowText($"  AnsiVtConsole.NetCore v{Assembly.GetExecutingAssembly().GetName().Version}");

    console.Out.WriteLine();

    var bar = RaimbowText("".PadLeft(113, '─'));

    console.Out.WriteLine();
    console.Out.WriteLine();

    var anims =
        new AnimationGroup(
            new IntAnimation(0, 255, 2000d)
                .For(() => bar.OriginRGB.R),
            new IntAnimation(0, 255, 2000d)
                .For(() => bar.OriginRGB.G),
            new IntAnimation(128, 255, 2000d)
                .For(() => bar.OriginRGB.B))
         .Target(bar.OriginRGB);

    var anim = new Animation()
        .Add(
            new TimeLine()
                .Loop()
                .AutoReverse()
                .Add(anims)
                .Update(bar)
            )
        .Start();
}

void Images(IAnsiVtConsole console)
{
    console.Out.WriteLine("Images with AnsiVtConsole.NetCore.Imaging(br)");

    var img1 = new Image("assets/smiley.png", 32, 16, false, (x, y, c) => "☻")
        .Add(console);

    console.Out.SetCursorPos(img1.Width!.Value, img1.Y);

    var img2 = new Image("assets/smiley.png", 16, 16)
        .Add(console);
}

void WaitKey(IAnsiVtConsole console, bool hasNextPage = true)
{
    var cursorTop = console.Out.CursorTop;
    console.Out.Write(DECTCEMShow);
    console.Inp.WaitKeyPress();
    if (hasNextPage)
        NextPage(console, cursorTop);
}

void NextPage(IAnsiVtConsole console, int cursorTop)
{
    console.Out.CursorTop = cursorTop;
    console.Out.Write(ED(0) + CUP(1, cursorTop + 1) + DECTCEMShow);
}
