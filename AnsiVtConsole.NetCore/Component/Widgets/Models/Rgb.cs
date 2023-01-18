namespace AnsiVtConsole.NetCore.Component.Widgets.Models;

/// <summary>
/// rgb color values
/// </summary>
public sealed class Rgb
{
    int _r;
    /// <summary>
    /// red
    /// </summary>
    public int R
    {
        get => _r;
        set
        {
            AssertValueIsValid(value);
            _r = value;
        }
    }

    int _g;
    /// <summary>
    /// green
    /// </summary>
    public int G
    {
        get => _g;
        set
        {
            AssertValueIsValid(value);
            _g = value;
        }
    }

    int _b;
    /// <summary>
    /// blue
    /// </summary>
    public int B
    {
        get => _b;
        set
        {
            AssertValueIsValid(value);
            _b = value;
        }
    }

    /// <summary>
    /// Rgb
    /// </summary>
    /// <param name="r">red</param>
    /// <param name="g">green</param>
    /// <param name="b">blue</param>
    public Rgb(int r = 0, int g = 0, int b = 0)
    {
        _r = r;
        _g = g;
        _b = b;
    }

    static void AssertValueIsValid(int value)
    {
        if (value < 0 || value > 255)
            throw new ArgumentException(value.ToString());
    }
}
