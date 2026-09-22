namespace CellularAutomaton;

/// <summary>
/// Straightforward, deliberately naive implementation of the rules, used as the oracle the
/// optimized <see cref="GenerationProcessor"/> is compared against.
/// </summary>
internal sealed class ReferenceProcessor
{
    private readonly bool _cleanBorders;
    private readonly bool[,]? _immortals;
    private readonly bool[,]? _unviables;

    private bool[,] _current;
    private bool[,] _previous;

    public ReferenceProcessor(bool[,] initial, bool cleanBorders, bool[,]? immortals = null, bool[,]? unviables = null)
    {
        _current = initial;
        _previous = (bool[,])initial.Clone();
        _cleanBorders = cleanBorders;
        _immortals = immortals;
        _unviables = unviables;
    }

    public bool GetAt(int x, int y) => _current[x, y];

    public (int Died, int Revived, int Survived) Next()
    {
        (_current, _previous) = (_previous, _current);

        int xmax = _previous.GetLength(0);
        int ymax = _previous.GetLength(1);

        if (_cleanBorders)
        {
            for (int x = 0; x < xmax; x++)
                for (int y = 0; y < ymax; y++)
                    _current[x, y] = false;
        }

        int died = 0;
        int revived = 0;
        int survived = 0;

        for (int x = 1; x < xmax - 1; x++)
        {
            for (int y = 1; y < ymax - 1; y++)
            {
                int neighbours = 0;
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if ((dx != 0 || dy != 0) && _previous[x + dx, y + dy])
                            neighbours++;

                if (_previous[x, y])
                {
                    if (neighbours is < 2 or > 3)
                    {
                        if (_immortals is null || !_immortals[x, y])
                        {
                            _current[x, y] = false;
                            died++;
                        }
                        else
                        {
                            _current[x, y] = true;
                            survived++;
                        }
                    }
                    else
                    {
                        _current[x, y] = true;
                        survived++;
                    }
                }
                else
                {
                    if (neighbours == 3)
                    {
                        if (_unviables is null || !_unviables[x, y])
                        {
                            _current[x, y] = true;
                            revived++;
                        }
                        else
                        {
                            _current[x, y] = false;
                        }
                    }
                    else
                    {
                        _current[x, y] = false;
                    }
                }
            }
        }

        return (died, revived, survived);
    }
}
