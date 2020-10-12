using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Graphic;
using System;

namespace Framework.PlayControl
{
    /// <summary>
    /// 玩家镜头控制器
    /// (调整不同参数可实现不同类型的玩家镜头控制)
    /// </summary>
    public class PlayCameraControl : MonoBehaviour
    {
        
        private static PlayCameraControl _activeIns;
        public static PlayCameraControl ActiveIns { get { return _activeIns; } }

        private static readonly List<PlayCameraControl> m_pvcList = new List<PlayCameraControl>();
        private static readonly Dictionary<int, PlayCameraControl> m_pvcDic = new Dictionary<int, PlayCameraControl>();

        private static void m_refreshActiveIns()
        {
            if(m_pvcList.Count > 0){
                _activeIns = m_pvcList[m_pvcList.Count - 1];
            }else {
                _activeIns = null;
            }
        }

        public static void RegistPVC(PlayCameraControl ctl)
        {
            if (!m_pvcDic.ContainsKey(ctl.ID))
            {
                m_pvcDic.Add(ctl.ID, ctl);
                m_pvcList.Add(ctl);
                m_refreshActiveIns();
            }
        }

        public static void UnregistPVC(int id)
        {
            UnregistPVC(GetPVC(id));
        }

        public static void UnregistPVC(PlayCameraControl ctl)
        {
            if (m_pvcDic.ContainsKey(ctl.ID))
            {
                m_pvcDic.Remove(ctl.ID);
                m_pvcList.Remove(ctl);
                m_refreshActiveIns();
            }
        }

        public static PlayCameraControl GetPVC(int id)
        {
            if (m_pvcDic.ContainsKey(id))
                return m_pvcDic[id];
            return null;
        }

        //Todo ... 后期将此功能移至编辑功能
        [Obsolete("以后将此功能移到编辑器")]
        public static PlayCameraControl Create(bool UseVisualCamera = false, Transform followTarget = null, GameObject target = null)
        {
            if (!target)
                target = new GameObject("PlayerCamRoot");

            GameObject rotateRoot = new GameObject("_rotateRoot");
            rotateRoot.transform.SetParent(target.transform);

            GameObject subCam = new GameObject("_subCam");
            subCam.transform.SetParent(rotateRoot.transform);
            
            Camera cam = subCam.GetComponent<Camera>();
            if (!cam) subCam.AddComponent<Camera>();

            if (UseVisualCamera)
            {
                VisualCamera vc = subCam.GetComponent<VisualCamera>();
                if (!vc) vc = subCam.AddComponent<VisualCamera>();
            }

            PlayCameraControl pcc = subCam.GetComponent<PlayCameraControl>();
            if (!pcc) pcc = subCam.AddComponent<PlayCameraControl>();

            pcc.FollowTarget = followTarget;

            return pcc;
        }

        public bool UseFixedUpdate = true;

        public Transform FollowTarget;
        public Camera Camera;
        //public bool ViewMode = true;

        //-------------------------------------------------------------

        public Vector3 FollowTargetOffset = new Vector3(0, 2, 0);

        public Vector3 CamOffset = new Vector3(0, 0, -8);
        public Vector3 CamAngleOffset = Vector3.zero;

        public bool UseRotateYlimit = true;
        [Range(-89.5f, 89.5f)]
        //public Vector2 RotateYLimit = new Vector2(0, 85);
        public float RotateYLimitMin = 0;
        [Range(-89.5f, 89.5f)]
        public float RotateYLimitMax = 85;
        public float RotatSpeed = 1f;

        //--------------------------------------------------------------

        protected bool m_isSetuped;
        
        protected Transform m_Root;
        protected Transform m_rotateRoot;
        protected Transform m_camRoot;

        private int m_id;
        public int ID { get { return m_id; } }

        protected void Awake()
        {
            //暂时屏蔽掉强制依赖组件的代码
            //PlayControlManager.Request(() =>
            //{
                m_id = GetHashCode();
            //});
        }

        protected void OnEnable()
        {
            if (!m_isSetuped)
            {

                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;

                if (!Camera) Camera = GetComponentInChildren<Camera>(true);

                if (Camera)
                {

                    m_camRoot = Camera.transform;
                    m_rotateRoot = m_camRoot.parent;
                    m_Root = m_rotateRoot.parent;

                    m_isSetuped = true;
                }
            }

            if(m_isSetuped) RegistPVC(this);
        }

        protected void OnDisable()
        {
            UnregistPVC(this);
        }

        public void onUpdate()
        {
            if (FollowTarget)
            {

                m_Root.position = FollowTarget.position + FollowTarget.rotation * FollowTargetOffset;

                if (!m_resetCRDirty && !m_Root.eulerAngles.y.Equals(FollowTarget.eulerAngles.y))
                    m_R2f = vf(m_Root.eulerAngles.y) - vf(FollowTarget.eulerAngles.y);
                else
                {
                    m_R2f = 0;
                    m_resetCRDirty = false;
                }
                m_Root.eulerAngles = new Vector3(0, FollowTarget.eulerAngles.y, 0);

                m_rotateRoot.localRotation = Quaternion.Euler(m_rotateRoot.localEulerAngles.x, m_rotateRoot.localEulerAngles.y + m_R2f, m_rotateRoot.localEulerAngles.z);

            }

            m_camRoot.localPosition = CamOffset;
            m_camRoot.localEulerAngles = CamAngleOffset;

        }

        protected Quaternion m_t_quaternion;
        protected Vector3 m_t_eulerCache;
        protected Vector3 m_msDelta;

        protected float m_R2f;
        
        public void Process(float msX, float msY)
        {

            m_t_quaternion = m_rotateRoot.localRotation;
            m_msDelta = new Vector3(msX, msY, 0) * RotatSpeed;
            m_t_eulerCache = new Vector3(vf(m_t_quaternion.eulerAngles.x) - m_msDelta.y, vf(m_t_quaternion.eulerAngles.y) + m_msDelta.x, m_t_quaternion.eulerAngles.z);

            m_t_quaternion = Quaternion.Euler(m_t_eulerCache);
            m_rotateRoot.localRotation = m_t_quaternion;

            if (UseRotateYlimit)
            {

                if (RotateYLimitMin > RotateYLimitMax) RotateYLimitMin = RotateYLimitMax;
                if (RotateYLimitMax < RotateYLimitMin) RotateYLimitMax = RotateYLimitMin;

                float ly = Mathf.Clamp(vf2(m_rotateRoot.localEulerAngles.x), RotateYLimitMin, RotateYLimitMax);

                m_rotateRoot.localEulerAngles = new Vector3(ly, m_rotateRoot.localEulerAngles.y, m_rotateRoot.localEulerAngles.z);
            }

        }

        protected bool m_resetCRDirty;

        public Quaternion ResetCamRotate()
        {
            Quaternion o = Quaternion.Euler(0, m_rotateRoot.rotation.eulerAngles.y, 0);
            m_rotateRoot.localEulerAngles = new Vector3(m_rotateRoot.localEulerAngles.x, 0, m_rotateRoot.localEulerAngles.z);
            m_resetCRDirty = true;
            return o;
        }

        private void LateUpdate()
        {
            if (UseRotateYlimit)
            {
                
            }
        }

        private float vf(float v)
        {
            return (v >= -180 && v < 0) ? 360 + v : v;
        }

        private float vf2(float v)
        {
            return (v >= 180) ? v - 360 : v;
        }

    }

}
