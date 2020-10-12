using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AorBaseUtility.Extends;
using UnityEngine.UI;
using Framework.Editor;

namespace Framework.FrameAnim.Editor
{
    [CustomEditor(typeof(RawImageFrameAnim))]
    public class RawImageFrameAnimEditor : FrameAnimBaseEditor
    {

        private RawImageFrameAnim m_proTarget;
        protected override void Awake()
        {
            base.Awake();
            m_proTarget = (RawImageFrameAnim)m_target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //-------------------------

            if (m_proTarget.enabled)
            {

                Texture2D tex = (Texture2D)m_proTarget.GetNonPublicField("m_FramesTexture");
                if (tex && m_proTarget.GridU > 0 && m_proTarget.GridV > 0)
                {
                    GUILayout.Space(12);

                    GUILayout.BeginVertical("box");
                    {
                        GUILayout.Space(5);
                        GUILayout.Label("Tool >>", EditorStyles.boldLabel);
                        GUILayout.Space(5);
                        
                        GUILayout.BeginVertical("box");
                        {

                            GUILayout.Label("Info >");

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Animation Length : (" + m_proTarget.Length + " sec.)");
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Label("Texture Size : (" + tex.width + "," + tex.height + ")");
                                Vector2 cullsize = m_proTarget.GetCullSize();
                                GUILayout.Label("Cull Size : (" + (int)cullsize.x + "," + (int)cullsize.y + ")");
                            }
                            GUILayout.EndHorizontal();

                            string ORGINALSIZE_string = MetaUserDataUtility.TrygetUserDataByTag(tex, MetaUserDataUtility.USERDATA_ORGINALSIZE);
                            if (!string.IsNullOrEmpty(ORGINALSIZE_string))
                            {
                                string[] osp = ORGINALSIZE_string.Split(',');
                                int ow = int.Parse(osp[0]);
                                int oh = int.Parse(osp[1]);
                                if (!ow.Equals(tex.width) || !oh.Equals(tex.height))
                                {
                                    GUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Label("Orginal Size : (" + ORGINALSIZE_string + ")");
                                        GUILayout.Label("Orginal Cull Size : (" + Mathf.FloorToInt(ow / m_proTarget.GridU) + "," + Mathf.FloorToInt(oh / m_proTarget.GridV) + ")");
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }

                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        if (GUILayout.Button("SetFristFrameToRawImage"))
                        {
                            RawImage rawImage = m_proTarget.GetComponent<RawImage>();
                            rawImage.texture = tex;

                            float m_w = 1f / m_proTarget.GridU;
                            float m_h = 1f / m_proTarget.GridV;

                            rawImage.uvRect = new Rect(
                                                        (0 % m_proTarget.GridU) * m_w,
                                                        1f - Mathf.Floor(0 / m_proTarget.GridU) * m_h - m_h,
                                                        m_w,
                                                        m_h
                                                        );
                            rawImage.SetAllDirty();
                        }

                        if (GUILayout.Button("SetNativeSize"))
                        {
                            m_proTarget.SetNativeSize();
                        }

                        GUILayout.Space(5);
                    }
                    GUILayout.EndVertical();
                }

                GUILayout.Space(12);

                if (Application.isPlaying)
                    draw_debugToolUI();
                else
                    draw_exportToolUI();
            }

        }

        private void draw_exportToolUI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.Label("Export Tool >>", EditorStyles.boldLabel);
                GUILayout.Space(5);
                //
                if (GUILayout.Button("转换为Animator动画数据"))
                {
                    if(EditorUtility.DisplayDialog("警告", "你确定要该动画内容生成Animator动画内容?", "确定", "取消"))
                    {
                        ToAnimatorData();
                    }
                }
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
        }

        private void ToAnimatorData()
        {

            RawImageFrameAnim rawImageFrameAnim = (RawImageFrameAnim)m_target;
            Texture2D tex = (Texture2D)rawImageFrameAnim.GetNonPublicField("m_FramesTexture");
            int frameLens = (int)rawImageFrameAnim.GetNonPublicField("m_totalFrame");

            //数据检查
            if (!tex || frameLens == 0 || rawImageFrameAnim.GridU == 0 || rawImageFrameAnim.GridV == 0 || rawImageFrameAnim.FPS == 0)
            {
                EditorUtility.DisplayDialog("提示", "参数不符合生成Animator动画要求，生成终止.", "确定");
                return;
            }

            FrameAnimPlayMode mode = rawImageFrameAnim.FrameAnimPlayMode;

            float m_w = 1f / rawImageFrameAnim.GridU;
            float m_h = 1f / rawImageFrameAnim.GridV;

            //step 1 根据内容创建.anim文件
            AnimationClip clip = new AnimationClip();
            clip.name = rawImageFrameAnim.gameObject.name;
            clip.frameRate = rawImageFrameAnim.FPS;
            AnimationCurve[] curves = new AnimationCurve[4];

            // m_UVRect.x
            curves[0] = new AnimationCurve();
            // m_UVRect.y
            curves[1] = new AnimationCurve();
            // m_UVRect.width
            curves[2] = new AnimationCurve();
            // m_UVRect.height
            curves[3] = new AnimationCurve();

            
            if (mode == FrameAnimPlayMode.PINGPONG)
            {
                clip.wrapMode = WrapMode.PingPong;

                for (int i = 0; i < (frameLens-1) * 2; i++)
                {

                    int idx = (int)Mathf.PingPong(i, frameLens - 1);

                    float x = (idx % rawImageFrameAnim.GridU) * m_w;
                    float y = 1f - Mathf.Floor(idx / rawImageFrameAnim.GridU) * m_h - m_h;

                    float time = i * 1f / rawImageFrameAnim.FPS;

                    curves[0].AddKey(time, x);
                    curves[1].AddKey(time, y);
                    curves[2].AddKey(time, m_w);
                    curves[3].AddKey(time, m_h);

                }

            }
            else
            {

                if (mode == FrameAnimPlayMode.NORMAL)
                {
                    clip.wrapMode = WrapMode.Default;
                }

                for (int i = 0; i < frameLens; i++)
                {
                    float x = (i % rawImageFrameAnim.GridU) * m_w;
                    float y = 1f - Mathf.Floor(i / rawImageFrameAnim.GridU) * m_h - m_h;

                    float time = i * 1f / rawImageFrameAnim.FPS;
                    curves[0].AddKey(time, x);
                    curves[1].AddKey(time, y);
                    curves[2].AddKey(time, m_w);
                    curves[3].AddKey(time, m_h);

                }

            }

            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < curves[i].length; k++)
                {
                    AnimationUtility.SetKeyLeftTangentMode(curves[i], k, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyRightTangentMode(curves[i], k, AnimationUtility.TangentMode.Constant);
                }
            }

            clip.SetCurve("", typeof(RawImage), "m_UVRect.x", curves[0]);
            clip.SetCurve("", typeof(RawImage), "m_UVRect.y", curves[1]);
            clip.SetCurve("", typeof(RawImage), "m_UVRect.width", curves[2]);
            clip.SetCurve("", typeof(RawImage), "m_UVRect.height", curves[3]);

            if (mode == FrameAnimPlayMode.LOOP)
            {
                AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                clipSettings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
                clip.wrapMode = WrapMode.Loop;
            }

            //保存AnimationClip
            string saveDir = AssetDatabase.GetAssetPath(m_target);
            string saveName = clip.name;
            string savePath = EditorUtility.SaveFilePanel("保存.anim文件", saveDir, saveName, "anim");
            if (string.IsNullOrEmpty(savePath))
            {
                //Error 
                return;
            }
            
            savePath = savePath.Replace(Application.dataPath, "Assets");
            AssetDatabase.CreateAsset(clip, savePath);

            //创建Animator
            Animator animator = m_target.GetComponent<Animator>();
            if (!animator) animator = m_target.gameObject.AddComponent<Animator>();

            string ctrl_savePath = savePath.Replace(".anim", ".controller");
            AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPathWithClip(ctrl_savePath, clip);
            animator.runtimeAnimatorController = (RuntimeAnimatorController)animatorController;
        
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            RawImage rawImage = rawImageFrameAnim.GetComponent<RawImage>();
            rawImage.texture = tex;

            rawImage.uvRect = new Rect(
                    (0 % rawImageFrameAnim.GridU) * m_w,
                    1f - Mathf.Floor(0 / rawImageFrameAnim.GridU) * m_h - m_h,
                    m_w, m_h
            );
            rawImageFrameAnim.enabled = false;
        }

    }
}