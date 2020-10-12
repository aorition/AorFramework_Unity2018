using System;
using UnityEngine;

namespace Framework.PlayControl
{
    public class PlayControlManager : ManagerBase
    { 

        //========= Manager 模版 =============================
        //
        // 基于MonoBehavior的Manager类 需要遵循的约定:
        //
        // *. 采用_instance字段保存静态单例.
        // *. 非自启动Manager必须提供CreateInstance静态方法.
        // *. 提供Request静态方法.
        // *. 提供IsInit静态方法判定改Manager是否初始化
        // *. 须Awake中调用ManagerBase.VerifyUniqueOnInit验证单例唯一
        // *. 须Awake中调用ManagerBase.VerifyUniqueOnInit验证单例唯一
        //
        //=============== 基于MonoBehavior的Manager====================

        //@@@ 静态方法实现区

        private static PlayControlManager _instance;
        public static PlayControlManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 创建带有独立GameObject的Instance
        /// </summary>
        public static PlayControlManager CreateInstance(Transform parenTransform = null)
        {
            ManagerBase.CreateInstance<PlayControlManager>(ref _instance, parenTransform);
            return _instance;
        }

        /// <summary>
        /// 在目标GameObject上的创建Instance
        /// </summary>
        public static PlayControlManager CreateInstanceOnGameObject(GameObject target)
        {
            //自动初始化
            ManagerBase.CreateInstanceOnGameObject<PlayControlManager>(ref _instance, target);
            return _instance;
        }

        public static void Request(Action GraphicsManagerIniteDoSh)
        {
            //CreateInstance();
            ManagerBase.Request(ref _instance, GraphicsManagerIniteDoSh);
        }

        public static bool IsInit()
        {
            return ManagerBase.VerifyIsInit(ref _instance);
        }

        //=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        //@@@ override方法实现区

        protected override void Awake()
        {
            base.Awake();
            ManagerBase.VerifyUniqueOnAwake(ref _instance, this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ManagerBase.VerifyUniqueOnDispose(ref _instance, this);
        }

        //=======================================================================

        //@@@ Manager功能实现区

        public Transform controlTarget;
        private Transform m_ct;

        public bool UseFixedUpdate = true;

        public float Speed = 5f;
        public float SpeedMultiply = 1f;
        public float RotateSpeed = 260f;
        public float RotateMultiply = 1f;

        private void FixedUpdate()
        {
            if (!_isInit || !controlTarget) return;
            if (UseFixedUpdate) process(Time.fixedDeltaTime);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_isInit || !controlTarget) return;
            if (!UseFixedUpdate) process(Time.deltaTime);
        }



        protected virtual void targetInit()
        {
            m_ct = controlTarget;
        }
        
        /// <summary>
        /// 处理玩家控制的核心实现
        /// </summary>
        protected virtual void process(float delta)
        {

            if (m_ct != controlTarget)
                targetInit();
            if (m_tansPsMethod == null) m_tansPsMethod = new PlayControlPCMethod();
            m_tansPsMethod.Process(m_ct, delta);
        }

        private IPlayControlMethod m_tansPsMethod;
        public void SetPlayTransformProcessMethod(IPlayControlMethod method)
        {
            m_tansPsMethod = method;
        }

    }

}
