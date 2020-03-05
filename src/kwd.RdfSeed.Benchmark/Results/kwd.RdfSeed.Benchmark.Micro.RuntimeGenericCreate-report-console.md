
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=3.1.102
  [Host]     : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT DEBUG
  Job-TVPRDA : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT DEBUG

BuildConfiguration=Debug  

|               Method |         Mean |     Error |    StdDev |  Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |-------------:|----------:|----------:|-------:|--------:|-------:|------:|------:|----------:|
|        NewBasicThing |     6.331 ns | 0.0841 ns | 0.0787 ns |   1.00 |    0.00 | 0.0029 |     - |     - |      24 B |
| DynamicNewTypedThing | 1,102.472 ns | 4.4582 ns | 3.9521 ns | 174.30 |    2.48 | 0.0668 |     - |     - |     568 B |
|        NewTypedThing |     7.897 ns | 0.0354 ns | 0.0331 ns |   1.25 |    0.02 | 0.0029 |     - |     - |      24 B |
