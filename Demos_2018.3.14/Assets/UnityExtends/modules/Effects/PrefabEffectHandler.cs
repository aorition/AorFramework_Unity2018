using System;
using UnityEngine;
//using LuaInterface;

namespace Framework
{
    /// <summary>
    /// 预制体 Effect 处理器
    /// </summary>
    public class PrefabEffectHandler : MonoBehaviour
    {
        /// <summary>
        /// 生存时长 (大于0时起作用)
        /// </summary>
        public float SurviveDuration = 0;

        /// <summary>
        /// 使用检查方式判断动画是否播放完毕,注意使用此功能会使Loop动画失效,默认为关闭
        /// </summary>
        public bool CheckAnimPlayEnd = false;

        /// <summary>
        /// 忽略 TimeScale
        /// </summary>
        public bool IgnoreTimeScale = false;
        /// <summary>
        /// 使用FixedUupdate
        /// </summary>
        public bool UseFixedUpdate = false;
        /// <summary>
        /// 忽略Destroy行为
        /// </summary>
        public bool IgnoreDestroyOnPlayEnd = false;
        /// <summary>
        /// 为真OnPlayEnd自动移除对象, 否则OnPlayEnd只隐藏对象(SetActive(false))
        /// </summary>
        public bool AutoDestroy = true;

        public Action<GameObject, string> OnEffectTrigger;
        public Action<GameObject> OnEffectPlayEnd;

        private void Update()
        {
            if (!UseFixedUpdate)
            {
                if (IgnoreTimeScale)
                    process(Time.unscaledDeltaTime);
                else
                    process(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (UseFixedUpdate)
            {
                if (IgnoreTimeScale)
                    process(Time.fixedUnscaledDeltaTime);
                else
                    process(Time.fixedDeltaTime);
            }
        }

        private Animator m_animator;
        private void OnEnable()
        {
            m_animator = GetComponent<Animator>();
            m_surviveT = 0;
        }

        private void OnDestory()
        {
            OnEffectTrigger = null;
            OnEffectPlayEnd = null;
            m_animator = null;
        }

        private float m_surviveT = 0;
        protected virtual void process(float deltaTime)
        {
            if (CheckAnimPlayEnd && m_animator)
            {
                if(m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    m_playEnd();
                    return;
                }
            }

            if (SurviveDuration > 0) {
                m_surviveT += deltaTime;
                if (m_surviveT >= SurviveDuration)
                {
                    m_playEnd();
                }
            }
        }

        #region Animation 事件接口方法 (程序无需使用)
        public void EffectTrigger(string info)
        {
            if (OnEffectTrigger != null) OnEffectTrigger(gameObject, info);
        }

        public void EffectEnd()
        {
            m_playEnd();
        }
        #endregion

        private void m_playEnd()
        {
            if (OnEffectPlayEnd != null) OnEffectPlayEnd(gameObject);
            if (!IgnoreDestroyOnPlayEnd)
            {
                if (AutoDestroy) ResourcesLoadBridge.UnLoadPrefab(gameObject);
                else gameObject.SetActive(false);
            }
        }

    }
}
