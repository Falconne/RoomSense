using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using UnityEngine;
using Verse;

namespace RoomSense
{
    [HarmonyPatch(typeof(DynamicDrawManager), "DrawDynamicThings")]
    public class DynamicDrawManager_DrawDynamicThings_Patch
    {
        private static Mesh _testMesh;

        static bool Prefix(ref DynamicDrawManager __instance)
        {
            var map = Find.VisibleMap;
            var color = Color.white;
            color.a = 0.33f;

            //var mat = MaterialPool.MatFrom(Resources.Beauty, ShaderDatabase.Transparent, color);
            var mat = MaterialPool.MatFrom(Resources.Font, ShaderDatabase.Transparent, color);
            CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;
            foreach (IntVec3 sectionCell in currentViewRect)
            {
                var room = sectionCell.GetRoom(map, RegionType.Set_All);
                if (room == null || room.PsychologicallyOutdoors)
                    continue;

                var topLeft = room.BorderCells.First();
                if (_testMesh == null)
                    CreateTestMesh();

                var s = new Vector3(.5f, 1f, .5f);
                Matrix4x4 matrix = default;
                var pos = sectionCell.ToVector3();
                pos.x -= .5f;
                pos.z -= .5f;
                matrix.SetTRS(pos, Quaternion.identity, s);

                Graphics.DrawMesh(_testMesh, matrix, mat, 0);
            }

            return true;
        }

        public static void CreateTestMesh()
        {
            Vector3[] array = new Vector3[4];
            Vector2[] array2 = new Vector2[4];
            var size = new Vector2
            {
                x = 1f,
                y = 1f
            };

            int[] array3 = new int[6];
            array[0] = new Vector3(-0.5f * size.x, 0f, -0.5f * size.y);
            array[1] = new Vector3(-0.5f * size.x, 0f, 0.5f * size.y);
            array[2] = new Vector3(0.5f * size.x, 0f, 0.5f * size.y);
            array[3] = new Vector3(0.5f * size.x, 0f, -0.5f * size.y);

            array2[0] = new Vector2(0.015f, 0f);
            array2[1] = new Vector2(0.015f, 1f);
            array2[2] = new Vector2(0f, 1f);
            array2[3] = new Vector2(0f, 0f);

            array3[0] = 0;
            array3[1] = 1;
            array3[2] = 2;
            array3[3] = 0;
            array3[4] = 2;
            array3[5] = 3;
            _testMesh = new Mesh
            {
                name = "NewPlaneMesh()",
                vertices = array,
                uv = array2
            };
            _testMesh.SetTriangles(array3, 0);
            _testMesh.RecalculateNormals();
            _testMesh.RecalculateBounds();
        }

    }

}