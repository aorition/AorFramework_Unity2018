using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Framework.Editor.Tools
{
    /// <summary>
    /// 效果不好，重新找。。。。
    /// </summary>
    public class ObjExporter
    {
        //
        // Static Methods
        //
        public static void MeshToFile(MeshFilter mf, string savePath, float scale)
        {
            AorBaseUtility.AorIO.SaveStringToFile(savePath, ObjExporter.MeshToString(mf, scale));

        }

        public static string MeshToString(MeshFilter mf, float scale)
        {
            Mesh mesh = Application.isPlaying ? mf.mesh : mf.sharedMesh;
            //Material[] sharedMaterials = mf.GetComponent<Renderer>().sharedMaterials;
            Renderer renderer = mf.GetComponent<Renderer>();
            Material material = Application.isPlaying ? renderer.material : renderer.sharedMaterial;

            Vector2 textureOffset = Vector2.zero;
            Vector2 textureScale = Vector2.one;

            if (material.HasProperty("_MainTex"))
            {
                textureOffset = material.GetTextureOffset("_MainTex");
                textureScale = material.GetTextureScale("_MainTex");
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("mtllib design.mtl").Append("\n");
            stringBuilder.Append("g ").Append(mf.name).Append("\n");
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vector = vertices[i];
                stringBuilder.Append(string.Format("v {0} {1} {2}\n", vector.x * scale, vector.z * scale, vector.y * scale));
            }
            stringBuilder.Append("\n");
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            if (mesh.subMeshCount > 1)
            {
                int[] triangles = mesh.GetTriangles(1);
                for (int j = 0; j < triangles.Length; j += 3)
                {
                    if (!dictionary.ContainsKey(triangles[j]))
                    {
                        dictionary.Add(triangles[j], 1);
                    }
                    if (!dictionary.ContainsKey(triangles[j + 1]))
                    {
                        dictionary.Add(triangles[j + 1], 1);
                    }
                    if (!dictionary.ContainsKey(triangles[j + 2]))
                    {
                        dictionary.Add(triangles[j + 2], 1);
                    }
                }
            }
            for (int num = 0; num != mesh.uv.Length; num++)
            {
                Vector2 vector2 = Vector2.Scale(mesh.uv[num], textureScale) + textureOffset;
                if (dictionary.ContainsKey(num))
                {
                    stringBuilder.Append(string.Format("vt {0} {1}\n", mesh.uv[num].x, mesh.uv[num].y));
                }
                else
                {
                    stringBuilder.Append(string.Format("vt {0} {1}\n", vector2.x, vector2.y));
                }
            }
            for (int k = 0; k < mesh.subMeshCount; k++)
            {
                stringBuilder.Append("\n");
                if (k == 0)
                {
                    stringBuilder.Append("usemtl ").Append("Material_design").Append("\n");
                }
                if (k == 1)
                {
                    stringBuilder.Append("usemtl ").Append("Material_logo").Append("\n");
                }
                int[] triangles2 = mesh.GetTriangles(k);
                for (int l = 0; l < triangles2.Length; l += 3)
                {
                    stringBuilder.Append(string.Format("f {0}/{0} {1}/{1} {2}/{2}\n", triangles2[l] + 1, triangles2[l + 1] + 1, triangles2[l + 2] + 1));
                }
            }
            return stringBuilder.ToString();
        }
    }
}