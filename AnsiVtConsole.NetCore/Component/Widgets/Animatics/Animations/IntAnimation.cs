﻿using AnsiVtConsole.NetCore.Component.Widgets.Animatics.Easings;

namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics.Animations;

/// <summary>
/// int animation
/// </summary>
public sealed class IntAnimation : ValueAnimation<int>
{
    /// <inheritdoc/>
    public IntAnimation(
        int from,
        int to,
        double duration,
        int increment,
        Easing? easing = null) : base(from, to, duration, increment, easing) { }

    /// <inheritdoc/>
    public IntAnimation(
        int to,
        double duration,
        int increment,
        Easing? easing = null) : base(to, duration, increment, easing) { }

    /// <inheritdoc/>
    public override void SetValueAt(double position) => SetValue((int)Math.Min(
            To,
            (To - From)
                * (position / Duration)));
}
