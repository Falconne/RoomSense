using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RoomSense
{
    [HarmonyPatch(typeof(MapInterface), "MapInterfaceUpdate")]
    public static class MapInterface_Patch
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            if (Find.CurrentMap == null || WorldRendererUtility.WorldRenderedNow)
            {
                return;
            }

            Main.Instance.UpdateOverlays();
        }
    }
}