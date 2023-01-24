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
    /// set the text of the deepest wrapped widget in the hierarchy
    /// </summary>
    /// <param name="text">text</param>
    void SetText(string text);

    /// <summary>
    /// get the text of the deepest wrapped widget in the hierarchy
    /// </summary>
    /// <returns></returns>
    string GetText();

    /// <summary>
    /// fixed location X if any else location when rendered (origin 0)
    /// </summary>
    int X { get; }

    /// <summary>
    /// fixed location Y if any else else location when rendered (origin 0)
    /// </summary>
    int Y { get; }

    /// <summary>
    /// right location X after rendering (origin 0) else -1
    /// </summary>
    int RightX { get; }

    /// <summary>
    /// bottom location Y after rendering (origin 0) else -1
    /// </summary>
    int BottomY { get; }

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

    /// <summary>
    /// set parent widget
    /// </summary>
    /// <param name="parent">parent</param>
    void SetParent(IWidget parent);
}