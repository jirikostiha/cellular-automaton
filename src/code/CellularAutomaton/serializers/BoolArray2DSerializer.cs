using CommunityToolkit.Diagnostics;
using System;
using System.Linq;
using System.Text;

namespace CellularAutomaton
{
    public class BoolArray2DSerializer : Array2DSerializerBase
    {
        public string? Serialize(BoolArray2D? array2D) => base.Serialize(array2D);

        public BoolArray2D Deserialize(string content, BoolArray2D? array2D = null)
        {
            Guard.IsNotNullOrEmpty(content);

            var lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var xmax = lines.First().Length;
            array2D ??= BoolArray2D.Create(xmax, lines.Length);

            for (int y = 0; y < Math.Min(lines.Length, array2D.YCount); y++)
            {
                for (int x = 0; x < Math.Min(lines[y].Length, array2D.XCount); x++)
                    array2D.SetAt(x, y, lines[y][x] == TrueValue);
            }

            return array2D;
        }

        public override IArray2D<bool>? Deserialize(string? content)
        {
            throw new NotImplementedException();
        }
    }
}
