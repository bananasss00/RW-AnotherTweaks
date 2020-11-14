using Verse;

namespace AnotherTweaks
{
    public class ModActive
    {
        private static bool? _tdEnhancmentPack, _mehniMiscModifications;

        public static bool TDEnhancmentPack
        {
            get
            {
                if (_tdEnhancmentPack == null)
                    _tdEnhancmentPack = LoadedModManager.RunningModsListForReading.Any(x => x.Name.Equals("TD Enhancement Pack") || x.Name.Equals("TD Enhancement Pack - Dev Build"));
                return (bool) _tdEnhancmentPack;
            }
        }

        public static bool MehniMiscModifications
        {
            get
            {
                if (_mehniMiscModifications == null)
                    _mehniMiscModifications = LoadedModManager.RunningModsListForReading.Any(x => x.Name.Equals("4M Mehni's Misc Modifications"));
                return (bool) _mehniMiscModifications;
            }
        }
    }
}