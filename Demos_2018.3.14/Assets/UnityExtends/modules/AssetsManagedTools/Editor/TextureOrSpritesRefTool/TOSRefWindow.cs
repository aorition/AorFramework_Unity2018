using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using Framework.Extends;
using Framework.Editor;

namespace Framework.Utility.Editor
{

    public class TOSRefWindow : UnityEditor.EditorWindow {

        private static GUIStyle _titleStyle;
        protected static GUIStyle titleStyle {
            get {
                if( _titleStyle == null ) {
                    _titleStyle = EditorStyles.largeLabel.Clone( );
                    _titleStyle.fontSize = 16;
                }
                return _titleStyle;
            }
        }

        //--------------------------------------
        private static TOSRefWindow _instance;
        [MenuItem( "AssetsManagedTools/Bitmaps/Sprite|Texture2D 引用检查工具", false, 10100 )]
        public static TOSRefWindow init () {

            _instance = UnityEditor.EditorWindow.GetWindow<TOSRefWindow>( );
            _instance.minSize = new Vector2( 495, 612 );

            return _instance;
        }

        //--------------------------------------

        private TOSRefCacheAsset m_cache;

        private Vector2 m_scrollPos;
        private Vector2 m_scrollPos2;
        private readonly StringBuilder m_expInfo = new StringBuilder();

        private UnityEngine.Object[] m_selection_cache;
        private UnityEngine.Object[] m_selection_cache2;

        private bool m_refResult = false;
        private readonly Dictionary<Sprite, Dictionary<string, List<string>>> m_refSPDic = new Dictionary<Sprite, Dictionary<string, List<string>>>();
        private readonly Dictionary<Texture, Dictionary<string, List<string>>> m_refTexDic = new Dictionary<Texture, Dictionary<string, List<string>>>();

        private UnityEngine.Object tmp_obj;
        private UnityEngine.GameObject tmp_Go;

        //private bool m_useSelectLib;
        private bool m_needUpdateSelectLib;
        private readonly List<UnityEngine.Object> m_selectLib = new List<UnityEngine.Object>();
        private readonly List<UnityEngine.Object> m_selectDelCache = new List<UnityEngine.Object>();

        //private bool m_selectionContinue;
        
        private void OnGUI()
        {

            if(EditorApplication.isCompiling)
                Close();

            if(!m_cache)
                m_cache = TOSRefCacheAsset.GetAsset();

            GUILayout.Space(15);
            _draw_toolTitle_UI();
            GUILayout.Space(15);
            _draw_cacheInfo_UI();
            _draw_selection_set_UI();
            m_selection_process();
            if(m_refResult)
            {
                GUILayout.Space(15);
                _draw_result_UI();
                GUILayout.FlexibleSpace();
                _draw_exportResult_UI();
            }
            GUILayout.Space(15);
            //EditorPlusMethods.Draw_DebugWindowSizeUI();
            Repaint();
        }

        private void _draw_toolTitle_UI() {
            GUILayout.BeginVertical( "box" );
            GUILayout.Space( 10 );

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label( "      Sprite | Texture2D 引用检查工具      ", titleStyle );
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space( 10 );

            GUILayout.EndVertical();
        }
        private void _draw_cacheInfo_UI() {
            GUILayout.BeginVertical( "box" );
            {
                GUILayout.Space( 5 );
                if( m_cache.CacheIsReady ) {
                    GUILayout.BeginHorizontal( );
                    {
                        GUILayout.Label( "缓存数据Info: 预制体:" + m_cache.PrefabCounts + ", Sprites:" + m_cache.SpriteCounts +
                                         ", 生成时间:" + m_cache.Date );
                        GUILayout.FlexibleSpace( );
                        if( GUILayout.Button( "重新生成缓存数据" ) ) {
                            m_cache.CacheIsReady = false;
                            m_cache_update( );
                        }
                    }
                    GUILayout.EndHorizontal( );
                }
                else {
                    GUILayout.BeginHorizontal( );
                    {
                        GUILayout.Label( "引用缓存数据尚未建立" );
                        GUILayout.FlexibleSpace( );
                        if( GUILayout.Button( "生成缓存数据" ) ) {
                            m_cache_update( );
                        }
                    }
                    GUILayout.EndHorizontal( );
                }
                GUILayout.Space( 5 );
            }
            GUILayout.EndVertical( );
        }
        private void _draw_result_UI()
        {
            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos,"box");
            {
                GUILayout.Space( 10 );

                List<Sprite> spList = new List<Sprite>(m_refSPDic.Keys);
                for (int i = 0; i < spList.Count; i++)
                {
                    if (i > 0) GUILayout.Space(5);
                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.Space(5);

                        GUILayout.BeginVertical("box");
                        {
                            string sp_path = AssetDatabase.GetAssetPath(spList[i]);
                            GUILayout.Label(sp_path);
                            EditorGUILayout.ObjectField(spList[i], typeof(Sprite), false);
                        }
                        GUILayout.EndVertical();
                        
                        GUILayout.Space( 8 );

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(36);
                            GUILayout.BeginVertical();
                            {
                                //List<string> pGuidList = m_refDic[spList[i]];
                                //for (int j = 0; j < pGuidList.Count; j++)
                                Dictionary<string, List<string>> pGuidDic = m_refSPDic[spList[i]];
                                int j = 0;
                                foreach(string guid in pGuidDic.Keys)
                                {
                                    if(j > 0)
                                        GUILayout.Space(2);
                                    string path = AssetDatabase.GUIDToAssetPath(guid);
                                    GUILayout.BeginVertical("box");
                                    {
                                        GUILayout.Label(path);
                                        GUILayout.Space(2);
                                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                                        EditorGUILayout.ObjectField(go, typeof(GameObject), false);

                                        GUILayout.BeginHorizontal();
                                        {
                                            GUILayout.Space(36);
                                            GUILayout.BeginVertical();
                                            {

                                                List<string> hList = pGuidDic[guid];
                                                for(int h = 0; h < hList.Count; h++)
                                                {
                                                    if(h > 0)
                                                        GUILayout.Space(2);
                                                    GUILayout.Label(hList[h]);
                                                }

                                            }
                                            GUILayout.EndVertical();
                                        }
                                        GUILayout.EndHorizontal();

                                    }
                                    GUILayout.EndVertical();
                                    j++;
                                }
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);
                    }
                    GUILayout.EndVertical();
                }

                //---------

                List<Texture> txList = new List<Texture>(m_refTexDic.Keys);
                for(int i = 0; i < txList.Count; i++)
                {
                    if(i > 0)
                        GUILayout.Space(5);
                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.Space(5);

                        GUILayout.BeginVertical("box");
                        {
                            string tx_path = AssetDatabase.GetAssetPath(txList[i]);
                            GUILayout.Label(tx_path);
                            EditorGUILayout.ObjectField(txList[i], typeof(Sprite), false);
                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(8);

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(36);
                            GUILayout.BeginVertical();
                            {
                                Dictionary<string, List<string>> pGuidDic2 = m_refTexDic[txList[i]];
                                int j = 0;
                                foreach(string guid in pGuidDic2.Keys)
                                {
                                    if(j > 0)
                                        GUILayout.Space(2);

                                    string path = AssetDatabase.GUIDToAssetPath(guid);
                                    GUILayout.BeginVertical("box");
                                    {
                                        GUILayout.Label(path);
                                        GUILayout.Space(2);
                                        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                                        EditorGUILayout.ObjectField(go, typeof(GameObject), false);

                                        GUILayout.BeginHorizontal();
                                        {
                                            GUILayout.Space(36);
                                            GUILayout.BeginVertical();
                                            {

                                                List<string> hList = pGuidDic2[guid];
                                                for(int h = 0; h < hList.Count; h++)
                                                {
                                                    if(h > 0)
                                                        GUILayout.Space(2);
                                                    GUILayout.Label(hList[h]);
                                                }

                                            }
                                            GUILayout.EndVertical();
                                        }
                                        GUILayout.EndHorizontal();

                                    }
                                    GUILayout.EndVertical();
                                    j++;
                                }
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(5);
                    }
                    GUILayout.EndVertical();
                }

                GUILayout.Space( 10 );
            }
            GUILayout.EndScrollView();
        }
        private void _draw_exportResult_UI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                if(GUILayout.Button("导出查询日志"))
                {
                    string savePath = EditorUtility.SaveFilePanel("保存日志", "Assets", "XSRFindedLog", "txt");
                    if(!string.IsNullOrEmpty(savePath))
                        m_exportLog(savePath);
                }
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }
        private void _draw_selection_set_UI()
        {
            GUI.backgroundColor = Color.cyan;
            GUILayout.BeginVertical("box");
            {
                GUI.backgroundColor = Color.white;

                GUILayout.Space(5);

                if(m_selectLib.Count > 0)
                {
                    m_scrollPos2 = GUILayout.BeginScrollView(m_scrollPos2);
                    {
                        GUILayout.Space(5);
                        int len = m_selectLib.Count;
                        for(int i = 0; i < len; i += 3)
                        {

                            if(i > 0)
                                GUILayout.Space(2);

                            GUILayout.BeginHorizontal();
                            {

                                if(i < len)
                                {
                                    GUILayout.BeginHorizontal("box");
                                    {
                                        GUILayout.Label(m_selectLib[i].name);
                                        if(GUILayout.Button("-", GUILayout.Width(22)))
                                        {
                                            m_selectDelCache.Add(m_selectLib[i]);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }

                                if((i + 1) < len)
                                {
                                    GUILayout.BeginHorizontal("box");
                                    {
                                        GUILayout.Label(m_selectLib[i + 1].name);
                                        if(GUILayout.Button("-", GUILayout.Width(22)))
                                        {
                                            m_selectDelCache.Add(m_selectLib[i + 1]);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }

                                if((i + 2) < len)
                                {
                                    GUILayout.BeginHorizontal("box");
                                    {
                                        GUILayout.Label(m_selectLib[i + 2].name);
                                        if(GUILayout.Button("-", GUILayout.Width(22)))
                                        {
                                            m_selectDelCache.Add(m_selectLib[i + 2]);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
                            GUILayout.EndHorizontal();

                        }

                        GUILayout.Space(5);
                    }
                    GUILayout.EndScrollView();
                }

                GUILayout.Space(5);
                
                GUILayout.BeginHorizontal("box");
                {
                    if(m_selectLib.Count > 0)
                    {
                        GUI.color = Color.red;
                        if(GUILayout.Button("清空锁定选择集(Clear)"))
                        {
                            if(EditorUtility.DisplayDialog("提示", "确定清空锁定选择集?", "确定", "取消"))
                            {
                                m_selection_cache2 = null;
                                m_selectLib.Clear();
                                m_needUpdateSelectLib = true;
                            }
                        }
                        GUI.color = Color.white;
                    }
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.green;
                    if(GUILayout.Button("当前选择加入到锁定选择集(+)"))
                    {
                        if(Selection.objects != null && Selection.objects.Length > 0)
                        {
                            m_selectLib.AddRange(Selection.objects);
                            m_needUpdateSelectLib = true;
                        }
                    }
                    GUI.color = Color.white;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
            }
            GUILayout.EndVertical();

            if(m_selectDelCache.Count > 0)
            {
                foreach(var del in m_selectDelCache)
                {
                    m_selectLib.Remove(del);
                }
                m_selectDelCache.Clear();
                m_needUpdateSelectLib = true;
            }

        }
        //======================================

        private void m_selection_process()
        {

            if(m_cache == null || !m_cache.CacheIsReady)
                return;

            bool needUpdate = false;
            //if(m_useSelectLib)
            //{
            //    if(m_needUpdateSelectLib || m_selection_cache == null)
            //    {
            //        m_selection_cache = m_selectLib.ToArray();
            //        needUpdate = true;
            //        //
            //        m_needUpdateSelectLib = false;
            //    }

            //    if(m_selection_cache2 != Selection.objects)
            //    {
            //        m_selection_cache2 = Selection.objects;
            //        needUpdate = true;
            //    }

            //}
            //else
            //{
            //    if(m_selection_cache != Selection.objects)
            //    {
            //        m_selection_cache = Selection.objects;
            //        needUpdate = true;
            //    }
            //}

            if(m_needUpdateSelectLib || m_selection_cache == null)
            {
                m_selection_cache = m_selectLib.ToArray();
                needUpdate = true;
                //
                m_needUpdateSelectLib = false;
            }

            if(m_selection_cache2 != Selection.objects)
            {
                m_selection_cache2 = Selection.objects;
                needUpdate = true;
            }

            if (needUpdate)
            {
                if( m_selection_cache == null)
                {
                    m_refSPDic.Clear();
                    m_refTexDic.Clear();
                    m_refResult = false;
                    return;
                }

                List<Sprite> sp_removeList = new List<Sprite>(m_refSPDic.Keys);
                List<Sprite> sp_tarList = new List<Sprite>();
                List<Texture> tx_removeList = new List<Texture>(m_refTexDic.Keys);
                List<Texture> tx_tarList = new List<Texture>();

                foreach (UnityEngine.Object o in m_selection_cache)
                {
                    if (o is Sprite)
                    {
                        Sprite s = (Sprite) o;
                        sp_tarList.Add(s);
                        if (sp_removeList.Contains(s))
                            sp_removeList.Remove(s);
                    }else if(o is Texture)
                    {
                        Texture t = (Texture) o;
                        tx_tarList.Add(t);
                        if(tx_removeList.Contains(t))
                            tx_removeList.Remove(t);
                    }

                }

                //if(m_selectionContinue)
                //{
                    foreach(UnityEngine.Object o in m_selection_cache2)
                    {
                        if(o is Sprite)
                        {
                            Sprite s = (Sprite)o;
                            sp_tarList.Add(s);
                            if(sp_removeList.Contains(s))
                                sp_removeList.Remove(s);
                        }
                        else if(o is Texture)
                        {
                            Texture t = (Texture)o;
                            tx_tarList.Add(t);
                            if(tx_removeList.Contains(t))
                                tx_removeList.Remove(t);
                        }

                    }
                //}
                
                //--- Sprite
                if(sp_tarList.Count > 0)
                {
                    for (int i = 0; i < sp_tarList.Count; i++)
                    {
                        m_checkRefBySprite(sp_tarList[i]);
                    }
                }

                if (sp_removeList.Count > 0)
                {
                    for (int i = 0; i < sp_removeList.Count; i++)
                    {
                        m_refSPDic.Remove(sp_removeList[i]);
                    }
                }

                //--- Textrue
                if(tx_tarList.Count > 0)
                {
                    for(int i = 0; i < tx_tarList.Count; i++)
                    {
                        m_checkRefByTexture(tx_tarList[i]);
                    }
                }

                if(tx_removeList.Count > 0)
                {
                    for(int i = 0; i < tx_removeList.Count; i++)
                    {
                        m_refTexDic.Remove(tx_removeList[i]);
                    }
                }
                //---

                if(m_refSPDic.Count > 0 || m_refTexDic.Count > 0)
                    m_refResult = true;
                else
                    m_refResult = false;

            }

        }
        private void m_checkRefBySprite(Sprite sp)
        {
            if (!m_refSPDic.ContainsKey(sp))
            {
                m_refSPDic.Add(sp, new Dictionary<string, List<string>>());
                string ref_sGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sp)) + "@" + sp.name;
                for (int i = 0; i < m_cache.PrefabItems.Count; i++)
                {
                    TOS_Prefab_CacheItem item = m_cache.PrefabItems[i];
                    for (int j = 0; j < item.sp_guidAddNameDependencies.Count; j++)
                    {
                        string tar_sGUID = item.sp_guidAddNameDependencies[j];
                        if (ref_sGUID == tar_sGUID)
                        {
                            string guid = m_cache.Prefab_GUIDs[i];
                            string hierarchyPath = item.sp_hierarchies[j];
                            if(!m_refSPDic[sp].ContainsKey(guid))
                                m_refSPDic[sp].Add(guid, new List<string>());

                            m_refSPDic[sp][guid].Add(hierarchyPath);
                        }
                    }
                }

            }
        }
        private void m_checkRefByTexture(Texture tex)
        {
            if(!m_refTexDic.ContainsKey(tex))
            {
                m_refTexDic.Add(tex, new Dictionary<string, List<string>>());
                string ref_sGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(tex));
                for(int i = 0; i < m_cache.PrefabItems.Count; i++)
                {
                    TOS_Prefab_CacheItem item = m_cache.PrefabItems[i];
                    for(int j = 0; j < item.tx_guidAddNameDependencies.Count; j++)
                    {
                        string tar_sGUID = item.tx_guidAddNameDependencies[j];
                        if(ref_sGUID == tar_sGUID)
                        {
                            string guid = m_cache.Prefab_GUIDs[i];
                            string hierarchyPath = item.tx_hierarchies[j];
                            if(!m_refTexDic[tex].ContainsKey(guid))
                                m_refTexDic[tex].Add(guid, new List<string>());

                            m_refTexDic[tex][guid].Add(hierarchyPath);
                        }
                    }
                }

            }
        }
        private void m_cache_update()
        {
            if( m_cache == null || m_cache.CacheIsReady )
                return;

            m_cache.Reset();

            List<EditorAssetInfo> assetInfoList = EditorAssetInfo.FindEditorAssetInfoInPath(Application.dataPath).filter(
                (inf) =>
                {
                    return inf.suffix != ".meta"
                           && inf.suffix != ".dll"
                           && inf.suffix != ".cs"
                        ;
                });

            List<string> m_prefabPaths = new List<string>();

            if (assetInfoList != null && assetInfoList.Count > 0)
            {
                for (int i = 0; i < assetInfoList.Count; i++)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("..", "...", (float) i / assetInfoList.Count))
                    {
                        EditorUtility.ClearProgressBar();
                        return;
                    }
                    EditorAssetInfo assetInfo = assetInfoList[i];
                    string guid = AssetDatabase.AssetPathToGUID(assetInfo.path);

                    tmp_obj = null;
                    tmp_obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetInfo.path);

                    if ( tmp_obj == null)
                        continue;

                    if ( tmp_obj is UnityEngine.GameObject )
                    {
                        m_prefabPaths.Add(assetInfo.path);
                    }
                    else if ( tmp_obj is UnityEngine.Texture )
                    {
                        UnityEngine.Object [ ] sps = AssetDatabase.LoadAllAssetsAtPath( assetInfo.path );
                        if( sps != null && sps.Length > 0 ) {
                            foreach( UnityEngine.Object sp in sps ) {
                                if( sp is Sprite )
                                {
                                    string sGUID = guid + "@" + sp.name;
                                    m_cache.AddSpriteData(sGUID, new TOS_Sprite_CacheItem(sp.name));
                                }
                            }
                        }
                    }

                }
            }

            EditorUtility.ClearProgressBar();

            if (m_prefabPaths.Count > 0)
            {
                //foreach (string path in m_prefabPaths)
                for (int i = 0; i < m_prefabPaths.Count; i++)
                {

                    string path = m_prefabPaths[i];

                    if( EditorUtility.DisplayCancelableProgressBar( "..", "...", ( float ) i / assetInfoList.Count ) ) {
                        EditorUtility.ClearProgressBar( );
                        return;
                    }

                    tmp_Go = null;
                    tmp_Go = AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(path);

                    if( tmp_Go == null )
                        continue;

                    string guid = AssetDatabase.AssetPathToGUID(path);
                    List<string> sp_depens = new List<string>();
                    List<string> sp_hierarchies = new List<string>();
                    List<string> tx_depens = new List<string>();
                    List<string> tx_hierarchies = new List<string>();
                    m_subProcessLoop(tmp_Go.transform, (img) =>
                    {
                        if (img && img.sprite)
                        {
                            string sPath = AssetDatabase.GetAssetPath(img.sprite);
                            string guidAddName = AssetDatabase.AssetPathToGUID(sPath) + "@" + img.sprite.name;
                            string hierarchyPath = img.transform.GetHierarchyPath();
                            sp_depens.Add(guidAddName);
                            sp_hierarchies.Add(hierarchyPath);
                            TOS_Sprite_CacheItem item = m_cache.GetSpriteItemData(guidAddName);
                            if (item != null)
                                item.RefCounts += 1;
                        }
                    }, (rawImg) => 
                    {
                        if(rawImg && rawImg.texture)
                        {
                            string tx_Path = AssetDatabase.GetAssetPath(rawImg.texture);
                            string tx_guid = AssetDatabase.AssetPathToGUID(tx_Path);
                            string hierarchyPath = rawImg.transform.GetHierarchyPath();
                            tx_depens.Add(tx_guid);
                            tx_hierarchies.Add(hierarchyPath);
                            TOS_Texture_CacheItem item = m_cache.GetTextureItemData(tx_guid);
                            if(item != null)
                                item.RefCounts += 1;
                        }
                    });
                    m_cache.AddPrefabData(guid, new TOS_Prefab_CacheItem(tmp_Go.name, sp_depens, sp_hierarchies, tx_depens, tx_hierarchies));
                }
            }
            m_cache.Date = createDateString();
            m_cache.CacheIsReady = true;
            m_cache.SetDirty();
            EditorUtility.ClearProgressBar( );
        }
        private void m_subProcessLoop(Transform t, Action<Image> finedSpDo, Action<RawImage> finedTxDo)
        {
            Image img = t.GetComponent<Image>();
            if (img) finedSpDo(img);

            RawImage rawImg = t.GetComponent<RawImage>();
            if(rawImg) finedTxDo(rawImg);

            if (t.childCount > 0)
            {
                for (int i = 0; i < t.childCount; i++)
                {
                    Transform subT = t.GetChild(i);
                    m_subProcessLoop(subT, finedSpDo, finedTxDo);
                }
            }
        }
        private string createDateString()
        {
            DateTime dt = DateTime.Now;
            return dt.ToShortDateString() + "  " + dt.ToShortTimeString();
        }
        private void m_exportLog(string savePath)
        {
            m_expInfo.Clear();
            int indent = 0;
            _infoAppendLine(indent, "查询日志:" + createDateString() + ">");

            indent++;
            foreach(var keyValue in m_refSPDic)
            {
                string path = AssetDatabase.GetAssetPath(keyValue.Key);
                _infoAppendLine(indent, "Sprite > " + keyValue.Key.name + "(" + path + ")");
                Dictionary<string, List<string>> rDic = keyValue.Value;
                indent++;
                foreach(string guid in rDic.Keys)
                {
                    string p = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                    _infoAppendLine(indent, "Prefab > " + go.name + "(" + p + ")");

                    indent++;
                    List<string> hList = rDic[guid];
                    for(int h = 0; h < hList.Count; h++)
                    {
                        _infoAppendLine(indent, "Node > " + hList[h]);
                    }
                    indent--;
                }
                indent--;
                _infoAppendLine(indent, " ");
            }
            indent--;
            _infoAppendLine(indent, " ");

            File.WriteAllText(savePath, m_expInfo.ToString());
        }
        private void _infoAppendLine(int indent, string info)
        {
            m_expInfo.AppendLine(_getIndent(indent) + info);
        }
        protected string _getIndent(int indent)
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
