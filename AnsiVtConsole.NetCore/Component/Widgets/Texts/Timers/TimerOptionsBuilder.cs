using AnsiVtConsole.NetCore.Component.Widgets.Texts.Timers;

namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// timer option builder
/// </summary>
public class TimerOptionsBuilder : OptionsBuilder<TextTimer>
{
    /// <summary>
    /// type writer options builder
    /// </summary>
    /// <param name="widget">widget</param>
    public TimerOptionsBuilder(TextTimer widget)
        : base(widget) { }
}
