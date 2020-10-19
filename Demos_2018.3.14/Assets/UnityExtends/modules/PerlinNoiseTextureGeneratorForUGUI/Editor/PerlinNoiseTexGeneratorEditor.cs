using UnityEngine;
using UnityEditor;
using System.IO;

namespace Framework.Editor
{
    [CustomEditor(typeof(PerlinNoiseTexGenerator))]
    public class PerlinNoiseTexGeneratorEditor : UnityEditor.Editor
    {

        [MenuItem("Component/UI/RawImagePlus/PerlinNoiseTexGenerator")]
        private static void AddComponent()
        {
            GameObject select = Selection.activeGameObject;
            if(select)
            {
                select.AddComponent<PerlinNoiseTexGenerator>();
            }
        }
        //-------------------------------------------------

        private const int BTNHEIGHT = 26;

        private PerlinNoiseTexGenerator m_target;

        private void Awake()
        {
            m_target = target as PerlinNoiseTexGenerator;
        }

        public override void OnInspectorGUI()
        {
            
            GUILayout.Space(12);

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("PerlinNoiseDataWraper配置");
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("打开配置窗口"))
                    {
                        PerlinNoiseDatasetWindow.init(m_target);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
            GUILayout.EndVertical();

            if(m_target.CacheLens > 0)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("CacheLens[" + m_target.CacheLens + "]");
                    if(GUILayout.Button("ClearCache"))
                    {
                        m_target.ClearCache();
                    }
                    if(GUILayout.Button("TryRecoverTexFormCache"))
                    {
                        m_target.TryRecoverTexFormCache();
                    }
                }
                GUILayout.EndHorizontal();

            }

            if(m_target.texture)
            {
                GUILayout.BeginHorizontal();
                {
                    if(GUILayout.Button("Export Texture To Asset", GUILayout.Height(BTNHEIGHT)))
                    {
                        ExportTex2Asset();
                    }
                    if(GUILayout.Button("ClearTexture", GUILayout.Height(BTNHEIGHT)))
                    {
                        m_target.texture = null;
                    }
                }
                GUILayout.EndHorizontal();

            }
        }

        private void ExportTex2Asset()
        {

            Texture2D texture2D = m_target.texture as Texture2D;
            if(!texture2D)
                EditorUtility.DisplayDialog("提示", "Texture引用不是Texture2D,导出PNG失败.", "OK");

            string path = EditorUtility.SaveFilePanel("导出到Asset", "Assets", "ExportFile", "png");
            if(!string.IsNullOrEmpty(path))
            {
                byte[] bytes = texture2D.EncodeToPNG();
                File.WriteAllBytes(path, bytes);
                m_target.ClearTexture();
                EditorUtility.DisplayDialog("提示", "PNG文件导出完成.", "OK");
                AssetDatabase.Refresh();
            }
        }

    }
}
