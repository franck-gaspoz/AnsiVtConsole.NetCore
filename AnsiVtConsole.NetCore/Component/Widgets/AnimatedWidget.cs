﻿namespace AnsiVtConsole.NetCore.Component.Widgets;

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
    /// on start event
    /// </summary>
    public event EventHandler OnStart;

    /// <summary>
    /// on stop event
    /// </summary>
    public event EventHandler OnStop;

    /// <summary>
    /// tick coutn
    /// </summary>
    public long Tick { get; private set; }

    /// <summary>
    /// animated widget
    /// </summary>
    /// <param name="fps">frames per seconds</param>
    /// <param name="wrappedWidget">wrapped widget</param>
#pragma warning disable CS8618
    public AnimatedWidget(
        double fps,
        IWidget? wrappedWidget = null)
        : base(wrappedWidget) => SetFPS(fps);

    /// <summary>
    /// animated widget
    /// </summary>
    /// <param name="x">cursor x</param>
    /// <param name="y">cursor y</param>
    /// <param name="fps">frames per seconds</param>
    /// <param name="wrappedWidget">wrapped widget</param>
#pragma warning disable CS8618
    public AnimatedWidget(
        int x,
        int y,
        double fps,
        IWidget? wrappedWidget = null)
        : base(x, y, wrappedWidget) => SetFPS(fps);

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
#pragma warning restore CS8618

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
        }
        WaitNextTick();
        return (this as WidgetType)!;
    }

    /// <summary>
    /// starts the thread
    /// </summary>
    public WidgetType Start()
    {
        Tick = 0;
        StartInit();
        IsRunning = true;
        (_thread = new(() => Run()))
            .Start();
        OnStart?.Invoke(this, EventArgs.Empty);
        return (this as WidgetType)!;
    }

    /// <summary>
    /// wait the next animation tick
    /// </summary>
    public void WaitNextTick()
    {
        if (!IsRunning) return;
        var tick = Tick;
        while (Tick == tick)
            Thread.Yield();
    }

    /// <summary>
    /// stops the thread
    /// </summary>
    public WidgetType Stop()
    {
        _stop = true;
        _thread = null;
        OnStop?.Invoke(this, EventArgs.Empty);
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
        while (!_end && !_stop)
        {
            var oldRightX = RightX;

            lock (Console!.Out.Lock)
            {
                PrepareFrame();
                RenderFrame();
            }

            if (RightX > oldRightX)
            {
                Thread.Sleep(_timeLapse);
                Tick++;
            }

            _end = IsEnd();
        }
        IsRunning = false;
        OnStop?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// indicates if run must end
    /// </summary>
    /// <returns>true if must end, false otherwise</returns>
    protected abstract bool IsEnd();

    /// <summary>
    /// operation of the thread (step 1). Run in a console lock
    /// </summary>
    protected virtual void PrepareFrame() { }

    /// <summary>
    /// operation of the thread (step 2). Run in a console lock
    /// </summary>
    protected abstract void RenderFrame();

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
