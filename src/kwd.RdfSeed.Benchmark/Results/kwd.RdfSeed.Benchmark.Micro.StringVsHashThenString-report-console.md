
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=3.1.102
  [Host]     : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT DEBUG
  Job-TVPRDA : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT DEBUG

BuildConfiguration=Debug  

|               Method |     Mean |   Error |  StdDev | Ratio |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |---------:|--------:|--------:|------:|-------:|------:|------:|----------:|
|         FindByString | 148.9 ns | 0.50 ns | 0.44 ns |  1.00 | 0.0038 |     - |     - |      32 B |
| FindByHashThenString | 121.4 ns | 1.06 ns | 0.99 ns |  0.81 | 0.0038 |     - |     - |      32 B |
