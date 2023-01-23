namespace AnsiVtConsole.NetCore.Component.Widgets.Texts.Timers;

/// <summary>
/// text animation
/// </summary>
class TextAnimation : AnimatedWidget<TextAnimation, OptionsBuilder<TextAnimation>>
{
    /// <summary>
    /// text
    /// </summary>
    public Text Text => (Text)WrappedWidget!;

    /// <inheritdoc/>
    public TextAnimation(string text) : base(new Text(text)) { }

    protected override bool IsEnd() => throw new NotImplementedException();
    protected override void RunOperation() => throw new NotImplementedException();
    protected override void StartInit() => throw new NotImplementedException();
}
