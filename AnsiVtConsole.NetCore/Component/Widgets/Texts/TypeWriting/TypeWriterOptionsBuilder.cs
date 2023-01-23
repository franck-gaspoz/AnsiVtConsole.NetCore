namespace AnsiVtConsole.NetCore.Component.Widgets.Texts.TypeWriting;

/// <summary>
/// type options writer builder
/// </summary>
public sealed class TypeWriterOptionsBuilder
{
    readonly TypeWriter _typeWriter;

    /// <summary>
    /// type writer options builder
    /// </summary>
    /// <param name="typeWriter">type writer</param>
    public TypeWriterOptionsBuilder(TypeWriter typeWriter)
        => _typeWriter = typeWriter;

    /// <summary>
    /// cursor
    /// </summary>
    /// <param name="cursor">cursor</param>
    /// <returns>this object</returns>
    public TypeWriterOptionsBuilder Cursor(string cursor)
    {
        _typeWriter.SetCursor(cursor);
        return this;
    }

    /// <summary>
    /// cursor
    /// </summary>
    /// <param name="cps">cps</param>
    /// <returns>this object</returns>
    public TypeWriterOptionsBuilder Cps(double cps)
    {
        _typeWriter.SetCps(cps);
        return this;
    }

    /// <summary>
    /// text
    /// </summary>
    /// <param name="text">text</param>
    /// <returns>this object</returns>
    public TypeWriterOptionsBuilder Text(string text)
    {
        _typeWriter.SetText(text);
        return this;
    }

    /// <summary>
    /// build the configured object
    /// </summary>
    /// <returns>the configured object</returns>
    public TypeWriter Build() => _typeWriter;
}
