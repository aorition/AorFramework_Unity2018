using UnityEngine;
using UnityEditor;
using AorBaseUtility;

namespace Framework.Editor.Tools
{

    public enum ExportT4MAssetMatType {
        T4MLite_Diffuse,
        T4MLite_Specular,
        T4MLite_Unlit
    }

    public class TerrainEditToolUtility
    {
        /// <summary>
        /// 从TerrainData导出Mesh数据
        /// </summary>
        /// <param name="data">TerrainData</param>
        /// <param name="segmentX">X段数</param>
        /// <param name="segmentY">Y段数</param>
        public static Mesh ExportMeshFromTerrainData(TerrainData data, int segmentX, int segmentY)
        {

            var meshScale = new Vector2(data.size.x / (segmentX - 1), data.size.z / (segmentY - 1));
            var uvScale = new Vector2(1.0f / (segmentX - 1), 1.0f / (segmentY - 1));

            var tVertices = new Vector3[segmentX * segmentY];
            var tNormals = new Vector3[segmentX * segmentY];
            var tTangents = new Vector4[segmentX * segmentY];
            var tUV = new Vector2[segmentX * segmentY];
            var tColors = new Color[segmentX * segmentY];

            int index = 0;
            int y, x;
            Vector2 uv = Vector2.zero;

            for (y = 0; y < segmentY; y++)
            {
                for (x = 0; x < segmentX; x++)
                {
                    
                    index = y * segmentX + x;
                    uv = new Vector2(x * uvScale.x, y * uvScale.y);
                    tVertices[index] = new Vector3(x * meshScale.x, data.GetInterpolatedHeight(uv.x, uv.y) ,y * meshScale.y);
                    tUV[index] = new Vector2(uv.x, uv.y);
                    tNormals[index] = data.GetInterpolatedNormal(uv.x, uv.y);
                    tTangents[index] = new Vector4(1, 0, 0, 1);
                    tColors[index] = new Color(1, 1, 1, 1);
                }
            }

            index = 0;

            var tPolys = new int[(segmentX - 1) * (segmentY - 1) * 6];

            for (y = 0; y < segmentX - 1; y++)
            {
                for (x = 0; x < segmentX - 1; x++)
                {
                    // For each grid cell output two triangles  
                    tPolys[index++] = (y * segmentX) + x;
                    tPolys[index++] = ((y + 1) * segmentX) + x;
                    tPolys[index++] = (y * segmentX) + x + 1;

                    tPolys[index++] = ((y + 1) * segmentX) + x;
                    tPolys[index++] = ((y + 1) * segmentX) + x + 1;
                    tPolys[index++] = (y * segmentX) + x + 1;
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = tVertices;
            mesh.uv = tUV;
            mesh.normals = tNormals;
            mesh.colors = tColors;
            mesh.triangles = tPolys;

            return mesh;
        }

        public static void ExportT4MAsset(Terrain terrain, string savedir, int segment, ExportT4MAssetMatType matType) {

            TerrainData data = terrain.terrainData;
            if (!data) return;

            string tName = terrain.name;
            //创建导出文件夹
            if (!AssetDatabase.IsValidFolder(savedir + "/" + tName))
            {
                AssetDatabase.CreateFolder(savedir, tName);
            }
            string dir = savedir + "/" + tName;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //检查mesh文件夹
            if (!AssetDatabase.IsValidFolder(dir + "/mesh")) AssetDatabase.CreateFolder(dir, "mesh");
            //检查tex文件夹
            if (!AssetDatabase.IsValidFolder(dir + "/tex")) AssetDatabase.CreateFolder(dir, "tex");
            //检查mat文件夹
            if (!AssetDatabase.IsValidFolder(dir + "/mat")) AssetDatabase.CreateFolder(dir, "mat");

            if (data.alphamapTextures != null && data.alphamapTextures.Length > 0)
            {

                //创建预制体
                GameObject prefab = new GameObject(tName);
                prefab.transform.position = terrain.transform.position;

                for (int i = 0; i < data.alphamapTextures.Length; i++)
                {
                    //导出 mesh
                    Mesh mesh = TerrainEditToolUtility.ExportMeshFromTerrainData(data, segment, segment);
                    string lp_mesh = dir + "/mesh/" + tName + "_mesh_" + i +".asset";
                    AssetDatabase.DeleteAsset(lp_mesh);
                    AssetDatabase.CreateAsset(mesh, lp_mesh);

                    //导出 Splatmap
                    string lp_control = dir + "/tex/" + tName + "_splat_" + i + ".png";
                    byte[] bytes = data.alphamapTextures[i].EncodeToPNG();
                    AorIO.SaveBytesToFile(lp_control, bytes);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Texture2D control = AssetDatabase.LoadAssetAtPath<Texture2D>(lp_control);

                    //导出 材质球
                    Material material = CreateT4MMaterial(matType, data, control, i);
                    if (material == null) {
                        Debug.LogError("*** TerrainEditToolUtility.ExportT4MAsset Error :: material can not created.");
                        continue;
                    }
                    string lp_mat = dir + "/mat/" + tName + "_mat_" + i + ".mat";
                    AssetDatabase.DeleteAsset(lp_mat);
                    AssetDatabase.CreateAsset(material, lp_mat);

                    //创建subLayer
                    GameObject subLayer = new GameObject("layer_" + i);
                    subLayer.transform.SetParent(prefab.transform, false);
                    MeshFilter meshFilter = subLayer.AddComponent<MeshFilter>();
                    MeshRenderer renderer = subLayer.AddComponent<MeshRenderer>();
                    if (Application.isPlaying)
                    {
                        meshFilter.mesh = mesh;
                        renderer.material = material;
                    }
                    else
                    {
                        meshFilter.sharedMesh = mesh;
                        renderer.sharedMaterial = material;
                    }

                    //导出 Obj文件
                    string lp_obj = dir + "/mesh/" + tName + "_obj_" + i + ".obj";
                    ObjExporter.MeshToFile(meshFilter, lp_obj, 1.0f);

                }

                //导出预制体
                string lp_prefab = dir + "/" + tName + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(prefab, lp_prefab);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (Application.isPlaying)
                {
                    GameObject.Destroy(prefab);
                }
                else
                {
                    GameObject.DestroyImmediate(prefab);
                }

            }
        }

        private static Material CreateT4MMaterial(ExportT4MAssetMatType matType, TerrainData data, Texture2D Control, int splatId)
        {
            Shader shader = null;
            Material material = null;
            switch (matType)
            {
                default:
                    shader = Shader.Find("T4MLiteShaders/T4MLite_Diffuse");
                    if (shader)
                    {
                        material = new Material(shader);
                        //_Control
                        material.SetTexture("_Control", Control);
                        //
                        int splatIndex = splatId * 4;
                        //_Splat0
                        if (splatIndex < data.terrainLayers.Length)
                        {
                            SetMaterialSplatValue(ref material, "_Splat0", data.terrainLayers[splatIndex]);
                            splatIndex++;
                        }
                        //_Splat1
                        if (splatIndex < data.terrainLayers.Length)
                        {
                            SetMaterialSplatValue(ref material, "_Splat1", data.terrainLayers[splatIndex]);
                            splatIndex++;
                        }
                        //_Splat2
                        if (splatIndex < data.terrainLayers.Length)
                        {
                            SetMaterialSplatValue(ref material, "_Splat2", data.terrainLayers[splatIndex]);
                            splatIndex++;
                        }
                        //_Splat3
                        if (splatIndex < data.terrainLayers.Length)
                        {
                            SetMaterialSplatValue(ref material, "_Splat3", data.terrainLayers[splatIndex]);
                            splatIndex++;
                        }
                    }
                    break;
            }
            return material;
        }

        private static void SetMaterialSplatValue(ref Material material, string SplatName, TerrainLayer tLayer) {
            Texture2D diffuse = tLayer.diffuseTexture;
            material.SetTexture(SplatName, tLayer.diffuseTexture);
            material.SetTextureScale(SplatName, new Vector2(diffuse.width / tLayer.tileSize.x, diffuse.height / tLayer.tileSize.y));
            material.SetTextureOffset(SplatName, new Vector2(tLayer.tileOffset.x, tLayer.tileOffset.y));
        }

    }
}