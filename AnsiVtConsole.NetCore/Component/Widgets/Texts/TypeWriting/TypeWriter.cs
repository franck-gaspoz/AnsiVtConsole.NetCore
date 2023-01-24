namespace AnsiVtConsole.NetCore.Component.Widgets.Texts.TypeWriting;

/// <summary>
/// type writer
/// </summary>
public sealed class TypeWriter : AnimatedWidget<TypeWriter, TypeWriterOptionsBuilder>
{
    /// <summary>
    /// text
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// cursor
    /// </summary>
    public string? Cursor { get; private set; }

    int _charIndex = 0;
    char[]? _chars;
    string _str = string.Empty;
    string _rawStr = string.Empty;

    /// <summary>
    /// type writer
    /// </summary>
    /// <param name="text">text</param>
    /// <param name="fps">frames per second</param>
    /// <param name="cursor">cursor</param>
    public TypeWriter(
        string text,
        double fps,
        string? cursor = null)
        : base(fps, new Text(string.Empty))
    {
        Cursor = cursor;
        Value = text;
    }

    /// <inheritdoc/>
    protected override string RenderWidget(string render)
        => render;

    /// <inheritdoc/>
    protected override void StartInit()
    {
        _str = string.Empty;
        _rawStr = Console!.Out.GetPrint(Value, false);
        _chars = _rawStr.ToCharArray();
        _charIndex = 0;
    }

    /// <inheritdoc/>
    protected override bool IsEnd() => _charIndex >= _rawStr.Length;

    /// <inheritdoc/>
    protected override void RunOperation()
    {
        _str += _chars![_charIndex++];

        base.SetText(_str
            + (_charIndex < _rawStr.Length ?
                GetCursor()
                : string.Empty));
    }

    /// <summary>
    /// set the cursor
    /// </summary>
    /// <param name="cursor">cursor</param>
    /// <returns>this object</returns>
    public TypeWriter SetCursor(string cursor)
    {
        AssertNotRunning();
        Cursor = cursor;
        return this;
    }

    /// <summary>
    /// set the text
    /// </summary>
    /// <param name="text">text</param>
    /// <returns>this object</returns>
    public new TypeWriter SetText(string text)
    {
        AssertNotRunning();
        Value = text;
        base.SetText(string.Empty);
        return this;
    }

    string? GetCursor()
        => Cursor?.ToString();
}
