``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.505
  [Host]     : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT


```
|     Method |      Mean |     Error |    StdDev | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|----------- |----------:|----------:|----------:|------------:|------------:|------------:|--------------------:|
| PascalCase |  5.232 us | 0.0239 us | 0.0223 us |      2.4109 |           - |           - |             9.91 KB |
|  CamelCase |  5.168 us | 0.0242 us | 0.0226 us |      2.4109 |           - |           - |             9.91 KB |
|  SnakeCase | 10.118 us | 0.0159 us | 0.0141 us |      2.5635 |           - |           - |            10.54 KB |
