using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public static class GraphOverlay
    {
        public static void OnGUI(InfoCollector infoCollector)
        {
            var map = Find.VisibleMap;
            foreach (var roomInfo in infoCollector.RelevantRooms.Values)
            {
                if (map.fogGrid.IsFogged(roomInfo.PanelCellTopLeft))
                    continue;

                var drawTopLeft = GenMapUI.LabelDrawPosFor(roomInfo.PanelCellTopLeft);
                GenMapUI.DrawThingLabel(drawTopLeft, "X", Color.blue);
            }
        }
    }
}