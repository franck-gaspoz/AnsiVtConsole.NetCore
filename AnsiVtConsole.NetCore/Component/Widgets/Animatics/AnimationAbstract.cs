namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// animation of value of type T
/// </summary>
/// <typeparam name="T">value type</typeparam>
public class AnimationAbstract<T> : IAnimation
    where T : class
{
    /// <summary>
    /// current value
    /// </summary>
    public T? Value { get; protected set; }

    /// <summary>
    /// from
    /// </summary>
    public T? From { get; protected set; }

    /// <summary>
    /// to
    /// </summary>
    public T? To { get; protected set; }

    /// <inheritdoc/>
    public double Duration { get; protected set; }

    /// <inheritdoc/>
    TValue? IAnimation.Value<TValue>()
        where TValue : class
            => Value as TValue;

    /// <inheritdoc/>
    TValue? IAnimation.From<TValue>()
        where TValue : class
            => From as TValue;

    /// <inheritdoc/>
    TValue? IAnimation.To<TValue>()
        where TValue : class
            => To as TValue;

    /// <inheritdoc/>
    double IAnimation.Duration => Duration;

}
