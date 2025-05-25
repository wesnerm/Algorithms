global using BenchmarkDotNet.Attributes;
using Algorithms.Benchmark;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

// This may be useful:
// https://code-maze.com/how-to-integrate-benchmarkdotnet-with-unit-tests/

Summary summary = BenchmarkRunner.Run<Md5VsSha256>();