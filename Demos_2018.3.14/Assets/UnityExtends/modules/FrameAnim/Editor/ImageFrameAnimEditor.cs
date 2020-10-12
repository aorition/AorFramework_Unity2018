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
    [CustomEditor(typeof(ImageFrameAnim))]
    public class ImageFrameAnimEditor : FrameAnimBaseEditor
    {

        private ImageFrameAnim m_proTarget;
        private Image m_image;
        protected override void Awake()
        {
            base.Awake();
            m_proTarget = (ImageFrameAnim)m_target;
            m_image = m_proTarget.GetComponent<Image>();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //-------------------------

            if (this.m_target.enabled)
            {

                Sprite[] m_Frames = (Sprite[])m_proTarget.GetNonPublicField("m_Frames");
                if (m_Frames != null && m_Frames.Length > 0)
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

                        }
                        GUILayout.EndVertical();

                        GUILayout.Space(5);

                        if (GUILayout.Button("SetFristFrameToImage"))
                        {
                            if (m_Frames != null && m_Frames.Length > 0)
                            {

                                m_image.sprite = m_Frames[0];
                                m_image.SetAllDirty();
                            }
                        }

                        if (GUILayout.Button("SetNativeSize"))
                        {
                            m_image.SetNativeSize();
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
                    if (EditorUtility.DisplayDialog("警告", "你确定要该动画内容生成Animator动画内容?", "确定", "取消"))
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

            ImageFrameAnim imageFrameAnim = (ImageFrameAnim)m_target;

            //todo 数据检查
            Sprite[] m_Frames = (Sprite[])imageFrameAnim.GetNonPublicField("m_Frames");
            int frameLens = 0;
            if (m_Frames != null && m_Frames.Length > 0)
            {
                frameLens = m_Frames.Length;
            }

            FrameAnimPlayMode mode = imageFrameAnim.FrameAnimPlayMode;

            //step 1 根据内容创建.anim文件
            AnimationClip clip = new AnimationClip();
            clip.name = imageFrameAnim.gameObject.name + "_anim";
            clip.frameRate = imageFrameAnim.FPS;

            EditorCurveBinding editorCurveBinding = new EditorCurveBinding();
            editorCurveBinding.type = typeof(Image);
            editorCurveBinding.path = "";
            editorCurveBinding.propertyName = "m_Sprite";
            List<ObjectReferenceKeyframe> objectReferenceList = new List<ObjectReferenceKeyframe>();

            if (imageFrameAnim.EnableSetNativeSizeByFrame)
            {
                AnimationCurve[] curves = new AnimationCurve[2];
                // m_SizeDelta.x
                curves[0] = new AnimationCurve();
                // m_SizeDelta.y
                curves[1] = new AnimationCurve();

                if (mode == FrameAnimPlayMode.PINGPONG)
                {
                    clip.wrapMode = WrapMode.PingPong;

                    for (int i = 0; i < (frameLens - 1) * 2; i++)
                    {
                        int idx = (int)Mathf.PingPong(i, frameLens - 1);
                        float time = i * 1f / imageFrameAnim.FPS;
                        Sprite s = m_Frames[idx];
                        ObjectReferenceKeyframe ork = new ObjectReferenceKeyframe();
                        ork.time = time;
                        ork.value = s;
                        objectReferenceList.Add(ork);

                        curves[0].AddKey(time, s.rect.width);
                        curves[1].AddKey(time, s.rect.height);
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
                        float time = i * 1f / imageFrameAnim.FPS;
                        Sprite s = m_Frames[i];
                        ObjectReferenceKeyframe ork = new ObjectReferenceKeyframe();
                        ork.time = time;
                        ork.value = s;
                        objectReferenceList.Add(ork);

                        curves[0].AddKey(time, s.rect.width);
                        curves[1].AddKey(time, s.rect.height);

                    }

                }

                for (int i = 0; i < 2; i++)
                {
                    for (int k = 0; k < curves[i].length; k++)
                    {
                        AnimationUtility.SetKeyLeftTangentMode(curves[i], k, AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(curves[i], k, AnimationUtility.TangentMode.Constant);
                    }
                }

                if (mode == FrameAnimPlayMode.LOOP)
                {
                    AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                    clipSettings.loopTime = true;
                    AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
                    clip.wrapMode = WrapMode.Loop;
                }

                clip.SetCurve("", typeof(RectTransform), "m_SizeDelta.x", curves[0]);
                clip.SetCurve("", typeof(RectTransform), "m_SizeDelta.y", curves[1]);

                AnimationUtility.SetObjectReferenceCurve(clip, editorCurveBinding, objectReferenceList.ToArray());

            }
            else
            {

                if (mode == FrameAnimPlayMode.PINGPONG)
                {
                    clip.wrapMode = WrapMode.PingPong;

                    for (int i = 0; i < (frameLens - 1) * 2; i++)
                    {

                        int idx = (int)Mathf.PingPong(i, frameLens - 1);
                        float time = i * 1f / imageFrameAnim.FPS;
                        Sprite s = m_Frames[idx];
                        ObjectReferenceKeyframe ork = new ObjectReferenceKeyframe();
                        ork.time = time;
                        ork.value = s;
                        objectReferenceList.Add(ork);
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
                        float time = i * 1f / imageFrameAnim.FPS;
                        Sprite s = m_Frames[i];
                        ObjectReferenceKeyframe ork = new ObjectReferenceKeyframe();
                        ork.time = time;
                        ork.value = s;
                        objectReferenceList.Add(ork);
                    }

                }

                if (mode == FrameAnimPlayMode.LOOP)
                {
                    AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                    clipSettings.loopTime = true;
                    AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
                    clip.wrapMode = WrapMode.Loop;
                }

                AnimationUtility.SetObjectReferenceCurve(clip, editorCurveBinding, objectReferenceList.ToArray());

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

            //创建Animator
            Animator animator = m_target.GetComponent<Animator>();
            if (!animator) animator = m_target.gameObject.AddComponent<Animator>();

            string ctrl_savePath = savePath.Replace(".anim", ".controller");
            AnimatorController animatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(ctrl_savePath);
            if (!animatorController) animatorController = new AnimatorController();

            //layer
            bool isNewLayer = false;
            AnimatorControllerLayer layer = null;
            foreach (var l in animatorController.layers)
            {
                if (l.name == "Base Layer") layer = l;
            }
            if (layer == null)
            {
                layer = new AnimatorControllerLayer();
                isNewLayer = true;
            }
            layer.name = "Base Layer";
            //state
            AnimatorState state = new AnimatorState();
            state.name = clip.name;
            state.motion = (Motion)clip;

            layer.stateMachine = new AnimatorStateMachine();
            layer.stateMachine.AddState(state, Vector3.one);
            layer.stateMachine.defaultState = state;

            if (isNewLayer)
            {
                animatorController.AddLayer(layer);
            }

            animator.runtimeAnimatorController = (RuntimeAnimatorController)animatorController;

            AssetDatabase.CreateAsset(clip, savePath);
            AssetDatabase.CreateAsset(animatorController, ctrl_savePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            //后续
            Image image = imageFrameAnim.GetComponent<Image>();
            image.sprite = m_Frames[0];
            imageFrameAnim.enabled = false;
        }

    }
}