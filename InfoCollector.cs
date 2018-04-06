using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RoomSense
{
    public struct RoomStat
    {
        public RoomStatDef StatDef;
        public int CurrentLevel;
        public int MaxLevel;
    }

    public class RoomInfo
    {
        public IntVec3 PanelCellTopLeft;
        public List<RoomStat> Stats = new List<RoomStat>();
        public int MaxStatSize;
    };

    public class InfoCollector
    {
        private int _nextUpdateTick;

        public Dictionary<Room, RoomInfo> RelevantRooms { get; }

        public int MaxStatCount { get; private set; }

        public int MaxStatSize { get; private set; }

        public InfoCollector()
        {
            RelevantRooms = new Dictionary<Room, RoomInfo>();
            MaxStatCount = 0;
            MaxStatSize = 0;
        }

        public bool IsValid()
        {
            return MaxStatCount > 0 && MaxStatSize > 0 && RelevantRooms.Count > 0;
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

                if (RelevantRooms.ContainsKey(room))
                    continue;

                var roomInfo = new RoomInfo();
                if (!ComputeRoomStats(room, roomInfo.Stats))
                    continue;

                roomInfo.MaxStatSize = roomInfo.Stats.Max(s => s.MaxLevel);

                roomInfo.PanelCellTopLeft = GetPanelTopLeftCornerForRoom(room, map);

                RelevantRooms[room] = roomInfo;
            }
        }

        public void Reset()
        {
            _nextUpdateTick = 0;
            RelevantRooms.Clear();
        }

        private bool ComputeRoomStats(Room room, List<RoomStat> stats)
        {
            foreach (var statDef in DefDatabase<RoomStatDef>.AllDefsListForReading)
            {
                if (statDef.isHidden)
                    continue;

                if (!room.Role.IsStatRelated(statDef))
                    continue;

                var stat = room.GetStat(statDef);
                var roomStat = new RoomStat()
                {
                    StatDef = statDef,
                    CurrentLevel = statDef.GetScoreStageIndex(stat),
                    MaxLevel = statDef.scoreStages.Count
                };

                if (roomStat.MaxLevel > MaxStatSize)
                    MaxStatSize = roomStat.MaxLevel;

                stats.Add(roomStat);
            }

            if (stats.Count > MaxStatCount)
                MaxStatCount = stats.Count;

            return stats.Count > 0;
        }

        private IntVec3 GetPanelTopLeftCornerForRoom(Room room, Map map)
        {
            var bestCell = room.BorderCells.First();
            foreach (var cell in room.BorderCells)
            {
                if (cell.x < bestCell.x || cell.z > bestCell.z)
                    bestCell = cell;
            }

            var possiblyBetterCell = bestCell;
            possiblyBetterCell.x++;
            possiblyBetterCell.z--;
            if (possiblyBetterCell.GetRoom(map) == room)
                bestCell = possiblyBetterCell;

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