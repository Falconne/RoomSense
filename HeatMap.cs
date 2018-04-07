using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public class HeatMap : ICellBoolGiver
    {
        private readonly List<Color> _stageIndexToColorMap = new List<Color>();

        private readonly InfoCollector _infoCollector;

        private CellBoolDrawer _drawerInt;

        private Color _nextColor;

        public HeatMap(InfoCollector infoCollector)
        {
            _infoCollector = infoCollector;

            var maxPossibleLevel = 0;
            foreach (var statDef in DefDatabase<RoomStatDef>.AllDefsListForReading)
            {
                if (statDef.isHidden)
                    continue;

                if (statDef.scoreStages.Count > maxPossibleLevel)
                    maxPossibleLevel = statDef.scoreStages.Count;
            }

            // HSV gradient
            // HSV hue 0 dgrees to 120 degrees goes from red to green.
            // Going a little past 120 degrees at high end to get more greens.
            var delta = .38f / maxPossibleLevel;
            var hue = 0f;
            for (var i = 0; i < maxPossibleLevel; i++)
            {
                var color = Color.HSVToRGB(hue, 1f, 1f);
                _stageIndexToColorMap.Add(color);
                hue += delta;
            }

            /*
            // Linear RGB gradient
            var delta = 1f / maxPossibleLevel;
            var color = Color.red;
            for (var i = 0; i < maxPossibleLevel; i++)
            {
                _stageIndexToColorMap.Add(color);
                color.r -= delta;
                color.g += delta;
            }
            */
        }

        public CellBoolDrawer Drawer
        {
            get
            {
                if (_drawerInt == null)
                {
                    var map = Find.VisibleMap;
                    _drawerInt = new CellBoolDrawer(this, map.Size.x, map.Size.z,
                        Main.Instance.GetHeatMapOpacity());
                }
                return _drawerInt;
            }
        }

        public bool GetCellBool(int index)
        {
            var map = Find.VisibleMap;
            if (map.fogGrid.IsFogged(index))
                return false;

            var room = map.cellIndices.IndexToCell(index).GetRoom(
                map, RegionType.Set_All);

            if (room == null)
                return false;

            if (!_infoCollector.RelevantRooms.TryGetValue(room, out RoomInfo roomInfo))
                return false;

            var primaryStat = roomInfo.GetPrimaryStat();
            if (primaryStat == null)
                return false;

            var stageColorIndexScaler = _stageIndexToColorMap.Count / (float)primaryStat.MaxLevel;
            var stageColorIndex = (int)(primaryStat.CurrentLevel * stageColorIndexScaler);
            if (stageColorIndex >= _stageIndexToColorMap.Count)
                stageColorIndex = _stageIndexToColorMap.Count - 1;

            _nextColor = _stageIndexToColorMap[stageColorIndex];

            return true;
        }

        public Color GetCellExtraColor(int index)
        {
            return _nextColor;
        }

        public Color Color => Color.white;

        public void Update()
        {
            Drawer.MarkForDraw();
            if (_infoCollector.IsTimeToUpdateHeatMap())
            {
                Drawer.SetDirty();
            }
            Drawer.CellBoolDrawerUpdate();
        }

        public void Reset()
        {
            _drawerInt = null;
        }

    }
}