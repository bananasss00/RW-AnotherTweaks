using Verse;

namespace AnotherTweaks
{
    public class ModActive
    {
        private static bool? _tdEnhancmentPack, _mehniMiscModifications, _coreSk, _shareTheLoad, _replaceStuff;

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

        public static bool CoreSK
        {
            get
            {
                if (_coreSk == null)
                    _coreSk = LoadedModManager.RunningModsListForReading.Any(x => x.Name.Equals("Core SK"));
                return (bool) _coreSk;
            }
        }

        public static bool ShareTheLoad
        {
            get
            {
                if (_shareTheLoad == null)
                    _shareTheLoad = LoadedModManager.RunningModsListForReading.Any(x => x.Name.Equals("Share The Load") || x.Name.Equals("Share The Load - Dev Build"));
                return (bool) _shareTheLoad;
            }
        }

        public static bool ReplaceStuff
        {
            get
            {
                if (_replaceStuff == null)
                    _replaceStuff = LoadedModManager.RunningModsListForReading.Any(x => x.Name.Equals("Replace Stuff") || x.Name.Equals("Replace Stuff - Dev Build"));
                return (bool) _replaceStuff;
            }
        }
    }
}