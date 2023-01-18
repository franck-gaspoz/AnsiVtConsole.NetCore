using System.Text;

using AnsiVtConsole.NetCore.Component.Widgets.Models;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;

namespace AnsiVtConsole.NetCore.Component.Widgets.Text.Raimbow;

/// <summary>
/// raimbow text
/// </summary>
public sealed class RaimbowText : WidgetAbstact<RaimbowText>
{
    /// <summary>
    /// origin RGB of the gradient
    /// </summary>
    public Rgb OriginRGB { get; private set; } = new();

    /// <summary>
    /// current RGB of the gradient
    /// </summary>
    public Rgb Rgb { get; private set; } = new();

    /// <summary>
    /// delta RGB of the gradient
    /// </summary>
    public Rgb DRgb { get; private set; } = new();

    /// <summary>
    /// text of the raimbow text
    /// </summary>
    public string? Text { get; private set; }

    /// <summary>
    /// raimbow text
    /// </summary>
    /// <param name="text">text</param>
    public RaimbowText(string text)
        => Text = text;

    readonly StringBuilder _sb = new();

    /// <summary>
    /// raimbow text embeding a widget
    /// </summary>
    /// <param name="wrappedWidget">wrapped widget</param>
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
                Rgb.R = OriginR;
                Rgb.G = OriginG;
                Rgb.B = OriginB;
            }
            (R, G, B) = NextColor(R, G, B);

            _sb.Append(SGR_SetForegroundColor24bits(R, G, B));
            _sb.Append(c);
        }
        return _sb.ToString();
    }

    (int r, int g, int b) NextColor(int r, int g, int b)
    {
        r += DR;
        g += DG;
        b += DB;
        if (r < 0)
        {
            r = 0;
            DR = DR * -1;
        }
        if (r > 255)
        {
            r = 255;
            DR = DR *= -1;
        }
        if (g < 0)
        {
            g = 0;
            DG = DG * -1;
        }
        if (g > 255)
        {
            g = 255;
            DG = DG *= -1;
        }
        if (b < 0)
        {
            b = 0;
            DB = DB * -1;
        }
        if (b > 255)
        {
            b = 255;
            DB = DB *= -1;
        }
        return (r, g, b);
    }
}
