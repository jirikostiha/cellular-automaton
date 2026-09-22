using Godot;

namespace CellularAutomaton.UI.Godot;

/// <summary>
/// Autoloaded singleton with the random helpers shared by the scenes.
/// </summary>
public partial class AppGlobal : Node
{
    public static int NextInt(int min, int max) => Random.Shared.Next(min, max);

    public static float NextFloat(float inclusiveMin, float exclusiveMax)
        => inclusiveMin + (Random.Shared.NextSingle() * (exclusiveMax - inclusiveMin));

    public static bool NextBool() => Random.Shared.Next(2) == 1;
}
