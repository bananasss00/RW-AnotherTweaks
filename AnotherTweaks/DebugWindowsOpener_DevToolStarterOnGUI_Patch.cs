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
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static void ImmediateWindow(int ID, Rect rect, WindowLayer layer, ref Action doWindowFunc, bool doBackground, bool absorbInputAroundWindow, float shadowAlpha)
        {
            if (ID != 1593759361 || !Settings.Get().DevToolsWithHoldShift || Event.current.shift) // window from DebugWindowsOpener.DevToolStarterOnGUI
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

            //if (!Prefs.DevMode)
            //    return false;

            //if (!Settings.Get().DevToolsWithHoldShift)
            //    return true;

            //if (!Event.current.shift)
            //{
            //    if (DrawInfoCached == null)
            //    {
            //        DrawInfoCached = () =>
            //        {
            //            ___widgetRow.Init(0f, 0f);
            //            ___widgetRow.Label(DebugSettings.godMode ? "Dev mode. God mode active" : "Dev mode");
            //        };
            //    }

            //    // original code DevToolStarterOnGUI
            //    Vector2 center = new Vector2(UI.screenWidth * 0.5f, 3f);
            //    int num = 6;
            //    if (Current.ProgramState == ProgramState.Playing)
            //        num += 2;
            //    float num2 = 25f;
            //    if (Current.ProgramState == ProgramState.Playing && DebugSettings.godMode)
            //        num2 += 15f;
            //    Find.WindowStack.ImmediateWindow(1593759361, new Rect(center.x, center.y, num * 28f - 4f + 1f, num2).Rounded(), WindowLayer.GameUI, _drawInfoCached, false, false, 0f);
            //    return false;
            //}
            //return true;
        }
    }
}