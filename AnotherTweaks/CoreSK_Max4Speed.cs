using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnotherTweaks
{
    class CoreSK_Max4Speed
	{
        public static void Postfix(ref TimeSpeed currTimeSpeed, ref float __result)
        {
            if (__result == 15f) // its 4x speed
            {
                int speed = Settings.Get().CoreSK_Max4Speed;
                if (speed == 900 || speed <= 0) // default 4x max
                    return;
                __result = speed / 60f;
            }
        }
	}
}
