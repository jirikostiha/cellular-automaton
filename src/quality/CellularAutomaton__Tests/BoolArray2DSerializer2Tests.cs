namespace CellularAutomaton
{
    using System;
    using System.Collections;
    using System.Linq;
    using Xunit;

    public class BoolArray2DSerializer2Tests
    {
        [Fact]
        public void Serialize()
        {
            var array2D = BitArray2D.Create(4, 11, true, false);
            var serializer = new BoolArray2DSerializer2((xcount, ycount) =>
                BitArray2D.Create(xcount, ycount, true));

            var content = serializer.Serialize(array2D);
            
            Assert.Equal("size:4x11", content.Split(Environment.NewLine).First());
        }

        [Fact]
        public void Deserialize()
        {
            var array2D = BitArray2D.Create(4, 11, true, false);
            var serializer = new BoolArray2DSerializer2((xcount, ycount) =>
                BitArray2D.Create(xcount, ycount, true));
            var content = serializer.Serialize(array2D);

            var newArray2D = serializer.Deserialize(content);

            Assert.Equal(4, newArray2D.XCount);
            Assert.Equal(11, newArray2D.YCount);
            Assert.False(newArray2D.GetAt(0, 0));
            Assert.True(newArray2D.GetAt(1, 1));
        }
    }
}
