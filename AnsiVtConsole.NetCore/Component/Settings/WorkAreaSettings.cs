using AnsiVtConsole.NetCore.Component.UI;

namespace AnsiVtConsole.NetCore.Component.Settings;

/// <summary>
/// work area settings
/// </summary>
public sealed class WorkAreaSettings
{
#pragma warning disable CS1591
    public EventHandler? ViewSizeChanged { get; set; }

    public EventHandler<WorkAreaScrollEventArgs>? WorkAreaScrolled { get; set; }

    public bool EnableConstraintConsolePrintInsideWorkArea { get; set; } = false;

    public bool RedrawUIElementsEnabled = true;

#pragma warning restore CS1591
}
