using AnsiVtConsole.NetCore.Component.Widgets.Texts.Coloring;

namespace AnsiVtConsole.NetCore.Component.Widgets.Bars;

/// <summary>
/// animatable raimbow bar
/// </summary>
public sealed class GardientBar : Widget<GardientBar>
{
    /// <summary>
    /// bar char
    /// </summary>
    public char BarChar { get; private set; } = DefaultBarChar;

    /// <summary>
    /// raimbow
    /// </summary>
    public Gradient Gradient => (Gradient)WrappedWidget!;

    const char DefaultBarChar = '─';

    /// <summary>
    /// length
    /// </summary>
    public int Length { get; private set; }

    /// <inheritdoc/>
    public GardientBar(int length, char? barChar = null)
        : base(new Gradient(GetBarText(length, barChar ?? DefaultBarChar)))
    {
        Length = length;
        BarChar = barChar ?? DefaultBarChar;
    }

    static string GetBarText(int length, char barChar)
        => "".PadLeft(length, barChar);

    /// <inheritdoc/>
    protected override string RenderWidget(string render)
        => render;
}
