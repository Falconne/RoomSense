using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using UnityEngine;
using Verse;

namespace RoomSense
{
    //[HarmonyPatch(typeof(SectionLayer), "DrawLayer")]
    [HarmonyPatch(typeof(DynamicDrawManager), "DrawDynamicThings")]
    public class SectionLayer_DrawLayer_Patch
    {
        private static readonly FieldInfo _sectionGetter = typeof(SectionLayer).GetField("section",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static Type _sectionLayerTerrainType;

        static void Postfix(ref DynamicDrawManager __instance)
        {
            /*if (_sectionLayerTerrainType == null)
            {
                _sectionLayerTerrainType = GenTypes.GetTypeInAnyAssembly("Verse.SectionLayer_Terrain");
                if (_sectionLayerTerrainType == null)
                {
                    Main.Instance.Logger.Warning("Could not get Verse.SectionLayer_Terrain via reflection");
                    return;
                }
            }

            if (__instance.GetType() != _sectionLayerTerrainType)
                return;

            if (!__instance.Visible)
                return;

            var section = (Section)_sectionGetter.GetValue(__instance);
                */
            var map = Find.VisibleMap;
            /*
            var foundRooms = new HashSet<Room>();
            var listerBuildings = map.listerBuildings;
            foreach (var building in listerBuildings.allBuildingsColonist)
            {
                var room = InfoCollector.GetRoomContainingBuildingIfRelevant(building, map);
                if (room == null)
                    continue;

                if (foundRooms.Contains(room))
                    continue;

                foundRooms.Add(room);
                var topLeft = InfoCollector.GetPanelTopLeftCornerForRoom(room, map);
                if (section.CellRect.Contains(topLeft))
                {
                    GenDraw.DrawLineBetween(topLeft.ToVector3(), topLeft.ToVector3() + new Vector3(0f, 0f, 5f));
                }
            }
            */
            var color = Color.white;
            color.a = 0.33f;
            var texture = GraphOverlay.TestTexture;
            if (texture == null)
                return;

            var mat = MaterialPool.MatFrom(Resources.Beauty, ShaderDatabase.Transparent, color);
            //var mat = MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, Color.white);
            CellRect currentViewRect = Find.CameraDriver.CurrentViewRect;
            foreach (IntVec3 sectionCell in currentViewRect)
            {
                var room = sectionCell.GetRoom(map, RegionType.Set_All);
                if (room == null || room.PsychologicallyOutdoors)
                    continue;

                var topLeft = room.BorderCells.First();
                //GenDraw.DrawLineBetween(topLeft.ToVector3(), topLeft.ToVector3() + new Vector3(5f, 0f, 5f));
                //var rect = new Rect(topLeft.x, topLeft.y, 10f, 10f);
                //Graphics.DrawTexture(rect, Resources.Beauty);
                //Graphics.DrawMesh(mesh, new Vector3(100f, 100f), Quaternion.identity, mat, 0);
                Vector3 s = new Vector3(.5f, 1f, .5f);
                Matrix4x4 matrix = default(Matrix4x4);
                var pos = sectionCell.ToVector3();
                pos.x -= .5f;
                pos.z -= .5f;
                matrix.SetTRS(pos, Quaternion.identity, s);

                Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 0);

                /*TextGenerationSettings settings = new TextGenerationSettings();
                settings.textAnchor = TextAnchor.MiddleCenter;
                settings.color = Color.red;
                settings.generationExtents = new Vector2(100f, 100F);
                settings.pivot = Vector2.zero;
                settings.richText = true;
                settings.font = Text.textFieldStyles[2].font;
                settings.fontSize = 32;
                settings.fontStyle = FontStyle.Normal;
                //settings.wrapMode = TextWrapMode.Wrap;
                TextGenerator generator = new TextGenerator();
                generator.Populate("A", settings);
                mat = Text.textFieldStyles[2].font.material;
                Graphics.DrawMesh(GetMesh(generator), matrix, mat, 0);
                */



                //GenDraw.DrawMeshNowOrLater(mesh, sectionCell.ToVector3(), Quaternion.identity, mat, false);
                //Main.Instance.Logger.Message("Draw");
                //              return;
            }
            //Main.Instance.Logger.Message("Here");
        }

        static public Mesh GetMesh(TextGenerator i_Generator)
        {
            var o_Mesh = new Mesh();
 
            int vertSize = i_Generator.vertexCount;
            Vector3[] tempVerts = new Vector3[vertSize];
            Color32[] tempColours = new Color32[vertSize];
            Vector2[] tempUvs = new Vector2[vertSize];
            IList<UIVertex> generatorVerts = i_Generator.verts;
            for (int i = 0; i < vertSize; ++i)
            {
                tempVerts[i] = generatorVerts[i].position;
                tempColours[i] = generatorVerts[i].color;
                tempUvs[i] = generatorVerts[i].uv0;
            }
            o_Mesh.vertices = tempVerts;
            o_Mesh.colors32 = tempColours;
            o_Mesh.uv = tempUvs;
 
            int characterCount = vertSize / 4;
            int[] tempIndices = new int[characterCount * 6];
            for(int i = 0; i < characterCount; ++i)
            {
                int vertIndexStart = i * 4;
                int trianglesIndexStart = i * 6;
                tempIndices[trianglesIndexStart++] = vertIndexStart;
                tempIndices[trianglesIndexStart++] = vertIndexStart + 1;
                tempIndices[trianglesIndexStart++] = vertIndexStart + 2;
                tempIndices[trianglesIndexStart++] = vertIndexStart;
                tempIndices[trianglesIndexStart++] = vertIndexStart + 2;
                tempIndices[trianglesIndexStart] = vertIndexStart + 3;
            }
            o_Mesh.triangles = tempIndices;
            //TODO: setBounds manually
            o_Mesh.RecalculateBounds();

            return o_Mesh;
        }

        static public Mesh TextGenToMesh(TextGenerator generator)
        {
            var mesh = new Mesh();

            mesh.vertices = generator.verts.Select(v => v.position).ToArray();
            mesh.colors32 = generator.verts.Select(v => v.color).ToArray();
            //mesh.uv = generator.verts.Select(v => v.uv).ToArray();
            var triangleCount = generator.vertexCount * 6;
            mesh.triangles = new int[triangleCount];
            for (var i = 0; i < triangleCount;)
            {
                var t = i;
                mesh.triangles[i++] = t;
                mesh.triangles[i++] = t + 1;
                mesh.triangles[i++] = t + 2;
                mesh.triangles[i++] = t;
                mesh.triangles[i++] = t + 2;
                mesh.triangles[i++] = t + 3;
            }
            return mesh;
        }
    }

}