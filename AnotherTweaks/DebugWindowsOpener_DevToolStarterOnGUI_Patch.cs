using System;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    [HarmonyPatch(typeof(DebugWindowsOpener), nameof(DebugWindowsOpener.DevToolStarterOnGUI))]
    public class DebugWindowsOpener_DevToolStarterOnGUI_Patch
    {
        private static Action _drawInfoCached;

        [HarmonyPrefix]
        public static bool DebugWindowsOpener_DevToolStarterOnGUI_Prefix(WidgetRow ___widgetRow)
        {
            if (!Prefs.DevMode)
                return false;

            if (!Settings.Get().DevToolsWithHoldShift)
                return true;

            if (!Event.current.shift)
            {
                if (_drawInfoCached == null)
                {
                    _drawInfoCached = () =>
                    {
                        ___widgetRow.Init(0f, 0f);
                        ___widgetRow.Label(DebugSettings.godMode ? "Dev mode. God mode active" : "Dev mode");
                    };
                }

                // original code DevToolStarterOnGUI
                Vector2 center = new Vector2(UI.screenWidth * 0.5f, 3f);
                int num = 6;
                if (Current.ProgramState == ProgramState.Playing)
                    num += 2;
                float num2 = 25f;
                if (Current.ProgramState == ProgramState.Playing && DebugSettings.godMode)
                    num2 += 15f;
                Find.WindowStack.ImmediateWindow(1593759361, new Rect(center.x, center.y, num * 28f - 4f + 1f, num2).Rounded(), WindowLayer.GameUI, _drawInfoCached, false, false, 0f);
                return false;
            }
            return true;
        }
    }
}