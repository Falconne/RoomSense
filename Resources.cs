using UnityEngine;
using Verse;

namespace RoomSense
{
    [StaticConstructorOnStartup]
    public class Resources
    {
        public static Texture2D IconImpressiveness = ContentFinder<Texture2D>.Get("Impressiveness");
        public static Texture2D IconWealth = ContentFinder<Texture2D>.Get("Wealth");
        public static Texture2D IconSpace = ContentFinder<Texture2D>.Get("Space");
        public static Texture2D IconBeauty = ContentFinder<Texture2D>.Get("Beauty");
        public static Texture2D IconCleanliness = ContentFinder<Texture2D>.Get("Cleanliness");
    }
}