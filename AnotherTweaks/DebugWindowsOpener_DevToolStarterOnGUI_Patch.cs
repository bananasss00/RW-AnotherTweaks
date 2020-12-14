using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    [DefOf]
    public static class KeyBindingDefOf
    {
        static KeyBindingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(KeyBindingDefOf));
        }

        public static KeyBindingDef Dev_ToggleDevMode;
    }

    // replace action here, because DebugWindowsOpener.DevToolStarterOnGUI handle dev key binds
    [HarmonyPatch(typeof(WindowStack), nameof(WindowStack.ImmediateWindow))]
    public class WindowStack_ImmediateWindow_Patch
    {
        public const int DevToolStarterOnGUI_ID = 1593759361;

        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static void ImmediateWindow(int ID, ref Rect rect, WindowLayer layer, ref Action doWindowFunc, bool doBackground, bool absorbInputAroundWindow, float shadowAlpha)
        {
            if (ID != DevToolStarterOnGUI_ID) // window from DebugWindowsOpener.DevToolStarterOnGUI
                return;

            var settings = Settings.Get();
            if (settings.DevToolsPositionX != 0) rect.x = settings.DevToolsPositionX;
            if (settings.DevToolsPositionY != 0) rect.y = settings.DevToolsPositionY;

            if (!settings.DevToolsWithHoldShift || Event.current.control)
                return;

            doWindowFunc = DebugWindowsOpener_DevToolStarterOnGUI_Patch.DrawInfoCached;
        }
    }

    [HarmonyPatch(typeof(DebugWindowsOpener), nameof(DebugWindowsOpener.DevToolStarterOnGUI))]
    public class DebugWindowsOpener_DevToolStarterOnGUI_Patch
    {
        public static Action DrawInfoCached;

        [HarmonyPrefix]
        public static bool DebugWindowsOpener_DevToolStarterOnGUI_Prefix(WidgetRow ___widgetRow)
        {
            if (KeyBindingDefOf.Dev_ToggleDevMode.KeyDownEvent)
            {
                Prefs.DevMode = !Prefs.DevMode;
                Event.current.Use();
            }

            if (DrawInfoCached == null)
            {
                DrawInfoCached = () =>
                {
                    ___widgetRow.Init(0f, 0f);
                    ___widgetRow.Label(DebugSettings.godMode ? "Dev mode. God mode active" : "Dev mode");
                };
            }
            return true;
        }
    }
}