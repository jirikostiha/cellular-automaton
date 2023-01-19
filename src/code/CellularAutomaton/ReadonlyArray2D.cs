namespace CellularAutomaton
{
    public struct ReadonlyArray2D<T> : IReadableArray2D<T>
    {
        private readonly IArray2D<T> _array;

        public ReadonlyArray2D(IArray2D<T> underlyingArray)
        {
            _array = underlyingArray;
        }

        public int XCount => _array.XCount;

        public int YCount => _array.YCount;

        public T GetAt(int x, int y) => _array.GetAt(x,y);

        public object Clone() => _array.Clone();
    }
}
