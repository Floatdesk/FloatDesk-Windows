# FloatDesk for Windows

A small always-on-top floating button for Windows. Click it to open a quick menu; drag it to move it, and it snaps to the nearest left, right or top screen edge.

## Menu actions
- **Screenshot**: opens the Windows snipping overlay (same as Win+Shift+S)
- **Power**: shuts down the PC (asks for confirmation first)
- **Lock**: locks the workstation

## Build and run
Requires the .NET 10 SDK on Windows.

```
dotnet run --project FloatingB/FloatingB.csproj
```

## Project layout
- `FloatingB/Core`: interfaces and models
- `FloatingB/Features`: one folder per feature (FloatingControl, and planned Power, Lock, Screenshot)
- `FloatingB/Infrastructure`: Windows-specific services such as window positioning
