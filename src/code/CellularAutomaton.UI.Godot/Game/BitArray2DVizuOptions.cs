using Godot;

namespace CellularAutomaton.UI.Godot
{
    public record BitArray2DVizuOptions
    {
        public Color FalseColor { get; set; } = new Color("black");

        public Color TrueColor { get; set; } = new Color("white");
    }
}