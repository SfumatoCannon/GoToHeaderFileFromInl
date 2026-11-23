# Go To Header / Inline File

English | [简体中文](README.md)

A Visual Studio extension that jumps between a `.h` file and its same-named `.inl` file. Default shortcut: `Ctrl+K, Ctrl+0` (configurable).

## Features

- When the active document is `.h`, opens the same-named `.inl` file in the same directory.
- When the active document is `.inl`, opens the same-named `.h` file.
- If the target file is already open, its window is activated instead of opening a duplicate.
- Extensions are matched case-insensitively (`.H` / `.INL` also work).
- The command is also available under the **Tools** menu: `Go to Header / Inline File (.h / .inl)`.

In the following cases only a message is shown; no file is created:

- No document is open;
- The active file is neither `.h` nor `.inl`;
- The active document has not been saved to disk;
- No same-named counterpart exists in the same directory.

## Requirements

- Visual Studio 2022 (17.0 or later) or Visual Studio 2026, amd64;
- The "Visual Studio core editor" component (CoreEditor) must be installed.

## Installation

1. Close all Visual Studio instances.
2. Double-click `GoToHeaderFileFromInl.vsix`, or choose Visual Studio Version Selector / VSIX Installer from "Open with".
3. Select the target Visual Studio instance in the installer and click **Install**.
4. Start Visual Studio.

To uninstall: Extensions → Manage Extensions → Installed → `Go To Header/Inline File` → Uninstall.

## Usage

| Action | Result |
| --- | --- |
| Press `Ctrl+K, Ctrl+0` in `Foo.h` | Opens and jumps to `Foo.inl` |
| Press `Ctrl+K, Ctrl+0` in `Foo.inl` | Opens and jumps to `Foo.h` |

The same command is available from the **Tools** menu.

## Changing the shortcut

1. Open **Tools → Options → Environment → Keyboard**.
2. Type `Go to Header` in "Show commands containing", or press your desired chord to see which command currently owns it.
3. Select `Go to Header / Inline File (.h / .inl)`, enter the new shortcut in "Press shortcut keys" and click **Assign**.
4. Click **OK** to save.

## Building from source

Requires Visual Studio with the "Visual Studio extension development" workload and the .NET Framework 4.7.2 targeting pack. The first build restores `Microsoft.VisualStudio.SDK` and `Microsoft.VSSDK.BuildTools` from NuGet.

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe" `
  GoToHeaderFileFromInl.sln -restore -p:Configuration=Release
```

Output:

```
src/GoToHeaderFileFromInl/bin/Release/net472/GoToHeaderFileFromInl.vsix
```

Debugging: open the solution in Visual Studio and have the project start `devenv.exe /rootsuffix Exp` (experimental instance); F5 deploys the extension to the experimental instance without affecting your regular Visual Studio.

## Project layout

| Path | Description |
| --- | --- |
| `GoToHeaderFileFromInl.sln` | Solution |
| `src/GoToHeaderFileFromInl/GoToHeaderFileFromInl.csproj` | VSIX project (net472, VSSDK build tools) |
| `src/GoToHeaderFileFromInl/ToggleHeaderInlPackage.cs` | Package registration/initialization, creates the command |
| `src/GoToHeaderFileFromInl/ToggleHeaderInlCommand.cs` | Command logic: locate the counterpart file and open it |
| `src/GoToHeaderFileFromInl/ToggleHeaderInlCommand.vsct` | Command definition, menu placement, default key binding |
| `src/GoToHeaderFileFromInl/source.extension.vsixmanifest` | VSIX manifest (install targets, assets) |

## Implementation notes

- Uses DTE (`EnvDTE80.DTE2`) to get the active document path and `ItemOperations.OpenFile` to open the target in the text view.
- Commands and key bindings live in the `.vsct` file and are registered globally, so the shortcut works from any window (a message is shown when there is no active document).
- Only a same-directory, same-base-name lookup is performed; the whole solution is not scanned.
