using System.Globalization;
using Godot;

namespace CellularAutomaton.UI.Godot;

/// <summary>
/// Shows the statistics emitted by the <see cref="Arena"/>.
/// </summary>
public partial class InfoPanel : HBoxContainer
{
    private Label _timeLabel = null!;
    private Label _iterationLabel = null!;
    private Label _diedLabel = null!;
    private Label _survivedLabel = null!;
    private Label _resurectedLabel = null!;

    public double Time { set => _timeLabel.Text = value.ToString("0.0", CultureInfo.InvariantCulture); }

    public int Iteration { set => _iterationLabel.Text = value.ToString(CultureInfo.InvariantCulture); }

    public int Died { set => _diedLabel.Text = value.ToString(CultureInfo.InvariantCulture); }

    public int Survived { set => _survivedLabel.Text = value.ToString(CultureInfo.InvariantCulture); }

    public int Resurected { set => _resurectedLabel.Text = value.ToString(CultureInfo.InvariantCulture); }

    public override void _Ready()
    {
        _timeLabel = GetNode<Label>("Time/Value");
        _iterationLabel = GetNode<Label>("Iteration/Value");
        _diedLabel = GetNode<Label>("Died/Value");
        _survivedLabel = GetNode<Label>("Survived/Value");
        _resurectedLabel = GetNode<Label>("Resurected/Value");

        var arena = FindParent("GameScreen").GetNode<Arena>("%Arena");
        arena.TimeChanged += TimeChangedHandler;
        arena.IterationChanged += IterationChangedHandler;

        Time = 0;
        Iteration = 0;
        Died = 0;
        Survived = 0;
        Resurected = 0;
    }

    public void TimeChangedHandler(double time) => Time = time;

    public void IterationChangedHandler(int iteration, int died, int survived, int resurected)
    {
        Iteration = iteration;
        Died = died;
        Survived = survived;
        Resurected = resurected;
    }
}
