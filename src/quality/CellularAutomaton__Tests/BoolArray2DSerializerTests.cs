namespace CellularAutomaton
{
    using System.Collections;
    using System.Linq;
    using Xunit;

    public class BoolArray2DSerializerTests
    {
        [Fact]
        public void Serialize()
        {
            var array2D = BitArray2D.Create(4, 4, true, false);
            var serializer = new BoolArray2DSerializer();

            var content = serializer.Serialize(array2D);
            
            Assert.Equal(4, content.Split(serializer.Separator).Length);
            Assert.Equal("1,1", content.Split(serializer.Separator).First());
        }

        [Fact]
        public void Deserialize()
        {
            var array2D = BitArray2D.Create(4, 4, true, false);
            var serializer = new BoolArray2DSerializer();
            var content = serializer.Serialize(array2D);

            var newArray2D = BitArray2D.Create(4, 4, false);
            serializer.Deserialize(content, newArray2D);

            Assert.False(newArray2D.GetAt(0, 0));
            Assert.False(newArray2D.GetAt(3, 3));
            Assert.True(newArray2D.GetAt(1, 1));
            Assert.True(newArray2D.GetAt(1, 2));
            Assert.True(newArray2D.GetAt(2, 2));
        }
    }
}
