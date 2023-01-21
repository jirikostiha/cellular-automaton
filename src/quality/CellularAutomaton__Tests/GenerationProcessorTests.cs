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
            Assert.Equal(0, stats.Revived);
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
            Assert.Equal(0, stats.Revived);
            Assert.Equal(0, stats.Survived);
        }

        [Fact]
        public void Next_BlockScenario()
        {
            var matrix = BitArray2D.Create(4, 4, true, false);
            var processor = new GenerationProcessor(matrix);

            var stats = processor.Next();

            Assert.True(processor.Matrix.GetAt(1, 1));
            Assert.True(processor.Matrix.GetAt(2, 1));
            Assert.True(processor.Matrix.GetAt(1, 2));
            Assert.True(processor.Matrix.GetAt(2, 2));

            Assert.Equal(0, stats.Died);
            Assert.Equal(0, stats.Revived);
            Assert.Equal(4, stats.Survived);
        }

        /// <summary>
        /// 0 0 0 0 0    0 0 0 0 0
        /// 0 0 1 0 0    0 0 0 0 0
        /// 0 0 1 0 0 -> 0 1 1 1 0
        /// 0 0 1 0 0    0 0 0 0 0
        /// 0 0 0 0 0    0 0 0 0 0
        /// </summary>
        [Fact]
        public void Next_OscilatorScenario()
        {
            var matrix = BitArray2D.Create(5, 5);
            matrix.SetAt(2, 1, true);
            matrix.SetAt(2, 2, true);
            matrix.SetAt(2, 3, true);
            var processor = new GenerationProcessor(matrix);

            var stats1 = processor.Next();

            Assert.True(processor.Matrix.GetAt(1, 2));
            Assert.True(processor.Matrix.GetAt(2, 2));
            Assert.True(processor.Matrix.GetAt(3, 2));
            Assert.False(processor.Matrix.GetAt(2, 1));
            Assert.False(processor.Matrix.GetAt(2, 3));

            Assert.Equal(2, stats1.Died);
            Assert.Equal(2, stats1.Revived);
            Assert.Equal(1, stats1.Survived);

            var stats2 = processor.Next();

            Assert.Equal(2, stats2.Died);
            Assert.Equal(2, stats2.Revived);
            Assert.Equal(1, stats2.Survived);
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
            Assert.Equal(0, stats.Revived);
            Assert.Equal(0, stats.Survived);
        }

        /// <summary>
        /// 0 0 0 0    0 0 0 0
        /// 0 1 1 0    0 1 1 0
        /// 0 1 0 0 -> 0 0 0 0 -> 0
        /// 0 1 1 0    0 1 1 0
        /// 0 0 0 0    0 0 0 0
        /// </summary>
        [Fact]
        public void Next_CShapeScwnario()
        {
            var matrix = BitArray2D.Create(4, 5, true, false);
            matrix.SetAt(2, 2, false);
            var processor = new GenerationProcessor(matrix);

            var stats1 = processor.Next();

            Assert.Equal(4, stats1.Survived);
            Assert.Equal(1, stats1.Died);
            Assert.Equal(0, stats1.Revived);

            var stats2 = processor.Next();

            Assert.Equal(0, stats2.Survived);
            Assert.Equal(4, stats2.Died);
            Assert.Equal(0, stats2.Revived);
        }
    }
}
