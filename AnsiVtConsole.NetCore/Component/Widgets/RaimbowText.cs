namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// raimbow text
/// </summary>
public class RaimbowText : WidgetAbstact
{
    /// <summary>
    /// oring R of the gradient
    /// </summary>
    public int OriginR { get; private set; }

    /// <summary>
    /// oring G of the gradient
    /// </summary>
    public int OriginG { get; private set; }

    /// <summary>
    /// oring B of the gradient
    /// </summary>
    public int OriginB { get; private set; }

    /// <summary>
    /// current R of the gradient
    /// </summary>
    public int R { get; private set; }

    /// <summary>
    /// current G of the gradient
    /// </summary>
    public int G { get; private set; }

    /// <summary>
    /// current B of the gradient
    /// </summary>
    public int B { get; private set; }

    /// <summary>
    /// delta R of the gradient
    /// </summary>
    public int DR { get; private set; }

    /// <summary>
    /// delta G of the gradient
    /// </summary>
    public int DG { get; private set; }

    /// <summary>
    /// delta B of the gradient
    /// </summary>
    public int DB { get; private set; }

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
        var text = Text;
        return RenderFor(text);
    }
}
