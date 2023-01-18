using AnsiVtConsole.NetCore.Component.Widgets.Text.Raimbow;

namespace AnsiVtConsole.NetCore.Component.Widgets.Bar;

/// <summary>
/// animatable raimbow bar
/// </summary>
public sealed class RaimbowBar : RaimbowText
{
    /// <inheritdoc/>
    public RaimbowBar(string text) : base(text) { }

    /// <inheritdoc/>
    public RaimbowBar(IWidgetAbstact wrappedWidget) : base(wrappedWidget) { }

    /// <inheritdoc/>
    protected override string RenderWidget() => string.Empty;
}
