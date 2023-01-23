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

    /// <summary>
    /// text changed event
    /// </summary>
    public event EventHandler TextChanged;

    /// <summary>
    /// widget text
    /// </summary>
    /// <param name="text">text</param>
    public Text(string text)
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
