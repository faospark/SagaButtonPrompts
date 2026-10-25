# Changelog

All notable changes to **SaGaButtonPrompts** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.2.0] - 2026-10-25

### Added
- **In-Game Hotkey Cycle (`F9`)**: Added configurable hotkey (`HotkeyCycleStyle`, default `F9`) to cycle sequentially through button prompt styles (`PS4` &rarr; `PS5` &rarr; `PS1` &rarr; `Switch` &rarr; `Xbox` &rarr; `Custom`) at runtime with automatic config persistence.
- **Hotkeys Configuration Section**: Added `EnableHotkeys` toggle and configurable keybinding for prompt style cycling in `com.sagafrontier.sagabuttonprompts.cfg`.
- **Direct BepInEx Download Link**: Updated installation guide with direct link to BepInEx 6.0.0-pre.2 IL2CPP x64 release.

---

## [1.1.0] - 2026-10-25

### Added
- **Multi-Style Controller Prompts**:
  - `PS4`: DualShock 4 iconic colored prompts (Cross, Circle, Square, Triangle, D-pad, Bumpers, Triggers, Options, Share/Touchpad).
  - `PS5`: DualSense minimal white glyphs.
  - `PS1`: Classic PSone retro grey prompts.
  - `Switch`: Nintendo Switch button layout (B/A/Y/X).
  - `Xbox`: Original Xbox prompt set.
  - `Custom`: External PNG sprite sheet loading support via `CustomPngFileName`.
- **Dynamic TextMeshPro Tag Patching**: Intercepts in-game dialogue, system text, and tutorial tags (`<sprite name="btn_list_steam_X">`) and updates them dynamically to match the active controller style.
- **`ForceGamepadPromptsAlways` Option**: Forces controller glyphs even when keyboard and mouse are used, or when Steam Input is running in non-exclusive mode.
- **Live Configuration Hot-Reload**: Automatically reloads and repaints button prompt sheets and UI containers when saving changes in the configuration file (`com.sagafrontier.sagabuttonprompts.cfg`) without needing to restart the game.
- **Embedded Fallback Assets**: Sprite sheets are compiled directly into the DLL as embedded resources, with disk override capability.
- **Root-Drop Directory Bundling**: Distributed with full directory hierarchy (`BepInEx/plugins/...`, `BepInEx/config/...`) for simple drag-and-drop installation into the Steam game root.

### Changed
- Rebranded and refactored plugin architecture from `PlayStationPrompts` to `SagaButtonPrompts`.
- Updated configuration file naming to `com.sagafrontier.sagabuttonprompts.cfg`.

---

## [1.0.0] - 2026-10-24

### Added
- Initial release replacing default Xbox controller button prompt icons in SaGa Frontier Remastered with PlayStation prompts using BepInEx 6 IL2CPP and Harmony.

