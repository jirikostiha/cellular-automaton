using Godot;

namespace CellularAutomaton.UI.Godot
{
    public record BitArray2DVizuOptions
    {
        public Color FalseColor { get; set; } = Color.ColorN("black");

        public Color TrueColor { get; set; } = Color.ColorN("white");
    }
}