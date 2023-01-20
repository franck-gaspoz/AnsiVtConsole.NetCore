namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// animation
/// <para>run concurrent values animations grouped in time lines</para>
/// </summary>
public sealed class Animation
{
    /// <summary>
    /// animatables
    /// </summary>
    public IReadOnlyList<TimeLine> TimeLines
        => _timeLines;

    readonly List<TimeLine> _timeLines = new();

    double _fps = 20;
    /// <summary>
    /// frame per seconds
    /// </summary>
    public double Fps
    {
        get => _fps;
        set
        {
            if (IsRunning)
                throw new InvalidOperationException("can't change IsRunning when running");
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
    public Animation() => _animator = new(this);

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
