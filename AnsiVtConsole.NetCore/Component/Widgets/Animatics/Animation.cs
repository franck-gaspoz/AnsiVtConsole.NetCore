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
        => _timeeLines;

    readonly List<TimeLine> _timeeLines = new();

    /// <summary>
    /// add a time line to the animation
    /// </summary>
    /// <param name="timeline">time line</param>
    /// <returns>this object</returns>
    public Animation Add(TimeLine timeline)
    {
        _timeeLines.Add(timeline);
        return this;
    }
}
