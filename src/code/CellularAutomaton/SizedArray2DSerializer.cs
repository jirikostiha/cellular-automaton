using System.Globalization;
using CommunityToolkit.Diagnostics;

namespace CellularAutomaton;

/// <summary>
/// Converts a boolean matrix from and to a text prefixed with a <c>size:{x}x{y}</c> header, so
/// that the matrix can be restored without knowing its dimensions in advance.
/// </summary>
public class SizedArray2DSerializer
{
    private const string SizePrefix = "size:";

    private readonly Array2DSerializerBase _arraySerializer;
    private readonly Func<int, int, IArray2D<bool>> _arrayCreator;

    public SizedArray2DSerializer(
        Func<int, int, IArray2D<bool>> arrayCreator,
        Array2DSerializerBase? underlyingSerializer = null)
    {
        Guard.IsNotNull(arrayCreator);

        _arrayCreator = arrayCreator;
        _arraySerializer = underlyingSerializer ?? new BoolArray2DSerializer();
    }

    public string Serialize(IReadableArray2D<bool> array2D)
    {
        Guard.IsNotNull(array2D);

        return string.Concat(
            SizePrefix,
            array2D.XCount.ToString(CultureInfo.InvariantCulture),
            "x",
            array2D.YCount.ToString(CultureInfo.InvariantCulture),
            Environment.NewLine,
            _arraySerializer.Serialize(array2D));
    }

    public IArray2D<bool> Deserialize(string contentWithHeader)
    {
        Guard.IsNotNullOrEmpty(contentWithHeader);

        int headerEnd = contentWithHeader.IndexOf('\n');
        if (headerEnd < 0)
            throw new FormatException("The content does not contain a size header.");

        var size = ParseHeader(contentWithHeader.AsSpan(0, headerEnd));
        var array2D = _arrayCreator(size.X, size.Y);
        if (array2D.XCount > 0 && array2D.YCount > 0)
            array2D.SetRegion(0, 0, array2D.XCount - 1, array2D.YCount - 1, false);

        var body = contentWithHeader.Substring(headerEnd + 1);
        if (body.Length > 0)
            _arraySerializer.Deserialize(body, array2D);

        return array2D;
    }

    private static MatrixSize ParseHeader(ReadOnlySpan<char> header)
    {
        if (header.Length > 0 && header[header.Length - 1] == '\r')
            header = header.Slice(0, header.Length - 1);

        if (!header.StartsWith(SizePrefix.AsSpan(), StringComparison.Ordinal))
            throw new FormatException($"The size header is expected to start with '{SizePrefix}'.");

        var values = header.Slice(SizePrefix.Length);
        int separator = values.IndexOf('x');
        if (separator < 0
            || !int.TryParse(values.Slice(0, separator), NumberStyles.None, CultureInfo.InvariantCulture, out int x)
            || !int.TryParse(values.Slice(separator + 1), NumberStyles.None, CultureInfo.InvariantCulture, out int y))
        {
            throw new FormatException($"The size header '{header.ToString()}' is malformed.");
        }

        return new MatrixSize(x, y);
    }
}
