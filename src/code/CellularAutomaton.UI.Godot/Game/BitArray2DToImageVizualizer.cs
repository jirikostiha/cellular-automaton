using Godot;

namespace CellularAutomaton.UI.Godot;

/// <summary>
/// Renders a boolean matrix into an <see cref="ImageTexture"/>.
/// </summary>
/// <remarks>
/// The matrix is written into a reused single channel buffer and uploaded in one call. Setting the
/// pixels one by one through <see cref="Image.SetPixel"/> costs an interop call per cell, which
/// dominates the frame time of larger matrices.
/// </remarks>
public sealed class BitArray2DToImageVizualizer
{
    private const Image.Format TextureFormat = Image.Format.L8;

    private byte[] _buffer = [];

    public BitArray2DToImageVizualizer(BitArray2DVizuOptions options)
    {
        Options = options;
    }

    public BitArray2DVizuOptions Options { get; }

    /// <summary>
    /// Creates a texture matching the given matrix size.
    /// </summary>
    public static ImageTexture CreateTexture(int xcount, int ycount)
        => ImageTexture.CreateFromImage(Image.Create(xcount, ycount, false, TextureFormat));

    public void Vizualize(IReadableArray2D<bool> matrix, ImageTexture imageTexture)
    {
        ArgumentNullException.ThrowIfNull(imageTexture);

        int xcount = matrix.XCount;
        int ycount = matrix.YCount;
        if (xcount == 0 || ycount == 0)
            return;

        if (_buffer.Length != xcount * ycount)
            _buffer = new byte[xcount * ycount];

        byte trueValue = ToLuminance(Options.TrueColor);
        byte falseValue = ToLuminance(Options.FalseColor);

        int index = 0;
        for (int y = 0; y < ycount; y++)
            for (int x = 0; x < xcount; x++)
                _buffer[index++] = matrix.GetAt(x, y) ? trueValue : falseValue;

        using var image = Image.CreateFromData(xcount, ycount, false, TextureFormat, _buffer);
        imageTexture.Update(image);
    }

    private static byte ToLuminance(Color color)
        => (byte)Mathf.Clamp(Mathf.RoundToInt(color.Luminance * 255f), 0, 255);
}
