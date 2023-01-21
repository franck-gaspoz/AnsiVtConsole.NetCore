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
    /// widgets that must be updated during animation
    /// </summary>
    public IReadOnlyList<IWidget> Widgets => _widgets;

    readonly List<IWidget> _widgets = new();

    /// <summary>
    /// add animation(s) to the timeline
    /// </summary>
    /// <param name="animations">animations</param>
    /// <returns>this object</returns>
    public TimeLine Add(params IAnimation[] animations)
    {
        _animations.AddRange(animations);
        return this;
    }

    /// <summary>
    /// add animation(s) to the timeline
    /// </summary>
    /// <param name="animations">animations</param>
    /// <returns>this object</returns>
    public TimeLine Add(AnimationGroup animations)
    {
        _animations.AddRange(animations.Animations);
        return this;
    }

    /// <summary>
    /// specifiy widgets to be updated during animation
    /// </summary>
    /// <param name="widgets">widgets</param>
    /// <returns>this object</returns>
    public TimeLine Update(params IWidget[] widgets)
    {
        _widgets.AddRange(widgets);
        return this;
    }

    /// <summary>
    /// render widgets
    /// </summary>
    /// <returns>this object</returns>
    public TimeLine Render()
    {
        foreach (var widget in _widgets)
            widget.Update();
        return this;
    }

    /// <summary>
    /// enable loop
    /// </summary>
    /// <returns>this object</returns>
    public TimeLine Loop()
    {
        IsLoop = true;
        return this;
    }

    /// <summary>
    /// enable auto reverse
    /// </summary>
    /// <returns>this object</returns>
    public TimeLine AutoReverse()
    {
        IsAutoReverse = true;
        return this;
    }

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
