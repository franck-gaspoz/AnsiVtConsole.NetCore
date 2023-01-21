using System.Text;

using AnsiVtConsole.NetCore.Component.Widgets;

using SkiaSharp;

using static AnsiVtConsole.NetCore.Component.Console.ANSI;

namespace AnsiVtConsole.NetCore.Imaging.Component.Widgets.Images;

/// <summary>
/// iamge to ansi widget
/// </summary>
public sealed class Image : Widget<Image>
{
    /// <summary>
    /// text
    /// </summary>
    public string? Filename { get; private set; }

    /// <summary>
    /// width
    /// </summary>
    public int? Width { get; private set; }

    /// <summary>
    /// height
    /// </summary>
    public int? Height { get; private set; }

    /// <summary>
    /// pixel char function
    /// </summary>
    public Func<int, int, string> PixelChar { get; private set; }

    /// <summary>
    /// if true the bitmap is painted on background
    /// </summary>
    public bool PaintOnBackground { get; private set; }

    const string DefaultPixelChar = "  ";

    SKBitmap? _image;
    SKBitmap? _scaledImage;
    StringBuilder? _sb;

    /// <summary>
    /// widget image
    /// <para>if both width and height are null use the image size</para>
    /// <para>if height is null it is computed according to the image width/height ratio</para>
    /// <para>if width is null it is computed according to the image width/height ratio</para>
    /// </summary>
    /// <param name="filename">filename</param>
    /// <param name="width">width</param>
    /// <param name="height">height</param>
    /// <param name="paintOnBackground">if true the bitmap is painted on background</param>
    /// <param name="pixelChar">a function that gives the string to draw a pixel from position x and y (default is "  ")</param>
    public Image(
        string filename,
        int? width = null,
        int? height = null,
        bool paintOnBackground = true,
        Func<int, int, string>? pixelChar = null)
    {
        if (width is not null && width <= 0)
            throw new ArgumentException("width must be > 0");
        if (height is not null && height <= 0)
            throw new ArgumentException("height must be > 0");
        Width = width;
        Height = height;
        Filename = filename;
        PixelChar = pixelChar ?? DefaultPixelCharFunc;
        PaintOnBackground = paintOnBackground;
    }

    string DefaultPixelCharFunc(int x, int y) => DefaultPixelChar;

    /// <inheritdoc/>
    public Image(IWidget wrappedWidget)
        : base(wrappedWidget)
        => throw new NotImplementedException();

    string GetImage()
    {
        if (_image is null)
        {
            _image = SKBitmap.Decode(Filename);
            if (_image is null)
                throw new InvalidOperationException($"failed to load or decode image: " + Filename);
        }

        if (_scaledImage is null && Width is not null)
        {
            _scaledImage = new SKBitmap(Width!.Value, Height!.Value);
            if (!_image.ScalePixels(_scaledImage, SKFilterQuality.High))
                throw new InvalidOperationException($"failed to scale image: {Filename} from ({_image.Width},{_image.Height}) to ({Width},{Height})");
        }

        var image = Width is null ? _image : _scaledImage;
        if (_sb is null)
            _sb = new StringBuilder(image!.Width * image.Height * 16 + image.Height * 18);
        else _sb.Clear();

        var curY = Y;

        var pixels = image!.Pixels;
        var pixelIndex = 0;
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                var color = pixels[pixelIndex++];
                var alpha = color.Alpha / 255d;
                var red = (int)Math.Round(color.Red * alpha);
                var green = (int)Math.Round(color.Green * alpha);
                var blue = (int)Math.Round(color.Blue * alpha);
                _sb.Append(
                    (PaintOnBackground ?
                        SGR_SetBackgroundColor24bits(
                            red,
                            green,
                            blue
                            )
                        : SGR_SetForegroundColor24bits(
                            red,
                            green,
                            blue
                            ))
                    + PixelChar(x, y));
            }
            _sb.Append(RSTXTA + CRLF);
            if (y != image.Height)
                _sb.Append(CUP(X + 1, Y + 1 + y));
        }

        return _sb.ToString();
    }

    /// <inheritdoc/>
    protected override string RenderWidget() => GetImage();
}
