namespace AnsiVtConsole.NetCore.Component.Widgets.Texts;
class WaitText : Widget<WaitText>
{
    /// <inheritdoc/>
    public WaitText(string text) : base(new Text(text)) { }
}
