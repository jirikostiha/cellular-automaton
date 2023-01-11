namespace CellularAutomaton
{
    using System;
    using System.Collections;

    public class GenerationProcessor
    {
        private IArray2D<bool> _current;
        private IArray2D<bool> _previous;

        private GenerationProcessorOptions _options;

        public GenerationProcessor(IArray2D<bool> matrix, GenerationProcessorOptions options = null)
        {
            _current = matrix;
            _options = options;
            _previous = _current.Clone() as IArray2D<bool>;
        }

        public GenerationProcessorOptions Options => _options;

        public IArray2D<bool> Matrix => _current;

        /// <summary>
        /// Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
        /// Any live cell with two or three live neighbours lives on to the next generation.
        /// Any live cell with more than three live neighbours dies, as if by overpopulation.
        /// Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
        /// </summary>
        public (int Died, int Resurested, int Survived) Next()
        {
            // swap matrices
            var previousBackup = _previous; // backup and reuse memory
            _previous = _current;
            var xmax = _current.XCount;
            var ymax = _current.YCount;
            if (_options.CleanBorders)
                //reset all cells
                previousBackup.SetRegion(0, 0, xmax - 1, ymax - 1, false);
            else
                //reset inner cells only
                previousBackup.SetRegion(1, 1, xmax - 2, ymax - 2, false);
            _current = previousBackup;

            var died = 0;
            var resurected = 0;
            var survived = 0;
            for (int x = 1; x < xmax - 1; x++)
            {
                for (int y = 1; y < ymax - 1; y++)
                {
                    var livingNeighborsCount = GetLivingNeighboursCount(_previous, x, y);
                    if (_previous.GetAt(x, y)) // live cell from previous matrix
                    {
                        if (livingNeighborsCount < 2 || livingNeighborsCount > 3)
                        {
                            _current.SetAt(x, y, false); // died
                            died++;
                        }
                        else
                        {
                            _current.SetAt(x, y, true); // survived
                            survived++;
                        }
                    }
                    else // dead cell from previous generation
                    {
                        if (livingNeighborsCount == 3)
                        {
                            _current.SetAt(x, y, true); // resurection
                            resurected++;
                        }
                        else
                        {
                            _current.SetAt(x, y, false); // still dead
                        }
                    }
                }
            }

            return (died, resurected, survived);
        }

        private int GetLivingNeighboursCount(IArray2D<bool> matrix, int x, int y)
        {
            var count = 0;
            if (matrix.GetAt(x, y - 1))
                count++;
            if (matrix.GetAt(x, y + 1))
                count++;
            if (matrix.GetAt(x - 1, y))
                count++;
            if (matrix.GetAt(x + 1, y))
                count++;
            if (matrix.GetAt(x + 1, y + 1))
                count++;
            if (matrix.GetAt(x + 1, y - 1))
                count++;
            if (matrix.GetAt(x - 1, y + 1))
                count++;
            if (matrix.GetAt(x - 1, y - 1))
                count++;

            return count;
        }
    }
}
