using UnityEngine;
using Verse;

namespace RoomSense
{
    [StaticConstructorOnStartup]
    public class Resources
    {
        public static Texture2D Impressiveness = ContentFinder<Texture2D>.Get("Impressiveness");
        public static Texture2D Wealth = ContentFinder<Texture2D>.Get("Wealth");
        public static Texture2D Space = ContentFinder<Texture2D>.Get("Space");
        public static Texture2D Beauty = ContentFinder<Texture2D>.Get("Beauty");
        public static Texture2D Cleanliness = ContentFinder<Texture2D>.Get("Cleanliness");
        public static Texture2D GraphToggle = ContentFinder<Texture2D>.Get("GraphToggle");
    }
}