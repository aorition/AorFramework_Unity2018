using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using Framework.Extends;
using Framework.Editor;
using AorBaseUtility.Extends;

namespace Framework.Utility.Editor
{

    public class PrefabBatchProcessor :UnityEditor.EditorWindow
    {

        private struct SpriteItem
        {

            public SpriteItem(Sprite sprite, bool useNativeSize)
            {
                this.sprite = sprite;
                this.useNativeSize = useNativeSize;
            }

            public Sprite sprite;
            public bool useNativeSize;
        }

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

        private static PrefabBatchProcessor _instance;

        [MenuItem("AssetsManagedTools/Prefab/Prefab批量处理工具", false, 10100)]
        public static PrefabBatchProcessor init()
        {

            _instance = UnityEditor.EditorWindow.GetWindow<PrefabBatchProcessor>();
            _instance.minSize = new Vector2(495, 612);

            return _instance;
        }

        private List<Type> m_pbaSubTypeList;
        private string[] m_pbaLabels;
        private int m_pbaCreateIdx;

        private string m_targetDirPath;

        private readonly StringBuilder m_info = new StringBuilder();

        private PrefabBatchActionBase m_processAction;

        private void Awake()
        {
            _initActionLabelList();
        }

        private void OnGUI()
        {
            if(EditorApplication.isCompiling)
                Close();

            GUILayout.Space(15);
            _draw_toolTitle_UI();
            GUILayout.Space(15);
            _draw_createProcessAction_UI();
            GUILayout.Space(5);
            _draw_processAction_UI();
            GUILayout.Space(5);
            if(m_processAction)
            {
                _draw_processActionSet_UI();
                GUILayout.Space(5);
            }
            _draw_targetDirPath_UI();
            GUILayout.Space(5);
            GUILayout.FlexibleSpace();
            _draw_start_UI();

        }

        private void _draw_toolTitle_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Prefab批量处理工具", titleStyle);
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
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("搜索路径");
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

        private void _initActionLabelList()
        {
            m_pbaCreateIdx = 0;
            m_pbaSubTypeList = new List<Type>();
            Type baseType = typeof(PrefabBatchActionBase);
            Type[] types = baseType.Assembly.GetTypes();
            int i, len = types.Length;
            for(i = 0; i < len; i++)
            {
                if(_checkBaseType(types[i], baseType))
                {
                    m_pbaSubTypeList.Add(types[i]);
                }
            }
            len = m_pbaSubTypeList.Count;
            m_pbaLabels = new string[len];
            for(i = 0; i < len; i++)
            {
                object[] attributes = m_pbaSubTypeList[i].GetCustomAttributes(true);
                if(attributes != null && attributes.Length > 0)
                {
                    int vlen = attributes.Length;
                    for(int v = 0; v < vlen; v++)
                    {
                        if(attributes[v] is PBPTagLabelAttribute)
                        {
                            m_pbaLabels[i] = ((PBPTagLabelAttribute)attributes[v]).label;
                        }
                    }
                }
                else
                {
                    m_pbaLabels[i] = m_pbaSubTypeList[i].Name;
                }
            }
        }

        private bool _checkBaseType(Type type, Type baseType)
        {
            if(type.BaseType == null)
                return false;
            if(type.BaseType == baseType)
                return true;
            else
                return _checkBaseType(type.BaseType, baseType);
        }

        private void _draw_createProcessAction_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("创建动作脚本");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                //
                GUILayout.BeginHorizontal();
                {
                    m_pbaCreateIdx = EditorGUILayout.Popup(m_pbaCreateIdx, m_pbaLabels);
                    if(GUILayout.Button("创建行为脚本"))
                    {
                        string savePath = EditorUtility.SaveFilePanelInProject("保存文件", m_pbaSubTypeList[m_pbaCreateIdx].Name, "asset", "");
                        if(!string.IsNullOrEmpty(savePath))
                        {
                            m_processAction = (PrefabBatchActionBase)ScriptableObject.CreateInstance(m_pbaSubTypeList[m_pbaCreateIdx]);

                            savePath = savePath.Replace(Application.dataPath, "Assets");
                            AssetDatabase.CreateAsset(m_processAction, savePath);

                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                        }
                    }
                }
                GUILayout.EndHorizontal();
                //
                GUILayout.Space(5);

            }
            GUILayout.EndVertical();
        }

        private void _draw_processAction_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("动作脚本");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                //
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("(PrefabBatchActionBase)");
                    m_processAction = (PrefabBatchActionBase)EditorGUILayout.ObjectField(m_processAction, typeof(PrefabBatchActionBase), false);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);

            }
            GUILayout.EndVertical();
        }

        private void _draw_processActionSet_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);

                GUILayout.BeginVertical("box");
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("动作脚本描述");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    m_processAction.DescriptionDraw();
                    GUILayout.Space(5);
                }
                GUILayout.EndVertical();

                GUILayout.Space(5);

                GUILayout.BeginVertical("box");
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("动作脚本设置");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    m_processAction.ActionInspectorDraw();
                    GUILayout.Space(5);
                }
                GUILayout.EndVertical();

                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private void _draw_start_UI()
        {

            GUILayout.BeginHorizontal();
            {
                if(!string.IsNullOrEmpty(m_targetDirPath) && m_processAction)
                {
                    if(GUILayout.Button("开始", GUILayout.Height(36)))
                    {
                        if(EditorUtility.DisplayDialog("提示", "确认开始?!", "开始", "取消"))
                        {
                            _start_Process();
                        }
                    }
                }
                else
                {
                    GUI.color = Color.gray;
                    if(GUILayout.Button("开始", GUILayout.Height(36)))
                    {
                        //do nothing 
                    }
                    GUI.color = Color.white;
                }

                if(m_info.Length > 0)
                {
                    if(GUILayout.Button("导出日志", GUILayout.Height(36), GUILayout.Width(Screen.width * 0.3f)))
                    {
                        if(EditorUtility.DisplayDialog("导出确认", "确认导出日志?!", "确定", "取消"))
                        {
                            string exportPath = EditorUtility.SaveFilePanel("导出日志", "Assets", "PrefabBatchProcessorLog", "txt");
                            if(!string.IsNullOrEmpty(exportPath))
                            {
                                File.WriteAllText(exportPath, m_info.ToString());
                                AssetDatabase.Refresh();
                            }
                        }
                    }
                }

            }
            GUILayout.EndHorizontal();

        }

        private void _start_Process()
        {
            m_processAction.Init();
            m_processAction.SetInfoBuilder(m_info);
            //
            m_info.AppendLine(0, "ProcessStart >");

            List<EditorAssetInfo> infoList = EditorAssetInfo.FindEditorAssetInfoInPath(m_targetDirPath, "*.prefab");
            if(infoList != null && infoList.Count > 0)
            {
                GameObject asset;
                int len = infoList.Count;
                for(int i = 0; i < len; i++)
                {
                    EditorUtility.DisplayProgressBar("Progress", "正在处理" + (i + 1) + "/" + len, (float)(i + 1) / len);
                    asset = AssetDatabase.LoadAssetAtPath<GameObject>(infoList[i].path);
                    if(asset)
                        m_processAction.Process(asset);
                }
                EditorUtility.ClearProgressBar();
            }
            m_info.AppendLine(0, "< ProcessEnd");
        }

    }
}


