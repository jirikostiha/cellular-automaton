using System;

namespace CellularAutomaton
{
    public interface IArray2D<T> : IReadableArray2D<T>, IWritableArray2D<T>
    {
    }

    public interface IReadableArray2D<T> : ICloneable
    {
        int XCount { get; }

        int YCount { get; }

        T GetAt(int x, int y);
    }

    public interface IWritableArray2D<T>
    {
        void SetAt(int x, int y, T value);

        void SetRegion(int xA, int yA, int xB, int yB, T value);

        void Clean();
    }
}
