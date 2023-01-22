namespace AnsiVtConsole.NetCore.Component.Widgets.Texts;
class WaitText : Widget<WaitText>
{
    /// <summary>
    /// text
    /// </summary>
    public Text Text => (Text)WrappedWidget!;

    /// <inheritdoc/>
    public WaitText(string text) : base(new Text(text)) { }
}
