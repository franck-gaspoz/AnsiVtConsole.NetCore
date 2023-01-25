namespace AnsiVtConsole.NetCore.Component.Widgets.Texts;

/// <summary>
/// animated text
/// </summary>
public sealed class AnimatedText : AnimatedWidget<AnimatedText, AnimatedOptionsBuilder<AnimatedText>>
{
    readonly Func<string> _frameContent;

    /// <summary>
    /// animated text
    /// </summary>    
    /// <param name="fps">fps</param>
    /// <param name="frameContent">func that gives each frame content</param>
    public AnimatedText(
        double fps,
        Func<string> frameContent)
        : base(fps, new Text(string.Empty)) => _frameContent = frameContent;

    /// <summary>
    /// animated text
    /// </summary>
    /// <param name="x">cursor x</param>
    /// <param name="y">cursor y</param>
    /// <param name="fps">fps</param>
    /// <param name="frameContent">func that gives each frame content</param>
    public AnimatedText(
        int x,
        int y,
        double fps,
        Func<string> frameContent)
        : base(x, y, fps, new Text(string.Empty))
            => _frameContent = frameContent;

    /// <inheritdoc/>
    protected override bool IsEnd() => false;

    /// <inheritdoc/>
    protected override void RenderFrame()
        => SetText(_frameContent());

    /// <inheritdoc/>
    protected override string RenderWidget(string render)
        => render;

    /// <inheritdoc/>
    protected override void StartInit() { }
}
