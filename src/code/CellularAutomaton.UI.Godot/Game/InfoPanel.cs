using Godot;
using System;
using System.Globalization;

namespace CellularAutomaton.UI.Godot
{
    public partial class InfoPanel : HBoxContainer
    {
        private Label? _timeLabel;
        private Label? _iterationLabel;
        private Label? _diedLabel;
        private Label? _survivedLabel;
        private Label? _resurectedLabel;

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

            var arena = FindParent("GameScreen").GetNode("%Arena");
            arena.Connect(nameof(Arena.TimeChanged),new Callable(this,nameof(TimeChangedHandler)));
            arena.Connect(nameof(Arena.IterationChanged),new Callable(this,nameof(IterationChangedHandler)));

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
}