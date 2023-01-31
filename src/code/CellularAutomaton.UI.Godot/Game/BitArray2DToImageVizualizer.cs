using Godot;
using System;
using System.Collections;

namespace CellularAutomaton.UI.Godot
{
    public partial class BitArray2DToImageVizualizer
    {
        public BitArray2DToImageVizualizer(BitArray2DVizuOptions _options)
        {
            Options = _options;
        }

        public BitArray2DVizuOptions Options { get; private set; }

        public void Vizualize(IReadableArray2D<bool> matrix, ImageTexture imageTexture)
        {
            var image = imageTexture.GetImage();
            //image.Lock();
            for (int y = 0; y < matrix.YCount; y++)
                for (int x = 0; x < matrix.XCount; x++)
                    image.SetPixel(x, y, matrix.GetAt(x,y) ? Options.TrueColor : Options.FalseColor);

            //image.Unlock();

            imageTexture.SetImage(image);
        }
    }
}