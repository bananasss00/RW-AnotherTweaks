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
	    public bool BetterHostileReadouts = true;
	    public bool SkillArrows = true;
	    public bool ResearchingArrow = true;

        public void DoSettingsWindowContents(Rect rect)
		{
			Listing_Standard modOptions = new Listing_Standard();

			modOptions.Begin(rect);
			modOptions.Gap(20f);

            var defColor = GUI.color;
            GUI.color = Color.yellow;
            modOptions.Label("   Vanilla tweaks");
            GUI.color = defColor;

		    modOptions.CheckboxLabeled("HiveAttackTarget".Translate(), ref HiveAttackTarget);
            modOptions.Label("DropOneWithControl".Translate());

            if (!ModActive.MehniMiscModifications)
            {
                GUI.color = Color.yellow;
                modOptions.Label("   MehniMiscModifications tweaks");
                GUI.color = defColor;
                modOptions.CheckboxLabeled("BetterHostileReadouts".Translate(), ref BetterHostileReadouts);
            }
            if (!ModActive.TDEnhancmentPack)
            {
                GUI.color = Color.yellow;
                modOptions.Label("   TDEnhancmentPack tweaks");
                GUI.color = defColor;
                modOptions.CheckboxLabeled("SkillArrows".Translate(), ref SkillArrows);
                modOptions.CheckboxLabeled("ResearchingArrow".Translate(), ref ResearchingArrow);
            }


            modOptions.End();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref HiveAttackTarget, "HiveAttackTarget", false);
			Scribe_Values.Look(ref BetterHostileReadouts, "BetterHostileReadouts", true);
			Scribe_Values.Look(ref SkillArrows, "SkillArrows", true);
			Scribe_Values.Look(ref ResearchingArrow, "ResearchingArrow", true);
        }
	}
}