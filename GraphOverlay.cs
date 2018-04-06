using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public class GraphOverlay
    {
        private List<Color> _barColorGradients = new List<Color>();

        public GraphOverlay()
        {
            var maxPossibleLevel = 0;
            foreach (var statDef in DefDatabase<RoomStatDef>.AllDefsListForReading)
            {
                if (statDef.isHidden)
                    continue;

                if (statDef.scoreStages.Count > maxPossibleLevel)
                    maxPossibleLevel = statDef.scoreStages.Count;
            }

            var delta = 1f / maxPossibleLevel;
            var color = Color.red;
            for (var i = 0; i < maxPossibleLevel; i++)
            {
                _barColorGradients.Add(color);
                color.r -= delta;
                color.g += delta;
            }
        }

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
                    var barColorIndexScaler = _barColorGradients.Count / (float) infoStat.MaxLevel;
                    var barColorIndex = (int) (infoStat.CurrentLevel * barColorIndexScaler);
                    if (barColorIndex >= _barColorGradients.Count)
                        barColorIndex = _barColorGradients.Count - 1;

                    var barColor = _barColorGradients[barColorIndex];

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