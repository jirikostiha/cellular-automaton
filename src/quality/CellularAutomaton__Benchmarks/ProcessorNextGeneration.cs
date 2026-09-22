using BenchmarkDotNet.Attributes;

namespace CellularAutomaton;

/// <summary>
/// Throughput of a single generation over a square matrix of <see cref="Size"/> cells per side.
/// </summary>
[MemoryDiagnoser]
public class ProcessorNextGeneration
{
    private GenerationProcessor _bitProcessor = null!;
    private GenerationProcessor _parallelBitProcessor = null!;
    private GenerationProcessor _boolProcessor = null!;

    [Params(64, 512, 4096)]
    public int Size { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        // The same seed for every matrix, so that all of them do the same amount of work.
        var random = new Random(20240115);
        var seed = new bool[Size, Size];
        for (int y = 0; y < Size; y++)
            for (int x = 0; x < Size; x++)
                seed[x, y] = random.Next(2) == 0;

        _bitProcessor = new GenerationProcessor(
            BitArray2D.Create(Size, Size, (x, y) => seed[x, y]),
            new GenerationProcessorOptions { CleanBorders = true });

        _parallelBitProcessor = new GenerationProcessor(
            BitArray2D.Create(Size, Size, (x, y) => seed[x, y]),
            new GenerationProcessorOptions
            {
                CleanBorders = true,
                MaxDegreeOfParallelism = 0,
                ParallelThreshold = 0,
            });

        _boolProcessor = new GenerationProcessor(
            BoolArray2D.Create(Size, Size, (x, y) => seed[x, y]),
            new GenerationProcessorOptions { CleanBorders = true });
    }

    [Benchmark(Baseline = true)]
    public (int, int, int) Next_BoolArray2D() => _boolProcessor.Next();

    [Benchmark]
    public (int, int, int) Next_BitArray2D() => _bitProcessor.Next();

    [Benchmark]
    public (int, int, int) Next_BitArray2D_Parallel() => _parallelBitProcessor.Next();
}
