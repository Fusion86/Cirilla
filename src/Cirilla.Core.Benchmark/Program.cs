using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Cirilla.Core.Models;

namespace Cirilla.Core.Benchmark
{
    [MemoryDiagnoser]
    public class LoadSaveBenchmark
    {
        [Benchmark]
        public void LoadSave()
        {
            var save = new SaveData("C:/Steam/userdata/112073240/582010/remote/SAVEDATA1000");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<LoadSaveBenchmark>();
        }
    }
}
