namespace CellularAutomaton;

using Xunit;

/// <summary>
/// Verifies that every code path of <see cref="GenerationProcessor"/> - the bit parallel one, its
/// parallelized variant and the generic fallback - produces exactly the same generations and
/// statistics as <see cref="ReferenceProcessor"/>.
/// </summary>
public class GenerationProcessorParityTests
{
    // Sizes around the 64 bit word boundaries, where the bit parallel path is most fragile.
    public static TheoryData<int, int> Sizes { get; } = new()
    {
        { 3, 3 }, { 4, 4 }, { 5, 9 }, { 9, 5 }, { 16, 3 }, { 3, 16 },
        { 63, 7 }, { 64, 7 }, { 65, 7 }, { 66, 7 }, { 127, 5 }, { 128, 5 },
        { 129, 5 }, { 130, 11 }, { 200, 37 }, { 37, 200 },
    };

    [Theory]
    [MemberData(nameof(Sizes))]
    public void BitArray2D_MatchesReference(int xcount, int ycount)
        => AssertParity(xcount, ycount, cleanBorders: false, flags: false, parallel: false, BitArray2D.Create);

    [Theory]
    [MemberData(nameof(Sizes))]
    public void BitArray2D_CleanBorders_MatchesReference(int xcount, int ycount)
        => AssertParity(xcount, ycount, cleanBorders: true, flags: false, parallel: false, BitArray2D.Create);

    [Theory]
    [MemberData(nameof(Sizes))]
    public void BitArray2D_WithFlags_MatchesReference(int xcount, int ycount)
        => AssertParity(xcount, ycount, cleanBorders: false, flags: true, parallel: false, BitArray2D.Create);

    [Theory]
    [MemberData(nameof(Sizes))]
    public void BoolArray2D_MatchesReference(int xcount, int ycount)
        => AssertParity(xcount, ycount, cleanBorders: false, flags: false, parallel: false, BoolArray2D.Create);

    [Theory]
    [MemberData(nameof(Sizes))]
    public void BoolArray2D_CleanBordersWithFlags_MatchesReference(int xcount, int ycount)
        => AssertParity(xcount, ycount, cleanBorders: true, flags: true, parallel: false, BoolArray2D.Create);

    [Theory]
    [InlineData(200, 61)]
    [InlineData(64, 128)]
    [InlineData(1000, 17)]
    public void Parallel_MatchesReference(int xcount, int ycount)
    {
        AssertParity(xcount, ycount, cleanBorders: false, flags: false, parallel: true, BitArray2D.Create);
        AssertParity(xcount, ycount, cleanBorders: true, flags: true, parallel: true, BitArray2D.Create);
    }

    [Fact]
    public void MixedFlagImplementations_MatchReference()
    {
        // Bit packed matrix with non bit packed flags has to fall back to the generic path.
        var seed = Seed(xcount: 40, ycount: 20, 1234);
        var immortals = Seed(40, 20, 99, density: 8);
        var unviables = Seed(40, 20, 77, density: 8);

        var reference = new ReferenceProcessor((bool[,])seed.Clone(), cleanBorders: false, immortals, unviables);
        var processor = new GenerationProcessor(ToBit(seed))
        {
            Immortals = ToBool(immortals),
            Unviables = ToBool(unviables),
        };

        for (int generation = 0; generation < 8; generation++)
            AssertSameGeneration(reference, processor, generation);
    }

    private static void AssertParity(
        int xcount,
        int ycount,
        bool cleanBorders,
        bool flags,
        bool parallel,
        Func<int, int, Func<int, int, bool>, IArray2D<bool>> creator)
    {
        var seed = Seed(xcount, ycount, (xcount * 31) + ycount);
        var immortals = flags ? Seed(xcount, ycount, 11, density: 10) : null;
        var unviables = flags ? Seed(xcount, ycount, 13, density: 10) : null;

        var reference = new ReferenceProcessor((bool[,])seed.Clone(), cleanBorders, immortals, unviables);
        var options = new GenerationProcessorOptions
        {
            CleanBorders = cleanBorders,
            MaxDegreeOfParallelism = parallel ? 4 : 1,
            ParallelThreshold = parallel ? 0 : 1 << 18,
        };
        var processor = new GenerationProcessor(creator(xcount, ycount, (x, y) => seed[x, y]), options);
        if (flags)
        {
            processor.Immortals = creator(xcount, ycount, (x, y) => immortals![x, y]);
            processor.Unviables = creator(xcount, ycount, (x, y) => unviables![x, y]);
        }

        for (int generation = 0; generation < 12; generation++)
            AssertSameGeneration(reference, processor, generation);
    }

    private static void AssertSameGeneration(ReferenceProcessor reference, GenerationProcessor processor, int generation)
    {
        var expected = reference.Next();
        var actual = processor.Next();

        Assert.Equal(expected, actual);

        var matrix = processor.Matrix;
        for (int y = 0; y < matrix.YCount; y++)
        {
            for (int x = 0; x < matrix.XCount; x++)
            {
                Assert.True(
                    reference.GetAt(x, y) == matrix.GetAt(x, y),
                    $"generation {generation}, cell [{x},{y}]: expected {reference.GetAt(x, y)}.");
            }
        }
    }

    private static bool[,] Seed(int xcount, int ycount, int seed, int density = 2)
    {
        var random = new Random(seed);
        var cells = new bool[xcount, ycount];
        for (int x = 0; x < xcount; x++)
            for (int y = 0; y < ycount; y++)
                cells[x, y] = random.Next(density) == 0;

        return cells;
    }

    private static BitArray2D ToBit(bool[,] cells)
        => BitArray2D.Create(cells.GetLength(0), cells.GetLength(1), (x, y) => cells[x, y]);

    private static BoolArray2D ToBool(bool[,] cells)
        => BoolArray2D.Create(cells.GetLength(0), cells.GetLength(1), (x, y) => cells[x, y]);
}
