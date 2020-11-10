using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace AnotherTweaks
{
    [HarmonyPatch(typeof(Hive), nameof(Hive.ThreatDisabled))]
    public class HiveAttackTargetDisabler
    {
        [HarmonyPrefix]
        public static bool ThreatDisabled(IAttackTargetSearcher disabledFor, ref bool __result)
        {
            if (!Settings.Get().HiveAttackTarget)
            {
                return !(__result = true);
            }
            return true;
        }
    }
}