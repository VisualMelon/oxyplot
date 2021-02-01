using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Running;

namespace OxyPlot.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var switcher = new BenchmarkSwitcher(assembly);
            switcher.Run(args);
        }
    }
}
