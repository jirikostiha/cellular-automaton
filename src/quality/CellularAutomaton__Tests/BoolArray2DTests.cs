namespace CellularAutomaton
{
    using Xunit;

    public class BoolArray2DTests
    {
        [Fact]
        public void New_CorrectSize()
        {
            Assert.Equal(3, BoolArray2D.Create(3, 1).XCount);
            Assert.Equal(1, BoolArray2D.Create(3, 1).YCount);
        }

        [Fact]
        public void GetSetAt_CorrectCells()
        {
            var gen = BoolArray2D.Create(5, 1);
            gen.SetAt(4, 0, true);

            Assert.True(gen.GetAt(4, 0));
            Assert.False(gen.GetAt(3, 0));
        }
    }
}
