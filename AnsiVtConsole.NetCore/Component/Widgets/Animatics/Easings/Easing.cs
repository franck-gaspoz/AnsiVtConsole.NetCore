namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics.Easings;

/// <summary>
/// easing function
/// </summary>
public abstract class Easing
{
    /// <summary>
    /// get the time line position ratio from begin to end according to the easing function
    /// </summary>
    /// <param name="animation">animation</param>
    /// <param name="progress">progress</param>
    /// <param name="fps">fps</param>
    /// <returns>the value</returns>
    public abstract double GetPosition<T>(
       ValueAnimation<T> animation,
        double progress,
        double fps);
}
