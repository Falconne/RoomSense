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

        public string RawCurrentLevel;
        public string ValueLabel;
    }

    public class RoomInfo
    {
        public readonly List<RoomStat> Stats;
        public readonly IntVec3 PanelCellTopLeft;
        public readonly int MaxStatSize;

        public RoomInfo(List<RoomStat> stats, IntVec3 panelCellTopLeft, int maxStatSize)
        {
            Stats = stats;
            PanelCellTopLeft = panelCellTopLeft;
            MaxStatSize = maxStatSize;
        }
    };

    public class InfoCollector
    {
        private int _nextUpdateTick;

        public Dictionary<Room, RoomInfo> RelevantRooms { get; }

        public int MaxStatCount { get; private set; }

        public int MaxStatSize { get; private set; }

        private bool _infoChanged = false;

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

        public bool IsTimeToUpdateHeatMap()
        {
            if (!_infoChanged)
                return false;

            _infoChanged = false;
            return true;

        }

        public void Update(int updateDelay)
        {
            var tick = Find.TickManager.TicksGame;
            if (_nextUpdateTick != 0 && tick < _nextUpdateTick)
                return;

            _nextUpdateTick = tick + updateDelay;
            _infoChanged = true;
            RelevantRooms.Clear();

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

                var stats = GetRoomStats(room);
                if (stats.Count == 0)
                    continue;

                var roomInfo = new RoomInfo(
                    stats,
                    GetPanelTopLeftCornerForRoom(room, map),
                    stats.Max(s => s.MaxLevel)
                );

                RelevantRooms[room] = roomInfo;
            }
        }

        public void Reset()
        {
            _nextUpdateTick = 0;
            RelevantRooms.Clear();
        }

        private List<RoomStat> GetRoomStats(Room room)
        {
            var stats = new List<RoomStat>();
            foreach (var statDef in DefDatabase<RoomStatDef>.AllDefsListForReading)
            {
                if (statDef.isHidden)
                    continue;

                if (!room.Role.IsStatRelated(statDef))
                    continue;

                var stat = room.GetStat(statDef);
                RoomStatScoreStage scoreStage = statDef.GetScoreStage(stat);
                var roomStat = new RoomStat()
                {
                    StatDef = statDef,
                    CurrentLevel = statDef.GetScoreStageIndex(stat),
                    MaxLevel = statDef.scoreStages.Count,
                    RawCurrentLevel = statDef.ScoreToString(stat),
                    ValueLabel = scoreStage != null ? scoreStage.label : string.Empty
                };

                if (roomStat.MaxLevel > MaxStatSize)
                    MaxStatSize = roomStat.MaxLevel;

                stats.Add(roomStat);
            }

            if (stats.Count > MaxStatCount)
                MaxStatCount = stats.Count;

            return stats;
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