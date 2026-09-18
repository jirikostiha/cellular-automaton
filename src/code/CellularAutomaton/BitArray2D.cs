using System.Collections;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace CellularAutomaton;

/// <summary>
/// Bit packed two dimensional array of booleans.
/// </summary>
/// <remarks>
/// Cells are stored in a single flat <see cref="ulong"/> buffer, row by row, so that cells
/// neighbouring on the x axis are neighbours in memory as well. This layout makes it possible to
/// evaluate 64 cells at once (see <see cref="GenerationProcessor"/>) and keeps the whole matrix in
/// one allocation instead of one per row.
/// </remarks>
public sealed class BitArray2D : IArray2D<bool>
{
    /// <summary>Number of cells stored in a single word.</summary>
    internal const int BitsPerWord = 64;

    private const int WordShift = 6;
    private const int BitMask = BitsPerWord - 1;

    private readonly ulong[] _words;
    private readonly int _xcount;
    private readonly int _ycount;
    private readonly int _stride;

    /// <summary>
    /// Creates an empty matrix of the given size.
    /// </summary>
    public BitArray2D(int xcount, int ycount)
    {
        Guard.IsGreaterThanOrEqualTo(xcount, 0);
        Guard.IsGreaterThanOrEqualTo(ycount, 0);

        _xcount = xcount;
        _ycount = ycount;
        _stride = WordsPerRow(xcount);
        _words = new ulong[(long)_stride * ycount <= int.MaxValue
            ? _stride * ycount
            : throw new ArgumentOutOfRangeException(nameof(ycount), "The matrix is too large.")];
    }

    /// <summary>
    /// Creates a matrix from one <see cref="BitArray"/> per row. The content is copied.
    /// </summary>
    public BitArray2D(BitArray[] underlyingArray)
    {
        Guard.IsNotNull(underlyingArray);

        _ycount = underlyingArray.Length;
        _xcount = _ycount > 0 ? underlyingArray[0].Length : 0;
        _stride = WordsPerRow(_xcount);
        _words = new ulong[_stride * _ycount];

        for (int y = 0; y < _ycount; y++)
        {
            var row = underlyingArray[y];
            Guard.IsNotNull(row);

            int xmax = Math.Min(row.Length, _xcount);
            for (int x = 0; x < xmax; x++)
            {
                if (row[x])
                    _words[(y * _stride) + (x >> WordShift)] |= 1UL << (x & BitMask);
            }
        }
    }

    private BitArray2D(ulong[] words, int xcount, int ycount, int stride)
    {
        _words = words;
        _xcount = xcount;
        _ycount = ycount;
        _stride = stride;
    }

    public int XCount => _xcount;

    public int YCount => _ycount;

    /// <summary>Raw bit buffer, row major, <see cref="Stride"/> words per row.</summary>
    internal ulong[] Words => _words;

    /// <summary>Number of <see cref="ulong"/> words occupied by a single row.</summary>
    internal int Stride => _stride;

    public bool GetAt(int x, int y)
    {
        ValidateIndex(x, y);

        return (_words[(y * _stride) + (x >> WordShift)] & (1UL << (x & BitMask))) != 0UL;
    }

    public void SetAt(int x, int y, bool value)
    {
        ValidateIndex(x, y);

        int index = (y * _stride) + (x >> WordShift);
        ulong mask = 1UL << (x & BitMask);
        if (value)
            _words[index] |= mask;
        else
            _words[index] &= ~mask;
    }

    public void SetRegion(int xA, int yA, int xB, int yB, bool value)
    {
        if (xA > xB || yA > yB)
            return;

        Guard.IsInRange(xA, 0, _xcount);
        Guard.IsInRange(xB, 0, _xcount);
        Guard.IsInRange(yA, 0, _ycount);
        Guard.IsInRange(yB, 0, _ycount);

        int firstWord = xA >> WordShift;
        int lastWord = xB >> WordShift;

        for (int y = yA; y <= yB; y++)
        {
            int rowOffset = y * _stride;
            if (firstWord == lastWord)
            {
                ulong mask = RangeMask(xA & BitMask, xB & BitMask);
                Apply(ref _words[rowOffset + firstWord], mask, value);
                continue;
            }

            Apply(ref _words[rowOffset + firstWord], RangeMask(xA & BitMask, BitMask), value);
            for (int w = firstWord + 1; w < lastWord; w++)
                _words[rowOffset + w] = value ? ulong.MaxValue : 0UL;
            Apply(ref _words[rowOffset + lastWord], RangeMask(0, xB & BitMask), value);
        }
    }

    public void Clean() => Array.Clear(_words, 0, _words.Length);

    public object Clone() => new BitArray2D((ulong[])_words.Clone(), _xcount, _ycount, _stride);

    /// <summary>
    /// Creates a matrix where every cell holds <paramref name="value"/>.
    /// </summary>
    public static BitArray2D Create(int xcount, int ycount, bool value = false)
    {
        var array = new BitArray2D(xcount, ycount);
        if (value && xcount > 0 && ycount > 0)
            array.SetRegion(0, 0, xcount - 1, ycount - 1, true);

        return array;
    }

    /// <summary>
    /// Creates a matrix where the outermost cells hold <paramref name="boundaryValue"/> and the
    /// remaining ones <paramref name="innerValue"/>.
    /// </summary>
    public static BitArray2D Create(int xcount, int ycount, bool innerValue, bool boundaryValue = false)
    {
        var array = Create(xcount, ycount, boundaryValue);
        if (xcount > 2 && ycount > 2)
            array.SetRegion(1, 1, xcount - 2, ycount - 2, innerValue);

        return array;
    }

    /// <summary>
    /// Creates a matrix filled by <paramref name="bitValueProvider"/>.
    /// </summary>
    public static BitArray2D Create(int xcount, int ycount, Func<int, int, bool> bitValueProvider)
    {
        Guard.IsNotNull(bitValueProvider);

        var array = new BitArray2D(xcount, ycount);
        var words = array._words;
        for (int y = 0; y < ycount; y++)
        {
            int rowOffset = y * array._stride;
            for (int x = 0; x < xcount; x++)
            {
                if (bitValueProvider(x, y))
                    words[rowOffset + (x >> WordShift)] |= 1UL << (x & BitMask);
            }
        }

        return array;
    }

    /// <summary>Number of words needed to store a row of <paramref name="xcount"/> cells.</summary>
    internal static int WordsPerRow(int xcount) => (xcount + BitsPerWord - 1) / BitsPerWord;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Apply(ref ulong word, ulong mask, bool value)
    {
        if (value)
            word |= mask;
        else
            word &= ~mask;
    }

    /// <summary>Mask with bits <paramref name="from"/>..<paramref name="to"/> (inclusive) set.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong RangeMask(int from, int to)
        => (ulong.MaxValue << from) & (ulong.MaxValue >> (BitMask - to));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidateIndex(int x, int y)
    {
        if ((uint)x >= (uint)_xcount)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(x));
        if ((uint)y >= (uint)_ycount)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(y));
    }
}
