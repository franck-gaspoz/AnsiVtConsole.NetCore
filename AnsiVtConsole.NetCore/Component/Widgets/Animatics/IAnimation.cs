namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// animation
/// </summary>
public interface IAnimation
{
    /// <summary>
    /// current value
    /// </summary>
    /// <typeparam name="TValue">value type</typeparam>
    /// <returns>value</returns>
    TValue? Value<TValue>()
        where TValue : class;

    /// <summary>
    /// from value
    /// </summary>
    /// <typeparam name="TValue">value type</typeparam>
    /// <returns>value</returns>
    TValue? From<TValue>()
        where TValue : class;

    /// <summary>
    /// to value
    /// </summary>
    /// <typeparam name="TValue">value type</typeparam>
    /// <returns>value</returns>
    TValue? To<TValue>()
        where TValue : class;

    /// <summary>
    /// duration
    /// </summary>
    /// <returns>value</returns>
    double Duration { get; }
}