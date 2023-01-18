namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// story board
/// </summary>
public sealed class TimeLine<T>
    where T : class
{
    /// <summary>
    /// animatables
    /// </summary>
    public IReadOnlyList<IAnimation> Animatables
        => _animatables;

    readonly List<IAnimation> _animatables = new();

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
    public TimeLine<T> Add(IAnimation animatable)
    {
        _animatables.Add(animatable);
        return this;
    }

    /// <summary>
    /// enable loop
    /// </summary>
    /// <returns>this object</returns>
    public TimeLine<T> Loop()
        => (this as TimeLine<T>)!;

    /// <summary>
    /// enable auto reverse
    /// </summary>
    /// <returns>this object</returns>
    public TimeLine<T> AutoReverse()
        => (this as TimeLine<T>)!;

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
}
