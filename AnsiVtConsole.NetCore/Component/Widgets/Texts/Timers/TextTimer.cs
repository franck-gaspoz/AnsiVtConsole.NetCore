namespace AnsiVtConsole.NetCore.Component.Widgets.Texts.Timers;

/// <summary>
/// text animation
/// </summary>
public sealed class TextTimer : AnimatedWidget<TextTimer, AnimatedOptionsBuilder<TextTimer>>
{
    /// <summary>
    /// text
    /// </summary>
    public Text Text => (Text)WrappedWidget!;

    /// <summary>
    /// timer duration
    /// </summary>
    public TimeSpan Duration { get; private set; }

    /// <summary>
    /// duration to string presentation method
    /// </summary>
    public Func<TimeSpan, string> DurationToString { get; private set; }

    readonly string _pattern;
    readonly DateTime? _startTime;
    DateTime? _endTime;

    /// <summary>
    /// text timer
    /// </summary>
    /// <param name="text">text pattern. {0} indicates the duration</param>
    /// <param name="fps">frames per second</param>
    /// <param name="duration">timer duration</param>
    /// <param name="durationToString">eventually a personalized prensentation function of the duration</param>
    public TextTimer(
        string text,
        double fps,
        TimeSpan duration,
        Func<TimeSpan, string>? durationToString = null
        ) : base(fps, new Text(string.Empty))
    {
        _pattern = text;
        Duration = duration;
        DurationToString = durationToString ?? DefaultDurationToString;
    }

    static string DefaultDurationToString(TimeSpan duration)
    {
        var r = string.Empty;
        if (duration.Days > 0) r += duration.Days + " day ";
        if (duration.Hours > 0) r += duration.Hours + " h ";
        if (duration.Minutes > 0) r += duration.Minutes + " min ";
        if (duration.Seconds > 0) r += duration.Seconds + " s ";
        if (duration.Milliseconds > 0) r += duration.Milliseconds + "ms";
        return r;
    }

    /// <inheritdoc/>
    protected override bool IsEnd() => DateTime.Now > _endTime;

    /// <inheritdoc/>
    protected override void RunOperation()
    {
        var remaining = DateTime.Now >= _endTime ? TimeSpan.FromSeconds(0)
            : _endTime!.Value - DateTime.Now;
        Text.Value = string.Format(_pattern, DurationToString(remaining));
    }

    /// <inheritdoc/>
    protected override void StartInit() => _endTime = DateTime.Now + Duration;

    /// <inheritdoc/>
    protected override string RenderWidget(string render)
        => render;
}
