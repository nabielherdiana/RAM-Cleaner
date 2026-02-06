# RAM Cleaner

Lightweight Windows utility to optimize RAM usage by trimming working set memory from running processes.

## Features

- Clean and minimal interface
- Works on both 32-bit and 64-bit Windows
- Displays real-time memory statistics
- Shows freed memory after cleaning
- Desktop notification on completion
- Portable - no installation required
- Tiny footprint (~14 KB)

## Screenshot

![RAM Cleaner](screenshot.png)

## Download

Get the latest release from the [Releases](../../releases) page.

| File | Architecture |
|------|--------------|
| `RAMCleaner_x64.exe` | Windows 64-bit |
| `RAMCleaner_x86.exe` | Windows 32-bit |

## Usage

1. Download the appropriate version for your system
2. Run the executable (requires Administrator privileges)
3. Click **CLEAN RAM**
4. Check Task Manager to verify results

## How It Works

The application uses the Windows API `SetProcessWorkingSetSize` to request the operating system to trim the working set of each running process. This moves inactive memory pages to the page file, freeing up physical RAM for other applications.

This is a safe, documented Windows memory management operation that does not delete files or modify system settings.

## Requirements

- Windows 7 / 8 / 10 / 11
- .NET Framework 4.0 (included in Windows by default)
- Administrator privileges for full functionality

## Building from Source

Compile using the .NET Framework C# compiler:

```batch
:: 64-bit
%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /platform:x64 /out:RAMCleaner_x64.exe /win32manifest:app.manifest /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /reference:Microsoft.VisualBasic.dll RAMCleaner.cs

:: 32-bit
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe /target:winexe /platform:x86 /out:RAMCleaner_x86.exe /win32manifest:app.manifest /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /reference:Microsoft.VisualBasic.dll RAMCleaner.cs
```

Or run `build_all.bat` to compile both versions.

## License

MIT License
