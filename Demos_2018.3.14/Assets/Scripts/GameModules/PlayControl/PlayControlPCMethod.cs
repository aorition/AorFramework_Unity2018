using System;
using UnityEngine;

namespace Framework.PlayControl
{

    public interface IPlayControlMethod
    {
        void Process(Transform m_ct, float delta);
    }

    /// <summary>
    /// PC端玩家角色控制实现（魔兽世界方式控制角色）
    /// </summary>
    public class PlayControlPCMethod : IPlayControlMethod
    {

        private bool m_tag_f;
        private bool m_tag_b;
        private bool m_tag_l;
        private bool m_tag_r;

        private bool m_tag_tl;
        private bool m_tag_tr;

        private bool m_tag_rot;
        private bool m_tag_rot2;
        private bool m_tag_msB;

        private Quaternion m_t_rotation;
        private Vector3 m_t_position;

        private float m_speed;
        private float m_speedMultiply;
        private float m_rotateSpeed;
        private float m_rotateMultiply;

        public void Process(Transform target, float delta)
        {
            // 暂时不考虑输入按键绑定
            if (Input.GetKeyDown(KeyCode.W))
                m_tag_f = true;
            else if (Input.GetKeyUp(KeyCode.W))
                m_tag_f = false;

            if (Input.GetKeyDown(KeyCode.S))
                m_tag_b = true;
            else if (Input.GetKeyUp(KeyCode.S))
                m_tag_b = false;

            if (Input.GetKeyDown(KeyCode.A))
                m_tag_l = true;
            else if (Input.GetKeyUp(KeyCode.A))
                m_tag_l = false;

            if (Input.GetKeyDown(KeyCode.D))
                m_tag_r = true;
            else if (Input.GetKeyUp(KeyCode.D))
                m_tag_r = false;

            if (Input.GetKeyDown(KeyCode.Q))
                m_tag_tl = true;
            else if (Input.GetKeyUp(KeyCode.Q))
                m_tag_tl = false;

            if (Input.GetKeyDown(KeyCode.E))
                m_tag_tr = true;
            else if (Input.GetKeyUp(KeyCode.E))
                m_tag_tr = false;

            if (Input.GetMouseButtonDown(0))
                m_tag_rot = true;
            else if (Input.GetMouseButtonUp(0))
                m_tag_rot = false;

            if (Input.GetMouseButtonDown(1))
                m_tag_rot2 = true;
            else if (Input.GetMouseButtonUp(1))
                m_tag_rot2 = false;

            //同步参数
            m_speed = PlayControlManager.Instance.Speed;
            m_speedMultiply = PlayControlManager.Instance.SpeedMultiply;
            m_rotateSpeed = PlayControlManager.Instance.RotateSpeed;
            m_rotateMultiply = PlayControlManager.Instance.RotateMultiply;

            m_t_rotation = target.rotation;
            if (PlayCameraControl.ActiveIns)
            {

                PlayCameraControl.ActiveIns.onUpdate();

                if (m_tag_rot || m_tag_rot2)
                {
                    PlayCameraControl.ActiveIns.Process(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                }

                if (m_tag_rot && m_tag_rot2)
                    m_tag_msB = true;
                else
                    m_tag_msB = false;

                if (m_tag_rot2 && PlayCameraControl.ActiveIns.FollowTarget)
                {
                    m_t_rotation = PlayCameraControl.ActiveIns.ResetCamRotate();
                }
                else
                {
                    if (m_tag_tl) m_t_rotation = Quaternion.RotateTowards(m_t_rotation, Quaternion.Euler(0, m_t_rotation.eulerAngles.y - 90, 0), m_rotateSpeed * m_rotateMultiply * delta);
                    if (m_tag_tr) m_t_rotation = Quaternion.RotateTowards(m_t_rotation, Quaternion.Euler(0, m_t_rotation.eulerAngles.y + 90, 0), m_rotateSpeed * m_rotateMultiply * delta);
                }
            }
            else
            {
                if (m_tag_tl) m_t_rotation = Quaternion.RotateTowards(m_t_rotation, Quaternion.Euler(0, m_t_rotation.eulerAngles.y - 90, 0), m_rotateSpeed * m_rotateMultiply * delta);
                if (m_tag_tr) m_t_rotation = Quaternion.RotateTowards(m_t_rotation, Quaternion.Euler(0, m_t_rotation.eulerAngles.y + 90, 0), m_rotateSpeed * m_rotateMultiply * delta);
            }

            m_t_position = target.position;
            if (m_tag_f || m_tag_msB) m_t_position += target.forward * m_speed * m_speedMultiply * delta;
            if (m_tag_b) m_t_position -= target.forward * m_speed * m_speedMultiply * delta;
            if (m_tag_r) m_t_position += target.right * m_speed * m_speedMultiply * delta;
            if (m_tag_l) m_t_position -= target.right * m_speed * m_speedMultiply * delta;

            target.rotation = m_t_rotation;
            target.position = m_t_position;
        }
    }
}
