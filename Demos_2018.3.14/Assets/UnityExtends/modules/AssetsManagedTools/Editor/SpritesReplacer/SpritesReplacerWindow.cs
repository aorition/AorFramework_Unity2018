using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Text;
using UnityEditor.SceneManagement;
using System.IO;
using Framework.Extends;
using Framework.Editor;

namespace Framework.Utility.Editor
{

    public class SpritesReplacerWindow :UnityEditor.EditorWindow
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

        private static GUIContent _content_ns;
        protected static GUIContent contentNs
        {
            get {
                if(_content_ns == null)
                {
                    _content_ns = new GUIContent("Ns", "Use NativeSize");
                }
                return _content_ns;
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

        private static SpritesReplacerWindow _instance;

        [MenuItem("AssetsManagedTools/Bitmaps/Sprite 引用批量替换工具", false, 10100)]
        public static SpritesReplacerWindow init()
        {

            _instance = UnityEditor.EditorWindow.GetWindow<SpritesReplacerWindow>();
            _instance.minSize = new Vector2(495, 612);

            return _instance;
        }

        private bool m_useThumbnail;
        private bool m_removeItemComf = true;

        private string m_targetDirPath;

        private Vector2 m_replaceDicScrollPos;
        
        private readonly List<Sprite> m_delSrcList = new List<Sprite>();

        private readonly Dictionary<Sprite, SpriteItem> m_replaceDataDic = new Dictionary<Sprite, SpriteItem>();

        private Sprite m_addedSrc;
        private Sprite m_addedTar;
        private bool m_addeduseNativeSize;

        private readonly HashSet<string> m_cacheHash = new HashSet<string>();

        private readonly StringBuilder m_info = new StringBuilder();

        private void OnGUI()
        {
            if(EditorApplication.isCompiling)
                Close();

            GUILayout.Space(15);
            _draw_toolTitle_UI();
            GUILayout.Space(15);
            _draw_dataOption_UI();
            GUILayout.Space(5);
            _draw_targetDirPath_UI();
            GUILayout.Space(5);
            _draw_replaceList_UI();
            GUILayout.FlexibleSpace();
            _draw_option_UI();
            _draw_start_UI();

            _try_delListProcess();
        }

        private void _draw_toolTitle_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Sprites替换工具", titleStyle);
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

        private void _draw_replaceList_UI()
        {

            if(m_replaceDataDic.Count > 0)
            {

                GUILayout.BeginVertical("box");
                {
                    GUILayout.Space(5);

                    m_replaceDicScrollPos = GUILayout.BeginScrollView(m_replaceDicScrollPos, "box");
                    {
                        GUILayout.Space(5);

                        //draw title
                        GUILayout.BeginHorizontal("box");
                        {
                            GUILayout.BeginHorizontal(GUILayout.MinWidth(205));
                            {
                                GUILayout.Label("Source");
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Label("->");
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal(GUILayout.MinWidth(205));
                            {
                                GUILayout.Label("Target");
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Label("UseNativeSize");
                            if(GUILayout.Button(" ","label", GUILayout.Width(42)))
                            {
                                //do nothing
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);

                        int i = 0;
                        foreach(var keyValue in m_replaceDataDic)
                        {
                            if(i > 0)
                                GUILayout.Space(2);

                            //draw item
                            GUILayout.BeginHorizontal();
                            {
                                Sprite nkey;
                                if(m_useThumbnail)
                                    nkey = (Sprite)EditorGUILayout.ObjectField(keyValue.Key.name, keyValue.Key, typeof(Sprite), false);
                                else
                                    nkey = (Sprite)EditorGUILayout.ObjectField(keyValue.Key, typeof(Sprite), false);
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Label("->");
                                    GUILayout.FlexibleSpace();
                                }
                                GUILayout.EndHorizontal();
                                Sprite nValue;
                                if(m_useThumbnail)
                                    nValue = (Sprite)EditorGUILayout.ObjectField(keyValue.Value.sprite.name, keyValue.Value.sprite, typeof(Sprite), false);
                                else
                                    nValue = (Sprite)EditorGUILayout.ObjectField(keyValue.Value.sprite, typeof(Sprite), false);
                                bool nNs = EditorGUILayout.ToggleLeft(contentNs, keyValue.Value.useNativeSize, GUILayout.Width(40));
                                if(GUILayout.Button("-", GUILayout.Width(42)))
                                {
                                    if(m_removeItemComf)
                                    {
                                        if(EditorUtility.DisplayDialog("提示","确认移除该条目?!","确定", "取消"))
                                            m_delSrcList.Add(keyValue.Key);
                                    }
                                    else
                                        m_delSrcList.Add(keyValue.Key);
                                }
                            }
                            GUILayout.EndHorizontal();

                        }

                        GUILayout.Space(5);
                    }
                    GUILayout.EndScrollView();

                    GUILayout.Space(5);

                }
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("(未配置替换列表)");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }
                GUILayout.EndVertical();
            }

            GUILayout.Space(5);

            GUI.backgroundColor = Color.green;
            GUILayout.BeginVertical("box");
            {
                GUI.backgroundColor = Color.white;
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {

                    if(m_useThumbnail)
                    {
                        m_addedSrc = (Sprite)EditorGUILayout.ObjectField("Source", m_addedSrc, typeof(Sprite), false);
                    }
                    else
                    {
                        GUILayout.Label("Source");
                        m_addedSrc = (Sprite)EditorGUILayout.ObjectField(m_addedSrc, typeof(Sprite), false);
                    }
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("->");
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    if(m_useThumbnail)
                    {
                        m_addedTar = (Sprite)EditorGUILayout.ObjectField("Target", m_addedTar, typeof(Sprite), false);
                    }
                    else
                    {
                        GUILayout.Label("Target");                    GUILayout.Label("Target");
                        m_addedTar = (Sprite)EditorGUILayout.ObjectField(m_addedTar, typeof(Sprite), false);
                    }
                    m_addeduseNativeSize = EditorGUILayout.ToggleLeft(contentNs, m_addeduseNativeSize, GUILayout.Width(40));
                    GUI.color = Color.green;
                    if(GUILayout.Button("+", GUILayout.Width(42)))
                    {
                        if(m_addedSrc && m_addedTar)
                        {
                            Sprite key = m_addedSrc;
                            Sprite sp = m_addedTar;
                            bool ns = m_addeduseNativeSize;

                            if(!m_replaceDataDic.ContainsKey(key))
                                m_replaceDataDic.Add(key, new SpriteItem(sp, ns));
                            else
                                EditorUtility.DisplayDialog("提示", "发现重复项 : " + key.name, "确定");

                            m_addedSrc = null;
                            m_addedTar = null;
                            m_addeduseNativeSize = false;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("提示", "无法添加条目(可能是数据缺失?).", "确定");
                        }
                    }
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();

            GUILayout.Space(5);
        }

        private void _draw_option_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("工具设置项");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    m_useThumbnail = EditorGUILayout.ToggleLeft("使用缩略图显示", m_useThumbnail);
                    m_removeItemComf = EditorGUILayout.ToggleLeft("删除条目确认", m_removeItemComf);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private void _draw_dataOption_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("配置管理项");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {

                    if(!string.IsNullOrEmpty(m_targetDirPath) && m_replaceDataDic.Count > 0)
                    {
                        if(GUILayout.Button("保存配置", GUILayout.Height(22)))
                        {
                            string savePath = EditorUtility.SaveFilePanel("保存", "Assets", "SpritesReplaceData", "asset");
                            if(!string.IsNullOrEmpty(savePath))
                            {
                                savePath = savePath.Replace(Application.dataPath, "Assets");
                                SpritesReplaceDataAsset dataAsset = ScriptableObject.CreateInstance<SpritesReplaceDataAsset>();
                                
                                List<Sprite> srcList = new List<Sprite>();
                                List<Sprite> tarList = new List<Sprite>();
                                List<bool> nsList = new List<bool>();

                                foreach(var keyValue in m_replaceDataDic)
                                {
                                    srcList.Add(keyValue.Key);
                                    tarList.Add(keyValue.Value.sprite);
                                    nsList.Add(keyValue.Value.useNativeSize);
                                }

                                dataAsset.targetDirPath = m_targetDirPath;
                                dataAsset.useThumbnail = m_useThumbnail;
                                dataAsset.removeItemComf = m_removeItemComf;

                                dataAsset.srcList = srcList.ToArray();
                                dataAsset.tarList = tarList.ToArray();
                                dataAsset.nsList = nsList.ToArray();

                                AssetDatabase.CreateAsset(dataAsset, savePath);
                                AssetDatabase.Refresh();

                            }
                        }
                    }
                    else
                    {
                        GUI.color = Color.gray;
                        if(GUILayout.Button("保存配置", GUILayout.Height(22)))
                        {
                            //do nothing 
                        }
                        GUI.color = Color.white;
                    }
                    if(GUILayout.Button("加载配置", GUILayout.Height(22)))
                    {
                        string loadPath = EditorUtility.OpenFilePanel("加载", "Assets", "asset");
                        if(!string.IsNullOrEmpty(loadPath))
                        {
                            loadPath = loadPath.Replace(Application.dataPath, "Assets");
                            SpritesReplaceDataAsset dataAsset = AssetDatabase.LoadAssetAtPath<SpritesReplaceDataAsset>(loadPath);
                            if(dataAsset)
                            {
                                m_replaceDataDic.Clear();
                                m_targetDirPath = dataAsset.targetDirPath;
                                m_useThumbnail = dataAsset.useThumbnail;
                                m_removeItemComf = dataAsset.removeItemComf;
                                for(int i = 0; i < dataAsset.srcList.Length; i++)
                                {
                                    m_replaceDataDic.Add(dataAsset.srcList[i], new SpriteItem(dataAsset.tarList[i], dataAsset.nsList[i]));
                                }
                            }
                        }
                    }

                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private void _draw_start_UI()
        {

            GUILayout.BeginHorizontal();
            {
                if(!string.IsNullOrEmpty(m_targetDirPath) && m_replaceDataDic.Count > 0)
                {
                    if(GUILayout.Button("开始替换", GUILayout.Height(36)))
                    {
                        if(EditorUtility.DisplayDialog("提示", "确认开始替换?!", "开始", "取消"))
                        {
                            _start_replaceProcess();
                        }
                    }
                }
                else
                {
                    GUI.color = Color.gray;
                    if(GUILayout.Button("开始替换", GUILayout.Height(36)))
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
                            string exportPath = EditorUtility.SaveFilePanel("导出日志", "Assets", "SpritesReplaceLog", "txt");
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

        private void _try_delListProcess()
        {
            if(m_delSrcList.Count > 0)
            {
                for(int i = 0; i < m_delSrcList.Count; i++)
                {
                    m_replaceDataDic.Remove(m_delSrcList[i]);
                }
                m_delSrcList.Clear();
            }

            //if(m_delTarList.Count > 0)
            //{
            //    for(int i = 0; i < m_delTarList.Count; i++)
            //    {
            //        m_replaceTarList.Remove(m_delTarList[i]);
            //    }
            //    m_delTarList.Clear();
            //}

        }

        private void _start_replaceProcess()
        {
            m_info.Clear();
            _infoAppendLine(0, "ProcessStart >");
            m_cacheHash.Clear();
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
                    {
                        _replaceProcess(asset, 1, infoList[i].path);
                    }
                }
                EditorUtility.ClearProgressBar();
            }
            _infoAppendLine(0, "< ProcessEnd");
        }

        private void _replaceProcess(GameObject asset, int indent, string path)
        {
            string guid = AssetDatabase.AssetPathToGUID(path);
            if(!m_cacheHash.Contains(guid))
            {
                m_cacheHash.Add(guid);
                //
                _infoAppendLine(indent, "Prefab> " + asset.name + "(" + path + ")");
                bool dirty = false;
                int indentNext = indent + 1;
                _subLoopProcess(asset.transform, indentNext, ref dirty);
                _infoAppendLine(indent, "");
                if(dirty)
                {
                    EditorUtility.SetDirty(asset);
                    PrefabUtility.SavePrefabAsset(asset);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        private void _subLoopProcess(Transform transform, int indent, ref bool dirty)
        {
            bool isIns = PrefabUtility.IsPartOfPrefabInstance(transform);
            int indentNext = indent + 1;

            if(isIns)
            {
                bool isModify = _checkModify(transform.gameObject);
                if(!isModify)
                {
                    GameObject linkRoot = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(transform.gameObject);
                    if(linkRoot)
                    {
                        string linkPath = AssetDatabase.GetAssetPath(linkRoot);
                        _replaceProcess(linkRoot, indent, linkPath);
                        return;
                    }
                }
            }

            Image image = transform.GetComponent<Image>();
            if(image)
            {
                _replaceImg(image, indent, ref dirty);
            }

            for(int i = 0; i < transform.childCount; i++)
            {
                Transform sub = transform.GetChild(i);
                _subLoopProcess(sub, indentNext, ref dirty);
            }

        }

        private void _replaceImg(Image image, int indent, ref bool dirty)
        {
            foreach(var keyValue in m_replaceDataDic)
            {
                if(image.sprite == keyValue.Key)
                {
                    bool ns = keyValue.Value.useNativeSize;
                    image.sprite = keyValue.Value.sprite;
                    if(ns)
                        image.SetNativeSize();
                    _infoAppendLine(indent, "Node>" + image.transform.getHierarchyPath() + " :: " + keyValue.Key.name + " -> " + keyValue.Value.sprite.name + (ns ? "(Ns)" : ""));
                    if(!dirty)
                        dirty = true;
                }
            }
        }

        private bool _checkModify(GameObject node)
        {
            List<AddedComponent> addedComponents = PrefabUtility.GetAddedComponents(node);
            if(addedComponents != null && addedComponents.Count > 0)
            {
                return true;
            }
            //逻辑需要检查modify不需要检查AddedGameObject
            //List<AddedGameObject> addedGameObjects = PrefabUtility.GetAddedGameObjects(node);
            //if(addedGameObjects != null && addedGameObjects.Count > 0)
            //{
            //    return true;
            //}
            List<ObjectOverride> objectOverrideList = PrefabUtility.GetObjectOverrides(node);
            if(objectOverrideList != null && objectOverrideList.Count > 0)
            {
                return true;
            }
            return false;
        }

        private void _infoAppendLine(int indent, string info)
        {
            m_info.AppendLine(_getIndent(indent) + info);
        }

        private string _getIndent(int indent)
        {
            string t = string.Empty;
            for(int i = 0; i < indent; i++)
            {
                t += "\t";
            }
            return t;
        }

    }
}


