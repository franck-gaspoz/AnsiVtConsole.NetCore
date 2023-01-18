namespace AnsiVtConsole.NetCore.Component.Widgets;

/// <summary>
/// raimbow text
/// </summary>
public class RaimbowText : WidgetAbstact
{
    /// <summary>
    /// oring R of the gradient
    /// </summary>
    public int OriginR { get; private set; }

    /// <summary>
    /// oring G of the gradient
    /// </summary>
    public int OriginG { get; private set; }

    /// <summary>
    /// oring B of the gradient
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

    /// <summary>
    /// raimbow text embeding a wiDGet
    /// </summary>
    /// <param name="wrappedWiDGet">wrapped wiDGet</param>
    public RaimbowText(WidgetAbstact wrappedWiDGet)
        : base(wrappedWiDGet) { }

    /// <summary>
    /// raimbow text at a fixed location
    /// </summary>
    /// <param name="text">text</param>
    /// <param name="x">x</param>
    /// <param name="y">y</param>
    public RaimbowText(string text, int x, int y)
        : base(x, y)
            => Text = text;

    /// <inheritdoc/>
    public override string Render()
    {
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
        }
        return RenderFor(Text);
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
