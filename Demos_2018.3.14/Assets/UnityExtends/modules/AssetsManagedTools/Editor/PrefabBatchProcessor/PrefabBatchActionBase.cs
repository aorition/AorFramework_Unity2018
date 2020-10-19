using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEditor.SceneManagement;
using Framework.Extends;
using AorBaseUtility.Extends;

namespace Framework.Utility.Editor
{
    public class PrefabBatchActionBase : ScriptableObject
    {

        private static GUIStyle _actionDesStyle;
        protected static GUIStyle actionDesStyle
        {
            get {
                if(_actionDesStyle == null)
                {
                    _actionDesStyle = EditorStyles.label.Clone();
                    _actionDesStyle.wordWrap = true;
                }
                return _actionDesStyle;
            }
        }

        public virtual string DesInfo
        {
            get {
                return "(无描述)";
            }
        }

        protected StringBuilder m_info;

        protected readonly HashSet<string> m_cacheHash = new HashSet<string>();

        /// <summary>
        /// 设置日志依赖StringBuilder
        /// </summary>
        public void SetInfoBuilder(StringBuilder stringBuilder)
        {
            m_info = stringBuilder;
            m_info.Clear();
        }

        public virtual void Init()
        {
            m_cacheHash.Clear();
        }

        /// <summary>
        ///  动作行为描述UI绘制 （默认行为：居中显示DesInfo属性）
        /// </summary>
        public virtual void DescriptionDraw()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(DesInfo, actionDesStyle);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 动作行为附带设置UI绘制
        /// </summary>
        public virtual void ActionInspectorDraw()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("(无设置)");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 主入口
        /// </summary>
        public virtual void Process(GameObject asset)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            _prefabProcess(asset, 1, path);
        }

        protected void _prefabProcess(GameObject asset, int indent, string path)
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
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        protected void _subLoopProcess(Transform transform, int indent, ref bool dirty)
        {
            bool isIns = PrefabUtility.IsPartOfPrefabInstance(transform);

            if(isIns)
            {
                bool isModify = _checkModify(transform.gameObject);
                if(!isModify)
                {
                    GameObject linkRoot = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(transform.gameObject);
                    if(linkRoot)
                    {
                        string linkPath = AssetDatabase.GetAssetPath(linkRoot);
                        _prefabProcess(linkRoot, indent, linkPath);
                        return;
                    }
                }
            }

            _foreachTransformProcess(transform, indent, ref dirty);

            int indentNext = indent + 1;
            for(int i = 0; i < transform.childCount; i++)
            {
                Transform sub = transform.GetChild(i);
                _subLoopProcess(sub, indentNext, ref dirty);
            }

        }

        /// <summary>
        ///  对遍历到的每一个Transform对象进行处理
        /// </summary>
        protected virtual void _foreachTransformProcess(Transform transform, int indent, ref bool dirty)
        {
            //To do ...
        }

        /// <summary>
        /// 检查资源对象的节点是否被修改过（是否是原始引用）
        /// </summary>
        protected virtual bool _checkModify(GameObject node)
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

        /// <summary>
        /// 写入一行日志字符串
        /// </summary>
        /// <param name="indent">间隔</param>
        /// <param name="info">字符串</param>
        protected void _infoAppendLine(int indent, string info)
        {
            m_info.AppendLine(indent, info);
        }

        /// <summary>
        /// 尝试标脏
        /// </summary>
        protected void _trySetDirty(ref bool dirty)
        {
            if(!dirty) dirty = true;
        }

    }
}
