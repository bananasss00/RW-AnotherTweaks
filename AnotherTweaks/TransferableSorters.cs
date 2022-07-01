using System;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    public class TransferableComparer_ArmorRating_Sharp : TransferableComparer
    {
        public override int Compare(Transferable lhs, Transferable rhs)
        {
            if (!lhs.AnyThing.def.IsApparel && !rhs.AnyThing.def.IsApparel)
                return 0;
            if (lhs.AnyThing.def.IsApparel && !rhs.AnyThing.def.IsApparel)
                return -1;
            if (!lhs.AnyThing.def.IsApparel && rhs.AnyThing.def.IsApparel)
                return 1;
            return -lhs.AnyThing.GetStatValue(StatDefOf.ArmorRating_Sharp).CompareTo(rhs.AnyThing.GetStatValue(StatDefOf.ArmorRating_Sharp));
        }
    }
    public class TransferableComparer_ArmorRating_Blunt : TransferableComparer
    {
        public override int Compare(Transferable lhs, Transferable rhs)
        {
            if (!lhs.AnyThing.def.IsApparel && !rhs.AnyThing.def.IsApparel)
                return 0;
            if (lhs.AnyThing.def.IsApparel && !rhs.AnyThing.def.IsApparel)
                return -1;
            if (!lhs.AnyThing.def.IsApparel && rhs.AnyThing.def.IsApparel)
                return 1;
            return -lhs.AnyThing.GetStatValue(StatDefOf.ArmorRating_Blunt).CompareTo(rhs.AnyThing.GetStatValue(StatDefOf.ArmorRating_Blunt));
        }
    }

    public class TransferableComparer_DPS : TransferableComparer
    {
        public override int Compare(Transferable lhs, Transferable rhs)
        {
            if (!lhs.AnyThing.def.IsWeapon && !rhs.AnyThing.def.IsWeapon)
                return 0;
            if (lhs.AnyThing.def.IsWeapon && !rhs.AnyThing.def.IsWeapon)
                return -1;
            if (!lhs.AnyThing.def.IsWeapon && rhs.AnyThing.def.IsWeapon)
                return 1;
            return -GetDps(lhs.AnyThing).CompareTo(GetDps(rhs.AnyThing));
        }

        private static float GetDps(Thing th)
        {
            if (th.def.IsRangedWeapon)
                return GetDpsRanged(th);
            if (th.def.IsMeleeWeapon && !th.def.IsStuff && !th.def.CountAsResource)
                return GetDpsMelee(th);
            return 0f;
        }

        private static float GetDpsMelee(Thing th)
        {
            return (float)Math.Round(th.GetStatValue(StatDefOf.MeleeWeapon_AverageDPS), 2);
        }

        private static float GetDpsRanged(Thing th)
        {
            float warmup = 0f, damage = 0f;
            int burstShotCount = 0, ticksBetweenBurstShots = 0;
            if (th.def?.Verbs != null) {
                try {
                    foreach (VerbProperties vp in th.def.Verbs) {
                        if (vp.ToString().StartsWith("VerbProperties")) {
                            warmup = vp.warmupTime;
                            damage = vp.defaultProjectile.projectile.GetDamageAmount (th);
                            if (vp.burstShotCount > 0) {
                                burstShotCount = vp.burstShotCount;
                            } else {
                                burstShotCount = 1;
                            }
                            if (vp.ticksBetweenBurstShots > 0) {
                                ticksBetweenBurstShots = vp.ticksBetweenBurstShots;
                            } else {
                                ticksBetweenBurstShots = 10;
                            }
                        }
                    }
                } catch (Exception) {}
            }

            float cooldown = th.GetStatValue (StatDefOf.RangedWeapon_Cooldown);
            float burstDamage = damage * burstShotCount;
            float warmupTicks = (cooldown + warmup) * 60/*TPS*/;
            float burstTicks = burstShotCount * ticksBetweenBurstShots;
            float totalTime = (warmupTicks + burstTicks) / 60/*TPS*/;

            return (float)Math.Round (burstDamage / totalTime, 2);
        }
    }

    public class TransferableComparer_NutritionPerMarketValue : TransferableComparer
    {
        public override int Compare(Transferable lhs, Transferable rhs)
        {
            if (!lhs.AnyThing.def.IsNutritionGivingIngestible && !rhs.AnyThing.def.IsNutritionGivingIngestible)
                return 0;
            if (lhs.AnyThing.def.IsNutritionGivingIngestible && !rhs.AnyThing.def.IsNutritionGivingIngestible)
                return -1;
            if (!lhs.AnyThing.def.IsNutritionGivingIngestible && rhs.AnyThing.def.IsNutritionGivingIngestible)
                return 1;
            return -GetNutritionPerMarketValue(lhs.AnyThing).CompareTo(GetNutritionPerMarketValue(rhs.AnyThing));
        }

        private static float GetNutritionPerMarketValue(Thing th)
        {
            return th.GetStatValue(StatDefOf.Nutrition) / th.MarketValue;
        }
    }

    public class TransferableComparer_SellCount : TransferableComparer
    {
        public override int Compare(Transferable lhs, Transferable rhs)
        {
            return lhs.GetMinimumToTransfer().CompareTo(rhs.GetMinimumToTransfer()); // Descending
        }
    }

    public class TransferableComparer_BuyCount : TransferableComparer
    {
        public override int Compare(Transferable lhs, Transferable rhs)
        {
            return -lhs.GetMaximumToTransfer().CompareTo(rhs.GetMaximumToTransfer()); // Descending
        }
    }

    [HotSwappable]
    public class TransferableComparer_SellMarketValueAll : TransferableComparer
    {
        public override int Compare(Transferable lhs, Transferable rhs)
        {
            bool isFormCaravan /* form caravan only? */ = lhs is TransferableOneWay && rhs is TransferableOneWay;

            Func <Transferable, float> valueGetter = isFormCaravan
                ? (Transferable tr) => -tr.GetMaximumToTransfer() * tr.AnyThing.MarketValue
                : (Transferable tr) => tr.GetMinimumToTransfer() * tr.AnyThing.MarketValue;

            return valueGetter(lhs).CompareTo(valueGetter(rhs)); // Descending
        }
    }
}