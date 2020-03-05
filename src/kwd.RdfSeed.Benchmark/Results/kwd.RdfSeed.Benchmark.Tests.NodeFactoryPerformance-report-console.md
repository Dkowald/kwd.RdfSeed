
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=3.1.102
  [Host]     : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT DEBUG
  Job-TVPRDA : .NET Core 3.1.2 (CoreCLR 4.700.20.6602, CoreFX 4.700.20.6702), X64 RyuJIT DEBUG

BuildConfiguration=Debug  

|                                       Method |      Mean |    Error |   StdDev |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
|--------------------------------------------- |----------:|---------:|---------:|--------:|-------:|------:|----------:|
| 'Create 100X4 unique nodes of various types' | 135.33 us | 1.182 us | 1.106 us |  5.8594 | 0.2441 |     - |  48.27 KB |
|                         '100 new text nodes' |  69.23 us | 0.835 us | 0.740 us |  6.7139 |      - |     - |  55.65 KB |
|                          '100 new uri nodes' |  73.08 us | 0.766 us | 0.717 us |  5.9814 | 0.3662 |     - |  49.34 KB |
|                      '100 new literal nodes' | 161.52 us | 3.155 us | 2.951 us |  7.0801 | 0.2441 |     - |  59.58 KB |
|                       '100 new double nodes' | 291.89 us | 2.949 us | 2.615 us | 20.9961 | 0.4883 |     - |  174.5 KB |
|                '100 new typed literal nodes' | 162.92 us | 2.322 us | 2.172 us |  7.3242 | 0.2441 |     - |  60.14 KB |
