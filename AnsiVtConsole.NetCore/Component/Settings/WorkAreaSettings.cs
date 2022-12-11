using AnsiVtConsole.NetCore.Component.UI;

namespace AnsiVtConsole.NetCore.Component.Settings;

/// <summary>
/// work area settings
/// </summary>
public class WorkAreaSettings
{
    public EventHandler? ViewSizeChanged { get; set; }

    public EventHandler<WorkAreaScrollEventArgs>? WorkAreaScrolled { get; set; }

    public bool EnableConstraintConsolePrintInsideWorkArea { get; set; } = false;

    public bool RedrawUIElementsEnabled = true;
}
