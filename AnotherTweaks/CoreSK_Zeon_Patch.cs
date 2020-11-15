using System;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace AnotherTweaks
{
    public static class CoreSK_Zeon_Patch
    {
        static MethodBase TargetMethod() => AccessTools.Method("SK.Events.Zeon:SpawnSetup");

        static FieldInfo ioncannonenabled = Type.GetType("SK.Events.Zeon, Core_SK").GetField("ioncannonenabled");
        
        private static void Postfix(object __instance)
        {
            //if (!Settings.ZeonIonCannonEnabled)
            {
                ioncannonenabled.SetValue(__instance, false);
                Log.Warning($"[Zeon spawned] ioncannonenabled = false");
            }
        }

        //private static IEnumerable<CodeInstruction> SpawnSetupTranspiler(IEnumerable<CodeInstruction> code)
        //{
        //    //35	006A	ldarg.0 - this
        //    //36	006B	ldc.i4.1 - true
        //    //37	006C	stfld	bool SK.Events.Zeon::ioncannonenabled

        //    List<CodeInstruction> codes = code.ToList();
        //    for (int i = 1; i < codes.Count; i++)
        //    {
        //        CodeInstruction instruction = codes[i];
        //        if (instruction.opcode == OpCodes.Ldc_I4_1 && codes[i + 1].opcode == OpCodes.Stfld && codes[i + 1].operand == ioncannonenabled)
        //        {
        //            instruction = new CodeInstruction(OpCodes.Ldc_I4_0);
        //        }

        //        yield return instruction;
        //    }
        //}
    }
}