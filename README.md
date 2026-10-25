# SaGaButtonPrompts

A BepInEx 6 (IL2CPP) mod for **SaGa Frontier Remastered** that replaces the default Xbox controller button prompt icons in the UI and dialogue text tags with customized PlayStation (PS4, PS5, PS1), Nintendo Switch, or user-supplied custom button glyphs.

## Features

- **Multiple Prompt Styles**:
  - `PS4`: DualShock 4 styled button prompts with iconic colors (Cross, Circle, Square, Triangle, L1/R1, L2/R2, Options, Share/Touchpad).
  - `PS5`: DualSense styled button prompts (White / Modern minimal glyphs).
  - `PS1`: Classic PSone grey styled button glyphs.
  - `Switch`: Nintendo Switch style button prompts (B/A/Y/X layout).
  - `Xbox`: Original Xbox prompt sheet.
  - `Custom`: Loads any custom 23-icon horizontal sprite sheet (`1472x64` or equivalent aspect ratio) from disk.
- **Dynamic Dialogue & Tag Replacement**: Patches TextMeshPro tags and `ButtonIconSpriteAssetContainer` to ensure in-game text prompts (`<sprite name="btn_list_steam_X">`) dynamically show the selected controller layout.
- **Force Controller Prompts Option**: Configurable option to force controller prompt icons even when playing on PC / Steam Deck without an active gamepad detected.
- **Live Reload Support**: Automatically reloads sprites when configuration entries change.

## Configuration

Configuration is located at `BepInEx/config/com.sagafrontier.sagabuttonprompts.cfg`:

```ini
[General]

## Controller button prompt style: PS4, PS5, PS1, Switch, Xbox, or Custom.
# Setting type: ButtonPromptStyle
# Default value: PS4
# Acceptable values: PS4, PS5, PS1, Switch, Xbox, Custom
PromptStyle = PS4

## PNG file name inside BepInEx/plugins/SagaButtonPrompts/ to use when PromptStyle = Custom.
# Setting type: String
# Default value: btn_list_custom.png
CustomPngFileName = btn_list_custom.png

## If true, always display controller prompts even when no controller is currently detected.
# Setting type: Boolean
# Default value: true
ForceGamepadPromptsAlways = true
```

## Building

Prerequisites:
- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or newer
- BepInEx 6.x IL2CPP installed in the game directory

To build in Release configuration:
```bash
dotnet build -c Release
```

The build output is automatically copied to `../../BepInEx/plugins/SagaButtonPrompts/`.
