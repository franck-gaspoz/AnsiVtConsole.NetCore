namespace AnsiVtConsole.NetCore.Component.Widgets.Texts.TypeWriting;

/// <summary>
/// type wrtier
/// </summary>
public sealed class TypeWriter : AnimatedWidget<TypeWriter>
{
    /// <summary>
    /// text
    /// </summary>
    public Text Text => (Text)WrappedWidget!;

    /// <summary>
    /// text
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// cursor
    /// </summary>
    public string? Cursor { get; private set; }

    readonly Lazy<TypeWriterOptionsBuilder> _optionsBuilder = new();

    int _charIndex = 0;
    char[]? _chars;
    string _str = string.Empty;
    string _rawStr = string.Empty;

    /// <summary>
    /// type writer
    /// </summary>
    /// <param name="text">text</param>
    /// <param name="cps">charcacters per seconds</param>
    /// <param name="cursor">cursor</param>
    public TypeWriter(
        string text,
        double cps,
        string? cursor = null)
        : base(new Text(string.Empty))
    {
        SetFPS(cps);
        Cursor = cursor;
        Value = text;
    }

    /// <summary>
    /// get options builder
    /// </summary>
    /// <returns></returns>
    public TypeWriterOptionsBuilder Options()
        => _optionsBuilder.Value;

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

        Text.Value = _str
            + (_charIndex < _rawStr.Length ?
                GetCursor()
                : string.Empty);
    }

    /// <summary>
    /// set the cps
    /// </summary>
    /// <param name="cps">cps</param>
    /// <returns>this object</returns>
    public TypeWriter SetCps(double cps)
    {
        AssertNotRunning();
        SetFPS(cps);
        return this;
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
    public TypeWriter SetText(string text)
    {
        AssertNotRunning();
        Value = text;
        Text.Value = string.Empty;
        return this;
    }


    string? GetCursor()
        => Cursor?.ToString();
}
