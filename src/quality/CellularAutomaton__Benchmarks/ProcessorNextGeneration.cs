using BenchmarkDotNet.Attributes;

namespace CellularAutomaton
{
    [MemoryDiagnoser]
    public class ProcessorNextGeneration
    {
        [Params(1, 10, 100)]
        public int N;

        [Params(100, 1_000, 10_000)]
        public int Size;

        private BitArray2D _bitArray2D;
        private BoolArray2D _boolArray2D;

        private GenerationProcessorOptions _options = new() { CleanBorders = true };

        [IterationSetup]
        public void IterationSetup()
        {
            _bitArray2D = BitArray2D.Create(Size, Size);
            _boolArray2D = BoolArray2D.Create(Size, Size);
        }

        [Benchmark]
        public void Next_BitArray2D()
        {
            var processor = new GenerationProcessor(_bitArray2D, _options);
            for (int i = 0; i < N; i++)
            {
                processor.Next();
            }
        }

        [Benchmark]
        public void Next_BoolArray2D()
        {
            var processor = new GenerationProcessor(_boolArray2D, _options);
            for (int i = 0; i < N; i++)
            {
                processor.Next();
            }
        }
    }
}
