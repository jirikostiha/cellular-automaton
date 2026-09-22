namespace CellularAutomaton;

/// <summary>
/// Read only wrapper around an <see cref="IReadableArray2D{T}"/>.
/// </summary>
public readonly struct ReadonlyArray2D<T> : IReadableArray2D<T>, IEquatable<ReadonlyArray2D<T>>
{
    private readonly IReadableArray2D<T>? _array;

    public ReadonlyArray2D(IReadableArray2D<T> underlyingArray)
    {
        _array = underlyingArray ?? throw new ArgumentNullException(nameof(underlyingArray));
    }

    /// <summary>Indicates that the wrapper does not reference any array.</summary>
    public bool IsEmpty => _array is null;

    public int XCount => _array?.XCount ?? 0;

    public int YCount => _array?.YCount ?? 0;

    public T GetAt(int x, int y) => Underlying.GetAt(x, y);

    public object Clone() => Underlying.Clone();

    private IReadableArray2D<T> Underlying
        => _array ?? throw new InvalidOperationException("The array wrapper is not initialized.");

    public bool Equals(ReadonlyArray2D<T> other) => ReferenceEquals(_array, other._array);

    public override bool Equals(object? obj) => obj is ReadonlyArray2D<T> other && Equals(other);

    public override int GetHashCode() => _array?.GetHashCode() ?? 0;

    public static bool operator ==(ReadonlyArray2D<T> left, ReadonlyArray2D<T> right) => left.Equals(right);

    public static bool operator !=(ReadonlyArray2D<T> left, ReadonlyArray2D<T> right) => !left.Equals(right);
}
