namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// widget that has a regulart threaded update
/// </summary>
public abstract class AnimatedWidget<WidgetType, OptionsBuilderType>
        : Widget<WidgetType, OptionsBuilderType>, IAnimatedWidget
    where WidgetType : class, IAnimatedWidget
    where OptionsBuilderType : AnimatedOptionsBuilder<WidgetType>
{
    /// <summary>
    /// frames per seconds
    /// </summary>
    public double FPS { get; private set; }

    /// <summary>
    /// is running
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <summary>
    /// animated widget
    /// </summary>
    /// <param name="fps">frames per seconds</param>
    /// <param name="wrappedWidget">wrapped widget</param>
    public AnimatedWidget(
        double fps,
        IWidget? wrappedWidget = null)
        : base(wrappedWidget) => SetFPS(fps);

    /// <summary>
    /// widget at a fixed location
    /// </summary>
    /// <param name="fps">frames per seconds</param>
    /// <param name="x">cursor x</param>
    /// <param name="y">cursor y</param>
    public AnimatedWidget(
        double fps,
        int x,
        int y)
        : base(x, y)
        => SetFPS(fps);

    int _timeLapse => (int)(1d / FPS * 1000d);

    Thread? _thread;
    bool _stop;

    /// <summary>
    /// set the fps
    /// </summary>
    /// <param name="fps">fps</param>
    /// <returns>this object</returns>
    public WidgetType SetFPS(double fps)
    {
        AssertNotRunning();
        if (fps < 0)
            throw new ArgumentException("fps must be > 0");
        FPS = fps;
        return (this as WidgetType)!;
    }

    /// <inheritdoc/>
    public override WidgetType Add(IAnsiVtConsole console)
    {
        lock (console.Out.Lock)
        {
            base.Add(console);
            if (_thread is null)
                Start();
            return (this as WidgetType)!;
        }
    }

    /// <summary>
    /// starts the thread
    /// </summary>
    public WidgetType Start()
    {
        StartInit();
        IsRunning = true;
        (_thread = new(() => Run()))
            .Start();
        return (this as WidgetType)!;
    }

    /// <summary>
    /// stops the thread
    /// </summary>
    public WidgetType Stop()
    {
        _stop = true;
        _thread = null;
        return (this as WidgetType)!;
    }

    /// <summary>
    /// wait end of animation. if not running returns immediately
    /// </summary>
    public WidgetType Wait()
    {
        while (IsRunning)
            Thread.Yield();
        return (this as WidgetType)!;
    }

    /// <summary>
    /// Wait for delay milliseconds
    /// </summary>
    public WidgetType Wait(int delay)
    {
        Thread.Sleep(delay);
        return (this as WidgetType)!;
    }

    void Run()
    {
        var _end = _stop = false;
        var str = string.Empty;
        int newX;
        while (!_end && !_stop)
        {
            lock (Console!.Out.Lock)
            {
                RunOperation();

                newX = Console.Cursor.GetCursorX();
            }
            if (newX > RightX)
                Thread.Sleep(_timeLapse);

            _end = IsEnd();
        }
        IsRunning = false;
    }

    /// <summary>
    /// indicates if run must end
    /// </summary>
    /// <returns>true if must end, false otherwise</returns>
    protected abstract bool IsEnd();

    /// <summary>
    /// operation of the thread. Run in a console lock
    /// </summary>
    protected abstract void RunOperation();

    /// <summary>
    /// init at start
    /// </summary>
    protected abstract void StartInit();

    /// <summary>
    /// assert is not running
    /// </summary>
    /// <exception cref="InvalidOperationException">operation not allowed while animation is playing</exception>
    protected void AssertNotRunning()
    {
        if (IsRunning)
            throw new InvalidOperationException("operation not allowed while animation is playing");
    }
}
