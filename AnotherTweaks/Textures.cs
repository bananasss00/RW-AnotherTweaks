using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    [StaticConstructorOnStartup]
    public static class Textures
    {
        public static readonly Texture2D Arrow = ContentFinder<Texture2D>.Get("UI/Overlays/Arrow", true);
        public static readonly Texture2D BondIcon = ContentFinder<Texture2D>.Get("UI/Icons/Animal/Bond");
        public static readonly Texture2D BondBrokenIcon = ContentFinder<Texture2D>.Get("UI/Icons/Animal/BondBroken");
        public static Texture2D GoingArrow = ContentFinder<Texture2D>.Get("ResearchingArrowAT", true);
    }
}