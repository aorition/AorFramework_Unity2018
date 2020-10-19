using System;
using UnityEngine;
using UnityEditor;

namespace Framework.GPUSkinning.Editor
{
    [CustomEditor(typeof(GPUSkinAnimDebugTool))]
    public class GPUSkinAnimDebugToolEditor :UnityEditor.Editor
    {

        private GPUSkinAnimDebugTool m_target;

        private void Awake()
        {
            m_target = target as GPUSkinAnimDebugTool;
        }

        private int m_animNameIndex = -1;
        private string m_selectAnimName;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(!m_target || !m_target.isInit || !m_target.texturen)
                return;

            if(!Application.isPlaying)
            {
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUI.color = Color.yellow;
                        GUILayout.Label("此工具需要编辑器运行时");
                        GUI.color = Color.white;
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                }
                GUILayout.EndVertical();
                return;
            }

            if(!m_target.UseScenePanel)
            {
                GUILayout.Space(12);
                _draw_mainToolUI();
            }
        }

        private Rect GetMainUIRect()
        {
            float w = 180;
            float h = 220;
            float w2 = 10;
            float h2 = 20;

            return new Rect(
                    (float)Screen.width - w - w2,
                    ((float)Screen.height - h) / 2,
                    w - w2 / 2,
                    h - h2 / 2
                );
            ;

        }

        private void OnSceneGUI()
        {
            if(!m_target || !m_target.isInit || !m_target.texturen)
                return;

            if(Application.isPlaying && m_target.UseScenePanel)
            {
                Rect rect = GetMainUIRect();
                GUI.BeginGroup(rect);
                {
                    _draw_mainToolUI(rect.width, rect.height);
                }
                GUI.EndGroup();
            }
        }

        private void _draw_mainToolUI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);

                _draw_toolCoreUI();

                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private void _draw_mainToolUI(float w, float h)
        {
            GUILayout.BeginVertical("box", GUILayout.Width(w), GUILayout.Height(h));
            {
                GUILayout.Space(5);

                _draw_toolCoreUI();

                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private void _draw_toolCoreUI()
        {

            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("GPUSkin Anim DebugTool");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            m_animNameIndex = EditorGUILayout.Popup(m_animNameIndex, m_target.texturen.frameNames);

            if(m_animNameIndex != -1)
            {
                GUI.color = Color.yellow;
                if(GUILayout.Button("Play", GUILayout.Height(26)))
                {
                    m_target.Play(m_target.texturen.frameNames[m_animNameIndex]);
                }
                GUI.color = Color.white;
            }
            else
            {
                GUI.color = Color.gray;
                if(GUILayout.Button("Play", GUILayout.Height(26)))
                {
                    //
                }
                GUI.color = Color.white;
            }
            if(m_target.isPlaying)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Info");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Space(5);
                    GUILayout.Label("Frame Name : " + m_target.m_currentAnimName);
                    GUILayout.Space(5);
                    GUILayout.Label("Frame : " + m_target.m_frame);
                    GUILayout.Space(5);
                }
                GUILayout.EndVertical();
            }

        }
    }
}

