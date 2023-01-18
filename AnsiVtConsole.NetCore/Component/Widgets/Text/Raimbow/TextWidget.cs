namespace AnsiVtConsole.NetCore.Component.Widgets.Text.Raimbow;

/// <summary>
/// text widget
/// </summary>
public abstract class TextWidget : WidgetAbstact<RaimbowText>
{
    /// <summary>
    /// text
    /// </summary>
    public string? Text { get; private set; }

    /// <summary>
    /// widget text
    /// </summary>
    /// <param name="text">text</param>
    public TextWidget(string text)
        => Text = text;

    /// <inheritdoc/>
    public TextWidget(IWidgetAbstact wrappedWidget)
        : base(wrappedWidget) { }
}
