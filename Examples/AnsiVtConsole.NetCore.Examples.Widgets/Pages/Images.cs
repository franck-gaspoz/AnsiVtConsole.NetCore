using AnsiVtConsole.NetCore.Imaging.Component.Widgets.Images;

namespace AnsiVtConsole.NetCore.Examples.Widgets.Pages;

sealed class Images : DemoPage
{
    public override void Run()
    {
        SubTitle("Images with AnsiVtConsole.NetCore.Imaging(br)");

        var img1 = new Image("assets/smiley.png", 32, 16, false, (x, y, c) => "☻")
            .Add(_);

        _.Out.SetCursorPos(img1.Width!.Value, img1.Y);

        var img2 = new Image("assets/smiley.png", 16, 16)
            .Add(_);
    }
}
