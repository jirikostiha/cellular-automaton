namespace CellularAutomaton;

/// <summary>
/// Dimensions of a two dimensional matrix.
/// </summary>
public readonly record struct MatrixSize(int X, int Y)
{
    /// <summary>Total number of cells.</summary>
    public long Count => (long)X * Y;
}
