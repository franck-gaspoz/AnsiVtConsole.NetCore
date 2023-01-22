using AnsiVtConsole.NetCore.Component.Widgets.Texts.Raimbows;

namespace AnsiVtConsole.NetCore.Component.Widgets.Bars;

/// <summary>
/// animatable raimbow bar
/// </summary>
public sealed class RaimbowBar : Widget<RaimbowBar>
{
    /// <summary>
    /// bar char
    /// </summary>
    public char BarChar { get; private set; } = DefaultBarChar;

    /// <summary>
    /// raimbow
    /// </summary>
    public Raimbow Raimbow => (Raimbow)WrappedWidget!;

    const char DefaultBarChar = '─';

    /// <summary>
    /// length
    /// </summary>
    public int Length { get; private set; }

    /// <inheritdoc/>
    public RaimbowBar(int length, char? barChar = null)
        : base(new Raimbow(GetBarText(length, barChar ?? DefaultBarChar)))
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
