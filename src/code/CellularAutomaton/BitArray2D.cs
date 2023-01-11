using System;
using System.Collections;

namespace CellularAutomaton
{
    public class BitArray2D : IArray2D<bool>
    {
        private BitArray[] _arrayList;

        public BitArray2D(BitArray[] underlyingArray)
        {
            _arrayList = underlyingArray;
        }

        public int XCount => _arrayList.Length > 0 ? _arrayList[0].Length : 0;

        public int YCount => _arrayList.Length;

        public bool GetAt(int x, int y) => _arrayList[y][x];

        public void SetAt(int x, int y, bool value) => _arrayList[y][x] = value;

        public void SetRegion(int xA, int yA, int xB, int yB, bool value)
        {
            for (int y = yA; y <= yB; y++)
                for (int x = xA; x <= xB; x++)
                    _arrayList[y][x] = value;
        }

        public object Clone()
        {
            var bitArrayClone = new BitArray[_arrayList.Length];
            for (int i = 0; i < bitArrayClone.Length; i++)
                bitArrayClone[i] = _arrayList[i].Clone() as BitArray;

            return new BitArray2D(bitArrayClone);
        }

        public static BitArray2D Create(int xcount, int ycount, bool value = false)
        {
            var array = new BitArray[ycount];
            for (int y = 0; y < ycount; y++)
                array[y] = new BitArray(xcount, value);

            return new BitArray2D(array);
        }

        public static BitArray2D Create(int xcount, int ycount, bool innerValue, bool bounderyValue = false)
        {
            var array = new BitArray[ycount];

            // first row
            array[0] = new BitArray(xcount, bounderyValue);

            // inner rows
            for (int y = 1; y < ycount - 1; y++)
            {
                array[y] = new BitArray(xcount, innerValue);
                array[y][0] = bounderyValue;
                array[y][xcount - 1] = bounderyValue;
            }

            //last row
            array[ycount - 1] = new BitArray(xcount, bounderyValue);

            return new BitArray2D(array);
        }

        public static BitArray2D Create(int xcount, int ycount, Func<int, int, bool> bitValueProvider)
        {
            var array = new BitArray[ycount];
            for (int i = 0; i < array.Length; i++)
                array[i] = new BitArray(xcount, false);

            for (var y = 0; y < ycount; y++)
                for (var x = 0; x < xcount; x++)
                    array[y].Set(x, bitValueProvider(x, y));

            return new BitArray2D(array);
        }
    }
}
