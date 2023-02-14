namespace CellularAutomaton
{
    public interface ISerializer<T> : ISerializer
    {
        T? Deserialize(string? content, T? array2D = default);
    }

    public interface ISerializer
    {
        string? Serialize(IArray2D<bool>? array2D);

        IArray2D<bool>? Deserialize(string? content);
    }
}
