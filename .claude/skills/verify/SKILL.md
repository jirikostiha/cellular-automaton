---
name: verify
description: Build this repository and drive the Game of Life library and its UIs to see a change actually running. Use when verifying a change, reproducing a bug, or capturing evidence that the automaton behaves correctly.
---

# Verifying cellular-automaton

## Toolchain

The Ubuntu SDK package is enough for the library, the tests, the benchmarks and the
Godot project:

```bash
apt-get update -q && apt-get install -y -q dotnet-sdk-8.0
```

`dotnet --info` reporting nothing means it is not installed yet; the official
`dotnet-install.sh` is usually blocked by the egress proxy, so use apt.

## Build

```bash
cd src
dotnet build "cellular automaton.sln" -c Release        # library + tests + benchmarks
dotnet build "cellular automaton godot.sln" -c Release  # Godot UI, builds on Linux
```

`Directory.Build.props` redirects output, so assemblies land in `<repo>/asm/<project>/bin/...`,
**not** in `bin/` next to the project. Stale `src/**/obj` directories from a build made before
that redirection existed cause `error CS0579: Duplicate ... attribute`; delete them:

```bash
find src -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +
```

## Surfaces worth driving

### The benchmark console app — a real CLI in the repo

```bash
dotnet run --project src/quality/CellularAutomaton__Benchmarks -c Release -- --list flat
dotnet run --project src/quality/CellularAutomaton__Benchmarks -c Release -- \
    --job short --filter '*ProcessorNextGeneration*'
```

Always redirect stdin from `/dev/null`; it writes `BenchmarkDotNet.Artifacts/` into the
current directory (gitignored). `--job dry` finishes in seconds when you only need it to run.

### The library — go through the built assembly, not the sources

Reference `asm/CellularAutomaton/bin/Release/net8.0/CellularAutomaton.dll` from a scratch
console project (plus a `CommunityToolkit.Diagnostics` package reference) so you exercise the
package boundary. Flows that are worth driving, because each one has caught something:

- **Glider** in a 10x10 box with `CleanBorders`: after 4 generations it must be translated by
  exactly (1,1) with 5 live cells and `died=2 revived=2 survived=3` every step.
- **Blinker**: period 2. Also the clearest way to see whether a caller is looking at the
  current generation or a stale buffer.
- **Save / load**: serialize, write to a file, read it back, compare cell by cell. Do it with
  both `\n` and `\r\n`, and with the default `FalseValue = ' '`.
- **Border widths 63/64/65/66/129**: the bit packed path masks the border columns per word, so
  widths either side of a 64 bit boundary are where masking bugs show up.
- **Parallel vs sequential**: same seed with `MaxDegreeOfParallelism` 1 / 0 / a negative / a
  huge value must produce identical cells and identical statistics.

Render matrices with `new BitArray2DSerializer { TrueValue = '#', FalseValue = '.' }` — the
default dead-cell space is unreadable in a terminal capture.

### Gotchas that cost time

- A 50% random soup in a small box with `CleanBorders` dies out within about five generations,
  which makes a useless sample. Use roughly 30% density in a box of at least 24x14.
- `GenerationProcessor` swaps two buffers, so the matrix object you passed to the constructor is
  the *current* generation only after an even number of `Next()` calls. Read `processor.Matrix`.
- `MaxDegreeOfParallelism` alone does nothing below `ParallelThreshold` (262144 cells by
  default). Set `ParallelThreshold = 0` to force the parallel path in a small repro.
- Allocation per generation is a useful signal: wrap a loop in
  `GC.GetTotalAllocatedBytes(precise: true)`.

## What cannot be run here

- **WinForms** needs `net8.0-windows`; the Ubuntu SDK package ships no
  `Microsoft.NET.Sdk.WindowsDesktop`, so `dotnet build` fails on the import. To at least
  type-check the sources, compile them from a scratch `net8.0` project with
  `<EnableWindowsTargeting>true</EnableWindowsTargeting>` and
  `<FrameworkReference Include="Microsoft.WindowsDesktop.App.WindowsForms" />`, listing the
  `.cs` files explicitly. The `Build Winforms UI` workflow builds it for real on
  `windows-latest`.
- **Godot** compiles, but the engine is not installed and the download is blocked, so no scene
  can be launched. Godot code is verifiable only to the compile boundary.

## CI

`dotnet_format` is red on `main` as well: `lint-code.yml` points `dotnet_format_dir` at `src`,
which holds three `.sln` files, so `dotnet format` refuses to choose a workspace and exits 1.
The enclosing job sets `continue_on_error`, so only the check run goes red. Reproduce with
`dotnet format src --verify-no-changes`.
