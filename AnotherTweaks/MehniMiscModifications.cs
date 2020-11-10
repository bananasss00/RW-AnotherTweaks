using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AnotherTweaks
{
    public class MehniMiscModifications
    {
        #region BetterHostileReadouts
        /// <summary>
        /// Show label names on hostile animals. Credits: mehni. 4misc mods
        /// </summary>
        /// <param name="___pawn"></param>
        public static void DrawPawnGUIOverlay_Postfix(Pawn ___pawn)
        {
            // First two checks are just to prevent duplicates
            if (Settings.Get().BetterHostileReadouts && !___pawn.RaceProps.Humanlike && ___pawn.Faction != Faction.OfPlayer && ___pawn.HostileTo(Faction.OfPlayer))
                GenMapUI.DrawPawnLabel(___pawn, GenMapUI.LabelDrawPosFor(___pawn, -0.6f), font: GameFont.Tiny);
        }
        #endregion BetterHostileReadouts

        #region showLovers
        public static IEnumerable<CodeInstruction> DoWindowContents_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            MethodInfo getViewRectWidth = AccessTools.Property(typeof(Rect), nameof(Rect.width)).GetGetMethod();
            MethodInfo getPawnName = AccessTools.Property(typeof(Entity), nameof(Entity.LabelCap)).GetGetMethod();
            MethodInfo makeReeect = AccessTools.Method(typeof(MehniMiscModifications), nameof(MehniMiscModifications.ShowHeart));

            List<CodeInstruction> instructionList = codeInstructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
            {
                if (i > 2 && instructionList[i - 1].Calls(getPawnName))
                {
                    yield return instructionList[i];
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
                    yield return new CodeInstruction(OpCodes.Call, getViewRectWidth);
                    yield return new CodeInstruction(OpCodes.Ldc_R4, offsetPosition);
                    yield return new CodeInstruction(OpCodes.Mul); //viewrect.width * 0.55
                    yield return new CodeInstruction(OpCodes.Ldloc_2); //y
                    yield return new CodeInstruction(instructionList[i - 2]); //pawn
                    yield return new CodeInstruction(OpCodes.Call, makeReeect);
                    yield return new CodeInstruction(OpCodes.Brtrue, instructionList.Where(x => x != null && x.labels != null && x.labels.Any()).Skip(instructionList.Where(x => x != null && x.labels != null && x.labels.Any()).Count() - 2).First().labels.First()); //2nd to last ret
                }
                else
                    yield return instructionList[i];
            }
        }

        private static float resizeHeart = 0.50f;
        private static float offsetPosition = 0.62f;

        private static bool ShowHeart(float x, float y, Pawn pawn)
        {
            Texture2D iconFor;
            if (pawn == null || !pawn.IsColonist)
                return false;

            DirectPawnRelation directPawnRelation = LovePartnerRelationUtility.ExistingMostLikedLovePartnerRel(pawn, false);
            if (directPawnRelation == null || directPawnRelation.otherPawn == null)
                iconFor = null;

            else if (!directPawnRelation.otherPawn.IsColonist || directPawnRelation.otherPawn.IsWorldPawn() || !directPawnRelation.otherPawn.relations.everSeenByPlayer)
                iconFor = null;

            else if (pawn.ownership?.OwnedBed != null && pawn.ownership?.OwnedBed == directPawnRelation.otherPawn.ownership?.OwnedBed)
                iconFor = Textures.BondIcon;

            else
                iconFor = Textures.BondBrokenIcon;

            if (iconFor != null)
            {
                Vector2 iconSize = new Vector2(iconFor.width, iconFor.height) * resizeHeart;
                Rect drawRect = new Rect(x, y, iconSize.x, iconSize.y);
                TooltipHandler.TipRegion(drawRect, directPawnRelation.otherPawn.LabelCap);
                GUI.DrawTexture(drawRect, iconFor);

                if (iconFor == Textures.BondBrokenIcon && Mouse.IsOver(drawRect) && Input.GetMouseButtonDown(0))
                {
                    if (pawn.ownership?.OwnedBed?.SleepingSlotsCount >= 2)
                    {
                        pawn.ownership.OwnedBed.GetComp<CompAssignableToPawn>().TryAssignPawn(directPawnRelation.otherPawn);
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion showLovers
    }
}