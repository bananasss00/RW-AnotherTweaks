using Verse;

namespace AnotherTweaks
{
    public class CoreSK_Utils
    {
        public static void Set_Config_MaxRaidCount(int value)
        {
            var config = DefDatabase<SK.Config>.GetNamed("Config");
            if (config != null)
            {
                config.MaxRaidCount = value;
                Log.Message($"[AnotherTweaks] CoreSK config MaxRaidCount = {value}");
            }
        }
    }
}