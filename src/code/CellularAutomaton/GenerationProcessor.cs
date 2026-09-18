using CommunityToolkit.Diagnostics;

namespace CellularAutomaton;

/// <summary>
/// Evaluates generations of Conway's Game of Life over a matrix of cells.
/// </summary>
/// <remarks>
/// Two matrices are kept and swapped on every generation so that no allocation happens while the
/// automaton runs. When the matrices are <see cref="BitArray2D"/> instances, a bit parallel
/// algorithm evaluates 64 cells per machine word; any other <see cref="IArray2D{T}"/>
/// implementation falls back to a cell by cell evaluation.
/// </remarks>
public class GenerationProcessor
{
    private readonly GenerationProcessorOptions _options;

    private IArray2D<bool> _current;
    private IArray2D<bool> _previous;
    private IArray2D<bool>? _immortals;
    private IArray2D<bool>? _unviables;

    // Scratch buffers, allocated on first use and reused by every generation.
    private ulong[]? _innerMask;
    private byte[][]? _rowBuffers;

    public GenerationProcessor(IArray2D<bool> initialMatrix, GenerationProcessorOptions? options = null)
    {
        Guard.IsNotNull(initialMatrix);

        _current = initialMatrix;
        _options = options ?? new GenerationProcessorOptions();
        _previous = (IArray2D<bool>)_current.Clone();
    }

    public GenerationProcessorOptions Options => _options;

    /// <summary>Current generation.</summary>
    public ReadonlyArray2D<bool> Matrix => new(_current);

    /// <summary>
    /// Immortal cell flags. An immortal cell, once revived, can no longer die.
    /// </summary>
    public IArray2D<bool>? Immortals
    {
        get => _immortals;
        set => _immortals = ValidateFlags(value);
    }

    /// <summary>
    /// Unviable cell flags. An unviable cell, once dead, cannot be revived.
    /// </summary>
    public IArray2D<bool>? Unviables
    {
        get => _unviables;
        set => _unviables = ValidateFlags(value);
    }

    /// <summary>
    /// Evaluates the next generation.
    /// <list type="bullet">
    /// <item>Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.</item>
    /// <item>Any live cell with two or three live neighbours lives on to the next generation.</item>
    /// <item>Any live cell with more than three live neighbours dies, as if by overpopulation.</item>
    /// <item>Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.</item>
    /// </list>
    /// </summary>
    /// <returns>Number of cells that died, were revived and survived.</returns>
    public (int Died, int Revived, int Survived) Next()
    {
        // Swap the matrices, the previous generation becomes the source and its buffer is reused
        // as the target of the new one.
        (_current, _previous) = (_previous, _current);

        int xmax = _previous.XCount;
        int ymax = _previous.YCount;

        if (xmax < 3 || ymax < 3)
        {
            // There is no inner cell to evaluate.
            if (_options.CleanBorders && xmax > 0 && ymax > 0)
                _current.SetRegion(0, 0, xmax - 1, ymax - 1, false);

            return (0, 0, 0);
        }

        if (_options.CleanBorders)
        {
            _current.SetRegion(0, 0, xmax - 1, 0, false);
            _current.SetRegion(0, ymax - 1, xmax - 1, ymax - 1, false);
            _current.SetRegion(0, 1, 0, ymax - 2, false);
            _current.SetRegion(xmax - 1, 1, xmax - 1, ymax - 2, false);
        }

        return _previous is BitArray2D previousBits
            && _current is BitArray2D currentBits
            && _immortals is null or BitArray2D
            && _unviables is null or BitArray2D
                ? NextBitParallel(previousBits, currentBits, (BitArray2D?)_immortals, (BitArray2D?)_unviables)
                : NextGeneric(xmax, ymax);
    }

    private IArray2D<bool>? ValidateFlags(IArray2D<bool>? value)
    {
        if (value is null)
            return null;

        Guard.IsEqualTo(value.XCount, _current.XCount, nameof(value.XCount));
        Guard.IsEqualTo(value.YCount, _current.YCount, nameof(value.YCount));

        return value;
    }

    #region bit parallel path

    /// <summary>
    /// Evaluates the generation 64 cells at a time.
    /// </summary>
    /// <remarks>
    /// For every row a bit sliced sum of the cell and its two horizontal neighbours is computed
    /// (<see cref="ComputeRowTriples"/>). Summing the triples of three consecutive rows yields the
    /// number of live cells in the 3x3 block around every cell, from which the next state follows:
    /// a cell is alive when the block holds exactly three live cells, or four of which it is one.
    /// </remarks>
    private (int Died, int Revived, int Survived) NextBitParallel(
        BitArray2D previous, BitArray2D current, BitArray2D? immortals, BitArray2D? unviables)
    {
        int xmax = previous.XCount;
        int ymax = previous.YCount;
        int stride = previous.Stride;

        var innerMask = GetInnerMask(xmax, stride);

        int firstRow = 1;
        int lastRow = ymax - 2;
        int rowCount = lastRow - firstRow + 1;
        int partitions = GetPartitionCount(xmax, ymax, rowCount);

        if (partitions <= 1)
        {
            var scratch = new RowTriples(stride);

            return ProcessRows(previous, current, immortals, unviables, innerMask, scratch, firstRow, lastRow);
        }

        int died = 0;
        int revived = 0;
        int survived = 0;
        int rowsPerPartition = (rowCount + partitions - 1) / partitions;

        Parallel.For(
            0,
            partitions,
            () => new RowTriples(stride),
            (partition, _, scratch) =>
            {
                int from = firstRow + (partition * rowsPerPartition);
                int to = Math.Min(from + rowsPerPartition - 1, lastRow);
                if (from <= to)
                {
                    var stats = ProcessRows(previous, current, immortals, unviables, innerMask, scratch, from, to);
                    Interlocked.Add(ref died, stats.Died);
                    Interlocked.Add(ref revived, stats.Revived);
                    Interlocked.Add(ref survived, stats.Survived);
                }

                return scratch;
            },
            static _ => { });

        return (died, revived, survived);
    }

    private (int Died, int Revived, int Survived) ProcessRows(
        BitArray2D previous,
        BitArray2D current,
        BitArray2D? immortals,
        BitArray2D? unviables,
        ulong[] innerMask,
        RowTriples scratch,
        int firstRow,
        int lastRow)
    {
        var source = previous.Words;
        var target = current.Words;
        var immortalWords = immortals?.Words;
        var unviableWords = unviables?.Words;
        int stride = previous.Stride;
        bool cleanBorders = _options.CleanBorders;

        var sums = scratch.Sums;
        var carries = scratch.Carries;

        // Prime the rolling window with the triples of the three rows around the first one.
        for (int y = firstRow - 1; y <= firstRow + 1; y++)
            ComputeRowTriples(source, y * stride, stride, sums, carries, (y % 3) * stride);

        int died = 0;
        int revived = 0;
        int survived = 0;

        for (int y = firstRow; y <= lastRow; y++)
        {
            int above = ((y + 2) % 3) * stride; // (y - 1) % 3
            int middle = (y % 3) * stride;
            int below = ((y + 1) % 3) * stride;
            int rowOffset = y * stride;

            for (int j = 0; j < stride; j++)
            {
                ulong s0 = sums[above + j], s1 = sums[middle + j], s2 = sums[below + j];
                ulong c0 = carries[above + j], c1 = carries[middle + j], c2 = carries[below + j];

                // Bit sliced sum of the nine cells of the block: b0 + 2*b1 + 4*b2 + 8*b3.
                ulong b0 = s0 ^ s1 ^ s2;
                ulong sumCarry = (s0 & s1) | ((s0 ^ s1) & s2);
                ulong carryLow = c0 ^ c1 ^ c2;
                ulong carryHigh = (c0 & c1) | ((c0 ^ c1) & c2);
                ulong b1 = sumCarry ^ carryLow;
                ulong overflow = sumCarry & carryLow;
                ulong b2 = carryHigh ^ overflow;
                ulong b3 = carryHigh & overflow;

                ulong alive = source[rowOffset + j];
                ulong isThree = b0 & b1 & ~(b2 | b3);
                ulong isFour = ~b0 & ~b1 & b2 & ~b3;
                ulong byRules = isThree | (alive & isFour);

                ulong born = isThree & ~alive;
                if (unviableWords is not null)
                    born &= ~unviableWords[rowOffset + j];

                ulong survivors = byRules & alive;
                ulong immortal = immortalWords is not null
                    ? alive & ~byRules & immortalWords[rowOffset + j]
                    : 0UL;
                ulong dying = alive & ~byRules & ~immortal;

                ulong mask = innerMask[j];
                died += Bits.PopCount(dying & mask);
                revived += Bits.PopCount(born & mask);
                survived += Bits.PopCount((survivors | immortal) & mask);

                ulong next = (survivors | immortal | born) & mask;
                target[rowOffset + j] = cleanBorders
                    ? next
                    : next | (target[rowOffset + j] & ~mask);
            }

            // The row two below becomes the bottom of the next window.
            int nextRow = y + 2;
            if (nextRow < previous.YCount && y < lastRow)
                ComputeRowTriples(source, nextRow * stride, stride, sums, carries, (nextRow % 3) * stride);
        }

        return (died, revived, survived);
    }

    /// <summary>
    /// Computes, for every cell of a row, the two bit sum of the cell and its horizontal neighbours.
    /// </summary>
    private static void ComputeRowTriples(
        ulong[] words, int rowOffset, int stride, ulong[] sums, ulong[] carries, int slotOffset)
    {
        for (int j = 0; j < stride; j++)
        {
            ulong w = words[rowOffset + j];
            ulong previousWord = j > 0 ? words[rowOffset + j - 1] : 0UL;
            ulong nextWord = j + 1 < stride ? words[rowOffset + j + 1] : 0UL;

            ulong left = (w << 1) | (previousWord >> 63);
            ulong right = (w >> 1) | (nextWord << 63);

            sums[slotOffset + j] = left ^ w ^ right;
            carries[slotOffset + j] = (left & w) | ((left ^ w) & right);
        }
    }

    private ulong[] GetInnerMask(int xmax, int stride)
    {
        var mask = _innerMask;
        if (mask is not null && mask.Length == stride)
            return mask;

        mask = new ulong[stride];
        for (int x = 1; x <= xmax - 2; x++)
            mask[x >> 6] |= 1UL << (x & 63);

        return _innerMask = mask;
    }

    private int GetPartitionCount(int xmax, int ymax, int rowCount)
    {
        int requested = _options.MaxDegreeOfParallelism;
        if (requested == 1 || rowCount < 2)
            return 1;

        if ((long)xmax * ymax < _options.ParallelThreshold)
            return 1;

        int maximum = requested <= 0 ? Environment.ProcessorCount : requested;

        // At least three rows per partition, the rolling window needs them anyway.
        return Math.Max(1, Math.Min(maximum, rowCount / 3));
    }

    /// <summary>Per thread scratch space of the bit parallel path.</summary>
    private sealed class RowTriples
    {
        public RowTriples(int stride)
        {
            Sums = new ulong[3 * stride];
            Carries = new ulong[3 * stride];
        }

        public ulong[] Sums { get; }

        public ulong[] Carries { get; }
    }

    #endregion

    #region generic path

    /// <summary>
    /// Evaluates the generation cell by cell for matrices that are not bit packed.
    /// </summary>
    /// <remarks>
    /// Three rows of the source matrix are buffered so that every cell is read through
    /// <see cref="IReadableArray2D{T}.GetAt"/> exactly once, and the block sum is maintained as a
    /// sliding window along the x axis.
    /// </remarks>
    private (int Died, int Revived, int Survived) NextGeneric(int xmax, int ymax)
    {
        var rows = GetRowBuffers(xmax);
        for (int y = 0; y < 3; y++)
            LoadRow(_previous, rows[y % 3], y, xmax);

        int died = 0;
        int revived = 0;
        int survived = 0;

        for (int y = 1; y <= ymax - 2; y++)
        {
            var above = rows[(y + 2) % 3];
            var middle = rows[y % 3];
            var below = rows[(y + 1) % 3];

            int left = above[0] + middle[0] + below[0];
            int center = above[1] + middle[1] + below[1];

            for (int x = 1; x <= xmax - 2; x++)
            {
                int right = above[x + 1] + middle[x + 1] + below[x + 1];
                int self = middle[x];
                int neighbours = left + center + right - self;

                bool next;
                if (self != 0) // living cell of the previous generation
                {
                    if (neighbours is 2 or 3)
                    {
                        next = true;
                        survived++;
                    }
                    else if (_immortals is not null && _immortals.GetAt(x, y))
                    {
                        next = true; // cannot die - is immortal
                        survived++;
                    }
                    else
                    {
                        next = false;
                        died++;
                    }
                }
                else // dead cell of the previous generation
                {
                    if (neighbours == 3 && (_unviables is null || !_unviables.GetAt(x, y)))
                    {
                        next = true;
                        revived++;
                    }
                    else
                    {
                        next = false; // remains dead, possibly because it is unviable
                    }
                }

                _current.SetAt(x, y, next);

                left = center;
                center = right;
            }

            int nextRow = y + 2;
            if (nextRow < ymax)
                LoadRow(_previous, rows[nextRow % 3], nextRow, xmax);
        }

        return (died, revived, survived);
    }

    private static void LoadRow(IReadableArray2D<bool> matrix, byte[] buffer, int y, int xmax)
    {
        for (int x = 0; x < xmax; x++)
            buffer[x] = matrix.GetAt(x, y) ? (byte)1 : (byte)0;
    }

    private byte[][] GetRowBuffers(int xmax)
    {
        var buffers = _rowBuffers;
        if (buffers is not null && buffers[0].Length == xmax)
            return buffers;

        buffers = new byte[3][];
        for (int i = 0; i < buffers.Length; i++)
            buffers[i] = new byte[xmax];

        return _rowBuffers = buffers;
    }

    #endregion
}
