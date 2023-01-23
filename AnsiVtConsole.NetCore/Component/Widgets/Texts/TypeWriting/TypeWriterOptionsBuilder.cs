namespace AnsiVtConsole.NetCore.Component.Widgets.Texts.TypeWriting;

/// <summary>
/// type options writer builder
/// </summary>
public sealed class TypeWriterOptionsBuilder : AnimatedOptionsBuilder<TypeWriter>
{
    /// <summary>
    /// type writer options builder
    /// </summary>
    /// <param name="typeWriter">type writer</param>
    public TypeWriterOptionsBuilder(TypeWriter typeWriter)
        : base(typeWriter) { }

    /// <summary>
    /// cursor
    /// </summary>
    /// <param name="cursor">cursor</param>
    /// <returns>this object</returns>
    public TypeWriterOptionsBuilder Cursor(string cursor)
    {
        Widget.SetCursor(cursor);
        return this;
    }

    /// <summary>
    /// text
    /// </summary>
    /// <param name="text">text</param>
    /// <returns>this object</returns>
    public TypeWriterOptionsBuilder Text(string text)
    {
        Widget.SetText(text);
        return this;
    }

}
