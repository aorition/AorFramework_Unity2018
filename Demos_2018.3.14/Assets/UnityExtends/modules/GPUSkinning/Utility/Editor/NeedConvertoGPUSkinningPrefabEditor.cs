using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEditor;
using Framework.Extends;

namespace Framework.GPUSkinning.Editor
{
    [CustomEditor(typeof(NeedConvertoGPUSkinningPrefab))]
    public class NeedConvertoGPUSkinningPrefabEditor :UnityEditor.Editor
    {

        private static GUIStyle _sTitleStyle;

        protected static GUIStyle sTitleStyle
        {
            get {
                if(_sTitleStyle == null)
                {
                    _sTitleStyle = EditorStyles.largeLabel.Clone();
                    _sTitleStyle.fontSize = 14;
                    _sTitleStyle.fontStyle = FontStyle.Bold;
                }

                return _sTitleStyle;
            }
        }

        private NeedConvertoGPUSkinningPrefab m_target;

        private void Awake()
        {
            m_target = target as NeedConvertoGPUSkinningPrefab;
        }

        public override void OnInspectorGUI()
        {

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                _draw_subTitle_UI("标记Prefab需要转成GPUSkinning预制体");
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                _draw_subTitle_UI("配置项目");
                GUILayout.Space(5);

                this.serializedObject.Update();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("reName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("checkRoot"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("replaceOrignPrefab"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useStandaloneMesh"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mLighting"));

                this.serializedObject.ApplyModifiedProperties();

                /*
                m_target.reName = EditorGUILayout.TextField("重命名", m_target.reName);
                m_target.checkRoot = EditorGUILayout.Toggle("是否检查根节点", m_target.checkRoot);
                m_target.replaceOrignPrefab = EditorGUILayout.Toggle("替换原始Prefab", m_target.replaceOrignPrefab);
                m_target.mLighting = EditorGUILayout.Toggle("是否接受光照", m_target.mLighting);
                m_target.useStandaloneMesh = EditorGUILayout.Toggle("使用独立Mesh数据", m_target.useStandaloneMesh);
                */

                GUILayout.Space(5);
            }
            GUILayout.EndVertical();

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                _draw_subTitle_UI("工具栏");
                GUILayout.Space(5);

                GUI.color = Color.yellow;
                if(GUILayout.Button(m_target.replaceOrignPrefab ? "转换成GPUSinning预制体" : "创建GPUSkinning预制体", GUILayout.Height(26)))
                {
                    GPUSkinningUtility.BakeGPUSkinningPrefab(m_target.gameObject, m_target.reName, m_target.checkRoot, !m_target.replaceOrignPrefab, m_target.mLighting, m_target.useStandaloneMesh);
                }
                GUI.color = Color.white;

                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private void _draw_subTitle_UI(string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(label, sTitleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

    }

}
