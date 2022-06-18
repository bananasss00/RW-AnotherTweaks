using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace AnotherTweaks
{
    public static class CoreSK_TradeTransponder_Patch
    {
        public static IEnumerable<CodeInstruction> IncidentWorker_OrbitalTraderArrivalSBT_TryExecuteWorker(IEnumerable<CodeInstruction> code)
        {
            var settingsGetter = typeof(Settings).GetMethod(nameof(Settings.Get), AccessTools.all);
            var valueField = typeof(Settings).GetField(nameof(Settings.CoreSK_MaxTradeshipsTransponder), AccessTools.all);

            var list = code.ToList();
            int idx = list.FindIndex(ci => ci.opcode == OpCodes.Ldc_I4_5);
            if (idx == -1)
            {
                Log.Error("Transpiler outdated");
                return list;
            }

            list.RemoveAt(idx);
            list.InsertRange(idx, new []
            {
                new CodeInstruction(OpCodes.Call, settingsGetter),
                new CodeInstruction(OpCodes.Ldfld, valueField),
            });

            return list;

        }
    }
}