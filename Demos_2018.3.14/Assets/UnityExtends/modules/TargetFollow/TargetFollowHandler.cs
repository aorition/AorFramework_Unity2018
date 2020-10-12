using UnityEngine;
using System;

namespace Framework.module
{
    /// <summary>
    /// 目标跟随(追萝卜) 组件
    /// </summary>
    public class TargetFollowHandler : MonoBehaviour
    {
        /// <summary>
        /// 开始跟随
        /// </summary>
        public Action<TargetFollowHandler> OnFollowStart;
        /// <summary>
        /// 跟随结束
        /// </summary>
        public Action<TargetFollowHandler> OnFollowEnd;
        /// <summary>
        /// 跟随中每帧触发
        /// </summary>
        public Action<TargetFollowHandler> OnFollowingUpdate;

        //public bool DebugInfo = false;

        /// <summary>
        /// 是否使用FixedUpdate
        /// </summary>
        public bool UseFixedUpdate = false;

        /// <summary>
        /// 追逐目标
        /// </summary>
        public Transform Target;

        public float FollowThreshold = 0.1f;

        /// <summary>
        /// 忽略方向
        /// </summary>
        public bool IgnoreDirection = false;
        /// <summary>
        /// 方向限制
        /// </summary>
        public bool DirectionLimit = true;
        /// <summary>
        /// 方向限制系数
        /// </summary>
        public float DirectionLimitVelocity = 5f;
        /// <summary>
        /// Velocity模式
        /// </summary>
        public bool VelocityMode = false;
        /// <summary>
        /// 速度
        /// </summary>
        public float Velocity = 10f;
        /// <summary>
        /// 目标跟随系数
        /// </summary>
        public float FollowCoefficient = 0.1f;

        private Vector3 m_nDir;
        /// <summary>
        /// 目标的方向向量
        /// </summary>
        public Vector3 TargetDirection { get { return m_nDir; } }
        private float m_dis;
        /// <summary>
        /// 目标的距离
        /// </summary>
        public float TargetDistance { get { return m_dis; } }

        [SerializeField]
        private bool m_isFollowing;
        /// <summary>
        /// 是否正在跟随目标
        /// </summary>
        public bool IsFollowing { get{ return m_isFollowing; } }

        private void Update()
        {
            if (Target && !UseFixedUpdate) process(Time.deltaTime);
        }
        private void FixedUpdate()
        {
            if (Target && UseFixedUpdate) process(Time.fixedDeltaTime);
        }

        private Vector3 m_dir;
        private float m_v;
        private Vector3 m_vel;
        private Quaternion m_qt;

        private void process(float deltaTime)
        {
            m_dir = Target.position - transform.position;
            m_nDir = m_dir.normalized;
            //Direction
            if (!IgnoreDirection)
            {
                m_qt = Quaternion.LookRotation(m_nDir, Vector3.up);
                if (DirectionLimit)
                    transform.localRotation = Quaternion.RotateTowards(transform.localRotation, m_qt, DirectionLimitVelocity);
                else
                    transform.localRotation = m_qt;
            }

            //Position
            m_dis = Vector3.Distance(Target.position, transform.position);
            if (m_dis > FollowThreshold)
            {
                if (!m_isFollowing)
                {
                    m_isFollowing = true;
                    onFollowStart();
                }

                if (VelocityMode)
                {
                    m_v = Velocity * deltaTime;
                    if (m_dis < m_v)
                    {
                        m_vel = m_nDir * m_dis * FollowCoefficient;
                    }
                    else
                    {
                        if (IgnoreDirection)
                            m_vel = m_nDir * m_v;
                        else
                            m_vel = transform.forward * m_v;
                    }
                }
                else
                {
                    m_vel = m_dir * FollowCoefficient;
                }
                transform.position += m_vel;
                onFollowingUpdate();
            }
            else
            {
                if (m_isFollowing)
                {
                    m_isFollowing = false;
                    onFollowEnd();
                }
            }
        }

        protected virtual void onFollowStart()
        {
            //if (DebugInfo) Debug.Log("TargetFollowHandler.onFollowStart().");
            if (OnFollowStart != null) OnFollowStart(this);
        }
        protected virtual void onFollowEnd()
        {
            //if (DebugInfo) Debug.Log("TargetFollowHandler.onFollowEnd().");
            if (OnFollowEnd != null) OnFollowEnd(this);
        }
        protected virtual void onFollowingUpdate()
        {
            //if(DebugInfo) Debug.Log("TargetFollowHandler.onFollowingUpdate().");
            if (OnFollowingUpdate != null) OnFollowingUpdate(this);
        }

        protected virtual void OnDestroy()
        {
            Target = null;
            OnFollowStart = null;
            OnFollowingUpdate = null;
            OnFollowEnd = null;
        }

    }

}