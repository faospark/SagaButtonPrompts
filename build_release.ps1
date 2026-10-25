param(
    [string]$Version = "1.2.0"
)

$ErrorActionPreference = "Stop"

$projectDir = $PSScriptRoot
$distDir = Join-Path $projectDir "dist"
$packageDir = Join-Path $distDir "package"
$zipName = "SagaButtonPrompts-v$Version.zip"
$zipPath = Join-Path $distDir $zipName

Write-Host "Building SagaButtonPrompts v$Version in Release configuration..." -ForegroundColor Cyan
dotnet build (Join-Path $projectDir "SagaButtonPrompts.csproj") -c Release

if (Test-Path $distDir) {
    Remove-Item -Recurse -Force $distDir
}

$pluginTarget = Join-Path $packageDir "BepInEx\plugins\SagaButtonPrompts"
$configTarget = Join-Path $packageDir "BepInEx\config"

New-Item -ItemType Directory -Force -Path $pluginTarget | Out-Null
New-Item -ItemType Directory -Force -Path $configTarget | Out-Null

Copy-Item (Join-Path $projectDir "bin\Release\net6.0\SagaButtonPrompts.dll") $pluginTarget -Force
Copy-Item (Join-Path $projectDir "bin\Release\net6.0\SagaButtonPrompts.pdb") $pluginTarget -Force
Copy-Item (Join-Path $projectDir "btn_list_*.png") $pluginTarget -Force

$defaultCfg = @"
## Settings file was created by plugin SaGa Frontier Button Prompts v$Version
## Plugin GUID: com.sagafrontier.sagabuttonprompts

[General]

## Controller button prompt style to display: PS4 (Colored), PS5 (DualSense White), PS1 (Classic Grey), Switch (Nintendo B/A/Y/X), Xbox (Original), or Custom.
# Setting type: ButtonPromptStyle
# Default value: PS4
# Acceptable values: PS4, PS5, PS1, Switch, Xbox, Custom
PromptStyle = PS4

## PNG file name inside BepInEx/plugins/SagaButtonPrompts/ to use when PromptStyle = Custom.
# Setting type: String
# Default value: btn_list_custom.png
CustomPngFileName = btn_list_custom.png

## If true, always display controller prompts even when no controller is currently detected (overrides keyboard prompts).
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
"@

Set-Content -Path (Join-Path $configTarget "com.sagafrontier.sagabuttonprompts.cfg") -Value $defaultCfg

Write-Host "Compressing release package to $zipPath..." -ForegroundColor Cyan
Compress-Archive -Path (Join-Path $packageDir "*") -DestinationPath $zipPath -Force

Write-Host "Release package successfully created at: $zipPath" -ForegroundColor Green

