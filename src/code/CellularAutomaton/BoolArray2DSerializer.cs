using System;
using System.Text;

namespace CellularAutomaton
{
    public class BoolArray2DSerializer
    {
        public string Separator { get; set; } = Environment.NewLine;

        public string Serialize(IArray2D<bool> array2D)
        {
            var builder = new StringBuilder();
            for (int x = 0; x < array2D.XCount; x++)
            {
                for (int y = 0; y < array2D.YCount; y++)
                {
                    if (array2D.GetAt(x,y))
                        builder.Append($"{x},{y}").Append(Separator);
                }
            }
            builder.Remove(builder.Length - Separator.Length, Separator.Length);

            return builder.ToString();
        }

        public void Deserialize(string content, IArray2D<bool> array2D)
        {
            var items = content.Split(Separator);
            var xmax = array2D.XCount - 1;
            var ymax = array2D.YCount - 1;
            foreach (var item in items)
            {
                var delimiterIndex = item.IndexOf(',');
                var x = int.Parse(item.Substring(0, delimiterIndex));
                var y = int.Parse(item.Substring(delimiterIndex + 1));

                if (x <= xmax && y <= ymax)
                    array2D.SetAt(x, y, true);
            }
        }
    }
}
