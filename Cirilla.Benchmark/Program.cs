using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cirilla.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            var type = typeof(IBenchmark);
            List<Type> benchmarks = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)).ToList();

            foreach (Type t in benchmarks)
                BenchmarkRunner.Run(t);
        }
    }
}
