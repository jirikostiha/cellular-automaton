using Godot;
using System;

namespace CellularAutomaton.UI.Godot
{
    public class Arena : ColorRect
    {
        private AppGlobal _global;

        private float _time; // processing time
        private int _iteration; // generations
        private int _toProcess;

        private GenerationProcessorOptions _processorOptions;
        private GenerationProcessor _processor;

        public Arena()
        {
            VizuOptions = new();
            Vizer = new BitArray2DToImageVizualizer(VizuOptions);
        }

        public BitArray2DToImageVizualizer Vizer { get; set; }

        public BitArray2DVizuOptions VizuOptions { get; private set; }

        public ControlPanel ControlPanel { get; private set; }

        public InfoPanel InfoPanel { get; private set; }

        protected Sprite MatrixSprite { get; private set; }

        public override void _Ready()
        {
            _global = GetTree().Root.GetNode<AppGlobal>("AppGlobal");
            InfoPanel = GetNode<InfoPanel>("%InfoPanel");
            ControlPanel = GetNode<ControlPanel>("%ControlPanel");
            MatrixSprite = GetNode<Sprite>("%MatrixSprite");
            
            ControlPanel.GetNode("CreateNew").Connect("pressed", this, nameof(CreateNewHandler));
            ControlPanel.GetNode("Run").Connect("toggled", this, nameof(RunHandler));
            ControlPanel.GetNode("NextGen").Connect("pressed", this, nameof(NextGenerationHandler));
            ControlPanel.GetNode("Save").Connect("pressed", this, nameof(SaveHandler));
            ControlPanel.GetNode("Load").Connect("pressed", this, nameof(LoadHandler));
        }

        public override void _Process(float delta)
        {
            if (_toProcess <= 0)
                return;

            if (_processor is not null)
            {
                ProcessNextGeneration(delta);
                Update();
            }
        }

        public override void _Draw()
        {
            if (_processor is not null && MatrixSprite.Texture is not null)
                Vizer.Vizualize(_processor.Matrix, MatrixSprite.Texture as ImageTexture);
        }

        public void CreateNewHandler()
        {
            if (_toProcess != 0)
                return;

            var matrixSize = new MatrixSize()
            {
                X = (int)ControlPanel.GetNode<SpinBox>("%XSizeSelection").Value,
                Y = (int)ControlPanel.GetNode<SpinBox>("%YSizeSelection").Value,
            };

            var processorOptions = new GenerationProcessorOptions()
            {
                CleanBorders = true,
            };

            var rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            var matrix = BitArray2D.Create(
                matrixSize.X,
                matrixSize.Y,
                (x, y) => rnd.NextDouble() >= 0.5d ? true : false);
            var processor = new GenerationProcessor(matrix, processorOptions);

            _processorOptions = processorOptions;
            _processor = processor;

            var imageTexture = new ImageTexture();
            var image = new Image();
            image.Create(matrixSize.X, matrixSize.Y, false, Image.Format.L8);
            imageTexture.CreateFromImage(image);
            MatrixSprite.Texture = imageTexture;

            _time = 0;
            _iteration = 0;
            InfoPanel.Time = 0;
            InfoPanel.Iteration = 0;
            InfoPanel.Died = 0;
            InfoPanel.Survived = 0;
            InfoPanel.Resurected = 0;

            Update();
        }

        public void RunHandler(bool pressed)
        {
            if (pressed && _processor is not null)
            {
                _toProcess = 1_000_000;
                ControlPanel.GetNode<Button>("Run").Text = "Stop";
            }
            else
            {
                _toProcess = 0;
                var btn = ControlPanel.GetNode<Button>("Run");
                btn.Text = "Run";
                btn.Pressed = false;
            }
        }

        public void NextGenerationHandler()
        {
            if (_processor is not null)
                _toProcess++;
        }

        public void SaveHandler()
        {
            var saveDialog = new FileDialog()
            {

            };

        }

        public void LoadHandler()
        {
            var loadDialog = new FileDialog()
            {

            };
        }

        private void ProcessNextGeneration(float delta)
        {
            _time += delta;
            _iteration++;
            
            var stats = _processor.Next();

            //emit signals
            //emit time as one variable (delta)
            InfoPanel.Time = _time;
            InfoPanel.Iteration = _iteration;

            //died-survived-resurected
            InfoPanel.Died = stats.Died;
            InfoPanel.Survived = stats.Survived;
            InfoPanel.Resurected = stats.Resurested;

            _toProcess--;
        }
    }
}