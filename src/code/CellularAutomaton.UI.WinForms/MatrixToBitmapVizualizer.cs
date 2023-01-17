namespace CellularAutomaton.UI.WinForms
{
    using System;
    using System.Drawing;

    public class MatrixToBitmapVizualizer
    {
        public Color LiveColor { get; set; } = Color.Black;
        
        public Color DeadColor { get; set; } = Color.White;

        public Bitmap Vizualize(IArray2D<bool> matrix, Bitmap bitmap = null)
        {
            bitmap ??= new Bitmap(matrix.XCount, matrix.YCount);

            for (var x1 = 0; x1 < bitmap.Width; x1++)
                for (var x2 = 0; x2 < bitmap.Height; x2++)
                    bitmap.SetPixel(x1, x2, matrix.GetAt(x1,x2) ? LiveColor : DeadColor);

            return bitmap;
        }

        //?
        public Bitmap Vizualize2(IArray2D<bool> matrix, Bitmap bitmap = null)
        {
            bitmap ??= new Bitmap(matrix.XCount, matrix.YCount);

            // Lock the bitmap's bits.  
            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            //int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            //byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            //System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Set every third value to 255. A 24bpp bitmap will look red.
            //for (int counter = 2; counter < rgbValues.Length; counter += 3)
              //  rgbValues[counter] = 255;

            // Copy the RGB values back to the bitmap
            //System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }
    }
}
