namespace CellularAutomaton;

using System.Collections;
using Xunit;

/// <summary>
/// Behaviour shared by every <see cref="IArray2D{T}"/> implementation.
/// </summary>
public class Array2DTests
{
    public static TheoryData<string> Implementations { get; } = new() { "bit", "bool" };

    [Theory]
    [MemberData(nameof(Implementations))]
    public void Create_ReportsGivenSize(string implementation)
    {
        var array2D = Create(implementation, 70, 5);

        Assert.Equal(70, array2D.XCount);
        Assert.Equal(5, array2D.YCount);
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void Create_WithValue_FillsEveryCell(string implementation)
    {
        var array2D = Create(implementation, 130, 3, true);

        for (int y = 0; y < array2D.YCount; y++)
            for (int x = 0; x < array2D.XCount; x++)
                Assert.True(array2D.GetAt(x, y), $"[{x},{y}]");
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void Create_WithBoundary_SeparatesInnerAndOuterCells(string implementation)
    {
        var array2D = CreateBounded(implementation, 100, 4, innerValue: true, boundaryValue: false);

        for (int y = 0; y < array2D.YCount; y++)
        {
            for (int x = 0; x < array2D.XCount; x++)
            {
                bool isInner = x > 0 && y > 0 && x < array2D.XCount - 1 && y < array2D.YCount - 1;
                Assert.True(isInner == array2D.GetAt(x, y), $"[{x},{y}]");
            }
        }
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void SetAt_AffectsOnlyTheAddressedCell(string implementation)
    {
        var array2D = Create(implementation, 200, 4);

        array2D.SetAt(64, 2, true);
        array2D.SetAt(127, 2, true);

        for (int y = 0; y < array2D.YCount; y++)
            for (int x = 0; x < array2D.XCount; x++)
                Assert.True((y == 2 && x is 64 or 127) == array2D.GetAt(x, y), $"[{x},{y}]");
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void SetRegion_SpansWordBoundaries(string implementation)
    {
        var array2D = Create(implementation, 200, 6);

        array2D.SetRegion(5, 1, 150, 4, true);

        for (int y = 0; y < array2D.YCount; y++)
        {
            for (int x = 0; x < array2D.XCount; x++)
            {
                bool inRegion = x >= 5 && x <= 150 && y >= 1 && y <= 4;
                Assert.True(inRegion == array2D.GetAt(x, y), $"[{x},{y}]");
            }
        }

        array2D.SetRegion(5, 1, 150, 4, false);
        array2D.SetRegion(63, 0, 64, 0, true);

        Assert.False(array2D.GetAt(62, 0));
        Assert.True(array2D.GetAt(63, 0));
        Assert.True(array2D.GetAt(64, 0));
        Assert.False(array2D.GetAt(65, 0));
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void Clean_ResetsEveryCell(string implementation)
    {
        var array2D = Create(implementation, 100, 3, true);

        array2D.Clean();

        for (int y = 0; y < array2D.YCount; y++)
            for (int x = 0; x < array2D.XCount; x++)
                Assert.False(array2D.GetAt(x, y), $"[{x},{y}]");
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void Clone_IsIndependentCopy(string implementation)
    {
        var array2D = Create(implementation, 100, 3);
        array2D.SetAt(70, 1, true);

        var clone = (IArray2D<bool>)array2D.Clone();
        clone.SetAt(10, 1, true);

        Assert.True(clone.GetAt(70, 1));
        Assert.False(array2D.GetAt(10, 1));
        Assert.Equal(array2D.XCount, clone.XCount);
        Assert.Equal(array2D.YCount, clone.YCount);
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void GetAt_OutOfRange_Throws(string implementation)
    {
        var array2D = Create(implementation, 10, 4);

        Assert.Throws<ArgumentOutOfRangeException>(() => array2D.GetAt(10, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => array2D.GetAt(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => array2D.GetAt(0, 4));
        Assert.Throws<ArgumentOutOfRangeException>(() => array2D.SetAt(0, -1, true));
    }

    [Theory]
    [MemberData(nameof(Implementations))]
    public void Create_EmptyMatrix_IsSupported(string implementation)
    {
        var array2D = Create(implementation, 0, 0);

        Assert.Equal(0, array2D.XCount);
        Assert.Equal(0, array2D.YCount);
        array2D.Clean();
    }

    [Fact]
    public void BitArray2D_FromBitArrays_CopiesContent()
    {
        var rows = new[] { new BitArray(70), new BitArray(70) };
        rows[1][65] = true;

        var array2D = new BitArray2D(rows);
        rows[0][0] = true; // the source is not aliased

        Assert.Equal(70, array2D.XCount);
        Assert.Equal(2, array2D.YCount);
        Assert.True(array2D.GetAt(65, 1));
        Assert.False(array2D.GetAt(0, 0));
    }

    [Fact]
    public void BoolArray2D_FromArray_CopiesContent()
    {
        var cells = new bool[3, 2];
        cells[2, 1] = true;

        var array2D = new BoolArray2D(cells);
        cells[0, 0] = true; // the source is not aliased

        Assert.Equal(3, array2D.XCount);
        Assert.Equal(2, array2D.YCount);
        Assert.True(array2D.GetAt(2, 1));
        Assert.False(array2D.GetAt(0, 0));
    }

    private static IArray2D<bool> Create(string implementation, int xcount, int ycount, bool value = false)
        => implementation == "bit"
            ? BitArray2D.Create(xcount, ycount, value)
            : BoolArray2D.Create(xcount, ycount, value);

    private static IArray2D<bool> CreateBounded(
        string implementation, int xcount, int ycount, bool innerValue, bool boundaryValue)
        => implementation == "bit"
            ? BitArray2D.Create(xcount, ycount, innerValue, boundaryValue)
            : BoolArray2D.Create(xcount, ycount, innerValue, boundaryValue);
}
