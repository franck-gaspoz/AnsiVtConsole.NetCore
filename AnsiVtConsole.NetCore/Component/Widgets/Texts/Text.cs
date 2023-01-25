namespace AnsiVtConsole.NetCore.Component.Widgets.Texts;

/// <summary>
/// text widget
/// </summary>
public sealed class Text : Widget<Text, OptionsBuilder<Text>>
{
    string? _value;
    /// <summary>
    /// text
    /// </summary>
    public string Value
    {
        get => _value!;
        set
        {
            _value = value;
            TextChanged?.Invoke(this, new EventArgs());
        }
    }

    /// <inheritdoc/>
    public override void SetText(string text)
        => Value = text;

    /// <inheritdoc/>
    public override string GetText()
        => Value;

    /// <summary>
    /// text changed event
    /// </summary>
    public event EventHandler TextChanged;

    /// <summary>
    /// widget text
    /// </summary>
    /// <param name="x">cursor x</param>
    /// <param name="y">cursor y</param>
    /// <param name="text">text</param>
    public Text(int x, int y, string text) : base(x, y)
    {
        Value = text;
        TextChanged += OnTextChanged;
    }

    /// <summary>
    /// widget text
    /// </summary>
    /// <param name="text">text</param>
    public Text(string text) : base(null)
    {
        Value = text;
        TextChanged += OnTextChanged;
    }

    void OnTextChanged(object? sender, EventArgs e)
    {
        lock (Console!.Out.Lock)
        {
            Update();
        }
    }

    /// <inheritdoc/>
    protected override string RenderWidget() => Value;
}
