@echo off
echo ========================================
echo   RAM Cleaner Build Script (32-bit)
echo ========================================
echo.

set CSC32=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe

if not exist "%CSC32%" (
    echo ERROR: C# compiler not found at:
    echo %CSC32%
    echo.
    echo Please ensure .NET Framework 4.0 is installed.
    pause
    exit /b 1
)

echo Building RAMCleaner_x86.exe...
echo.

"%CSC32%" /nologo /target:winexe /platform:x86 /out:RAMCleaner_x86.exe /win32manifest:app.manifest /reference:System.dll /reference:System.Drawing.dll /reference:System.Windows.Forms.dll /reference:Microsoft.VisualBasic.dll RAMCleaner.cs

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   SUCCESS! Created: RAMCleaner_x86.exe
    echo ========================================
) else (
    echo.
    echo ========================================
    echo   BUILD FAILED!
    echo ========================================
)

echo.
pause
