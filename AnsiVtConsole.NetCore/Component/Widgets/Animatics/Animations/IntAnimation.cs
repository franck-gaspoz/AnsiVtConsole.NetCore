using AnsiVtConsole.NetCore.Component.Widgets.Animatics.Easings;

namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics.Animations;

/// <summary>
/// int animation
/// </summary>
public class IntAnimation : ValueAnimation<int>
{
    /// <inheritdoc/>
    public IntAnimation(
        int from,
        int to,
        double duration,
        Easing? easing = null) : base(from, to, duration, easing)
    {
    }

    /// <inheritdoc/>
    public IntAnimation(
        int to,
        double duration,
        Easing? easing = null) : base(to, duration, easing)
    {
    }

    /// <inheritdoc/>
    internal override ValueAnimation<int> SetValue(double position)
        => SetValue(Math.Max(To, (From - To) * position));
}
