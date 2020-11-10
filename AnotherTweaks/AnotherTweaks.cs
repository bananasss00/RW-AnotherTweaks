using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    [StaticConstructorOnStartup]
    public class AnotherTweaksMod : Mod
    {
        public static AnotherTweaksMod Instance;

        public AnotherTweaksMod(ModContentPack content) : base(content)
        {
            Instance = this;
            var h = new Harmony("pirateby.AnotherTweaks");
            h.PatchAll();

            if (!ModActive.TDEnhancmentPack)
            {
                h.Patch(AccessTools.Method(typeof(SkillUI), nameof(SkillUI.DrawSkill), new Type[] { typeof(SkillRecord), typeof(Rect), typeof(SkillUI.SkillDrawMode), typeof(string) }),
                    transpiler: new HarmonyMethod(typeof(SkillLearningIndicator), nameof(SkillLearningIndicator.Transpiler)));

                h.Patch(AccessTools.Method(typeof(SkillRecord), nameof(SkillRecord.Learn)), postfix: new HarmonyMethod(typeof(Learn_Patch), nameof(Learn_Patch.Postfix)));

                h.Patch(AccessTools.Method(typeof(MainButtonWorker), nameof(MainButtonWorker.DoButton)), postfix: new HarmonyMethod(typeof(ResearchingIndicator), nameof(ResearchingIndicator.Postfix)));
                
                h.Patch(AccessTools.Method(typeof(ResearchManager), nameof(ResearchManager.ResearchPerformed)), postfix: new HarmonyMethod(typeof(ResearchPerformed), nameof(ResearchPerformed.Postfix)));
                Log.Message($"[AnotherTweaks] TDEnhancmentPack tweaks initialized");
            }

            if (!ModActive.MehniMiscModifications)
            {
                h.Patch(AccessTools.Method(typeof(PawnUIOverlay), nameof(PawnUIOverlay.DrawPawnGUIOverlay)),
                    postfix: new HarmonyMethod(typeof(MehniMiscModifications),
                        nameof(MehniMiscModifications.DrawPawnGUIOverlay_Postfix)));
                h.Patch(
                    AccessTools.Method(typeof(Dialog_AssignBuildingOwner),
                        nameof(Dialog_AssignBuildingOwner.DoWindowContents)),
                    transpiler: new HarmonyMethod(typeof(MehniMiscModifications),
                        nameof(MehniMiscModifications.DoWindowContents_Transpiler)));
                Log.Message($"[AnotherTweaks] MehniMiscModifications tweaks initialized");
            }

            Log.Message($"[AnotherTweaks] Initialized");
        }

        public override void DoSettingsWindowContents(Rect rect) => Settings.Get().DoSettingsWindowContents(rect);

        public override string SettingsCategory() => "Another Tweaks";
    }
}
