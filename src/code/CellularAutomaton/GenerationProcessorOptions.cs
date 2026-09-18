namespace CellularAutomaton;

/// <summary>
/// Behaviour of the <see cref="GenerationProcessor"/>.
/// </summary>
public record GenerationProcessorOptions
{
    /// <summary>
    /// When set, the outermost cells are reset to <see langword="false"/> on every generation.
    /// Otherwise they keep the value they were initialized with.
    /// </summary>
    public bool CleanBorders { get; set; }

    /// <summary>
    /// Maximum number of threads used to evaluate a generation. <c>1</c> (the default) keeps the
    /// processing on the calling thread, values below <c>1</c> mean
    /// <see cref="Environment.ProcessorCount"/>. The produced generation and statistics are the
    /// same regardless of this setting.
    /// </summary>
    /// <remarks>
    /// Parallel processing only pays off for large matrices; it is applied to the bit packed
    /// fast path only.
    /// </remarks>
    public int MaxDegreeOfParallelism { get; set; } = 1;

    /// <summary>
    /// Matrices smaller than this number of cells are always processed on a single thread.
    /// </summary>
    public int ParallelThreshold { get; set; } = 1 << 18;
}
