namespace AnsiVtConsole.NetCore.Component.Console;

/// <summary>
/// predefined colors
/// </summary>
public sealed class ColorSettings
{
    readonly IAnsiVtConsole _console;

    /// <summary>
    /// attach the settings to a console
    /// </summary>
    /// <param name="console">console</param>
    public ColorSettings(IAnsiVtConsole console) => _console = console;

    /// <summary>
    /// defaults shell foreground and background - if is setted. designed to preserve console default background transparency
    /// </summary>
    /// <returns>text color</returns>
    public TextColor Default => new(
        _console.Settings.DefaultForeground,
        _console.Settings.DefaultBackground, ANSI.RSTXTA);

    /// <summary>
    /// inverted default shell foreground and background
    /// </summary>
    public TextColor Inverted => new(
        _console.Settings.DefaultBackground,
        _console.Settings.DefaultForeground);

    // states colors

#pragma warning disable CS1591

    public TextColor Log = new(ConsoleColor.Green, null);
    public TextColor Error = new(ConsoleColor.Red, null);
    public TextColor Success = new(ConsoleColor.Green, null);
    public TextColor Warning = new(ConsoleColor.DarkYellow, null);
    public TextColor Debug = new(ConsoleColor.Green, null);

    // states as text in a box

    public TextColor BoxOk = new(ConsoleColor.White, ConsoleColor.DarkGreen);
    public TextColor BoxError = new(ConsoleColor.Yellow, ConsoleColor.Red);
    public TextColor BoxUnknown = new(ConsoleColor.Green, ConsoleColor.DarkCyan);
    public TextColor BoxNotIdentified = new(ConsoleColor.Yellow, ConsoleColor.Red);
    public TextColor Information = new(ConsoleColor.DarkCyan, null);
    public TextColor TextExtract = new(ConsoleColor.Green, null);
    public TextColor TextExtractSelectionBlock = new(ConsoleColor.Yellow, ConsoleColor.Green);

    public TextColor TaskInformation = new(ConsoleColor.Blue, null);

    // UI

    public TextColor TitleBar = new(ConsoleColor.White, ConsoleColor.DarkBlue);
    public TextColor TitleDarkText = new(ConsoleColor.Gray, ConsoleColor.DarkBlue);
    public TextColor InteractionBar = new(ConsoleColor.White, ConsoleColor.DarkBlue);
    public TextColor InteractionPanel = new(ConsoleColor.White, ConsoleColor.DarkBlue);
    public TextColor InteractionPanelCmdKeys = new(ConsoleColor.Black, ConsoleColor.White);
    public TextColor InteractionPanelDisabledCmdKeys = new(ConsoleColor.DarkGray, ConsoleColor.Black);

    public TextColor InteractionPanelCmdLabel = new(ConsoleColor.Yellow, ConsoleColor.DarkBlue);
    public TextColor InteractionPanelDisabledCmdLabel = new(ConsoleColor.DarkGray, ConsoleColor.DarkBlue);

    // system library types

    public TextColor ExceptionText = new(ConsoleColor.Red, null);
    public TextColor ExceptionName = new(ConsoleColor.Yellow, ConsoleColor.Red);

    // Shell

    //      values types

    public TextColor Null = new(ConsoleColor.Green, null);
    public TextColor Quotes = new(ConsoleColor.Green, null);
    public TextColor Numeric = new(ConsoleColor.Cyan, null);
    public TextColor Boolean = new(ConsoleColor.Magenta, null);
    public TextColor BooleanTrue = new(ConsoleColor.Magenta, null);
    public TextColor BooleanFalse = new(ConsoleColor.DarkMagenta, null);
    public TextColor Integer = new(ConsoleColor.Cyan, null);
    public TextColor Double = new(ConsoleColor.Cyan, null);
    public TextColor Float = new(ConsoleColor.Cyan, null);
    public TextColor Decimal = new(ConsoleColor.Cyan, null);
    public TextColor Char = new(ConsoleColor.Green, null);

    public TextColor Label = new(ConsoleColor.Cyan, null);
    public TextColor Name = new(ConsoleColor.DarkYellow, null);
    public TextColor HighlightSymbol = new(ConsoleColor.Yellow, null);
    public TextColor Symbol = new(ConsoleColor.DarkYellow, null);
    public TextColor Value = new(ConsoleColor.DarkYellow, null);
    public TextColor HalfDarkLabel = new(ConsoleColor.DarkCyan, null);
    public TextColor DarkLabel = new(ConsoleColor.DarkBlue, null);
    public TextColor MediumDarkLabel = new(ConsoleColor.Blue, null);
    public TextColor HighlightIdentifier = new(ConsoleColor.Green, null);

    public TextColor MarginText = new(ConsoleColor.DarkGray, null);

    //      colors by effect

    public TextColor Highlight = new(ConsoleColor.Yellow, null);
    public TextColor HalfDark = new(ConsoleColor.Gray, null);
    public TextColor Dark = new(ConsoleColor.DarkGray, null);

    //      table

    public TextColor TableBorder = new(ConsoleColor.Cyan, null);
    public TextColor TableColumnName = new(ConsoleColor.Yellow, null);

    //      syntax

    public TextColor ParameterName = new(ConsoleColor.Yellow, null);
    public TextColor KeyWord = new(ConsoleColor.Yellow, null);
    public TextColor ParameterValueType = new(ConsoleColor.DarkYellow, null);
    public TextColor OptionPrefix = new(ConsoleColor.Yellow, null);
    public TextColor OptionName = new(ConsoleColor.Yellow, null);
    public TextColor TypeName = new(ConsoleColor.DarkYellow, null);
    public TextColor OptionValue = new(ConsoleColor.DarkYellow, null);
    public TextColor SyntaxSymbol = new(ConsoleColor.Cyan, null);

#pragma warning restore CS1591

}

