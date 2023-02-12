using Godot;
using System;

namespace CellularAutomaton.UI.Godot
{
	public partial class Arena : ColorRect
	{
		private AppGlobal _global;

		private double _time; // processing time
		private int _iteration; // generations
		private int _toProcess;

		private GenerationProcessorOptions _processorOptions;
		private GenerationProcessor _processor;

		public Arena()
		{
			VizuOptions = new();
			Vizer = new BitArray2DToImageVizualizer(VizuOptions);
		}

		[Signal] 
		public delegate void TimeChangedEventHandler(double time);

		[Signal] 
		public delegate void IterationChangedEventHandler(int iteration, int died, int survived, int resurected);

		public BitArray2DToImageVizualizer Vizer { get; set; }

		public BitArray2DVizuOptions VizuOptions { get; private set; }

		public ControlPanel ControlPanel { get; private set; }

		public InfoPanel InfoPanel { get; private set; }

		protected Sprite2D MatrixSprite { get; private set; }

		public override void _Ready()
		{
			_global = GetTree().Root.GetNode<AppGlobal>("AppGlobal");
			InfoPanel = GetNode<InfoPanel>("%InfoPanel");
			ControlPanel = GetNode<ControlPanel>("%ControlPanel");
			MatrixSprite = GetNode<Sprite2D>("%MatrixSprite");
			
			ControlPanel.GetNode("CreateNew").Connect("pressed",new Callable(this,nameof(CreateNewHandler)));
			ControlPanel.GetNode("Run").Connect("toggled",new Callable(this,nameof(RunHandler)));
			ControlPanel.GetNode("NextGen").Connect("pressed",new Callable(this,nameof(NextGenerationHandler)));
			ControlPanel.GetNode("Save").Connect("pressed",new Callable(this,nameof(SaveHandler)));
			ControlPanel.GetNode("Load").Connect("pressed",new Callable(this,nameof(LoadHandler)));
		}

		public override void _Process(double delta)
		{
			if (_toProcess <= 0)
				return;

			if (_processor is not null)
			{
				ProcessNextGeneration(delta);
				QueueRedraw();
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

			var image = Image.Create(matrixSize.X, matrixSize.Y, false, Image.Format.L8);
			var imageTexture = ImageTexture.CreateFromImage(image);
			MatrixSprite.Texture = imageTexture;

			_time = 0;
			_iteration = 0;
			EmitSignal(nameof(TimeChanged), _time);
			EmitSignal(nameof(IterationChanged), 0, 0, 0, 0);

			QueueRedraw();
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
				btn.ButtonPressed = false;
			}
		}

		public void NextGenerationHandler()
		{
			if (_processor is not null)
				_toProcess++;
		}

		public void SaveHandler()
		{
			var saveFileDialog = new FileDialog()
			{
				
			};

			AddChild(saveFileDialog);
			saveFileDialog.Show();

			//if (saveFileDialog.ShowDialog() == DialogResult.OK)
			//{
			//    Stream stream;
			//    if ((stream = saveFileDialog.OpenFile()) != null)
			//    {
			//        var strMatrix = _serializer.Serialize(Matrix);

			//        using StreamWriter writer = new StreamWriter(stream);
			//        writer.Write(strMatrix);
			//    }
			//}

		}

		public void LoadHandler()
		{
			var loadDialog = new FileDialog()
			{

			};
		}

		private void ProcessNextGeneration(double delta)
		{
			_time += delta;
			_iteration++;
			
			var stats = _processor.Next();

			EmitSignal(nameof(TimeChanged), _time);
			EmitSignal(nameof(IterationChanged), _iteration, stats.Died, stats.Survived, stats.Revived);

			_toProcess--;
		}
	}
}
