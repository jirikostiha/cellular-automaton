using CommunityToolkit.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CellularAutomaton
{
    public class BoolArray2DToGoLSerializer : GoLSerializerBase, ISerializer<BoolArray2D>
    {
        public string Serialize(IArray2D<bool> array2D)
        {
            return default;
        }

        public BoolArray2D Deserialize(string content, BoolArray2D? array2D = null)
        {
            Guard.IsNotNullOrEmpty(content);


            return array2D;
        }

        public BoolArray2D Deserialize(string encoded) => new BoolArray2D(DeserializeRaw(encoded));

        private bool[,] DeserializeRaw(string encoded)
        {
            string[] rows = encoded.Split('$');
            int width = rows[0].Length;
            int height = rows.Length;
            bool[,] decoded = new bool[width, height];

            for (int y = 0; y < height; y++)
            {
                int x = 0;
                for (int i = 0; i < rows[y].Length; i++)
                {
                    char c = rows[y][i];
                    if (char.IsDigit(c))
                    {
                        int count = int.Parse(c.ToString());
                        i++;
                        c = rows[y][i];
                        for (int j = 0; j < count; j++)
                        {
                            decoded[x, y] = (c == 'o');
                            x++;
                        }
                    }
                    else
                    {
                        decoded[x, y] = (c == 'o');
                        x++;
                    }
                }
            }

            return decoded;
        }
    }
}
