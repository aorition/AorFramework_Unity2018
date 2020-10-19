#pragma warning disable
using System;
using System.Collections.Generic;
using Framework.Editor;
using Framework.Editor.Utility;
using Framework.Extends;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Utility.Editor
{

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Animator), true)]
    public class AnimatorRuntimeDevTool :DecoratorEditor
    //public class AnimatorRuntimeDevTool :UnityEditor.Editor
    {

        public AnimatorRuntimeDevTool() : base("AnimatorInspector") { }


        private Animator _target;

        protected override void Awake()
        {
            base.Awake();
            _target = target as Animator;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(Application.isPlaying && _target.runtimeAnimatorController)
            {
                GUILayout.Space(5);
                _draw_RuntimeDevToolUI();

                this.Repaint();
            }
        }

        private void OnDestroy()
        {
            EditorApplication.update -= updateAnim;
        }

        private readonly List<List<string>> _stateNameByLayer = new List<List<string>>();
        private readonly List<List<int>> _stateHashBylayer = new List<List<int>>();

        private readonly List<int> _stateIndexBylayer = new List<int>();
        //private int _animatorStateIndex = 0;

        private bool m_isInitStates;

        private int _layerSelectIndex = 0;
        private int _layerCounts = 1;


        private float _startTime;
        private float _corssTime;
        private float _corssOffset;
        private float _speed = 1f;

        private bool _isPausing;

        private void _draw_RuntimeDevToolUI()
        {

            GUILayout.BeginVertical("box");
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Runtime动作测试工具");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                //获取state

                if(!m_isInitStates)
                {
                    initStates();
                    m_isInitStates = true;
                }
                else
                    updateStates();

                //绘制State GUI
                if(_layerCounts > 1)
                {
                    _layerSelectIndex = EditorGUILayout.IntSlider("层", _layerSelectIndex, 0, _layerCounts - 1);
                }
                _stateIndexBylayer[_layerSelectIndex] = EditorGUILayout.Popup("动作列表", _stateIndexBylayer[_layerSelectIndex], _stateNameByLayer[_layerSelectIndex].ToArray());

                if(_isPausing)
                    _speed = EditorGUILayout.FloatField("播放速率", _speed);
                else
                {
                    float nSpeed = EditorGUILayout.FloatField("播放速率", _speed);
                    if(!nSpeed.Equals(_speed))
                    {
                        _speed = nSpeed < 0 ? 0 : nSpeed;
                        _target.speed = _speed;
                    }
                }

                float nCorssTime = EditorGUILayout.FloatField("过渡时间", _corssTime);
                if(!nCorssTime.Equals(_corssTime))
                {
                    _corssTime = nCorssTime < 0 ? 0 : nCorssTime;
                }
                if(_corssTime > 0)
                {
                    _corssOffset = EditorGUILayout.FloatField("过渡偏移", _corssOffset);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if(GUILayout.Button("重置过渡参数"))
                        {
                            _corssTime = 0;
                            _corssOffset = 0;
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.Space(5);

                if(_isPausing)
                {
                    float nNt = EditorGUILayout.Slider("NormalizedTime", _startTime, 0f, 1f);
                    if(!nNt.Equals(_startTime))
                    {
                        _startTime = nNt;
                        pauseState_runtime(_stateHashBylayer[_layerSelectIndex][_stateIndexBylayer[_layerSelectIndex]], _layerSelectIndex, _startTime);
                    }
                }
                else
                    EditorGUILayout.LabelField("NormalizedTime", _startTime.ToString());

                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                {
                    if(GUILayout.Button(new GUIContent("播放"), GUILayout.Height(42)))
                    {
                        if(_corssTime > 0)
                        {
                            rossFadeState_runtime(_stateHashBylayer[_layerSelectIndex][_stateIndexBylayer[_layerSelectIndex]], _layerSelectIndex);
                        }
                        else
                        {
                            playState_runtime(_stateHashBylayer[_layerSelectIndex][_stateIndexBylayer[_layerSelectIndex]], _layerSelectIndex);
                        }

                    }
                    if(GUILayout.Button(new GUIContent("帧停"), GUILayout.Height(42)))
                    {
                        pauseState_runtime(_stateHashBylayer[_layerSelectIndex][_stateIndexBylayer[_layerSelectIndex]], _layerSelectIndex, _startTime);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private void initStates()
        {

            _stateNameByLayer.Clear();
            _stateHashBylayer.Clear();

            AnimatorController animatorController = _target.runtimeAnimatorController as AnimatorController;
            if(!animatorController)
            {
                AnimatorOverrideController animatorOverrideController = _target.runtimeAnimatorController as AnimatorOverrideController;
                animatorController = animatorOverrideController.runtimeAnimatorController as AnimatorController;
            }
            if(animatorController)
            {

                int layer, layerLen = animatorController.layers.Length;
                for(layer = 0; layer < layerLen; layer++)
                {
                    AnimatorStateInfo info = _target.GetCurrentAnimatorStateInfo(layer);
                    int stateIndex = 0;
                    List<string> stateNameList = new List<string>();
                    List<int> stateHashList = new List<int>();
                    AnimatorStateMachine stateMachine = animatorController.layers[layer].stateMachine;
                    if(stateMachine.states != null)
                    {

                        int s, slen = stateMachine.states.Length;
                        for(s = 0; s < slen; s++)
                        {
                            AnimatorState state = stateMachine.states[s].state;
                            if(state)
                            {
                                if(info.shortNameHash == state.nameHash)
                                {
                                    stateIndex = stateHashList.Count;
                                }
                                stateNameList.Add(state.name);
                                stateHashList.Add(state.nameHash);
                            }
                        }
                    }
                    _stateNameByLayer.Add(stateNameList);
                    _stateHashBylayer.Add(stateHashList);
                    _stateIndexBylayer.Add(stateIndex);
                }

                _layerCounts = _stateHashBylayer.Count;
                _speed = _target.speed;

                EditorApplication.update -= updateAnim;
                EditorApplication.update += updateAnim;
            }
        }

        private void updateStates()
        {

            _stateNameByLayer.Clear();
            _stateHashBylayer.Clear();

            AnimatorController animatorController = _target.runtimeAnimatorController as AnimatorController;
            if(!animatorController)
            {
                AnimatorOverrideController animatorOverrideController = _target.runtimeAnimatorController as AnimatorOverrideController;
                animatorController = animatorOverrideController.runtimeAnimatorController as AnimatorController;
            }
            if(animatorController)
            {
                int layer, layerLen = animatorController.layers.Length;
                for(layer = 0; layer < layerLen; layer++)
                {
                    List<string> stateNameList = new List<string>();
                    List<int> stateHashList = new List<int>();
                    AnimatorStateMachine stateMachine = animatorController.layers[layer].stateMachine;
                    if(stateMachine.states != null)
                    {
                        int s, slen = stateMachine.states.Length;
                        for(s = 0; s < slen; s++)
                        {
                            AnimatorState state = stateMachine.states[s].state;
                            if(state)
                            {
                                stateNameList.Add(state.name);
                                stateHashList.Add(state.nameHash);
                            }
                        }
                    }
                    _stateNameByLayer.Add(stateNameList);
                    _stateHashBylayer.Add(stateHashList);
                }
                _layerCounts = _stateHashBylayer.Count;
            }
        }

        private void updateAnim()
        {

            if(this == null || this.target == null)
                EditorApplication.update -= updateAnim;

            if(!_isPausing)
            {
                AnimatorStateInfo info = _target.GetCurrentAnimatorStateInfo(_layerSelectIndex);
                _startTime = info.loop ? info.normalizedTime % 1.0f : info.normalizedTime;
            }
        }

        private void pauseState_runtime(int stateHash, int layerId, float t)
        {
            _target.speed = 0f;
            _target.Play(stateHash, layerId, t);
            _isPausing = true;
        }

        private void playState_runtime(int stateHash, int layerId)
        {
            _target.speed = _speed;
            if(_target.updateMode == AnimatorUpdateMode.Normal)
                _target.Play(stateHash, layerId);
            else
                _target.PlayInFixedTime(stateHash, layerId);
            _isPausing = false;
        }

        private void rossFadeState_runtime(int stateHash, int layerId)
        {
            _target.speed = _speed;
            if(_target.updateMode == AnimatorUpdateMode.Normal)
                _target.CrossFade(stateHash, _corssTime, layerId, _corssOffset, 0);
            else
                _target.CrossFadeInFixedTime(stateHash, _corssTime, layerId, _corssOffset, 0);
            _isPausing = false;
        }

    }
}
