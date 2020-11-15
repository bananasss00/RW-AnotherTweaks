using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    public class TPSPatch
    {
        public static bool Prefix(float leftX, float width, ref float curBaseY)
        {
            if ((Settings.Get().CoreSK_ShowTPSInRegularGame || Prefs.DevMode) == false)
                return false;

            TPSNew.RimWorld_GlobalControlsUtility_DoTimespeedControls_Postfix(leftX, width, ref curBaseY);
            //TicksPerSecond.RimWorld_GlobalControlsUtility_DoTimespeedControls_Postfix(leftX, width, ref curBaseY);
            return false;
        }
    }

    public static class TPSNew
    {
        public static void RimWorld_GlobalControlsUtility_DoTimespeedControls_Postfix(float leftX, float width, ref float curBaseY)
        {
            float tickRateMultiplier = Find.TickManager.TickRateMultiplier;
            int num = (int)Math.Round((double)((tickRateMultiplier == 0f) ? 0f : (60f * tickRateMultiplier)));
            if (PrevTicks == -1)
            {
                PrevTicks = GenTicks.TicksAbs;
                PrevTime = DateTime.Now;
            }
            else
            {
                DateTime now = DateTime.Now;
                if (now.Second != PrevTime.Second)
                {
                    PrevTime = now;
                    TPSActual = GenTicks.TicksAbs - PrevTicks;
                    PrevTicks = GenTicks.TicksAbs;
                }
            }
            Rect rect = new Rect(leftX - 20f, curBaseY - 26f, width + 20f - 7f, 26f);
            Text.Anchor = TextAnchor.MiddleRight;

            string text = Settings.Get().CoreSK_ShowRaidPoints
                ? $"TPS: {TPSActual}({num}) P: {Mathf.Round(StorytellerUtility.DefaultThreatPointsNow(Find.CurrentMap))}"
                : $"TPS: {TPSActual}({num})";

            Widgets.Label(rect, text);
            Text.Anchor = TextAnchor.UpperLeft;
            curBaseY -= 26f;
        }

        private static DateTime PrevTime;

        private static int PrevTicks = -1;

        private static int TPSActual = 0;
    }
}