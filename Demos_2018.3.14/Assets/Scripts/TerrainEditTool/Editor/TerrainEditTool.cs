using AorBaseUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor.Tools
{

    public class TerrainEditTool : UnityEditor.EditorWindow
    {

        private static GUIContent m_titleContent;

        [MenuItem("DEV/TerrainEdit/TerrainEditWindow")]
        public static void init()
        {
            TerrainEditTool window = UnityEditor.EditorWindow.GetWindow<TerrainEditTool>();
            if (m_titleContent == null)
            {
                m_titleContent = new GUIContent("TerrainEditWindow");
            }
            window.titleContent = m_titleContent;
        }

        private void OnGUI()
        {
            GUILayout.Space(5);

            _draw_savePathUI();

            GUILayout.Space(5);

            _draw_exportUI();

            GUILayout.Space(5);
        }

        private string m_saveDir;
        private void _draw_savePathUI() {

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Save Path", GUILayout.Width(70));
                    m_saveDir = EditorGUILayout.TextField(m_saveDir);
                    if (GUILayout.Button("UseSelection", GUILayout.Width(100)))
                    {
                        if (Selection.activeObject)
                        {
                            GUI.SetNextControlName("UseSelection");
                            m_saveDir = new EditorAssetInfo(Selection.activeObject).dirPath;
                            GUI.FocusControl("UseSelection");
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();

        }

        private int m_segment = 32;
        private void _draw_exportUI() {

            m_segment = EditorGUILayout.IntField("Mesh Segment", m_segment);

            if (GUILayout.Button("ExprotT4MAsset"))
            {
                if (!string.IsNullOrEmpty(m_saveDir)) {
                    exportT4MAsset(m_saveDir, m_segment, ExportT4MAssetMatType.T4MLite_Diffuse);
                }
            }
        }

        private void exportT4MAsset(string savedir, int segment, ExportT4MAssetMatType matType) {
            GameObject[] selects = Selection.gameObjects;
            if (selects != null && selects.Length > 0)
            {
                for (int i = 0; i < selects.Length; i++)
                {
                    GameObject select = selects[i];
                    exportT4MAssetLoop(select, savedir, segment, matType);
                }
            }
        }

        private void exportT4MAssetLoop(GameObject select, string savedir, int segment, ExportT4MAssetMatType matType)
        {

            Terrain terrain = select.GetComponent<Terrain>();
            if (terrain)
            {
                TerrainEditToolUtility.ExportT4MAsset(terrain, savedir, segment, matType);
            }

            if(select.transform.childCount > 0)
            {
                for (int i = 0; i < select.transform.childCount; i++)
                {
                    Transform sub = select.transform.GetChild(i);
                    exportT4MAssetLoop(sub.gameObject, savedir, segment, matType);
                }
            }
        }

    }


}
