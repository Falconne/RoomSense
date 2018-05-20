using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RoomSense
{
    class Label
    {
        public Mesh LabelMesh;
        public Vector3 Position;
    }

    public class LabelPlacementHandler
    {
        private readonly FontHandler _fontHandler = new FontHandler();

        private readonly List<Label> _currentLabels = new List<Label>();

        private int _nextUpdateTick;

        public bool IsReady()
        {
            return _fontHandler.IsFontLoaded();
        }

        public void Regenerate()
        {
            _currentLabels.Clear();
            var foundRooms = new HashSet<Room>();

            var map = Find.VisibleMap;
            var listerBuildings = map.listerBuildings;
            // Room roles are defined by buildings, so only need to check rooms with buildings
            foreach (var building in listerBuildings.allBuildingsColonist)
            {
                var room = InfoCollector.GetRoomContainingBuildingIfRelevant(building, map);
                if (room == null)
                    continue;

                if (foundRooms.Contains(room))
                    continue;

                foundRooms.Add(room);
                var labelPosForRoom = InfoCollector.GetPanelTopLeftCornerForRoom(room, map);
            }
        }

        public void Draw()
        {
            var tick = Find.TickManager.TicksGame;
            if (tick < _nextUpdateTick)
                return;

            _nextUpdateTick = tick + 200;

        }
    }
}