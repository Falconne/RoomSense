﻿using System.Collections.Generic;
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

        private Mesh CreateMeshFor(string label)
        {
            Vector3[] array = new Vector3[4];
            Vector2[] array2 = new Vector2[4];
            var size = new Vector2
            {
                x = 0.5f,
                y = 1f
            };

            int[] array3 = new int[6];
            array[0] = new Vector3(-0.5f * size.x, 0f, -0.5f * size.y);
            array[1] = new Vector3(-0.5f * size.x, 0f, 0.5f * size.y);
            array[2] = new Vector3(0.5f * size.x, 0f, 0.5f * size.y);
            array[3] = new Vector3(0.5f * size.x, 0f, -0.5f * size.y);

            array2[0] = new Vector2(0.030f, 0f);
            array2[1] = new Vector2(0.030f, 1f);
            array2[2] = new Vector2(0.015f, 1f);
            array2[3] = new Vector2(0.015f, 0f);

            array3[0] = 0;
            array3[1] = 1;
            array3[2] = 2;
            array3[3] = 0;
            array3[4] = 2;
            array3[5] = 3;
            var mesh = new Mesh
            {
                name = "NewPlaneMesh()",
                vertices = array,
                uv = array2
            };
            mesh.SetTriangles(array3, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }


    }
}