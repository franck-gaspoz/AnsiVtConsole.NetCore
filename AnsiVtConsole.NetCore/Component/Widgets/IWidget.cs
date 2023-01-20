namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// widget abstract
/// </summary>
public interface IWidget
{
    /// <summary>
    /// wrapped widget
    /// </summary>
    IWidget? WrappedWidget { get; }

    /// <summary>
    /// fixed location X if any else -1
    /// </summary>
    int X { get; }

    /// <summary>
    /// fixed location Y if any else -1
    /// </summary>
    int Y { get; }

    /// <summary>
    /// render the widget
    /// </summary>
    /// <param name="console">the console to render to</param>
    /// <returns>the render of the widget</returns>
    string Render(IAnsiVtConsole console);

    /// <summary>
    /// update the display of the widget previously attached to a console (already rendered)
    /// </summary>
    void Update(bool shouldHideCursor = true);
}