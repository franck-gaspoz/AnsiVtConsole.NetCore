using static AnsiVtConsole.NetCore.Component.Console.ANSI;

namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// widget base class
/// </summary>
public abstract class Widget<WidgetType, OptionsBuilderType> : IWidget
    where WidgetType : class, IWidget
    where OptionsBuilderType : OptionsBuilder<WidgetType>
{
    /// <summary>
    /// backup colors markup
    /// </summary>
    protected const string BackupColors = "(bkf,bkb)";

    /// <summary>
    /// restore colors markup
    /// </summary>
    protected const string RestoreColors = "(rsf,rsb)";

    /// <inheritdoc/>
    public int X { get; protected set; } = -1;

    /// <inheritdoc/>
    public int Y { get; protected set; } = -1;

    /// <inheritdoc/>
    public int RightX { get; protected set; } = -1;

    /// <inheritdoc/>
    public int BottomY { get; protected set; } = -1;

    /// <summary>
    /// true if not already rendered
    /// </summary>
    bool _notRendered = true;

    /// <summary>
    /// console the widet is attached to
    /// </summary>
    public IAnsiVtConsole? Console { get; protected set; }

    /// <summary>
    /// wrapped widget
    /// </summary>
    public IWidget? WrappedWidget { get; private set; }

    /// <summary>
    /// parent widget
    /// </summary>
    public IWidget? Parent { get; private set; }

    /// <summary>
    /// options builder
    /// </summary>
    protected Lazy<OptionsBuilderType> OptionsBuilder = new();

    /// <summary>
    /// widget
    /// </summary>
    public Widget(IWidget? wrappedWidget = null)
    {
        if (wrappedWidget is not null)
        {
            WrappedWidget = wrappedWidget;
            WrappedWidget.SetParent(this);
        }
    }

    /// <inheritdoc/>
    public void SetParent(IWidget parent)
        => Parent = parent;

    /// <summary>
    /// widget at a fixed location
    /// </summary>
    /// <param name="x">cursor x</param>
    /// <param name="y">cursor y</param>
    public Widget(int x, int y)
        => (X, Y) = (x, y);

    /// <inheritdoc/>
    public string Render(IAnsiVtConsole console)
    {
        lock (console.Out.Lock)
        {
            if (Console is null)
                Console = console;
            var render = WrappedWidget is null ?
                RenderWidget()
                : RenderWidget(
                    WrappedWidget.Render(console));
            return ManagedRender(render);
        }
    }

    /// <inheritdoc/>
    public void Update(bool shouldHideCursor = true)
    {
        if (Console is null)
            throw new InvalidOperationException("widget is not attached to a console");

        lock (Console.Out.Lock)
        {
            if (Parent is not null)
                Parent.Update(shouldHideCursor);
            else
            {
                if (shouldHideCursor)
                    Console.Out.HideCur();

                var cur = Console.Out.CursorPos;
                Console.Out.SetCursorPos(X, Y);

                Output(Console, Render(Console));

                Console.Out.CursorPos = cur;
            }
        }
    }

    /// <summary>
    /// render a widget that has not wrapped widget content
    /// </summary>
    /// <returns>the render of the widget</returns>
    protected virtual string RenderWidget()
        => throw new NotImplementedException();

    /// <summary>
    /// render a widget that wraps a widget content
    /// </summary>
    /// <returns>the render of the widget</returns>
    protected virtual string RenderWidget(string render)
        => throw new NotImplementedException();

    /// <summary>
    /// render for any widget
    /// </summary>
    /// <param name="render">a widget render to be finalized</param>
    /// <returns>rendered widget</returns>
    protected string ManagedRender(string render)
    {
        if (Parent == null)
        {
            render = Widget<WidgetType, OptionsBuilderType>.BackupColors + render;
            if (X != -1 && Y != -1)
                render = CUP(X + 1, Y + 1) + render;
            render += Widget<WidgetType, OptionsBuilderType>.RestoreColors;
        }
        return render;
    }

    /// <summary>
    /// add the widget to a console, so it is rendered
    /// </summary>
    /// <param name="console">console</param>
    /// <returns>this object</returns>
    public virtual WidgetType Add(IAnsiVtConsole console)
    {
        lock (console.Out.Lock)
        {
            Console = console;

            if (Parent is not null)
                throw new InvalidOperationException("a widget that has a parent must not be added to a console. Only the root parent must be added");

            if (_notRendered)
            {
                if (X == -1)
                    X = console.Cursor.GetCursorX();
                if (Y == -1)
                    Y = console.Cursor.GetCursorY();
                _notRendered = false;
            }

            Output(console, Render(console));

            return (this as WidgetType)!;
        }
    }

    /// <summary>
    /// final render of the widget
    /// </summary>
    /// <param name="console">console</param>
    /// <param name="render">render content</param>
    void Output(IAnsiVtConsole console, string render)
    {
        console.Out.WriteLine(render);
        RightX = console.Cursor.GetCursorX() - 1;
        BottomY = console.Cursor.GetCursorY() - 1;
    }

    /// <summary>
    /// set location (no render)
    /// </summary>
    /// <param name="x">cursor x</param>
    /// <param name="y">cursor y</param>
    /// <returns>this object</returns>
    public WidgetType Location(int x, int y)
    {
        X = x;
        Y = y;
        return (this as WidgetType)!;
    }
}
