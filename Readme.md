# FloatDesk for Windows

A small always-on-top floating button for Windows. It rests half-tucked into the edge of the screen and slides out when your mouse comes near. Click it to open a ring of quick actions; drag it to move it, and it docks to the nearest left, right or top edge.

## Menu actions
Right-click the button and choose **Customize…** to pick up to 5 of these, choose their icons and set their order:

- **Screenshot**: opens the Windows snipping overlay (same as Win+Shift+S)
- **Power**: opens a second ring with **Restart** and **Shut down**
- **Lock**: locks the PC
- **Show desktop**, **Task Manager**, **File Explorer**, **Calculator**, **Windows Settings**

You can also change the icon on the floating button, or type your own (up to 2 characters). Settings are saved to `%AppData%\FloatDesk\settings.json`.

Right-click → **Exit FloatDesk** closes the app. It has no taskbar entry, so this is also how to stop a copy that is still running (otherwise the next build fails with "file is locked").

## Build and run
Requires the .NET 10 SDK on Windows.

```
dotnet run --project FloatingB/FloatingB.csproj
```

## Project layout
- `FloatingB/Core`: interfaces and models (settings, screen edges)
- `FloatingB/Features`: one folder per feature
  - `FloatingControl`: the button, its ring menu and the action catalog
  - `Settings`: the Customize window
  - `Power`, `Lock`, `Screenshot`: the system actions
- `FloatingB/Infrastructure`: Windows-specific services (window positioning, monitors, cursor, settings file)
- `FloatingB/Shared`: helpers and converters
