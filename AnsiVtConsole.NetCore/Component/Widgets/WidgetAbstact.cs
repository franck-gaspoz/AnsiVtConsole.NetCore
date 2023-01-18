using static AnsiVtConsole.NetCore.Component.Console.ANSI;

namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// widget base class
/// </summary>
public abstract class WidgetAbstact<T> : IWidgetAbstact
    where T : class, IWidgetAbstact
{
    /// <summary>
    /// backup colors markup
    /// </summary>
    protected const string BackupColors = "(bkf,bkb)";

    /// <summary>
    /// restore colors markup
    /// </summary>
    protected const string RestoreColors = "(rsf,rsb)";

    static readonly object _lock = new();

    /// <inheritdoc/>
    public int X { get; protected set; } = -1;

    /// <inheritdoc/>
    public int Y { get; protected set; }

    /// <summary>
    /// wrapped widget
    /// </summary>
    public IWidgetAbstact? WrappedWidget { get; private set; }

    /// <summary>
    /// widget
    /// </summary>
    public WidgetAbstact(IWidgetAbstact? wrappedWidget = null)
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
    /// <param name="console">the console to render to</param>
    /// <param name="render">any render when no wrapped widget</param>
    /// <returns>the render of the widget</returns>
    protected string RenderFor(IAnsiVtConsole console, string? render = null)
        => ManagedRender(WrappedWidget is not null
            ? WrappedWidget.Render(console)
            : render ?? string.Empty);

    /// <inheritdoc/>
    public string Render(IAnsiVtConsole console)
    {
        lock (_lock)
        {
            var render = WrappedWidget is null ?
                RenderWidget()
                : WrappedWidget.Render(console);
            return ManagedRender(render);
        }
    }

    /// <summary>
    /// render the widget itself without context or wrapping considerations
    /// </summary>
    /// <returns>the render of the widget itself</returns>
    protected abstract string RenderWidget();

    /// <summary>
    /// render for any widget
    /// </summary>
    /// <param name="render">a widget render to be finalized</param>
    /// <returns>rendered widget</returns>
    protected string ManagedRender(string render)
    {
        render = WidgetAbstact<T>.BackupColors + render;
        if (X != -1 && Y != -1)
            render = CUP(X, Y) + render;
        render += WidgetAbstact<T>.RestoreColors;
        return render;
    }

    /// <summary>
    /// add the widget to a console, so it is rendered
    /// </summary>
    /// <param name="console">console</param>
    /// <returns>this object</returns>
    public T Add(IAnsiVtConsole console)
    {
        lock (_lock)
        {
            console.Out.WriteLine(Render(console));
            return (this as T)!;
        }
    }

    /// <summary>
    /// set location
    /// </summary>
    /// <param name="x">cursor x</param>
    /// <param name="y">cursor y</param>
    /// <returns>this object</returns>
    public T Location(int x, int y)
    {
        X = x;
        Y = y;
        return (this as T)!;
    }
}
