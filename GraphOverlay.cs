using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public class GraphOverlay
    {
        private readonly Dictionary<RoomStatDef, Texture2D> _statToIconMap = 
            new Dictionary<RoomStatDef, Texture2D>();

        public GraphOverlay()
        {
            _statToIconMap[RoomStatDefOf.Impressiveness] = Resources.IconImpressiveness;
            _statToIconMap[RoomStatDefOf.Wealth] = Resources.IconWealth;
            _statToIconMap[RoomStatDefOf.Space] = Resources.IconSpace;
            _statToIconMap[RoomStatDefOf.Beauty] = Resources.IconBeauty;
            _statToIconMap[RoomStatDefOf.Cleanliness] = Resources.IconCleanliness;
        }

        public void OnGUI(InfoCollector infoCollector, float opacity)
        {
            if (!infoCollector.IsValid())
                return;

            var map = Find.VisibleMap;

            var barLength = 10f;
            var barHeight = 8f;
            var iconSize = barHeight;
            var margin = 4f;

            CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;

            foreach (var roomInfo in infoCollector.RelevantRooms.Values)
            {
                if (!currentViewRect.Contains(roomInfo.PanelCellTopLeft))
                    continue;

                if (map.fogGrid.IsFogged(roomInfo.PanelCellTopLeft))
                    continue;

                var panelLength = barLength * roomInfo.MaxStatSize + margin * 3 + iconSize;
                var panelHeight = barHeight * roomInfo.Stats.Count + margin * (roomInfo.Stats.Count + 1);

                var panelSize = new Vector2(panelLength, panelHeight);

                var drawTopLeft = GenMapUI.LabelDrawPosFor(roomInfo.PanelCellTopLeft);
                var panelRect = new Rect(drawTopLeft, panelSize);
                var panelColor = Color.black;
                panelColor.a = opacity;
                Widgets.DrawBoxSolid(panelRect, panelColor);
                Widgets.DrawBox(panelRect);
                Text.Font = GameFont.Small;

                var iconRectLeft = drawTopLeft.x + margin;
                var meterDrawY = drawTopLeft.y + margin;
                foreach (var infoStat in roomInfo.Stats)
                {
                    if (_statToIconMap.TryGetValue(infoStat.StatDef, out Texture2D icon))
                    {
                        var iconRect = new Rect(iconRectLeft, meterDrawY, iconSize, iconSize);
                        GUI.DrawTexture(iconRect, icon);
                    }

                    var currentLevelFraction = (infoStat.CurrentLevel + 1f) / infoStat.MaxLevel;
                    var barColor = Color.green;
                    if (currentLevelFraction < .66f)
                        barColor = Color.yellow;
                    if (currentLevelFraction < .33)
                        barColor = Color.red;

                    barColor.a = opacity;

                    var meterDrawX = drawTopLeft.x + margin * 2 + iconSize;
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