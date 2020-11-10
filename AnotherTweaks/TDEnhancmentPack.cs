using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    #region SkillArrows
	public static class SkillLearningIndicator
	{
		//public static void DrawSkill(SkillRecord skill, Rect holdingRect, SkillUI.SkillDrawMode mode, string tooltipPrefix = "")
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			MethodInfo EndGroupInfo = AccessTools.Method(typeof(GUI), "EndGroup");

			MethodInfo LabelLearningInfo = AccessTools.Method(typeof(SkillLearningIndicator), nameof(LabelLearning));

			foreach (CodeInstruction i in instructions)
			{
				if(i.Calls(EndGroupInfo))
				{
					yield return new CodeInstruction(OpCodes.Ldarg_0);//SkillRecord
					yield return new CodeInstruction(OpCodes.Ldarg_1);//holdingRect
					yield return new CodeInstruction(OpCodes.Call, LabelLearningInfo);
				}
				yield return i;
			}
		}

		public static void LabelLearning(SkillRecord skillRecord, Rect holdingRect)
		{
			if (!Settings.Get().SkillArrows) return;

			List<LearnedInfo> rec = Current.Game.GetComponent<LearnedGameComponent>().learnedInfo;
			if (rec.FirstOrDefault(i => i.record == skillRecord) is LearnedInfo info)
			{
				float skillGain = info.xp;
				if (skillGain == 0) return;
				if (skillGain > 0)
				{
					//if (!Settings.Get().skillUpArrows) return;
					skillGain *= 5;
				}
				else
				{
					//if (!Settings.Get().skillDownArrows) return;
					skillGain /= 10;
				}

				Color oldColor = GUI.color;

				Color arrowColor = skillGain > 0 ? Color.green : Color.red;
				arrowColor.a = Mathf.Clamp01(Math.Abs(skillGain));
				GUI.color = arrowColor;

				Rect iconRect = new Rect(Vector2.zero, Vector2.one * holdingRect.height);
				iconRect.x += (float)AccessTools.Field(typeof(SkillUI), "levelLabelWidth").GetValue(null);
				//Hack in the end result of LeftEdgeMargin + SkillHeight + 4 + skill # width ish
				iconRect.x += 6 + 24 + 4 + 36;

				Widgets.DrawTextureFitted(iconRect, Textures.Arrow, 1, new Vector2((float)Textures.Arrow.width, (float)Textures.Arrow.height), new Rect(0f, 0f, 1f, 1f), skillGain > 0 ? 0 : 180);

				GUI.color = oldColor;
			}
		}
	}
    
	public class LearnedInfo
	{
		public SkillRecord record;
		public float xp;
		public int tickToKill;

		public LearnedInfo(SkillRecord r, float x, int f)
		{
			record = r;
			xp = x;
			tickToKill = f;
		}

		public override string ToString()
		{
			return String.Format("LInfo: {0}@{1}:{2}", record.ToString(), xp, tickToKill);
		}
	}

	public class LearnedGameComponent : GameComponent
	{
		public List<LearnedInfo> learnedInfo = new List<LearnedInfo>();

		public LearnedGameComponent(Game game) { }

		public override void GameComponentTick()
		{
			base.GameComponentTick();
			if (!Settings.Get().SkillArrows) return;

			learnedInfo.RemoveAll(i => i.tickToKill <= GenTicks.TicksGame);
		}
	}

	public static class Learn_Patch
	{
		//SkillRecord public void Learn(float xp, bool direct = false)
		public static void Postfix(SkillRecord __instance, float xp)
		{
			if (!Settings.Get().SkillArrows) return;

			List<LearnedInfo> rec = Current.Game.GetComponent<LearnedGameComponent>().learnedInfo;

			int killAt = (GenTicks.TicksGame + 200);// loss ticks every 200, so this is fine
			if (rec.FirstOrDefault(i => i.record == __instance) is LearnedInfo info)
			{
				info.tickToKill = killAt;
				info.xp = xp;
			}
			else
				rec.Add(new LearnedInfo(__instance, xp, killAt));
		}
	}
    #endregion SkillArrows

    #region ResearchArrow
    public static class ResearchingIndicator
    {
        public static float amount;
        public static int showUntilTick;

        //public virtual void DoButton(Rect rect)
        //
        public static void Postfix(MainButtonWorker __instance, Rect rect)
        {
            if (!(__instance is MainButtonWorker_ToggleResearchTab)) return;

            if (!Settings.Get().ResearchingArrow) return;

            if (GenTicks.TicksGame > showUntilTick) return;

            Rect iconRect = rect.LeftPartPixels(rect.height);//.ContractedBy(1);
            GUI.color = new Color(1, 1, 1, amount);
            Widgets.DrawTextureFitted(iconRect, Textures.GoingArrow, 1.0f);
            GUI.color = Color.white;
        }
    }

    public static class ResearchPerformed
    {
        public static readonly float maxAmount = 0.015f;//I don't know why 0.015f is about the max amount done
        //public void ResearchPerformed(float amount, Pawn researcher)
        public static void Postfix(float amount)
        {
            if (!Settings.Get().ResearchingArrow) return;
		
            ResearchingIndicator.amount = 0.5f + amount / maxAmount / 2 ;
            ResearchingIndicator.showUntilTick = (GenTicks.TicksGame + 200);
        }
    }
    #endregion ResearchArrow
}