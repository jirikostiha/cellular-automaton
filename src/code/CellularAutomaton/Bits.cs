using System.Runtime.CompilerServices;

namespace CellularAutomaton;

/// <summary>
/// Bit level helpers shared by the bit packed matrix and the generation processor.
/// </summary>
internal static class Bits
{
    /// <summary>Number of bits set in <paramref name="value"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PopCount(ulong value)
    {
#if NET
        return System.Numerics.BitOperations.PopCount(value);
#else
        // SWAR fallback for targets without System.Numerics.BitOperations.
        value -= (value >> 1) & 0x5555555555555555UL;
        value = (value & 0x3333333333333333UL) + ((value >> 2) & 0x3333333333333333UL);
        value = (value + (value >> 4)) & 0x0F0F0F0F0F0F0F0FUL;

        return (int)((value * 0x0101010101010101UL) >> 56);
#endif
    }
}
