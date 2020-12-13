using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    [HarmonyPatch(typeof(DebugActionsUtility), nameof(DebugActionsUtility.PointsOptions))]
    public class MoreDevRaidpoints // add 11k - 30k raidpoints
    {
        [HarmonyPostfix]
        public static void PointsOptions_Postfix(bool extended, ref IEnumerable<float> __result)
        {
            var raidPoints = __result.ToList();

            for (int i = 11000; i <= 30000; i += 10000)
                raidPoints.Add(i);

            __result = raidPoints;
        }
    }
}