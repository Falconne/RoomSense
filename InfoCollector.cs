using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public struct RoomStat
    {
        public RoomStatDef Def;

    }

    public class RoomInfo
    {
        public IntVec3 PanelCellTopLeft;
        public List<RoomStat> Stats = new List<RoomStat>();
    };

    public class InfoCollector
    {
        private int _nextUpdateTick;

        public Dictionary<Room, RoomInfo> RelevantRooms { get; }

        public InfoCollector()
        {
            RelevantRooms = new Dictionary<Room, RoomInfo>();
        }

        public void Update(int updateDelay)
        {
            var tick = Find.TickManager.TicksGame;
            if (_nextUpdateTick != 0 && tick < _nextUpdateTick)
                return;

            _nextUpdateTick = tick + updateDelay;

            var map = Find.VisibleMap;
            var listerBuildings = map.listerBuildings;
            // Room roles are defined by buildings, so only need to check rooms with buildings
            foreach (var building in listerBuildings.allBuildingsColonist)
            {
                var room = GetRoomContainingBuildingIfRelevant(building, map);
                if (room == null)
                    continue;

                var roomInfo = new RoomInfo
                {
                    PanelCellTopLeft = GetPanelTopLeftCornerForRoom(room)
                };

                RelevantRooms[room] = roomInfo;
            }
        }

        public void Reset()
        {
            _nextUpdateTick = 0;
            RelevantRooms.Clear();
        }

        private IntVec3 GetPanelTopLeftCornerForRoom(Room room)
        {
            var bestCell = room.BorderCells.First();
            foreach (var cell in room.BorderCells)
            {
                if (cell.x < bestCell.x || cell.z < bestCell.z)
                    bestCell = cell;
            }

            return bestCell;
        }

        // Filter for indoor rooms with a role
        private static Room GetRoomContainingBuildingIfRelevant(Building building, Map map)
        {
            if (building.Faction != Faction.OfPlayer)
                return null;

            if (building.Position.Fogged(map))
                return null;

            var room = building.Position.GetRoom(map);
            if (room == null || room.PsychologicallyOutdoors)
                return null;

            if (room.Role == RoomRoleDefOf.None)
                return null;

            return room;
        }

    }
}