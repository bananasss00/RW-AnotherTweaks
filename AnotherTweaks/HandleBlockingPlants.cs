using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AnotherTweaks
{
    [DefOf]
    public static class WorkTypeDefOf
    {
        static WorkTypeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(WorkTypeDefOf));
        }

        public static WorkTypeDef PlantCutting;
    }

    public static class HandleBlockingPlants
    {
        public static IEnumerable<CodeInstruction> HandleBlockingThingJob(MethodBase __originalMethod, IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            var code = instructions.ToList();
            
            int insertIdx = -1;
            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldc_I4_4 && code[i + 1].opcode == OpCodes.Bne_Un_S) // if (thing.def.category == ThingCategory.Plant)
                    insertIdx = i + 2;
            }

            if (insertIdx == -1)
            {
                Log.Error($"[HandleBlockingPlants] Can't find insertion place");
                return code;
            }

            bool isRoofUtility = __originalMethod.DeclaringType == typeof(RoofUtility);
            var continueLabel = ilGen.DefineLabel();
            code[insertIdx].labels.Add(continueLabel);
            code.InsertRange(insertIdx, new []
            {
                // if (!CanCutBlockingPlant(worker, thing)) return null
                new CodeInstruction(OpCodes.Ldarg_1), 
                isRoofUtility ? new CodeInstruction(OpCodes.Ldarg_0) : new CodeInstruction(OpCodes.Ldloc_0), // RoofUtility blocker from argument, GenConstruct blocker from local var
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(HandleBlockingPlants), nameof(HandleBlockingPlants.CanCutBlockingPlant))), 
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel), 
                new CodeInstruction(OpCodes.Ldnull), 
                new CodeInstruction(OpCodes.Ret)
            });

            return code;
        }

        private static bool CanCutBlockingPlant(Pawn worker, Thing t)
        {
            if (!Settings.Get().CutPlantsBeforeBuilding)
                return true;

            if (worker.workSettings.WorkIsActive(WorkTypeDefOf.PlantCutting))
                return true;

            if (t is Plant p)
            {
                var dm = t.Map.designationManager;
                bool hasDesignation = dm.AllDesignationsOn(p).Any(x => x.def == DesignationDefOf.CutPlant || x.def == DesignationDefOf.HarvestPlant);
                if (!hasDesignation)
                {
                    dm.RemoveAllDesignationsOn(p, false);
                    if (p.HarvestableNow)
                        dm.AddDesignation(new Designation(p, DesignationDefOf.HarvestPlant));
                    else
                        dm.AddDesignation(new Designation(p, DesignationDefOf.CutPlant));
                }
            }

            return false;
        }
    }
}