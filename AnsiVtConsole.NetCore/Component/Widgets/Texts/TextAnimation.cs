namespace AnsiVtConsole.NetCore.Component.Widgets.Texts;
class TextAnimation : Widget<TextAnimation>
{
    /// <summary>
    /// text
    /// </summary>
    public Text Text => (Text)WrappedWidget!;

    /// <inheritdoc/>
    public TextAnimation(string text) : base(new Text(text)) { }
}
