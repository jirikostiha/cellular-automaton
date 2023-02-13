namespace CellularAutomaton.UI.WinForms
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using CellularAutomaton;

    public partial class LifeViewer : Form
    {
        private GenerationProcessor _processor;
        private readonly MatrixToBitmapVizualizer _vizualizer = new MatrixToBitmapVizualizer();
        private BoolArray2DSerializer _serializer;
        private int _speedDelayMs = 0;
        private Func<int, int, IArray2D<bool>> _matrixCreator;

        public LifeViewer(Func<int, int, IArray2D<bool>> matrixCreator)
        {
            _matrixCreator = matrixCreator;
            _serializer = new BoolArray2DSerializer();
            InitializeComponent();
        }

        public GenerationProcessorOptions ProcessorOptions { get; set; }

        public IArray2D<bool> Matrix { get; private set; }

        public int GenerationNumber { get; private set; }

        private async Task NextIteration()
        {
            var bitmap = await Task.Run(() =>
            {
                var stats = _processor.Next();
                return _vizualizer.Vizualize(Matrix);
            });

            gridPictureBox.Image = bitmap;
            GenerationNumber++;
            GenerationTextBox.Text = GenerationNumber.ToString();
        }

        private async Task Run(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await NextIteration();
                if (_speedDelayMs > 10)
                    await Task.Delay(_speedDelayMs, ct);
            }
        }

        private void CreateRandomButton_Click(object sender, EventArgs e)
        {
            int xcount = (int)X1Num.Value;
            int ycount = (int)X2Num.Value;
            var matrix = _matrixCreator(xcount, ycount);
            _processor = new GenerationProcessor(matrix, ProcessorOptions);
            Matrix = matrix;
            GenerationNumber = 0;
            GenerationTextBox.Text = GenerationNumber.ToString();

            var bitmap = _vizualizer.Vizualize(Matrix);
            gridPictureBox.Image = bitmap;
        }

        private async void NextButton_Click(object sender, EventArgs e)
        {
            await NextIteration();
        }

        private CancellationTokenSource _cts;

        private async void RunButton_Click(object sender, EventArgs e)
        {
            if (_cts == null)
            {
                _cts = new CancellationTokenSource();
                RunButton.Text = "Stop";
                try
                {
                    await Run(_cts.Token);
                }
                catch (TaskCanceledException)
                {}
            }
            else
            {
                _cts.Cancel();
                _cts = null;
                RunButton.Text = "Run";
            }
        }

        private void SpeedTrackBar_ValueChanged(object sender, EventArgs e)
        {
            _speedDelayMs = SpeedTrackBar.Value;
            DelayTextBox.Text = _speedDelayMs.ToString();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream stream;
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    var strMatrix = _serializer.Serialize(Matrix);

                    using StreamWriter writer = new StreamWriter(stream);
                    writer.Write(strMatrix);
                }
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var fileStream = openFileDialog.OpenFile();
                using var reader = new StreamReader(fileStream);
                string fileContent = reader.ReadToEnd();
                var matrix = _serializer.Deserialize(fileContent);
                _processor = new GenerationProcessor(matrix, ProcessorOptions);
                Matrix = matrix;
                
                var bitmap = _vizualizer.Vizualize(Matrix);
                gridPictureBox.Image = bitmap;
            }
        }
    }
}
