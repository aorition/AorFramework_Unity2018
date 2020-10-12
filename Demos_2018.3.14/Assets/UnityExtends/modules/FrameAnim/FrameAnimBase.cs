using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.FrameAnim
{

    public enum FrameAnimPlayMode
    {
        NORMAL, //正常
        LOOP,   //循环
        PINGPONG //乒乓
    }
    /// <summary>
    /// 逐帧动画播放器
    /// </summary>
    [AddComponentMenu("")]
    public class FrameAnimBase : MonoBehaviour
    {
        public bool UseFixedUpdate = false;
        public bool IgnoreTimeScale = false;

        public bool AutoPlayOnEnabled = true;
        public bool AutoPauseOnDisable = true;

        public float FPS = 30f;
        public FrameAnimPlayMode FrameAnimPlayMode = FrameAnimPlayMode.NORMAL;

        protected float m_playTime = 0;
        protected int m_index = 0;
        protected int m_index_cache = -1;
        protected int m_FrameLens = 0;
        protected bool m_isPlaying = false;
        protected bool m_isPaused = false;

        private int i;

        protected virtual void OnEnable()
        {
            if (AutoPlayOnEnabled) Play();
        }

        protected virtual void OnDisable()
        {
            if (AutoPauseOnDisable)
                Pause();
            else
                Stop();
        }

        private void Update()
        {
            if (!UseFixedUpdate && m_isPlaying && !m_isPaused)
            {
                float delta = IgnoreTimeScale ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime;
                process(delta);
            }
        }

        private void FixedUpdate()
        {
            if (UseFixedUpdate && m_isPlaying && !m_isPaused)
            {
                float delta = IgnoreTimeScale ? UnityEngine.Time.fixedUnscaledDeltaTime : UnityEngine.Time.fixedDeltaTime;
                process(delta);
            }
        }

        private void process(float delta)
        {

            m_playTime += delta;
            i = Mathf.FloorToInt(m_playTime * FPS);

            if (this.FrameAnimPlayMode == FrameAnimPlayMode.LOOP)
            {
                m_index = i % m_FrameLens;
            }
            else if(this.FrameAnimPlayMode == FrameAnimPlayMode.PINGPONG)
            {

                m_index = (int)Mathf.PingPong(i, m_FrameLens - 1);
            }
            else
            {
                m_index = Mathf.Clamp(i, 0, m_FrameLens - 1);
            }

            m_updateIndexCache();
        }

        private void m_updateIndexCache()
        {
            if (m_index_cache != m_index)
            {
                m_index_cache = m_index;
                onIndexChanged();
            }
        }

        /// <summary>
        /// 当Index发生改变时调用
        /// </summary>
        protected virtual void onIndexChanged()
        {
            //--- todo
        }

        //------------------------

        public float Time
        {
            get
            {
                return this.m_playTime;
            }
            set
            {
                this.m_playTime = value;
                process(0);
            }
        }

        public virtual float Length
        {
            get
            {
                return 1f / this.FPS * this.m_FrameLens;
            }
        }

        public virtual void Play(float startTime = 0)
        {
            if (m_isPaused)
            {
                m_isPaused = false;
            }
            else
            {
                m_playTime = startTime;
                process(0);
            }
            m_isPlaying = true;
        }

        public virtual void Pause()
        {
            m_isPaused = true;
        }

        public virtual void Stop()
        {
            m_isPlaying = false;
            m_isPaused = false;
            m_playTime = 0;
            m_index = 0;
            m_updateIndexCache();
        }

    }

}