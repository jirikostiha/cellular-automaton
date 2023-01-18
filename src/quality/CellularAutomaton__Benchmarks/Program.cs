using BenchmarkDotNet.Running;

namespace CellularAutomaton
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var a = BenchmarkRunner.Run<ProcessorNextGeneration>();

            Console.ReadKey();
        }
    }
}
