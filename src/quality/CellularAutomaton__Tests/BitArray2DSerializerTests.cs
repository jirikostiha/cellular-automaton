namespace CellularAutomaton
{
    using System;
    using System.Collections;
    using System.Linq;
    using Xunit;

    public class BitArray2DSerializerTests
    {
        [Fact]
        public void Serialize()
        {
            var array2D = BitArray2D.Create(4, 5, true, false);
            var serializer = new BitArray2DSerializer();

            var content = serializer.Serialize(array2D);

            Assert.Equal(5, content.Split(Environment.NewLine).Length);
            Assert.Equal(4, content.Split(Environment.NewLine).First().Length);
            Assert.Equal(4, content.Split(Environment.NewLine).Last().Length);
        }

        [Fact]
        public void Deserialize_ToNewArray()
        {
            var array2D = BitArray2D.Create(4, 3, true, false);
            var serializer = new BitArray2DSerializer();
            var content = serializer.Serialize(array2D);

            var newArray2D = BitArray2D.Create(4, 3, false);
            serializer.Deserialize(content, newArray2D);

            Assert.False(newArray2D.GetAt(0, 0));
            Assert.False(newArray2D.GetAt(3, 2));
            Assert.True(newArray2D.GetAt(1, 1));
            Assert.True(newArray2D.GetAt(1, 1));
            Assert.True(newArray2D.GetAt(2, 1));
        }

        [Fact]
        public void Deserialize_ToNewSmallerArray()
        {
            var array2D = BitArray2D.Create(4, 3, true, false);
            var serializer = new BitArray2DSerializer();
            var content = serializer.Serialize(array2D);

            var newArray2D = BitArray2D.Create(3, 2, false);
            serializer.Deserialize(content, newArray2D);

            Assert.False(newArray2D.GetAt(0, 0));
            Assert.True(newArray2D.GetAt(2, 1));
            Assert.True(newArray2D.GetAt(1, 1));
            Assert.True(newArray2D.GetAt(1, 1));
            Assert.True(newArray2D.GetAt(2, 1));
        }
    }
}
