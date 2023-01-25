using AnsiVtConsole.NetCore.Component.Widgets.Texts.Coloring;

namespace AnsiVtConsole.NetCore.Component.Widgets.Bars;

/// <summary>
/// animatable raimbow bar
/// </summary>
public sealed class GradientBar : Widget<GradientBar, OptionsBuilder<GradientBar>>
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

    /// <summary>
    /// gradient bar
    /// </summary>
    /// <param name="length">length</param>
    /// <param name="barChar">character used to draw the bar</param>
    public GradientBar(int length, char? barChar = null)
        : base(new Gradient(GetBarText(length, barChar ?? DefaultBarChar)))
    {
        Length = length;
        BarChar = barChar ?? DefaultBarChar;
    }

    /// <summary>
    /// gradient bar
    /// </summary>
    /// <param name="x">cursor x</param>
    /// <param name="y">cursor y</param>
    /// <param name="length">length</param>
    /// <param name="barChar">character used to draw the bar</param>
    public GradientBar(int x, int y, int length, char? barChar = null)
        : base(
            x,
            y,
            new Gradient(GetBarText(length, barChar ?? DefaultBarChar)))
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
