using System;

namespace CellularAutomaton
{
    public interface IArray2D<T> : ICloneable
    {
        int XCount { get; }

        int YCount { get; }

        T GetAt(int x, int y);

        void SetAt(int x, int y, T value);

        void SetRegion(int xA, int yA, int xB, int yB, T value);
    }
}
