namespace CellularAutomaton.UI.WinForms;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

/// <summary>
/// Renders a boolean matrix into a <see cref="Bitmap"/>.
/// </summary>
/// <remarks>
/// The pixels are written through <see cref="Bitmap.LockBits(Rectangle, ImageLockMode, PixelFormat)"/>
/// into a reused buffer. <see cref="Bitmap.SetPixel"/> costs a GDI+ call per pixel, which is orders
/// of magnitude slower and dominates the frame time of larger matrices.
/// </remarks>
public sealed class MatrixToBitmapVizualizer
{
    private const PixelFormat Format = PixelFormat.Format32bppPArgb;

    private int[] _buffer = Array.Empty<int>();

    public Color LiveColor { get; set; } = Color.Black;

    public Color DeadColor { get; set; } = Color.White;

    /// <summary>
    /// Creates a bitmap matching the size of the matrix.
    /// </summary>
    public static Bitmap CreateBitmap(IReadableArray2D<bool> matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        return new Bitmap(Math.Max(matrix.XCount, 1), Math.Max(matrix.YCount, 1), Format);
    }

    /// <summary>
    /// Draws the matrix into <paramref name="bitmap"/>, creating a new one when it is missing or
    /// does not match the size of the matrix.
    /// </summary>
    public Bitmap Vizualize(IReadableArray2D<bool> matrix, Bitmap? bitmap = null)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        int xcount = matrix.XCount;
        int ycount = matrix.YCount;
        if (bitmap is null || bitmap.Width != xcount || bitmap.Height != ycount || bitmap.PixelFormat != Format)
        {
            bitmap?.Dispose();
            bitmap = new Bitmap(Math.Max(xcount, 1), Math.Max(ycount, 1), Format);
        }

        if (xcount == 0 || ycount == 0)
            return bitmap;

        if (_buffer.Length < xcount)
            _buffer = new int[xcount];

        int live = LiveColor.ToArgb();
        int dead = DeadColor.ToArgb();

        var data = bitmap.LockBits(new Rectangle(0, 0, xcount, ycount), ImageLockMode.WriteOnly, Format);
        try
        {
            for (int y = 0; y < ycount; y++)
            {
                for (int x = 0; x < xcount; x++)
                    _buffer[x] = matrix.GetAt(x, y) ? live : dead;

                Marshal.Copy(_buffer, 0, data.Scan0 + (y * data.Stride), xcount);
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        return bitmap;
    }
}
