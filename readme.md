
# C0D3RMemory

A library to read, write and scan (WIP) a process's memory. Based on Swedz C# Tutorial on Youtube.


## Usage/Examples

Install via [NuGet.org](https://www.nuget.org/packages/c0d3rmemory) and add it to you project like this:

```csharp
using C0D3RMem;

C0D3RMemory memoryManager = new C0D3RMemory("notepad");

byte[] bytesRead = new byte[10];
//Read bytes from Memory
bytesRead = memoryManager.Reader.ReadBytes(memoryManager.BaseAddress,bytes.Length);

//Write bytes to Memory
int bytesWritten = memoryManager.Writer.WriteMemory(memoryManager.BaseAddress,new byte[]{0x90, 0x90, 0x90});

//Scan memory for byte signature (WIP)
List<int> listOfFoundPattern = memoryManager.Scanner.ScanMemory("FE75??D48??99A2");
```

## Troubleshooting

For any problems related to this project, please open a Issue and be as detailed as possible!


## Contributing

Contributions are always welcome!

Use PRs to help making this great!

