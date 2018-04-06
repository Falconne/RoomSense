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
            var cellSize = Find.CameraDriver.CellSizePixels;
            var panelLength = cellSize * 3;
            var panelHeight = cellSize * 2;
            var margin = cellSize * .2f;
            var iconWidth = cellSize * .25f;

            var panelSize = new Vector2(panelLength, panelHeight);
            var barHeight = (panelHeight - margin * (1 + infoCollector.MaxStatCount)) / infoCollector.MaxStatCount;
            var barLength = (panelLength - margin * 3 - iconWidth) / infoCollector.MaxStatSize;

            foreach (var roomInfo in infoCollector.RelevantRooms.Values)
            {
                if (map.fogGrid.IsFogged(roomInfo.PanelCellTopLeft))
                    continue;

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