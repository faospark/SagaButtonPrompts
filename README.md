# SaGaButtonPrompts

A **BepInEx 6 (IL2CPP)** mod for **SaGa Frontier Remastered** (Steam) that replaces the default Xbox controller button prompt icons in the UI and dialogue text tags with customized PlayStation (PS4, PS5, PS1), Nintendo Switch, or user-supplied custom button glyphs.

---

## Features

- **Multiple Controller Prompt Styles**:
  - `PS4`: DualShock 4 styled button prompts with iconic colors (Cross, Circle, Square, Triangle, L1/R1, L2/R2, Options, Share/Touchpad).
  - `PS5`: DualSense styled button prompts (White / Modern minimal glyphs).
  - `PS1`: Classic PSone retro grey styled button glyphs.
  - `Switch`: Nintendo Switch style button prompts (B/A/Y/X layout).
  - `Xbox`: Original Xbox prompt sheet.
  - `Custom`: Loads any custom 23-icon horizontal sprite sheet (`1472x64` or equivalent aspect ratio) from disk.
- **Dynamic Dialogue & Tag Replacement**: Patches TextMeshPro tags and `ButtonIconSpriteAssetContainer` to ensure in-game text prompts (`<sprite name="btn_list_steam_X">`) dynamically show the selected controller layout.
- **Force Controller Prompts Option**: Configurable option to force controller prompt icons even when playing on PC / Steam Deck without an active gamepad detected.
- **In-Game Hotkey Cycle (`F9`)**: Press **`F9`** anytime during gameplay to cycle through styles on-the-fly (`PS4` &rarr; `PS5` &rarr; `PS1` &rarr; `Switch` &rarr; `Xbox` &rarr; `Custom`) with instant UI updates and automatic config persistence.
- **Live Reload Support**: Automatically reloads sprites when configuration entries change without requiring a game restart.

---

## Requirements

1. **SaGa Frontier Remastered** (Steam edition, 64-bit Windows)
2. **BepInEx 6.0 (Unity IL2CPP x64)**
   - Download **[BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip)**.

---

## Installation Guide (Steam)

### 1. Locate Game Root Directory
In your Steam client:
1. Open your **Library** and right-click **SaGa Frontier Remastered**.
2. Select **Manage** &rarr; **Browse local files**.
3. A file explorer window will open at your game's root directory:
   ```text
   ...\steamapps\common\SaGa Frontier Remastered\
   ```
   *(This folder contains `SaGa Frontier Remastered.exe`).*

---

### 2. Install BepInEx 6 (If not already installed)
1. Download **[BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-Unity.IL2CPP-win-x64-6.0.0-pre.2.zip)**.
2. Extract the contents of the ZIP archive directly into your **`SaGa Frontier Remastered`** root folder.
3. You should see `doorstop_config.ini`, `winhttp.dll` (or `version.dll`), and the `BepInEx/` directory alongside `SaGa Frontier Remastered.exe`.
4. **Launch the game once via Steam**, then quit to desktop.
   - This generates necessary IL2CPP interop assemblies and configuration directories.

---

### 3. Install SaGaButtonPrompts
The release package is bundled with the complete directory structure for instant drag-and-drop installation.

1. Download **`SagaButtonPrompts-vX.X.X.zip`** from [Releases](https://github.com/faospark/SagaButtonPrompts/releases).
2. Open the ZIP archive. You will see a `BepInEx` folder.
3. **Drag and drop the `BepInEx` folder directly into your game's root directory**:
   ```text
   SaGa Frontier Remastered/
   ├── BepInEx/
   │   ├── config/
   │   │   └── com.sagafrontier.sagabuttonprompts.cfg
   │   └── plugins/
   │       └── SagaButtonPrompts/
   │           ├── SagaButtonPrompts.dll
   │           ├── btn_list_ps4.png
   │           ├── btn_list_ps5.png
   │           ├── btn_list_ps1.png
   │           └── btn_list_switch.png
   ├── doorstop_config.ini
   └── SaGa Frontier Remastered.exe
   ```
4. If prompted by Windows, choose **Replace the files in the destination**.

---

### 4. Verify Installation
1. Start the game through Steam.
2. If BepInEx console is enabled, you will see:
   ```text
   [Info   :   BepInEx] Loading [SaGa Frontier Button Prompts 1.1.0]
   [Info   :SaGa Frontier Button Prompts] SaGa Frontier Button Prompts v1.1.0 loaded! Current PromptStyle: PS4
   [Info   :SaGa Frontier Button Prompts] Loaded 'btn_list_ps4.png' for PromptStyle=PS4 (1472x64).
   ```
3. Your controller prompts in the title screen, battle menus, and dialogue boxes will now display PlayStation button glyphs.

---

## Configuration & Style Selection

Edit `BepInEx/config/com.sagafrontier.sagabuttonprompts.cfg` with Notepad or any text editor:

```ini
[General]

## Controller button prompt style to display:
## Options: PS4, PS5, PS1, Switch, Xbox, Custom
# Setting type: ButtonPromptStyle
# Default value: PS4
PromptStyle = PS4

## PNG file name inside BepInEx/plugins/SagaButtonPrompts/ to use when PromptStyle = Custom.
# Setting type: String
# Default value: btn_list_custom.png
CustomPngFileName = btn_list_custom.png

## If true, always display controller prompts even when no controller is currently detected.
# Setting type: Boolean
# Default value: true
ForceGamepadPromptsAlways = true

[Hotkeys]

## Enable in-game hotkeys to control button prompts.
# Setting type: Boolean
# Default value: true
EnableHotkeys = true

## Hotkey to cycle to the next controller button prompt style (Default: F9).
# Setting type: KeyCode
# Default value: F9
HotkeyCycleStyle = F9
```

### In-Game Hotkey (`F9`)
You can cycle prompt styles on-the-fly without restarting the game or editing files:
- Press **`F9`** anytime during gameplay to cycle to the next style (`PS4` &rarr; `PS5` &rarr; `PS1` &rarr; `Switch` &rarr; `Xbox` &rarr; `Custom`).
- The mod immediately updates both UI button sprites and dialogue text tags, and automatically persists your selection to `com.sagafrontier.sagabuttonprompts.cfg`.
- The hotkey can be rebound to any Unity `KeyCode` or disabled via `EnableHotkeys = false`.

### Config File Hot-Reloading
You can also change settings externally while the game is running:
1. Edit `BepInEx/config/com.sagafrontier.sagabuttonprompts.cfg`.
2. Change `PromptStyle = PS5` (or `PS1`, `Switch`, `Xbox`, `Custom`) and save.
3. The mod will instantly hot-reload and update on-screen button icons immediately.

---

## Building from Source

Prerequisites:
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or newer
- BepInEx 6.x IL2CPP installed in your game directory

To compile:
```bash
dotnet build -c Release
```

The build target automatically compiles the DLL and deploys it to `../../BepInEx/plugins/SagaButtonPrompts/`.

---

## License

This project is licensed under the [MIT License](LICENSE).

## Credits

- **Author**: [faospark](https://github.com/faospark)
- **Target Game**: SaGa Frontier Remastered (Square Enix)
- **Framework**: [BepInEx](https://github.com/BepInEx/BepInEx) & [HarmonyX](https://github.com/BepInEx/HarmonyX)
