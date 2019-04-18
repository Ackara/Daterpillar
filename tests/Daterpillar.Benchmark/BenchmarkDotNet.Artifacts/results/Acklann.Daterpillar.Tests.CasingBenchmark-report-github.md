``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.505
  [Host]     : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT


```
|     Method |     Mean |     Error |    StdDev |
|----------- |---------:|----------:|----------:|
| PascalCase | 1.817 us | 0.0105 us | 0.0093 us |
|  CamelCase | 2.277 us | 0.0094 us | 0.0079 us |
|  SnakeCase | 5.357 us | 0.0092 us | 0.0086 us |
