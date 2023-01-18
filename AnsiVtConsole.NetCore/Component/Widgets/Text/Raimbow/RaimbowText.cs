using System.Text;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;

namespace AnsiVtConsole.NetCore.Component.Widgets.Text.Raimbow;

/// <summary>
/// raimbow text
/// </summary>
public class RaimbowText : WidgetAbstact<RaimbowText>
{
    /// <summary>
    /// origin R of the gradient
    /// </summary>
    public int OriginR { get; private set; }

    /// <summary>
    /// origin G of the gradient
    /// </summary>
    public int OriginG { get; private set; }

    /// <summary>
    /// origin B of the gradient
    /// </summary>
    public int OriginB { get; private set; }

    /// <summary>
    /// current R of the gradient
    /// </summary>
    public int R { get; private set; }

    /// <summary>
    /// current G of the gradient
    /// </summary>
    public int G { get; private set; }

    /// <summary>
    /// current B of the gradient
    /// </summary>
    public int B { get; private set; }

    /// <summary>
    /// delta R of the gradient
    /// </summary>
    public int DR { get; private set; }

    /// <summary>
    /// delta G of the gradient
    /// </summary>
    public int DG { get; private set; }

    /// <summary>
    /// delta B of the gradient
    /// </summary>
    public int DB { get; private set; }

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
    /// raimbow text embeding a wiDGet
    /// </summary>
    /// <param name="wrappedWiDGet">wrapped wiDGet</param>
    public RaimbowText(IWidgetAbstact wrappedWiDGet)
        : base(wrappedWiDGet) { }

    /// <summary>
    /// set origin r,g,b of the gradient
    /// </summary>
    /// <param name="r">oring R of the gradient</param>
    /// <param name="g">oring G of the gradient</param>
    /// <param name="b">oring B of the gradient</param>
    /// <returns>this object</returns>
    public RaimbowText Origin(int r, int g, int b)
    {
        (OriginR, OriginG, OriginB) = (r, g, b);
        return this;
    }

    /// <summary>
    /// setup cyclic grandient with dr,dg,db increments
    /// </summary>
    /// <param name="dr">delta R</param>
    /// <param name="dg">delta G</param>
    /// <param name="db">detla B</param>
    /// <returns>this object</returns>
    public RaimbowText CyclicGradient(int dr, int dg, int db)
    {
        (DR, DG, DB) = (dr, dg, db);
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
                R = OriginR;
                G = OriginG;
                B = OriginB;
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
