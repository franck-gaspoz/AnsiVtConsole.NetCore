#define dbg

namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics.Easings;

/// <summary>
/// linear easing function
/// </summary>
public class Linear : Easing
{
    /// <inheritdoc/>
    public override double GetPosition<T>(
        ValueAnimation<T> animation,
        double progress,
        double fps)
    {
        var position = Math.Max(
            1,
            progress / animation.Duration);

#if dbg
        System.Diagnostics.Debug.WriteLine($"easing {GetType().Name} : position = {position}");
#endif
        return position;
    }
}
