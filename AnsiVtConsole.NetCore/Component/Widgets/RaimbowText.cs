namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// raimbow text
/// </summary>
public class RaimbowText : WidgetAbstact
{
    /// <summary>
    /// text of the raimbow text
    /// </summary>
    public string? Text { get; private set; }

    /// <summary>
    /// raimbow text
    /// </summary>
    /// <param name="text">text</param>
    public RaimbowText(string text)
        => Text = text;

    /// <summary>
    /// raimbow text embeding a widget
    /// </summary>
    /// <param name="wrappedWidget">wrapped widget</param>
    public RaimbowText(WidgetAbstact wrappedWidget)
        : base(wrappedWidget) { }

    /// <summary>
    /// raimbow text at a fixed location
    /// </summary>
    /// <param name="text">text</param>
    /// <param name="x">x</param>
    /// <param name="y">y</param>
    public RaimbowText(string text, int x, int y)
        : base(x, y)
            => Text = text;

    /// <inheritdoc/>
    public override string Render()
    {
        if (WrappedWidget is not null)
            return base.Render();
        return Text!;
    }
}
