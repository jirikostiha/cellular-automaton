namespace CellularAutomaton
{
    using Xunit;

    public class GenerationProcessorTests
    {
        [Fact]
        public void Next_3x3AllFalse_WholeMatrixIsFalse()
        {
            var processor = new GenerationProcessor(BitArray2D.Create(3, 3));

            var stats = processor.Next();

            for (int x = 0; x < processor.Matrix.XCount; x++)
                for (int y = 0; y < processor.Matrix.YCount; y++)
                    Assert.False(processor.Matrix.GetAt(x, y));

            Assert.Equal(0, stats.Died);
            Assert.Equal(0, stats.Resurested);
            Assert.Equal(0, stats.Survived);
        }

        [Fact]
        public void Next_2x2True_AllTrue()
        {
            var matrix = BitArray2D.Create(2, 2, true);
            var processor = new GenerationProcessor(matrix);

            var stats = processor.Next();

            Assert.True(processor.Matrix.GetAt(0, 0));
            Assert.True(processor.Matrix.GetAt(1, 0));
            Assert.True(processor.Matrix.GetAt(0, 1));
            Assert.True(processor.Matrix.GetAt(1, 1));

            Assert.Equal(0, stats.Died);
            Assert.Equal(0, stats.Resurested);
            Assert.Equal(0, stats.Survived);
        }

        [Fact]
        public void Next_4x4InnersTrue_AllInnersSurvived()
        {
            var matrix = BitArray2D.Create(4, 4, true, false);
            var processor = new GenerationProcessor(matrix);

            var stats = processor.Next();

            Assert.True(processor.Matrix.GetAt(1, 1));
            Assert.True(processor.Matrix.GetAt(2, 1));
            Assert.True(processor.Matrix.GetAt(1, 2));
            Assert.True(processor.Matrix.GetAt(2, 2));

            Assert.Equal(0, stats.Died);
            Assert.Equal(0, stats.Resurested);
            Assert.Equal(4, stats.Survived);
        }

        [Fact]
        public void Next_4x4InnersFalseBoundTrue_NoChange()
        {
            var matrix = BitArray2D.Create(4, 4, false, true);
            var processor = new GenerationProcessor(matrix);

            var stats = processor.Next();

            //inners
            Assert.False(processor.Matrix.GetAt(1, 1));
            Assert.False(processor.Matrix.GetAt(2, 1));
            Assert.False(processor.Matrix.GetAt(1, 2));
            Assert.False(processor.Matrix.GetAt(2, 2));

            //corners
            Assert.True(processor.Matrix.GetAt(0, 0));
            Assert.True(processor.Matrix.GetAt(0, 3));
            Assert.True(processor.Matrix.GetAt(3, 0));
            Assert.True(processor.Matrix.GetAt(3, 3));

            Assert.Equal(0, stats.Died);
            Assert.Equal(0, stats.Resurested);
            Assert.Equal(0, stats.Survived);
        }
    }
}
