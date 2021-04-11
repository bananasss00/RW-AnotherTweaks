using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using HugsLib.Settings;
using Verse;

namespace AnotherTweaks
{
    [HarmonyPatch(typeof(Dialog_DebugOptionListLister), nameof(Dialog_DebugOptionListLister.DoListingItems))]
    public class Dialog_DebugOptionListLister_Patch
    {
        private static List<DebugMenuOption> GetList(Dialog_DebugOptionListLister __instance)
        {
            return __instance.options.Where(x => __instance.FilterAllows(x.label)).ToList();
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var t = typeof(Dialog_DebugOptionListLister_Patch);
            var options = AccessTools.Field(typeof(Dialog_DebugOptionListLister), nameof(Dialog_DebugOptionListLister.options));
            var code = instructions.ToList();
            var myList = ilGen.DeclareLocal(typeof(List<Dialog_DebugActionsMenu.DebugActionOption>));
            
            // get filtered list in method start
            yield return new CodeInstruction(OpCodes.Ldarg_0); // this
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(t, nameof(GetList)));
            yield return new CodeInstruction(OpCodes.Stloc, myList);

            // replacements
            foreach (var ci in code)
            {
                if (ci.opcode == OpCodes.Ldfld && ci.operand == options)
                {
                    // remove pushed Ldarg_0 above from stack
                    yield return new CodeInstruction(OpCodes.Pop);
                    // use instead options my from local var
                    yield return new CodeInstruction(OpCodes.Ldloc, myList);
                }
                // return original instruction
                else yield return ci;
            }
        }
    }

    [HarmonyPatch(typeof(Dialog_DebugActionsMenu), nameof(Dialog_DebugActionsMenu.DoListingItems))]
    public class Dialog_DebugActionsMenu_Patch
    {
        private static List<Dialog_DebugActionsMenu.DebugActionOption> GetList(Dialog_DebugActionsMenu __instance)
        {
            return __instance.debugActions.Where(x => __instance.FilterAllows(x.label)).ToList();
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var t = typeof(Dialog_DebugActionsMenu_Patch);
            var debugActions = AccessTools.Field(typeof(Dialog_DebugActionsMenu), nameof(Dialog_DebugActionsMenu.debugActions));
            var code = instructions.ToList();
            var myList = ilGen.DeclareLocal(typeof(List<Dialog_DebugActionsMenu.DebugActionOption>));
            
            // get filtered list in method start
            yield return new CodeInstruction(OpCodes.Ldarg_0); // this
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(t, nameof(GetList)));
            yield return new CodeInstruction(OpCodes.Stloc, myList);

            // replacements
            foreach (var ci in code)
            {
                if (ci.opcode == OpCodes.Ldfld && ci.operand == debugActions)
                {
                    // remove pushed Ldarg_0 above from stack
                    yield return new CodeInstruction(OpCodes.Pop);
                    // use instead debugActions my from local var
                    yield return new CodeInstruction(OpCodes.Ldloc, myList);
                }
                // return original instruction
                else yield return ci;
            }
        }
    }

    [HarmonyPatch(typeof(Dialog_DebugOutputMenu), nameof(Dialog_DebugOutputMenu.DoListingItems))]
    public class Dialog_DebugOutputMenu_Patch
    {
        private static List<Dialog_DebugOutputMenu.DebugOutputOption> GetList(Dialog_DebugOutputMenu __instance)
        {
            return __instance.debugOutputs.Where(x => __instance.FilterAllows(x.label)).ToList();
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var t = typeof(Dialog_DebugOutputMenu_Patch);
            var debugOutputs = AccessTools.Field(typeof(Dialog_DebugOutputMenu), nameof(Dialog_DebugOutputMenu.debugOutputs));
            var code = instructions.ToList();
            var myList = ilGen.DeclareLocal(typeof(List<Dialog_DebugOutputMenu.DebugOutputOption>));
            
            // get filtered list in method start
            yield return new CodeInstruction(OpCodes.Ldarg_0); // this
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(t, nameof(GetList)));
            yield return new CodeInstruction(OpCodes.Stloc, myList);

            // replacements
            foreach (var ci in code)
            {
                if (ci.opcode == OpCodes.Ldfld && ci.operand == debugOutputs)
                {
                    // remove pushed Ldarg_0 above from stack
                    yield return new CodeInstruction(OpCodes.Pop);
                    // use instead debugActions my from local var
                    yield return new CodeInstruction(OpCodes.Ldloc, myList);
                }
                // return original instruction
                else yield return ci;
            }
        }
    }

}