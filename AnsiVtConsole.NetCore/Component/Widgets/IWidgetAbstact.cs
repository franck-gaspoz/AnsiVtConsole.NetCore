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
    /// render
    /// </summary>
    /// <returns>the widget render</returns>
    string Render();
}