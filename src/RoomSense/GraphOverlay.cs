using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public class GraphOverlay
    {
        private readonly Dictionary<RoomStatDef, Texture2D> _statToIconMap =
            new Dictionary<RoomStatDef, Texture2D>();

        private class InfoRow
        {
            public readonly string[] Columns = new string[3];
        }

        public GraphOverlay()
        {
            _statToIconMap[RoomStatDefOf.Impressiveness] = Resources.Impressiveness;
            _statToIconMap[RoomStatDefOf.Wealth] = Resources.Wealth;
            _statToIconMap[RoomStatDefOf.Space] = Resources.Space;
            _statToIconMap[RoomStatDefOf.Beauty] = Resources.Beauty;
            _statToIconMap[RoomStatDefOf.Cleanliness] = Resources.Cleanliness;
        }

        public void OnGUI(InfoCollector infoCollector, float opacity, bool showRoomRoles)
        {
            if (!infoCollector.IsValid())
                return;

            var map = Find.CurrentMap;

            const float barLength = 10f;
            const float barHeight = 8f;
            const float iconSize = barHeight;
            const float margin = 4f;

            CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;

            foreach (var roomInfo in infoCollector.RelevantRooms.Values)
            {
                if (!currentViewRect.Contains(roomInfo.PanelCellTopLeft))
                    continue;

                if (map.fogGrid.IsFogged(roomInfo.PanelCellTopLeft))
                    continue;


                var panelLength = barLength * roomInfo.MaxStatSize + margin * 3 + iconSize;
                var panelHeight = barHeight * roomInfo.Stats.Count + margin * (roomInfo.Stats.Count + 1);
                
                var roleLabel = roomInfo.RoomName;

                var roleLabelHeight = 0f;
                if (showRoomRoles)
                {
                    Text.Font = GameFont.Tiny;
                    roleLabelHeight = Text.CalcSize(roleLabel).y;
                    panelHeight += roleLabelHeight + margin;
                }

                var panelSize = new Vector2(panelLength, panelHeight);

                var drawTopLeft = GenMapUI.LabelDrawPosFor(roomInfo.PanelCellTopLeft);
                var panelRect = new Rect(drawTopLeft, panelSize);
                var panelColor = Color.black;
                panelColor.a = opacity;
                Widgets.DrawBoxSolid(panelRect, panelColor);
                Widgets.DrawBox(panelRect);

                if (showRoomRoles)
                {
                    var roomRoleLabelRect = new Rect(drawTopLeft.x + margin, drawTopLeft.y + margin, 
                        panelSize.x - margin, panelSize.y - margin);
                    Widgets.Label(roomRoleLabelRect, roleLabel);
                }

                var iconRectLeft = drawTopLeft.x + margin;
                var meterDrawY = drawTopLeft.y + margin + roleLabelHeight;
                var tooltipRows = new List<InfoRow>();
                var showTooltip = !Find.PlaySettings.showRoomStats;
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

                    if (showTooltip)
                    {
                        tooltipRows.Add
                        (
                            new InfoRow
                            {
                                Columns =
                                {
                                    [0] = infoStat.StatDef.LabelCap,
                                    [1] = infoStat.RawCurrentLevel,
                                    [2] = infoStat.ValueLabel
                                }
                            }
                         );
                    }
                }

                if (!showTooltip)
                    continue;

                var tooltip = new StringBuilder();
                tooltip.Append(roleLabel);
                tooltip.AppendLine();
                tooltip.AppendLine();
                foreach (var row in tooltipRows)
                {
                    tooltip.Append($"{row.Columns[0]}:  [{row.Columns[1]}]  ({row.Columns[2]})");
                    tooltip.AppendLine();
                    tooltip.AppendLine();
                }

                TooltipHandler.TipRegion(panelRect, tooltip.ToString());
            }
        }
    }
}