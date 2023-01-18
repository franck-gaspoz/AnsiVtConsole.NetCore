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

    double _fps = 2;
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
        IsRunning = true;
        return this;
    }

    /// <summary>
    /// starts animation
    /// </summary>
    /// <returns>this object</returns>
    public Animation Stop()
    {
        IsRunning = false;
        return this;
    }
}
