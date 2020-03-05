
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=3.1.102
  [Host]     : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT DEBUG
  Job-TVPRDA : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT DEBUG
  Job-PLORGU : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT DEBUG

BuildConfiguration=Debug  InvocationCount=1  

|       Method | UnrollFactor |        Mean |     Error |    StdDev |     Gen 0 |    Gen 1 |    Gen 2 |   Allocated |
|------------- |------------- |------------:|----------:|----------:|----------:|---------:|---------:|------------:|
|     Read2000 |           16 | 32,957.5 us | 169.96 us | 158.98 us |  562.5000 | 187.5000 |        - |   4648.6 KB |
|  Read2000VDS |           16 | 19,712.0 us | 156.32 us | 138.57 us | 1500.0000 | 562.5000 | 250.0000 | 10578.67 KB |
|    Write2000 |            1 |    228.1 us |  12.46 us |  34.94 us |         - |        - |        - |   153.54 KB |
| WriteVDS2000 |            1 |  7,312.4 us | 143.04 us | 209.67 us |         - |        - |        - |  7659.29 KB |
