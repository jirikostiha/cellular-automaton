# Cellular Automaton  
![Build workflow](https://github.com/jirikostiha/cellular-automaton/actions/workflows/build.yml/badge.svg)
![GitHub repo size](https://img.shields.io/github/repo-size/jirikostiha/cellular-automaton)  

Is implementation of Conway's Game of Life.  

## Additional features  
### Immortality  
An immortal cell, once revived, can no longer die.  

### Unviability  
An unviable cell, once dead, cannot be revieved.  

## Structure  
| Project | Target | Description |
| --- | --- | --- |
| `src/code/CellularAutomaton` | `netstandard2.1`, `net8.0` | The automaton itself, no UI dependencies. |
| `src/code/CellularAutomaton.UI.WinForms` | `net8.0-windows` | Windows Forms front end. |
| `src/code/CellularAutomaton.UI.Godot` | `net6.0` | Godot 4 front end. |
| `src/quality/CellularAutomaton__Tests` | `net8.0` | Unit tests. |
| `src/quality/CellularAutomaton__Benchmarks` | `net8.0` | BenchmarkDotNet benchmarks. |

## Performance  
`BitArray2D` packs the cells into a flat buffer of 64 bit words and `GenerationProcessor`
evaluates a whole word at a time: the number of live cells in every 3x3 block is computed as a
bit sliced sum of three rows, so one generation costs a few dozen instructions per 64 cells
instead of a per cell neighbour walk. Matrices that are not bit packed fall back to a cell by
cell evaluation that buffers three rows and keeps a sliding block sum.

Set `GenerationProcessorOptions.MaxDegreeOfParallelism` to spread the rows of large matrices over
several threads; the resulting generation and statistics do not depend on it.

```csharp
var matrix = BitArray2D.Create(1024, 1024, (x, y) => Random.Shared.Next(2) == 0);
var processor = new GenerationProcessor(matrix, new GenerationProcessorOptions
{
    CleanBorders = true,
    MaxDegreeOfParallelism = 0, // 0 = Environment.ProcessorCount, 1 = current thread only
});

var (died, revived, survived) = processor.Next();
```

## Build and test  
```shell
dotnet build "src/cellular automaton.sln"
dotnet test "src/cellular automaton.sln"
dotnet run --project src/quality/CellularAutomaton__Benchmarks -c Release
```

## Sources  
[Wikipedia](https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)  
[Plus magazine](https://plus.maths.org/content/cellular-automata)  
[ConwayLife.com - Life Lexicon](https://conwaylife.com/ref/lexicon/lex_home.htm)  
[ConwayLife.com - patterns forum](https://conwaylife.com/forums/viewtopic.php?f=2&t=2160)  
[Game of Life](https://playgameoflife.com/)  
[b3s23life blog](http://b3s23life.blogspot.com/)  
