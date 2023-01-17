using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CellularAutomaton
{
    public class BoolArray2DSerializer2
    {
        private readonly BoolArray2DSerializer _arraySerializer;
        private Func<int, int, IArray2D<bool>> _arrayCreator;

        public BoolArray2DSerializer2(Func<int, int, IArray2D<bool>> arrayCreator, BoolArray2DSerializer? underlyinSerializer = null)
        {
            _arraySerializer = underlyinSerializer ?? new BoolArray2DSerializer();
            _arrayCreator = arrayCreator;
        }

        public string Serialize(IArray2D<bool> array2D)
            => $"size:{array2D.XCount}x{array2D.YCount}"
                + Environment.NewLine
                + _arraySerializer.Serialize(array2D);

        public IArray2D<bool> Deserialize(string contentWithHeader)
        {
            var header = contentWithHeader.Substring(0, contentWithHeader.IndexOf(Environment.NewLine));
            var size = Regex.Match(header, "size:([\\d]*)x([\\d]*)");
            var xmax = int.Parse(size.Groups[1].Value);
            var ymax = int.Parse(size.Groups[2].Value);
            IArray2D<bool> array2D = _arrayCreator(xmax, ymax);
            array2D.SetRegion(0, 0, xmax - 1, ymax - 1, false);
            string body = contentWithHeader.Substring(contentWithHeader.IndexOf(Environment.NewLine) + 1);
            _arraySerializer.Deserialize(body, array2D);

            return array2D;
        }
    }
}
