using System;

namespace CellularAutomaton
{
    public class BoolArray2D : IArray2D<bool>
    {
        private bool[,] _array;

        public BoolArray2D(bool[,] underlyingArray)
        {
            _array = underlyingArray;
        }

        public int XCount => _array.Length > 0 ? _array.GetLength(0) : 0;

        public int YCount => _array.Length > 0 ? _array.GetLength(1) : 0;

        public bool GetAt(int x, int y) => _array[x,y];

        public void SetAt(int x, int y, bool value) => _array[x,y] = value;

        public void SetRegion(int xA, int yA, int xB, int yB, bool value)
        {
            for (int y = yA; y <= yB; y++)
                for (int x = xA; x <= xB; x++)
                    _array[x,y] = value;
        }

        public void Clean() => SetRegion(0, 0, XCount - 1, YCount - 1, false);

        public object Clone() => new BoolArray2D((bool[,])_array.Clone());

        public static BoolArray2D Create(int xcount, int ycount, bool value = false)
        {
            var array = new bool[xcount, ycount];
            if (value)
            {
                for (int y = 0; y < ycount; y++)
                    for (int x = 0; x < xcount; x++)
                        array[x, y] = value;
            }

            return new BoolArray2D(array);
        }

        public static BoolArray2D Create(int xcount, int ycount, bool innerValue, bool bounderyValue = false)
        {
            var array = Create(xcount, ycount, innerValue);
            // first row
            array.SetRegion(0, 0, xcount - 1, 0, bounderyValue);
            //last row
            array.SetRegion(0, ycount - 1, xcount - 1, ycount - 1, bounderyValue);
            //left row
            array.SetRegion(0, 1, 0, ycount - 1, bounderyValue);
            //right row
            array.SetRegion(xcount - 1, 1, xcount - 1, ycount - 1, bounderyValue);

            return array;
        }

        public static BoolArray2D Create(int xcount, int ycount, Func<int, int, bool> bitValueProvider)
        {
            var array = new bool[xcount, ycount];
            for (var y = 0; y < ycount; y++)
                for (var x = 0; x < xcount; x++)
                    array[x,y] = bitValueProvider(x, y);

            return new BoolArray2D(array);
        }
    }
}
