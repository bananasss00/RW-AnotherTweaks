using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    public class DropOneWithControl
    {
        public static bool InterfaceDrop(ITab_Pawn_Gear __instance, Thing t)
        {
            if (!Event.current.control || t.stackCount < 2)
                return true;

            Pawn selPawnForGear = __instance.SelPawnForGear; // Traverse.Create(__instance).Property("SelPawnForGear", null).GetValue<Pawn>();

            ThingWithComps thingWithComps = t as ThingWithComps;
            if (t is Apparel apparel && selPawnForGear.apparel != null && selPawnForGear.apparel.WornApparel.Contains(apparel))
            {
                // RemoveApparel
            }
            else if (thingWithComps != null && selPawnForGear.equipment != null && selPawnForGear.equipment.AllEquipmentListForReading.Contains(thingWithComps))
            {
                // DropEquipment
            }
            else if (!t.def.destroyOnDrop)
            {
                selPawnForGear.inventory.innerContainer.TryDrop(t, selPawnForGear.Position, selPawnForGear.Map, ThingPlaceMode.Near, 1, out _);
            }

            return false;
        }
    }
}