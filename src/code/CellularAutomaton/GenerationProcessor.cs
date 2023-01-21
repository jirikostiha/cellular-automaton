using CommunityToolkit.Diagnostics;

namespace CellularAutomaton
{
    public class GenerationProcessor
    {
        private IArray2D<bool> _current;
        private IArray2D<bool> _previous;
        private IArray2D<bool>? _immortals;
        private IArray2D<bool>? _unviables; //Unviability

        private GenerationProcessorOptions _options;

        public GenerationProcessor(IArray2D<bool> initialMatrix, GenerationProcessorOptions? options = null)
        {
            Guard.IsNotNull(initialMatrix);

            _current = initialMatrix;
            _options = options ?? new();
            _previous = _current.Clone() as IArray2D<bool>;
        }

        public GenerationProcessor(IArray2D<bool> initialMatrix, IArray2D<bool>? immortals, IArray2D<bool>? unviables, GenerationProcessorOptions? options = null)
            : this(initialMatrix, options)
        {
            if (immortals is not null)
            {
                Guard.IsEqualTo(immortals.XCount, initialMatrix.XCount);
                Guard.IsEqualTo(immortals.YCount, initialMatrix.YCount);

                _immortals = immortals;
            }
            if (unviables is not null)
            {
                Guard.IsEqualTo(unviables.XCount, initialMatrix.XCount);
                Guard.IsEqualTo(unviables.YCount, initialMatrix.YCount);

                _unviables = unviables;
            }
        }

        public GenerationProcessorOptions Options => _options;

        public ReadonlyArray2D<bool> Matrix => new ReadonlyArray2D<bool>(_current);

        /// <summary>
        /// Immortal cell flags.
        /// </summary>
        public IArray2D<bool>? Immortals => _immortals;

        /// <summary>
        /// Unviable cell flags.
        /// </summary>
        public IArray2D<bool>? Unviables => _unviables;

        /// <summary>
        /// Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
        /// Any live cell with two or three live neighbours lives on to the next generation.
        /// Any live cell with more than three live neighbours dies, as if by overpopulation.
        /// Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
        /// </summary>
        public (int Died, int Revived, int Survived) Next()
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
            var revived = 0;
            var survived = 0;
            for (int x = 1; x < xmax - 1; x++)
            {
                for (int y = 1; y < ymax - 1; y++)
                {
                    var livingNeighborsCount = GetLivingNeighboursCount(_previous, x, y);
                    if (_previous.GetAt(x, y)) // living cell from previous generation matrix
                    {
                        if (livingNeighborsCount < 2 || livingNeighborsCount > 3)
                        {
                            if (_immortals is null || !_immortals.GetAt(x,y))
                            {
                                _current.SetAt(x, y, false); // died
                                died++;
                            }
                            //else cannot die - is immortal
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
                            if (_unviables is null || !_unviables.GetAt(x, y))
                            {
                                _current.SetAt(x, y, true); // revival
                                revived++;
                            }
                            //else cannot be revived - is unviable
                        }
                        else
                        {
                            _current.SetAt(x, y, false); // remains dead
                        }
                    }
                }
            }

            return (died, revived, survived);
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
