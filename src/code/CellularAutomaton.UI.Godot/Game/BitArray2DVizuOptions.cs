using Godot;

namespace CellularAutomaton.UI.Godot;

/// <summary>
/// Colors used to render a boolean matrix.
/// </summary>
public record BitArray2DVizuOptions
{
    public Color FalseColor { get; set; } = Colors.Black;

    public Color TrueColor { get; set; } = Colors.White;
}
