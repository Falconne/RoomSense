using Harmony;
using RimWorld;
using Verse;

namespace RoomSense
{
    [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
    public static class PlaySettings_Patch
    {
        static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView)
                return;

            row?.ToggleableIcon(ref Main.Instance.ShowGraphOverlay, Resources.GraphToggle,
                "FALCRS.GraphToggle".Translate(), SoundDefOf.MouseoverToggle);
        }
    }
}