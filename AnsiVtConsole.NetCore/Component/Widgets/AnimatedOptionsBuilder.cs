namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// generic option builder
/// </summary>
/// <typeparam name="T">type of builded widget</typeparam>
public class AnimatedOptionsBuilder<T> : OptionsBuilder<T>
    where T : class, IAnimatedWidget
{
    /// <summary>
    /// type writer options builder
    /// </summary>
    /// <param name="widget">widget</param>
    public AnimatedOptionsBuilder(T widget)
        : base(widget) { }

    /// <summary>
    /// fps
    /// </summary>
    /// <param name="fps">fps</param>
    /// <returns>this object</returns>
    public AnimatedOptionsBuilder<T> Fps(double fps)
    {
        var o = Widget as AnimatedWidget<T, AnimatedOptionsBuilder<T>>;
        o.SetFPS(fps);
        return this;
    }
}
