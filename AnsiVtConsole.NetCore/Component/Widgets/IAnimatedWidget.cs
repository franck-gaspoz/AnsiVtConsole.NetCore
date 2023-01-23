namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// widget that has a regulart threaded update
/// </summary>
public interface IAnimatedWidget : IWidget
{
    /// <summary>
    /// frames per seconds
    /// </summary>
    double FPS { get; }

    /// <summary>
    /// is running
    /// </summary>
    bool IsRunning { get; }
}