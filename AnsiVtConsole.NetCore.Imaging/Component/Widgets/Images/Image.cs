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
    public int Width { get; private set; }

    /// <summary>
    /// height
    /// </summary>
    public int Height { get; private set; }

    SKBitmap? _image;
    SKBitmap? _scaledImage;
    readonly StringBuilder _sb = new();

    /// <summary>
    /// widget image
    /// </summary>
    /// <param name="filename">filename</param>
    /// <param name="width">width</param>
    /// <param name="height">height</param>
    public Image(string filename, int width, int height)
    {
        if (width <= 0)
            throw new ArgumentException("width must be > 0");
        if (height <= 0)
            throw new ArgumentException("height must be > 0");
        Width = width;
        Height = height;
        Filename = filename;
    }

    /// <inheritdoc/>
    public Image(IWidget wrappedWidget)
        : base(wrappedWidget)
        => throw new NotImplementedException();

    string GetImage()
    {
        if (_image is null)
            _image = SKBitmap.Decode(Filename);

        if (_scaledImage is null)
        {
            _scaledImage = new SKBitmap(Width, Height);
            if (!_image.ScalePixels(_scaledImage, SKFilterQuality.High))
                throw new InvalidOperationException($"failed to scale image: {Filename} from ({_image.Width},{_image.Height}) to ({Width},{Height})");
        }

        if (_sb.Length == 0)
        {
            var pixelIndex = 0;
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var color = _scaledImage.Pixels[pixelIndex++];
                    _sb.Append(
                        SGR_SetBackgroundColor24bits(
                            color.Red,
                            color.Green,
                            color.Blue
                            ) + " ");
                }
                _sb.Append(RSTXTA);
                _sb.Append(CRLF);
            }
        }

        return _sb.ToString();
    }

    /// <inheritdoc/>
    protected override string RenderWidget() => GetImage();
}
