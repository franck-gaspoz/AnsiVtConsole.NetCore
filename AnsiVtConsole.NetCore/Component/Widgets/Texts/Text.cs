namespace AnsiVtConsole.NetCore.Component.Widgets.Texts;

/// <summary>
/// text widget
/// </summary>
public sealed class Text : Widget<Text>
{
    string _value;
    /// <summary>
    /// text
    /// </summary>
    public string Value
    {
        get => _value;
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
#pragma warning disable CS8618
    public Text(string text)
#pragma warning restore CS8618
    {
        Value = text;
        TextChanged += OnTextChanged;
    }

    void OnTextChanged(object? sender, EventArgs e)
    {

    }

    /// <inheritdoc/>
    protected override string RenderWidget() => _value;
}
