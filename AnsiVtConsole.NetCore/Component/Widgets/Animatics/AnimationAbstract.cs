namespace AnsiVtConsole.NetCore.Component.Widgets.Animatics;

/// <summary>
/// animation abstract
/// </summary>
public class AnimationAbstract<T>
    where T : class
{
    /// <summary>
    /// start of animation
    /// </summary>
    /// <returns>this object</returns>
    public T From() => (this as T)!;

    /// <summary>
    /// start of animation
    /// </summary>
    /// <returns>this object</returns>
    public T To() => (this as T)!;

    /// <summary>
    /// time lapse
    /// </summary>
    /// <returns>this object</returns>
    public T TimeLapse() => (this as T)!;

}
