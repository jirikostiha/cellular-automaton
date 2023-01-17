namespace CellularAutomaton.UI.WinForms
{
    using System;
    using System.Windows.Forms;

    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            var screen = new LifeViewer(
                (xcount, ycount) => BitArray2D.Create(xcount, ycount, (x, y) => rnd.NextDouble() >= 0.5d ? true : false))
            {
                ProcessorOptions = new GenerationProcessorOptions { CleanBorders = true }
            };

            Application.Run(screen);
        }
    }
}
