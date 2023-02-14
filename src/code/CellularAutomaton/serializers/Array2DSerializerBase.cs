using System;
using System.Text;

namespace CellularAutomaton
{
    public abstract class Array2DSerializerBase : ISerializer
    {
        public char TrueValue { get; set; } = '1';
        public char FalseValue { get; set; } = ' ';

        public string? Serialize(IArray2D<bool>? array2D)
        {
            if (array2D is null)
                return null;

            if (array2D.XCount == 0 || array2D.YCount == 0)
                return string.Empty;

            var builder = new StringBuilder();
            for (int y = 0; y < array2D.YCount; y++)
            {
                for (int x = 0; x < array2D.XCount; x++)
                    builder.Append(array2D.GetAt(x, y) ? TrueValue : FalseValue);

                builder.AppendLine();
            }

            builder.Remove(builder.Length - Environment.NewLine.Length, Environment.NewLine.Length);

            return builder.ToString();
        }

        public abstract IArray2D<bool>? Deserialize(string? content);
    }
}
