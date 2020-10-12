using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AorBaseUtility;
using System;

namespace Framework.PlayControl
{
    /// <summary>
    /// 角色基础数据
    /// </summary>
    public class ActorDataBase
    {

        public ActorDataBase()
        {
            m_uuid = this.GetHashCode();
        }

        protected int m_uuid;
        public int uuid { get { return m_uuid; } }

        protected Vector3f m_scale = Vector3f.One;
        public Vector3 GetScale()
        {
            return new Vector3(m_scale.x, m_scale.y, m_scale.z);
        }
        public void SetSacle(float x, float y, float z)
        {
            m_scale = new Vector3f(x, y, z);
        }
        public void SetSacle(Vector3 v3)
        {
            SetSacle(v3.x, v3.y, v3.z);
        }

        protected aQuaternion m_rotation = aQuaternion.Identity;
        public Quaternion GetRotation()
        {
            return new Quaternion((float)m_rotation.x, (float)m_rotation.y, (float)m_rotation.z, (float)m_rotation.w);
        }
        public void SetRotation(float x, float y, float z, float w)
        {
            m_rotation = new aQuaternion(x, y, z, w);
        }
        public void SetRotation(Quaternion q)
        {
            SetRotation(q.x, q.y, q.z, q.w);
        }

        protected Vector3f m_position = Vector3f.Zero;
        public Vector3 GetPosition()
        {
            return new Vector3(m_position.x, m_position.y, m_position.z);
        }
        public void SetPosition(float x, float y, float z)
        {
            m_position = new Vector3f(x, y, z);
        }
        public void SetPosition(Vector3 v3)
        {
            SetPosition(v3.x, v3.y, v3.z);
        }
    }
}
