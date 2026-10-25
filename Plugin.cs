using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Constant.UI;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TMPro;
using UnityEngine;

namespace SagaButtonPrompts
{
    public enum ButtonPromptStyle
    {
        PS4,
        PS5,
        PS1,
        Switch,
        Xbox,
        Custom
    }

    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BasePlugin
    {
        public const string PluginGuid = "com.sagafrontier.sagabuttonprompts";
        public const string PluginName = "SaGa Frontier Button Prompts";
        public const string PluginVersion = "1.1.0";

        internal static new ManualLogSource Log;
        internal static ConfigEntry<ButtonPromptStyle> PromptStyle;
        internal static ConfigEntry<string> CustomPngFileName;
        internal static ConfigEntry<bool> ForceGamepadPromptsAlways;

        internal static Texture2D CustomTexture;
        internal static Sprite[] CustomSprites = new Sprite[23];
        internal static Sprite[] OriginalXboxSprites = new Sprite[23];
        internal static Texture OriginalXboxSpriteSheet;
        internal static float CachedPixelsPerUnit = 100f;
        internal static ButtonPromptStyle? LoadedStyle;

        public override void Load()
        {
            Log = base.Log;

            PromptStyle = Config.Bind(
                "General",
                "PromptStyle",
                ButtonPromptStyle.PS4,
                "Controller button prompt style to display: PS4 (Colored), PS5 (DualSense White), PS1 (Classic Grey), Switch (Nintendo B/A/Y/X), Xbox (Original), or Custom."
            );

            CustomPngFileName = Config.Bind(
                "General",
                "CustomPngFileName",
                "btn_list_custom.png",
                "PNG file name inside BepInEx/plugins/SagaButtonPrompts/ to use when PromptStyle = Custom."
            );

            ForceGamepadPromptsAlways = Config.Bind(
                "General",
                "ForceGamepadPromptsAlways",
                true,
                "If true, always display controller prompts even when no controller is currently detected (overrides keyboard prompts)."
            );

            PromptStyle.SettingChanged += OnStyleConfigChanged;
            CustomPngFileName.SettingChanged += OnStyleConfigChanged;

            Harmony.CreateAndPatchAll(typeof(Patches), PluginGuid);
            Log.LogInfo($"{PluginName} v{PluginVersion} loaded! Current PromptStyle: {PromptStyle.Value}");
        }

        private static void OnStyleConfigChanged(object sender, EventArgs e)
        {
            LoadedStyle = null;
            CustomTexture = null;
            for (int i = 0; i < CustomSprites.Length; i++)
                CustomSprites[i] = null;

            RefreshAllLoadedContainers();
        }

        internal static void RefreshAllLoadedContainers()
        {
            try
            {
                Patches.LoadButtonImage_Postfix();
                if (ButtonIconSpriteAssetContainer.Instance != null &&
                    ButtonIconSpriteAssetContainer.Instance.currentControllerSpriteAsset != null)
                {
                    ApplyToSpriteAsset(ButtonIconSpriteAssetContainer.Instance.currentControllerSpriteAsset);
                }
                ButtonIconImageChanger.AdjustButtonIcon();
            }
            catch (Exception ex)
            {
                Log.LogWarning($"Could not live-refresh icons: {ex.Message}");
            }
        }

        internal static string GetSheetFileName(ButtonPromptStyle style)
        {
            return style switch
            {
                ButtonPromptStyle.PS4 => "btn_list_ps4.png",
                ButtonPromptStyle.PS5 => "btn_list_ps5.png",
                ButtonPromptStyle.PS1 => "btn_list_ps1.png",
                ButtonPromptStyle.Switch => "btn_list_switch.png",
                ButtonPromptStyle.Custom => string.IsNullOrWhiteSpace(CustomPngFileName.Value) ? "btn_list_ps4.png" : CustomPngFileName.Value.Trim(),
                _ => null
            };
        }

        internal static bool EnsureTexturesAndSprites(float pixelsPerUnit = 100f)
        {
            ButtonPromptStyle targetStyle = PromptStyle.Value;
            if (targetStyle == ButtonPromptStyle.Xbox)
                return false;

            try
            {
                if (CustomTexture == null || LoadedStyle != targetStyle)
                {
                    string fileName = GetSheetFileName(targetStyle);
                    byte[] pngBytes = LoadPngBytes(fileName);
                    if (pngBytes == null || pngBytes.Length == 0)
                    {
                        Log.LogError($"Failed to find prompt sheet '{fileName}' for style {targetStyle}!");
                        return false;
                    }

                    CustomTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    CustomTexture.hideFlags = HideFlags.HideAndDontSave;

                    var il2cppBytes = new Il2CppStructArray<byte>(pngBytes);
                    if (!ImageConversion.LoadImage(CustomTexture, il2cppBytes, false))
                    {
                        Log.LogError($"ImageConversion.LoadImage failed for '{fileName}'!");
                        return false;
                    }

                    CustomTexture.filterMode = FilterMode.Bilinear;
                    CustomTexture.wrapMode = TextureWrapMode.Clamp;
                    CustomTexture.name = "btn_list_steam";
                    LoadedStyle = targetStyle;
                    CustomSprites[0] = null; // Force sprite recreation
                    Log.LogInfo($"Loaded '{fileName}' for PromptStyle={targetStyle} ({CustomTexture.width}x{CustomTexture.height}).");
                }

                if (CustomSprites[0] == null || Math.Abs(CachedPixelsPerUnit - pixelsPerUnit) > 0.01f)
                {
                    CachedPixelsPerUnit = pixelsPerUnit;
                    for (int i = 0; i < 23; i++)
                    {
                        Rect rect = new Rect(i * 64f, 0f, 64f, 64f);
                        Vector2 pivot = new Vector2(0.5f, 0.5f);
                        Sprite spr = Sprite.Create(CustomTexture, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.FullRect);
                        spr.hideFlags = HideFlags.HideAndDontSave;
                        // Keep the internal name "btn_list_steam_{i}" so GetTextTagButtonIcon("<sprite name=...>") matches TMP_SpriteAsset
                        spr.name = $"btn_list_steam_{i}";
                        CustomSprites[i] = spr;
                    }
                    Log.LogInfo($"Created 23 button sprites for PromptStyle={targetStyle} (PPU={pixelsPerUnit}).");
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.LogError($"Error in EnsureTexturesAndSprites: {ex}");
                return false;
            }
        }

        private static byte[] LoadPngBytes(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            string dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrEmpty(dllDir))
            {
                string externalPath = Path.Combine(dllDir, fileName);
                if (File.Exists(externalPath))
                {
                    Log.LogInfo($"Loading prompt sheet from disk: {externalPath}");
                    return File.ReadAllBytes(externalPath);
                }
            }

            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{typeof(Plugin).Namespace}.{fileName}");
            if (stream != null)
            {
                using MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms.ToArray();
            }

            return null;
        }

        internal static void ApplyToSpriteAsset(TMP_SpriteAsset spriteAsset)
        {
            if (spriteAsset == null)
                return;

            if (OriginalXboxSpriteSheet == null && spriteAsset.spriteSheet != null && spriteAsset.spriteSheet != CustomTexture)
            {
                OriginalXboxSpriteSheet = spriteAsset.spriteSheet;
            }

            if (PromptStyle.Value == ButtonPromptStyle.Xbox)
            {
                if (OriginalXboxSpriteSheet != null && spriteAsset.spriteSheet != OriginalXboxSpriteSheet)
                {
                    spriteAsset.spriteSheet = OriginalXboxSpriteSheet;
                    if (spriteAsset.material != null)
                        spriteAsset.material.mainTexture = OriginalXboxSpriteSheet;
                }
                return;
            }

            if (!EnsureTexturesAndSprites(CachedPixelsPerUnit))
                return;

            if (spriteAsset.spriteSheet != CustomTexture)
            {
                spriteAsset.spriteSheet = CustomTexture;
                if (spriteAsset.material != null)
                {
                    spriteAsset.material.mainTexture = CustomTexture;
                }

                var glyphTable = spriteAsset.spriteGlyphTable;
                if (glyphTable != null)
                {
                    int count = Math.Min(glyphTable.Count, CustomSprites.Length);
                    for (int i = 0; i < count; i++)
                    {
                        var glyph = glyphTable[i];
                        if (glyph != null && CustomSprites[i] != null)
                        {
                            glyph.sprite = CustomSprites[i];
                        }
                    }
                }

                Log.LogInfo($"Updated TMP_SpriteAsset '{spriteAsset.name}' texture to {PromptStyle.Value}.");
            }
        }
    }

    internal static class Patches
    {
        // Force controller prompts to be active even if Steam Input / XInput reports None or Xbox
        [HarmonyPatch(typeof(UIConstant), nameof(UIConstant.GetConnectControllerType))]
        [HarmonyPostfix]
        public static void GetConnectControllerType_Postfix(ref ControllerType __result)
        {
            if (Plugin.ForceGamepadPromptsAlways.Value || __result != ControllerType.None)
            {
                __result = ControllerType.Steam;
            }
        }

        // Replace sprites inside ButtonIconImageContainer.buttonImageDict after LoadButtonImage()
        [HarmonyPatch(typeof(ButtonIconImageContainer), nameof(ButtonIconImageContainer.LoadButtonImage))]
        [HarmonyPostfix]
        public static void LoadButtonImage_Postfix()
        {
            try
            {
                var dict = ButtonIconImageContainer.buttonImageDict;
                if (dict == null || !dict.ContainsKey(ButtonIconPlatformType.Steam))
                    return;

                var steamDict = dict[ButtonIconPlatformType.Steam];
                if (steamDict == null)
                    return;

                // Backup original Xbox sprites on first run
                if (Plugin.OriginalXboxSprites[0] == null && steamDict.ContainsKey(ButtonType.Pad_RD))
                {
                    for (int i = 0; i < 23; i++)
                    {
                        ButtonType bt = (ButtonType)i;
                        if (steamDict.ContainsKey(bt))
                            Plugin.OriginalXboxSprites[i] = steamDict[bt];
                    }
                }

                float ppu = 100f;
                if (Plugin.OriginalXboxSprites[0] != null && Plugin.OriginalXboxSprites[0].pixelsPerUnit > 0f)
                {
                    ppu = Plugin.OriginalXboxSprites[0].pixelsPerUnit;
                }

                if (Plugin.PromptStyle.Value == ButtonPromptStyle.Xbox)
                {
                    for (int i = 0; i < 23; i++)
                    {
                        if (Plugin.OriginalXboxSprites[i] != null)
                            steamDict[(ButtonType)i] = Plugin.OriginalXboxSprites[i];
                    }
                    return;
                }

                if (!Plugin.EnsureTexturesAndSprites(ppu))
                    return;

                for (int i = 0; i < 23; i++)
                {
                    ButtonType bt = (ButtonType)i;
                    steamDict[bt] = Plugin.CustomSprites[i];
                }

                if (!dict.ContainsKey(ButtonIconPlatformType.PS4))
                    dict.Add(ButtonIconPlatformType.PS4, steamDict);
                else
                    dict[ButtonIconPlatformType.PS4] = steamDict;

                if (!dict.ContainsKey(ButtonIconPlatformType.XboxOne))
                    dict.Add(ButtonIconPlatformType.XboxOne, steamDict);
                else
                    dict[ButtonIconPlatformType.XboxOne] = steamDict;

                if (!dict.ContainsKey(ButtonIconPlatformType.Switch))
                    dict.Add(ButtonIconPlatformType.Switch, steamDict);
                else
                    dict[ButtonIconPlatformType.Switch] = steamDict;

                Plugin.Log.LogInfo($"Applied {Plugin.PromptStyle.Value} button sprites to ButtonIconImageContainer.");
            }
            catch (Exception ex)
            {
                Plugin.Log.LogError($"Error in LoadButtonImage_Postfix: {ex}");
            }
        }

        // Guarantee GetButtonIconImage returns the custom sprite for all gamepad ButtonTypes (0..22)
        [HarmonyPatch(typeof(ButtonIconImageContainer), nameof(ButtonIconImageContainer.GetButtonIconImage))]
        [HarmonyPostfix]
        public static void GetButtonIconImage_Postfix(ButtonType buttonType, ButtonIconPlatformType buttonPlatformType, ref Sprite __result)
        {
            if (Plugin.PromptStyle.Value == ButtonPromptStyle.Xbox)
                return;

            int idx = (int)buttonType;
            if (idx >= 0 && idx < 23)
            {
                float ppu = (__result != null && __result.pixelsPerUnit > 0f) ? __result.pixelsPerUnit : Plugin.CachedPixelsPerUnit;
                if (Plugin.EnsureTexturesAndSprites(ppu))
                {
                    __result = Plugin.CustomSprites[idx];
                }
            }
        }

        // Replace TextMeshPro inline button icon sheet on Load()
        [HarmonyPatch(typeof(ButtonIconSpriteAssetContainer), nameof(ButtonIconSpriteAssetContainer.Load))]
        [HarmonyPostfix]
        public static void SpriteAssetContainer_Load_Postfix(ButtonIconSpriteAssetContainer __instance)
        {
            if (__instance != null && __instance.currentControllerSpriteAsset != null)
            {
                Plugin.ApplyToSpriteAsset(__instance.currentControllerSpriteAsset);
            }
        }

        // Ensure GetSpriteAsset() always returns the patched TMP_SpriteAsset
        [HarmonyPatch(typeof(ButtonIconSpriteAssetContainer), nameof(ButtonIconSpriteAssetContainer.GetSpriteAsset))]
        [HarmonyPostfix]
        public static void SpriteAssetContainer_GetSpriteAsset_Postfix(ref TMP_SpriteAsset __result)
        {
            if (__result != null)
            {
                Plugin.ApplyToSpriteAsset(__result);
            }
        }
    }
}
