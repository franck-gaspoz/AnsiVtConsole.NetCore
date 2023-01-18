namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// story board
/// </summary>
public sealed class TimeLine
{
    /// <summary>
    /// animatables
    /// </summary>
    public IReadOnlyList<IAnimation> Animations
        => _animations;

    readonly List<IAnimation> _animations = new();

    /// <summary>
    /// is loop
    /// </summary>
    public bool IsLoop { get; private set; }

    /// <summary>
    /// is auto reverse
    /// </summary>
    public bool IsAutoReverse { get; private set; }

    /// <summary>
    /// add an animatable to the animation
    /// </summary>
    /// <param name="animatable">animatable</param>
    /// <returns>this object</returns>
    public TimeLine Add(IAnimation animatable)
    {
        _animations.Add(animatable);
        return this;
    }

    /// <summary>
    /// enable loop
    /// </summary>
    /// <returns>this object</returns>
    public TimeLine Loop()
        => this!;

    /// <summary>
    /// enable auto reverse
    /// </summary>
    /// <returns>this object</returns>
    public TimeLine AutoReverse()
        => this!;

    /// <summary>
    /// time line
    /// </summary>
    /// <param name="isLoop">is loop</param>
    /// <param name="isAutoReverse">is auto reverse</param>
    public TimeLine(bool isLoop = false, bool isAutoReverse = false)
    {
        IsLoop = isLoop;
        IsAutoReverse = isAutoReverse;
    }

    /// <summary>
    /// get total time line duration
    /// </summary>
    /// <returns>max duration of animations</returns>
    public double Duration
        => !Animations.Any() ? 0
            : _animations.Select(x => x.Duration).Max();
}
