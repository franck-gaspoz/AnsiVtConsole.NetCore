namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// widget base class
/// </summary>
public abstract class WidgetAbstact
{
    /// <summary>
    /// fixed location X if any else -1
    /// </summary>
    public int X { get; private set; } = -1;

    /// <summary>
    /// fixed location Y if any else -1
    /// </summary>
    public int Y { get; private set; }

    /// <summary>
    /// wrapped widget if any
    /// </summary>
    public WidgetAbstact? WrappedWidget { get; private set; }

    /// <summary>
    /// widget
    /// </summary>
    public WidgetAbstact(WidgetAbstact? wrappedWidget = null)
        => WrappedWidget = wrappedWidget;

    /// <summary>
    /// widget at a fixed location
    /// </summary>
    /// <param name="x">cursor x</param>
    /// <param name="y">cursor y</param>
    public WidgetAbstact(int x, int y)
        => (X, Y) = (x, y);

    /// <summary>
    /// render the widget
    /// </summary>
    /// <param name="render">any render when no wrapped widget</param>
    /// <returns>the render of the widget</returns>
    protected string RenderFor(string? render = null)
        => ManagedRender(WrappedWidget is not null
            ? WrappedWidget.Render()
            : render ?? string.Empty);

    /// <summary>
    /// render the widget
    /// </summary>
    /// <returns>the render of the widget</returns>
    public abstract string Render();

    /// <summary>
    /// render for any widget
    /// </summary>
    /// <param name="render">a widget render to be finalized</param>
    /// <returns>rendered widget</returns>
    protected string ManagedRender(string render) => render;
}
