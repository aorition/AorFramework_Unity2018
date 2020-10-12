using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AorBaseUtility.Extends;

namespace Framework.FrameAnim.Editor
{
    public class FrameAnimBaseEditor : UnityEditor.Editor
    {
        protected FrameAnimBase m_target;
        protected virtual void Awake()
        {
            m_target = (FrameAnimBase)this.target;
        }

        protected void draw_debugToolUI()
        {

            float t = (float)m_target.GetNonPublicField("m_playTime");
            int i = (int)m_target.GetNonPublicField("m_index");
            int len = (int)m_target.GetNonPublicField("m_FrameLens");
            bool isPlaying = (bool)m_target.GetNonPublicField("m_isPlaying");
            bool isPaused = (bool)m_target.GetNonPublicField("m_isPaused");

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.Label("Debug Tool >>", EditorStyles.boldLabel);
                GUILayout.Space(5);
                //
                GUILayout.BeginVertical("box");
                {
                    GUILayout.Label("Info :");
                    GUILayout.BeginHorizontal();
                    {

                        GUILayout.Label("PlayTime:");
                        GUILayout.Label(t.ToString("F2"));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {

                        GUILayout.Label("PlayFrame:");
                        GUILayout.Label(i.ToString());
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);

                if (isPlaying && !isPaused)
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Pause"))
                        {
                            m_target.Pause();
                        }
                        if (GUILayout.Button("Stop"))
                        {
                            m_target.Stop();
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                }
                else if (isPaused)
                {

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Play"))
                        {
                            m_target.Play();
                        }
                        if (GUILayout.Button("Stop"))
                        {
                            m_target.Stop();
                        }
                    }
                    GUILayout.EndHorizontal();
                    
                    float ns = EditorGUILayout.IntSlider(i, 0, len - 1);
                    if (ns != i)
                    {
                        float nt = ns * 1f / m_target.FPS;
                        m_target.Time = nt;
                    }
                }
                else
                {

                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Play"))
                        {
                            m_target.Play();
                        }
                    }
                    GUILayout.EndHorizontal();

                }

                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }


    }
}
