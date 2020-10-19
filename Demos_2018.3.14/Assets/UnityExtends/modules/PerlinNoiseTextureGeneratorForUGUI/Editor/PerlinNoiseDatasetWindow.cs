using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Framework.Editor
{
    public class PerlinNoiseDatasetWindow :UnityEditor.EditorWindow
    {

        private static GUIContent m_windowTitle;
        protected static GUIContent mWindowTitle 
        {
            get {
                if(m_windowTitle == null)
                {
                    m_windowTitle = new GUIContent("PerlinNoiseGen");
                }
                return m_windowTitle;
            }
        }

        private const int OPBTNWIDTH = 36;
        private const int OPTLWIDTH = 36;
        private const int SEEDRANDOMLIMIT = 999999999;
        private const int BTNHEIGHT = 26;

        private static PerlinNoiseDatasetWindow _instance;
        public static PerlinNoiseDatasetWindow init(PerlinNoiseTexGenerator target)
        {
            _instance = UnityEditor.EditorWindow.GetWindow<PerlinNoiseDatasetWindow>();
            _instance.titleContent = mWindowTitle;
            _instance.Setup(target);
            return _instance;
        }

        //------------------------------------------

        private bool m_isInit;
        private PerlinNoiseTexGenerator m_target;
        private Vector2 m_scrollPos;
        private readonly List<PerlinNoiseDataWraper> m_deleteList = new List<PerlinNoiseDataWraper>();

        private int m_swap_a = 0;
        private int m_swap_b = 0;

        public void Setup(PerlinNoiseTexGenerator target)
        {
            m_target = target;
            m_isInit = true;
        }

        private void Awake()
        {
            //
        }

        private void OnDestroy()
        {
            m_target = null;
            m_isInit = false;
        }

        private void OnGUI()
        {

            if(!m_target || !m_isInit)
            {
                this.Close();
                return;
            }

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("PerlinNoise Generator 设置器");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(2);
                if(!m_target.texture)
                {
                    int nGTSize = EditorGUILayout.IntField("GenTexSize", m_target.GenTexSizeWidth);
                    if(nGTSize != m_target.GenTexSizeWidth)
                    {
                        m_target.GenTexSizeWidth = nGTSize;
                        m_target.GenTexSizeHeight = nGTSize;
                    }

                }
                m_target.Offset = EditorGUILayout.Vector2Field("Global Offset", m_target.Offset);
                m_target.BlurSize = EditorGUILayout.IntSlider("Blur Size", m_target.BlurSize, 0, 16);
                GUILayout.Space(2);
            }
            GUILayout.EndVertical();

            if(m_target.dataWraperList != null && m_target.dataWraperList.Count > 0)
            {
                m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, "box");
                {
                    GUILayout.Space(5);

                    for(int i = 0; i < m_target.dataWraperList.Count; i++)
                    {

                        if(i > 0)
                            GUILayout.Space(2);

                        GUILayout.BeginVertical("box");
                        {
                            //--------------------------------------------
                            PerlinNoiseDataWraper dataWraper = m_target.dataWraperList[i];
                            bool dirty = false;

                            GUILayout.Space(2);

                            bool n_enabled;
                            GUILayout.BeginHorizontal("box");
                            {

                                n_enabled = EditorGUILayout.ToggleLeft("Enabled", dataWraper.enabled);
                                if(n_enabled != dataWraper.enabled)
                                    dirty = true;

                                GUILayout.FlexibleSpace();

                                GUI.backgroundColor = Color.cyan;
                                GUILayout.BeginHorizontal("box", GUILayout.Width(Screen.width * 0.3f));
                                {
                                    GUI.backgroundColor = Color.white;
                                    GUILayout.Label("Index");
                                    GUILayout.Label(i.ToString());
                                }
                                GUILayout.EndHorizontal();

                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(2);

                            float n_scale = EditorGUILayout.FloatField("Scale", dataWraper.scale);
                            if(!n_scale.Equals(dataWraper.scale))
                                dirty = true;

                            int n_octaves = EditorGUILayout.IntField("Octaves", dataWraper.octaves);
                            if(!n_octaves.Equals(dataWraper.octaves))
                                dirty = true;

                            float n_persistance = EditorGUILayout.FloatField("Persistance", dataWraper.persistance);
                            if(!n_persistance.Equals(dataWraper.persistance))
                                dirty = true;

                            float n_lacunarity = EditorGUILayout.FloatField("Lacunarity", dataWraper.lacunarity);
                            if(!n_lacunarity.Equals(dataWraper.lacunarity))
                                dirty = true;

                            Vector2 n_offset = EditorGUILayout.Vector2Field("Offset", dataWraper.offset);
                            if(!n_offset.Equals(dataWraper.offset))
                                dirty = true;

                            PerlinNoiseDataBlendMethod n_blendMethod = (PerlinNoiseDataBlendMethod)EditorGUILayout.EnumPopup("BlendMethod", dataWraper.blendMethod);
                            if(!n_blendMethod.Equals(dataWraper.blendMethod))
                                dirty = true;

                            bool n_R, n_G, n_B, n_A;
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();
                                n_R = EditorGUILayout.ToggleLeft("R", dataWraper.R, GUILayout.MinWidth(OPTLWIDTH));
                                GUILayout.FlexibleSpace();
                                if(!n_R.Equals(dataWraper.R))
                                    dirty = true;
                                n_G = EditorGUILayout.ToggleLeft("G", dataWraper.G, GUILayout.MinWidth(OPTLWIDTH));
                                GUILayout.FlexibleSpace();
                                if(!n_G.Equals(dataWraper.G))
                                    dirty = true;
                                n_B = EditorGUILayout.ToggleLeft("B", dataWraper.B, GUILayout.MinWidth(OPTLWIDTH));
                                GUILayout.FlexibleSpace();
                                if(!n_B.Equals(dataWraper.B))
                                    dirty = true;
                                n_A = EditorGUILayout.ToggleLeft("A", dataWraper.A, GUILayout.MinWidth(OPTLWIDTH));
                                if(!n_A.Equals(dataWraper.A))
                                    dirty = true;
                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                            if(dirty)
                            {
                                m_target.dataWraperList[i] = new PerlinNoiseDataWraper(dataWraper.seed, n_scale, n_octaves, n_persistance, n_lacunarity, n_offset, n_R, n_G, n_B, n_A, n_blendMethod, n_enabled);
                            }

                            //--------------------------------------------
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.FlexibleSpace();

                                if(GUILayout.Button("SOLO", GUILayout.Width(OPBTNWIDTH * 1.50f)))
                                {
                                    m_target.TryBuildingTexture((tex) => {
                                        m_target.BuildingPerlinNoise(tex, i);
                                    });
                                }
                                if(GUILayout.Button("CP", GUILayout.Width(OPBTNWIDTH)))
                                {
                                    PerlinNoiseDataWraper current = m_target.dataWraperList[i];
                                    m_target.dataWraperList.Add(current.Copy());
                                }
                                if(GUILayout.Button("-", GUILayout.Width(OPBTNWIDTH)))
                                {
                                    m_deleteList.Add(m_target.dataWraperList[i]);
                                }
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(2);

                        }
                        GUILayout.EndVertical();

                    }

                    GUILayout.Space(5);
                }
                GUILayout.EndScrollView();

                if(m_target.dataWraperList.Count > 1)
                {

                    GUILayout.BeginHorizontal("box");
                    {
                        m_swap_a = EditorGUILayout.IntField(m_swap_a);
                        GUILayout.Label(" < Index > ");
                        m_swap_b = EditorGUILayout.IntField(m_swap_b);
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button("SWAP"))
                        {
                            if(m_swap_a >= 0 && m_swap_a < m_target.dataWraperList.Count
                                && m_swap_b >= 0 || m_swap_b < m_target.dataWraperList.Count
                                && m_swap_a != m_swap_b
                                )
                            {
                                PerlinNoiseDataWraper tmp = m_target.dataWraperList[m_swap_a];
                                m_target.dataWraperList[m_swap_a] = m_target.dataWraperList[m_swap_b];
                                m_target.dataWraperList[m_swap_b] = tmp;
                                m_swap_a = 0;
                                m_swap_b = 0;
                                GUI.FocusControl(null);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();

                }

            }
            GUILayout.BeginHorizontal("box");
            {
                GUILayout.FlexibleSpace();
                if(GUILayout.Button("+", GUILayout.Width(OPBTNWIDTH)))
                {
                    int seed = UnityEngine.Random.Range(0, SEEDRANDOMLIMIT);
                    PerlinNoiseDataWraper perlinNoiseDataWraper = new PerlinNoiseDataWraper(seed);
                    if(m_target.dataWraperList == null)
                        m_target.dataWraperList = new List<PerlinNoiseDataWraper>();
                    m_target.dataWraperList.Add(perlinNoiseDataWraper);
                }
            }
            GUILayout.EndHorizontal();

            if(m_deleteList.Count > 0)
            {
                for(int i = 0; i < m_deleteList.Count; i++)
                {
                    m_target.dataWraperList.Remove(m_deleteList[i]);
                }
                m_deleteList.Clear();
            }

            GUILayout.Space(24);

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            {
                if(GUILayout.Button("Building PerlinNoise Texture", GUILayout.Height(BTNHEIGHT)))
                {
                    m_target.TryBuildingTexture((tex) => {
                        m_target.BuildingPerlinNoise(tex);
                    });
                }
                if(m_target.texture)
                {
                    if(GUILayout.Button("Clear Texture", GUILayout.Width(100), GUILayout.Height(BTNHEIGHT)))
                    {
                        m_target.ClearTexture();
                    }
                }
            }
            GUILayout.EndHorizontal();

        }

    }
}
