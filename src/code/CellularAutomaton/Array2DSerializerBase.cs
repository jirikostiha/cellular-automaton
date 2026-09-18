using CommunityToolkit.Diagnostics;

namespace CellularAutomaton;

/// <summary>
/// Converts a boolean matrix from and to a text of one line per row.
/// </summary>
public abstract class Array2DSerializerBase
{
    /// <summary>Character representing a live cell.</summary>
    public char TrueValue { get; set; } = '1';

    /// <summary>Character representing a dead cell.</summary>
    public char FalseValue { get; set; } = ' ';

    /// <summary>
    /// Writes the matrix as one line per row, rows separated by <see cref="Environment.NewLine"/>.
    /// </summary>
    public string Serialize(IReadableArray2D<bool> array2D)
    {
        Guard.IsNotNull(array2D);

        int xcount = array2D.XCount;
        int ycount = array2D.YCount;
        if (xcount == 0 || ycount == 0)
            return string.Empty;

        var newLine = Environment.NewLine;
        int length = (ycount * xcount) + ((ycount - 1) * newLine.Length);

        Span<char> buffer = length <= 1024 ? stackalloc char[length] : new char[length];
        int position = 0;
        for (int y = 0; y < ycount; y++)
        {
            if (y > 0)
            {
                newLine.AsSpan().CopyTo(buffer.Slice(position));
                position += newLine.Length;
            }

            for (int x = 0; x < xcount; x++)
                buffer[position++] = array2D.GetAt(x, y) ? TrueValue : FalseValue;
        }

        return buffer.ToString();
    }

    /// <summary>
    /// Fills <paramref name="array2D"/> with the content of <paramref name="content"/>. Cells that
    /// are not covered by the content keep their current value, content that does not fit the
    /// matrix is ignored.
    /// </summary>
    /// <remarks>
    /// Both LF and CRLF line endings are accepted, whichever platform wrote the content.
    /// </remarks>
    public void Deserialize(string content, IWritableArray2D<bool> array2D, int xcount, int ycount)
    {
        Guard.IsNotNullOrEmpty(content);
        Guard.IsNotNull(array2D);

        int y = 0;
        foreach (var line in EnumerateLines(content))
        {
            if (y >= ycount)
                break;

            int xmax = Math.Min(line.Length, xcount);
            for (int x = 0; x < xmax; x++)
                array2D.SetAt(x, y, line[x] == TrueValue);

            y++;
        }
    }

    /// <summary>
    /// Fills <paramref name="array2D"/> with the content of <paramref name="content"/>.
    /// </summary>
    public void Deserialize(string content, IArray2D<bool> array2D)
    {
        Guard.IsNotNull(array2D);

        Deserialize(content, array2D, array2D.XCount, array2D.YCount);
    }

    /// <summary>
    /// Size of the matrix described by <paramref name="content"/>: the number of non empty lines
    /// and the length of the longest one.
    /// </summary>
    protected static MatrixSize MeasureContent(string content)
    {
        Guard.IsNotNullOrEmpty(content);

        int xcount = 0;
        int ycount = 0;
        foreach (var line in EnumerateLines(content))
        {
            xcount = Math.Max(xcount, line.Length);
            ycount++;
        }

        return new MatrixSize(xcount, ycount);
    }

    /// <summary>
    /// Splits the content into non empty lines without allocating a string per line.
    /// </summary>
    protected static LineEnumerator EnumerateLines(string content) => new(content.AsSpan());

    /// <summary>
    /// Enumerates the non empty lines of a character span, accepting both LF and CRLF.
    /// </summary>
    protected ref struct LineEnumerator
    {
        private ReadOnlySpan<char> _remaining;

        internal LineEnumerator(ReadOnlySpan<char> content)
        {
            _remaining = content;
            Current = default;
        }

        public ReadOnlySpan<char> Current { get; private set; }

        public readonly LineEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            while (!_remaining.IsEmpty)
            {
                int end = _remaining.IndexOf('\n');
                ReadOnlySpan<char> line;
                if (end < 0)
                {
                    line = _remaining;
                    _remaining = default;
                }
                else
                {
                    line = _remaining.Slice(0, end);
                    _remaining = _remaining.Slice(end + 1);
                }

                if (line.Length > 0 && line[line.Length - 1] == '\r')
                    line = line.Slice(0, line.Length - 1);

                if (line.Length == 0)
                    continue;

                Current = line;

                return true;
            }

            return false;
        }
    }
}
