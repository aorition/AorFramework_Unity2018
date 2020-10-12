using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AorBaseUtility.Extends;
using Framework.Editor;
using Framework.Extends;

namespace Framework.Audio.Editor
{


    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : UnityEditor.Editor
    {


        private static GUIStyle m_dtBtnStyle;
        protected static GUIStyle dtBtnStyle
        {
            get
            {
                if(m_dtBtnStyle == null)
                {
                    m_dtBtnStyle = GUI.skin.GetStyle("label").Clone();
                    m_dtBtnStyle.normal.textColor = Color.gray;
                    m_dtBtnStyle.wordWrap = true;
                }
                return m_dtBtnStyle;
            }
        }

        private AudioManager m_target;
        private void Awake()
        {
            m_target = target as AudioManager;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying || !m_target) return;

            GUILayout.Space(12);

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                draw_clipCacheInfo_UI();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();

            GUILayout.Space(12);
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                draw_accChannelInfo_UI();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();

            GUILayout.Space(12);
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                draw_bgmChannelInfo_UI();
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();

            this.Repaint();
        }

        #region Clip Cache 监视器实现

        //Clip Cache 监视器实现
        private bool m_clipcache_detailed;
        private List<AudioManager.AudioClipKeeper> m_clipKeeperList;
        private Vector2 m_clipcacheSPos = Vector2.zero;
        private void draw_clipCacheInfo_UI()
        {
            if (Event.current.type == EventType.Layout)
            {
                m_clipKeeperList = (List<AudioManager.AudioClipKeeper>)m_target.GetNonPublicField("_clipKeeperList");
            }

            if (m_clipKeeperList != null)
            {
                int l = m_clipKeeperList.Count;
                int m = m_target.AudioClipCacheLimit;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Clip缓存信息:" + l + " / " + m);
                    GUILayout.FlexibleSpace();
                    if (!m_clipcache_detailed)
                    {
                        if (GUILayout.Button("详细", GUILayout.Width(64)))
                        {
                            m_clipcache_detailed = true;
                        }
                    }
                }
                GUILayout.EndHorizontal();
                
                float value = (float)l / m;
                Color c = Color.green;
                if(value > 0.9f)
                {
                    c = Color.red;
                }else if(value > 0.6f)
                {
                    c = Color.yellow;
                }
                GUI.color = c;
                GUILayout.BeginHorizontal();
                {
                    EditorPlusMethods.Draw_ProgressBar(value, (value * 100).ToString("F2") + " %");
                }
                GUILayout.EndHorizontal();
                GUI.color = Color.white;
                
                //显示详细信息
                if (m_clipcache_detailed)
                {
                    GUILayout.Space(5);

                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("Clip 缓存详细信息");
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("X", GUILayout.Width(24)))
                            {
                                m_clipcache_detailed = false;
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(2);

                        m_clipcacheSPos = GUILayout.BeginScrollView(m_clipcacheSPos);
                        {
                            for (int i = 0; i < m_clipKeeperList.Count; i++)
                            {
                                if(i > 0) GUILayout.Space(2);

                                AudioManager.AudioClipKeeper keeper = m_clipKeeperList[i];
                                if (keeper != null)
                                {
                                    if(keeper.clip != null)
                                    {
                                        c = Color.green;
                                        GUI.color = c;
                                        GUILayout.BeginVertical("box");
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                GUILayout.Label(keeper.clip.name);
                                                GUILayout.FlexibleSpace();
                                                GUILayout.Label((keeper.IsPlaying ? "<Playing>" : "") + (keeper.IsDead ? "<Dead>" : ""));
                                            }
                                            GUILayout.EndHorizontal();
                                            GUILayout.BeginHorizontal();
                                            {
                                                if (GUILayout.Button(keeper.LoadPath, dtBtnStyle))
                                                {
                                                    UnityEngine.Object obj = Resources.Load<AudioClip>(keeper.LoadPath);
                                                    if (obj) Selection.activeObject = obj;
                                                }
                                            }
                                            GUILayout.EndHorizontal();

                                            if (!keeper.IsDead)
                                            {
                                                float liveSec = (float)keeper.GetNonPublicField("_liveSeconds");
                                                float SurvivalSec = keeper.SurvivalSeconds;
                                                GUILayout.BeginHorizontal();
                                                {
                                                    EditorPlusMethods.Draw_ProgressBar(liveSec / SurvivalSec, "", 18, 2);
                                                }
                                                GUILayout.EndHorizontal();
                                            }
                                        }
                                        GUILayout.EndVertical();
                                        GUI.color = Color.white;
                                    }
                                    else
                                    {
                                        //无Clip异常
                                        c = Color.red;
                                        GUI.color = c;
                                        GUILayout.BeginHorizontal("box");
                                        {
                                            GUILayout.Label("Cache Clip已被移除 ... ");
                                        }
                                        GUILayout.EndHorizontal();
                                        GUI.color = Color.white;
                                    }
                                }
                                else
                                {
                                    //空异常
                                    c = Color.gray;
                                    GUI.color = c;
                                    GUILayout.BeginHorizontal("box");
                                    {
                                        GUILayout.Label("AudioClipKeeper 为空");
                                    }
                                    GUILayout.EndHorizontal();
                                    GUI.color = Color.white;
                                }
                            }
                        }
                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(5);
                }

            }
            
        }

        #endregion

        #region ACC 通道 监视器实现

        //ACC 通道 监视器实现

        private bool m_acc_detailed;
        private List<AudioSource> m_accChannelList;
        private Vector2 m_accSPos = Vector2.zero;
        private void draw_accChannelInfo_UI()
        {
            if (Event.current.type == EventType.Layout)
            {
                m_accChannelList = (List<AudioSource>)m_target.GetNonPublicField("_ACChannels");
            }

            if (m_accChannelList != null)
            {
                int l = m_accChannelList.Count;
                int m = m_target.ACChannelLimit;

                int u = 0;
                for (int i = 0; i < m_accChannelList.Count; i++)
                {
                    AudioSource sudioSource = m_accChannelList[i];
                    if (sudioSource)
                    {
                        if (sudioSource.clip && sudioSource.isPlaying)
                            u++;
                    }
                }

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("ACC通道信息 : " + u + " / " + l + " / " + m);
                    GUILayout.FlexibleSpace();
                    if (!m_acc_detailed)
                    {
                        if (GUILayout.Button("详细", GUILayout.Width(64)))
                        {
                            m_acc_detailed = true;
                        }
                    }
                }
                GUILayout.EndHorizontal();

                float value = (float)l / m;
                Color c = Color.green;
                if (value > 0.9f)
                {
                    c = Color.red;
                }
                else if (value > 0.6f)
                {
                    c = Color.yellow;
                }
                GUI.color = c;
                GUILayout.BeginHorizontal();
                {
                    EditorPlusMethods.Draw_ProgressBar(value, (value * 100).ToString("F2") + " %");
                }
                GUILayout.EndHorizontal();
                GUI.color = Color.white;

                //显示详细信息
                if (m_acc_detailed)
                {
                    GUILayout.Space(5);

                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("ACC通道详细信息");
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("X", GUILayout.Width(24)))
                            {
                                m_acc_detailed = false;
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(2);

                        m_accSPos = GUILayout.BeginScrollView(m_accSPos);
                        {
                            for (int i = 0; i < m_accChannelList.Count; i++)
                            {
                                if (i > 0) GUILayout.Space(2);

                                AudioSource source = m_accChannelList[i];
                                if (source != null)
                                {
                                    if (source.clip != null)
                                    {
                                        c = Color.cyan;
                                        GUI.color = c;
                                        GUILayout.BeginVertical("box", GUILayout.Height(24));
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                if (source.loop)
                                                {
                                                    GUILayout.Label(source.clip.name + "  [" + source.time.ToString("F3") + "]");
                                                    GUILayout.FlexibleSpace();
                                                    GUILayout.Label("<Loop>");
                                                }
                                                else
                                                {
                                                    float p = source.time / source.clip.length;
                                                    EditorPlusMethods.Draw_ProgressBar(p, source.clip.name);
                                                }
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                        GUILayout.EndVertical();
                                        GUI.color = Color.white;
                                    }
                                    else
                                    {
                                        c = Color.gray;
                                        GUI.color = c;
                                        GUILayout.BeginHorizontal("box", GUILayout.Height(24));
                                        {
                                            GUILayout.FlexibleSpace();
                                            GUILayout.Label("<闲置>");
                                        }
                                        GUILayout.EndHorizontal();
                                        GUI.color = Color.white;
                                    }
                                }
                                else
                                {
                                    //空异常
                                    c = Color.gray;
                                    GUI.color = c;
                                    GUILayout.BeginHorizontal("box", GUILayout.Height(24));
                                    {
                                        GUILayout.FlexibleSpace();
                                        GUILayout.Label("AudioSource为空");
                                        GUILayout.FlexibleSpace();
                                    }
                                    GUILayout.EndHorizontal();
                                    GUI.color = Color.white;
                                }
                            }
                        }
                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(5);
                }

            }

        }

        #endregion

        #region /BMG 通道 监视器实现

        //BMG 通道 监视器实现

        private bool m_bgm_detailed;
        private List<AudioSource> m_bgmChannelList;
        private Vector2 m_bgmSPos = Vector2.zero;
        private void draw_bgmChannelInfo_UI()
        {
            if (Event.current.type == EventType.Layout)
            {
                m_bgmChannelList = (List<AudioSource>)m_target.GetNonPublicField("_BGMChannels");
            }

            if (m_bgmChannelList != null)
            {
                int l = m_bgmChannelList.Count;
                int m = m_target.BGMChannelLimit;

                int u = 0;
                for (int i = 0; i < m_bgmChannelList.Count; i++)
                {
                    AudioSource sudioSource = m_bgmChannelList[i];
                    if (sudioSource)
                    {
                        if (sudioSource.clip && sudioSource.isPlaying)
                            u++;
                    }
                }

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("BGM通道信息 : " + u + " / " + l + " / " + m);
                    GUILayout.FlexibleSpace();
                    if (!m_bgm_detailed)
                    {
                        if (GUILayout.Button("详细", GUILayout.Width(64)))
                        {
                            m_bgm_detailed = true;
                        }
                    }
                }
                GUILayout.EndHorizontal();

                float value = (float)l / m;
                Color c = Color.green;
                if (value > 0.9f)
                {
                    c = Color.red;
                }
                else if (value > 0.6f)
                {
                    c = Color.yellow;
                }
                GUI.color = c;
                GUILayout.BeginHorizontal();
                {
                    EditorPlusMethods.Draw_ProgressBar(value, (value * 100).ToString("F2") + " %");
                }
                GUILayout.EndHorizontal();
                GUI.color = Color.white;

                //显示详细信息
                if (m_bgm_detailed)
                {
                    GUILayout.Space(5);

                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label("BGM通道详细信息");
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("X", GUILayout.Width(24)))
                            {
                                m_bgm_detailed = false;
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(2);

                        m_bgmSPos = GUILayout.BeginScrollView(m_bgmSPos);
                        {
                            for (int i = 0; i < m_bgmChannelList.Count; i++)
                            {
                                if (i > 0) GUILayout.Space(2);

                                AudioSource source = m_bgmChannelList[i];
                                if (source != null)
                                {
                                    if (source.clip != null)
                                    {
                                        c = Color.magenta;
                                        GUI.color = c;
                                        GUILayout.BeginVertical("box", GUILayout.Height(24));
                                        {
                                            GUILayout.BeginHorizontal();
                                            {
                                                if (source.loop)
                                                {
                                                    GUILayout.Label(source.clip.name + "  [" + source.time.ToString("F3") + "]");
                                                    GUILayout.FlexibleSpace();
                                                    GUILayout.Label("<Loop>");
                                                }
                                                else
                                                {
                                                    float p = source.time / source.clip.length;
                                                    EditorPlusMethods.Draw_ProgressBar(p, source.clip.name);
                                                }
                                            }
                                            GUILayout.EndHorizontal();
                                        }
                                        GUILayout.EndVertical();
                                        GUI.color = Color.white;
                                    }
                                    else
                                    {
                                        c = Color.gray;
                                        GUI.color = c;
                                        GUILayout.BeginHorizontal("box", GUILayout.Height(24));
                                        {
                                            GUILayout.FlexibleSpace();
                                            GUILayout.Label("<闲置>");
                                        }
                                        GUILayout.EndHorizontal();
                                        GUI.color = Color.white;
                                    }
                                }
                                else
                                {
                                    //空异常
                                    c = Color.gray;
                                    GUI.color = c;
                                    GUILayout.BeginHorizontal("box", GUILayout.Height(24));
                                    {
                                        GUILayout.FlexibleSpace();
                                        GUILayout.Label("AudioSource为空");
                                        GUILayout.FlexibleSpace();
                                    }
                                    GUILayout.EndHorizontal();
                                    GUI.color = Color.white;
                                }
                            }
                        }
                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();

                    GUILayout.Space(5);
                }

            }

        }
        #endregion
    }
}


