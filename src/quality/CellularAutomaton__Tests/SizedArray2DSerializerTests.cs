namespace CellularAutomaton;

using Xunit;

public class SizedArray2DSerializerTests
{
    [Fact]
    public void Serialize_WritesSizeHeader()
    {
        var array2D = BitArray2D.Create(4, 11, true, false);
        var serializer = new SizedArray2DSerializer((xcount, ycount) => BitArray2D.Create(xcount, ycount));

        var content = serializer.Serialize(array2D);

        Assert.Equal("size:4x11", content.Split(Environment.NewLine).First());
    }

    [Fact]
    public void Deserialize_RestoresSizeAndContent()
    {
        var array2D = BitArray2D.Create(4, 11, true, false);
        var serializer = new SizedArray2DSerializer((xcount, ycount) => BitArray2D.Create(xcount, ycount));
        var content = serializer.Serialize(array2D);

        var newArray2D = serializer.Deserialize(content);

        Assert.Equal(4, newArray2D.XCount);
        Assert.Equal(11, newArray2D.YCount);
        Assert.False(newArray2D.GetAt(0, 0));
        Assert.True(newArray2D.GetAt(1, 1));
    }

    [Fact]
    public void Deserialize_RoundTripsEveryCell()
    {
        var random = new Random(4242);
        var array2D = BitArray2D.Create(70, 9, (_, _) => random.Next(2) == 1);
        var serializer = new SizedArray2DSerializer((xcount, ycount) => BitArray2D.Create(xcount, ycount));

        var restored = serializer.Deserialize(serializer.Serialize(array2D));

        for (int y = 0; y < array2D.YCount; y++)
            for (int x = 0; x < array2D.XCount; x++)
                Assert.Equal(array2D.GetAt(x, y), restored.GetAt(x, y));
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void Deserialize_AcceptsBothLineEndings(string newLine)
    {
        var serializer = new SizedArray2DSerializer((xcount, ycount) => BoolArray2D.Create(xcount, ycount));
        var content = string.Join(newLine, "size:3x2", "1 1", " 1 ");

        var array2D = serializer.Deserialize(content);

        Assert.Equal(3, array2D.XCount);
        Assert.Equal(2, array2D.YCount);
        Assert.True(array2D.GetAt(0, 0));
        Assert.False(array2D.GetAt(1, 0));
        Assert.True(array2D.GetAt(1, 1));
        Assert.False(array2D.GetAt(2, 1));
    }

    [Theory]
    [InlineData("no header at all")]
    [InlineData("size:axb\n1")]
    [InlineData("width:1x1\n1")]
    public void Deserialize_MalformedHeader_Throws(string content)
    {
        var serializer = new SizedArray2DSerializer((xcount, ycount) => BoolArray2D.Create(xcount, ycount));

        Assert.Throws<FormatException>(() => serializer.Deserialize(content));
    }
}
