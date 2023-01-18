namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// widget abstract
/// </summary>
public interface IWidgetAbstact
{
    /// <summary>
    /// wrapped widget
    /// </summary>
    IWidgetAbstact? WrappedWidget { get; }

    /// <summary>
    /// cursor x
    /// </summary>
    int X { get; }

    /// <summary>
    /// cursor y
    /// </summary>
    int Y { get; }

    /// <summary>
    /// render the widget
    /// </summary>
    /// <param name="console">the console to render to</param>
    /// <returns>the render of the widget</returns>
    string Render(IAnsiVtConsole console);
}