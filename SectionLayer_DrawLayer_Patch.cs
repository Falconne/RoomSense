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
    public class SectionLayer_DrawLayer_Patch
    {
        static bool Prefix(ref DynamicDrawManager __instance)
        {
            var map = Find.VisibleMap;
            var color = Color.white;
            color.a = 0.33f;

            var mat = MaterialPool.MatFrom(Resources.Beauty, ShaderDatabase.Transparent, color);
            CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;
            foreach (IntVec3 sectionCell in currentViewRect)
            {
                var room = sectionCell.GetRoom(map, RegionType.Set_All);
                if (room == null || room.PsychologicallyOutdoors)
                    continue;

                var topLeft = room.BorderCells.First();
                Vector3 s = new Vector3(.5f, 1f, .5f);
                Matrix4x4 matrix = default(Matrix4x4);
                var pos = sectionCell.ToVector3();
                pos.x -= .5f;
                pos.z -= .5f;
                matrix.SetTRS(pos, Quaternion.identity, s);

                Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 0);
            }

            return true;
        }
    }

}