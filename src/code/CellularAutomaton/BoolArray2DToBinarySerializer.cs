using CommunityToolkit.Diagnostics;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CellularAutomaton
{
    public class BoolArray2DToBinarySerializer : Array2DSerializerBase
    {
        public string Serialize(BoolArray2D array2D) => base.Serialize(array2D);

        public BoolArray2D Deserialize(string content, BoolArray2D? array2D = null)
        {
            Guard.IsNotNullOrEmpty(content);

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    for (int i = 0; i < array2D.GetLength(0); i++)
                    {
                        for (int j = 0; j < array2D.GetLength(1); j++)
                        {
                            writer.Write(array2D[i, j]);
                        }
                    }

                    byte[] bytes = stream.ToArray();
                    //Console.WriteLine(BitConverter.ToString(bytes));
                }
            }

            return array2D;
        }
    }
}
