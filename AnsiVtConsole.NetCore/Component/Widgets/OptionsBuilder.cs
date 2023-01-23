namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// generic option builder
/// </summary>
/// <typeparam name="T">type of builded widget</typeparam>
public class OptionsBuilder<T>
    where T : class, IWidget
{
    /// <summary>
    /// builded widget
    /// </summary>
    public T Widget { get; protected set; }

    /// <summary>
    /// type writer options builder
    /// </summary>
    /// <param name="widget">widget</param>
    public OptionsBuilder(T widget)
        => Widget = widget;

    /// <summary>
    /// build the configured object
    /// </summary>
    /// <returns>the configured object</returns>
    public T Build() => Widget;
}
