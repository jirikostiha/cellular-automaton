using Godot;
using System;
using System.Globalization;

namespace CellularAutomaton.UI.Godot
{
    public class InfoPanel : HBoxContainer
    {
        private Label? _timeLabel;
        private Label? _iterationLabel;
        private Label? _diedLabel;
        private Label? _survivedLabel;
        private Label? _resurectedLabel;

        public float Time { set => _timeLabel.Text = value.ToString("0.0", CultureInfo.InvariantCulture); }
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

            Time = 0;
            Iteration = 0;
            Died = 0;
            Survived = 0;
            Resurected = 0;
        }
    }
}