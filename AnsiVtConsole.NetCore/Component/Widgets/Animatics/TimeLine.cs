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
}
