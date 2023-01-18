using System.Text;

using AnsiVtConsole.NetCore.Component.Widgets.Models;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;

namespace AnsiVtConsole.NetCore.Component.Widgets.Text.Raimbow;

/// <summary>
/// raimbow text
/// </summary>
public class RaimbowText : TextWidget
{
    /// <summary>
    /// origin RGB of the gradient
    /// </summary>
    public Rgb OriginRGB { get; private set; } = new(0, 0, 128);

    /// <summary>
    /// current RGB of the gradient
    /// </summary>
    public Rgb Rgb { get; private set; } = new();

    /// <summary>
    /// delta RGB of the gradient
    /// </summary>
    public Rgb DRgb { get; private set; } = new(4, 9, 14);

    /// <inheritdoc/>
    public RaimbowText(string text) : base(text) { }

    readonly StringBuilder _sb = new();

    /// <inheritdoc/>
    public RaimbowText(IWidgetAbstact wrappedWidget)
        : base(wrappedWidget) { }

    /// <summary>
    /// set origin r,g,b of the gradient
    /// </summary>
    /// <param name="r">oring R of the gradient</param>
    /// <param name="g">oring G of the gradient</param>
    /// <param name="b">oring B of the gradient</param>
    /// <returns>this object</returns>
    public RaimbowText Origin(int r, int g, int b)
    {
        OriginRGB = new(r, g, b);
        return this;
    }

    /// <summary>
    /// set origin r,g,b of the gradient
    /// </summary>
    /// <param name="rgb">rgb</param>
    /// <returns>this object</returns>
    public RaimbowText Origin(Rgb rgb)
    {
        OriginRGB = rgb;
        return this;
    }

    /// <summary>
    /// setup cyclic gradient with dr,dg,db increments
    /// </summary>
    /// <param name="dr">delta R</param>
    /// <param name="dg">delta G</param>
    /// <param name="db">detla B</param>
    /// <returns>this object</returns>
    public RaimbowText CyclicGradient(int dr, int dg, int db)
    {
        DRgb = new(dr, dg, db);
        return this;
    }

    /// <summary>
    /// setup cyclic gradient with dr,dg,db increments
    /// </summary>
    /// <param name="rgb">rgb</param>
    /// <returns>this object</returns>
    public RaimbowText CyclicGradient(Rgb rgb)
    {
        DRgb = rgb;
        return this;
    }

    /// <inheritdoc/>
    protected override string RenderWidget()
    {
        _sb.Clear();
        if (Text is null)
            return string.Empty;

        var t = Text!.ToCharArray();
        foreach (var c in t)
        {
            if (c == '\n')
            {
                Rgb.Set(OriginRGB);
            }
            Rgb = NextColor(Rgb, DRgb);

            _sb.Append(SGR_SetForegroundColor24bits(Rgb.R, Rgb.G, Rgb.B));
            _sb.Append(c);
        }
        return _sb.ToString();
    }

    static Rgb NextColor(Rgb rgb, Rgb drgb)
    {
        var r = rgb.R + drgb.R;
        var g = rgb.G + drgb.G;
        var b = rgb.B + drgb.B;
        if (r < 0)
        {
            r = 0;
            drgb.R = drgb.R * -1;
        }
        if (r > 255)
        {
            r = 255;
            drgb.R = drgb.R *= -1;
        }
        if (g < 0)
        {
            g = 0;
            drgb.G = drgb.G * -1;
        }
        if (g > 255)
        {
            g = 255;
            drgb.G = drgb.G *= -1;
        }
        if (b < 0)
        {
            b = 0;
            drgb.B = drgb.B * -1;
        }
        if (b > 255)
        {
            b = 255;
            drgb.B = drgb.B *= -1;
        }
        rgb.Set(r, g, b);
        return rgb;
    }
}
