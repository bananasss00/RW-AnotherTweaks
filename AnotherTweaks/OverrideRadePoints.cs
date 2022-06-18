using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(StorytellerUtility), nameof(StorytellerUtility.DefaultThreatPointsNow))]
    public class OverrideRadePoints // add 11k - 30k raidpoints
    {
        [HarmonyPrefix]
        public static bool RaidPoints(ref float __result)
        {
            if (!Prefs.DevMode || !Settings.Get().OverrideRadePoints)
                return true;

            __result = Settings.Get().OverrideRadePointsValue;
            return false;
        }
    }
}