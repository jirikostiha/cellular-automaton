using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace CellularAutomaton;

/// <summary>
/// Two dimensional array of booleans backed by a flat, row major buffer.
/// </summary>
/// <remarks>
/// A flat <c>bool[]</c> is used instead of a <c>bool[,]</c> because the runtime cannot elide the
/// bounds checks of multidimensional arrays; row major order also keeps the cells of one row
/// contiguous, which is the order in which they are traversed and rendered.
/// </remarks>
public sealed class BoolArray2D : IArray2D<bool>
{
    private readonly bool[] _cells;
    private readonly int _xcount;
    private readonly int _ycount;

    /// <summary>
    /// Creates an empty matrix of the given size.
    /// </summary>
    public BoolArray2D(int xcount, int ycount)
    {
        Guard.IsGreaterThanOrEqualTo(xcount, 0);
        Guard.IsGreaterThanOrEqualTo(ycount, 0);

        _xcount = xcount;
        _ycount = ycount;
        _cells = new bool[(long)xcount * ycount <= int.MaxValue
            ? xcount * ycount
            : throw new ArgumentOutOfRangeException(nameof(ycount), "The matrix is too large.")];
    }

    /// <summary>
    /// Creates a matrix from a <c>[x, y]</c> indexed array. The content is copied.
    /// </summary>
    public BoolArray2D(bool[,] underlyingArray)
    {
        Guard.IsNotNull(underlyingArray);

        _xcount = underlyingArray.GetLength(0);
        _ycount = underlyingArray.GetLength(1);
        _cells = new bool[_xcount * _ycount];

        for (int y = 0; y < _ycount; y++)
        {
            int rowOffset = y * _xcount;
            for (int x = 0; x < _xcount; x++)
                _cells[rowOffset + x] = underlyingArray[x, y];
        }
    }

    private BoolArray2D(bool[] cells, int xcount, int ycount)
    {
        _cells = cells;
        _xcount = xcount;
        _ycount = ycount;
    }

    public int XCount => _xcount;

    public int YCount => _ycount;

    /// <summary>Raw cell buffer, row major, <see cref="XCount"/> cells per row.</summary>
    internal bool[] Cells => _cells;

    public bool GetAt(int x, int y)
    {
        ValidateIndex(x, y);

        return _cells[(y * _xcount) + x];
    }

    public void SetAt(int x, int y, bool value)
    {
        ValidateIndex(x, y);

        _cells[(y * _xcount) + x] = value;
    }

    public void SetRegion(int xA, int yA, int xB, int yB, bool value)
    {
        if (xA > xB || yA > yB)
            return;

        Guard.IsInRange(xA, 0, _xcount);
        Guard.IsInRange(xB, 0, _xcount);
        Guard.IsInRange(yA, 0, _ycount);
        Guard.IsInRange(yB, 0, _ycount);

        int length = xB - xA + 1;
        for (int y = yA; y <= yB; y++)
            _cells.AsSpan((y * _xcount) + xA, length).Fill(value);
    }

    public void Clean() => Array.Clear(_cells, 0, _cells.Length);

    public object Clone() => new BoolArray2D((bool[])_cells.Clone(), _xcount, _ycount);

    /// <summary>
    /// Creates a matrix where every cell holds <paramref name="value"/>.
    /// </summary>
    public static BoolArray2D Create(int xcount, int ycount, bool value = false)
    {
        var array = new BoolArray2D(xcount, ycount);
        if (value)
            Array.Fill(array._cells, true);

        return array;
    }

    /// <summary>
    /// Creates a matrix where the outermost cells hold <paramref name="boundaryValue"/> and the
    /// remaining ones <paramref name="innerValue"/>.
    /// </summary>
    public static BoolArray2D Create(int xcount, int ycount, bool innerValue, bool boundaryValue = false)
    {
        var array = Create(xcount, ycount, boundaryValue);
        if (xcount > 2 && ycount > 2)
            array.SetRegion(1, 1, xcount - 2, ycount - 2, innerValue);

        return array;
    }

    /// <summary>
    /// Creates a matrix filled by <paramref name="bitValueProvider"/>.
    /// </summary>
    public static BoolArray2D Create(int xcount, int ycount, Func<int, int, bool> bitValueProvider)
    {
        Guard.IsNotNull(bitValueProvider);

        var array = new BoolArray2D(xcount, ycount);
        var cells = array._cells;
        for (int y = 0; y < ycount; y++)
        {
            int rowOffset = y * xcount;
            for (int x = 0; x < xcount; x++)
                cells[rowOffset + x] = bitValueProvider(x, y);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidateIndex(int x, int y)
    {
        if ((uint)x >= (uint)_xcount)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(x));
        if ((uint)y >= (uint)_ycount)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(y));
    }
}
