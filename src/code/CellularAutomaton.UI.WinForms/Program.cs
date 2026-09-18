namespace CellularAutomaton.UI.WinForms;

using System;
using System.Windows.Forms;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        using var screen = new LifeViewer(
            (xcount, ycount) => BitArray2D.Create(xcount, ycount, (_, _) => Random.Shared.Next(2) == 0))
        {
            ProcessorOptions = new GenerationProcessorOptions { CleanBorders = true },
        };

        Application.Run(screen);
    }
}
