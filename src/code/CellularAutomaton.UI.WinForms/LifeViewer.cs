namespace CellularAutomaton.UI.WinForms;

using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CellularAutomaton;

public partial class LifeViewer : Form
{
    private readonly MatrixToBitmapVizualizer _vizualizer = new();
    private readonly BitArray2DSerializer _serializer = new();
    private readonly Func<int, int, IArray2D<bool>> _matrixCreator;

    // Two bitmaps are alternated so that the one being painted by the picture box is never the one
    // written to by the next generation.
    private Bitmap? _frontBuffer;
    private Bitmap? _backBuffer;

    private GenerationProcessor? _processor;
    private CancellationTokenSource? _cts;
    private Task _runTask = Task.CompletedTask;
    private bool _busy;
    private int _speedDelayMs;

    public LifeViewer(Func<int, int, IArray2D<bool>> matrixCreator)
    {
        _matrixCreator = matrixCreator ?? throw new ArgumentNullException(nameof(matrixCreator));

        InitializeComponent();

        FormClosing += (_, _) => _cts?.Cancel();
        FormClosed += (_, _) =>
        {
            // A generation may still be rendering into a buffer, in that case the bitmaps are left
            // to the garbage collector instead of being disposed underneath it.
            if (_runTask.IsCompleted)
                ReleaseBuffers();
            else
                gridPictureBox.Image = null;
        };
    }

    public GenerationProcessorOptions? ProcessorOptions { get; set; }

    public IArray2D<bool>? Matrix { get; private set; }

    public int GenerationNumber { get; private set; }

    private async Task NextIterationAsync()
    {
        var processor = _processor;
        var matrix = Matrix;
        if (processor is null || matrix is null || _busy)
            return;

        _busy = true;
        try
        {
            var target = _backBuffer;
            var bitmap = await Task.Run(() =>
            {
                processor.Next();

                return _vizualizer.Vizualize(matrix, target);
            }).ConfigureAwait(true);

            ShowFrame(bitmap);
            GenerationNumber++;
            GenerationTextBox.Text = GenerationNumber.ToString();
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task RunAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await NextIterationAsync().ConfigureAwait(true);
            if (_speedDelayMs > 10)
                await Task.Delay(_speedDelayMs, ct).ConfigureAwait(true);
        }
    }

    private void ShowFrame(Bitmap bitmap)
    {
        // The rendered bitmap becomes the front buffer, the previous one is reused next time.
        _backBuffer = ReferenceEquals(bitmap, _frontBuffer) ? _backBuffer : _frontBuffer;
        _frontBuffer = bitmap;
        gridPictureBox.Image = bitmap;
        gridPictureBox.Invalidate();
    }

    private async Task SetMatrixAsync(IArray2D<bool> matrix)
    {
        await StopAsync().ConfigureAwait(true);
        ReleaseBuffers();

        Matrix = matrix;
        _processor = new GenerationProcessor(matrix, ProcessorOptions);
        GenerationNumber = 0;
        GenerationTextBox.Text = GenerationNumber.ToString();

        ShowFrame(_vizualizer.Vizualize(matrix));
    }

    private void ReleaseBuffers()
    {
        gridPictureBox.Image = null;
        _frontBuffer?.Dispose();
        _backBuffer?.Dispose();
        _frontBuffer = null;
        _backBuffer = null;
    }

    /// <summary>
    /// Stops the running loop and waits until the generation in flight is finished, so that the
    /// buffers it writes to are not released underneath it.
    /// </summary>
    private async Task StopAsync()
    {
        var cts = _cts;
        if (cts is null)
            return;

        _cts = null;
        cts.Cancel();
        try
        {
            await _runTask.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Stopped by the user.
        }
        finally
        {
            cts.Dispose();
            _runTask = Task.CompletedTask;
            RunButton.Text = "Run";
        }
    }

    private async void CreateRandomButton_Click(object? sender, EventArgs e)
        => await SetMatrixAsync(_matrixCreator((int)X1Num.Value, (int)X2Num.Value)).ConfigureAwait(true);

    private async void NextButton_Click(object? sender, EventArgs e)
        => await NextIterationAsync().ConfigureAwait(true);

    private async void RunButton_Click(object? sender, EventArgs e)
    {
        if (_cts is not null)
        {
            await StopAsync().ConfigureAwait(true);

            return;
        }

        if (_processor is null)
            return;

        var cts = new CancellationTokenSource();
        _cts = cts;
        RunButton.Text = "Stop";
        _runTask = RunAsync(cts.Token);
        try
        {
            await _runTask.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Stopped by the user.
        }

        if (ReferenceEquals(_cts, cts))
        {
            _cts = null;
            cts.Dispose();
            _runTask = Task.CompletedTask;
            RunButton.Text = "Run";
        }
    }

    private void SpeedTrackBar_ValueChanged(object? sender, EventArgs e)
    {
        _speedDelayMs = SpeedTrackBar.Value;
        DelayTextBox.Text = _speedDelayMs.ToString();
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        if (Matrix is null || saveFileDialog.ShowDialog() != DialogResult.OK)
            return;

        using var stream = saveFileDialog.OpenFile();
        if (stream is null)
            return;

        using var writer = new StreamWriter(stream);
        writer.Write(_serializer.Serialize(Matrix));
    }

    private async void LoadButton_Click(object? sender, EventArgs e)
    {
        if (openFileDialog.ShowDialog() != DialogResult.OK)
            return;

        using var stream = openFileDialog.OpenFile();
        if (stream is null)
            return;

        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        if (string.IsNullOrEmpty(content))
            return;

        await SetMatrixAsync(_serializer.Deserialize(content)).ConfigureAwait(true);
    }
}
