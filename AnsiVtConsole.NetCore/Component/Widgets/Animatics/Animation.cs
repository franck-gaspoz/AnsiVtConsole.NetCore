namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// animation
/// <para>run parallely values animations grouped in one time line</para>
/// <para>run sequencially times lines</para>
/// </summary>
public sealed class Animation
{
    /// <summary>
    /// animatables
    /// </summary>
    public IReadOnlyList<TimeLine> TimeLines
        => _timeLines;

    readonly List<TimeLine> _timeLines = new();

    double _fps = 10;
    /// <summary>
    /// frame per seconds
    /// </summary>
    public double Fps
    {
        get => _fps;
        set
        {
            if (IsRunning)
                throw new InvalidOperationException("can't change fps when running");
            _fps = value;
        }
    }

    /// <summary>
    /// is running
    /// </summary>
    public bool IsRunning { get; private set; }

    readonly Animator _animator;

    /// <summary>
    /// animation
    /// </summary>
    /// <param name="fps">frames per seconds (default 10)</param>
    public Animation(double? fps = null)
    {
        _animator = new(this);
        _fps = fps ?? 10;
    }

    /// <summary>
    /// add a time line to the animation
    /// </summary>
    /// <param name="timeline">time line</param>
    /// <returns>this object</returns>
    public Animation Add(TimeLine timeline)
    {
        _timeLines.Add(timeline);
        return this;
    }

    /// <summary>
    /// starts animation
    /// </summary>
    /// <returns>this object</returns>
    public Animation Start()
    {
        _animator.OnStart += OnStart;
        _animator.OnStop += OnStop;
        _animator.Start();
        return this;
    }

    void OnStart(object? o, EventArgs e) => IsRunning = true;

    /// <summary>
    /// starts animation
    /// </summary>
    /// <returns>this object</returns>
    public Animation Stop()
    {
        _animator.Stop();
        return this;
    }

    void OnStop(object? o, EventArgs e)
    {
        IsRunning = false;
        _animator.OnStart -= OnStart;
        _animator.OnStop -= OnStop;
    }

    /// <summary>
    /// wait end of animation. blocks current thread
    /// </summary>
    /// <returns>this object</returns>    
    public Animation Wait()
    {
        while (IsRunning)
        {
            Thread.Yield();
        }
        return this;
    }
}
