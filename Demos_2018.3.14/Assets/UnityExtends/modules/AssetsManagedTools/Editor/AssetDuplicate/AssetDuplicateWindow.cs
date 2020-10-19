using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Framework.Extends;
using Framework.Editor;

namespace Framework.Utility.Editor
{
    public class AssetDuplicateWindow :UnityEditor.EditorWindow
    {

        private struct AnalysResultItem
        {

        }

        //------------------------------------------

        private const float BUTTONUIHEIGHT = 28;

        private static GUIStyle _titleStyle;
        protected static GUIStyle titleStyle
        {
            get {
                if(_titleStyle == null)
                {
                    _titleStyle = EditorStyles.largeLabel.Clone();
                    _titleStyle.fontSize = 16;
                }
                return _titleStyle;
            }
        }

        private static GUIContent _content_UseSelection;
        protected static GUIContent contentUseSelection
        {
            get {
                if(_content_UseSelection == null)
                {
                    _content_UseSelection = new GUIContent("UseSelection", "使用Selection选中路径");
                }
                return _content_UseSelection;
            }
        }

        private static GUIContent _content_Set;
        protected static GUIContent contentSet
        {
            get {
                if(_content_Set == null)
                {
                    _content_Set = new GUIContent("Set", "设置搜索路径");
                }
                return _content_Set;
            }
        }

        private static AssetDuplicateWindow _instance;

        [MenuItem("AssetsManagedTools/Prefab/资产复制工具", false, 10100)]
        public static AssetDuplicateWindow init()
        {

            _instance = UnityEditor.EditorWindow.GetWindow<AssetDuplicateWindow>();
            _instance.minSize = new Vector2(495, 612);

            return _instance;
        }

        private void Awake()
        {
            //
        }

        private void OnGUI()
        {
            if(EditorApplication.isCompiling)
                Close();

            GUILayout.Space(15);
            _draw_toolTitle_UI();
            GUILayout.Space(15);
            _draw_targetDirPath_UI();
            //
            GUILayout.Space(5);
            _draw_assetAnalysing_UI();
            GUILayout.Space(5);

            GUILayout.FlexibleSpace();
            _draw_duplicateStart_UI();

        }


        private string m_srcDirPath;
        private string m_targetDirPath;
        private void _draw_toolTitle_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("资产复制工具", titleStyle);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private void _draw_targetDirPath_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("源文件夹路径");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    m_srcDirPath = EditorGUILayout.TextField(m_srcDirPath);
                    if(GUILayout.Button(contentUseSelection, GUILayout.Width(120)))
                    {
                        if(Selection.activeObject)
                        {
                            string tp = AssetDatabase.GetAssetPath(Selection.activeObject);
                            if(!string.IsNullOrEmpty(tp))
                            {

                                EditorAssetInfo info = new EditorAssetInfo(tp);
                                m_srcDirPath = info.dirPath;

                            }
                            else
                            {
                                m_srcDirPath = "";
                            }
                        }
                        else
                        {
                            m_srcDirPath = "";
                        }
                    }
                    if(GUILayout.Button(contentSet, GUILayout.Width(50)))
                    {
                        m_srcDirPath = EditorUtility.SaveFolderPanel("设置搜索路径", "", "");
                        m_srcDirPath = m_srcDirPath.Replace(Application.dataPath, "Assets");
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("目标文件夹路径");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    m_targetDirPath = EditorGUILayout.TextField(m_targetDirPath);
                    if(GUILayout.Button(contentUseSelection, GUILayout.Width(120)))
                    {
                        if(Selection.activeObject)
                        {
                            string tp = AssetDatabase.GetAssetPath(Selection.activeObject);
                            if(!string.IsNullOrEmpty(tp))
                            {

                                EditorAssetInfo info = new EditorAssetInfo(tp);
                                m_targetDirPath = info.dirPath;

                            }
                            else
                            {
                                m_targetDirPath = "";
                            }
                        }
                        else
                        {
                            m_targetDirPath = "";
                        }
                    }
                    if(GUILayout.Button(contentSet, GUILayout.Width(50)))
                    {
                        m_targetDirPath = EditorUtility.SaveFolderPanel("设置搜索路径", "", "");
                        m_targetDirPath = m_targetDirPath.Replace(Application.dataPath, "Assets");
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private void _draw_assetAnalysing_UI()
        {
            if( !string.IsNullOrEmpty(m_srcDirPath)
                && !string.IsNullOrEmpty(m_targetDirPath)
                //
                )
            {
                if(GUILayout.Button("开始资源复制", GUILayout.Height(BUTTONUIHEIGHT)))
                {
                    
                }
            }
            else
            {
                GUI.color = Color.gray;
                if(GUILayout.Button("开始资源复制", GUILayout.Height(BUTTONUIHEIGHT)))
                {
                    //do nothing;
                }
                GUI.color = Color.white;
            }
        }

        private void _draw_analysingResult_UI()
        {

        }

        private void _draw_duplicateStart_UI()
        {
            if(!string.IsNullOrEmpty(m_srcDirPath))
            {
                if(GUILayout.Button("分析源文件夹", GUILayout.Height(BUTTONUIHEIGHT * 1.5f)))
                {
                    assetAnalysing();
                }
            }
            else
            {
                GUI.color = Color.gray;
                if(GUILayout.Button("分析源文件夹", GUILayout.Height(BUTTONUIHEIGHT * 1.5f)))
                {
                    //do nothing;
                }
                GUI.color = Color.white;
            }
        }

        //----------------------------------------------------

        private readonly List<AnalysResultItem> m_analysList = new List<AnalysResultItem>();

        private void assetAnalysing()
        {
            m_analysList.Clear();

            //List<EditorAssetInfo> info

        }

        private void duplicateStart()
        {

        }

    }
}


