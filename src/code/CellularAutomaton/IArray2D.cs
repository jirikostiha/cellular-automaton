namespace CellularAutomaton;

/// <summary>
/// Two dimensional array of <typeparamref name="T"/> that can be both read and written.
/// </summary>
public interface IArray2D<T> : IReadableArray2D<T>, IWritableArray2D<T>
{
}

/// <summary>
/// Read only view over a two dimensional array of <typeparamref name="T"/>.
/// </summary>
public interface IReadableArray2D<T> : ICloneable
{
    /// <summary>Number of cells along the x axis.</summary>
    int XCount { get; }

    /// <summary>Number of cells along the y axis.</summary>
    int YCount { get; }

    /// <summary>Gets the value of the cell at <paramref name="x"/>, <paramref name="y"/>.</summary>
    T GetAt(int x, int y);
}

/// <summary>
/// Write only view over a two dimensional array of <typeparamref name="T"/>.
/// </summary>
public interface IWritableArray2D<T>
{
    /// <summary>Sets the value of the cell at <paramref name="x"/>, <paramref name="y"/>.</summary>
    void SetAt(int x, int y, T value);

    /// <summary>Sets every cell of the inclusive rectangle to <paramref name="value"/>.</summary>
    void SetRegion(int xA, int yA, int xB, int yB, T value);

    /// <summary>Resets every cell to its default value.</summary>
    void Clean();
}
