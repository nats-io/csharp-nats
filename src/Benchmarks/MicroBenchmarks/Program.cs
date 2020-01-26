﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using NATS.Client;
using NATS.Client.Internals;

namespace MicroBenchmarks
{
    [DisassemblyDiagnoser(printAsm: true, printSource: true)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [RPlotExporter]
    [MarkdownExporterAttribute.GitHub]
    [SimpleJob(RuntimeMoniker.Net462)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class RandomBenchmark
    {
        private readonly NUID _nuid = NUID.Instance;
        private readonly Nuid _newNuid = new Nuid(null, 0, 1);

        public RandomBenchmark()
        {
            _nuid.Seq = 0;
        }

        [BenchmarkCategory("NextNuid")]
        [Benchmark(Baseline = true)]
        public string NUIDNext() => _nuid.Next;

        [BenchmarkCategory("NextNuid"), Benchmark]
        public string NextNuid() => _newNuid.GetNext();

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<RandomBenchmark>();
        }
    }
}
