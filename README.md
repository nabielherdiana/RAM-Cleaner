# RAMCleaner

A tiny, powerful utility to optimize memory usage on Windows.

![RAM Cleaner](Screenshot%202026-02-06%20210139.png)

## Features

- **Instant Optimization**: Automatically cleans RAM upon launch.
- **Tiny Footprint**: Only ~44 KB (Static Build).
- **Native Performance**: Written in C++ (Win32 API), no dependencies.
- **Portable**: No installation required.
- **Safe**: Uses standard Windows APIs to trim working sets.

## Download

Get the latest release from the [Releases](../../releases) page.

| File | Architecture | Size |
|------|--------------|------|
| `RAMCleanerAuto_x64.exe` | Windows 64-bit | 44 KB |
| `RAMCleanerAuto_x86.exe` | Windows 32-bit | 44 KB |

## How to Use

1. **Download** the executable for your system.
2. **Run** it (Run as Administrator for best results).
3. The tool will:
   - Display initial RAM usage
   - Clean memory pages from all processes
   - Show amount of RAM freed
4. **Press any key** to close the window.

## How It Works

It uses the native Windows API `SetProcessWorkingSetSize` to request the OS to trim the working set of running processes. This safely moves inactive memory pages to the System Page File, freeing up physical RAM for other applications. It does **not** terminate processes or delete any data.

## Building from Source

Requires MinGW-w64 (g++).

```bash
# 64-bit static build (Tiny size)
g++ -Os -s -static -o RAMCleanerAuto_x64.exe RAMCleanerAuto.cpp -lpsapi

# 32-bit static build
g++ -Os -s -static -o RAMCleanerAuto_x86.exe RAMCleanerAuto.cpp -lpsapi
```

## License

MIT License
