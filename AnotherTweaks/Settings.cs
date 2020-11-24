using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    public class Settings : ModSettings
    {
        private static Settings _settings;

        public static Settings Get()
        {
            if (_settings == null)
                _settings = AnotherTweaksMod.Instance.GetSettings<Settings>();
            return _settings;
        }

	    public bool HiveAttackTarget = false;
	    public bool CutPlantsBeforeBuilding = true;
	    public bool CoreSK_ShowTPSInRegularGame = false;
	    public bool CoreSK_ShowRaidPoints = true;
	    public bool ShareTheLoad_DeliverAsMuchAsYouCan = false;
	    public bool ReplaceStuff_CornerBuildable = false;
	    public bool BetterHostileReadouts = true;
	    public bool SkillArrows = true;
	    public bool ResearchingArrow = true;

        public void DoSettingsWindowContents(Rect rect)
		{
			Listing_Standard modOptions = new Listing_Standard();

			modOptions.Begin(rect);
			modOptions.Gap(20f);

            var defColor = GUI.color;
            
            if (ModActive.CoreSK)
            {
                GUI.color = Color.yellow;
                modOptions.Label("   CoreSK");
                GUI.color = defColor;
                modOptions.CheckboxLabeled("CoreSK_ShowTPSInRegularGame".Translate(), ref CoreSK_ShowTPSInRegularGame);
                modOptions.CheckboxLabeled("CoreSK_ShowRaidPoints".Translate(), ref CoreSK_ShowRaidPoints);
            }

            GUI.color = Color.yellow;
            modOptions.Label("   Vanilla tweaks");
            GUI.color = defColor;

            modOptions.CheckboxLabeled("HiveAttackTarget".Translate(), ref HiveAttackTarget);
            modOptions.CheckboxLabeled("CutPlantsBeforeBuilding".Translate(), ref CutPlantsBeforeBuilding);
            modOptions.Label("DropOneWithControl".Translate());

            if (!ModActive.ShareTheLoad)
            {
                //GUI.color = Color.yellow;
                //modOptions.Label("   ShareTheLoad");
                //GUI.color = defColor;
                modOptions.CheckboxLabeled("ShareTheLoad_DeliverAsMuchAsYouCan".Translate(), ref ShareTheLoad_DeliverAsMuchAsYouCan);
            }

            if (!ModActive.ReplaceStuff)
            {
                //GUI.color = Color.yellow;
                //modOptions.Label("   ReplaceStuff");
                //GUI.color = defColor;
                modOptions.CheckboxLabeled("ReplaceStuff_CornerBuildable".Translate(), ref ReplaceStuff_CornerBuildable);
            }

            if (!ModActive.MehniMiscModifications)
            {
                //GUI.color = Color.yellow;
                //modOptions.Label("   MehniMiscModifications tweaks");
                //GUI.color = defColor;
                modOptions.CheckboxLabeled("BetterHostileReadouts".Translate(), ref BetterHostileReadouts);
            }

            if (!ModActive.TDEnhancmentPack)
            {
                //GUI.color = Color.yellow;
                //modOptions.Label("   TDEnhancmentPack tweaks");
                //GUI.color = defColor;
                modOptions.CheckboxLabeled("SkillArrows".Translate(), ref SkillArrows);
                modOptions.CheckboxLabeled("ResearchingArrow".Translate(), ref ResearchingArrow);
            }

            modOptions.GapLine();

            GUI.color = Color.cyan;
            modOptions.Label("AT_Credits".Translate());
            GUI.color = defColor;

            modOptions.End();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref HiveAttackTarget, "HiveAttackTarget", false);
			Scribe_Values.Look(ref CutPlantsBeforeBuilding, "CutPlantsBeforeBuilding", true);
			Scribe_Values.Look(ref CoreSK_ShowTPSInRegularGame, "CoreSK_ShowTPSInRegularGame", false);
			Scribe_Values.Look(ref CoreSK_ShowRaidPoints, "CoreSK_ShowRaidPoints", true);
			Scribe_Values.Look(ref ShareTheLoad_DeliverAsMuchAsYouCan, "ShareTheLoad_DeliverAsMuchAsYouCan", false);
			Scribe_Values.Look(ref ReplaceStuff_CornerBuildable, "ReplaceStuff_CornerBuildable", true);
			Scribe_Values.Look(ref BetterHostileReadouts, "BetterHostileReadouts", true);
			Scribe_Values.Look(ref SkillArrows, "SkillArrows", true);
			Scribe_Values.Look(ref ResearchingArrow, "ResearchingArrow", true);
        }
	}
}