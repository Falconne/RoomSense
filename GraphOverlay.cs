using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public class GraphOverlay
    {
        public void OnGUI(InfoCollector infoCollector)
        {
            if (!infoCollector.IsValid())
                return;

            var map = Find.VisibleMap;

            var barLength = 10f;
            var barHeight = 8f;
            var iconWidth = barHeight;
            var margin = 4f;

            CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;

            foreach (var roomInfo in infoCollector.RelevantRooms.Values)
            {
                if (!currentViewRect.Contains(roomInfo.PanelCellTopLeft))
                    continue;

                if (map.fogGrid.IsFogged(roomInfo.PanelCellTopLeft))
                    continue;

                var panelLength = barLength * roomInfo.MaxStatSize + margin * 3 + iconWidth;
                var panelHeight = barHeight * roomInfo.Stats.Count + margin * (roomInfo.Stats.Count + 1);

                var panelSize = new Vector2(panelLength, panelHeight);

                var drawTopLeft = GenMapUI.LabelDrawPosFor(roomInfo.PanelCellTopLeft);
                var panelRect = new Rect(drawTopLeft, panelSize);
                Widgets.DrawBoxSolid(panelRect, Color.black);
                Widgets.DrawBox(panelRect);


                var meterDrawY = drawTopLeft.y + margin;
                foreach (var infoStat in roomInfo.Stats)
                {
                    var currentLevelFraction = (infoStat.CurrentLevel + 1f) / infoStat.MaxLevel;
                    var barColor = Color.green;
                    if (currentLevelFraction < .66f)
                        barColor = Color.yellow;
                    if (currentLevelFraction < .33)
                        barColor = Color.red;

                    var meterDrawX = drawTopLeft.x + margin * 2 + iconWidth;
                    for (var i = 0; i < infoStat.MaxLevel; i++, meterDrawX += barLength)
                    {
                        var barRect = new Rect(meterDrawX, meterDrawY, barLength, barHeight);
                        var color = (i <= infoStat.CurrentLevel) ? barColor : Color.clear;
                        Widgets.DrawBoxSolid(barRect, color);
                        Widgets.DrawBox(barRect);
                    }

                    meterDrawY += barHeight + margin;
                }
            }
        }
    }
}