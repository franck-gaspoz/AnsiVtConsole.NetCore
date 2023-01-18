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
        set => _r = value;
    }

    int _g;
    /// <summary>
    /// green
    /// </summary>
    public int G
    {
        get => _g;
        set => _g = value;
    }

    int _b;
    /// <summary>
    /// blue
    /// </summary>
    public int B
    {
        get => _b;
        set => _b = value;
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

    /// <summary>
    /// set from another Rgb
    /// </summary>
    /// <param name="rgb">rgb</param>
    /// <returns>this object</returns>
    public Rgb Set(Rgb rgb)
    {
        _r = rgb.R;
        _g = rgb.G;
        _b = rgb.B;
        return this;
    }

    /// <summary>
    /// set from rgb
    /// </summary>
    /// <param name="r">red</param>
    /// <param name="g">green</param>
    /// <param name="b">blue</param>
    /// <returns>this object</returns>
    public Rgb Set(int r = 0, int g = 0, int b = 0)
    {
        _r = r;
        _g = g;
        _b = b;
        return this;
    }
}
