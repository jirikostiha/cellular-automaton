using CommunityToolkit.Diagnostics;
using System;
using System.Linq;
using System.Text;

namespace CellularAutomaton
{
    public class GoLSerializerBase : ISerializer
    {
        public string Serialize(IArray2D<bool> array2D)
        {
            return default;
        }

        public IArray2D<bool> Deserialize(string content)
        {
            throw new NotImplementedException();
        }
    }
}
