namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// widget that has a regulart threaded update
/// </summary>
public abstract class AnimatedWidget<T> : Widget<T>, IAnimatedWidget
    where T : class, IAnimatedWidget
{
    /// <summary>
    /// frames per seconds
    /// </summary>
    public double FPS { get; private set; }

    /// <summary>
    /// is running
    /// </summary>
    public bool IsRunning { get; private set; }

    /// <inheritdoc/>
    public AnimatedWidget(IWidget? wrappedWidget = null)
        : base(wrappedWidget) { }

    /// <inheritdoc/>
    public AnimatedWidget(int x, int y)
        : base(x, y) { }

    int _timeLapse => (int)(1d / FPS * 1000d);

    Thread? _thread;
    bool _stop;

    /// <summary>
    /// set the fps
    /// </summary>
    /// <param name="fps">fps</param>
    /// <returns>this object</returns>
    public T SetFPS(double fps)
    {
        AssertNotRunning();
        if (fps < 0)
            throw new ArgumentException("fps must be > 0");
        FPS = fps;
        return (this as T)!;
    }

    /// <inheritdoc/>
    public override T Add(IAnsiVtConsole console)
    {
        lock (console.Out.Lock)
        {
            base.Add(console);
            if (_thread is null)
                Start();
            return (this as T)!;
        }
    }

    /// <summary>
    /// starts the thread
    /// </summary>
    public T Start()
    {
        StartInit();
        IsRunning = true;
        (_thread = new(() => Run()))
            .Start();
        return (this as T)!;
    }

    /// <summary>
    /// stops the thread
    /// </summary>
    public T Stop()
    {
        _stop = true;
        _thread = null;
        return (this as T)!;
    }

    /// <summary>
    /// wait end of animation. if not running returns immediately
    /// </summary>
    public T Wait()
    {
        while (IsRunning)
            Thread.Yield();
        return (this as T)!;
    }

    /// <summary>
    /// Wait for delay milliseconds
    /// </summary>
    public T Wait(int delay)
    {
        Thread.Sleep(delay);
        return (this as T)!;
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
