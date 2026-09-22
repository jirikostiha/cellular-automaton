namespace CellularAutomaton;

/// <summary>
/// Converts a <see cref="BoolArray2D"/> from and to text.
/// </summary>
public class BoolArray2DSerializer : Array2DSerializerBase
{
    public string Serialize(BoolArray2D array2D) => base.Serialize(array2D);

    /// <summary>
    /// Fills <paramref name="array2D"/> with the content, or creates a matrix fitting the content
    /// when no matrix is given.
    /// </summary>
    public BoolArray2D Deserialize(string content, BoolArray2D? array2D = null)
    {
        if (array2D is null)
        {
            var size = MeasureContent(content);
            array2D = BoolArray2D.Create(size.X, size.Y);
        }

        Deserialize(content, array2D, array2D.XCount, array2D.YCount);

        return array2D;
    }
}
