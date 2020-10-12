using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.module
{
    [Serializable]
    public class SplineAnimPlayEndEvent : UnityEngine.Events.UnityEvent<GameObject> { }
    public class SplineAnimationLauncher : MonoBehaviour
    {
        private bool m_isStarted = false;

        private void OnEnable()
        {
            if (m_isStarted) Init();
        }
        private void Start()
        {
            m_isStarted = true;
            Init();
        }

        private void OnDestroy()
        {
            m_spline = null;
            m_easeCurve = null;
            if(OnPlayEnd != null)
            {
                OnPlayEnd.RemoveAllListeners();
                OnPlayEnd = null;
            }
            if (m_splineAnimation)
            {
                MonoBehaviour del = m_splineAnimation;
                m_splineAnimation = null;
                GameObject.Destroy(del);
            }
        }

        [SerializeField, Tooltip("绑定spline路径")]
        protected Spline m_spline = null;

        [SerializeField,Tooltip("反向动画")]
        protected bool m_reverse = false;

        [SerializeField, Tooltip("运动速率/动画时间")]
        protected float m_velocityOrTime = 1f;

        [SerializeField, Tooltip("是否为时间动画?")]
        protected bool m_isTimeAnim = true;

        [SerializeField, Tooltip("是否启用物件跟随路径改变方向?")]
        protected bool m_ApplyLineDirection = false;

        [SerializeField, Tooltip("是否锁定Up轴方向? 仅在ApplyLineDirection属性为true时生效")]
        protected bool m_lockUPDirection = false;

        [SerializeField, Tooltip("动画模式")]
        protected SplineWalkerMode m_mode = SplineWalkerMode.Once;

        [SerializeField, Tooltip("动画曲线")]
        protected AnimationCurve m_easeCurve = AnimationCurve.Linear(0, 1, 1, 1);

        [SerializeField, Tooltip("运动偏移")]
        protected Vector3 m_animaOffset = Vector3.zero;
        
        public SplineAnimPlayEndEvent OnPlayEnd = new SplineAnimPlayEndEvent();

        private SplineAnimation m_splineAnimation;

        private void Init()
        {
            if (m_spline)
            {
                if (!m_splineAnimation)
                {
                    m_splineAnimation = GetComponent<SplineAnimation>();
                    if (!m_splineAnimation) m_splineAnimation = gameObject.AddComponent<SplineAnimation>();
                }

                m_splineAnimation.Play(m_spline, m_mode, m_animaOffset, 
                    m_reverse, m_ApplyLineDirection, m_lockUPDirection, 
                    m_velocityOrTime, m_isTimeAnim, m_onPlayEnd,0, m_easeCurve);

            }
        }

        private void m_onPlayEnd(GameObject go)
        {
            if(OnPlayEnd != null) OnPlayEnd.Invoke(go);
        }

        public bool IsPlaying
        {
            get
            {
                if (m_splineAnimation) return m_splineAnimation.isPlaying;
                return false;
            }
        }

        public bool Pause
        {
            get
            {
                if (m_splineAnimation) return m_splineAnimation.isPause;
                return false;
            }
            set
            {
                if (m_splineAnimation)
                {
                    if (value)
                        m_splineAnimation.Pause();
                    else
                        m_splineAnimation.Continue();
                }
            }
        }

        public void Stop()
        {
            if (m_splineAnimation) m_splineAnimation.Stop();
        }

    }
}
