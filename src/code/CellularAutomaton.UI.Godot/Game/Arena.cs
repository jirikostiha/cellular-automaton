using Godot;
using GodotFile = Godot.FileAccess;

namespace CellularAutomaton.UI.Godot;

/// <summary>
/// Game screen: owns the automaton, drives its generations and renders them.
/// </summary>
public partial class Arena : ColorRect
{
    private double _time; // processing time
    private int _iteration; // generations
    private int _toProcess;

    private GenerationProcessor? _processor;
    private ControlPanel _controlPanel = null!;
    private InfoPanel _infoPanel = null!;
    private Sprite2D _matrixSprite = null!;

    public Arena()
    {
        VizuOptions = new BitArray2DVizuOptions();
        Vizer = new BitArray2DToImageVizualizer(VizuOptions);
    }

    [Signal]
    public delegate void TimeChangedEventHandler(double time);

    [Signal]
    public delegate void IterationChangedEventHandler(int iteration, int died, int survived, int resurected);

    public BitArray2DToImageVizualizer Vizer { get; }

    public BitArray2DVizuOptions VizuOptions { get; }

    public ControlPanel ControlPanel => _controlPanel;

    public InfoPanel InfoPanel => _infoPanel;

    protected Sprite2D MatrixSprite => _matrixSprite;

    public override void _Ready()
    {
        _infoPanel = GetNode<InfoPanel>("%InfoPanel");
        _controlPanel = GetNode<ControlPanel>("%ControlPanel");
        _matrixSprite = GetNode<Sprite2D>("%MatrixSprite");

        _controlPanel.GetNode("CreateNew").Connect("pressed", Callable.From(CreateNewHandler));
        _controlPanel.GetNode("Run").Connect("toggled", Callable.From<bool>(RunHandler));
        _controlPanel.GetNode("NextGen").Connect("pressed", Callable.From(NextGenerationHandler));
        _controlPanel.GetNode("Save").Connect("pressed", Callable.From(SaveHandler));
        _controlPanel.GetNode("Load").Connect("pressed", Callable.From(LoadHandler));
    }

    public override void _Process(double delta)
    {
        if (_toProcess <= 0 || _processor is null)
            return;

        ProcessNextGeneration(delta);
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_processor is not null && _matrixSprite.Texture is ImageTexture texture)
            Vizer.Vizualize(_processor.Matrix, texture);
    }

    public void CreateNewHandler()
    {
        if (_toProcess != 0)
            return;

        var matrixSize = new MatrixSize(
            (int)_controlPanel.GetNode<SpinBox>("%XSizeSelection").Value,
            (int)_controlPanel.GetNode<SpinBox>("%YSizeSelection").Value);

        var matrix = BitArray2D.Create(matrixSize.X, matrixSize.Y, (_, _) => AppGlobal.NextBool());

        SetMatrix(matrix);
    }

    public void RunHandler(bool pressed)
    {
        var button = _controlPanel.GetNode<Button>("Run");
        if (pressed && _processor is not null)
        {
            _toProcess = int.MaxValue;
            button.Text = "Stop";
        }
        else
        {
            _toProcess = 0;
            button.Text = "Run";
            button.ButtonPressed = false;
        }
    }

    public void NextGenerationHandler()
    {
        if (_processor is not null && _toProcess < int.MaxValue)
            _toProcess++;
    }

    public void SaveHandler()
    {
        if (_processor is null)
            return;

        ShowFileDialog(FileDialog.FileModeEnum.SaveFile, "Save matrix", path =>
        {
            using var file = GodotFile.Open(path, GodotFile.ModeFlags.Write);
            if (file is null)
            {
                GD.PushError($"Cannot write '{path}': {GodotFile.GetOpenError()}");

                return;
            }

            file.StoreString(new BitArray2DSerializer().Serialize(_processor.Matrix));
        });
    }

    public void LoadHandler()
    {
        ShowFileDialog(FileDialog.FileModeEnum.OpenFile, "Load matrix", path =>
        {
            using var file = GodotFile.Open(path, GodotFile.ModeFlags.Read);
            if (file is null)
            {
                GD.PushError($"Cannot read '{path}': {GodotFile.GetOpenError()}");

                return;
            }

            var content = file.GetAsText();
            if (string.IsNullOrEmpty(content))
                return;

            SetMatrix(new BitArray2DSerializer().Deserialize(content));
        });
    }

    private void SetMatrix(BitArray2D matrix)
    {
        _toProcess = 0;
        _processor = new GenerationProcessor(
            matrix,
            new GenerationProcessorOptions { CleanBorders = true });

        _matrixSprite.Texture = BitArray2DToImageVizualizer.CreateTexture(matrix.XCount, matrix.YCount);

        _time = 0;
        _iteration = 0;
        EmitSignal(SignalName.TimeChanged, _time);
        EmitSignal(SignalName.IterationChanged, 0, 0, 0, 0);

        QueueRedraw();
    }

    private void ShowFileDialog(FileDialog.FileModeEnum mode, string title, Action<string> onSelected)
    {
        var dialog = new FileDialog
        {
            FileMode = mode,
            Access = FileDialog.AccessEnum.Filesystem,
            Title = title,
            Filters = ["*.txt ; Text files", "* ; All files"],
        };

        dialog.FileSelected += path =>
        {
            onSelected(path);
            dialog.QueueFree();
        };
        dialog.Canceled += dialog.QueueFree;

        AddChild(dialog);
        dialog.PopupCentered(new Vector2I(700, 500));
    }

    private void ProcessNextGeneration(double delta)
    {
        if (_processor is null)
            return;

        _time += delta;
        _iteration++;

        var stats = _processor.Next();

        EmitSignal(SignalName.TimeChanged, _time);
        EmitSignal(SignalName.IterationChanged, _iteration, stats.Died, stats.Survived, stats.Revived);

        if (_toProcess < int.MaxValue)
            _toProcess--;
    }
}
