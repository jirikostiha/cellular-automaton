using Godot;
using System;

namespace CellularAutomaton.UI.Godot
{
    public class AppGlobal : Node
    {
        private static Func<float> _uinProvider;

        static AppGlobal()
        {
            var rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            _uinProvider = () => (float)rnd.NextDouble();
        }

        public static int NextInt(int min, int max) => min + Convert.ToInt32(_uinProvider() * (max - min));

        public static float NextFloat(float inclusiveMin, float exclusiveMax) => inclusiveMin + _uinProvider() * (exclusiveMax - inclusiveMin);

        public static bool NextBool() => _uinProvider() < 0.5f ? false : true;
    }
}